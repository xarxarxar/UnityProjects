using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;
using System.Numerics;
using System.Linq;
using System;

namespace MergeBeast
{
    public class BookScreen : BaseScreen, IBookEnemy
    {
        [Header("Config")]
        [SerializeField] private BeastConfig _beastConfig;
        [SerializeField] private MonsterConfig _monsterConfig;

        [Header("TAB")]
        [SerializeField] private List<Button> _btnTabs;
        [SerializeField] private List<GameObject> _panelTabs;
        [SerializeField] private List<Sprite> _spriteLight;
        [SerializeField] private List<Sprite> _spriteDark;

        [SerializeField] private BookItemBeast _prfItemBeast;
        [SerializeField] private BookItemEnemy _prfItemEnemy;
        [SerializeField] private GameObject _prfItemTeam;

        [SerializeField] private Transform _parentBeast;
        [SerializeField] private Transform _parentBoos;
        [SerializeField] private Transform _parentTeam;

        [SerializeField] private Sprite _spriteUpgraded;
        [SerializeField] private Sprite _spriteLock;
        [SerializeField] private Sprite _spriteLightBG;
        [SerializeField] private Sprite _spriteDarkBG;
        [SerializeField] private Sprite _spriteGem;
        [SerializeField] private Sprite _spriteVideo;

        [SerializeField] private RectTransform _rectTooltipStats;
        [SerializeField] private RectTransform _rectTooltipHP;
        [SerializeField] private RectTransform _rectTooltipSkill;
        [SerializeField] private Text _txtTooltipHP;
        [SerializeField] private Text _txtTooltipSkill;
        [SerializeField] private Text _txtGems;
        [SerializeField] private Text _txtSoul;
        [SerializeField] private Text _txtMedal;
        [SerializeField] private Text _txtGem;
        [SerializeField] private Text _txtChest;

        [SerializeField] private UIButton _btnClose;
        [SerializeField] private ScrollRect _scrollBeast;
        [SerializeField] private ScrollRect _scrollMonster;
        [SerializeField] private ScrollRect _scrollTeam;

        [SerializeField] private BeastInfoHomePanel beastInfoHomePanel;

        private List<BookItemBeast> _listItemBeast;
        private List<BookItemEnemy> _listItemEnemy;

        private float _timeOpenAds;
        private int lvBeast;
        private int bookLvBeast;

        private bool shouldShowLoading = true;

        private void Awake()
        {
            _listItemBeast = new List<BookItemBeast>();
            _listItemEnemy = new List<BookItemEnemy>();

            for (int i = 0; i < _btnTabs.Count; i++)
            {
                int n = i;
                _btnTabs[i]?.onClick.AddListener(() => this.OnClickTab(n));
            }
        }

        // Use this for initialization
        void Start()
        {
            _btnClose?.onClick.AddListener(this.OnClickClose);
            this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => UpdateGem());
        }

        void UpdateGem()
        {
            _txtGems.text = Utils.GetCurrentRubyMoney().ToString();
        }

        private void OnEnable()
        {
            UpdateGem();
            StartCoroutine(IESpawnItem());
            //IESpawnItem();
            this.OnClickTab(0);
            if (_scrollBeast.verticalNormalizedPosition > 0f)
            {
                UIManager.Instance.ShowLoading(true);
                shouldShowLoading = true;
            }
            else
            {
                UIManager.Instance.ShowLoading(false);
                shouldShowLoading = false;
            }

        }

        private void OnDisable()
        {
            GameManager.Instance.ReadySpawnGift();
        }

        private IEnumerator IESpawnItem()
        {
            //yield return new WaitForEndOfFrame();
            lvBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0); //level beast trong qua trung
            bookLvBeast = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 0);
            int heighest = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST, 0);
            int maxmax = bookLvBeast > heighest ? bookLvBeast : heighest;
            int count = 1;
            int totalAscend = Utils.GetTotalAscend();
            float scrollTo = 1f - (float)bookLvBeast / maxmax;
            foreach (var beast in _beastConfig.BeastData)
            {

                if (beast.Value.ID - maxmax > 1) break;
                BookItemBeast item = null;
                if (count > _listItemBeast.Count)
                {
                    item = Instantiate(_prfItemBeast, _parentBeast);
                    _listItemBeast.Add(item);
                }
                else item = _listItemBeast[count - 1];
                yield return new WaitForEndOfFrame();

                item.TxtLevel.text = beast.Value.Level.ToString();
                BigInteger dmg = Utils.GetDameByLevelAndAscend(beast.Value.Level);
                item.TxtDPS.text = $"{Utils.FormatNumber(dmg)}<color=#FFC51CFF>/秒</color>";
                item.AnimCat.Skeleton.SetSkin(beast.Value.Level.ToString("D3"));
                item.BtnPurchase.gameObject.SetActive(true);
                item.BtnPurchase?.onClick.RemoveAllListeners();


                if (beast.Value.ID <= bookLvBeast)
                {
                    item.TxtName.text = beast.Value.Name;
                    item.AnimCat.color = Color.white;
                    item.ImgUpgrade.sprite = _spriteUpgraded;
                    item.ImgUpgrade.SetNativeSize();
                    item.ImgBG.sprite = _spriteLightBG;
                    item.ImgAdsIcon.gameObject.SetActive(false);

                    int price = Mathf.RoundToInt(Mathf.Pow(1.5f, beast.Value.ID - lvBeast));
                    int idBeast = beast.Value.ID;


                    if (bookLvBeast - lvBeast > 6 && bookLvBeast >= 4 && beast.Value.ID == bookLvBeast - 3 && Time.time > _timeOpenAds && AdsManager.Instance.IsLoaded())
                    {
                        item.TxtPrice.text = "获得";
                        item.ImgAdsIcon.gameObject.SetActive(true);
                        item.BtnPurchase?.onClick.AddListener(() => this.OnClickBuyBeastByWatchVideo(idBeast, price));
                    }
                    else if (beast.Value.ID - lvBeast >= 1)
                    {
                        item.TxtPrice.text = Utils.FormatNumber(price).ToString();
                        item.ImgIcon.sprite = _spriteGem;
                        item.ImgIcon.SetNativeSize();
                        item.BtnPurchase?.onClick.AddListener(() => this.OnClickBuyBeastByGem(price, idBeast));
                    }
                    else
                    {
                        item.BtnPurchase.gameObject.SetActive(false);
                    }
                }
                else if (beast.Value.ID <= heighest)
                {
                    item.TxtName.text = beast.Value.Name;
                    item.AnimCat.color = Color.white;
                    item.ImgUpgrade.sprite = _spriteLock;
                    item.ImgUpgrade.SetNativeSize();
                    item.ImgBG.sprite = _spriteLightBG;
                    item.ImgAdsIcon.gameObject.SetActive(false);
                    item.BtnPurchase.gameObject.SetActive(false);
                }
                else
                {
                    item.TxtName.text = "???";
                    item.AnimCat.color = Color.black;
                    item.ImgUpgrade.sprite = _spriteLock;
                    item.ImgUpgrade.SetNativeSize();
                    item.ImgBG.sprite = _spriteDarkBG;
                    item.BtnPurchase.gameObject.SetActive(false);
                }

                count++;
                yield return new WaitForEndOfFrame();

            }

            if (count < _listItemBeast.Count) // destroy phan thua
            {
                for (int i = count; i < _listItemBeast.Count; i++) Destroy(_listItemBeast[i].gameObject);
            }


            int lvMonster = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER, 0);
            int oldLv = PlayerPrefs.GetInt(StringDefine.OLD_LEVEL_ASCEND, 29) - 1;
            int levelToCheck = Utils.GetTotalAscend() > 0 ? oldLv : lvMonster;

            int countMonster = 1;
            foreach (var monster in _monsterConfig.Monster)
            {
                if (monster.Value.ID - levelToCheck >= 2) break;

                BookItemEnemy enemy = null;
                if (countMonster > _listItemEnemy.Count)
                {
                    enemy = Instantiate(_prfItemEnemy, _parentBoos);
                    _listItemEnemy.Add(enemy);
                }
                else
                {
                    enemy = _listItemEnemy[countMonster - 1];
                }

                enemy.TxtName.text = monster.Value.Name;
                enemy.AnimBoss.skeletonDataAsset = monster.Value.Skeleton;
                enemy.AnimBoss.Initialize(true);

                if (monster.Value.ID <= levelToCheck)
                {
                    enemy.AnimBoss.color = enemy.ImgLootbox.color = Color.white;
                    enemy.SetEvent(this, monster.Value.ID);
                }
                else
                {
                    enemy.AnimBoss.color = enemy.ImgLootbox.color = Color.black;
                    enemy.ClearEvent();
                }

                countMonster++;
                yield return new WaitForEndOfFrame();
            }

            if (countMonster < _listItemEnemy.Count)
            {
                for (int i = 0; i < _listItemEnemy.Count; i++) Destroy(_listItemEnemy[i].gameObject);
            }

            // SpawnHomeTeam();

            _scrollBeast.verticalNormalizedPosition = scrollTo;
            _scrollMonster.verticalNormalizedPosition = 0f;
            if (shouldShowLoading)
            {
                yield return new WaitForSeconds(0.2f);
                UIManager.Instance.ShowLoading(false);
            }
        }

        // private void SpawnHomeTeam()
        // {
        //     _scrollTeam.verticalNormalizedPosition = 0f;

        //     while (_parentTeam.childCount > 0)
        //     {
        //         var go = _parentTeam.GetChild(0).gameObject;
        //         ObjectPool.Instance.ReleaseObject(go);
        //     }

        //     var beastTeamInfos = PlayerData.instance.GetBeastTeamInfos();

        //     for (int k = 0; k < beastTeamInfos.Count(); k++)
        //     {
        //         var itemData = beastTeamInfos.ElementAt(k);

        //         var go = ObjectPool.Instance.GetGameObject(_prfItemTeam, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);

        //         go.transform.SetParent(_parentTeam);

        //         go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

        //         itemData.beastItemViewGo = go;

        //         Action action = () => ShowPanelInfo(_parentTeam, itemData, beastTeamInfos);

        //         var btiv = go.GetComponent<BeastTeamItemView>();
        //         btiv.SetUp(itemData, action, action);
        //     }
        // }

        // private void ShowPanelInfo(Transform _parentTeam, BeastTeamInfo beastTeamInfo, List<BeastTeamInfo> datas)
        // {
        //     beastInfoHomePanel?.gameObject.SetActive(true);
        //     beastInfoHomePanel.Init(_parentTeam, beastTeamInfo, datas, EnableType.FromListMonster);
        // }

        private void OnClickTab(int n)
        {
            for (int i = 0; i < _panelTabs.Count; i++)
            {
                _panelTabs[i].SetActive(i == n);
                _btnTabs[i].image.sprite = i == n ? _spriteLight[i] : _spriteDark[i];
                _btnTabs[i].GetComponentInChildren<Text>().color = i == n ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
                _btnTabs[i].transform.localScale = i == n ? UnityEngine.Vector3.one * 1.3f : UnityEngine.Vector3.one;
                if (i == 2)
                {
                    if (n != i)
                    {
                        if (ColorUtility.TryParseHtmlString("#383838", out var color))
                        {
                            _btnTabs[i].image.color = color;
                        }
                    }
                    else
                    {
                        _btnTabs[i].image.color = Color.white;
                    }

                }
            }

            this.OnUpHP();
            this.OnUpSkill();
            this.OnUpStats();
        }

        private void OnClickClose()
        {
            ScreenManager.Instance.DeActiveScreen();
        }

        private void OnClickBuyBeastByGem(int price, int idBeast)
        {
            int currentRuby = Utils.GetCurrentRubyMoney();
            if (currentRuby >= price)
            {
                string mgs = $"你确定要花费<color=purple>{price}宝石</color>购买该魔兽吗";
                ScreenManager.Instance.ShowConfirm(2, mgs, () =>
                 {
                     Utils.AddRubyCoin(-price);
                     this.PostEvent(EventID.OnUpDateMoney);

                     ScreenManager.Instance.ShowNoti($"恭喜！你获得了一只{idBeast + 1}级魔兽");
                     GameManager.Instance.AddGift(idBeast);
                 });
            }
            else
            {
                ScreenManager.Instance.ShowNoti("你的金钱不足");
            }
        }

        private void OnClickBuyBeastByWatchVideo(int idBeast, int price)
        {
            AdsManager.Instance.ShowAds(() =>
            {
                //    ScreenManager.Instance.ShowNoti($"Congratulations.You have just acquired a beast lv {idBeast + 1}");
                GameManager.Instance.AddGift(idBeast);
                _timeOpenAds = Time.time + 180f;

                _listItemBeast[idBeast].ImgAdsIcon.gameObject.SetActive(false);
                _listItemBeast[idBeast].TxtPrice.text = price.ToString();
                _listItemBeast[idBeast].ImgIcon.sprite = _spriteGem;
                _listItemBeast[idBeast].ImgIcon.SetNativeSize();
                _listItemBeast[idBeast].BtnPurchase?.onClick.AddListener(() => this.OnClickBuyBeastByGem(price, idBeast));

                ScreenManager.Instance.DeActiveScreen();
            }, StringDefine.ADS_RECEIVED_BEAST_BOOK);
        }

        #region Tooltip
        public void OnDownStats(int id, UnityEngine.Vector3 position)
        {
            _rectTooltipStats.gameObject.SetActive(true);
            int n = id % 3;
            _rectTooltipStats.pivot = new UnityEngine.Vector2(n < 2 ? 0 : 1, 0);
            _rectTooltipStats.transform.position = position;

            BigInteger dmg = Utils.GetHPEnemy(id + 1);
            string soul = DataConfig.ListSoulReward[id];
            _txtSoul.text = $"+{Utils.FormatNumber(BigInteger.Parse(soul))}";
            _txtGem.text = $"+{DataConfig.ListGemReward[id].AsInt}";
            _txtMedal.text = $"+{DataConfig.ListMedalReward[id].AsInt}";
            _txtChest.text = $"+{DataConfig.ListChestReward[id].AsInt}";
        }

        public void OnUpStats()
        {
            _rectTooltipStats.gameObject.SetActive(false);
        }

        public void OnDownHP(int id, UnityEngine.Vector3 position)
        {
            _rectTooltipHP.gameObject.SetActive(true);
            int n = id % 3;
            _rectTooltipHP.pivot = new UnityEngine.Vector2(n < 2 ? 0 : 1, 0);
            _rectTooltipHP.transform.position = position;

            BigInteger hp = Utils.GetHPEnemy(id + 1);
            _txtTooltipHP.text = $"生命：{Utils.FormatNumber(hp)}";
        }

        public void OnUpHP()
        {
            _rectTooltipHP.gameObject.SetActive(false);
        }

        public void OnDownSkill(int id, UnityEngine.Vector3 position)
        {
            _rectTooltipSkill.gameObject.SetActive(true);
            int n = id % 3;
            _rectTooltipSkill.pivot = new UnityEngine.Vector2(n < 2 ? 0 : 1, 0);
            _rectTooltipSkill.transform.position = position;

            string skl = "";
            if ((id + 1) % 3 == 0)
            {
                float time = Mathf.Round(21 - (id + 1) / 3f);
                BigInteger hp = Utils.GetHPEnemy(id + 1);
                string mau = Utils.FormatNumber(hp / 100);
                skl = $"<color=#55E539FF>Healing</color>:Each {time}s restore 1%<color=#55E539FF>({mau})</color>of total HP";
            }
            else
            {
                skl = "无技能";
            }
            _txtTooltipSkill.text = skl;
        }

        public void OnUpSkill()
        {
            _rectTooltipSkill.gameObject.SetActive(false);
        }
        #endregion
    }
}
