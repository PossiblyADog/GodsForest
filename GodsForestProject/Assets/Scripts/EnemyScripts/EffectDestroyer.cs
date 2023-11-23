using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    public void SayWhen(float seconds)
    {
        Destroy(gameObject, seconds);
    }
}
