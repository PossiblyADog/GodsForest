using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntressJumpTrigger : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerBullet")
        {
            transform.parent.GetComponentInChildren<HuntressAI>().AttemptDodge();
        }
    }

    private void Update()
    {
        transform.position = transform.parent.GetChild(0).transform.position + new Vector3(0f, -.25f);
    }
}
