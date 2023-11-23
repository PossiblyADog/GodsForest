using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    private void Start()
    {
        if(DungeonRoomGenerator.instance != null)
        {
            transform.position = new Vector3(DungeonRoomGenerator.instance.spawnRoomData[0].CurrentRoomCenter.x, DungeonRoomGenerator.instance.spawnRoomData[0].CurrentRoomCenter.y, -1);
        }

    }
    private void LateUpdate()
    {
        if(Vector2.Distance(transform.position, PlayerController.instance.transform.position) > 1.5f)
        {
            var moveVector = Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, Time.deltaTime*5);
            transform.position = new Vector3(moveVector.x, moveVector.y, -1);
        }
    }


}
