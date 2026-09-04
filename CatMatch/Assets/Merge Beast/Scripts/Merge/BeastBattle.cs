using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Observer;
using System.Numerics;

namespace MergeBeast
{
    public class BeastBattle : BeastItem
    {
        [SerializeField] private Image _imgFX;
        [SerializeField] private List<Sprite> _spriteFX;

        private UnityAction<BigInteger, int> _actionAttack;
        private float _timeCountAttack;

        private Animator _myAnim;
        private int _boostDPS = 0;
        private int _boostDPSFree = 0;
        private bool _stopAttack;
        private List<string> _listAnimCat;
    //    private int _ascendDPS;

        // Use this for initialization
        void Awake()
        {
            MyTag = StringDefine.BEAST_BATTLE_INDEX;
        //    _ascendDPS = Utils.GetTotalAscend();
            _myAnim = this.GetComponent<Animator>();
            this.RegisterListener(EventID.OnBoostDPS, (sender, param) => this.OnChangeDPSBoost((int)param));
            this.RegisterListener(EventID.OnBeastBattleMove, (sender, param) => this.OnBeastMoveNewStage((bool)param));
            this.RegisterListener(EventID.OnBoostDpsFree, (sender, param) => this.OnDpsBoostFree((int)param));
            _listAnimCat = new List<string>()
            {
                "Whack A Mouse/Whack_A_Mouse_Left","Whack A Mouse/Whack_A_Mouse_Middle","Whack A Mouse/Whack_A_Mouse_Right"
            };
        }

        // Update is called once per frame
        void Update()
        {
            if (base._myData != null && !_stopAttack)
            {
                _timeCountAttack += Time.deltaTime;
                if (_timeCountAttack >= 1f)
                {
                    _myAnim.SetTrigger("attack");
                    _actionAttack?.Invoke(_myDps, base._myData.ID);
                    _timeCountAttack = 0f;

                    var current = _skelCat.AnimationState.SetAnimation(0, _listAnimCat[Random.Range(0,_listAnimCat.Count)], false);
                    current.Complete += (t) =>
                    {
                        _skelCat.AnimationState.SetAnimation(0, "Sit_Idle", true);
                    };
                }
            }
        }

        public override void OnHandDownBeast(PointerEventData eventData)
        {
            if (!_canMerge) return;
            base.OnHandDownBeast(eventData);
        }

        public override void OnDragBeast(PointerEventData eventData)
        {
            if (!_canMerge) return;
            base.OnDragBeast(eventData);

            if (_firstTouch)
            {
                _skelCat.color = base.COLOR_FADE;
                _skelDrag.transform.localScale = UnityEngine.Vector3.one * 1.3f;
                _firstTouch = false;
            }
        }

        public override void OnHandleUpBeast(PointerEventData eventData)
        {
            if (!_canMerge) return;

            base.OnHandleUpBeast(eventData);

        }

        public override void SpawnBeast(BeastData data)
        {
            base._myData = data;
            this.SetBeast(data);
        }

        public override void SetBeast(BeastData data)
        {
            _myData = data;
            base.SetBeast(data);

            _timeCountAttack = _timeCountAttack == 0 ? Random.Range(-0.5f, 1f) : 0f;
            UpdateMyDPS();

            int index = data.ID % _spriteFX.Count;
            _imgFX.sprite = _spriteFX[index];
            _imgFX.SetNativeSize();

            if (_stopAttack)
            {
                this.BeastMove();
            }
        }

        public override void AddEvtAttack(UnityAction<BigInteger, int> action)
        {
            this._actionAttack = action;
        }

        private void OnChangeDPSBoost(int heso)
        {
            //Debug.LogError("=== change boost shop: " + heso);
            _boostDPS = heso - 1;
            if (base._myData != null)
                UpdateMyDPS();
        }

        private void OnDpsBoostFree(int hs)
        {
            //Debug.LogError("==== change boost free: " + hs);
            _boostDPSFree = hs - 1;
            if (base._myData != null)
                UpdateMyDPS();
        }

        private void UpdateMyDPS()
        {
            if (_myData == null) return;
            //Debug.LogError("boost shop: " + _boostDPS + ", boost free: " + _boostDPSFree);
            //final = dps x (1+boost shop) x (1+ Boost free)
            _myDps = Utils.GetDameByLevelAndAscend(_myData.Level) * (1 + _boostDPS) * (1 + _boostDPSFree);
            //_myDps = Utils.GetDameByLevel(base._myData.Level) * _boostDPS + Utils.GetDameByLevel(base._myData.Level) * Utils.GetTotalAscend() / 100;
        }

        private void OnBeastMoveNewStage(bool newStage)
        {
            StartCoroutine(IEDelayMove(newStage));
        }

        private IEnumerator IEDelayMove(bool newStage)
        {
            _stopAttack = newStage;
            if (newStage)
            {
                if (_myData != null)
                {
                    yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                    this.BeastMove();
                }
            }
        }

        private void BeastMove()
        {
            _skelCat.AnimationState.SetAnimation(0, "Run", true);
        }

        protected override void CallBackAfterMove()
        {
            if (_stopAttack && _myData == null)
            {
                _skelCat.AnimationState.SetAnimation(0, "Sit_Idle", true);
            }
        }

        public void ShowDPS()
        {
            if (_myData == null) return;
            UIManager.Instance?.ShowTooltipBeastDPS(_myDps, MyIndex, Camera.main.ScreenToWorldPoint(GetPositionSlot()));
        }

        public void HideDPS()
        {
            UIManager.Instance?.HideTooltipBeastDPS();
        }

        public void ShowTipUnlock()
        {
            if (PlayerPrefs.GetInt($"{StringDefine.BATTLE_SLOT}{0}") == StringDefine.NULL)
            {
                UIManager.Instance.ShowTipUnlockSlot(false, 0, Camera.main.ScreenToWorldPoint(GetPositionSlot()));
            }
        }

        public void HideTipUnlock()
        {
            UIManager.Instance.HideTipUnlockSlot();
        }

        public override BigInteger RootDPS()
        {
            return Utils.GetDameByLevel(base._myData.Level);
        }

        public override void OnAscendBeast()
        {
            base.OnAscendBeast();
        //    _ascendDPS = Utils.GetTotalAscend();
        }
    }
}
