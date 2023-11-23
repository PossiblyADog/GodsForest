using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAnimDecider : MonoBehaviour
{

    public void SetAnim(int animNumber)
    {
        switch(animNumber)
        {
            case 1:
                gameObject.GetComponent<Animator>().SetTrigger("1");
                break;
            case 2:
                gameObject.GetComponent<Animator>().SetTrigger("2");
                break;
            case 3:
                gameObject.GetComponent<Animator>().SetTrigger("3");
                break;
            case 4:
                gameObject.GetComponent<Animator>().SetTrigger("4");
                break;
            case 5:
                gameObject.GetComponent<Animator>().SetTrigger("5");
                break;
            case 6:
                gameObject.GetComponent<Animator>().SetTrigger("6");
                break;

            default:
                break;

        }
            
        Destroy(gameObject, .5f);
    }
}
