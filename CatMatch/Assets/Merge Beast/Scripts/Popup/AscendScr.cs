using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using Observer;
using DG.Tweening;

namespace MergeBeast
{
    public class AscendScr : BaseScreen
    {
        [Header("Info")]
        [SerializeField] private Text _txtTotalPercentDPS;
        [SerializeField] private Text _txtRewardDPS;
        [SerializeField] private Text _txtRewardSoul;
        [SerializeField] private Button _btnAscend;
        [SerializeField] private Button _btnAscendByGem;
        [SerializeField] private Button _btnAscendByItem;

        [Header("Popup")]
        [SerializeField] private Animator _animPopupInfo;
        [SerializeField] private Animator _animPopupConfirm;
        [SerializeField] private Button _btnOK;
        [SerializeField] private Button _btnCancel;
        [SerializeField] private GameObject _pnWheelReward;

        [Header("Reward")]
        [SerializeField] private Animator _animPopupReward;
        [SerializeField] private Text _txtDpsreward;
        [SerializeField] private Text _txtSoulReward;
        [SerializeField] private Button _btnRewardOK;
        [SerializeField] private Text _txtTooltipReward;
        [SerializeField] private Text _txtMinimumLv;

        [Header("Particle")]
        [SerializeField] GameObject particleTransition;



        private int _levelMonster;
        private bool _ascendByGem;
        private BigInteger _soulReward;
        private Animator btnAscendAnim;

        // Start is called before the first frame update
        void Start()
        {
            _btnAscend?.onClick.AddListener(() => this.OnClickAscend(false));
            _btnAscendByGem?.onClick.AddListener(() => this.OnClickAscend(true));
            _btnOK?.onClick.AddListener(this.OnClickOKAscend);
            _btnCancel?.onClick.AddListener(this.OnClickCancelAscend);
            _btnRewardOK?.onClick.AddListener(this.OnClickShowWheel);
            
        }

        private void Awake() {
            btnAscendAnim = _btnAscend.gameObject.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            particleTransition.SetActive(false);
            UIManager.Instance.CheckAscendNotice(false);

            _levelMonster = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER, 0) + 1;
            int currentLvBeast = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 29) + 1;
            int oldLv = PlayerPrefs.GetInt(StringDefine.OLD_LEVEL_ASCEND, 29) + 1;
  
            bool canAscend = _levelMonster >= 30 && _levelMonster >= oldLv;
            _btnAscend.interactable = _btnAscendByGem.interactable = _btnAscendByItem.interactable = canAscend;
            _txtTotalPercentDPS.text = Utils.GetTotalAscend() + "%";
            _txtRewardDPS.text = "DPS提升："+ ( Utils.GetDPSAscend(_levelMonster) + Mathf.FloorToInt(Utils.GetDPSAscend(_levelMonster) * CPlayer.GetVipConfig().dpsBuffAscend / 100)) + "%";
            //Debug.LogError("lv monster: " + _levelMonster + ", old: " + oldLv);
            _soulReward = BigInteger.Pow(2, (_levelMonster >= oldLv ? _levelMonster : oldLv) - 10);

            //luong soul nhan dc se = oldLV, cai nay se luon <= _soulReward
            _txtRewardSoul.text =  "灵魂奖励：" + Utils.FormatNumber(_soulReward);
            _txtSoulReward.text = Utils.FormatNumber(_soulReward);
            _txtDpsreward.text = $"<color=#D7BD3A>{Utils.GetTotalAscend()}</color>% + <color=#3AD640>{Utils.GetDPSAscend(oldLv)}</color>%";
            _animPopupInfo.Play("infoAscend-show");
            _txtMinimumLv.text = $"你必须达到{oldLv}级才能转生";

            btnAscendAnim.SetBool("scale", canAscend);
        }

        private void OnClickAscend(bool isGem)
        {
            _ascendByGem = isGem;
            _animPopupConfirm.gameObject.SetActive(true);
            _animPopupConfirm.Play("confirmAscend-show");
        }

        private void OnClickOKAscend()
        {
            if (_ascendByGem)
            {
                if(Utils.GetCurrentRubyMoney() <= 50)
                {
                    UIManager.Instance.ShowNotify($"宝石不足。");
                    return;
                }
                else
                {
                    Utils.AddRubyCoin(-50);
                    this.PostEvent(EventID.OnUpDateMoney);
                }
            }


            UIManager.Instance?.ResetSoul();
            int percentDPS = Utils.GetDPSAscend(_levelMonster) * (_ascendByGem ? 2 : 1);
            percentDPS += Mathf.RoundToInt(percentDPS * CPlayer.GetVipConfig().dpsBuffAscend / 100); //buff them dps neu la vip

            BigInteger rwSoul = _soulReward;
            UIManager.Instance?.UpdateMoneyCoin(rwSoul, false);
            _txtDpsreward.text = $"<color=#D7BD3A>{Utils.GetTotalAscend()}</color>% + <color=#3AD640>{percentDPS}</color>%";
            Utils.SetTotalAscend(percentDPS);
            this.PostEvent(EventID.OnAscend);
            UIManager.Instance.ShowNotify($"转生等级{_levelMonster}成功");

            _animPopupConfirm.Play("confirmAscend-hide");
            StartCoroutine(IEShowRewardAscend());
            PlayerPrefs.SetInt(StringDefine.OLD_LEVEL_ASCEND, _levelMonster);
            _btnAscend.interactable = _btnAscendByGem.interactable = _btnAscendByItem.interactable = false;
            DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.Ascend, EnumDefine.Mission.Ascend);
            UIManager.Instance.ActiveStarShopWhenAscend();
            GameManager.Instance.ResetRewardWhenAscend();
            btnAscendAnim.SetBool("scale", false);

            particleTransition.SetActive(true);
        }

        IEnumerator _HideParticle() {
            yield return new WaitForSeconds(3f);
            particleTransition.SetActive(false);
        }

        private void OnClickShowWheel()
        {
            _animPopupReward.Play("infoAscend-hide");
            StartCoroutine(IEShowWheel());
        }

        private void OnClickCancelAscend()
        {
            _animPopupConfirm.Play("confirmAscend-hide");
            StartCoroutine(IEDeactivePopup());
        }

        private IEnumerator IEDeactivePopup()
        {
            yield return new WaitForSeconds(0.4f);
            _animPopupConfirm.gameObject.SetActive(false);
        }

        private IEnumerator IEDeactiveMe()
        {
            yield return new WaitForSeconds(0.4f);
            ScreenManager.Instance.DeActiveScreen();
        }

        private IEnumerator IEShowRewardAscend()
        {
            yield return new WaitForSeconds(0.45f);
            _animPopupConfirm.gameObject.SetActive(false);
            _animPopupReward.gameObject.SetActive(true);
            _animPopupReward.Play("infoAscend-show");
        }

        private IEnumerator IEShowWheel()
        {
            yield return new WaitForSeconds(0.45f);
            _animPopupReward.gameObject.SetActive(false);
            _pnWheelReward.SetActive(true);
        }

        public void OnClickClose()
        {
            _animPopupInfo.Play("infoAscend-hide");
            StartCoroutine(IEDeactiveMe());
        }

        public void ShowToolTipReward(string mgs)
        {
            _txtTooltipReward.transform.parent.gameObject.SetActive(true);
            _txtTooltipReward.text = mgs;
        }

        public void HideTooltip()
        {
            _txtTooltipReward.transform.parent.gameObject.SetActive(false);
        }

    }
}
