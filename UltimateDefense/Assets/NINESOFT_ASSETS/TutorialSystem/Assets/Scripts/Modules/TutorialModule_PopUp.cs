using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialModule_PopUp : TutorialModule
    {
        [Space(5)]
        [SerializeField] private PopUpType PopUpType;
        [Space(10)]
        [SerializeField] private float startDelay;
        [SerializeField] private float speed = 1;

        [Space(10)]
        public UnityEvent OnNextButtonClicked;
        public UnityEvent OnPopUpOpened;
        public UnityEvent OnPopUpClosed;

        private GameObject child;

        [SerializeField] private string content;
        [SerializeField] private string button;

        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private TextMeshProUGUI buttonText;

#if UNITY_EDITOR
        private void OnValidate()
        {
            contentText?.SetText(content);
            buttonText?.SetText(button);
        }
#endif

        public override IEnumerator ActiveTheModuleEnum()
        {
            child = transform.GetChild(0).gameObject;
            child.gameObject.SetActive(false);
            yield return new WaitForSeconds(startDelay);
            child.gameObject.SetActive(true);

            switch (PopUpType)
            {
                case PopUpType.None:

                    break;
                case PopUpType.Scale:
                    StartCoroutine(Scale());
                    break;
                case PopUpType.Fade:
                    StartCoroutine(Fade());
                    break;
                case PopUpType.SlideToBottom:
                    StartCoroutine(Slide(false));
                    break;
                case PopUpType.SlideToTop:
                    StartCoroutine(Slide(true));
                    break;
                default:
                    break;
            }

        }

        private IEnumerator Scale(float startScale = 0, float endScale = 1)
        {
            child.transform.localScale = Vector3.one * startScale;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * speed * .1f;
                child.transform.localScale = Vector3.Lerp(child.transform.localScale, Vector3.one * endScale, t);
                if (Vector3.Distance(child.transform.localScale, Vector3.one * endScale) < .05f) break;
                yield return new WaitForEndOfFrame();
            }

            if (endScale == 0) Closed();
            else Opened();
        }

        private IEnumerator Fade(float startAlpha = 0, float endAlpha = 1)
        {
            Image[] images = GetComponentsInChildren<Image>();
            RawImage[] rawimages = GetComponentsInChildren<RawImage>();
            TextMeshProUGUI[] tmps = GetComponentsInChildren<TextMeshProUGUI>();

            for (int i = 0; i < images.Length; i++)
            {
                Color c = images[i].color;
                c.a = startAlpha;
                images[i].color = c;
            }

            for (int i = 0; i < rawimages.Length; i++)
            {
                Color c = rawimages[i].color;
                c.a = startAlpha;
                rawimages[i].color = c;
            }

            for (int i = 0; i < tmps.Length; i++)
            {
                Color c = tmps[i].color;
                c.a = startAlpha;
                tmps[i].color = c;
            }


            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                for (int i = 0; i < images.Length; i++)
                {
                    Color c = images[i].color;
                    if (endAlpha == 0)
                        c.a = 1f - (startAlpha * t);
                    else
                        c.a = endAlpha * t;

                    images[i].color = c;
                }
                for (int i = 0; i < rawimages.Length; i++)
                {
                    Color c = rawimages[i].color;
                    if (endAlpha == 0)
                        c.a = 1f - (startAlpha * t);
                    else
                        c.a = endAlpha * t;

                    rawimages[i].color = c;
                }
                for (int i = 0; i < tmps.Length; i++)
                {
                    Color c = tmps[i].color;
                    if (endAlpha == 0)
                        c.a = 1f - (startAlpha * t);
                    else
                        c.a = endAlpha * t;
                    tmps[i].color = c;
                }

                yield return new WaitForEndOfFrame();
            }

            if (endAlpha == 0) Closed();
            else Opened();
        }

        private IEnumerator Slide(bool top, float direction = 1)
        {
            Vector3 targetPos = Vector3.zero;

            if (direction == 0)
            {
                child.transform.localPosition = Vector3.zero;
                targetPos = new Vector3(0, top ? -1920 : 1920);
            }
            else if (top)
            {
                child.transform.localPosition = new Vector3(0, -1920);
            }
            else
            {
                child.transform.localPosition = new Vector3(0, 1920);
            }

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * speed * .1f;
                child.transform.localPosition = Vector3.Lerp(child.transform.localPosition, targetPos, t);
                if (Vector3.Distance(child.transform.localPosition, targetPos) < .05f) break;
                yield return new WaitForEndOfFrame();
            }

            if (direction == 0) Closed();
            else Opened();
        }



        public void CloseMe()
        {
            StopAllCoroutines();
            switch (PopUpType)
            {
                case PopUpType.None:
                    gameObject.SetActive(false);
                    break;
                case PopUpType.Scale:
                    StartCoroutine(Scale(1, 0));
                    break;
                case PopUpType.Fade:
                    StartCoroutine(Fade(1, 0));
                    break;
                case PopUpType.SlideToBottom:
                    StartCoroutine(Slide(false, 0));
                    break;
                case PopUpType.SlideToTop:
                    StartCoroutine(Slide(true, 0));
                    break;
                default:
                    break;
            }
            OnNextButtonClicked?.Invoke();
        }

        private void Opened()
        {
            OnPopUpOpened?.Invoke();
        }

        private void Closed()
        {
            OnPopUpClosed?.Invoke();
            gameObject.SetActive(false);
        }

    }
}