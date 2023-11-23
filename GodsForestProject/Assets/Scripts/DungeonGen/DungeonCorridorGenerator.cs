using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonCorridorGenerator : DungeonTilemapGenerator
{
    [SerializeField]
    private int corLength = 14, corCount = 5;

    [SerializeField]
    [Range(0.01f, 1)]
    private float roomPercent = .8f;

    protected override void RunProceduralGeneration()
    {
       CorridorGenerator();
    }

    private void CorridorGenerator()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomsPos = new HashSet<Vector2Int>();
        CreateCorridors(floorPositions, potentialRoomsPos);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomsPos);
        List<Vector2Int> deadEnds = FindDeadEnds(floorPositions);

        FillDeadEnds(deadEnds, roomPositions);
        floorPositions.UnionWith(roomPositions);
        tilemapRenderer.RenderFloorTiles(floorPositions); 
        DungeonWallGenerator.CreateWalls(floorPositions, tilemapRenderer);
    }

    private void FillDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var pos in deadEnds)
        {
            if (!roomFloors.Contains(pos))
            {
                var room = RunRandomWalk(walkData, pos);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            int neighborCount = 0;
            foreach (var direction in Walk.directionList)
            {
                if (floorPositions.Contains(direction + pos))
                {
                    neighborCount++;
                }
            }
            if (neighborCount == 1)
            {
                deadEnds.Add(pos);
            }
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomsPos)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomCount = Mathf.RoundToInt(potentialRoomsPos.Count * roomPercent);

        List<Vector2Int> roomList = potentialRoomsPos.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();

        foreach (var roomPos in roomList)
        {
            var roomFloor = RunRandomWalk(walkData, roomPos);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomsPos)
    {
        var currentPos = startPos;
        potentialRoomsPos.Add(currentPos);
        for (int i = 0; i < corCount; i++)
        {
            var corPath = DungeonTilemapAlgorithm.RandomCorridor(currentPos, corLength);
            currentPos = corPath[corPath.Count - 1];
            potentialRoomsPos.Add(currentPos);
            floorPositions.UnionWith(corPath);
        }
    }
}
