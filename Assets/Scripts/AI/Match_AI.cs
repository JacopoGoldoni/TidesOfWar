using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

public class Match_AI : MonoBehaviour
{
    public List<CaptainManager> controlledBattalions = new List<CaptainManager>();
    List<CaptainManager> enemyBattalions = new List<CaptainManager>();

    public List<OfficerManager> controlledCompanies = new List<OfficerManager>();
    List<OfficerManager> enemyCompanies = new List<OfficerManager>();

    public List<Group> groups = new List<Group>();

    public Factions faction;

    //AI EVENT BUS
    EventBinding<AIEvent> AIBinding;

    WarMachine WM;

    CountdownTimer strategyTimer = new CountdownTimer(5f);

    private void OnEnable()
    {
        //BIND AI BUS
        AIBinding = new EventBinding<AIEvent>(() => { return; });
        EventBus<AIEvent>.Register(AIBinding);

        strategyTimer.OnTimerStop = () => {
            Debug.Log("Request plan calculation");
            RequestPlan();
            strategyTimer.Start();
        };
    }

    private void Start()
    {
        //INITIALIZE
        GetCompanies();
        GetBattalions();

        WM = new WarMachine();

        strategyTimer.Start();

        RequestPlan();
    }

    private void Update()
    {
        strategyTimer.Tick(Time.deltaTime);
    }

    private void RequestPlan()
    {
        List<TacticAction> orders = new List<TacticAction>();

        //TODO: request planning

        foreach (TacticAction order in orders)
        {
            ExecuteOrder(order);
        }
    }

    private void GetCompanies()
    {
        foreach(OfficerManager o in GroundBattleUtility.GetAllCompanies())
        {
            if(o.faction == faction)
            {
                controlledCompanies.Add(o);
            }
            else
            {
                enemyCompanies.Add(o);
            }
        }
    }
    private void GetBattalions()
    {
        foreach (CaptainManager o in GroundBattleUtility.GetAllBattalions())
        {
            if (o.faction == faction)
            {
                controlledBattalions.Add(o);
            }
            else
            {
                enemyBattalions.Add(o);
            }
        }
    }

    private OfficerManager GetCompanyByID(int ID)
    {
        return controlledCompanies.Find(c => c.companyNumber == ID);
    }
    private CaptainManager GetBattalionByID(int ID)
    {
        return controlledBattalions.Find(c => c.battalionNumber == ID);
    }

    //ACTIONS
    delegate void Order(OfficerManager o);
    private void ExecuteOrder(TacticAction order)
    {
        CaptainManager unit;

        switch (order)
        {
            case MoveTo:
                MoveTo o1 = (MoveTo)order;
                unit = controlledBattalions[o1.unitID];
                unit.ReceiveMovementOrder(false, o1.pos, o1.dir);
                break;

            case FaceTarget:
                FaceTarget o2 = (FaceTarget)order;
                unit = controlledBattalions[o2.unitID];
                unit.ReceiveMovementOrder(false, Utility.V3toV2(unit.transform.position), o2.dir);
                break;

            case AttackEnemy:
                AttackEnemy o3 = (AttackEnemy)order;
                unit = controlledBattalions[o3.unitID];
                CaptainManager target = controlledBattalions[o3.enemyID];
                Attack(unit, target);
                break;
            case AttackNearest:
                AttackNearest o4 = (AttackNearest)order;
                unit = controlledBattalions[o4.unitID];
                float distance = Mathf.Infinity;
                int j = 0;
                for(int i = 0; i < enemyBattalions.Count; i++)
                {
                    float d = Vector3.Distance(
                        controlledBattalions[i].gameObject.transform.position, 
                        unit.gameObject.transform.position);
                    if(d < distance)
                    {
                        distance = d;
                        j = i;
                    }
                }
                CaptainManager target_1 = controlledBattalions[j];
                Attack(unit, target_1);
                break;
        }
    }

    //ORDERS
    private void MoveToLine(int groupN, Vector2 pos, Quaternion rot)
    {
        Group group = groups[groupN];
        for (int i = 0; i < group.battalionsID.Length; i++)
        {
            OfficerManager unit = controlledCompanies.Find(r => r.companyNumber == group.battalionsID[i]);

            float space = 1f;

            Vector2 position =
                pos +
                Utility.V3toV2(rot * Vector3.right) * (((float)i - (float)(group.battalionsID.Length - 1) * 0.5f) * (unit.companyFormation.Lines / 2f + space));

            group.battalionPosition[i] = position;
            group.battalionRotation[i] = rot;
        }
    }
    private void Attack(CaptainManager c, CaptainManager target)
    {
        Vector2 pos;
        float Range = c.companies[0].Range;
        if ((c.transform.position - target.transform.position).magnitude >= Range * 0.75f)
        {
            pos = Utility.V3toV2(target.transform.position + (c.transform.position - target.transform.position).normalized * Range * 0.75f);
        }
        else
        {
            pos = c.transform.position;
        }
        Quaternion rot = Quaternion.LookRotation((target.transform.position - c.transform.position).normalized, Vector3.up);

        c.ReceiveMovementOrder(false, pos, rot);
    }

}

public class AIEvent : IEvent
{
    public string name;
    public string description;
}

public class Group
{
    public int[] battalionsID;

    public Vector2[] battalionPosition;
    public Quaternion[] battalionRotation;
}