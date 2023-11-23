using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GingerQuest : AbstractQuestNPC
{
    public GameObject item;
    public GameObject acceptButton;
    public GameObject secretPortal, secretRoomEnemies;
    private GameObject roomAIRef;


    public override void AcceptQuest()
    {
        if (!acceptedQuest)
        {
            acceptedQuest = true;

            DungeonRoomGenerator.instance.CreateSecretRoom(new BoundsInt(new Vector3Int(60, 250), new Vector3Int(13, 13)));
            roomAIRef = Instantiate(secretRoomEnemies, new Vector2(65, 255), Quaternion.identity);
            roomAIRef.GetComponent<SecretRoom>().SetSecretRoom(this);
            var questPort = Instantiate(secretPortal, transform.position + new Vector3(2, 1), Quaternion.identity);
            questPort.GetComponent<SecretRoomPortal>().SetPortalData(new Vector2(65, 252));
            questText.text = "I don't know how he got out, please save him!";
            acceptButton.SetActive(false);
        }

    }

    public override void CompleteQuest()
    {
        if (acceptedQuest && roomAIRef.GetComponent<SecretRoom>().isComplete)
        {
            questText.text = "Thank you so much! I'll put him somewhere safe right away.";
            GameManager.instance.UnlockExpansion(3);
            GameManager.instance.levelThreeQuests[0] = null;
            Destroy(roomAIRef.GetComponent<SecretRoom>().questObj, 2.75f);
            base.CompleteQuest();
            for (int i = 0; i < 3; i++)
            {
                Instantiate(item, transform.position - new Vector3(1, 1), Quaternion.identity);
            }

            Destroy(gameObject, 3.0f);

        }
    }


    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

    }
}
