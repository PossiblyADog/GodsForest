using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoomPortal : AbstractInteractable
{
    private Vector2 destPos;
    public void SetPortalData(Vector2 destination) 
    {
        destPos = destination;
    }


    public override void PerformInteraction()
    {
        PlayerController.instance.transform.position = destPos;
    }

}
