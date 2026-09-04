using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using Observer;
using System;

namespace MergeBeast
{
    public class MapController : MonoBehaviour
    {
        public static MapController _Instance;

        [SerializeField] private List<UIButton> _listBtnMaps;
        [SerializeField] private Transform _arrowHere;
        [SerializeField] private Sprite _sprMapDark;
        [SerializeField] private Sprite _sprMapLight;
        [SerializeField] private Sprite _sprMapCurent;

        [SerializeField] private List<Transform> _listRegion;
        [SerializeField] private Transform _cloud;
        [SerializeField] private UIButton _btnBack;

        [SerializeField] private Text _txtMgs;
        [SerializeField] private GameObject _objContinue;
        [SerializeField] private GameObject _handTap;
        [SerializeField] private GameObject _btnFakeLv;

        public RectTransform listMonsterPanel;

        public StagePanel stagePanel;
        public ChapterRewardPanel chapterRewardPanel;

        public ShopPanel shopPanel;
        public PanelReceive2 receivePanel;

        public Image redDotIcon;

        private bool _canNextAction;
        private UnityAction _actionNextTut;

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
            _txtMgs.transform.parent.gameObject.SetActive(false);
            _handTap.SetActive(true);
            _btnFakeLv.SetActive(true);
        }

        public void OnClickLevlFake()
        {
            this.PostEvent(EventID.OnDoneTutMap);
            _btnBack?.onClick.Invoke();
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
            for (int i = 0; i < msg.Length; i++)
            {
                str += msg[i];
                _txtMgs.text = str;
                yield return new WaitForSeconds(0.01f);
            }
            _canNextAction = true;
            _objContinue.SetActive(true);
        }

        private void Awake()
        {
            if (_Instance == null) _Instance = this;
            _btnBack?.onClick.AddListener(this.OnClickBack);
            // this.RegisterListener(EventID.BackToMap, (sender, param) => ShowRegions());
        }

        private void OnEnable()
        {
            if (AdsManager.Instance != null) AdsManager.Instance.onInterestial = false;
            ShowRegions();

            // if(PlayerPrefs.GetInt("tutorial-map",0) == 0)
            // {
            //     Step1();
            //     _btnBack.enabled = false;
            //     PlayerPrefs.SetInt("tutorial-map", 10);
            // }
            SoundManager.Instance?.MuteMainGame(true);
            AudioManager.instance?.StartCrush();
        }

        public void ShowRegions()
        {
            if (redDotIcon != null) redDotIcon.gameObject.SetActive(RewardHelper.CheckHasChapterRewards());

            int maxStage = PlayerData.instance.MaxStage;
            int currentRegion = (maxStage - 1) / PlayerData.instance.MaxStagePerRegion;
            Debug.Log(currentRegion);

            for (int i = 0; i < _listBtnMaps.Count; i++)
            {
                var regionModel = _listBtnMaps[i].GetComponent<RegionModel>();
                regionModel.region = i;
                regionModel.starsGo.SetActive(true);
                _listBtnMaps[i].onClick.RemoveAllListeners();

                if (i == currentRegion)
                {
                    regionModel.starAmountTxt.text = GetStatStar(i);

                    _listBtnMaps[i].interactable = true;
                    _listBtnMaps[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprMapCurent;
                    _listBtnMaps[i].onClick.AddListener(() => { this.OnClickRegion(regionModel); });

                    _arrowHere.transform.position = _listBtnMaps[i].transform.position;

                    int regionCloud = currentRegion / 3;
                    for (int j = 0; j < _listRegion.Count; j++)
                    {
                        if (j == regionCloud)
                        {
                            _cloud.gameObject.SetActive(true);
                            _cloud.SetParent(_listRegion[j]);
                            _cloud.transform.localPosition = Vector3.zero;
                        }
                    }
                }
                else if (i < currentRegion)
                {
                    regionModel.starAmountTxt.text = GetStatStar(i);

                    _listBtnMaps[i].interactable = true;
                    _listBtnMaps[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprMapLight;
                    _listBtnMaps[i].onClick.AddListener(() => { this.OnClickRegion(regionModel); });
                }
                else
                {
                    _listBtnMaps[i].interactable = false;
                    _listBtnMaps[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprMapDark;
                    regionModel.starsGo.SetActive(false);
                }
            }

            stagePanel.gameObject.SetActive(false);

            // TutorialMap.Instance.OnClickShopBtn();
        }

        private string GetStatStar(int region)
        {
            var maxStageOfRegion = (region + 1) * PlayerData.instance.MaxStagePerRegion;
            var minStageOfRegion = region * PlayerData.instance.MaxStagePerRegion + 1;

            var myStagePair = PlayerData.instance.GetMyStagePair();

            var maxStar = PlayerData.instance.MaxStagePerRegion * 3;
            var totalStar = 0;

            var max = maxStageOfRegion > PlayerData.instance.MaxStage ? PlayerData.instance.MaxStage : maxStageOfRegion;
            for (int k = minStageOfRegion; k <= max; k++)
            {
                if (myStagePair.ContainsKey(k))
                {
                    totalStar += myStagePair[k].numStar;
                }
            }
            return totalStar + "/" + maxStar;
        }

        public void OnClickRegion(RegionModel regionModel)
        {
            Debug.Log("region:" + regionModel.region);

            stagePanel.gameObject.SetActive(true);
            stagePanel.Init(regionModel.region);
            PoolManager.Instance?.gameObject.SetActive(false);
            // SoundManager.Instance.MuteMainGame(true);
        }

        public void OnClickRegion(int i)
        {
            var regionModel = _listBtnMaps[i].GetComponent<RegionModel>();
            Debug.Log("region:" + regionModel.region);

            stagePanel.gameObject.SetActive(true);
            stagePanel.Init(regionModel.region);
            PoolManager.Instance?.gameObject.SetActive(false);
            // SoundManager.Instance.MuteMainGame(true);
        }

        private void OnClickBack()
        {
            if (AdsManager.Instance != null) AdsManager.Instance.onInterestial = true;

            PoolManager.Instance?.gameObject.SetActive(true);
            SoundManager.Instance.MuteMainGame(false);
            AudioManager.instance.StopAll();
            this.PostEvent(EventID.BackToMonsterList);
            this.PostEvent(EventID.BackFromMap);

            Scene map = SceneManager.GetSceneByName(StringDefine.SCENE_MAP);
            if (map.isLoaded)
            {
                SceneManager.UnloadScene(map);
                if (UIManager.Instance.shouldLoadEnemy)
                {
                    FindObjectOfType<EnemyManager>().BackFromMap();
                }
            }
        }

        public void OnClickChapterRewardBtn()
        {
            chapterRewardPanel?.gameObject.SetActive(true);
        }

        public void OnClickShop()
        {
            shopPanel.gameObject.SetActive(true);
            // receivePanel.gameObject.SetActive(true);    
            // this.PostEvent(EventID.OnClickShopTut);
        }

        public void OnClickListMonster()
        {
            listMonsterPanel.gameObject.SetActive(true);
        }
    }
}