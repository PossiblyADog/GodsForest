using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoom : MonoBehaviour
{
    bool activated = false;
    public List<GameObject> enemies;
    private AbstractQuestNPC npc;
    public bool isComplete = false;
    public GameObject teleporter, entryEffect, questObject, questObj;
    


    public void SetSecretRoom(AbstractQuestNPC currentQuest)
    {
        npc = currentQuest;
        StartCoroutine(EnemyChecker());
    }

    IEnumerator EnemyChecker()
    {
        yield return new WaitForSeconds(1.0f);
        enemies.RemoveAll(empty => empty == null);

        if(enemies.Count == 0)
        {
            isComplete = true;
            npc.transform.position = transform.position + new Vector3(.5f, 1.0f);
            if (questObject != null)
            { questObj = Instantiate(questObject, transform.position + new Vector3(0, 1.0f), Quaternion.identity); }
            var porter = Instantiate(teleporter, transform.position, Quaternion.identity);
            SpawnCloud(porter.transform.position, true, new Color(200.0f / 255.0f, 114.0f / 255.0f, 196.0f / 255.0f, 220.0f / 255.0f));
            TeleporterController.instance.AddPorter(porter.GetComponent<Teleporter>());
            npc.GetComponent<AbstractQuestNPC>().CompleteQuest();
        }
        else
        {
            StartCoroutine(EnemyChecker());
        }


    }

    private void SpawnCloud(Vector2 pos, bool isSpawner, Color shade)
    {

        var cloud = Instantiate(entryEffect, pos, Quaternion.identity);
        cloud.GetComponent<SpriteRenderer>().color = shade;
        if (isSpawner)
        {
            cloud.transform.localScale *= 2.5f;
        }
        Destroy(cloud, .35f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player" && !activated)
        {
            activated = true;
            foreach(var enemy in enemies)
            {
                enemy.SetActive(true);
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetStaticSpawnPos();
                enemy.GetComponentInChildren<AbstractEnemyBase>().Initialize();
                enemy.GetComponentInChildren<AbstractEnemyBase>().SetTarget();
            }
        }
    }
}
