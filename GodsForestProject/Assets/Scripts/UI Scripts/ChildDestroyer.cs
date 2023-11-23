using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    public void KillTheChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
