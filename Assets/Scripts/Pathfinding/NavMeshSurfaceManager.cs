using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshSurfaceManager : MonoBehaviour
{
    NavMeshSurface navSurface;

    //SETTINGS
    public bool BakeOnStart = false;

    private void Awake()
    {
        navSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        if(BakeOnStart)
        {
            Bake();
        }
    }

    private void Bake()
    {
        if(navSurface != null)
        {
            navSurface.BuildNavMesh();
        }
    }
}
