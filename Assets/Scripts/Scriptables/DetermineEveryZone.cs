using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DetermineEveryZone", menuName = "WheelSpin/DetermineEveryZone")]
public class DetermineEveryZone : ScriptableObject {

    public ZoneProp[] _zoneProps;
    [System.Serializable]
    public struct ZoneProp {
        public SliceProp[] _sliceProps;
    }
    
    [System.Serializable]
    public struct SliceProp {
        public int _levelOfObject;
        public float _weightOfObject;
        public int _countOfObject;
        public MainObjects _mainObject;
    }
}
