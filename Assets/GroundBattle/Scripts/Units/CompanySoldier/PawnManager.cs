using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PawnManager : UnitManager
{
    public OfficerManager masterOfficer;
    public NavAgentAuthoring navAgentAuthoring;

    public int local_ID;

    private GameObject rifle;
    bool riflePose = false; //UP true

    public bool Loaded = true;
    private float fireRange = 0f;

    AudioSource audioData;
    ParticleSystem particleSystem;

    //TIMERS
    private CountdownTimer fireTimer;
    private CountdownTimer reloadTimer;

    public override void Initialize()
    {
        //INITIALIZE COMPONENTS
        um = GetComponent<PawnMovement>();
        audioData = GetComponent<AudioSource>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        navAgentAuthoring = GetComponent<NavAgentAuthoring>();

        navAgentAuthoring.moveSpeed = masterOfficer.Speed + 1f;

        InitializeMeshes();
        InitializeMaterial();

        rifle = transform.GetChild(0).gameObject;
        fireRange = masterOfficer.Range;
    }

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    public void Update()
    {
        if (masterOfficer.companyFormation.name == "Column")
        {
            SetRifle(true);
        }
        else
        {
            if (!um.IsIdle())
            {
                SetRifle(true);
            }
            else
            {
                SetRifle(false);
            }
        }

        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (reloadTimer != null)
        {
            reloadTimer.Tick(Time.deltaTime);
        }

        if (fireTimer != null)
        {
            fireTimer.Tick(Time.deltaTime);
        }
    }

    public override float GetWidth()
    {
        throw new System.NotImplementedException();
    }
    public override float GetLenght()
    {
        throw new System.NotImplementedException();
    }

    //FIRE
    public void CallFire()
    {
        fireTimer = new CountdownTimer(Random.Range(0f, 2f));
        fireTimer.OnTimerStop = Fire;
        fireTimer.Start();
    }
    public void AbortFiring()
    {
        fireTimer.Stop();
        fireTimer.Reset();
    }
    private void Fire()
    {
        //UNLOAD GUN
        Loaded = false;

        //PLAY AUDIO CLIP
        audioData.clip = SFXUtility.GetAudio("MusketFire" + Random.Range(1, 4));
        audioData.Play(0);
        
        //PLAY PARTCLE EFFECT
        particleSystem.Play();

        //TRACE FOR HIT
        Transform muzzleTransform = transform.GetChild(1).transform;

        Vector3 FireDirection = transform.forward;

        UnitManager target = masterOfficer.targetUnit;

        float targetWidth = target.GetWidth();
        Vector3 targetPos = target.transform.position;
        Vector3 targetRight = target.transform.right;
        
        float s = (float)(local_ID + 1) / masterOfficer.companyFormation.Lines;

        FireDirection = (targetPos - targetRight * (targetWidth * (s - 0.5f)) ) - transform.position;
        FireDirection.Normalize();

        float precision = masterOfficer.Precision;

        FireDirection = Quaternion.Euler(Random.Range(- precision / 2f, precision / 2f), Random.Range(- precision, precision), 0f) * FireDirection;

        Ray ray = new Ray(muzzleTransform.position, FireDirection * fireRange);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * fireRange, Color.red, 2f);
        if (Physics.Raycast(ray, out hit))
        {
            UnitManager pm = hit.transform.GetComponent<UnitManager>();
            if(pm != null && pm.Killable)
            {
                //KILL PAWN
                pm.Die();
            }
        }
    }

    //RELOAD
    public void CallReload()
    {
        reloadTimer = new CountdownTimer(masterOfficer.companyTemplate.ReloadTime);
        reloadTimer.OnTimerStop = Reload;
        reloadTimer.Start();
        SetRifle(true);
    }
    public void AbortReload()
    {
        reloadTimer.Stop();
        reloadTimer.Reset();
        SetRifle(false);
    }
    private void Reload()
    {
        Loaded = true;
        SetRifle(false);
    }

    public void SetRifle(bool newPose)
    {
        if(riflePose != newPose)
        {
            riflePose = newPose;
            if (newPose)
            {
                //UP
                rifle.transform.localEulerAngles = new Vector3(-90, 0, 0);
            }
            else
            {
                //DOWN
                rifle.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    public override void OnSelection()
    {
        throw new System.NotImplementedException();
    }
    public override void OnDeselection()
    {
        throw new System.NotImplementedException();
    }
}