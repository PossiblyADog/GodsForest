using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapRenderer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    private DungeonPrefabGenerator dungeonPrefabGenerator;

    [SerializeField]
    private TileBase[] floorTiles, floorTilesBlue, floorTilesBoss, floorTilesNPC, floorTilesMiniBoss, floorTilesPurple, wallTop, wallRight, wallLeft, 
        wallBottom, wallFull, wallDownLeftInnerCorner, wallDownRightInnerCorner, wallBottomRightCorner, wallBottomLeftCorner, wallTopRightCorner, wallTopLeftCorner;

    private bool npcExists = false;


    public void RenderFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        RenderTiles(floorPositions, floorTilemap, floorTiles);
    }
    public void RenderFloorTiles(IEnumerable<Vector2Int> floorPositions, int tileBaseSelector, IEnumerable<Vector2Int> corridors)
    {
        //tileBaseSelector values translate 1 = BlueBrick, 2 = PurpleBrick, 3 = NPC, 4 = Boss, 5 = Loot
        if (tileBaseSelector == 0)
        {

        }
        else if (tileBaseSelector == 1)
        {
            RenderTiles(floorPositions, floorTilemap, floorTilesBlue);
        }
        else if (tileBaseSelector == 2)
        {
            RenderTiles(floorPositions, floorTilemap, floorTilesPurple);
        }
        else if (tileBaseSelector == 3 && npcExists == false)
        {
            RenderTiles(floorPositions, floorTilemap, floorTilesNPC);
            npcExists = true;
        }

        else
        {

        }

        if(corridors != null)
        dungeonPrefabGenerator.PopulateFloorDecorations(floorPositions, tileBaseSelector, corridors);
    }

    internal void RenderFloorTiles(HashSet<Vector2Int> bossRoom, int tileSelector)
    {
        if (tileSelector == 4)
        {
            RenderTiles(bossRoom, floorTilemap, floorTilesBoss);
        }
        else if (tileSelector == 5)
        {
            RenderTiles(bossRoom, floorTilemap, floorTilesMiniBoss);
        }
        else if(tileSelector == 3)
        {
            RenderTiles(bossRoom, floorTilemap, floorTilesNPC);
        }
    }

    internal void RenderWallSegment(Vector2Int pos, string binaryType)
    {
        int binType = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypeAssigner.wallTop.Contains(binType))
        {
            tile = wallTop[Random.Range(0, 2)];
        }
        else if (WallTypeAssigner.wallSideRight.Contains(binType))
        {
            tile = wallRight[0];
        }
        else if (WallTypeAssigner.wallSideLeft.Contains(binType))
        {
            tile = wallLeft[0];
        }
        else if (WallTypeAssigner.wallBottm.Contains(binType))
        {
            tile = wallBottom[0];
        }
        else if (WallTypeAssigner.wallFull.Contains(binType))
        {
            tile = wallFull[0];
        }

        if(tile != null)
        RenderSingleTile(wallTilemap, tile, pos);
    }

    private void RenderTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase[] tiles)
    {
        foreach (var position in positions)
        {
            if(tiles.Length > 1)
            RenderSingleTile(tilemap, tiles[Random.Range(0, (tiles.Length))], position);
            else
            RenderSingleTile(tilemap, tiles[0], position);
        }
    }

    private void RenderSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePos = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePos, tile);
    }

    public void Clear()
    {
        npcExists = false;
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
       foreach(var decor in dungeonPrefabGenerator.prefabsAlive)
        {
            DestroyImmediate(decor);
        }

    }

    internal void RenderCornerWallSegment(Vector2Int pos, string binaryType)
    {
        int binType = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypeAssigner.wallInnerCornerDownLeft.Contains(binType))
        {
            tile = wallDownLeftInnerCorner[0];
        }
        else if (WallTypeAssigner.wallInnerCornerDownRight.Contains(binType))
        {
            tile = wallDownRightInnerCorner[0];
        }
        else if (WallTypeAssigner.wallDiagonalCornerUpRight.Contains(binType))
        {
            tile = wallTopRightCorner[0];
        }
        else if (WallTypeAssigner.wallDiagonalCornerUpLeft.Contains(binType))
        {
            tile = wallTopLeftCorner[0];
        }
        else if (WallTypeAssigner.wallDiagonalCornerDownRight.Contains(binType))
        {
            tile = wallBottomRightCorner[0];
        }
        else if (WallTypeAssigner.wallDiagonalCornerDownLeft.Contains(binType))
        {
            tile = wallBottomLeftCorner[0];
        }
        else if (WallTypeAssigner.wallFullEightDirections.Contains(binType))
        {
            tile = wallFull[0];
        }

            if (tile != null)
            {
                RenderSingleTile(wallTilemap, tile, pos);
            }
        
    }
}
