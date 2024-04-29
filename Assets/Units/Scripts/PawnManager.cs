using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnManager : UnitManager
{
    public Material yourMaterial;

    public OfficerManager masterOfficer;

    public int ID;

    private GameObject rifle;

    public bool HaveFired = false;

    //TIMERS
    private Timer fireTimer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<PawnMovement>();

        ms.material = yourMaterial;

        rifle = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(masterOfficer.ArePawnIdle())
        {
            TakeRifleDown();
        }
        else
        {
            TakeRifleUp();
        }

        //UPDATE TIMERS
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if(fireTimer != null)
        {
            fireTimer.UpdateTimer(Time.deltaTime);

            if (fireTimer.finished)
            {
                Fire();
            }
        }
    }

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    public void CallFire()
    {
        fireTimer = new Timer(Random.Range(0f, 2f));
    }
    private void Fire()
    {
        HaveFired = true;
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