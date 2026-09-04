using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using System;

namespace MergeBeast
{
    public class UpgradeScreen : BaseScreen
    {

        [SerializeField] private UIButton _btnClose;
        
        [SerializeField] private List<UpgradeItem> listItem;

        // Use this for initialization
        void Start()
        {
            _btnClose?.onClick.AddListener(this.OnClickCloseScreen);            
        }

        private void OnClickCloseScreen()
        {
            ScreenManager.Instance.DeActiveScreen();
        }

        // Update is called once per frame


        private void OnEnable()
        {

            foreach(var item in listItem) {
                item.UpdateData();
            }
            
        }
    }
}
