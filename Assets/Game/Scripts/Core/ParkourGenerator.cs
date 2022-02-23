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
    [SerializeField] [Tooltip("Parent game object of all environment objects")] private Transform environmentRoot;
    [Header("Parkour Generation")]
    [SerializeField] private Tile groundTilePrefab;
    [SerializeField] private TileData groundTileData;
    [SerializeField] private Transform rootPoint;
    [SerializeField] private int sizeX, sizeY;   
    private List<List<GameObject>> parkours = new List<List<GameObject>>();
    private List<GameObject> parkourParents = new List<GameObject>();
    [Header("Stack Tile")]
    [SerializeField] private Tile stackTilePrefab;
    [SerializeField] private TileData stackTileData;
    private List<GameObject> stackTiles = new List<GameObject>();
    private GameObject stackTilesParent;
    [Header("Stack Tile Trigger for Bridge")]
    [SerializeField] private GameObject stackTileTriggerPrefab;
    [SerializeField] private Tile bridgeVisual;
    [SerializeField] private TileData bridgeVisualData;
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
                var generatedGroundTile = PrefabUtility.InstantiatePrefab(groundTilePrefab) as Tile;                
                generatedGroundTile.transform.position = spawnPoint;
                generatedGroundTile.transform.SetParent(parkourParent.transform);
                spawnPoint.x += groundTilePrefab.transform.localScale.x;
                // set color
                generatedGroundTile.data = groundTileData;
                generatedGroundTile.Initialize();

                parkour.Add(generatedGroundTile.gameObject);
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

        var inactiveTiles = new List<GameObject>();

        foreach (var parkour in parkours)
        {
            foreach (var tile in parkour)
            {
                if(tile.activeInHierarchy == false)
                {
                    var generatedStackTile = PrefabUtility.InstantiatePrefab(stackTilePrefab) as Tile;
                    generatedStackTile.transform.position = tile.transform.position;
                    generatedStackTile.transform.SetParent(stackTilesParent.transform);
                    // set color
                    generatedStackTile.data = stackTileData;
                    generatedStackTile.Initialize();

                    stackTiles.Add(generatedStackTile.gameObject);
                    inactiveTiles.Add(tile);
                }
            }
        }
        // delete unnecessary tiles
        inactiveTiles.ForEach(x => DestroyImmediate(x));
        inactiveTiles.Clear();

        stackTilesParent.transform.SetParent(environmentRoot);
    }

    [Button]
    public void GenerateBridgeTileTriggers()
    {
        var bridgeTriggersParent = new GameObject();
        bridgeTriggersParent.name = "TileTriggerParent";

        for (int i = 0; i < count; i++)
        {
            var generatedTrigger = PrefabUtility.InstantiatePrefab(stackTileTriggerPrefab) as GameObject;           
            var spawnPoint = rootPoint.position;

            if (horizontal)
                spawnPoint.x += (stackTilePrefab.transform.localScale.x + 0.1f) * i;
            else
                spawnPoint.z += (stackTilePrefab.transform.localScale.z + 0.1f) * i;
            
            generatedTrigger.transform.position = spawnPoint;
            generatedTrigger.transform.SetParent(bridgeTriggersParent.transform);
        }

        var generatedBridgeVisual = PrefabUtility.InstantiatePrefab(bridgeVisual) as Tile;
        
        // set visual's position
        var newBridgeVisualPos = rootPoint.position;

        if (horizontal)
            newBridgeVisualPos.x += (count - 1) / 2f * 1.1f - 0.1f;
        else
            newBridgeVisualPos.z += (count - 1) / 2f * 1.1f - 0.1f;
        
        newBridgeVisualPos.y -= 0.2f;
        generatedBridgeVisual.transform.position = newBridgeVisualPos;
        // set visual's scale
        var newBridgeVisualScale = generatedBridgeVisual.transform.localScale;

        if (horizontal)
        {
            newBridgeVisualScale.x = count + 1;
            newBridgeVisualScale.z = 0.3f;
        }          
        else
        {
            newBridgeVisualScale.z = count + 1;
            newBridgeVisualScale.x = 0.3f;
        }
            
        
        generatedBridgeVisual.transform.localScale = newBridgeVisualScale;
        // set visual's color
        generatedBridgeVisual.data = bridgeVisualData;
        generatedBridgeVisual.Initialize();

        generatedBridgeVisual.transform.SetParent(bridgeTriggersParent.transform);
    }
#endif
}
