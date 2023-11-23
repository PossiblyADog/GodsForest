using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonTilemapGenerator : AbstractDungeonGenerator
{
    protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField]
    protected RandomWalkData walkData;

    protected override void  RunProceduralGeneration()
    {   
        HashSet<Vector2Int> floorPositions = RunRandomWalk(walkData, startPosition);
        tilemapRenderer.RenderFloorTiles(floorPositions);
        DungeonWallGenerator.CreateWalls(floorPositions, tilemapRenderer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(RandomWalkData data, Vector2Int pos)
    {
        var currentPosition = pos;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < data.iterations; i++)
        {
            var walkPath = DungeonTilemapAlgorithm.RandomWalk(currentPosition, data.walkLength);
            floorPositions.UnionWith(walkPath);
            if (data.startRandomWalk)
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }
}
