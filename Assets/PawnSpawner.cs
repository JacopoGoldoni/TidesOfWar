using ProjectDawn.Navigation;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using Unity.Scenes;
using UnityEngine.SceneManagement;

public class PawnSpawner : MonoBehaviour
{
    public int count = 10;
    public GameObject prefab;

    private void Start()
    {
        SpawnPawn();
    }

    private void SpawnPawn()
    {
        GameObject pawn = Instantiate(prefab); 
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(pawn, SceneManager.GetSceneByName("EntitySubScene"));
    }
}