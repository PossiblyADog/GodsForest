using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishermanQuest : AbstractQuestNPC
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

            DungeonRoomGenerator.instance.CreateSecretRoom(new BoundsInt(new Vector3Int(20, 250), new Vector3Int(10, 10)));
            roomAIRef = Instantiate(secretRoomEnemies, new Vector2(24, 254), Quaternion.identity);
            roomAIRef.GetComponent<SecretRoom>().SetSecretRoom(this);
            var questPort = Instantiate(secretPortal, transform.position + new Vector3(2, 1), Quaternion.identity);
            questPort.GetComponent<SecretRoomPortal>().SetPortalData(new Vector2(24, 251));
            questText.text = "Great! It's right this way!";
            acceptButton.SetActive(false);
        }

    }

    public override void CompleteQuest()
    {
        if (acceptedQuest && roomAIRef.GetComponent<SecretRoom>().isComplete)
        {
            questText.text = "Ah bummer, this is useless to me. Here ya go.";
            GameManager.instance.UnlockExpansion(4);
            GameManager.instance.levelTwoQuests[1] = null;
            base.CompleteQuest();
            for (int i = 0; i < 3; i++)
            {
                Instantiate(item, transform.position - new Vector3(1, 1), Quaternion.identity);
            }

            Destroy(gameObject, 4.0f);

        }
    }


    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);       

    }
}