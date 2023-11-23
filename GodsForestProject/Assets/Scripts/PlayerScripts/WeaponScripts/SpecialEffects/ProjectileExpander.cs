using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExpander : MonoBehaviour
{
    private float expRate;
    private bool center;
    public void SetExpansionRate(float rate, bool centerExplosion)
    {
        expRate = rate;
        center = centerExplosion;
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3 || collision.gameObject.layer == 8 || collision.gameObject.layer == 11)
        {
            if (center)
            {
                transform.GetComponent<CircleCollider2D>().offset = Vector3.zero;
            }

            transform.localScale += new Vector3(expRate * 100f, expRate * 100f);
            transform.GetComponent<CircleCollider2D>().radius += expRate*25f;
            Destroy(this);
        }
    }
}
