using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    private float targetReachedThreshold = .5f;

    private bool reachedLastTarget = true;

    [SerializeField]
    private bool showGizmos = true;
    private Vector2 targetPositionCached;
    private float[] interestTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData data)
    {
        if(reachedLastTarget)
        {
            if(data.targets == null || data.targets.Count <= 0)
            {
                data.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                reachedLastTarget = false;
                data.currentTarget = data.targets.OrderBy(target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
            }
        }

        if(data.currentTarget != null && data.targets != null && data.targets.Contains(data.currentTarget))
        {
            targetPositionCached = data.currentTarget.position;
        }

        if(Vector2.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            data.currentTarget = null;
            return(danger, interest);
        }

        Vector2 directionToTarget = (targetPositionCached - (Vector2)transform.position);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, Walk.allDirections[i]);

            if(result > 0)
            {
                float valueInput = result;
                if(valueInput > interest[i])
                {
                    interest[i] = valueInput;
                }
            }
        }
        interestTemp = interest;
        return(danger, interest);
        
    }

    public void StartSeeking()
    {
        reachedLastTarget = false;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;

        Gizmos.DrawSphere(targetPositionCached, .2f);

        if(Application.isPlaying && interestTemp != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < interestTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, (Vector2)Walk.allDirections[i] * interestTemp[i]);
            }

            if(reachedLastTarget == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPositionCached, .1f);
            }
        }

    }
}
