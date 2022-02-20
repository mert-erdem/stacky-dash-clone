using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StackManager : Singleton<StackManager>
{
    [SerializeField] private Transform playerVisual, rootPos;
    [SerializeField] private BoxCollider stackTileCollider;
    private float DeltaPosY => stackTileCollider.size.y;
    [SerializeField] private List<GameObject> tiles;

    public void CollectTile(GameObject newTile)
    {
        var peakTile = tiles.Last().transform;

        var newTilePos = peakTile.position;
        newTilePos.y += DeltaPosY;
        newTile.transform.position = newTilePos;
        newTile.transform.SetParent(rootPos);
        tiles.Add(newTile);

        var newPlayerPos = playerVisual.position;
        newPlayerPos.y += DeltaPosY;
        playerVisual.position = newPlayerPos;       
    }
}
