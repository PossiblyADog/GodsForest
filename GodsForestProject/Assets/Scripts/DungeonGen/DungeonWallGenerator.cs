using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonWallGenerator 
{
  public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapRenderer renderer)
    {
        var basicWallPositions = FindWalls(floorPositions, Walk.directionList);
        var cornerWallPositions = FindWalls(floorPositions, Walk.diagDirectionList);
        CreateBasicWalls(renderer, basicWallPositions, floorPositions);
        CreateCornerWalls(renderer, cornerWallPositions, floorPositions);
    }

    private static void CreateCornerWalls(TilemapRenderer renderer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var pos in cornerWallPositions)
        {
            string neighborBinaryPos = "";
            foreach (var direction in Walk.allDirections)
            {
                var neighborPos = pos + direction;
                if (floorPositions.Contains(neighborPos))
                {
                    neighborBinaryPos += "1";
                }
                else
                {
                    neighborBinaryPos += "0";
                }
            }          
            renderer.RenderCornerWallSegment(pos, neighborBinaryPos);
        }
    }

    private static void CreateBasicWalls(TilemapRenderer renderer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var pos in basicWallPositions)
        {
            string neighborBinaryPos = "";
            foreach (var direction in Walk.directionList)
            {
                var neighborPos = pos + direction;
                if (floorPositions.Contains(neighborPos))
                {
                    neighborBinaryPos += "1";
                }
                else
                {
                    neighborBinaryPos += "0";
                }
            }
            renderer.RenderWallSegment(pos, neighborBinaryPos);
        }
    }

    private static HashSet<Vector2Int> FindWalls(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighborPos = pos + direction;
                if (floorPositions.Contains(neighborPos) == false)
                {
                    wallPositions.Add(neighborPos);
                }
            }
        }
        return wallPositions;
    }
}
