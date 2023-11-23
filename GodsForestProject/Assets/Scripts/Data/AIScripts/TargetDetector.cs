using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    public float targetDetectionRadius = 15;

    public LayerMask entityLayerMask, playerLayerMask;
    public int targetChoice = 0;//0=player 1=waypoints, 2 WaypointsMiniboss


    public bool showGizmos = false;

    private List<Transform> colliders;
    
    public override void Detect(AIData data)
    {

        if(targetChoice == 2)
        {
            targetDetectionRadius = 20;
        }

        if (targetChoice != 2)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, targetDetectionRadius, playerLayerMask);
            if (playerCollider != null)
            {
                Vector2 direction = (playerCollider.transform.position - transform.position).normalized;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDetectionRadius, entityLayerMask);
                if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    colliders = new List<Transform>() { playerCollider.transform };
                }
                else
                {
                    colliders = null;
                }
            }
            else
            {
                colliders = null;
            }
        }



        if (targetChoice == 0)
        {
            data.targets = colliders;
        }
        else if (targetChoice == 1 && colliders != null)
        {
            data.targets = new List<Transform>() { (transform.parent.parent.GetChild(1).GetComponent<WaypointAI>().transform) };
        }
        else if (targetChoice == 2)
        {
            data.targets = new List<Transform>() { (transform.parent.parent.GetChild(1).GetComponent<HuntressWaypointAI>().transform) };
        }
        else if(targetChoice == 3)
        {
            data.targets = new List<Transform>() { (transform.parent.parent.GetChild(1).GetComponent<NecromancerWaypointAI>().transform) };
        }  
        
 
    }

    private void OnDrawGizmos()
    {
        if(showGizmos == false)
        {
            return;
        }

        Gizmos.DrawWireSphere(transform.position, targetDetectionRadius);

        if(colliders == null)
        { return; }
        Gizmos.color = Color.magenta;
        foreach(var item in colliders)
        {
            Gizmos.DrawSphere(item.position, .1f);
        }

    }
}
