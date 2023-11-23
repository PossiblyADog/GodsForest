using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBlade : MonoBehaviour
{
    float rotSpeed;
    bool isActive = false;


    public void SetSpin(int damage, float knock, float rot)
    {
        transform.parent = PlayerController.instance.transform;
        transform.localPosition = new Vector2(0, -.15f);
        transform.GetComponentInChildren<PlayerProjectile>().SetStaticAttack(damage, knock, false, 3);
        rotSpeed = rot;
        isActive = true;
    }
    void Update()
    {
        if (isActive)
        {
            transform.Rotate(0, 0, -rotSpeed);          
        }
    }
}
