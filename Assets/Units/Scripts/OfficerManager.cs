using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerManager : UnitManager
{

    public int RegimentNumber;

    public GameObject PawnPrefab;

    public bool SpawnPawns = true;
    public int RegimentSize = 42;
    public List<PawnManager> pawns = new List<PawnManager>();

    //Formation variables
    public Formation RegimentFormation;
    public float Range = 20f;
    public bool Loaded = true;

    //STATS
    public float Morale = 100;
    public float Stamina = 100;
    public int Ammo = 50;
    public int MaxAmmo = 50;

    //State variable
    public OfficialStates state = OfficialStates.Idle;
    public bool FireAll = true;
    public bool MultipleLineFire = false;
    private OfficerManager targetRegiment = null;

    //TIMERS
    public Timer ReloadTimer = new Timer(5f);

    //DEBUG
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        InitializeFormation();

        if(SpawnPawns)
        {
            for (int i = 0; i < RegimentSize; i++)
            {
                Vector2 v2 = GetFormationCoords(i);
                SpawnPawn(Utility.V2toV3(v2) + transform.position);
            }
        }
    }

    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<OfficerMovement>();

        m = Instantiate(UnitMaterial);

        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            m.SetColor("_Color", Color.green);
        }
        else
        {
            m.SetColor("_Color", Color.red);
        }

        ms.material = m;

        //FLAG INITIALIZE

    }
    private void InitializeFormation()
    {
        RegimentFormation = new Line(42);
    }

    // Update is called once per frame
    void Update()
    {
        //STATE MACHINE
        StateTransitions();
        StateActions();

        //UPDATE TIMER
        UpdateTimers();

        targetRegiment = EnemyInRange(Range);
    }

    private void UpdateTimers()
    {
        float t = Time.deltaTime;

        ReloadTimer.UpdateTimer(t);
    }


    public void Highlight(bool highlight)
    {
        if(highlight)
        {
            m.SetInt("_Hightlight", 1);
        }
        else
        {
            m.SetInt("_Hightlight", 0);
        }
    }

    private void SpawnPawn(Vector3 pos)
    {
        GameObject pawn = Instantiate(PawnPrefab);
        pawn.transform.position = pos;

        PawnManager pm = pawn.GetComponent<PawnManager>();

        pawns.Add(pm);
        pm.masterOfficer = this;
        pm.ID = pawns.Count - 1;
        pm.faction = faction;

        pm.name = "Regiment" + RegimentNumber.ToString() + "_" + pm.ID;

        pm.Initialize();
    }

    //FORMATION MANAGEMENT
    public void SetFormation(Formation formation)
    {
        RegimentFormation = formation;
    }
    public void SendFormation()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            pawns[i].MoveTo(
                GetFormationCoords(i) + Utility.V3toV2(transform.position),
                um.CurrentRotation()
                );
        }
    }
    public Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = RegimentFormation.GetPos(ID);

        pos2.y *= -1;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    public bool ArePawnIdle()
    {
        foreach(PawnManager pm in pawns)
        {
            if(pm.um.IsMoving())
            {
                return false;
            }
            
        }
        return true;
    }
    public int GetPawnRank(int ID)
    {
        return RegimentFormation.GetRank(ID);
    }

    //FIRE MANAGEMENT
    public void SendFireMessage()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if(GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                pawns[i].CallFire();
            }
        }
        Ammo -= 1;
        Loaded = false;
    }
    public bool CheckFireStatus()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                //CHECK ONLY FIRST RANK
                if (!pawns[i].HaveFired)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //STATE MACHINE
    private void StateActions()
    {
        switch (state)
        {
            case OfficialStates.Idle:
                {
                    //SEND FORMATION TO ALL PAWNS
                    SendFormation();

                    break;
                }

            case OfficialStates.Reloading:
                {
                    //Start ReloadTimer
                    ReloadTimer.StartTimer();
                    break;
                }

            case OfficialStates.Firing:
                {
                    if(Loaded)
                    {
                        SendFireMessage();
                    }
                    break;
                }
        }
    }
    private void StateTransitions()
    {
        switch (state)
        {
            case OfficialStates.Idle:
                {
                    if (Loaded && FireAll && targetRegiment != null && ArePawnIdle() && um.IsIdle())
                    {
                        //FIRE IF ENEMY IN SIGHT AND LOADED AND FIREALL AND NOT MOVING
                        state = OfficialStates.Firing;
                        Debug.Log("Fire");
                    }
                    if (!Loaded)
                    {
                        //IF IS NOT LOADED
                        state = OfficialStates.Reloading;
                    }
                    break;
                }

            case OfficialStates.Reloading:
                {
                    if (ReloadTimer.finished)
                    {
                        //IF FINISHED RELOADING
                        state = OfficialStates.Idle;
                    }
                    break;
                }

            case OfficialStates.Firing:
                {
                    if (CheckFireStatus())
                    {
                        //IF FINISHED FIRING
                        state = OfficialStates.Idle;
                    }
                    break;
                }
        }
    }

    //SENSING
    private OfficerManager EnemyInRange(float Range)
    {
        float R2 = 5f;
        Vector3 Start = transform.position + transform.up * 1 + transform.forward * -R2;

        float d = (RegimentFormation.Lines/2 * (Range + R2)) / (2* R2);

        Vector2 a = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * d).normalized * (R2 + Range));
        Vector2 b = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * -d).normalized * (R2 + Range));

        if(faction == Factions.France && ShowSightLines)
        {
            Debug.DrawLine(Start, Start + transform.forward * (R2 + Range), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(a), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(b), Color.red, 0f, true);
        }
        OfficerManager[] Units = (OfficerManager[])FindObjectsOfType(typeof(OfficerManager));

        foreach(OfficerManager of in Units)
        {
            if(of.faction != faction)
            {
                //IF NOT SAME FACTION
                Vector2 p = Utility.V3toV2(of.transform.position - Start);


                if (faction == Factions.France && ShowSightLines)
                {
                    Debug.DrawLine(Start, Start + Utility.V2toV3(p), Color.yellow, 0f, true);
                }

                if (UtilityMath.IsInCircularSector(p, R2, Range + R2, b, a))
                {
                    //IS IN SIGHT
                    return of;
                }
            }
        }

        return null;
    }

    //GIZMOS
    public void OnDrawGizmos()
    {


        if (ShowFormation)
            FormationGizmo();
    }
    private void FormationGizmo()
    {
        for (int i = 0; i < RegimentSize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}

public enum OfficialStates
{
    Idle,
    Firing,
    Reloading
}