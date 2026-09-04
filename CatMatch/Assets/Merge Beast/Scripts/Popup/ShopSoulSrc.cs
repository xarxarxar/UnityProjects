using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Observer;

namespace MergeBeast
{
    public class ShopSoulSrc : BaseScreen
    {
        [SerializeField] private Text _txtStackSoul;
        [SerializeField] private Text _txtPackSoul;
        [SerializeField] private Text _txtChestSoul;
        [SerializeField] private UIButton _btnWatchAds;
        [SerializeField] private UIButton _btnStackSoul;
        [SerializeField] private UIButton _btnPackSoul;
        [SerializeField] private UIButton _btnChestSoul;

        private BigInteger _dpsStack, _dpsPack, _dpsChest;


        // Start is called before the first frame update
        void Start()
        {
            _btnWatchAds?.onClick.AddListener(this.OnClickShowAds);
            _btnStackSoul?.onClick.AddListener(() => this.OnClickPurchase(_dpsStack, 5));
            _btnPackSoul?.onClick.AddListener(() => this.OnClickPurchase(_dpsPack, 25));
            _btnChestSoul?.onClick.AddListener(() => this.OnClickPurchase(_dpsChest, 45));
        }

        private void OnEnable()
        {
            _dpsStack = GameManager.Instance.TotalDamage() * 300;
            _txtStackSoul.text = $"获得{Utils.FormatNumber(_dpsStack)}";

            _dpsPack = GameManager.Instance.TotalDamage() * 7200;
            _txtPackSoul.text = $"获得{Utils.FormatNumber(_dpsPack)}";

            _dpsChest = GameManager.Instance.TotalDamage() * 14400;
            _txtChestSoul.text = $"获得{Utils.FormatNumber(_dpsChest)}";
        }

        private void OnClickPurchase(BigInteger soul,int price)
        {
            if(Utils.GetCurrentRubyMoney() >= price)
            {
                Utils.AddRubyCoin(-price);
                UIManager.Instance.UpdateMoneyCoin(soul, false);
                this.PostEvent(EventID.OnUpDateMoney);
                string sl = Utils.FormatNumber(soul);
                UIManager.Instance.ShowNotify($"购买成功{sl}灵魂");
            }
            else
            {
                ScreenManager.Instance.ShowConfirm(4, "你的宝石不足，是否要购买宝石？", () =>
                {
                    ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);
                });
            }
        }

        private void OnClickShowAds()
        {
            AdsManager.Instance.ShowAds(() => this.OnClickPurchase(_dpsStack, 0));
        }
        
        public void DeActivePopup()
        {
            ScreenManager.Instance.DeActiveScreen();
        }
    }
}
