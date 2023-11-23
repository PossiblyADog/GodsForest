using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveTrapButton : MonoBehaviour
{
    private float timeTilReset;
    public GameObject boxingGlove;
    private void Start()
    {
        bool nearObstacle = false;

        var hits = Physics2D.OverlapCircleAll(transform.position, 1.50f);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer == 3)
            {
                if (Vector2.Distance(hits[i].ClosestPoint(transform.position), transform.position) < 2.0f)
                { nearObstacle = true; }
                break;
            }
        }

        if (!nearObstacle)
        {
            Destroy(gameObject);
        }
        else
        {
            timeTilReset = Time.time;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Time.time > timeTilReset && (collision.gameObject.layer == 6 || collision.gameObject.layer == 8))
        {
            timeTilReset = Time.time + 3.0f;
            GetComponent<Animator>().SetTrigger("isTriggered");
            var glove = Instantiate(boxingGlove, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
            glove.GetComponent<BoxingGlove>().SetGlove(collision.transform.position);
            Invoke("ResetAnim", 1.95f);
        }
    }

    private void ResetAnim()
    {
        GetComponent<Animator>().SetTrigger("isReset");
    }

    
}
