using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMatPosSet : MonoBehaviour
{

    void Start()
    {
        if(DungeonRoomGenerator.instance != null)
        {
            transform.position = new Vector3(DungeonRoomGenerator.instance.spawnRoomData[0].CurrentRoomCenter.x, DungeonRoomGenerator.instance.spawnRoomData[0].CurrentRoomCenter.y + 3);
        }
    }


}
