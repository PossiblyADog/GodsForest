using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceBehaviour : SteeringBehaviour
{
    private float radius = 2f, agentColliderSize = .6f;

    [SerializeField]
    private bool showGizmos = true;

    float[] dangerResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData data)
    {
        foreach(Collider2D obstacleCollider in data.obstacles)
        {
            Vector2 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;

            Vector2 normalizedDTO = directionToObstacle.normalized;

            for (int i = 0; i < Walk.allDirections.Count; i++)
            {
                float result = Vector2.Dot(normalizedDTO, Walk.allDirections[i]);

                float valueInput = result * weight;

                if(valueInput > danger[i])
                {
                    danger[i] = valueInput;
                }
            }
        }
        dangerResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        if (Application.isPlaying && dangerResultTemp != null)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < dangerResultTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, (Vector2)Walk.allDirections[i] * dangerResultTemp[i]);
            }

        
        }   
        else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, radius);
            }

    }

}
