using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Observer;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


namespace MergeBeast
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private Animator _myAnim;

        private bool _passTut_1;
        private bool _passTut_2;
        private bool _passTut_3;

        [SerializeField] private List<BeastItem> _slotBeasts;
        [SerializeField] private Canvas _myCanvas;
        [SerializeField] private Canvas _canvas_beast_1;
        [SerializeField] private Canvas _canvas_beast_2;
        [SerializeField] private Canvas _canvas_battle;
        [SerializeField] private Canvas _canvas_spawn;
        [SerializeField] private GameObject _objSlotFreeBoost;

        [SerializeField] private Text _txtMgs;
        [SerializeField] private Button _btnFakeBoost;
        [SerializeField] private Button _btnFakeDps;
        [SerializeField] private Button _btnClose;
        [SerializeField] private GameObject _objContinue;



        private bool _canNextAction;
        private UnityAction _actionNextTut;


        // Use this for initialization
        void Awake()
        {
        //    _myAnim = this.GetComponent<Animator>();
            this.RegisterListener(EventID.OnCompleteTutorialStep2, (sender, param) => OnCompleteTut2());
            this.RegisterListener(EventID.OnTutDoneMergeLv2, (sender, param) => PGAfterMergeLv2());
            this.RegisterListener(EventID.OnTutDoneMergeLv3, (sender, param) => OnDoneMergeLv3());
            this.RegisterListener(EventID.OnDeactiveHand, (sender, param) => OnDeactiveAnim());
            this.RegisterListener(EventID.OnDoneTutMap, (sender, param) => Step3());

            _btnFakeBoost?.onClick.AddListener(this.OnClickFakeBoost);
            _btnFakeDps?.onClick.AddListener(this.OnClickFakeDPS);
            _btnClose?.onClick.AddListener(this.OnClickFakeClose);

        }

        private void OnEnable()
        {
            GameManager.Instance.StopAutoSpawnBeast(true);
            _btnClose.gameObject.SetActive(false);
            _btnFakeBoost.gameObject.SetActive(false);
            _btnFakeDps.gameObject.SetActive(false);
            
        }

        private void Step1()
        {
            StartCoroutine(IEWriteMsg(StringDefine.MGS_1));
            _actionNextTut = Step2;
            
        }

        private void Step2()
        {
            StartCoroutine(IEWriteMsg(StringDefine.MGS_2));
            _actionNextTut = Step3;
        }

        private void Step3()
        {
            this.OnTutorialSpawn();
            _txtMgs.transform.parent.gameObject.SetActive(false);
        }

        private void Step4()
        {
            StartCoroutine(IEWriteMsg(StringDefine.MGS_3));
            _actionNextTut = OnTutMerge;
            _myAnim.gameObject.SetActive(false);

            for (int i = 0; i < _slotBeasts.Count; i++)
                _slotBeasts[i].enabled = false;
        }

        private void Step5()
        {
            _canvas_battle.sortingOrder = 70;
            _myAnim.transform.position = _canvas_beast_2.transform.position;
            _myAnim.gameObject.SetActive(true);
            _myAnim.Play("Tutorial-battle-1");
            _txtMgs.transform.parent.gameObject.SetActive(false);
        }

        private void Step6()
        {
            StartCoroutine(IEWriteMsg(StringDefine.MGS_6));
            _actionNextTut = Step7;
        }

        private void Step7()
        {
            GameManager.Instance.AddGift(1);
            GameManager.Instance.ReadySpawnGift();            
            _txtMgs.transform.parent.gameObject.SetActive(false);
            _myAnim.gameObject.SetActive(true);
            _myAnim.transform.position = _canvas_beast_1.transform.position;
            _myAnim.Play("Tutorial-battle-2");

        }

        private void Step8()
        {
            GameManager.Instance.isFirstTime = false;
            StartCoroutine(IEWriteMsg(StringDefine.MGS_8));
            _actionNextTut = Step9;
        }

        private void Step9()
        {
            StartCoroutine(IEWriteMsg(StringDefine.MGS_9));
            _actionNextTut = Step10;
        }

        private void Step10()
        {
            _txtMgs.transform.parent.gameObject.SetActive(false);
            _btnFakeBoost.gameObject.SetActive(true);
            _myAnim.gameObject.SetActive(true);
            _myAnim.transform.position = _btnFakeBoost.transform.position ;
            _myAnim.Play("Tutorial-Spawn");
        }

        private void OnDoneMergeLv3()
        {
            _myAnim.gameObject.SetActive(false);
            for (int i = 0; i < _slotBeasts.Count; i++)
                _slotBeasts[i].enabled = true;
            StartCoroutine(IEWriteMsg(StringDefine.MGS_7));
            _actionNextTut = Step8;
            GameManager.Instance.StopAutoSpawnBeast(false);
        }

        private void OnDeactiveAnim()
        {
            _myAnim.gameObject.SetActive(false);
        }

        public void OnClickNextTutorial()
        {
            if (!_canNextAction) return;
            _actionNextTut?.Invoke();
        }

        private IEnumerator IEWriteMsg(string msg)
        {
            _txtMgs.transform.parent.gameObject.SetActive(true);
            _objContinue.SetActive(false);
            _canNextAction = false;
            string str = string.Empty;
            for(int i = 0; i < msg.Length; i++)
            {
                str += msg[i];
                _txtMgs.text = str;
                yield return new WaitForSeconds(0.01f);
            }
            _canNextAction = true;
            _objContinue.SetActive(true);
        }

        private void OnTutorialSpawn()
        {
            _myAnim.transform.position = _canvas_spawn.transform.position;
            _myAnim.Play("Tutorial-Spawn");
            _myCanvas.overrideSorting = _canvas_battle.overrideSorting = _canvas_beast_1.overrideSorting = _canvas_beast_2.overrideSorting = true;
            _canvas_spawn.overrideSorting = true;
            _objSlotFreeBoost.SetActive(false);
        }

        private void OnCompleteTut2()
        {
            _passTut_2 = true;
            _myAnim.gameObject.SetActive(false);
            
        }

        private void PGAfterMergeLv2()
        {
            StartCoroutine(IEWriteMsg(StringDefine.MGS_4));
            _actionNextTut = Step5;
        }

        private void OnTutMerge()
        {
            _myAnim.gameObject.SetActive(true);
            _txtMgs.transform.parent.gameObject.SetActive(false);

            _myAnim.transform.position = _canvas_beast_1.transform.position;
            _myAnim.Play("Tutorial-Merge");
            _myCanvas.sortingOrder = 60;
            _canvas_beast_1.sortingOrder = _canvas_beast_2.sortingOrder = 65;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_passTut_1)
            {
                if (GameManager.Instance.CheckBeastSlot())
                {
                    this.Step4();
                    _passTut_1 = true;
                }
            }

            if (GameManager.Instance.CheckCompleteTutorial() && !_passTut_3)
            {
                _passTut_3 = true;
                _objSlotFreeBoost.SetActive(true);
                _myAnim.gameObject.SetActive(false);
                _myCanvas.overrideSorting = _canvas_battle.overrideSorting = _canvas_beast_1.overrideSorting = _canvas_beast_2.overrideSorting = false;
                StartCoroutine(IEWriteMsg(StringDefine.MGS_5));
                _actionNextTut = Step6;
            }
        }

        private void OnClickFakeBoost()
        {
            ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.BOOST);
            _myAnim.transform.position = _btnFakeDps.transform.position ;
            _btnFakeDps.gameObject.SetActive(true);
            _btnFakeBoost.gameObject.SetActive(false);
        }

        private void OnClickFakeDPS()
        {
            BoostManager.Instance.AddBoost(EnumDefine.BOOST.DAMAGE_BOOST_1);
            UIManager.Instance.ShowNotify("获得DPS增益");
            _myAnim.transform.position = _btnClose.transform.position ;
            _btnClose.gameObject.SetActive(true);
            _btnFakeDps.gameObject.SetActive(false);
        }

        private void OnClickFakeClose()
        {
            ScreenManager.Instance.DeActiveScreen();
            _myAnim.gameObject.SetActive(false);
            _btnClose.gameObject.SetActive(false);
        }
    }
}
