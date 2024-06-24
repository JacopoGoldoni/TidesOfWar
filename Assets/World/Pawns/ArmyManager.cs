using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArmyManager : MonoBehaviour
{
    Army army;

    NavMeshAgent agent;

    [Header("Movement")]
    public Vector2 destination;
    public Vector2 direction;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        destination = Utility.V3toV2(transform.position);
        direction = Utility.V3toV2(transform.forward);

        ArmyInitialize();
    }
    private void ArmyInitialize()
    {
        army = new Army();
    }

    private void PositionArmy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 10f, -1 * transform.up, out hit, Mathf.Infinity, 1 << 8))
        {
            NavMeshHit nmh;
            NavMesh.SamplePosition(hit.point, out nmh, Mathf.Infinity, NavMesh.AllAreas);

            transform.position = nmh.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!agent.isOnNavMesh)
        {
            PositionArmy();
        }
    }

    public void MoveTo(Vector3 dest)
    {
        destination = Utility.V3toV2(dest);

        Debug.Log(dest);

        agent.destination = dest;
    }
}