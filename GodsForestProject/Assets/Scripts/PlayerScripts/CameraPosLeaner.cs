using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosLeaner : MonoBehaviour
{
    public Camera playerCamera;
    private Transform playerTrans;
    public float leanThreshold;

    private void Start()
    {
        playerTrans = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        var target = playerTrans.position + (mousePos - playerTrans.position);

        target.x = Mathf.Clamp(target.x, playerTrans.position.x - leanThreshold, playerTrans.position.x + leanThreshold);
        target.y = Mathf.Clamp(target.y, playerTrans.position.y - leanThreshold * 1.6f, playerTrans.position.y + leanThreshold * 1.6f);

        if (Vector2.Distance(playerTrans.position, mousePos) > 2.8f)
        {
            var posDif = Vector2.Distance(transform.position, target);
            if (posDif > .25f)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, posDif * 3.0f * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTrans.position, 2.5f * Time.deltaTime);
        }

    }
}
