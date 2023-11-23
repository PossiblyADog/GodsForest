using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractQuestNPC : MonoBehaviour
{
    public Canvas npcCanvas;
    public TMPro.TMP_Text questText;
    public bool acceptedQuest = false;
    public GameObject questTracker;
    public virtual void AcceptQuest()
    {


    }

    public virtual void CompleteQuest()
    {
        if(PlayerController.instance.transform.GetComponentInChildren<PlayerQuestIndicator>() != null)
        Destroy(PlayerController.instance.transform.GetComponentInChildren<PlayerQuestIndicator>().gameObject);
    }

    public virtual void SetQuestTracker()
    {
        var tracker = Instantiate(questTracker, transform.position, Quaternion.identity);
        tracker.transform.SetParent(PlayerController.instance.gameObject.transform, false);
        tracker.GetComponent<PlayerQuestIndicator>().SetTracker();
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        { npcCanvas.enabled = true; }
    }

    /*public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        { npcCanvas.enabled = false; }

    }*/
}
