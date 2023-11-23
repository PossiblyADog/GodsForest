using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : Detector
{
    public float detectionRadius = 3;

    public LayerMask layerMask;
    public bool showGizmos = true;

    Collider2D[] colliders;

    public override void Detect(AIData data)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);
        data.obstacles = colliders;

    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
        { return; 
        }
        if(Application.isPlaying && colliders!= null)
        {
            Gizmos.color = Color.red;
            foreach(Collider2D collider in colliders)
            {
                Gizmos.DrawSphere(collider.transform.position, .2f);
            }
        }
    }
}
