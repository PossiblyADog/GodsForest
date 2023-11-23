using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    List<Teleporter> ports = new List<Teleporter>();
    public AudioClip porterSound;
    public static TeleporterController instance;
    public int nextSpawnChance = 100;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public void AddPorter(Teleporter port)
    {
        ports.Add(port);
    }

    public IEnumerator Teleport(Teleporter port)
    {
        if (ports.Count > 1)
        {
            StartCoroutine(GameManager.instance.ScreenFlash());
            int currentPort = ports.FindIndex(porter => porter == port);
            AudioSource.PlayClipAtPoint(porterSound, PlayerController.instance.transform.position);
            yield return new WaitForSeconds(.3f);
            if (currentPort >= ports.Count - 1)
            {
                PlayerController.instance.transform.position = ports[0].transform.position;
            }
            else
            {
                PlayerController.instance.transform.position = ports[currentPort + 1].transform.position;
            }

        }
        else
        {
            var nothing = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), port.transform.position + new Vector3(.4f, .4f), Quaternion.identity);
            nothing.transform.localScale = nothing.transform.lossyScale * 3;
            nothing.GetComponentInChildren<TMPro.TMP_Text>().text = "No active links...";
            Destroy(nothing, 1.0f);
        }
        yield return null;
    }
    


}
