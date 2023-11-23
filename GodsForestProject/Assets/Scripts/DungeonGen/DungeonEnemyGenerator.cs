using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DungeonEnemyGenerator : MonoBehaviour
{
    public static DungeonEnemyGenerator instance;

    private HashSet<DungeonRoomGenerator.RoomData> roomDataList = new HashSet<DungeonRoomGenerator.RoomData>();

    private HashSet<Vector2Int> anchorPositions = new HashSet<Vector2Int>();

    [SerializeField]
    private List<GameObject> meleeEnemyPrefabs, rangedEnemyPrefabs, utilityEnemyPrefabs, eliteEnemyPrefabs, minibossPrefabs, bossPrefabs, traps, npcStatues, spawnerList;

    [SerializeField]
    private GameObject spawner, invisMinionSpawner;

    public bool setMinionSpawners = false;

    [SerializeField]
    private List<GameObject> createdEnemies = new List<GameObject>();

    public GameObject enemyActivator;


    private DungeonRoomGenerator dungeonRoomGenerator;
    private DungeonPrefabGenerator dungeonPrefabGenerator;


    public void GenerateEnemies()
    {
        StartCoroutine(InitializeGeneration());
    }

    private IEnumerator InitializeGeneration()
    {
        yield return new WaitForSeconds(1.0f);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        if (dungeonRoomGenerator == null)
        { dungeonRoomGenerator = FindObjectOfType<DungeonRoomGenerator>(); }

        if (dungeonPrefabGenerator == null)
        { dungeonPrefabGenerator = FindObjectOfType<DungeonPrefabGenerator>(); }

        roomDataList = dungeonRoomGenerator.roomDataList;
        anchorPositions = dungeonPrefabGenerator.AnchorPositions;

        foreach (var room in roomDataList)//Ensure enemies do not spawn directly on an obstacle
        {
            List<Vector2Int> posToRemove = new List<Vector2Int>();
            foreach (var pos in room.CurrentRoomFloor)
            {
                if (anchorPositions.Contains(pos))
                { posToRemove.Add(pos); }
            }
            for (int i = 0; i < posToRemove.Count - 1; i++)
            {
                room.CurrentRoomFloor.Remove(posToRemove[i]);
            }
        }

        foreach (var room in roomDataList)
        {
            PopulateSpawners(room);
        }

        foreach (var room in roomDataList)
        {
            PopulateEnemies(room);
        }



        if (setMinionSpawners)
        {
            foreach (var room in roomDataList)
            {
                GenerateMinionSpawner(room);
            }
        }

        foreach (var room in roomDataList)
        {
            if(room.CurrentRoomType == 3)
            {
                PopulateNPCRoom(room);
            }
        }


            SetMiniBoss();
        ActivateLevelBoss();
        //SpawnBoss(dungeonRoomGenerator.bossRoomData[0], bossPrefabs[0], "AnkleLord");

    }

    private void SetMiniBoss()
    {
        if (SceneManager.GetActiveScene().name == "PettyCrime")
        {
            SpawnBoss(dungeonRoomGenerator.minibossRoomDataList[0], minibossPrefabs[0], "Huntress");
        }
        if (SceneManager.GetActiveScene().name == "MinorSin")
        {
            SpawnBoss(dungeonRoomGenerator.minibossRoomDataList[0], minibossPrefabs[0], "DwarfHorde");
        }
        if (SceneManager.GetActiveScene().name == "MajorSin")
        {
            SpawnBoss(dungeonRoomGenerator.minibossRoomDataList[0], minibossPrefabs[0], "Gatekeeper");
        }
    }

    private void GenerateMinionSpawner(DungeonRoomGenerator.RoomData room)
    {
        if (room.CurrentRoomType < 3)
        {
            var spawner = Instantiate(invisMinionSpawner, (Vector3Int)room.CurrentRoomCenter, Quaternion.identity);
            spawner.GetComponent<MinionSpawner>().SetRoom(room);
        }
    }

    public void ActivateLevelBoss()
    {
        if (SceneManager.GetActiveScene().name == "PettyCrime")
        {
            SpawnBoss(dungeonRoomGenerator.bossRoomData[0], bossPrefabs[0], "AnkleLord");
        }

        if (SceneManager.GetActiveScene().name == "MinorSin")
        {
            SpawnBoss(dungeonRoomGenerator.bossRoomData[0], bossPrefabs[0], "Necromancer");
        }

        if (SceneManager.GetActiveScene().name == "MajorSin")
        {
            SpawnBoss(dungeonRoomGenerator.bossRoomData[0], bossPrefabs[0], "FireJelly");
        }

    }

    private void PopulateNPCRoom(DungeonRoomGenerator.RoomData room)
    {
        int selectorOne = Random.Range(0, npcStatues.Count);

        var statueOne = Instantiate(npcStatues[selectorOne], new Vector3(room.CurrentRoomCenter.x - 2, room.CurrentRoomCenter.y), Quaternion.identity);
        GameObject statueTwo;
            try
            {
                statueTwo = Instantiate(npcStatues[selectorOne + 1], new Vector3(room.CurrentRoomCenter.x + 2, room.CurrentRoomCenter.y), Quaternion.identity);
            }
            catch
            {
                statueTwo = Instantiate(npcStatues[selectorOne - 1], new Vector3(room.CurrentRoomCenter.x + 2, room.CurrentRoomCenter.y), Quaternion.identity);
            }

        createdEnemies.Add(statueOne);
        createdEnemies.Add(statueTwo);

        try
        {
            if (SceneManager.GetActiveScene().name.Contains("PettyCrime"))
            {
                if (GameManager.instance.levelOneQuests.Count > 0)
                {
                    Instantiate(GameManager.instance.levelOneQuests[Random.Range(0, GameManager.instance.levelOneQuests.Count)], (Vector3Int)room.CurrentRoomCenter + new Vector3Int(0, 4), Quaternion.identity);
                }
            }

            if (SceneManager.GetActiveScene().name.Contains("MinorSin"))
            {
                if (GameManager.instance.levelTwoQuests.Count > 0)
                {
                    Instantiate(GameManager.instance.levelTwoQuests[Random.Range(0, GameManager.instance.levelTwoQuests.Count)], (Vector3Int)room.CurrentRoomCenter + new Vector3Int(0, 4), Quaternion.identity);
                }
            }

            if (SceneManager.GetActiveScene().name.Contains("MajorSin"))
            {
                if (GameManager.instance.levelThreeQuests.Count > 0)
                {
                    Instantiate(GameManager.instance.levelThreeQuests[Random.Range(0, GameManager.instance.levelThreeQuests.Count)], (Vector3Int)room.CurrentRoomCenter + new Vector3Int(0, 4), Quaternion.identity);
                }
            }
        }
        catch
        {

        }

    }

    private void PopulateSpawners(DungeonRoomGenerator.RoomData room)
    {
        if (Random.value < .15f)
        {
            if (room.CurrentRoomType != 3)
            {
                var thisSpawn = Instantiate(spawner, new Vector3(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y) + new Vector3(Random.Range(-3.500f, 3.500f), Random.Range(-3.500f, 3.500f)), Quaternion.identity);
                if (SceneManager.GetActiveScene().name.Contains("Petty"))
                {
                    thisSpawn.GetComponent<EnemySpawner>().SetSpawner(spawnerList, 7.0f, 100, room);
                }
                else if (SceneManager.GetActiveScene().name.Contains("Minor"))
                {
                    thisSpawn.GetComponent<EnemySpawner>().SetSpawner(spawnerList, 6.0f, 175, room);
                }
                else if (SceneManager.GetActiveScene().name.Contains("Major"))
                {
                    thisSpawn.GetComponent<EnemySpawner>().SetSpawner(spawnerList, 5.0f, 300, room);
                }
                //createdEnemies.Add(spawn);

            }
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        createdEnemies.Add(enemy);
    }

    private void SpawnBoss(DungeonRoomGenerator.RoomData bossRoom, GameObject desiredBoss, string bossName)
    {
        var statues = Resources.LoadAll<GameObject>("SummonStatues");

        foreach(var statue in statues)
        {
            if(statue.gameObject.name.Contains(bossName))
            {
                var summoner = Instantiate(statue, new Vector3(bossRoom.CurrentRoomCenter.x, bossRoom.CurrentRoomCenter.y), Quaternion.identity);
                summoner.GetComponent<AbstractBossSummoner>().SetStatueData(bossRoom, desiredBoss);
                createdEnemies.Add(summoner);
            }
        }
    }

    private void PopulateEnemies(DungeonRoomGenerator.RoomData room)
    {//values translate 0 = RedBrick 1 = BlueBrick, 2 = PurpleBrick, 3 = NPC, 4 = Boss, 5 = MiniBoss
        if (room.CurrentRoomType == 0)
        {
            SpawnRedRoomEnemies(room);
        }
        else if (room.CurrentRoomType == 1)
        {
            SpawnBlueRoomEnemies(room);
        }
        else if (room.CurrentRoomType == 2)
        {
            SpawnPurpleRoomEnemies(room);
        }
        else
        {

        }

        if (room.CurrentRoomType < 3)
        { SpawnTrap(room); }

        var activator = Instantiate(enemyActivator, new Vector3(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y), Quaternion.identity);
        activator.GetComponent<EnemyActivator>().DisableEnemies();
       
    }

    private void SpawnTrap(DungeonRoomGenerator.RoomData room)
    {
        if(Random.Range(0,101) %2 == 0)
        {
            if(traps.Count > 0)
            {
                int selector = Random.Range(0,traps.Count);
                var trap = Instantiate(traps[selector], transform.position, Quaternion.identity);
                trap.GetComponent<AbstractTrapBase>().PositionDecider(room);
                createdEnemies.Add(trap);
            }
        }
    }

    private void SpawnPurpleRoomEnemies(DungeonRoomGenerator.RoomData room)
    {
        int spinTheWheel = Random.Range(0, 1000);
        if (spinTheWheel < 333)
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
            }
            else
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
            }
        }
        else if (spinTheWheel < 666)
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }
            else
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }
            else
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
        }
    }



    private void SpawnBlueRoomEnemies(DungeonRoomGenerator.RoomData room)
    {
        int spinTheWheel = Random.Range(0, 1000);
        if (spinTheWheel < 500)
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
            else
            {
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
            else
            {
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
        }
    }

    private void SpawnRedRoomEnemies(DungeonRoomGenerator.RoomData room)
    {
        int spinTheWheel = Random.Range(0, 1000);
        if (spinTheWheel < 250)
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]); 
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
            else
            {
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
        }
        else if(spinTheWheel < 500)
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, eliteEnemyPrefabs[0]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
            else
            {
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
            }
        }
        else if(spinTheWheel < 750)
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }
            else
            {
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, meleeEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "MajorSin")
            {
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }
            else
            {
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, rangedEnemyPrefabs[Random.Range(0, rangedEnemyPrefabs.Count)]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
                SpawnEnemy(room, utilityEnemyPrefabs[0]);
            }

        }
    }

    private void SpawnEnemy(DungeonRoomGenerator.RoomData room, GameObject enemyToSpawn)
    {
        var enemy = Instantiate(enemyToSpawn, new Vector3(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y), Quaternion.identity);
        enemy.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
        createdEnemies.Add(enemy);
    }




    public void Clear()
    {
        foreach (var enemy in createdEnemies)
        {
            DestroyImmediate(enemy);
        }

        createdEnemies.Clear();
        Debug.Log("Happened");
        roomDataList.Clear();
        anchorPositions.Clear();
    }

    public void UpdateEnemyVolume()
    {
        for(int i = 0; i < createdEnemies.Count; i++)
        {
            if (createdEnemies[i].GetComponent<AbstractEnemyBase>() != null)
            {
                createdEnemies[i].GetComponent<AbstractEnemyBase>().UpdateEnemyVolume();
            }
        }
    }

}
