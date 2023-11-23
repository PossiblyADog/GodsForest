using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfHordeHandler : MonoBehaviour
{
    private DungeonRoomGenerator.RoomData room;
    public List<GameObject> bossParts = new List<GameObject>();
    public GameObject eliteDwarfRef;
    private int lootChance = 400;
    int maxHP = 5, currentHP = 5;

    private void Initialize()
    {

        for (int i = 0; i < bossParts.Count; i++)
        {
            if (bossParts[i].GetComponent<EnemySpawner>() != null)
            {
                bossParts[i].GetComponent<EnemySpawner>().ManualTrigger(room, 250);
            }
        }



        BossHealthUpdater.instance.Switch();
        BossHealthUpdater.instance.SetBossBar("Dwarf Horde", maxHP);
    


    var cornerBottomElite = Instantiate(eliteDwarfRef, (Vector3Int)room.CurrentRoomCenter + new Vector3Int(-6, -6), Quaternion.identity);
        cornerBottomElite.GetComponentInChildren<AbstractEnemyBase>().SetStaticSpawnPos();
        cornerBottomElite.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
        cornerBottomElite.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
        bossParts.Add(cornerBottomElite);

        var cornerTopElite = Instantiate(eliteDwarfRef, (Vector3Int)room.CurrentRoomCenter + new Vector3Int(-6, 6), Quaternion.identity);
        cornerTopElite.GetComponentInChildren<AbstractEnemyBase>().SetStaticSpawnPos();
        cornerTopElite.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(room);
        cornerTopElite.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
        bossParts.Add(cornerTopElite);



        StartCoroutine(CheckParts());
    }

    IEnumerator CheckParts()
    {
        bossParts.RemoveAll(empty => empty == null);
        currentHP = bossParts.Count;
        BossHealthUpdater.instance.SetCurrentHP(currentHP);
        if (bossParts.Count == 0)
        {
            BossHealthUpdater.instance.Switch();
            for (int i = 0; i < 2; i++)
            {
                if (GetComponent<LootDrop>() != null)
                {
                    GetComponent<LootDrop>().SetDrop(lootChance);
                }
            }
            MiniBossTrigger();

            List<Collider2D> localDwarves = new List<Collider2D>();
            localDwarves.AddRange(Physics2D.OverlapBoxAll(room.CurrentRoomCenter, new Vector2(12f, 12f), 0));
            foreach (var dwarf in localDwarves)
            {
                if(dwarf.GetComponentInChildren<AbstractEnemyBase>() != null)
                {
                    dwarf.GetComponentInChildren<AbstractEnemyBase>().EnemyDeath();
                }
            }

            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(CheckParts());
        }
    }

    public void MiniBossTrigger()
    {
        DungeonEnemyGenerator.instance.ActivateLevelBoss();
    }

    public void SetRoom(DungeonRoomGenerator.RoomData bossRoom)
    {
        room = bossRoom;  
        Initialize();
    }
}
