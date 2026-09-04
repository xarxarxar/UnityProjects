using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace MergeBeast
{
    public class UIButton : Button
    {

        private Image hover;

        private Vector3 vec3Default;

        private Vector2 buttonSize;

        protected override void Awake()
        {
            vec3Default = transform.localScale;
            if (transform.Find("hover") != null)
            {
                hover = transform.Find("hover").GetComponent<Image>();
            }
        }

        protected override void Start()
        {
            buttonSize = transform.GetComponent<RectTransform>().rect.size;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (this.IsInteractable() == false) return;
            if (hover != null)
            {
                hover.gameObject.SetActive(true);
                hover.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                if (buttonSize.x < 800) transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f);
                else transform.DOScale(new Vector3(1.05f, 1.05f, 1), 0.1f);
            }
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (this.IsInteractable() == false) return;
            transform.DOScale(vec3Default, 0.1f);
            if (hover != null)
            {
                hover.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.05f).OnComplete(() =>
                {
                    hover.gameObject.SetActive(false);
                });
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (this.IsInteractable() == false) return;
            transform.DOScale(new Vector3(0.9f, 0.9f, 1), 0.1f);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (this.IsInteractable() == false) return;
            transform.DOScale(vec3Default, 0.1f);
        }
    }
}
