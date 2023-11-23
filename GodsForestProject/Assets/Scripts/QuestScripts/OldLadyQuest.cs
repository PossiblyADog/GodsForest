using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldLadyQuest : AbstractQuestNPC
{
    public GameObject child;
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
                if(index++ < roomList.Count - 1)
                {
                    index++;
                }
                else
                {
                    index--;
                }
            }
            Instantiate(child, (Vector3Int)roomList[index].CurrentRoomCenter + new Vector3Int(-3, 4), Quaternion.identity);

            questText.text = "Please hurry! I don't know where she could have gone...";
            acceptButton.SetActive(false);
            SetQuestTracker();
        }

    }

    public override void CompleteQuest()
    {
        if (acceptedQuest)
        {
            base.CompleteQuest();
            questText.text = "Thank you so much, Lets get out of here child!";
            GameManager.instance.UnlockExpansion(0);
            GameManager.instance.levelOneQuests[0] = null;
            Instantiate(Resources.Load<GameObject>("Lootables/Old Woman's Pendant"), transform.position + new Vector3(1, -1), Quaternion.identity);
            Destroy(gameObject, 3.0f);

        }

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.name.Contains("Child"))
        {
            CompleteQuest();
            Destroy(collision.gameObject, 1.0f);
        }

    }
}
