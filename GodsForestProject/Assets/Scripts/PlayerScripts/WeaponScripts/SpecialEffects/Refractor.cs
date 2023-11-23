using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refractor : MonoBehaviour
{
    public void SetRefractData(float duration)
    {

        Invoke("End", duration);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            float refractAngle = Random.Range(-1.00f, 1.001f);
            collision.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * refractAngle * 10, ForceMode2D.Impulse);
        }
    }

    private void End()
    {
        Destroy(gameObject);
    }
}
