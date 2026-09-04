using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Observer;
using System.Numerics;
using Spine.Unity;

namespace MergeBeast
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private MonsterConfig _monsterData;
        [SerializeField] private SkeletonGraphic _monster;
        [SerializeField] private Image _imgHPBar;
        [SerializeField] private Image _imgLevelProgress;
        [SerializeField] private Text _txtHpEnemy;
        [SerializeField] private Text _txtMonsterInfo;
        [SerializeField] private Text _txtTooltipDes;
        [SerializeField] private Text _txtLevelPlayer;
        [SerializeField] private Animator _prfShowDame;
        [SerializeField] private CoinRewardFX _prfCoin;
        [SerializeField] private CoinRewardFX _prfMedalMerge;
        [SerializeField] private CoinRewardFX _prfGem;
        [SerializeField] private SoundManager _sound;
        [SerializeField] private Transform _transCoinTarget;
        [SerializeField] private Transform _transMedalMerge;
        [SerializeField] private Transform _transGem;
        [SerializeField] private GameObject _btnTooltipSkill;
        [SerializeField] private Image _imgCountDownHP;

        [SerializeField] private FadeScreen _fadeScr;
        [SerializeField] private NoticeUpgrade _notice;
        [SerializeField] private GameOverController gameOver;
        [SerializeField] private BossComplete bossComplete;


        private int _currentEnemyID;
        private int _currentSpriteEnemy;
        private BigInteger _totalHP;
        private BigInteger _currentHP;
        private Queue<Animator> _queueDame;
        private Queue<CoinRewardFX> _queueCoin;
        private Queue<CoinRewardFX> _queueMedalMerge;
        private Queue<CoinRewardFX> _queueGem;
        private bool _enemyActing = true;
        private bool _isBuffHP;
        private float _countTimeBuffHP;
        private float _timeBuffHP;


        // Use this for initialization
        private void Awake()
        {
            _currentEnemyID = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER, 0);            
            ActiveNewEnemy();
            _enemyActing = false;
        }

        void Start()
        {
            _queueDame = new Queue<Animator>();
            _queueCoin = new Queue<CoinRewardFX>();
            _queueMedalMerge = new Queue<CoinRewardFX>();
            _queueGem = new Queue<CoinRewardFX>();

            for (int i = 0; i < 30; i++)
            {
                Animator anim = Instantiate(_prfShowDame, _prfShowDame.transform.parent);
                _queueDame.Enqueue(anim);

                CoinRewardFX fx = Instantiate(_prfCoin, _prfCoin.transform.parent);
                EnqueueCoin(fx);

                CoinRewardFX medal = Instantiate(_prfMedalMerge, _prfMedalMerge.transform.parent);
                this.EnqueueMedalMerge(medal);

                CoinRewardFX gem = Instantiate(_prfGem, _prfGem.transform.parent);
                this.EnqueueGem(gem);
            }

            _queueDame.Enqueue(_prfShowDame);
            EnqueueCoin(_prfCoin);
            this.EnqueueMedalMerge(_prfMedalMerge);
            this.RegisterListener(EventID.OnTimeJump, (sender, param) => this.OnTimeJump((int)param));
            this.RegisterListener(EventID.OnAscend, (sender, param) => this.OnAscend());
        }

        private void Update()
        {
            if (_isBuffHP && !_enemyActing)
            {
                _countTimeBuffHP += Time.deltaTime;
                _imgCountDownHP.fillAmount = (_timeBuffHP - _countTimeBuffHP) / _timeBuffHP;

                if (_countTimeBuffHP >= _timeBuffHP) // Hoi 1% mau
                {
                    BigInteger hp = _totalHP / 100;
                    _currentHP += hp;
                    if (_currentHP < 0) _currentHP = 0;
                    if (_currentHP > _totalHP) _currentHP = _totalHP;
                    _imgHPBar.fillAmount = (float)_currentHP / (float)_totalHP;
                    _txtHpEnemy.text = $"{Utils.FormatNumber(_currentHP)} / {Utils.FormatNumber(_totalHP)}";
                    PlayerPrefs.SetString(StringDefine.MONSTER_HP, _currentHP.ToString());

                    PoolManager.Instance.PlayFXHealHP(UnityEngine.Vector2.up * 2f);
                    _countTimeBuffHP = 0f;
                    this.ShowTextDmg(new Color(0.28f, 0.68f, 0.4f, 1f), "+" + Utils.FormatNumber(hp));

                    _sound.PlaySound(EnumDefine.SOUND.BOSS_BUFF_HP);

                    var current = _monster.AnimationState.AddAnimation(1, "skill", false,0f);
                    current.Complete += (t) =>
                    {
                        _monster.AnimationState.ClearTrack(1);
                    };
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fade();
            }
        }

        public void ActiveNewEnemy()
        {
            _enemyActing = false;
            //_mapProgress.NextStage(_currentEnemyID);
            MonsterData data = _monsterData.GetMonster((EnumDefine.MONSTER)_currentEnemyID);
            _totalHP = Utils.GetHPEnemy(data.Level);
            _monster.skeletonDataAsset = data.Skeleton;
            _monster.Initialize(true);
            string skill = data.ID == 7 ? "empty_skill_miu" : "skill";
            var current = _monster.AnimationState.SetAnimation(0, skill, false);
            current.Complete += (t) =>
            {
                _monster.AnimationState.SetAnimation(0, "idle", true);
            };

            //beastFake.gameObject.SetActive(true);
            //beastFake.sprite = data.Display;
            //beastFake.SetNativeSize();

            _currentHP = BigInteger.Parse(PlayerPrefs.GetString(StringDefine.MONSTER_HP, "0"));
            if (_currentHP <= 0) _currentHP = _totalHP;

            int phanduchomuoi = (_currentEnemyID + 1) % 10;

            _isBuffHP = phanduchomuoi % 3 == 0;

            if (_isBuffHP)
            {
                _timeBuffHP = Mathf.Max(3f, Mathf.Round(21f - _currentEnemyID / 3f));
                //    _txtTooltipName.text = $"<color=#55E539FF><size=60> {data.Name} </size></color>";
                string mau = Utils.FormatNumber(_totalHP / 100);
                _txtTooltipDes.text = _isBuffHP ? $"<color=#55E539FF>治疗</color>：每{_timeBuffHP}秒恢复1%<color=#55E539FF>（{mau}）</color>最大生命" : "无技能";
            }
            _btnTooltipSkill.SetActive(_isBuffHP);

            _txtHpEnemy.text = $"{Utils.FormatNumber(_currentHP)} / {Utils.FormatNumber(_totalHP)}";
            _txtMonsterInfo.text = $"<color=#55E539FF><size=60> {data.Name} </size></color>";
            _imgHPBar.fillAmount = PercentAmount(); // (float)_currentHP / (float)_totalHP;
            _imgHPBar.transform.parent.gameObject.SetActive(true);
            _txtLevelPlayer.text = data.Level.ToString();
            _imgLevelProgress.fillAmount = 0f;

            // 
            if (_currentEnemyID >= PlayerPrefs.GetInt(StringDefine.OLD_LEVEL_ASCEND, 29))
            {
                UIManager.Instance.CheckAscendNotice(true);
            }

            //check truong hop time jump
            if (soulTimeJump <= 0 || secondTimeJump <= 0)
            {
                soulTimeJump = 0;
                secondTimeJump = 0;
            }
            else
            {
                TimeJump();
            }

        }

        private void ShowTextDmg(Color color, string dmg)
        {
            if (_queueDame.Count <= 0) return;
            var anim = _queueDame.Dequeue();
            Text txt = anim.GetComponentInChildren<Text>();
            txt.text = dmg;
            txt.color = color;
            UnityEngine.Vector3 monsterPos = _monster.transform.position;
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(
                Random.Range(monsterPos.x - 1.5f, monsterPos.x + 1.5f),
                Random.Range(monsterPos.y - 0.5f, monsterPos.y + 1.5f),
                0);

            anim.transform.position = pos;
            anim.SetTrigger("show");
            StartCoroutine(EnqueueAnim(anim));
        }

        public void Attack(BigInteger dame, int id)
        {
            if (_enemyActing) return;

            if (!CPlayer.OutMainGame)
            {
                _sound.PlaySound(EnumDefine.SOUND.BEAST_ATTACK);
            }
            // Toast Dame
            this.ShowTextDmg(new Color(0.68f, 0.28f, 0.28f, 1f), "-" + Utils.FormatNumber(dame));

            // Show Hit
            UnityEngine.Vector2 monsterPos = _monster.transform.position;
            UnityEngine.Vector2 hitPos = new UnityEngine.Vector3(Random.Range(monsterPos.x - 1.5f, monsterPos.x + 1f),
                Random.Range(monsterPos.y - 0.5f, monsterPos.y + 1.5f),
                0);
            PoolManager.Instance.PlayFXGetHit(id, hitPos);

            this.ShowCoinFX();
            this.DropMedal();

            // update hp bar
            _currentHP -= dame;
            if (_currentHP < 0) _currentHP = 0;
            _imgHPBar.fillAmount = PercentAmount(); // (float)_currentHP / (float)_totalHP;
            _txtHpEnemy.text = $"{Utils.FormatNumber(_currentHP)} / {Utils.FormatNumber(_totalHP)}";
            _imgLevelProgress.fillAmount = 1f - _imgHPBar.fillAmount;
            PlayerPrefs.SetString(StringDefine.MONSTER_HP, _currentHP.ToString());

            if (_currentHP > 0)
            {
                if (_monster.startingAnimation.Equals("behit")) return;
                var current = _monster.AnimationState.SetAnimation(0, "behit", false);
                current.Complete += (t) =>
                {
                    _monster.AnimationState.SetAnimation(0, "idle", true);
                };
            }
            else
            {
                _monster.AnimationState.SetAnimation(0, "die", false);
                _imgHPBar.transform.parent.gameObject.SetActive(false);

            //    PoolManager.Instance.PlayFXEnemyDead(new UnityEngine.Vector3(-0.25f, 2.2f, -1f));
                _enemyActing = true;
                StartCoroutine(DelayActiveEnemy());

                _sound.PlaySound(EnumDefine.SOUND.BOSS_DIE);
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.DefeatBoss, EnumDefine.Mission.RaidBoss);
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.None, EnumDefine.Mission.KillEnemies);
            }
        }

        private float PercentAmount()
        {
            string strTotalHP = _totalHP.ToString();
            float percent = 0f;
            if (strTotalHP.Length > 6)
            {
                string strCurrentHP = _currentHP.ToString();
                int mu = strTotalHP.Length - strCurrentHP.Length;
                if (mu == 0)
                {
                    string subCurrent = strCurrentHP.Substring(0, 3);
                    string subTotal = strTotalHP.Substring(0, 3);
                    percent = float.Parse(subCurrent) / float.Parse(subTotal);
                }
                else
                {
                    string subCurrent = strCurrentHP.Substring(0, strCurrentHP.Length > 3 ? 3 : strCurrentHP.Length);
                    string subTotal = strTotalHP.Substring(0, subCurrent.Length);
                    percent = float.Parse(subCurrent) / float.Parse(subTotal);
                    mu = mu > 3 ? 3 : mu;
                    percent /= Mathf.Pow(10, mu);
                }
            }
            else
            {
                percent = (float)_currentHP / (float)_totalHP;
            }
            return percent;
        }

        private void EnqueueCoin(CoinRewardFX fx)
        {
            fx.gameObject.SetActive(false);
            if (!_queueCoin.Contains(fx)) _queueCoin.Enqueue(fx);
        }

        private void EnqueueMedalMerge(CoinRewardFX fx)
        {
            fx.gameObject.SetActive(false);
            if (!_queueMedalMerge.Contains(fx)) _queueMedalMerge.Enqueue(fx);
        }

        private void EnqueueGem(CoinRewardFX fx)
        {
            fx.gameObject.SetActive(false);
            if (!_queueGem.Contains(fx)) _queueGem.Enqueue(fx);
        }

        private void ShowCoinFX()
        {
            if (_queueCoin.Count <= 0) return;
            CoinRewardFX fx = _queueCoin.Dequeue();
            fx.gameObject.SetActive(true);
            UnityEngine.Vector3 fxPos = new UnityEngine.Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.5f), 0);
            fx.ShowFX(fxPos, _transCoinTarget.position, this.EnqueueCoin);
        }

        private void ShowMedalFX()
        {
            if (_queueMedalMerge.Count <= 0) return;
            CoinRewardFX fx = _queueMedalMerge.Dequeue();
            fx.gameObject.SetActive(true);
            UnityEngine.Vector3 fxPos = new UnityEngine.Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.5f), 0);
            fx.ShowFX(fxPos, _transMedalMerge.position, this.EnqueueMedalMerge);
        }

        private void ShowGemFX()
        {
            if (_queueGem.Count <= 0) return;
            CoinRewardFX fx = _queueGem.Dequeue();
            fx.gameObject.SetActive(true);
            UnityEngine.Vector3 fxPos = new UnityEngine.Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1.5f, 3.5f), 0);
            fx.ShowFX(fxPos, _transGem.position, this.EnqueueGem);
        }

        private void DropMedal()
        {
            int _totalAutoMerge = PlayerPrefs.GetInt(StringDefine.AUTO_MERGE, 50);
            if (_totalAutoMerge > 99) return;

            if (Random.Range(1, 100) == 1)
            {
                this.ShowMedalFX();
                StartCoroutine(DelayAddMergeMedal(1));
            }
        }

        private IEnumerator DelayAddMergeMedal(int medal)
        {
            yield return new WaitForSeconds(1.3f);
            Utils.AddMedalMerge(medal);
            this.PostEvent(EventID.OnUpdateAutoMergeMedal);
        }

        private IEnumerator EnqueueAnim(Animator anim)
        {
            yield return new WaitForSeconds(0.5f);
            if (!_queueDame.Contains(anim)) _queueDame.Enqueue(anim);
        }

        private IEnumerator DelayActiveEnemy()
        {
            if (_currentEnemyID >= (int)Config.MAX_MONSTER)
            {
                //game over
                UIManager.Instance.ShowPopup(gameOver.transform);
                UIManager.Instance.isGameOver = true;
                yield break;
            }


            for (int i = 0; i < 10; i++)
                this.ShowCoinFX();

            for (int i = 0; i < 5; i++)
                this.ShowGemFX();

            this.DropMedal();

            yield return new WaitForSeconds(1.3f);


            _notice.SetNextStage();

            _currentEnemyID++;
            PlayerPrefs.SetInt(StringDefine.LEVEL_MONSTER, _currentEnemyID);

            if(_currentEnemyID > 5) {
                AdsManager.Instance.ShowAds(() => {
                }, "", EnumDefine.ADSTYPE.Interstitial, false, EnumDefine.GameType.Beast);
            }

            //show popup kill boss
            //_currentEnemyID > _monsterData.Monster.Count || _currentEnemyID == 3 || _currentEnemyID % 10 == 0
            //if (_currentEnemyID > _monsterData.Monster.Count || _currentEnemyID == 3 || _currentEnemyID % 10 == 0)
            //{
            //    string soul = DataConfig.ListSoulReward[_currentEnemyID - 1];
            //    UIManager.Instance.UpdateMoneyCoin(BigInteger.Parse(soul), false);
            //    Utils.AddRubyCoin(DataConfig.ListGemReward[_currentEnemyID - 1].AsInt);
            //    Utils.AddMedalMerge(DataConfig.ListMedalReward[_currentEnemyID - 1].AsInt);
            //    int chest = DataConfig.ListChestReward[_currentEnemyID - 1].AsInt;
            //    if (chest > 0)
            //    {
            //        int currentChest = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST, 0);
            //        chest += currentChest;
            //        PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST, chest);
            //    }
            //    UIManager.Instance.UpdateText();
            //    bossComplete.Show(_currentEnemyID, _monsterData.GetMonster((EnumDefine.MONSTER)(_currentEnemyID - 1)).Name);
            //    _mapProgress.NextStage(_currentEnemyID);


            //    yield break;

            //}
            //else
            //{
                string soul = DataConfig.ListSoulReward[_currentEnemyID - 1];
                UIManager.Instance.UpdateMoneyCoin(BigInteger.Parse(soul), false);
                Utils.AddRubyCoin(DataConfig.ListGemReward[_currentEnemyID - 1].AsInt);
                Utils.AddMedalMerge(DataConfig.ListMedalReward[_currentEnemyID - 1].AsInt);
                int chest = DataConfig.ListChestReward[_currentEnemyID - 1].AsInt;
                if (chest > 0)
                {
                    int currentChest = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST, 0);
                    chest += currentChest;
                    PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST, chest);
                }
                UIManager.Instance.UpdateText();
                //int phanduchomuoi = _currentEnemyID % 10;
                //if(_currentEnemyID > 10 && phanduchomuoi % 3 == 0 && AdsManager.Instance.IsLoaded()) {                    
                //    AdsManager.Instance.ShowAds(() => {
                //    }, "");
                //} 

                ActiveNewEnemy();
            //}


            this.PostEvent(EventID.OnBeastBattleMove, true);

            Fade();

            yield return new WaitForSeconds(3f);

            this.PostEvent(EventID.OnBeastBattleMove, false);
            this.PostEvent(EventID.OnUpDateMoney);
            _enemyActing = false;


        }

        public void BackFromMap()
        {
            ActiveNewEnemy();
            //khi giet bosss ko fade thi hien thi thanh mau
            _imgHPBar.transform.parent.gameObject.SetActive(true);
        }

        void Fade()
        {

            //move stone chinh xuong cuoi man hinh, move stone fake den vi tri, đưa stone fake lên trên màn hình,
            //đưa stone chính về chỗ cũ
            UnityEngine.Vector2 posTop = new UnityEngine.Vector2(0, 2300);
            UnityEngine.Vector2 pos = new UnityEngine.Vector2(0, 393);
            UnityEngine.Vector2 posBottom = new UnityEngine.Vector2(0, -1514);
            
        }

        private void OnTimeJump(int timeSecond)
        {
            Debug.LogError("on time jump");
            soulTimeJump = GameManager.Instance.GetRootDPS() * timeSecond;
            secondTimeJump = timeSecond;
            TimeJump();
        }

        private void OnAscend()
        {
            VipConfig vipConfig = CPlayer.GetVipConfig();
            _currentEnemyID = vipConfig.levelStageAscend - 1;
            PlayerPrefs.SetInt(StringDefine.LEVEL_MONSTER, _currentEnemyID);
            PlayerPrefs.SetString(StringDefine.MONSTER_HP, "0");
            this.ActiveNewEnemy();
        }

        /*
        private IEnumerator IETimeJump(int second)
        {
            Debug.LogError("root dps: " + GameManager.Instance.GetRootDPS());
            if (GameManager.Instance.GetRootDPS() <= 0) yield break;
            int timeSecond = second;
            Debug.LogError("time second: " + second);
            while(timeSecond > 0)
            {
                if (!_isBuffHP)
                {
                    BigInteger timeKill = _currentHP / GameManager.Instance.GetRootDPS();
                    Debug.LogError("time kill: " + timeKill);
                    if(timeKill < timeSecond)
                    {
                        timeSecond -= (int) timeKill;
                        this.Attack(_currentHP, 0);
                    }
                    else
                    {
                        BigInteger dmg = GameManager.Instance.GetRootDPS() * timeSecond;
                        this.Attack(dmg, 0);
                        timeSecond = 0;
                    }
                   
                }
                else
                {
                    BigInteger dps = (int)_timeBuffHP * GameManager.Instance.GetRootDPS() - _totalHP / 100;
                    Debug.LogError("dps: " + dps);
                    Debug.LogFormat("timebuffhp: {0}, root: {1}, totalHP: {2}", _timeBuffHP, GameManager.Instance.GetRootDPS(), _totalHP);
                    if (dps > 0)
                    {
                        int timeC = (int)(_currentHP / dps) * (int)_timeBuffHP;
                        if(timeC <= timeSecond)
                        {
                            this.Attack(_currentHP, 0);
                            timeSecond -= timeC;
                        }
                        else
                        {
                            int tm = timeSecond / (int)_timeBuffHP;
                            BigInteger jDmg = dps * tm;
                            this.Attack(jDmg, 0);
                            timeSecond = 0;
                        }
                    }
                    else
                    {
                        yield break;
                    }
                   
                }

                yield return new WaitForSeconds(1.5f);
            }
        }
        */
        BigInteger soulTimeJump = 0;
        int secondTimeJump = 0;
        private void TimeJump()
        {
            //cong thuc tinh soul: a = dps * time;

            BigInteger hpRegen = _isBuffHP ? (_totalHP / 100) * Config.HP_REGEN : 0;
            int timeBuff = Mathf.RoundToInt(1f / _timeBuffHP * 100);
            BigInteger currentHp = _currentHP + (hpRegen * secondTimeJump * timeBuff) / 100;
            //Debug.LogError("damage: " + soulTimeJump + ", hp quai: " + currentHp + ", isBuff: " + _isBuffHP + ", total hp: " + _totalHP + ", ID: " + _currentEnemyID);
            if (soulTimeJump > currentHp)
            {
                //Debug.LogError("quai chet");
                Attack(soulTimeJump, 0);
                //tinh damage con lai
                soulTimeJump -= currentHp;
                //Debug.LogError("damage con lai: " + soulTimeJump);
                //tinh lai thoi gian dua vao a' (damage sau khi tru)
                secondTimeJump = (int)(soulTimeJump / GameManager.Instance.GetRootDPS());
            }
            else
            {
                //Debug.LogError("quai ko chet");
                BigInteger h2 = currentHp - soulTimeJump;
                if (h2 >= _totalHP) h2 = _totalHP;
                _currentHP = h2;
                soulTimeJump = 0;
                secondTimeJump = 0;
            }

        }

    }
}
