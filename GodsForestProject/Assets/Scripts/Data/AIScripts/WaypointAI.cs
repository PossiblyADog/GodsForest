using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAI : MonoBehaviour //Script Manages a "waypoint" gameobject with a small colliderTrigger, when target reaches waypoint
                                        //the waypoint changes position to another random point inside its available space*
{
    private DungeonRoomGenerator.RoomData room = new DungeonRoomGenerator.RoomData(null, new Vector2Int(0, 0), 0);

    private List<Vector2Int> waypointLocations = new List<Vector2Int>();
    private Collider2D slimeCollider;
    bool roomSet = false;


    private void StartUp()
    {
        if (roomSet)
        {
            if (room.CurrentRoomFloor.Count != 0)
            {
                waypointLocations.AddRange(room.CurrentRoomFloor);
            }
        }

        int startPos = Random.Range(0, waypointLocations.Count);
        transform.position = new Vector3(waypointLocations[startPos].x + .5f, waypointLocations[startPos].y + .5f);

        slimeCollider = transform.parent.GetChild(0).GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == slimeCollider)
        {
            ChangeWaypointPosition(); 
        }
    }

    public void SetWaypointData(HashSet<Vector2Int> floor, Vector2Int center, int roomType)
    {
        room.CurrentRoomFloor = floor;
        room.CurrentRoomCenter = center;
        room.CurrentRoomType = roomType;
        roomSet = true;
        StartUp();
    }

    public void ChangeWaypointPosition()
    {
        int randomIndex = Random.Range(0, waypointLocations.Count);
        transform.position = new Vector2(waypointLocations[randomIndex].x + 1f, waypointLocations[randomIndex].y + .5f);
    }
}
