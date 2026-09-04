using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;
using System.Numerics;
using DG.Tweening;
using Spine.Unity;

namespace MergeBeast
{
    public class NewBeastScr : BaseScreen
    {
        [SerializeField] private Text _txtLevel;
        [SerializeField] private Text _txtName;
        [SerializeField] private Text _txtDmg;
        [SerializeField] private Text _txtRewardSoul;
        [SerializeField] private Text _txtRewardGem;

        [SerializeField] private SkeletonGraphic _imgOldBeast1;
        [SerializeField] private SkeletonGraphic _imgOldBeast2;
        [SerializeField] private SkeletonGraphic _imgCurrentBeast;
        [SerializeField] private SkeletonGraphic _imgNextBeast;

        [SerializeField] private UIButton _btnYay;
        [SerializeField] private Animator _animMerge;

        [SerializeField] private UIButton _btnClose;
        [SerializeField] private UIButton _btnX2;

        [SerializeField] private Transform _fxLigh1;
        [SerializeField] private Transform _fxLigh2;

        private BigInteger _soulReward;
        private bool _hasReward;
        private int currentBeast;
        private List<string> _listAnim = new List<string>() { "Level_Up", "Level_Up2", "Level_Up3", "Level_Up4", "Level_Up5" };

        // Use this for initialization
        void Start()
        {
            _btnClose?.onClick.AddListener(this.OnClickClose);
            _btnX2?.onClick.AddListener(this.OnClickX2);
        }

        private void OnClickClose()
        {
            if (!_hasReward)
                this.Reward();
            ScreenManager.Instance.DeActiveScreen();
            //show popup rate app            
            if ((currentBeast + 1) % 10 == 0)
            {
                if (PlayerPrefs.GetInt(StringDefine.RATE_SUCCESS, 0) == 0)
                {
                    ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.RATING);
                }
            }

            if (currentBeast == 1) this.PostEvent(EventID.OnTutDoneMergeLv2);
            else if (currentBeast == 2) this.PostEvent(EventID.OnTutDoneMergeLv3);

        //    ShowTut();
        }

        //private void Update()
        //{
        //    if (this.gameObject.activeInHierarchy)
        //    {
        //        _fxLigh1.Rotate(UnityEngine.Vector3.forward * 30 * Time.deltaTime);
        //        _fxLigh2.Rotate(UnityEngine.Vector3.forward * -30f * Time.deltaTime);
        //    }
        //}

        private void OnEnable()
        {
            StartCoroutine(IEOnEnable());
        }

        private IEnumerator IEOnEnable()
        {
            while (_imgCurrentBeast == null || _imgOldBeast1 == null || _imgOldBeast2 == null)
                yield return null;

            int curBeast = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST);
            currentBeast = curBeast;
            var curData = GameManager.Instance.GetBeast(curBeast);

            yield return new WaitForEndOfFrame();
            _imgCurrentBeast.Skeleton.SetSkin(curData.Level.ToString("D3"));
            _imgCurrentBeast.AnimationState.SetAnimation(0, _listAnim[Random.Range(0, _listAnim.Count)], true);

            _txtLevel.text = $"等级 {curData.Level}";
            _txtName.text = curData.Name;
            _soulReward = 500 * BigInteger.Pow(2, curData.Level - 1);
            _txtRewardSoul.text = Utils.FormatNumber(_soulReward);
            _txtRewardGem.text = $"{GameManager.Instance.levelBeastDiff * 5}";

            BigInteger dmg = Utils.GetDameByLevel(curData.Level);
            _txtDmg.text = $"{Utils.FormatNumber(dmg)}<color=#FFC000FF>/秒</color>";

            var nextData = GameManager.Instance.GetBeast(curBeast + 1);
            if (nextData != null)
            {
                _imgNextBeast.Skeleton.SetSkin(nextData.Level.ToString("D3"));
            }
            var oldData = GameManager.Instance.GetBeast(curBeast - 1);
            _imgOldBeast1.Skeleton.SetSkin(oldData.Level.ToString("D3"));
            _imgOldBeast2.Skeleton.SetSkin(oldData.Level.ToString("D3"));

            _animMerge.SetTrigger("merge");

            //    this.Reward();

            PoolManager.Instance.PlayFXBeastMerge(curBeast, UnityEngine.Vector2.zero);
            _btnYay.gameObject.SetActive(false);
            _btnX2.gameObject.SetActive(false);
            Invoke("ActiveYay", 1f);

            if (curBeast == 1)
            {
                this.PostEvent(EventID.OnCompleteTutorialStep2);
            }
            else if (curBeast == 2)
            {
                this.PostEvent(EventID.OnDeactiveHand);
            }

            SoundManager.Instance?.PlaySound(EnumDefine.SOUND.BEAST_NEW);

            if (AdsManager.Instance.IsLoaded()) UIManager.Instance.CheckNoticeBook();
            if (curBeast >= 9) UIManager.Instance?.ActiveShopStar();
            _hasReward = false;
            DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.None, EnumDefine.Mission.UnlockBeast);
            PlayerPrefs.SetInt(StringDefine.HIGHER_LV_BEAST_ASCEND, curBeast);

            _fxLigh1.gameObject.SetActive(false);
            _fxLigh2.gameObject.SetActive(false);

            // Log an event with multiple parameters, passed as an array:

            //Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //  "new_cat",
            //  new Firebase.Analytics.Parameter(
            //    "level_cat", curBeast));
        }

        private void ActiveYay()
        {
            _btnYay.gameObject.SetActive(true);
            _btnX2.gameObject.SetActive(true);
        }

        private void OnClickX2()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                Utils.AddRubyCoin(10);
                UIManager.Instance.UpdateMoneyCoin(_soulReward * 2, false);
                this.PostEvent(EventID.OnUpDateMoney);
                _btnX2.gameObject.SetActive(false);

                long myGem = 10;
                DOTween.To(() => myGem, (x) =>
                {
                    myGem = x;
                    _txtRewardGem.text = myGem.ToString();
                }, 10, 0.5f);

                _txtRewardSoul.text = Utils.FormatNumber(_soulReward * 2);
                _hasReward = true;

            //    ShowTut();
            }, StringDefine.ADS_RECEIVED_NEW_BEAST);
        }

        private void Reward()
        {
            Utils.AddRubyCoin(GameManager.Instance.levelBeastDiff * 5);
            UIManager.Instance.UpdateMoneyCoin(_soulReward, false);
            this.PostEvent(EventID.OnUpDateMoney);

            _hasReward = true;
        }

        void ShowTut()
        {
            var curData = GameManager.Instance.GetBeast(currentBeast);
            if (curData.Level == 15)
            {
                TutorialController.Instance.ButtonWorldShow();
            }
        }
    }
}
