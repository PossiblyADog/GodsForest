using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    public GameObject minion;
    public float spawnInterval, timeBetweenSpawns;
    public int spawnCount;
    private bool isSpawning = false;
    private DungeonRoomGenerator.RoomData room;
 
    public void SetRoom(DungeonRoomGenerator.RoomData currentRoom)
    {
        room = currentRoom;
        timeBetweenSpawns = 0.0f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            StopAllCoroutines();
            isSpawning = true;
            StartCoroutine(SpawnMinions());
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isSpawning = false;
        }
    }
   
    IEnumerator SpawnMinions()
    {
        if (isSpawning && Time.time > timeBetweenSpawns)
        {
                timeBetweenSpawns = Time.time + spawnInterval;
            for (int i = 0; i < spawnCount; i++)
            {
                var enemy = Instantiate(minion, (Vector3Int)room.CurrentRoomCenter, Quaternion.identity);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
                enemy.GetComponentInChildren<AbstractEnemyBase>().IsMinion();
                DungeonEnemyGenerator.instance.AddEnemy(enemy);
            }
            yield return new WaitForSeconds(2.0f);


            StartCoroutine(SpawnMinions());
        }
    }
}
