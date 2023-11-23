using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCall : MonoBehaviour
{

    public void KillMe(float timeTilDeath, bool grow)
    {
        if (grow == true)
            GrowBig();

        Destroy(gameObject, timeTilDeath);
    }

    private void GrowBig()
    {
        while(gameObject.transform.localScale.x < 1.5)
        {
            gameObject.transform.localScale += new Vector3(.001f, .001f, .001f);
        }
        
    }
}
