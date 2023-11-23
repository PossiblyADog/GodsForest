using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : AbstractDestructable
{
    [SerializeField]
    private List<GameObject> enemiesToSpawn = new List<GameObject>();

    private float frequency;

    [SerializeField]
    private Vector3 spawnPosition;

    [SerializeField]
    private GameObject spawnEffect;

    public Transform manualSpawnSet;

    public bool isSpawning = false; 

    [SerializeField]
    private List<Transform> spawnedEnemies = new List<Transform>();


    private DungeonRoomGenerator.RoomData room;
    public void SetSpawner(List<GameObject> setEnemies, float spawnFrequency, int spawnerHP, DungeonRoomGenerator.RoomData setRoom)
    {
        for (int i = 0; i < setEnemies.Count; i++)
        {
            enemiesToSpawn.Add(setEnemies[i]);
        }

        frequency = spawnFrequency;
        maxHP = spawnerHP;
        hp = spawnerHP;
        room = setRoom;
        deathTimer = .5f;
        transform.position = new Vector3(room.CurrentRoomCenter.x + Random.Range(-3, 3), room.CurrentRoomCenter.y + Random.Range(-3, 3));
        spawnPosition = transform.position + (new Vector3(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y) - transform.position).normalized;
    }

    private IEnumerator SpawnEnemy()
    {  
       if (spawnedEnemies.Count < 10)
       {
            if(enemiesToSpawn.Count > 1) 
            { 
                var enemy = Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)], spawnPosition, Quaternion.identity);
                var entryEffect = Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
                Destroy(entryEffect, .5f);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetStaticSpawnPos();
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
                enemy.GetComponentInChildren<AbstractEnemyBase>().IsMinion();
                if (enemy.GetComponentInChildren<LootDrop>() != null)
                { enemy.GetComponentInChildren<LootDrop>().enabled = false; }
                enemy.GetComponentInChildren<SpriteRenderer>().color = new Color(200.0f / 255.0f, 200.0f / 255.0f, 200.0f / 255.0f);
                spawnedEnemies.Add(enemy.transform);
            }
            else
            {
                var enemy = Instantiate(enemiesToSpawn[0], spawnPosition, Quaternion.identity);
                var entryEffect = Instantiate(spawnEffect, spawnPosition, Quaternion.identity);
                Destroy(entryEffect, .5f);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetStaticSpawnPos();
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
                enemy.GetComponentInChildren<AbstractEnemyBase>().IsMinion();
                enemy.GetComponentInChildren<SpriteRenderer>().color = new Color(200.0f / 255.0f, 200.0f / 255.0f, 200.0f / 255.0f);
                enemy.GetComponentInChildren<LootDrop>().enabled = false;
                spawnedEnemies.Add(enemy.transform);
            }
       }
       else
       {
            spawnedEnemies.RemoveAll(empty => empty == null);
       }

       yield return new WaitForSeconds(frequency);
       StartCoroutine(SpawnEnemy());
        
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnEnemy());
        }    
    }

    public void ManualTrigger(DungeonRoomGenerator.RoomData setRoom, int maxHP)
    {
        room = setRoom;
        isSpawning = true;
        hp = maxHP;
        deathTimer = .4f;
        frequency = Random.Range(5.0f, 7.0f);
        spawnPosition = manualSpawnSet.position;
        StartCoroutine(SpawnEnemy());


    }

}
