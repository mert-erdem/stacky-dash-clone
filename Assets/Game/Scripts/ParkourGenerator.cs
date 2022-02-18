using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ParkourGenerator : MonoBehaviour
{
    [SerializeField] private BoxCollider tile;
    [SerializeField] private Transform rootPoint;
    [SerializeField] private int size;
    private List<GameObject> parkours = new List<GameObject>();

#if UNITY_EDITOR
    [Button]
    public void GenerateParkour()
    {
        Vector3 spawnPoint = rootPoint.position;
        Vector3 alteredRootPoint = spawnPoint + Vector3.left * (((float)size - 1) / 2);
        spawnPoint = alteredRootPoint;

        var parkourParent = new GameObject();
        parkourParent.name = "Parkour";

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var spawnedTile = Instantiate(tile.gameObject, spawnPoint, Quaternion.identity);
                spawnedTile.transform.SetParent(parkourParent.transform);
                spawnPoint.x += tile.size.x;
            }
            spawnPoint = alteredRootPoint;
            spawnPoint.z += tile.size.z * (i+1);
        }
        parkourParent.transform.SetParent(transform);
        parkours.Add(parkourParent);
    }

    [Button]
    public void ClearAllParkours()
    {
        foreach (var parkour in parkours)
        {
            DestroyImmediate(parkour);
        }
        parkours.Clear();
    }
#endif
}
