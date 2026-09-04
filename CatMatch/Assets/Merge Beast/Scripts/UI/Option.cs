using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MergeBeast {
    public class Option : MonoBehaviour {
        [SerializeField] private GameObject[] listNotice;
        [SerializeField] private GameObject noticeSetting;
        [SerializeField] private Button btnOverlay;
        [SerializeField] private UIButton btnDaily;
        [SerializeField] private GameObject dailyPopup;        
        private Animator anim;

        private void Awake() {
            
            anim = GetComponent<Animator>();
            btnOverlay.onClick.AddListener(() => HideOption());
            btnDaily.onClick.AddListener(() => ShowDaily());            
        }

        private void AddGem() {
            Utils.AddRubyCoin(1000000);
            UIManager.Instance.UpdateMoneyRuby();
        }

        private void AddVip(int value) {
            CPlayer.AddVipPoint(value);
        }

        private void Start() {
            CheckShowNotice();
        }

        void ShowVip() {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.VIP);
        }

        private void HideOption() {
            //anim.SetBool("show", false);
            //btnOverlay.gameObject.SetActive(false);
            UIManager.Instance.OnClickShowOption();
        }

        public void CheckShowNotice() {
            int count = 0;
            for(int i = 0; i < listNotice.Length; i++) {
                if(listNotice[i].activeInHierarchy) {
                    count++;
                }
            }

            noticeSetting.SetActive(count > 0);
        }

        public void ShowBtnOverlay(bool active) {
            btnOverlay.gameObject.SetActive(active);
            if(!active) {
                CheckShowNotice();
            }
        }

        private void ShowDaily() {
            dailyPopup.SetActive(true);
            HideOption();
        }
    } //end class
}