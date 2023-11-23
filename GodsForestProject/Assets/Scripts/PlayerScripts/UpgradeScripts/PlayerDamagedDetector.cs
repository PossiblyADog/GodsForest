using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamagedDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyBullet")
        {
            PlayerController.instance.TriggerIncomingProjectileItems();
        }

    }
}
