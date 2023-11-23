using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonRoomGenerator : DungeonTilemapGenerator
{
    [SerializeField]
    private int minRoomWidth = 10, minRoomHeight = 10;
    [SerializeField]
    private int dungeonWidth = 100, dungeonHeight = 100;
    [SerializeField]
    private int minRooms = 15;

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;

    private bool goodGen = false;
    private bool specRoomExists = false;

    private bool randomRooms = true;
    public HashSet<HashSet<Vector2Int>> rooms = new HashSet<HashSet<Vector2Int>>();
    private BoundsInt roomHolder;

    public HashSet<RoomData> roomDataList = new HashSet<RoomData>();
    public List<RoomData> minibossRoomDataList = new List<RoomData>();
    public List<RoomData> bossRoomData = new List<RoomData>();
    public List<RoomData> spawnRoomData = new List<RoomData>();
    public List<RoomData> secretRoomData = new List<RoomData>();

    public static DungeonRoomGenerator instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        if(goodGen == false)
        {
            RunProceduralGeneration();
        }
    }

    protected override void RunProceduralGeneration()
    {
        goodGen = false;
        //tilemapRenderer.Clear();
        minibossRoomDataList.Clear();
        bossRoomData.Clear();
        roomDataList.Clear();
        rooms.Clear();
        CreateRooms();
        
    }

    /*public int[] DungeonSize()
    {
        return new int[2] { dungeonWidth, dungeonHeight };
    }*/

    private void CreateRooms()
    {
        var roomList = DungeonTilemapAlgorithm.BinarySpacePartion(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        if(roomList.Count >= minRooms)
        {
            goodGen = true;           
        }

        if (goodGen == true)
        {
            HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

            if (randomRooms) //also populates 'rooms' list of floor positions by room
            {
                floor = CreateRoomsRandomly(roomList);
            }
            else
            {
                floor = CreateFlatRooms(roomList);
            }


            List<Vector2Int> roomCenters = new List<Vector2Int>();

            HashSet<Vector2Int> spawnRoom = CreateSpawnRoom();
            roomCenters.Add(spawnRoomData[0].CurrentRoomCenter);
            floor.UnionWith(spawnRoom);

            foreach (var room in roomList)
            {
                roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
            }
            roomDataList = CreateRoomDataList(roomCenters, rooms);

            HashSet<Vector2Int> bossRoom = CreateBossRoom();
            tilemapRenderer.RenderFloorTiles(bossRoom, 4);
            //ConnectSingleRoom(new Vector2Int(bossHolder.min.x-1, (bossHolder.min.y + 10)), centerHolder, corridors);
            roomCenters.Add(new Vector2Int(roomHolder.min.x - 1, (roomHolder.min.y + 10)));
            roomHolder = new BoundsInt();

            HashSet<Vector2Int> miniBossRoom = CreateMiniBossRoom();
            tilemapRenderer.RenderFloorTiles(miniBossRoom, 5);
            //ConnectSingleRoom(new Vector2Int(bossHolder.max.x, (bossHolder.min.y + 10)), centerHolder, corridors);
            roomCenters.Add(new Vector2Int(roomHolder.max.x, (roomHolder.min.y + 10)));
            roomHolder = new BoundsInt();



            HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
            floor.UnionWith(corridors);
            tilemapRenderer.RenderFloorTiles(floor);
            PaintRandomRooms(roomDataList, corridors);
            floor.UnionWith(miniBossRoom);
            floor.UnionWith(bossRoom);

        

           DungeonWallGenerator.CreateWalls(floor, tilemapRenderer);
           enemyGenerator.GenerateEnemies();
        }
        //DungeonWallGenerator.CreateWalls(bossRoom, tilemapRenderer);
        //DungeonWallGenerator.CreateWalls(miniBossRoom, tilemapRenderer);
        else
        {
            RunProceduralGeneration();
        }


    }

    private void Start()
    {
        if(PlayerController.instance != null)
        {
            PlayerController.instance.transform.position = new Vector3(spawnRoomData[0].CurrentRoomCenter.x, spawnRoomData[0].CurrentRoomCenter.y);
        }   
    }


    private HashSet<RoomData> CreateRoomDataList(List<Vector2Int> roomCenters, HashSet<HashSet<Vector2Int>> roomFloors)
    {//tileBaseSelector values translate 0 = RedBrick 1 = BlueBrick, 2 = PurpleBrick, 3 = NPC, 4 = Boss, 5 = Loot, 6 = Spawn
        HashSet<RoomData> roomDataList = new HashSet<RoomData>();

        foreach (var floor in roomFloors)
        {
            foreach (var center in roomCenters)
            {
                if(floor.Contains(center))
                {
                    RoomData roomData = new RoomData(floor, center, SetRoomType());
                    
                    roomDataList.Add(roomData);
                }
            }
        }

        
        return roomDataList;
    }

    private int SetRoomType()
    {
         int selector = Random.Range(0, 1500);

        if(selector < 350)
            {
            return 0;
            }
        else if(selector < 700)
            {
            return 1;
            }
        else if(selector < 1050)
            {
            return 2;
            }
        else if(selector < 1500 && !specRoomExists)
            {
            specRoomExists = true;
            return 3;
            }
        else
        {
            return SetRoomType();
        }
        
    }
    private HashSet<Vector2Int> CreateMiniBossRoom()
    {
        BoundsInt miniBossRoom = new BoundsInt(new Vector3Int(-22, (Random.Range(10, dungeonHeight-10))), new Vector3Int(20, 20));
        roomHolder = miniBossRoom;
        HashSet <Vector2Int> miniBossFloor = CreateSquareRoom(miniBossRoom, 5);
        minibossRoomDataList.Add(new RoomData(miniBossFloor, new Vector2Int((int)miniBossRoom.center.x, (int)miniBossRoom.center.y), 5));
        return miniBossFloor;
    }

    private HashSet<Vector2Int> CreateBossRoom()
    {
        BoundsInt bossRoom = new BoundsInt(new Vector3Int(dungeonWidth + 10, (Random.Range(10, dungeonHeight-10))), new Vector3Int(20, 20));
        roomHolder = bossRoom;
        HashSet<Vector2Int> bossFloor = CreateSquareRoom(bossRoom, 4);
        bossRoomData.Add(new RoomData(bossFloor, new Vector2Int((int)bossRoom.center.x, (int)bossRoom.center.y), 4));
        return bossFloor;
    }

    private HashSet<Vector2Int> CreateSpawnRoom()
    {
        BoundsInt spawnRoom = new BoundsInt(new Vector3Int(Random.Range(10, dungeonWidth - 10), -15), new Vector3Int(10, 10));
        roomHolder = spawnRoom;
        HashSet<Vector2Int> spawnFloor = CreateSquareRoom(spawnRoom, 6);
        spawnRoomData.Add(new RoomData(spawnFloor, new Vector2Int((int)spawnRoom.center.x, (int)spawnRoom.center.y), 6));
        return spawnFloor;


    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomList)
    {
        
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomList.Count; i++)
        {
            var roomBounds = roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(walkData, roomCenter);
            
            var finalRoomFloor = new HashSet<Vector2Int>();
            foreach (var pos in roomFloor)
            {
                
                if (pos.x >= (roomBounds.xMin + offset) && pos.x <= (roomBounds.xMax - offset) && pos.y >= (roomBounds.yMin + offset) && pos.y <= (roomBounds.yMax - offset))
                {
                    finalRoomFloor.Add(pos);
                    floor.Add(pos);
                }
            }
            rooms.Add(finalRoomFloor);  
        }
        
        return floor;
    }

    private void PaintRandomRooms(HashSet<RoomData> roomDataList, IEnumerable<Vector2Int> corridors)
    {
        foreach (var roomData in roomDataList)
        {
           tilemapRenderer.RenderFloorTiles(roomData.CurrentRoomFloor, roomData.CurrentRoomType, corridors);
        }
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = spawnRoomData[0].CurrentRoomCenter;

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            if (Random.value > .2f)
            {
                roomCenters.Remove(closest);
            }
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }
    private void ConnectSingleRoom(Vector2Int anchorPoint, List<Vector2Int> roomCenters, HashSet<Vector2Int> corridors)
    {          
            Vector2Int closest = FindClosestPointTo(anchorPoint, roomCenters);
            HashSet<Vector2Int> newCorridor = CreateCorridor(anchorPoint, closest);
            corridors.UnionWith(newCorridor);
            anchorPoint = closest;
            Vector2Int nextClosest = FindClosestPointTo(anchorPoint, roomCenters);
            HashSet<Vector2Int> secondCorridor = CreateCorridor(anchorPoint, nextClosest);
            corridors.UnionWith(secondCorridor);

    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = currentRoomCenter;
        corridor.Add(pos);
        while (pos.y != destination.y)
        {
            if (pos.y < destination.y)
            {
                pos += Vector2Int.up;
            }
            else if (pos.y > destination.y)
            {
                pos += Vector2Int.down;
            }
            corridor.Add(pos);
        }


        while (pos.x != destination.x)
        {
            if (pos.x < destination.x)
            {
                pos += Vector2Int.right;
            }
            else if (pos.x > destination.x)
            {
                pos += Vector2Int.left;
            }
            corridor.Add(pos);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var pos in roomCenters)
        {
            float currentDistance = Vector2.Distance(pos, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = pos;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateFlatRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> rooms = new HashSet<Vector2Int>();
        foreach (var room in roomList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = 0; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    rooms.Add(position);
                }
            }
        }
        return rooms;
    }

    public void CreateSecretRoom(BoundsInt room)
    {
        HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                Vector2Int position = (Vector2Int)room.min + new Vector2Int(x, y);
                roomFloor.Add(position);
            }
        }

        tilemapRenderer.RenderFloorTiles(roomFloor, 3);
        DungeonWallGenerator.CreateWalls(roomFloor, tilemapRenderer);

        secretRoomData.Add(new RoomData(roomFloor, new Vector2Int((int)room.center.x, (int)room.center.y), 3));
    }

    private HashSet<Vector2Int> CreateSquareRoom(BoundsInt room, int type)
    {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

            for (int x = 0; x < room.size.x; x++)
            {
                for (int y = 0; y < room.size.y; y++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(x, y);
                    roomFloor.Add(position);
                }
            }
     
        return roomFloor;
    }

    public HashSet<RoomData> RoomDataList
    {
        get { return roomDataList; }
    }

    public struct RoomData
    {
        /*public RoomData()
        {
            CurrentRoomType = 0;
            CurrentRoomFloor = null;
            CurrentRoomCenter = new Vector2Int(0, 0);
        }*/
        public RoomData(HashSet<Vector2Int> roomFloor, Vector2Int roomCenter, int roomType)
        {
            CurrentRoomFloor = roomFloor;
            CurrentRoomCenter = roomCenter;
            CurrentRoomType = roomType;
        }

        public void ClearData()
        {
            CurrentRoomType = 0;
            CurrentRoomFloor = null;
            CurrentRoomCenter = new Vector2Int(0, 0);
        }

        public void SetRoomType(int type)
        {
            CurrentRoomType = type;
        }

        public  HashSet<Vector2Int> CurrentRoomFloor { get; set; }
        public Vector2Int CurrentRoomCenter { get; set; }
        public int CurrentRoomType { get; set; }
    }
}
