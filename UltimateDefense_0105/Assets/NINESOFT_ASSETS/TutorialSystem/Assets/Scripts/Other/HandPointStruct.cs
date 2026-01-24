
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [System.Serializable]
    public struct HandPointStruct
    {
        public Transform Point;
        public Vector3 Offset;
        public HandEventType HandEventType;
    }
}