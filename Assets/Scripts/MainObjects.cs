using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelObjectsType;

[CreateAssetMenu(fileName = "MainObjects", menuName = "WheelSpin/MainObjects")]
public class MainObjects : ScriptableObject {
    public ObjectsType _objectType;
    public GameObject[] _objects;
}
