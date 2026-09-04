using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace MergeBeast
{
    public class BigBeastSrc : BaseScreen
    {
        [SerializeField] private SkeletonGraphic _imgCurBeast;
        [SerializeField] private SkeletonGraphic _imgAdsBeast;
        [SerializeField] private Text _txtCurLevel;
        [SerializeField] private Text _txtAdsLevel;
        [SerializeField] private Text _txtCurDps;
        [SerializeField] private Text _txtAdsDps;
        [SerializeField] private UIButton _btnWatchVideo;
        [SerializeField] private UIButton _btnClose;


        // Start is called before the first frame update
        void Start()
        {
            _btnClose?.onClick.AddListener(this.OnClickClose);
        }

        private void OnEnable()
        {
            this.ShowPopupBigBeast();
        }

        private void ShowPopupBigBeast()
        {
            int lvBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            int maxLvBeast = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 0);
            
            var curBeast = GameManager.Instance.GetBeast(lvBeast);
            this.OnSetCatSkin(_imgCurBeast, curBeast.Level.ToString("D3"));
            _txtCurLevel.text = $"等级 {curBeast.Level}";
            _txtCurDps.text = $"DPS：{Utils.FormatNumber(Utils.GetDameByLevelAndAscend(curBeast.Level))}";

            int adsLv = Mathf.Min(lvBeast + 4, maxLvBeast);
            var adsBeast = GameManager.Instance.GetBeast(adsLv);
            this.OnSetCatSkin(_imgAdsBeast, adsBeast.Level.ToString("D3"));
            _txtAdsLevel.text = $"等级 {adsBeast.Level}";
            _txtAdsDps.text = $"DPS：{Utils.FormatNumber(Utils.GetDameByLevelAndAscend(adsBeast.Level))}";

            _btnWatchVideo?.onClick.RemoveAllListeners();
            _btnWatchVideo?.onClick.AddListener(() => this.WatchAdsBigBeast(adsLv));
        }

        private void OnSetCatSkin(SkeletonGraphic skel, string skin)
        {
            StartCoroutine(IESetCat(skel, skin));
        }

        private IEnumerator IESetCat(SkeletonGraphic skel, string skin)
        {
            yield return new WaitForEndOfFrame();
            skel.Skeleton.SetSkin(skin);
        }

        private void OnDisable()
        {
            GameManager.Instance.ReadySpawnGift();
        }

        private void WatchAdsBigBeast(int lv)
        {
            AdsManager.Instance.ShowAds(() => {
                GameManager.Instance.AddGift(lv);
                ScreenManager.Instance?.DeActiveScreen();
            }, StringDefine.ADS_RECEIVED_BIG_BEAST);
        }

        private void OnClickClose()
        {
            ScreenManager.Instance?.DeActiveScreen();
        }
    }
}
