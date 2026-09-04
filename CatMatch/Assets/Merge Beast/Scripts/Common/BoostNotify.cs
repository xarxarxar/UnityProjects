using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MergeBeast
{
    public class BoostNotify : MonoBehaviour
    {
        [SerializeField] private Image _imgIconBoost;
        [SerializeField] private Text _txtToolTip;
        [SerializeField] private Text _txtTimeCount;

        string days = "";
        string hours = "";
        string minutes = "";
        string seconds = "";

        // Use this for initialization
        void Start()
        {

        }

        public void SetBoost(Sprite icon, string des)
        {
            _imgIconBoost.sprite = icon;
            _imgIconBoost.SetNativeSize();
            _txtToolTip.text = des;
        }

        public void UpdateTime(float time)
        {

            var b = new TimeSpan(0, 0, (int)time);
            days = b.Days > 0 ? b.Days.ToString("D2") : "";
            hours = b.Hours > 0 ? b.Hours.ToString("D2") + ":" : "";
            minutes = b.Minutes.ToString("D2") + ":";
            seconds = b.Seconds > 0 ? b.Seconds.ToString("D2") : "00";

            if (b.Days > 0)
            {
                _txtTimeCount.text = $"{days}天";
            }
            else
            {
                if(b.Minutes < 1) minutes = "00:";
                _txtTimeCount.text = $"{hours}{minutes}{seconds}";
            }

        }

        public void DestroyMe()
        {
            Destroy(this.gameObject);
        }

    }
}
