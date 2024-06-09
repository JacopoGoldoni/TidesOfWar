using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

public class Match_AI : MonoBehaviour
{
    public List<OfficerManager> controlledRegiments = new List<OfficerManager>();
    List<OfficerManager> enemyRegiments = new List<OfficerManager>();

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
        GetRegiments();

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
        PassSensorsToWM();

        List<TacticAction> orders = WM.Plan();

        foreach (TacticAction order in orders)
        {
            ExecuteOrder(order);
        }
    }
    private void PassSensorsToWM()
    {
        WM.ClearSensors();
        //Controlled unit informations
        foreach (OfficerManager c in controlledRegiments)
        {
            int ID = controlledRegiments.IndexOf(c);
            WM.PopulateSensors(
                new FriendlySensor(ID, 0, Utility.V3toV2(c.transform.position), c.companySize)
                );
        }
        //Enemy unit informations
        foreach (OfficerManager e in enemyRegiments)
        {
            int ID = enemyRegiments.IndexOf(e);
            WM.PopulateSensors(
                new HostileSensor(ID, 0, Utility.V3toV2(e.transform.position), e.companySize)
                );
        }
    }

    private void GetRegiments()
    {
        foreach(OfficerManager o in GameUtility.GetAllCompanies())
        {
            if(o.faction == faction)
            {
                controlledRegiments.Add(o);
            }
            else
            {
                enemyRegiments.Add(o);
            }
        }
    }
    private void SubdvideGroups(int n)
    {
        int l = controlledRegiments.Count;

        for(int i = 0; i < n; i++)
        {
            Group g = new Group();
            g.regimentIDs = new int[l / n];
            g.regimentPosition = new Vector2[l / n];
            g.regimentRotation = new Quaternion[l / n];

            for (int j = 0; j < l / n; j++) 
            {
                g.regimentIDs[j] = controlledRegiments[i * (l/n) + j].companyNumber;
            }

            groups.Add(g);
        }

    }

    private OfficerManager GetRegimentByID(int ID)
    {
        return controlledRegiments.Find(c => c.companyNumber == ID);
    }

    //ACTIONS
    delegate void Order(OfficerManager o);
    private void ExecuteOrder(TacticAction order)
    {
        OfficerManager unit;

        switch (order)
        {
            case MoveTo:
                MoveTo o1 = (MoveTo)order;
                unit = controlledRegiments[o1.unitID];
                unit.SendOrder(false, o1.pos, o1.dir);
                break;

            case FaceTarget:
                FaceTarget o2 = (FaceTarget)order;
                unit = controlledRegiments[o2.unitID];
                unit.SendOrder(false, Utility.V3toV2(unit.transform.position), o2.dir);
                break;

            case AttackEnemy:
                AttackEnemy o3 = (AttackEnemy)order;
                unit = controlledRegiments[o3.unitID];
                OfficerManager target = enemyRegiments[o3.enemyID];
                Attack(unit, target);
                break;
        }
    }

    //ORDERS
    private void MoveToLine(int groupN, Vector2 pos, Quaternion rot)
    {
        Group group = groups[groupN];
        for (int i = 0; i < group.regimentIDs.Length; i++)
        {
            OfficerManager unit = controlledRegiments.Find(r => r.companyNumber == group.regimentIDs[i]);

            float space = 1f;

            Vector2 position =
                pos +
                Utility.V3toV2(rot * Vector3.right) * (((float)i - (float)(group.regimentIDs.Length - 1) * 0.5f) * (unit.companyFormation.Lines / 2f + space));

            group.regimentPosition[i] = position;
            group.regimentRotation[i] = rot;
        }
    }
    private void Attack(OfficerManager o, OfficerManager target)
    {
        Vector2 pos;
        if ((o.transform.position - target.transform.position).magnitude >= o.Range * 0.75f)
        {
            pos = Utility.V3toV2(target.transform.position + (o.transform.position - target.transform.position).normalized * o.Range * 0.75f);
        }
        else
        {
            pos = o.transform.position;
        }
        Quaternion rot = Quaternion.LookRotation((target.transform.position - o.transform.position).normalized, Vector3.up);

        o.SendOrder(false, pos, rot);
    }

}

public class AIEvent : IEvent
{
    public string name;
    public string description;
}

public class Group
{
    public int[] regimentIDs;

    public Vector2[] regimentPosition;
    public Quaternion[] regimentRotation;
}