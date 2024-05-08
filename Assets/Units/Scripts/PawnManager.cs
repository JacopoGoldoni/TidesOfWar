using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PawnManager : UnitManager
{
    public Material yourMaterial;

    public OfficerManager masterOfficer;

    public int ID;

    private GameObject rifle;

    public bool Loaded = true;
    private float fireRange = 0f;

    AudioSource audioData;
    ParticleSystem particleSystem;

    //TIMERS
    private CountdownTimer fireTimer;
    private CountdownTimer reloadTimer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<PawnMovement>();

        audioData = GetComponent<AudioSource>();
        particleSystem = GetComponentInChildren<ParticleSystem>();

        ms.material = yourMaterial;

        rifle = transform.GetChild(0).gameObject;

        fireRange = masterOfficer.Range;
    }

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    public void Update()
    {
        if(masterOfficer.RegimentFormation.name == "Column")
        {
            TakeRifleUp();
        }
        else
        {
            if (!um.IsIdle())
            {
                TakeRifleUp();
            }
            else
            {
                TakeRifleDown();
            }
        }

        if (reloadTimer != null)
        {
            reloadTimer.Tick(Time.deltaTime);
        }

        if (fireTimer != null)
        {
            fireTimer.Tick(Time.deltaTime);
        }
    }

    public void Die()
    {
        Destroy(transform.gameObject);
    }

    //FIRE
    public void CallFire()
    {
        fireTimer = new CountdownTimer(Random.Range(0f, 2f));
        fireTimer.OnTimerStop = Fire;
        fireTimer.Start();
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

        OfficerManager target = masterOfficer.targetRegiment;

        float targetWidth = target.RegimentFormation.Lines * target.RegimentFormation.a;
        Vector3 targetPos = target.transform.position;
        Vector3 targetRight = target.transform.right;
        
        float s = (float)(ID + 1) / masterOfficer.RegimentFormation.Lines;

        FireDirection = (targetPos - targetRight * (targetWidth * (s - 0.5f)) ) - transform.position;
        FireDirection.Normalize();

        float precision = masterOfficer.Precision;

        FireDirection = Quaternion.Euler(Random.Range(- precision / 2f, precision / 2f), Random.Range(- precision, precision), 0f) * FireDirection;

        Ray ray = new Ray(muzzleTransform.position, FireDirection * fireRange);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * fireRange, Color.red, 2f);
        if (Physics.Raycast(ray, out hit))
        {
            PawnManager pm = hit.transform.GetComponent<PawnManager>();
            if(pm != null)
            {
                //KILL PAWN
                pm.Die();
            }
        }
    }

    //RELOAD
    public void CallReload()
    {
        reloadTimer = new CountdownTimer(10f);
        reloadTimer.OnTimerStop = Reload;
        reloadTimer.Start();
    }
    private void Reload()
    {
        Loaded = true;
    }

    public void TakeRifleUp()
    {
        rifle.transform.localEulerAngles = new Vector3(-90,0,0);
    }
    public void TakeRifleDown()
    {
        rifle.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}