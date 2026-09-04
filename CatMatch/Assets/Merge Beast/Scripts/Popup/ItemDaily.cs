using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace MergeBeast
{
    public class ItemDaily : MonoBehaviour
    {
        public Image BgItem;
        public Image Icon;
        public Text TxtQuantity;
        public Text TxtDay;
        public GameObject ObjDone;
        public GameObject ObjSelect;
        public RectTransform MyRect;
        public Transform Light1;
        public Transform Light2;


        private DailyItem _daily;
        private bool _isReward;
        private int _value;

        public UnityAction<DailyItem, int> DelegateReward;
        public UnityAction<DailyItem,RectTransform> DelegateTooltip;
        public UnityAction DelegateClose;

        public void OnClickDown()
        {
            if (_isReward)
            {
                DelegateReward?.Invoke(_daily, _value);
                ObjDone.SetActive(true);
                ObjSelect.SetActive(false);
                _isReward = false;
            }
            else
            {
                DelegateTooltip?.Invoke(_daily,MyRect);
            }
        }

        public void OnClickUp()
        {
            DelegateClose?.Invoke();
        }

        public void SetEvent(bool isReward,DailyItem item, int value)
        {
            _isReward = isReward;
            _daily = item;
            _value = value;
        }

        private void Update()
        {
            if (ObjSelect.activeInHierarchy)
            {
                Light1.Rotate(Vector3.forward * 20f * Time.deltaTime);
                Light2.Rotate(Vector3.forward * -20f * Time.deltaTime);
            }
        }
    }
}
