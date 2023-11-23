using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class DungeonPrefabGenerator : MonoBehaviour
{
    //Runs for each room, Creates a list of appropriate positions for decor and then randomly populates those positions.
    [SerializeField]
    private List<GameObject> decorRedFloor, decorBlueFloor, decorPurpleFloor, decorNPC, decorWalls;

    public int floorDecorPercent;
    [SerializeField]
    public List<GameObject> prefabsAlive = new List<GameObject>();

    private HashSet<Vector2Int> anchorPositions = new HashSet<Vector2Int>();

    //Near static class, called from TilemapRenderer exclusively** Decorates rooms with designated decor objects randomly
    public void PopulateFloorDecorations(IEnumerable<Vector2Int> floorPositions, int tileBaseSelector,IEnumerable<Vector2Int> corridors)
    {

        HashSet<Vector2Int> wallAnchors = DesignateWallAnchors(floorPositions, corridors);
        HashSet<Vector2Int> floorAnchors = DesignateFloorAnchors(floorPositions, corridors);

        foreach (var wallAnchor in wallAnchors)
        {
            anchorPositions.Add(wallAnchor);
        }
        foreach (var floorAnchor in floorAnchors)
        {
            anchorPositions.Add(floorAnchor);
        }
        List<GameObject> currentDecorList = new List<GameObject>();

        if (tileBaseSelector == 0)
        {
            currentDecorList.AddRange(decorRedFloor);
        }
        else if (tileBaseSelector == 1)
        {
            currentDecorList.AddRange(decorBlueFloor);
        }
        else if(tileBaseSelector == 2)
        {
            currentDecorList.AddRange(decorPurpleFloor);
        }
        else if (tileBaseSelector == 3)
        {
            currentDecorList.AddRange(decorNPC);
        }
        else if (tileBaseSelector == 4)
        {
            currentDecorList.AddRange(decorRedFloor);
        }
        else if (tileBaseSelector == 5)
        {
            currentDecorList.AddRange(decorRedFloor);
        }

        PlaceWallDecor(wallAnchors, decorWalls);       
        PlaceFloorDecor(floorAnchors, currentDecorList, floorPositions);

        currentDecorList = null;
    }

    //Places floor decor, interior decor contained in n/2> and exterior the other half. Decision based on wall proximity
    private void PlaceFloorDecor(HashSet<Vector2Int> anchors, List<GameObject> decor, IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var anchor in anchors)
        {
            bool wallCheck = IfWallAdjacent(anchor, floorPositions);

            if (!wallCheck)
            {
                int decorSelect = Random.Range(0, decor.Count/2);
                GameObject decorPiece = Instantiate(decor[decorSelect], ((Vector3Int)anchor), Quaternion.identity);
                prefabsAlive.Add(decorPiece);
            }
            else
            {
                int decorSelect = Random.Range(decor.Count / 2, decor.Count);
                GameObject decorPiece = Instantiate(decor[decorSelect], ((Vector3Int)anchor), Quaternion.identity);
                prefabsAlive.Add(decorPiece);
            }
        }
    }

    //Checks to see if given floor tile is wall adjacent, effecting the type of decor to be placed
    private bool IfWallAdjacent(Vector2Int anchor, IEnumerable<Vector2Int> floorPositions)
    {
        bool wallCheck = false;
        foreach(var direction in Walk.allDirections)
        {
            Vector2Int neighborPos = anchor + direction;
            if(!floorPositions.Contains(neighborPos))
                wallCheck = true;              
        }
        return wallCheck;
    }

    //Places wall decor prefabs on the given anchors, currently iterates randomly through entire list
    private void PlaceWallDecor(HashSet<Vector2Int> anchors, List<GameObject> decor)
    {
        foreach (var anchor in anchors)
        {
            int decorSelect = Random.Range(0, decor.Count);
            GameObject decorPiece = Instantiate(decor[decorSelect], ((Vector3Int)anchor), Quaternion.identity);
            prefabsAlive.Add(decorPiece);
        }
    }

    //Randomly add small percentage of floor tiles to be populated with decor. returns anchor points, excludes corridor path
    private HashSet<Vector2Int> DesignateFloorAnchors(IEnumerable<Vector2Int> floorPositions, IEnumerable<Vector2Int> corridors) 
    {
        HashSet<Vector2Int> anchors = new HashSet<Vector2Int>();
        

        foreach(var pos in floorPositions)
        {
            int filterLevel = Random.Range(0, 100);
           
            if(!corridors.Contains(pos) && filterLevel < floorDecorPercent)
            {
                anchors.Add(pos);
            }
        }

        return anchors;
    }

    //iterates through each floor position in the room to gather its wall positions, then sends wall positions to be searched through for suitors. returns list to be decorated
    private HashSet<Vector2Int> DesignateWallAnchors(IEnumerable<Vector2Int> floorPositions, IEnumerable<Vector2Int> corridors)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var pos in floorPositions)
        {
            foreach (var direction in Walk.directionList)
            {
                var neighborPos = pos + direction;
                if (floorPositions.Contains(neighborPos) == false)
                {
                    wallPositions.Add(neighborPos);
                }
            }
        }
        HashSet<Vector2Int> anchors = FindDecorWalls(wallPositions, floorPositions, corridors);
        return anchors;
        
    }

    //Checks to see if the wall is suitable candidate for decor, if so adds to a list at a rate of 1/5. returns list of locations to be decorated
    private HashSet<Vector2Int> FindDecorWalls(HashSet<Vector2Int> wallPositions, IEnumerable<Vector2Int> floorPositions, IEnumerable<Vector2Int> corridors)
    {
        HashSet<Vector2Int> anchors = new HashSet<Vector2Int>();
        foreach (var pos in wallPositions)
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
            int binType = Convert.ToInt32(neighborBinaryPos, 2);
            if (WallTypeAssigner.wallForDecor.Contains(binType))
            {
                int filterLevel = Random.Range(0, 100);
                if (!corridors.Contains(pos) && filterLevel > 75)
                {
                    anchors.Add(pos);
                }
            }
        }
        return anchors;
    }

    public HashSet<Vector2Int> AnchorPositions
    {
        get { return anchorPositions; }
    }
}

