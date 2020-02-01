using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GGJ/LevelConfig", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject {
    public SequenceData Sequence;
    public ShapeData[] Shapes;
    public SunData Sun;
}