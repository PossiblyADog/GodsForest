using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatekeeperChildren : MonoBehaviour
{
    public List<GameObject> minionList = new List<GameObject>();
    public List<GameObject> currentMinions = new List<GameObject>();
    DungeonRoomGenerator.RoomData thisRoom = new DungeonRoomGenerator.RoomData();
    public int maxMinions;
    public void Activate(DungeonRoomGenerator.RoomData room)
    {
        thisRoom = room;
        if(maxMinions == 0)
        {
            maxMinions = 2;
        }
        StartCoroutine(GatekeeperMinionSpawn());
    }

    IEnumerator GatekeeperMinionSpawn()
    {
        yield return new WaitForSeconds(1.0f);
        currentMinions.RemoveAll(empty => empty == null);
        if (currentMinions.Count < maxMinions)
        {
            for (int i = currentMinions.Count; i < maxMinions; i++)
            {
                int selector = Random.Range(0, minionList.Count);
                var newMinion = Instantiate(minionList[selector], transform.position, Quaternion.identity);
                newMinion.GetComponentInChildren<AbstractEnemyBase>().SetRoomData(thisRoom);
                newMinion.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
                newMinion.GetComponentInChildren<AbstractEnemyBase>().IsMinion();
                currentMinions.Add(newMinion);
            }
        }
        else
        {

        }
        StartCoroutine(GatekeeperMinionSpawn());
    }

    public void TimeToDie()
    {
        StopAllCoroutines();
        if(currentMinions.Count > 0)
        {
            foreach (var minion in currentMinions)
            {
                minion.GetComponentInChildren<AbstractEnemyBase>().EnemyDeath();
            } 
        }
    }
}
