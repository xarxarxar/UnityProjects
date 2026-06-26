using System.Collections.Generic;
using UnityEngine;

namespace NaughtyAttributes.Test
{
    //[CreateAssetMenu(fileName = "NaughtyScriptableObject", menuName = "NaughtyAttributes/NaughtyScriptableObject")]
    public class NaughtyScriptableObject : ScriptableObject
    {
        [Expandable]
        public List<TestScriptableObjectA> listA;
    }
}
