using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using UnityEngine.UI;
using Observer;
using DG.Tweening;

namespace MergeBeast {
    public class AscendWheelScr : MonoBehaviour {
        [SerializeField] private List<AscendRewardItem> _listReward;
        [SerializeField] private Button _btnWheel;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Image _imgIconReward;
        [SerializeField] private Text _txtReward;
        [SerializeField] private Animator _animWheel;

        [SerializeField] private Text _txtTooltip;
        [SerializeField] AscendWheelReward reward;


        private List<int> _listPostCanReward;
        private int _cacheIndexReward;

        private Coroutine introWheel;
        // Start is called before the first frame update
        void Awake() {
            //_listPostCanReward = new List<int>() { PlayerPrefs.GetInt(StringDefine.ASCEND_REWARD, 1) };
            _listPostCanReward = new List<int>() { 1, 2, 3, 6, 9, 14 };
            _btnWheel?.onClick.AddListener(this.OnClickWheel);
            _btnClose?.onClick.AddListener(this.OnClickHidePopup);
        }

        private void OnEnable() {
            _btnWheel.interactable = true;
            int lvMonster = PlayerPrefs.GetInt(StringDefine.OLD_LEVEL_ASCEND, 0);
            int ran = Random.Range(0, _listPostCanReward.Count);
            _cacheIndexReward = _listPostCanReward[ran];

            for (int i = 0; i < _listReward.Count; i++) {
                _listReward[i].ResetState();
                _listReward[i]._showToolTip = ShowToolTip;
                _listReward[i]._hideTooltip = CloseToolTip;

                switch (i) {
                    case 1:
                    BigInteger soul = BigInteger.Pow(2, lvMonster - 10);
                    string sl = Utils.FormatNumber(soul);
                    _listReward[i].TxtSl.text = $"x{sl}";
                    _listReward[i].SetDescription($"获得{sl}灵魂");

                    if (_cacheIndexReward == 1) {
                        UIManager.Instance.UpdateMoneyCoin(soul, false);
                        _imgIconReward.sprite = _listReward[i].transform.GetChild(3).GetComponent<Image>().sprite;
                        _txtReward.text = $"x{sl}";
                    }
                    break;
                    case 0:
                    _listReward[i].SetDescription($"DPS提升2倍，持续{lvMonster / 5}分钟");
                    break;
                    case 2:
                    int star = 30 * lvMonster / 5;
                    _listReward[i].TxtSl.text = $"x{star}";
                    _listReward[i].SetDescription($"获得{star}颗星");

                    if (_cacheIndexReward == 2) {
                        UIManager.Instance.AddMoneyMerge(star);
                        _imgIconReward.sprite = _listReward[i].transform.GetChild(3).GetComponent<Image>().sprite;
                        _txtReward.text = $"x{star}";
                    }
                    break;
                    case 3:
                    int gem = 2 * lvMonster / 5;
                    _listReward[i].TxtSl.text = $"x{gem}";
                    _listReward[i].SetDescription($"获得{gem}宝石");
                    if (_cacheIndexReward == 3) {
                        Utils.AddRubyCoin(gem);
                        this.PostEvent(EventID.OnUpDateMoney);
                        _imgIconReward.sprite = _listReward[i].transform.GetChild(3).GetComponent<Image>().sprite;
                        _txtReward.text = $"x{gem}";
                    }
                    break;
                    case 4:
                    _listReward[i].SetDescription($"获得时间跳跃2小时");
                    break;
                    case 5:
                    _listReward[i].SetDescription($"获得时间跳跃8小时");
                    break;
                    case 6:
                    BigInteger sol = BigInteger.Pow(2, lvMonster - 12);
                    string slu = Utils.FormatNumber(sol);
                    _listReward[i].TxtSl.text = $"x{slu}";
                    _listReward[i].SetDescription($"获得{slu}灵魂");

                    if (_cacheIndexReward == 6) {
                        UIManager.Instance.UpdateMoneyCoin(sol, false);
                        _imgIconReward.sprite = _listReward[i].transform.GetChild(3).GetComponent<Image>().sprite;
                        _txtReward.text = $"x{sol}";
                    }
                    break;
                    case 7:
                    _listReward[i].SetDescription($"DPS提升5倍，持续{lvMonster / 10}分钟");
                    break;
                    case 8:
                    _listReward[i].SetDescription($"获得离线加速，持续{lvMonster}分钟");
                    break;
                    case 9:
                    int slMerge = 10 * lvMonster / 5;
                    _listReward[i].TxtSl.text = $"x{slMerge}";
                    _listReward[i].SetDescription($"获得{slMerge}合成勋章");
                    if (_cacheIndexReward == 9) {
                        BoostManager.Instance?.OnBoostMedalMerge(slMerge);
                        _imgIconReward.sprite = _listReward[i].transform.GetChild(3).GetComponent<Image>().sprite;
                        _txtReward.text = $"x{slMerge}";
                    }
                    break;
                    case 10:
                    _listReward[i].SetDescription($"转生等级提升2倍");
                    break;
                    case 11:
                    _listReward[i].SetDescription($"获得时间跳跃6小时");
                    break;
                    case 12:
                    _listReward[i].SetDescription($"生成加速，持续{lvMonster / 5}分钟");
                    break;
                    case 13:
                    _listReward[i].SetDescription($"获得自动合成增益，持续{lvMonster / 5}分钟");
                    break;
                    case 14:
                    int gems = lvMonster / 5;
                    _listReward[i].TxtSl.text = $"x{gems}";
                    _listReward[i].SetDescription($"获得{gems}宝石");
                    if (_cacheIndexReward == 14) {
                        Utils.AddRubyCoin(gems);
                        UIManager.Instance.UpdateMoneyRuby();
                        _imgIconReward.sprite = _listReward[i].transform.GetChild(3).GetComponent<Image>().sprite;
                        _txtReward.text = $"x{gems}";
                    }
                    break;
                    case 15:
                    _listReward[i].SetDescription($"获得更高等级生成增益");
                    break;
                    default: break;
                }
            }
            _animWheel.Play("infoAscend-show");
            _isIDLE = true;
            if (introWheel == null) {
                introWheel = StartCoroutine(IEWheelIDLE());
            }
        }

        private void OnClickWheel() {
            StartCoroutine(IEWheel());
            _btnWheel.interactable = false;
        }

        private void OnClickHidePopup() {
            _animWheel.Play("infoAscend-hide");
            StartCoroutine(IEHidelPopup());
        }

        private IEnumerator IEHidelPopup() {
            yield return new WaitForSeconds(0.4f);
            reward.gameObject.SetActive(true);
            this.gameObject.SetActive(false);

        }

        private bool _isIDLE;
        private IEnumerator IEWheelIDLE() {
            while (_isIDLE) {
                for (int i = 0; i < _listReward.Count; i++) {
                    _listReward[i].ImgSelected.color = Color.white;

                    if (i == 0) _listReward[_listReward.Count - 1].ResetState();
                    else _listReward[i - 1].ResetState();

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        private IEnumerator IEWheel() {
            _isIDLE = false;
            if (introWheel != null) StopCoroutine(introWheel);
            int wheelCount = Random.Range(1, 4) * _listReward.Count + _cacheIndexReward;
            int count = 0;

            while (count <= wheelCount) {
                int mainIndex = count % _listReward.Count;
                for (int i = 0; i < _listReward.Count; i++) {
                    int idex = ((_listReward.Count + mainIndex - i) % _listReward.Count);
                    if (count - i >= 0 && i < 5) {

                        _listReward[idex].ImgLight.color = _listReward[idex].ImgLight2.color = new Color(1f, 1f, 1f, 1f - i * 0.25f);
                        _listReward[idex].ImgSelected.color = new Color(1f, 1f, 1f, 1f - 0.1f * i);
                    } else {
                        _listReward[idex].ResetState();
                    }
                }
                count++;
                yield return new WaitForSeconds(0.05f);
            }

            for (int i = 4; i > 0; i--) {
                int idex = ((_listReward.Count + _cacheIndexReward - i) % _listReward.Count);
                _listReward[idex].ResetState();

                yield return new WaitForSeconds(0.05f);
            }

            _listReward[_cacheIndexReward].SetSelected();

            yield return new WaitForSeconds(1.3f);
            reward.gameObject.SetActive(true);
        }


        // Update is called once per frame


        private void ShowToolTip(string des) {
            _txtTooltip.transform.parent.gameObject.SetActive(true);
            _txtTooltip.text = des;
        }
        private void CloseToolTip() {
            _txtTooltip.transform.parent.gameObject.SetActive(false);
        }
    }
}
