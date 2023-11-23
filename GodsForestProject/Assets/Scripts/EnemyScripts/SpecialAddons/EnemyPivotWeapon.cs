using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPivotWeapon : MonoBehaviour
{
    public Transform target, rotationPoint;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        { 
        var direction = target.position - rotationPoint.position;
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90;

        rotationPoint.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
