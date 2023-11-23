using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParameters", menuName = "RandomWalkDataObj")]
public class RandomWalkData : ScriptableObject
{
    public int iterations = 10, walkLength = 10;
    public bool startRandomWalk = true;
}
