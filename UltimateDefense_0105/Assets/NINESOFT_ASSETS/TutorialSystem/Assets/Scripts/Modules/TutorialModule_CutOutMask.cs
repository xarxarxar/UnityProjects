using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [ExecuteAlways]
    public class TutorialModule_CutOutMask : TutorialModule
    {
        [SerializeField] private Image MaskObject;
        [SerializeField] private Image MaskFill;
        [SerializeField] private Image[] ClickBlockers;

        [SerializeField] [Range(1, 10)] private float HoleScale = 1;
        [SerializeField] [Range(0, 1)] private float HoleRadius = .8f;
        [SerializeField] private Color MaskColor;

        [SerializeField] private RectTransform TargetUI;
        [SerializeField] private bool RaycastTarget = true;

        public override IEnumerator ActiveTheModuleEnum()
        {
            if (!Application.isPlaying) yield break;

            for (int i = 0; i < ClickBlockers.Length; i++)
            {
                ClickBlockers[i].raycastTarget = RaycastTarget;
            }

            StartCoroutine(StartFocusAnimation());

            yield return new WaitForEndOfFrame();
        }

        private IEnumerator StartFocusAnimation()
        {
            float maskScale = HoleScale;
            HoleScale = 250f;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                HoleScale = Mathf.Lerp(HoleScale, maskScale, t * .1f);
                yield return new WaitForEndOfFrame();
            }

        }




#if UNITY_EDITOR

        private void Update()
        {
            UpdateData();
        }
#endif
        private void UpdateData()
        {

            float radius = Mathf.Clamp(((1 - HoleRadius) - .4f) * 10f, 0, 10f);
            MaskObject.pixelsPerUnitMultiplier = radius;

            MaskFill.color = MaskColor;

            if (TargetUI != null)
            {
                MaskObject.rectTransform.anchorMax = TargetUI.anchorMax;
                MaskObject.rectTransform.anchorMin = TargetUI.anchorMin;
                MaskObject.rectTransform.pivot = TargetUI.pivot;

                Vector2 scale = (Vector2.one * 20f) * (HoleScale - 1);
                Vector2 offset = scale / 2f;

                if (MaskObject.rectTransform.anchorMin.x == 0)
                {
                    offset.x *= -1;
                }
                else if (MaskObject.rectTransform.anchorMin.x == .5f && MaskObject.rectTransform.anchorMax.x == .5f)
                {
                    offset.x = 0;
                }

                if (MaskObject.rectTransform.anchorMax.y == 0)
                {
                    offset.y *= -1;
                }
                else if (MaskObject.rectTransform.anchorMin.y == .5f && MaskObject.rectTransform.anchorMax.y == .5f)
                {
                    offset.y = 0;
                }

                if (MaskObject.rectTransform.anchorMax == new Vector2(.5f, .5f) && MaskObject.rectTransform.anchorMin == new Vector2(.5f, .5f))
                {
                    offset *= 0;
                }

                MaskObject.rectTransform.sizeDelta = TargetUI.sizeDelta + scale;
                MaskObject.rectTransform.anchoredPosition = TargetUI.anchoredPosition + offset;


                ClickBlockers[0].transform.position = TargetUI.position + new Vector3(0, -1250 - (HoleScale * 10) - (TargetUI.pivot.y * TargetUI.sizeDelta.y));
                ClickBlockers[1].rectTransform.position = TargetUI.position + new Vector3(0, 1250 + (HoleScale * 10) +(TargetUI.pivot.y * TargetUI.sizeDelta.y));
                ClickBlockers[2].rectTransform.position = TargetUI.position + new Vector3( 1250 + (HoleScale * 10) + (TargetUI.pivot.x * TargetUI.sizeDelta.x),0);
                ClickBlockers[3].rectTransform.position = TargetUI.position + new Vector3(-1250 - (HoleScale * 10) - (TargetUI.pivot.x * TargetUI.sizeDelta.x), 0);
            }
        }

    }
}