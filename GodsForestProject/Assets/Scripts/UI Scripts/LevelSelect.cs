using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    bool inRange = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            inRange = false;
        }
    }

    private void Update()
    {
        if(inRange && Input.GetKeyDown(KeyCode.F))
        {
            if(gameObject.name.Contains("PettyCrime"))
            {
                GameManager.instance.PettyCrimeLevelLoad();
            }
            if (gameObject.name.Contains("MinorSin"))
            {
                GameManager.instance.MinorSinLevelLoad();
            }
            if (gameObject.name.Contains("MajorSin"))
            {
                GameManager.instance.MajorSinLevelLoad();
            }

            if (gameObject.name.Contains("Test"))
            {
                GameManager.instance.TestLevelLoad();
            }
        }
    }
}
