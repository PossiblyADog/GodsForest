using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonTilemapAlgorithm
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> walkPath = new HashSet<Vector2Int>();
        walkPath.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Walk.GetRandomDirection();
            walkPath.Add(newPosition);
            previousPosition = newPosition;
        }
        return walkPath;
    }

    public static List<Vector2Int> RandomCorridor(Vector2Int startPos, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Walk.GetRandomDirection();
        var currentPos = startPos;
        corridor.Add(startPos);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPos += direction;
            corridor.Add(currentPos);
        }

        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartion(BoundsInt area, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomList = new List<BoundsInt>();
        roomQueue.Enqueue(area);
        while (roomQueue.Count > 0)
        {
            var room = roomQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < .5f)
                {
                    if (room.size.y > minHeight + minHeight / 2)
                    {
                        SplitHorizontal(minHeight, roomQueue, room);
                    }
                    else if (room.size.x > minWidth + minWidth / 2)
                    {
                        SplitVertical(minWidth, roomQueue, room);
                    }
                    else if (room.size.y >= minHeight && room.size.x >= minWidth)
                    {
                        roomList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x > minWidth + minWidth / 2)
                    {
                        SplitVertical(minHeight, roomQueue, room);
                    }
                    else if (room.size.y > minHeight + minHeight / 2)
                    {
                        SplitHorizontal(minWidth, roomQueue, room);
                    }
                    else if (room.size.y >= minHeight && room.size.x >= minWidth)
                    {
                        roomList.Add(room);
                    }
                }
            }
        }
        return roomList;
    }

    private static void SplitVertical(int minWidth, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int((room.min.x + xSplit), room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }

    private static void SplitHorizontal(int minHeight, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
          
    }
}


public static class Walk
{
    public static List<Vector2Int> directionList = new List<Vector2Int>()
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0)
        
    };

    public static List<Vector2Int> diagDirectionList = new List<Vector2Int>()
    {
        new Vector2Int(1,1),//upright
        new Vector2Int(1,-1),//rightdown
        new Vector2Int(-1,-1),//leftdown
        new Vector2Int(-1,1)//upleft
    };

    public static List<Vector2Int> allDirections = new List<Vector2Int>()
    {
        new Vector2Int(0,1),
        new Vector2Int(1,1),//upright
        new Vector2Int(1,0),
        new Vector2Int(1,-1),//rightdown
        new Vector2Int(0, -1),
        new Vector2Int(-1,-1),//leftdown
        new Vector2Int(-1,0),   
        new Vector2Int(-1,1)//upleft  
    };

    public static Vector2Int GetRandomDirection()
    {
        return directionList[Random.Range (0, directionList.Count)];
    }
}