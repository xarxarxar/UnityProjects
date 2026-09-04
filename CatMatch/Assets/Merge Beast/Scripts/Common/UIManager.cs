using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Observer;
using System.Numerics;
using DG.Tweening;
using Spine.Unity;

namespace MergeBeast
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance;

        [SerializeField] private Text _txtMoneyCoin;
        [SerializeField] private Text _txtMoneyRuby;
        [SerializeField] private Button _btnSpawnBeast;
        [SerializeField] private UIButton _btnAutoMerge;
        [SerializeField] private Text _txtCountMerge;
        [SerializeField] private SkeletonGraphic _imgBeast;
        [SerializeField] private Text _txtPriceBeast;
        [SerializeField] private Text _txtBeastLv;
        [SerializeField] private Text _txtBeastDPS;
        [SerializeField] private Text _txtTipUnlock;
        [SerializeField] private Text _txtMergeCount;
        [SerializeField] private Text _txtPlayerName;
        [SerializeField] private RectTransform _rectBeastDPS;
        [SerializeField] private RectTransform _rectTipUnlock;
        [SerializeField] private MergeEffect _prfMerge;
        [SerializeField] private GiftBeastFX _prfGiftBeast;
        [SerializeField] private GameObject _objNoticeBook;
        [SerializeField] private GameObject _objNoticeBoost;
        [SerializeField] private GameObject _objNoticeAscend;
        [SerializeField] private Text _txtNotify;
        [SerializeField] private Animator _animNotify;
        [SerializeField] private GameObject _tooltipEnemy;
        [SerializeField] private Animator _animOption;
        [SerializeField] private GameObject _iconAdsMedalMerge;

        [Header("Button Show Screen")]
        [SerializeField] private UIButton _btnUpgrade;
        [SerializeField] private UIButton _btnBoost;
        [SerializeField] private UIButton _btnBook;
        [SerializeField] private UIButton _btnShop;
        [SerializeField] private UIButton _btnShopMerge;
        [SerializeField] private UIButton _btnAddGem;
        [SerializeField] private UIButton _btnAddSoul;
        [SerializeField] private UIButton _btnAscend;
        [SerializeField] private UIButton _btnSetting;
        [SerializeField] private UIButton _btnShowOption;
        [SerializeField] private UIButton _btnShowBattle;
        public Option optionCtl;
        [SerializeField] private Button btnQuickBuy;

        [SerializeField] private GameObject loading;
        private BigInteger _currentCoin;
        private BigInteger _currentPriceBeast;
        private Queue<MergeEffect> _queueMerge;
        private Queue<GiftBeastFX> _queueGift;
        private bool _lockSpawn, _isShowOption;
        private int _totalMerge;

        public ButtonSpawnBeast btnSpawnScriptAnimation;
        [HideInInspector] public bool isGameOver;

        [HideInInspector] public bool shouldLoadEnemy = false; //load enemy khi kill boss va di tu map vao


        // Use this for initialization
        private void Awake()
        {
            if (_instance == null) _instance = this;

            _queueMerge = new Queue<MergeEffect>();
            _queueGift = new Queue<GiftBeastFX>();
            for (int i = 0; i < 20; i++)
            {
                MergeEffect prf = Instantiate(_prfMerge, _prfMerge.transform.parent);
                prf.SetTakePool(this.EnQueueMerge);
                this.EnQueueMerge(prf);

                GiftBeastFX fx = Instantiate(_prfGiftBeast, _prfGiftBeast.transform.parent);
                fx.SetEvent(this.EnQueueGiftBeast);
                this.EnQueueGiftBeast(fx);
            }

            Destroy(_prfMerge.gameObject);
            Destroy(_prfGiftBeast.gameObject);
            this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => UpdateMoneyRuby());
            this.RegisterListener(EventID.BackFromMap, (sender, param) => BackToMap());

            //btnSpawnScript = _btnSpawnBeast.GetComponent<ButtonSpawnBeast>();
        }

        private void BackToMap()
        {
            if (!gameObject.activeSelf) return;
            var child = _btnShowBattle.transform.GetChild(1);
            if (child != null) child.gameObject.SetActive(RewardHelper.CheckHasChapterRewards());
        }

        void Start()
        {
            _currentCoin = BigInteger.Parse(PlayerPrefs.GetString(StringDefine.MONEY_COIN, "0"));
            _totalMerge = PlayerPrefs.GetInt(StringDefine.MONEY_MERGE, 0);
            UpdateMoneyRuby();
            UpdateMoneyMerge();
            FormatMoney(_currentCoin);
            _btnSpawnBeast?.onClick.AddListener(this.OnClickSpawnBeast);
            _btnAutoMerge?.onClick.AddListener(this.OnClickAutoMerge);
            _btnUpgrade?.onClick.AddListener(this.OnClickUpgrade);
            _btnBoost?.onClick.AddListener(this.OnClickBoost);
            _btnBook?.onClick.AddListener(this.OnClickBook);
            _btnShop?.onClick.AddListener(this.OnClickShop);
            _btnAddGem?.onClick.AddListener(this.OnClickShop);
            _btnAddSoul?.onClick.AddListener(this.OnClickAddSoul);
            _btnShopMerge?.onClick.AddListener(this.OnClickShopMerge);
            _btnShopMerge.interactable = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST) >= 9 ||
                PlayerPrefs.GetInt(StringDefine.TOTAL_DPS_ASCEND) > 0;
            _btnAscend?.onClick.AddListener(this.OnClickAscend);
            _btnSetting?.onClick.AddListener(this.OnClickSetting);
            _btnShowOption?.onClick.AddListener(this.OnClickShowOption);
            btnQuickBuy.onClick.AddListener(OnClickQuickBuy);
            _btnShowBattle.onClick.AddListener(() => ShowBattle());
            _txtPlayerName.text = $"灵魂者{PlayerPrefs.GetString(StringDefine.KEY_PLAYER_NAME, UnityEngine.Random.Range(0,99999).ToString())}";

            if(string.IsNullOrEmpty(PlayerPrefs.GetString(StringDefine.KEY_PLAYER_NAME,string.Empty)))
            {
                string namepl = $"灵魂者{ UnityEngine.Random.Range(0, 99999)}";
                PlayerPrefs.SetString(StringDefine.KEY_PLAYER_NAME, namepl);
            }

            _txtPlayerName.text = PlayerPrefs.GetString(StringDefine.KEY_PLAYER_NAME);

            ShowQuickBuy(CPlayer.GetVipConfig().quickBuy);

            if (AdsManager.Instance.IsLoaded()) this.CheckNoticeBook();

            var child = _btnShowBattle.transform.GetChild(1);
            if (child != null) child.gameObject.SetActive(RewardHelper.CheckHasChapterRewards());
        }


        void ShowBattle()
        {
            this.PostEvent(EventID.OnClickBattleAtMain);
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.BATTLE);
        }
        public void ActiveStarShopWhenAscend()
        {
            _btnShopMerge.interactable = true;
        }

        public void UpdateMoneyCoin(BigInteger coin, bool ignoreCheck)

        {
            _currentCoin += coin;
            //Debug.LogError("====================== add coint: " + coin);
            _currentCoin += coin;
            this.FormatMoney(_currentCoin, ignoreCheck);
            EventDispatcher.Instance.PostEvent(EventID.SoulChange, _currentCoin);
        }

        public void UpdateText()
        {
            this.PostEvent(EventID.OnUpdateAutoMergeMedal);
            UpdateMoneyRuby();
        }

        public void UpdateMoneyRuby()
        {
            _txtMoneyRuby.text = $"{PlayerPrefs.GetInt(StringDefine.MONEY_GEM)}";
        }

        public void UpdateMoneyMerge()
        {
            _txtMergeCount.text = $"{PlayerPrefs.GetInt(StringDefine.MONEY_MERGE)}";
        }

        public void SetActiveAutoMerge(bool active, int total, bool showAds = false)
        {
            _btnAutoMerge.interactable = active;
            _txtCountMerge.text = Utils.FormatNumber(total);
            _iconAdsMedalMerge.SetActive(showAds);
        }

        public void SetPriceBeast(BeastData data)
        {
            _imgBeast.Skeleton.SetSkin(data.Level.ToString("D3"));
            _currentPriceBeast = Utils.PriceOfBeast(data.Level);
            _txtPriceBeast.text = Utils.FormatNumber(_currentPriceBeast);
            _txtBeastLv.text = data.Level.ToString();
            _imgBeast.SetNativeSize();
        }

        public void EnQueueMerge(MergeEffect fx)
        {
            fx.gameObject.SetActive(false);
            if (!_queueMerge.Contains(fx)) _queueMerge.Enqueue(fx);
        }

        public void EnQueueGiftBeast(GiftBeastFX gift)
        {
            gift.gameObject.SetActive(false);
            if (!_queueGift.Contains(gift)) _queueGift.Enqueue(gift);
        }

        public void ShowEffectMerge(UnityEngine.Vector3 position, int lv, UnityAction callback)
        {
            var fx = _queueMerge.Dequeue();
            fx.transform.position = position;
            fx.gameObject.SetActive(true);
            fx.ShowEffectMerge(lv, callback);
        }

        public void ShowGiftBeast(UnityEngine.Vector2 pos, int slot, int id)
        {
            var gift = _queueGift.Dequeue();
            gift.gameObject.SetActive(true);
            gift.SetData(slot, id, pos);
        }

        private void FormatMoney(BigInteger money, bool ignore = false)
        {
            _txtMoneyCoin.text = Utils.FormatNumber(money);
            this.SaveMoney();

            if (!ignore)
                this.CheckInteracBtnSpawn();
        }

        private void OnClickSpawnBeast()
        {
            if (_currentCoin >= _currentPriceBeast)
            {
                GameManager.Instance.OnClickSpawnBeast(this.CallBackBuyBeastDone);

            }

        }

        public void ShowQuickBuy(bool active)
        {
            // btnQuickBuy.interactable = active;

            btnQuickBuy.transform.GetChild(0).gameObject.SetActive(active);
            btnQuickBuy.transform.GetChild(1).gameObject.SetActive(!active);

        }

        private void OnClickQuickBuy()
        {
            if (CPlayer.GetVipConfig().quickBuy)
            {
                int beastSpawned = (int)(_currentCoin / _currentPriceBeast);
                DailyQuestCtrl.Instane.QuickBuyBeast(beastSpawned);
                var arrBeast = Utils.ArrayPower2(beastSpawned);
                int currentBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                for (int i = 0; i < arrBeast.Count; i++)
                {
                    int maxbeast = Mathf.Min(arrBeast[i] + 1, 17);
                    GameManager.Instance.AddGift(maxbeast + currentBeast);
                }
                GameManager.Instance.ReadySpawnGift();
                //GameManager.Instance.
                UpdateMoneyCoin(-(beastSpawned * _currentPriceBeast), false);
            }
            else
            {
                CPlayer.goToVip3 = true;
                ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.VIP);
            }
        }

        private void OnClickAddSoul()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.SHOP_SOUL);
        }

        private void CallBackBuyBeastDone()
        {
            _currentCoin -= _currentPriceBeast;
            this.FormatMoney(_currentCoin);

            DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.QuickBuyBeast, EnumDefine.Mission.GetBeast);

            if (UnityEngine.Random.Range(0, 100) == 5)
            {
                int lvBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                int maxLvBeast = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 0);

                if (AdsManager.Instance.IsLoaded() && maxLvBeast >= lvBeast + 4)
                    ScreenManager.Instance?.ActiveScreen(EnumDefine.SCREEN.BIG_BEAST);
            }
        }

        private void SaveMoney()
        {
            PlayerPrefs.SetString(StringDefine.MONEY_COIN, _currentCoin.ToString());
        }

        public void DisableBtnSpawn()
        {
            _btnSpawnBeast.interactable = _btnAutoMerge.interactable = false;
        }

        public void CheckInteracBtnSpawn()
        {
            _btnSpawnBeast.interactable = _currentCoin > _currentPriceBeast && !_lockSpawn;
            //_btnSpawnBeast.image.color = (_currentCoin > _currentPriceBeast && !_lockSpawn) ? Color.white : new Color(1f, 1f, 1f, 0.8f);
            btnSpawnScriptAnimation.SetActive((_currentCoin > _currentPriceBeast && !_lockSpawn) ? true : false);
        }

        public void LockSpawnBeast(bool isLock)
        {
            _lockSpawn = isLock;
            CheckInteracBtnSpawn();
        }

        public bool IsLockSpawn()
        {
            return _lockSpawn;
        }

        private void OnClickAutoMerge()
        {
            if (!_iconAdsMedalMerge.activeInHierarchy)
            {
                _btnAutoMerge.interactable = false;
                GameManager.Instance.OnClickAutoMerge();
                DailyQuestCtrl.Instane.UpdateQuest(EnumDefine.DailyQuest.UseMedalMerge, EnumDefine.Mission.UseMergeMedal);
            }
            else
            {
                AdsManager.Instance.ShowAds(() =>
                {
                    BoostManager.Instance.AddBoost(EnumDefine.BOOST.FEW_MERGE_MEDAL);
                    ShowNotify($"获得合成勋章");
                }, StringDefine.ADS_RECEIVED_BOOST);
            }
        }

        public BigInteger CurrentCoin
        {
            get { return _currentCoin; }
        }

        public string GetTextMoney()
        {
            return _txtMoneyCoin.text;
        }

        public void MinusCoin(BigInteger coin)
        {
            _currentCoin -= coin;
            this.FormatMoney(_currentCoin);
        }

        public void AddMoneyMerge(int merge = 1)
        {
            Utils.AddStar(merge);
            _totalMerge = PlayerPrefs.GetInt(StringDefine.MONEY_MERGE, 0);
            //_txtMergeCount.text = Utils.FormatNumber(_totalMerge);
        }

        public void CheckNoticeBook()
        {
            int maxLvBeast = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST, 0);
            _objNoticeBook.SetActive(maxLvBeast >= 7);
        }

        public void CheckNoticeBoost()
        {
            _objNoticeBoost.SetActive(true);
        }

        public void CheckAscendNotice(bool show)
        {
            _objNoticeAscend.SetActive(show);
            optionCtl.CheckShowNotice();
        }

        public void ShowNotify(string mgs)
        {
            _txtNotify.text = mgs;
            _animNotify.SetTrigger("show");
        }

        #region OnClick UI Show Screen
        private void OnClickUpgrade()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.UPGRADE);
        }

        private void OnClickBoost()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.BOOST);
        }

        private void OnClickBook()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.BOOK);
        }

        private void OnClickShop()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);

        }

        public void OnClickTooltipEnemy()
        {
            //  ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.TOOLTIP_ENEMY);
            _tooltipEnemy.SetActive(true);
        }

        public void OnUpTooltipEnemy()
        {
            _tooltipEnemy.SetActive(false);
        }

        public void DeActiveScr()
        {
            ScreenManager.Instance.DeActiveScreen();
        }

        private void OnClickShopMerge()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.STAR_SHOP);
        }

        private void OnClickAscend()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.ASCEND);
            OnClickShowOption();
        }

        private void OnClickSetting()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.SETTING);
            OnClickShowOption();
        }

        public void OnClickShowOption()
        {
            _isShowOption = !_isShowOption;
            _animOption.SetBool("show", _isShowOption);
            optionCtl.ShowBtnOverlay(_isShowOption);
            //if (!_isShowOption) {
            //    optionCtl.CheckShowNotice();
            //}            
        }

        public void ShowTooltipBeastDPS(BigInteger dps, int index, UnityEngine.Vector2 pos)
        {
            _rectBeastDPS.gameObject.SetActive(true);
            _rectBeastDPS.pivot = new UnityEngine.Vector2(index <= 2 ? 0 : 1, 0);
            _rectBeastDPS.position = Camera.main.WorldToScreenPoint(pos);
            _txtBeastDPS.text = $"DPS：{Utils.FormatNumber(dps)}";
        }

        public void HideTooltipBeastDPS()
        {
            _rectBeastDPS.gameObject.SetActive(false);
        }

        public void ShowTipUnlockSlot(bool showLeft, int slot, UnityEngine.Vector2 pos)
        {
            _rectTipUnlock.gameObject.SetActive(true);
            _rectTipUnlock.pivot = new UnityEngine.Vector2(showLeft ? 0 : 1, 0);
            _rectTipUnlock.position = Camera.main.WorldToScreenPoint(pos);
            _txtTipUnlock.text = $"你需要先解锁槽位{slot}";
        }

        public void HideTipUnlockSlot()
        {
            _rectTipUnlock.gameObject.SetActive(false);
        }

        public void ActiveShopStar()
        {
            _btnShopMerge.interactable = true;
        }

        public void DeactiveShopStar()
        {
            _btnShopMerge.interactable = false;
        }

        public void ResetSoul()
        {
            _currentCoin = 0;
            this.SaveMoney();
        }

        public void OnUpdatePlayerName()
        {
            _txtPlayerName.text = PlayerPrefs.GetString(StringDefine.KEY_PLAYER_NAME);
        }
        #endregion

        #region Popup Anim
        public void ShowPopup(Transform tf, Action cb = null)
        {
            tf.gameObject.SetActive(true);
            tf.DOScale(1.2f, 0.1f).OnComplete(() =>
            {
                tf.DOScale(1f, 0.1f).OnComplete(() =>
                {
                    if (cb != null) cb();
                });
            });
        }

        public void HidePopup(Transform tf, Action cb = null)
        {
            tf.DOScale(1.2f, 0.1f).OnComplete(() =>
            {
                tf.DOScale(1f, 0.1f).OnComplete(() =>
                {
                    tf.gameObject.SetActive(false);
                    if (cb != null) cb();
                });
            });
        }

        public void ShowLoading(bool active)
        {
            loading.SetActive(active);
        }
        #endregion
    }
}
