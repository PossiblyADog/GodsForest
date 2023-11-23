using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectStarter : MonoBehaviour
{
    public ParticleSystem currentSystem;


    public void OnOffSwitch()
    {
        if (!currentSystem.isPlaying)
        {
            currentSystem.Play();
        }
        else
        {
            currentSystem.Stop();
        }
    }

    /*private void OnDisable()
    {
        currentSystem.Stop();
    }*/
}
