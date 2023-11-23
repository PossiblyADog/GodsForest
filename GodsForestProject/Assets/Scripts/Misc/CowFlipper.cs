using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowFlipper : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        }
    }


}
