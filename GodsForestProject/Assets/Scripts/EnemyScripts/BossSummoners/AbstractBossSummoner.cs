using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractBossSummoner : MonoBehaviour
{
    protected DungeonRoomGenerator.RoomData homeRoom;
    protected GameObject bossToSummon;
    protected bool canSummon;
    public void SetStatueData(DungeonRoomGenerator.RoomData room, GameObject desiredBoss)
    {
        canSummon = false;
        homeRoom = room;
        bossToSummon = desiredBoss;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            canSummon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            canSummon = false;
        }
    }

    private void Update()
    {
        if(canSummon && Input.GetKeyDown(KeyCode.F))
        {
            var spawnOffset = new Vector3(0, 4, 0);
            if (gameObject.name.Contains("Dwarf"))
            {
                spawnOffset = Vector3.zero;
            }

            var boss = Instantiate(bossToSummon, transform.position + spawnOffset, Quaternion.identity);
            if(boss.GetComponentInChildren<AbstractEnemyBase>() != null)
            {
                boss.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(homeRoom);
            }
            else
            {
                boss.GetComponent<DwarfHordeHandler>().SetRoom(homeRoom);
            }
            DungeonEnemyGenerator.instance.AddEnemy(boss);
            Destroy(gameObject);
        }
    }
}
