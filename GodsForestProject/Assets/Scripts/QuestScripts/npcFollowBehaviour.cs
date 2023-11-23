using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcFollowBehaviour : MonoBehaviour
{
    public bool startFollow;
    public Transform followTarget;
    public Rigidbody2D theBody;

    private void FixedUpdate()
    {
        if (startFollow && Vector2.Distance(transform.position, followTarget.position) > 1)
        {
            theBody.AddForce((followTarget.position - transform.position) * 500 * Time.fixedDeltaTime);
        }
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            var questText = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), transform.position + new Vector3(0, .25f), Quaternion.identity);
            questText.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
            questText.transform.localScale = questText.transform.lossyScale * 2.5f;

            switch (gameObject.name)
            {
                case "child":
                    questText.GetComponentInChildren<TMPro.TMP_Text>().text = "I lost grandma! Help!";
                    break;

                case "egg":
                    questText.GetComponentInChildren<TMPro.TMP_Text>().text = "*rustle* *rustle*";
                    break;
            }
            Destroy(questText, 1.0f);

            if (!startFollow)
            {
                startFollow = true;
                followTarget = collision.transform;
                try
                {
                    PlayerController.instance.transform.GetComponentInChildren<PlayerQuestIndicator>().questLocation = FindObjectOfType<AbstractQuestNPC>().transform.position;
                }
                catch
                {

                }
            }
        }
    }

}
