using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn打开 http://web3incubators.com/
//电报https://t.me/gamecode999
namespace MergeBeast {
    public class VipController : BaseScreen {
        public static VipController _Instance;        
        [SerializeField] Text currentVipText;
        [SerializeField] Text minText;
        [SerializeField] Text nextText;
        [SerializeField] Text currentVipProgressText;
        [SerializeField] Slider vipSlider;        

        [SerializeField] Text benefitText;
        [SerializeField] Button btnPrev;
        [SerializeField] Button btnNext;
        [SerializeField] Button btnClose;
        [SerializeField] RectTransform scroll;

        //[SerializeField] Button btnShowDesc;
        //[SerializeField] GameObject vipDescObj;
        //[SerializeField] Text desc1;
        //[SerializeField] Text desc2;
        //[SerializeField] Button btnCloseDesc;        

        private int prevVipIndex;
        private int nextVipIndex;
        private string yellow = "ffe84e";
        private float itemWidth = 900;
        private int scrollIndex;
        private bool canClickScroll = true;        
        private void Awake() {
            if (_Instance == null) _Instance = this;
        }

        private void Start() {
            btnClose.onClick.AddListener(() => Hide());
            btnNext.onClick.AddListener(() => Next());
            btnPrev.onClick.AddListener(() => Prev());
            
        }

        private void OnEnable() {
            Show();
        }

        public void Show() {                            
            VipConfig currentData = null;
            for (int i = 0; i < GameAssets.Instance.listVipConfig.Count; i++) {
                VipConfig data = GameAssets.Instance.listVipConfig[i];
                if (CPlayer.vipPoint >= data.min && CPlayer.vipPoint <= data.max) {
                    currentData = data;
                    prevVipIndex = i - 1;
                    nextVipIndex = i + 1;
                    break;
                }
            }
            if (prevVipIndex < 0) {
                //dang o 0
                vipSlider.minValue = currentData.min;
                vipSlider.maxValue = currentData.max;
                vipSlider.value = CPlayer.vipPoint;
                scrollIndex = 0;

            } else {
                VipConfig prevData = GameAssets.Instance.listVipConfig[prevVipIndex];
                vipSlider.minValue = currentData.min - prevData.max;
                vipSlider.maxValue = currentData.max - prevData.max;
                vipSlider.value = CPlayer.vipPoint - prevData.max;
                scrollIndex = prevVipIndex;
                
            }


            if (prevVipIndex >= GameAssets.Instance.listVipConfig.Count - 2) {
                //max vip 4
                vipSlider.minValue = 0;
                vipSlider.maxValue = 1;
                vipSlider.value = 1;
                currentVipProgressText.text = "已满";
            } else {
                currentVipProgressText.text = $"{CPlayer.vipPoint}/{currentData.max}";                
            }
            
            currentVipText.text = string.Format("你是{0}", $"<color=#{yellow}>{currentData.status}</color>");

            //VIP_BENEFITS
            benefitText.text = string.Format("{0}特权", currentData.status);
            //benefitText.text = $"{currentData.danhhieu} Benefits";

            minText.text = currentData.status;
            if (nextVipIndex >= GameAssets.Instance.listVipConfig.Count) {
                //max vip
                nextText.text = "";
            } else {
                nextText.text = GameAssets.Instance.listVipConfig[nextVipIndex].status;
            }

            scroll.anchoredPosition = new Vector2(-itemWidth * scrollIndex, 0);

            //check neu use bam vao nut quick buy
            if(CPlayer.goToVip3) {
                scrollIndex = 2;
            }

            DoScroll();           

        }

        private void Hide() {
            ScreenManager.Instance.DeActiveScreen();
            CPlayer.goToVip3 = false;
        }

        private void Next() {
            if (!canClickScroll) return;
            canClickScroll = false;
            scrollIndex++;
            DoScroll();
        }

        private void Prev() {
            if (!canClickScroll) return;
            canClickScroll = false;
            scrollIndex--;
            DoScroll();
        }

        private void DoScroll() {
            scrollIndex = Mathf.Clamp(scrollIndex, 0, GameAssets.Instance.listVipConfig.Count - 2);
            scroll.DOAnchorPosX(-itemWidth * scrollIndex, 0.3f).OnComplete(() => {
                canClickScroll = true;
            });
            benefitText.text = string.Format("{0}特权", GameAssets.Instance.listVipConfig[scrollIndex + 1].status);
        }

        
    } //end class
}