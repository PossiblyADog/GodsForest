using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Traps", menuName = "TrapName")] put this at top of each trap
public abstract class AbstractTrapBase : MonoBehaviour
{

    public int trapDamage, trapKnock;
    public float projSpeed, cooldown, nextCastTime = 0f;
    public GameObject trapEffect;

    public virtual void FireEffect(Vector3 targetPos)
    {

    }

    public virtual void PositionDecider(DungeonRoomGenerator.RoomData room)
    {

    }
}
