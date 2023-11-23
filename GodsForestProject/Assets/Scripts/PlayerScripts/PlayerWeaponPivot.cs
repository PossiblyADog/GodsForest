using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponPivot : MonoBehaviour
{
    private Vector2 mousePos;
    void Start()
    {
    }

 
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if(Mathf.Abs(angle) > 90.0f)
        {
            transform.GetChild(0).transform.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            transform.GetChild(0).transform.GetComponent<SpriteRenderer>().flipY = false;
        }
    }
}
