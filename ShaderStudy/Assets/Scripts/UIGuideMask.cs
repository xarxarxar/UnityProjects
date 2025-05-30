using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

    public enum GuideFinishType
    {
        None,
        Btns, //点击触发按钮
        Anywhere,//点击任意位置
    }
    public class UIGuideMask : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] RectTransform outTarget;
        [SerializeField] List<Button> canClickButton;
        [SerializeField] GuideFinishType finishType;

        [SerializeField] bool isFollow; //是否高亮跟随目标

        public event Action onClickCb; //点到目标的回调

        private Material material;
        private Vector4 outValue;
        private Camera uiCamera;

        void Awake()
        {
            Image image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("UIGuideMask need image");
            }
            material = image.material;
            if (material == null)
            {
                Debug.LogError("UIGuideMask need  Shader GuideMask Material ");
            }

        }


        public void SetOutTarget(RectTransform target)
        {
            if (target == null)
            {
                outTarget = null;
                gameObject.SetActive(false);
            }
            else
            {
                outTarget = target;
                gameObject.SetActive(true);
                ApplyOutMask();
            }
        }
        public void SetClickBtn(List<Button> btns)
        {
            canClickButton = btns;
        }
        public void AddClickBtn(Button btn)
        {
            if (canClickButton == null) canClickButton = new List<Button>();

            canClickButton.Add(btn);
        }
        public void Finish()
        {
            outTarget = null;
            canClickButton.Clear();
            finishType = GuideFinishType.None;

            gameObject.SetActive(false);
            onClickCb?.Invoke();
        }


        void Update()
        {
            if (isFollow)
            {
                ApplyOutMask();
            }
        }


        void ApplyOutMask()
        {
            if (outTarget != null)
            {
                if (uiCamera == null)
                {
                   uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
                }

                Vector3[] corners = new Vector3[4];
                outTarget.GetWorldCorners(corners);
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]);

                outValue.x = screenPos.x;
                outValue.y = screenPos.y;
                outValue.z = outTarget.rect.width;
                outValue.w = outTarget.rect.height;

                material.SetVector("_Origin", outValue);

            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null) return;

            if (finishType == GuideFinishType.Anywhere)
            {
                Finish();
            }
            else if (finishType == GuideFinishType.Btns)
            {
                if (canClickButton != null && canClickButton.Count > 0)
                {
                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(eventData, results);

                    for (int i = 0; i < results.Count; i++)
                    {
                        for (int j = 0; j < canClickButton.Count; j++)
                        {
                            if (results[i].gameObject == canClickButton[j].gameObject)
                            {
                                ExecuteEvents.Execute(canClickButton[j].gameObject, eventData, ExecuteEvents.pointerClickHandler);
                                onClickCb?.Invoke();
                            }
                        }
                    }
                }
            }

        }
    }
