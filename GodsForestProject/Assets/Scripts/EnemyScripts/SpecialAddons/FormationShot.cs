using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationShot : MonoBehaviour
{
    public GameObject proj;
    public List<Transform> shotPoints;
    public int damage;
    public float formationTravelSpeed, knockForce;
    private Vector2 target;

    public void CastFormation(Vector2 targetPos)
    {
        target = targetPos;
        StartCoroutine(SummonComponents());
    }
    public IEnumerator SummonComponents()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < shotPoints.Count; i++)
        {
            var bullet = Instantiate(proj, shotPoints[i].transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyProjectile>().SetBulletParams(0, damage, knockForce, Vector2.zero, 0, 0, false);
            list.Add(bullet);
            yield return new WaitForSeconds(.125f);
        }

        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < list.Count; i++)
        {
            try
            {
                list[i].GetComponent<Rigidbody2D>().AddForce((target + PlayerController.instance.CurrentVelocity() / 3).normalized * formationTravelSpeed, ForceMode2D.Impulse);
            }
            catch
            {

            }
        }
        yield return new WaitForSeconds(.5f);
        Destroy(this);
    }
}
