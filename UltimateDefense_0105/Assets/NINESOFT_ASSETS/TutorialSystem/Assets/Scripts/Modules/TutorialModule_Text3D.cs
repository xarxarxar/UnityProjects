using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialModule_Text3D : TutorialModule
    {
        public string text;
        public TextMeshPro textContent;
        
        [SerializeField]public TweenData TweenData;
#if UNITY_EDITOR
        private void OnValidate()
        {
            textContent?.SetText(text);
        }
#endif
        public override IEnumerator ActiveTheModuleEnum()
        {
            TweenData.TweenAnimation(this);
            yield return new WaitForEndOfFrame();
        }
    }
}
