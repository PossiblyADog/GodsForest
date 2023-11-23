using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    float miniTimer = 0.0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player") && Input.GetKeyDown(KeyCode.F))
        {
            if (Time.time > miniTimer)
            {
                try
                {
                    StartCoroutine(TeleporterController.instance.Teleport(this));
                    miniTimer = Time.time + 1.0f;
                }
                catch
                {

                }
            }
            else { }
        }
    }
}
