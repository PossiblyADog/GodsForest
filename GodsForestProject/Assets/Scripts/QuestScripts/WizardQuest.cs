using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardQuest : AbstractQuestNPC
{
    public GameObject egg;
    public GameObject acceptButton;

    public override void AcceptQuest()
    {
        if (!acceptedQuest)
        {
            acceptedQuest = true;
            List<DungeonRoomGenerator.RoomData> roomList = new List<DungeonRoomGenerator.RoomData>();
            roomList.AddRange(DungeonRoomGenerator.instance.roomDataList);

            int index = Random.Range(0, roomList.Count);
            if (roomList[index].CurrentRoomType == 3)
            {
                if (index++ < roomList.Count - 1)
                {
                    index++;
                }
                else
                {
                    index--;
                }
            }

            Instantiate(egg, (Vector3Int)roomList[index].CurrentRoomCenter + new Vector3Int(-3, 4), Quaternion.identity);
            questText.text = "It should be somewhere on this level, I'll reward you in return!";
            acceptButton.SetActive(false);
            SetQuestTracker();
        }

    }

    public override void CompleteQuest()
    {
        if (acceptedQuest)
        {
            questText.text = "Wow, you found it! I'll see you back in town.";
            GameManager.instance.UnlockExpansion(2);
            GameManager.instance.levelTwoQuests[0] = null;
            base.CompleteQuest();
            var coin = Instantiate(Resources.Load<GameObject>("Lootables/FavorItem"), transform.position + new Vector3(1, -1), Quaternion.identity);
            coin.GetComponent<FavorItem>().favorAmount = 500;
            Destroy(gameObject, 3.0f);
           
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.name.Contains("Egg"))
        {
            CompleteQuest();
            Destroy(collision.gameObject, 1.0f);
        }

    }
}
