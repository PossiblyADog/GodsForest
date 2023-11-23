using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField]
    private bool showGizmos = true;

    float[] interestGizmo = new float[0];
    Vector2 resultDirection = Vector2.zero;
    private float rayLength = 1;

    private void Start()
    {
        interestGizmo = new float[8];

    }

    public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours, AIData data)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach(SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, data);
        }

        for(int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }
        interestGizmo = interest;

        Vector2 moveDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            moveDirection += (Vector2)Walk.allDirections[i] * interest[i];
        }

        moveDirection.Normalize();

        resultDirection = moveDirection;

        return resultDirection;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLength);
        }
    }
}