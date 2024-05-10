using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

public class Match_AI : MonoBehaviour
{
    public List<OfficerManager> controlledRegiments = new List<OfficerManager>();
    List<OfficerManager> enemyRegiments = new List<OfficerManager>();

    public List<Group> groups = new List<Group>();

    public Vector2 pos;

    public Factions faction;

    //AI EVENT BUS
    EventBinding<AIEvent> AIBinding;

    private void OnEnable()
    {
        //BIND AI BUS
        AIBinding = new EventBinding<AIEvent>(() => { return; });
        EventBus<AIEvent>.Register(AIBinding);
    }

    private void Start()
    {
        GetRegiments();

        SubdvideGroups(3);

        MoveToLine(0, new Vector2(0, 0), Quaternion.LookRotation(Vector3.right, Vector3.up));
        MoveToLine(1, new Vector2(-4, 10), Quaternion.LookRotation(Vector3.right, Vector3.up));
        MoveToLine(2, new Vector2(-8, 5), Quaternion.LookRotation(Vector3.right, Vector3.up));

        ExecuteOrder(0);
        ExecuteOrder(1);
        ExecuteOrder(2);
    }

    private void GetRegiments()
    {
        foreach(OfficerManager o in GameUtility.FindAllRegiments())
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
                g.regimentIDs[j] = controlledRegiments[i * (l/n) + j].RegimentNumber;
            }

            groups.Add(g);
        }

    }

    private OfficerManager GetNearestEnemy(OfficerManager target)
    {
        OfficerManager current = null;
        float distance = float.MaxValue;

        foreach(OfficerManager o in enemyRegiments)
        {
            float d = (target.transform.position - o.transform.position).magnitude;
            if(d < distance)
            {
                current = o;
                distance = d;
            }
        }

        return current;
    }
    private OfficerManager GetRegimentByID(int ID)
    {
        return controlledRegiments.Find(c => c.RegimentNumber == ID);
    }

    //ACTIONS
    delegate void Order(OfficerManager o);
    private void DoAll(Order or)
    {
        controlledRegiments.ForEach(o => { or(o); });
    }
    private void ExecuteOrder(int groupN)
    {
        Group group = groups[groupN];

        for( int i = 0; i < group.regimentIDs.Length; i++)
        {
            OfficerManager unit = controlledRegiments.Find(g => g.RegimentNumber == group.regimentIDs[i]);


            //SEND ORDER
            unit.SendOrder(false, group.regimentPosition[i], group.regimentRotation[i]);
        }
    }

    //ORDERS
    private void MoveToLine(int groupN, Vector2 pos, Quaternion rot)
    {
        Group group = groups[groupN];
        for (int i = 0; i < group.regimentIDs.Length; i++)
        {
            OfficerManager unit = controlledRegiments.Find(r => r.RegimentNumber == group.regimentIDs[i]);

            float space = 1f;

            Vector2 position =
                pos +
                Utility.V3toV2(rot * Vector3.right) * (((float)i - (float)(group.regimentIDs.Length - 1) * 0.5f) * (unit.RegimentFormation.Lines / 2f + space));

            group.regimentPosition[i] = position;
            group.regimentRotation[i] = rot;
        }
    }
    private void AttackNearest(OfficerManager o)
    {
        Vector2 pos;
        OfficerManager target = GetNearestEnemy(o);
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
    private void Fallback(OfficerManager o)
    {
        o.SendOrder(false, pos, Quaternion.LookRotation(Utility.V2toV3( pos - Utility.V3toV2( o.transform.position)), Vector3.up));
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