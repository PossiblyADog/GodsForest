using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrapController : AbstractTrapBase
{
    private bool isActive = false;
    public GameObject trap1, trap2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !isActive)
        {
            isActive = true;
            trap1.GetComponent<ArrowTrapAI>().SetInRange(collision.transform);
            trap2.GetComponent<ArrowTrapAI>().SetInRange(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && isActive)
        {
            isActive = false;
            trap1.GetComponent<ArrowTrapAI>().SetOutRange();
            trap2.GetComponent<ArrowTrapAI>().SetOutRange();
        }
    }

    private void Update()
    {
        if(isActive)
        {
            transform.Rotate(0, 0, .8f);
        }
    }

    public override void PositionDecider(DungeonRoomGenerator.RoomData room)
    {
        transform.position = new Vector3(room.CurrentRoomCenter.x, room.CurrentRoomCenter.y);
    }
}
