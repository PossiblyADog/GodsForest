using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuestIndicator : MonoBehaviour
{
    private bool isActive = false;
    public Vector2 questLocation;
    // Start is called before the first frame update
    public void SetTracker()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        questLocation = (Vector2)GameObject.FindGameObjectWithTag("Quest").transform.position;
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Vector2 direction = questLocation - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

}
