using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public int damage;
    public float rotationSpeed, tornadoSpeed;
    bool isActive = false;
    Rigidbody2D nadoBod;
    Vector3 moveVect;
    
    public void SetTornado(float duration, Vector2 targetPos, float knock, float rot, float speed)
    {
        nadoBod = GetComponentInChildren<Rigidbody2D>();  
        transform.rotation = CursorController.instance.transform.rotation;
        damage = 7 + PlayerStateManager.playerManager.damageFlatModifier;
        tornadoSpeed = speed /** PlayerStateManager.playerManager.projectileTravelSpeedMultiplier*/;
        rotationSpeed = (rot * PlayerStateManager.playerManager.projectileTravelSpeedMultiplier);
        GetComponentInChildren<PlayerProjectile>().SetStaticAttack(damage, knock, false, 1);
        isActive = true;
        Invoke("Kill", duration);
        moveVect = (nadoBod.transform.localPosition - transform.position).normalized *.015f;
        //nadoBod.AddForce(Vector2.right * tornadoSpeed, ForceMode2D.Force);
    }

    private void Update()
    {
        if(isActive && transform.childCount > 0)
        {
            rotationSpeed -= .005f;
            moveVect += (transform.GetChild(0).transform.localPosition - transform.position).normalized / (Mathf.Pow((transform.GetChild(0).transform.localPosition - transform.position).magnitude * 600, 2.0f));
            transform.Rotate(0, 0, rotationSpeed);
            transform.GetChild(0).transform.Rotate(0, 0, -rotationSpeed);
            nadoBod.transform.localPosition += moveVect;
            //transform.GetChild(0).transform.right = (transform.GetChild(0).transform.position - transform.position).normalized;
            //nadoBod.AddForce(Vector2.right * tornadoSpeed, ForceMode2D.Force);
        }
    }

    public void Deactivate()
    {
        isActive = false;
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
