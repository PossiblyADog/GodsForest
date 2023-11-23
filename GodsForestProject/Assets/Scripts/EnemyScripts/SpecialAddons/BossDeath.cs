using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath : MonoBehaviour
{

    public GameObject nextLevelDoor;
    public GameObject CampStatue;

    public void OnBossDeath(DungeonRoomGenerator.RoomData room)
    {
        Instantiate(nextLevelDoor, (Vector3Int)room.CurrentRoomCenter + new Vector3Int(0, 3), Quaternion.identity);
        //Instantiate(CampStatue, (Vector3Int)room.CurrentRoomCenter, Quaternion.identity);
    }
}
