using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HuntressWaypointAI : MonoBehaviour
{
    private DungeonRoomGenerator.RoomData room = new DungeonRoomGenerator.RoomData(null, new Vector2Int(0, 0), 0);

    private List<Vector2Int> waypointLocations = new List<Vector2Int>();
    bool roomSet = false;
    public bool runningClockwise, jumping;
    int currentIndex;
    private HuntressAI parentH;



    private void Start()
    {
        if (roomSet)
        {
            if (room.CurrentRoomFloor.Count != 0)
            {
                SetWaypoints();
            }
        }
        currentIndex = 0;
        transform.position = new Vector3(waypointLocations[currentIndex].x, waypointLocations[currentIndex].y);

        parentH = transform.parent.GetChild(0).GetComponent<HuntressAI>();
        runningClockwise = true;
        jumping = false;

    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.name.Contains("Huntress"))
        {
            parentH.PerformXAttack(1);
            SetWaypointBySequence(runningClockwise);

        }
    }
    public void SetWaypointByQuadrant()
    {
        int randomIndex = currentIndex + 1;

        if (randomIndex > waypointLocations.Count - 1)
        {
            randomIndex = 0;
        }
            runningClockwise = !runningClockwise;
            currentIndex = randomIndex;
            transform.position = new Vector3(waypointLocations[currentIndex].x, waypointLocations[currentIndex].y);
        
    }

    public void SetWaypointBySequence(bool clockwise)
    {

        if (clockwise)
        {
            if (currentIndex == waypointLocations.Count - 1)
            {
                currentIndex = -1;
            }
            currentIndex++;
            transform.position = new Vector3(waypointLocations[currentIndex].x, waypointLocations[currentIndex].y);
        }
        else
        {
            if(currentIndex == 0)
            {
                currentIndex = waypointLocations.Count;
            }
            currentIndex--;
            transform.position = new Vector3(waypointLocations[currentIndex].x, waypointLocations[currentIndex].y);

        }

    }

    private void SetWaypoints()
    {

        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y + 8));
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x + 6, room.CurrentRoomCenter.y + 6));
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x + 8, room.CurrentRoomCenter.y));
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x + 6, room.CurrentRoomCenter.y - 6));
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y - 8));  
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x - 6, room.CurrentRoomCenter.y - 6));
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x - 8, room.CurrentRoomCenter.y)); 
        waypointLocations.Add(new Vector2Int(room.CurrentRoomCenter.x - 6, room.CurrentRoomCenter.y + 6)); 
    }

    public void SetWaypointData(HashSet<Vector2Int> floor, Vector2Int center, int roomType)
    {
        room.CurrentRoomFloor = floor;
        room.CurrentRoomCenter = center;
        room.CurrentRoomType = roomType;
        roomSet = true;
    }
}
