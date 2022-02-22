using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif
using System.Linq;


public class ParkourGenerator : MonoBehaviour
{
    [Header("Parkour Generation")]
    [SerializeField] private GameObject groundTilePrefab;
    [SerializeField] private Transform rootPoint;
    [SerializeField] private int sizeX, sizeY;
    [SerializeField] [Tooltip("Empty game object for all environment objects")] private Transform environmentRoot;
    private List<List<GameObject>> parkours = new List<List<GameObject>>();
    private List<GameObject> parkourParents = new List<GameObject>();

    [Header("Stack Tile")]
    [SerializeField] private GameObject stackTilePrefab;
    private List<GameObject> stackTiles = new List<GameObject>();
    private GameObject stackTilesParent;

    [Header("Stack Tile Trigger for Bridge")]
    [SerializeField] private GameObject stackTileTriggerPrefab;
    [SerializeField] private int count = 5;
    [SerializeField] [Tooltip("Generate direction")] private bool horizontal = true;

#if UNITY_EDITOR
    [Button]
    public void GenerateParkour()
    {
        Vector3 spawnPoint = rootPoint.position;
        Vector3 alteredRootPoint = spawnPoint + Vector3.left * (((float)sizeX - 1) / 2);
        spawnPoint = alteredRootPoint;

        var parkour = new List<GameObject>();
        var parkourParent = new GameObject();
        parkourParent.name = "Parkour";
        parkourParent.isStatic = true;
        parkourParents.Add(parkourParent);

        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                var spawnedTile = PrefabUtility.InstantiatePrefab(groundTilePrefab) as GameObject;                
                spawnedTile.transform.position = spawnPoint;
                spawnedTile.transform.SetParent(parkourParent.transform);
                spawnPoint.x += groundTilePrefab.transform.localScale.x;
                parkour.Add(spawnedTile);
            }
            spawnPoint = alteredRootPoint;
            spawnPoint.z += groundTilePrefab.transform.localScale.z * (i + 1);
        }
        parkourParent.transform.SetParent(environmentRoot);
        parkours.Add(parkour);
    }

    [Button]
    public void ClearAllParkours()
    {
        // destroy parkours
        foreach (var parkour in parkours)
        {
            parkour.ForEach(x => DestroyImmediate(x));
            parkour.Clear();
        }
        parkours.Clear();

        // destroy collectable tiles
        stackTiles.ForEach(x => DestroyImmediate(x));
        stackTiles.Clear();
        DestroyImmediate(stackTilesParent);

        // destroy parkour parent objects
        parkourParents.ForEach(x => DestroyImmediate(x));
        parkourParents.Clear();
    }

    // generates collectable tiles at inactive ground tiles' positions
    [Button]
    public void AddStackTilesToAllParkours()
    {
        if(stackTiles.Any())
        {
            stackTiles.ForEach(x => DestroyImmediate(x));
            stackTiles.Clear();

            DestroyImmediate(stackTilesParent);
        }
        
        stackTilesParent = new GameObject();
        stackTilesParent.name = "Stack Tiles";
        stackTilesParent.isStatic = true;

        foreach (var parkour in parkours)
        {
            foreach (var tile in parkour)
            {
                if(tile.activeInHierarchy == false)
                {
                    var spawnedStackTile = PrefabUtility.InstantiatePrefab(stackTilePrefab) as GameObject;
                    spawnedStackTile.transform.position = tile.transform.position;
                    spawnedStackTile.transform.SetParent(stackTilesParent.transform);

                    stackTiles.Add(spawnedStackTile);

                    parkour.Remove(tile);
                    DestroyImmediate(tile);
                }
            }
        }

        stackTilesParent.transform.SetParent(environmentRoot);
    }

    [Button]
    public void GenerateBridgeTileTriggers()
    {
        var bridgeTriggersParent = new GameObject();
        bridgeTriggersParent.name = "TileTriggerParent";

        for (int i = 0; i < count; i++)
        {
            var spawnedTrigger = PrefabUtility.InstantiatePrefab(stackTileTriggerPrefab) as GameObject;
            Vector3 spawnPoint = rootPoint.position;
            if(horizontal)
            {
                spawnPoint.x += (stackTilePrefab.transform.localScale.x + 0.1f) * i;
            }
            else
            {
                spawnPoint.z += (stackTilePrefab.transform.localScale.z + 0.1f) * i;
            }
            
            spawnedTrigger.transform.position = spawnPoint;

            spawnedTrigger.transform.SetParent(bridgeTriggersParent.transform);
        }
    }
#endif
}
