using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointStrike : MonoBehaviour
{
    public IEnumerator PointStrikeSet(float delay)
    {
        yield return new WaitForSeconds(delay /*+ Random.Range(-.25f, .25f)*/);
        GetComponent<Animator>().SetTrigger("isAttack");
        Destroy(this.gameObject, 1.0f);
    }
}
