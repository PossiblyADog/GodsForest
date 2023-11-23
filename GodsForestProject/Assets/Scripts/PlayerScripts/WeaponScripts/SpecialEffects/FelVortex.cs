using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FelVortex : MonoBehaviour
{
    private float rotationSpeed;
    private float currentRotation, multiplier;
    private PointEffector2D centerEffector;

    private Transform vortexCenter;
    void Start()
    {

        vortexCenter = transform.GetChild(0).transform;
        centerEffector = vortexCenter.GetComponent<PointEffector2D>();  
        currentRotation = 0f;
    }

    public void SetVortex(float vDuration, float vRotation, float endKnock)
    {
        rotationSpeed = vRotation;
        Invoke("EndLaunch", vDuration * .95f);
        Destroy(gameObject, vDuration);
        multiplier = endKnock;
    }

    void Update()
    {

        currentRotation += rotationSpeed;
       

        vortexCenter.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    private void EndLaunch()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        centerEffector.forceMagnitude = Mathf.Abs(centerEffector.forceMagnitude*10);
    }
}
