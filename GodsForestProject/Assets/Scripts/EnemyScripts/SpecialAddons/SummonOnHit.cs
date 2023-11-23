using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonOnHit : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyToSpawn;

    [SerializeField]
    private Transform spawnPoint;

    DungeonRoomGenerator.RoomData room;
    int count;

    bool hasHappened = false;
    public  void SetSpawnType(DungeonRoomGenerator.RoomData currentRoom, int spawnCount)
    {
        room = currentRoom;
        count = spawnCount;
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < count; i++)
        {
            var enemy = Instantiate(enemyToSpawn, spawnPoint.position + new Vector3(Random.Range(-.10f, .10f), Random.Range(-.10f, .10f)), Quaternion.identity);
            enemy.GetComponentInChildren<AbstractEnemyBase>().SetStaticSpawnPos();
            enemy.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
            enemy.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.layer == 3 && !hasHappened)
        {            
            hasHappened = true;
            SpawnEnemies();

        }
    }
}
