using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Observer;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;

public enum GameState
{
    None,
    Start,
    Win,
    Lose
}
[System.Serializable]
public class DmgByElement
{
    public Element elementAttacker;
    public Element elementTarget;
    public float ratio;
}

public class GameManager : MonoBehaviour
{
    public static Action<BeastBase> BeastRemove = null;

    public static GameManager instance;

    public Transform myGate;
    public GameObject myGateGo;

    public Transform enemyGate;
    public GameObject gatePref;
    public Transform landBottom;
    public Transform landTop;
    public Transform axeSmash;
    public Transform landBottom2;

    [SerializeField] Transform laneGround;
    [SerializeField] Transform laneFly;

    [Header("Debug")]
    [SerializeField] private bool offEnemy;
    [SerializeField] private bool offMy;
    [SerializeField] private bool offCache;
    [SerializeField] private bool debug;
    [SerializeField] private bool offLog;
    public bool offSkill0102;

    public bool fakeMonster;
    public bool fakeDamage;
    public int fakeSkill2 = 6;

    public List<MonsterData> fakeMyBeasts;
    public List<MonsterData> fakeEnemyBeasts;

    [Header("Fake hệ số")]
    public float DefRatio = 1f;
    public float AtkRatio = 1f;
    public float HpRatio = 1f;
    public List<DmgByElement> dmgByElements;

    public Dictionary<int, EntityBase> AllEntities { get; set; } = new Dictionary<int, EntityBase>();
    public int WorldIndex { get; set; }
    public Element MyTeamElement { get; set; }
    private List<BeastInfoItem> beastInfoItems;

    Dictionary<BeastId, DateTime> myBornTimePair = new Dictionary<BeastId, DateTime>();
    Dictionary<BeastId, DateTime> enemyBornTimePair = new Dictionary<BeastId, DateTime>();

    private List<BeastTeamInfo> myMonsterDatas, enemyMonsterDatas;

    private GameState currentGameState;
    private List<RewardModel> rewardModels;

    private Dictionary<int, float> myTeamBuffData, enemyTeamBuffData;
    private Dictionary<BeastId, BeastBase> myBeasts, enemyBeasts;

    Dictionary<int, int> myLaneIndexs, enemyLaneIndexs;
    Dictionary<int, int> myLaneFlyIndexs, enemyLaneFlyIndexs;

    private IEnumerator genMyBeastIE, genEnemyBeastIE;
    private IEnumerator runTimeIE;

    private List<Vector3> lanePositions;

    private int totalSec;
    private int totalMyMonster, totalEnemyMonster;

    void Awake()
    {
        instance = this;
        if (dmgByElements != null)
        {
            foreach (var dmgByElement in dmgByElements)
            {
                var key = (int)dmgByElement.elementAttacker * 100 + (int)dmgByElement.elementTarget;
                if (!Constant.damageByElementPair.ContainsKey(key))
                    Constant.damageByElementPair.Add(key, dmgByElement.ratio);
                else Constant.damageByElementPair[key] = dmgByElement.ratio;
            }
        }
    }

    void Start()
    {
        myLaneIndexs = new Dictionary<int, int>();
        enemyLaneIndexs = new Dictionary<int, int>();
        myLaneFlyIndexs = new Dictionary<int, int>();
        enemyLaneFlyIndexs = new Dictionary<int, int>();
        beastInfoItems = new List<BeastInfoItem>();
        rewardModels = new List<RewardModel>();
        lanePositions = new List<Vector3>();
        myBeasts = new Dictionary<BeastId, BeastBase>();
        enemyBeasts = new Dictionary<BeastId, BeastBase>();

        // #if UNITY_EDITOR
        //     Debug.unityLogger.logEnabled = offLog;
        // #else
        //     Debug.unityLogger.logEnabled = false;
        // #endif
        // Time.timeScale = 0.5f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        if (!fakeMonster)
        {
            myMonsterDatas = PlayerData.instance.GetCurTeam();
            if (myMonsterDatas.Count == 0) myMonsterDatas = FakeBeastData(new List<MonsterData>());

            Debug.Log("PlayerData.instance.CurStage:" + PlayerData.instance.CurStage);

            if (enemyMonsterDatas == null || enemyMonsterDatas.Count == 0) enemyMonsterDatas = LevelHelper.GetStageEnemies(PlayerData.instance.CurStage);
        }
        else
        {
            myMonsterDatas = FakeBeastData(fakeMyBeasts);
            enemyMonsterDatas = FakeBeastData(fakeEnemyBeasts);
        }

        if (!offCache)
        {
            CreateCache(myMonsterDatas);
            CreateCache(enemyMonsterDatas);
        }

        StartCoroutine(StartGameIE());
    }

    private List<BeastTeamInfo> FakeBeastData(List<MonsterData> list)
    {
        List<BeastTeamInfo> datas = new List<BeastTeamInfo>();
        var bns = Enum.GetValues(typeof(BeastId)).OfType<BeastId>()
                     .ToList();

        if (list.Count == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                var id = UnityEngine.Random.Range(0, bns.Count);
                list.Add(new MonsterData()
                {
                    beastId = bns[id]
                });
                bns.RemoveAt(id);
            }
        }


        for (int i = 0; i < list.Count; i++)
        {
            datas.Add(new BeastTeamInfo()
            {
                beastId = list[i].beastId,
                curStar = list[i].star,
                curLevel = list[i].level,
                fakeLane = list[i].laneIndex
            });
        }

        return datas;
    }

    private IEnumerator StartGameIE()
    {
        beastInfoItems.Clear();
        rewardModels.Clear();
        myBeasts.Clear();
        enemyBeasts.Clear();

        ObjectPool.Instance.ReleaseAll();
        AllEntities.Clear();
        currentGameState = GameState.None;
        WorldIndex = 0;

        // var myGateGO = ObjectPool.Instance.GetGameObject(gatePref, myGate.position, Quaternion.identity);
        // var myGateOb = myGateGO.GetComponent<Gate>();
        // // var gateHp = TeamBuffHelper.GetAverageHP(myMonsterDatas, true);
        // var id = AddEntity(myGateOb);
        // myGateOb.Setup(Team.My, 2000, id);

        // var enemyGateGO = ObjectPool.Instance.GetGameObject(gatePref, enemyGate.position, Quaternion.identity);
        // var enemyGateOb = enemyGateGO.GetComponent<Gate>();
        // // gateHp = GetAverageHP(enemyMonsterDatas, false);
        // id = AddEntity(enemyGateOb);
        // enemyGateOb.Setup(Team.Enemy, 2000, id);

        myLaneIndexs.Clear();
        enemyLaneIndexs.Clear();
        for (int i = 0; i < laneGround.childCount; i++)
        {
            myLaneIndexs[i] = 0;
            enemyLaneIndexs[i] = 0;
        }
        myLaneFlyIndexs.Clear();
        enemyLaneFlyIndexs.Clear();
        for (int i = 0; i < laneFly.childCount; i++)
        {
            myLaneFlyIndexs[i] = 0;
            enemyLaneFlyIndexs[i] = 0;
        }

        BeastRemove -= OnBeastRemove;
        BeastRemove += OnBeastRemove;

        myTeamBuffData = CalculateTeamBuffData(myMonsterDatas);
        enemyTeamBuffData = CalculateTeamBuffData(enemyMonsterDatas);

        CameraController.instance.Restart();

        UIGameManager.instance.StartGame();

        InitBeastItemViews();

        // tính Hp cho Gate

        yield return new WaitForSeconds(1f);

        currentGameState = GameState.Start;
        totalSec = fakeDamage ? 100000 : 120;
        totalMyMonster = myMonsterDatas.Count();
        totalEnemyMonster = enemyMonsterDatas.Count();

        if (genMyBeastIE != null) StopCoroutine(genMyBeastIE);
        if (genEnemyBeastIE != null) StopCoroutine(genEnemyBeastIE);
        genMyBeastIE = GenMyBeasts();
        genEnemyBeastIE = GenEnemyBeasts();
        StartCoroutine(genMyBeastIE);
        StartCoroutine(genEnemyBeastIE);
        // StartCoroutine(TeamBuffByTime(true));
        // StartCoroutine(TeamBuffByTime(false));

        if (runTimeIE != null) StopCoroutine(runTimeIE);
        runTimeIE = RunTimeIE();
        StartCoroutine(runTimeIE);

        AudioManager.instance.StartBattle();
    }

    private IEnumerator RunTimeIE()
    {
        while (IsPlaying())
        {
            yield return new WaitForSeconds(1f);

            totalSec -= 1;

            UIGameManager.instance.SetTimeText(Helper.TimeFormat(totalSec));

            if (totalSec <= 0) break;
        }

        GameOver(isWin: false);
    }

    private Dictionary<int, float> CalculateTeamBuffData(List<BeastTeamInfo> beastTeamInfos)
    {
        var res = new Dictionary<int, float>();
        foreach (var beastTeamInfo in beastTeamInfos)
        {
            var pref = BeastPrefs.Instance.GetBeastPref(beastTeamInfo.beastId);

            var beast = pref.GetComponent<BeastBase>();
            var teamBuffChance = beast.teamBuffChance;
            if (beastTeamInfo.beastId == BeastId.Furiosa)
                teamBuffChance = IncrByStar(teamBuffChance, beast, beastTeamInfo.curStar);

            var rd = UnityEngine.Random.Range(1, 101);
            if (rd > teamBuffChance) continue;

            if (beast.teamBuff != TeamBuff.None && beastTeamInfo.curStar >= 4)
            {
                int key = TeamBuffHelper.GenKey(beast.buffForElement, beast.buffForClass, (int)beast.teamBuff);

                var newValue = IncrByStar(beast.teamBuffValue, beast, beastTeamInfo.curStar);
                Debug.Log(beast.beastId + " teamBuff:" + beast.teamBuff + " newValue:" + newValue);
                if (res.ContainsKey(key))
                    res[key] += newValue;
                else
                    res.Add(key, newValue);
            }
        }
        return res;


        float IncrByStar(float value, BeastBase beast, int curStar)
        {
            if (curStar > 4 && beast.skillDes.Count > 3 && beast.skillDes[3].percentDmgByStar > 0)
                value += beast.skillDes[3].percentDmgByStar * (curStar - 4);
            return value;
        }
    }

    private void CreateCache(List<BeastTeamInfo> datas)
    {
        foreach (var data in datas)
        {
            var pref = BeastPrefs.Instance.GetBeastPref(data.beastId);

            var go = ObjectPool.Instance.CreateGameObject(pref, 2);
            var beastBase = go?.GetComponent<BeastBase>();
            if (beastBase != null)
            {
                if (beastBase.skill01?.effectHitPref != null)
                    ObjectPool.Instance.CreateGameObject(beastBase.skill01?.effectHitPref);

                if (beastBase.skill01?.bulletPref != null)
                {
                    var firstGO = ObjectPool.Instance.CreateGameObject(beastBase.skill01?.bulletPref);
                    CreateCacheInSkill(firstGO);
                }

                if (beastBase.skill02?.effectHitPref != null)
                    ObjectPool.Instance.CreateGameObject(beastBase.skill02?.effectHitPref);

                if (beastBase.skill02?.bulletPref != null)
                {
                    var firstGO = ObjectPool.Instance.CreateGameObject(beastBase.skill02?.bulletPref);
                    CreateCacheInSkill(firstGO);
                }

                var skill3Pref = BeastPrefs.Instance.GetBeastAllInfo(data.beastId).skill3;
                if (skill3Pref != null)
                {
                    var skill3Go = ObjectPool.Instance.CreateGameObject(skill3Pref, 15);
                    var sb = skill3Go.GetComponent<SkillBase>();

                    if (sb?.effectHitPref != null)
                        ObjectPool.Instance.CreateGameObject(sb?.effectHitPref, 15);

                    if (sb?.bulletPref != null)
                    {
                        var firstGO = ObjectPool.Instance.CreateGameObject(sb?.bulletPref, 15);
                        CreateCacheInSkill(firstGO);
                    }
                }
            }
        }
    }

    private void CreateCacheInSkill(GameObject go)
    {
        if (go == null) return;

        var bB = go.GetComponent<BulletBase>();
        if (bB != null)
        {
            if (bB.effPref != null)
                ObjectPool.Instance.CreateGameObject(bB.effPref);
            if (bB.explosionEffPref != null)
                ObjectPool.Instance.CreateGameObject(bB.explosionEffPref);
            if (bB.startEffPref != null)
                ObjectPool.Instance.CreateGameObject(bB.startEffPref);
        }

        var bA = go.GetComponent<BulletAtlantisSkill02>();
        if (bA != null)
        {
            if (bA.effPrefs != null)
                foreach (var pr in bA.effPrefs)
                    ObjectPool.Instance.CreateGameObject(pr, 5);
        }

        var bT = go.GetComponent<BulletThornSkill02>();
        if (bT != null)
        {
            if (bT.effPrefs != null)
                foreach (var pr in bT.effPrefs)
                    ObjectPool.Instance.CreateGameObject(pr, 5);
        }

        var bc = go.GetComponent<BulletChaos03>();
        if (bc != null)
        {
            if (bc.effAtStartPref != null)
                ObjectPool.Instance.CreateGameObject(bc.effAtStartPref, 5);
            if (bc.effAtMovePref != null)
                ObjectPool.Instance.CreateGameObject(bc.effAtMovePref, 5);
            if (bc.effAtEndPref != null)
                ObjectPool.Instance.CreateGameObject(bc.effAtEndPref, 5);
        }

        var bi = go.GetComponent<BulletIcePillars>();
        if (bi != null)
        {
            if (bi.effPref != null)
                ObjectPool.Instance.CreateGameObject(bi.effPref, 5);
        }
    }

    private void InitBeastItemViews()
    {
        foreach (var data in myMonsterDatas)
        {
            var pref = BeastPrefs.Instance.GetBeastPref(data.beastId);

            var beastBase = pref?.GetComponent<BeastBase>();
            var beastInfoItem = new BeastInfoItem()
            {
                icon = beastBase.icons.Count() >= data.curStar && data.curStar > 0 ? beastBase.icons[data.curStar - 1] : beastBase.icons[0],
                beastId = beastBase.beastId,
                curTeam = Team.My,
                beastName = Helper.WordFilt(beastBase.beastId.ToString(), out _)
            };
            beastInfoItems.Add(beastInfoItem);

            // if (data.curStar >= 3)
            //     UIGameManager.instance.InitSkill3(beastBase);
        }

        foreach (var data in enemyMonsterDatas)
        {
            var pref = BeastPrefs.Instance.GetBeastPref(data.beastId);

            var beastBase = pref?.GetComponent<BeastBase>();
            var beastInfoItem = new BeastInfoItem()
            {
                icon = beastBase.icons.Count() >= data.curStar && data.curStar > 0 ? beastBase.icons[data.curStar - 1] : beastBase.icons[0],
                beastId = beastBase.beastId,
                curTeam = Team.Enemy,
                beastName = Helper.WordFilt(beastBase.beastId.ToString(), out _)
            };
            beastInfoItems.Add(beastInfoItem);
        }

        UIGameManager.instance.UpdateBeastInfoItems(beastInfoItems);
    }

    private void OnBeastRemove(BeastBase obj)
    {
        if (obj.currentTeam == Team.My)
        {
            if (obj.moveType == MoveType.Ground)
                myLaneIndexs[obj.curLaneIndex]--;
            else
                myLaneFlyIndexs[obj.curLaneIndex]--;
        }
        else
        {
            // totalEnemyMonster--;
            if (obj.moveType == MoveType.Ground)
                enemyLaneIndexs[obj.curLaneIndex]--;
            else
                enemyLaneFlyIndexs[obj.curLaneIndex]--;
        }
        obj.Active(false, obj.currentTeam, new Vector3(1000, 1000, 0), 0, 0);

        // UIGameManager.instance.BeastDie(obj);

        // if (totalEnemyMonster == 0 && totalMyMonster == 0) GameOver(isWin: false);

        if (IsPlaying())
            if (obj.currentTeam == Team.My) myBornTimePair[obj.beastId] = DateTime.Now;
            else enemyBornTimePair[obj.beastId] = DateTime.Now;
    }

    private IEnumerator GenMyBeasts()
    {
        yield return null;
        if (offMy) yield break;
        myBornTimePair.Clear();

        BigIntCSharp totalBeastHp = 0;

        var fakeLanes = new Dictionary<BeastId, int>();
        var hasFrostQueen = myMonsterDatas.Exists(dt => dt.beastId == BeastId.FrostQueen);
        foreach (var data in myMonsterDatas)
        {
            myBornTimePair.Add(data.beastId, DateTime.Now.AddMinutes(-200));
            fakeLanes.Add(data.beastId, data.fakeLane);

            var beastPref = BeastPrefs.Instance.GetBeastPref(data.beastId);
            var beast = GenBeast2(beastPref, Team.My);
            if (hasFrostQueen)
                beast.perDmgWhenFreezeExplode = 1;
            beast.Active(false, Team.My, new Vector3(1000, 1000, 0), 0, 0);
            myBeasts.Add(beast.beastId, beast);
            UIGameManager.instance.InitSkill3(beast);

            totalBeastHp += beast.curHp;
        }

        myGateGo = ObjectPool.Instance.GetGameObject(gatePref, myGate.position, Quaternion.identity);
        var myGateOb = myGateGo.GetComponent<Gate>();
        // var gateHp = TeamBuffHelper.GetAverageHP(myMonsterDatas, true);
        var gateHp = totalBeastHp / myMonsterDatas.Count;
        var id = AddEntity(myGateOb);
        myGateOb.Setup(Team.My, gateHp <= 0 ? 2000 : gateHp, id);
        // myGateOb.Setup(Team.My, 1, id);

        Debug.Log("Player GetAverageHP:" + gateHp);

        while (IsPlaying())
        {
            var myBornTimePairTmp = myBornTimePair.Where(tp => (DateTime.Now - tp.Value).TotalSeconds > 60);

            if (myBornTimePairTmp != null && myBornTimePairTmp.Count() > 0)
            {
                var rd = UnityEngine.Random.Range(0, myBornTimePairTmp.Count());
                var beastTime = myBornTimePairTmp.ElementAt(rd);
                var beastId = beastTime.Key;
                var beast = myBeasts[beastId];
                ActiveBeastOnScreen(Team.My, myLaneFlyIndexs, myLaneFlyIndexs, myGate, beast, fakeLanes[beastTime.Key]);

                // var beastPref = BeastPrefs.Instance.GetBeastPref(beastTime.Key);
                // var beast = GenBeast(myLaneIndexs, myLaneFlyIndexs, myGate, beastPref, fakeLanes[beastTime.Key]);

                myBornTimePair[beastTime.Key] = DateTime.Now.AddMinutes(200);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator GenEnemyBeasts()
    {
        yield return null;
        if (offEnemy) yield break;
        enemyBornTimePair.Clear();

        BigIntCSharp totalBeastHp = 0;
        var fakeLanes = new Dictionary<BeastId, int>();
        var hasFrostQueen = enemyMonsterDatas.Exists(dt => dt.beastId == BeastId.FrostQueen);
        foreach (var data in enemyMonsterDatas)
        {
            enemyBornTimePair.Add(data.beastId, DateTime.Now.AddMinutes(-200));
            fakeLanes.Add(data.beastId, data.fakeLane);

            var beastPref = BeastPrefs.Instance.GetBeastPref(data.beastId);
            var beast = GenBeast2(beastPref, Team.Enemy);
            if (hasFrostQueen)
                beast.perDmgWhenFreezeExplode = 1;
            beast.Active(false, Team.Enemy, new Vector3(1000, 1000, 0), 0, 0);
            enemyBeasts.Add(beast.beastId, beast);

            totalBeastHp += beast.curHp;
        }

        var enemyGateGO = ObjectPool.Instance.GetGameObject(gatePref, enemyGate.position, Quaternion.identity);
        var enemyGateOb = enemyGateGO.GetComponent<Gate>();
        // gateHp = GetAverageHP(enemyMonsterDatas, false);
        var gateHp = totalBeastHp / enemyMonsterDatas.Count;
        var id = AddEntity(enemyGateOb);
        enemyGateOb.Setup(Team.Enemy, gateHp <= 0 ? 2000 : gateHp, id);
        Debug.Log("Enemy GetAverageHP:" + gateHp);

        while (IsPlaying())
        {
            var enemyBornTimePairTmp = enemyBornTimePair.Where(tp => (DateTime.Now - tp.Value).TotalSeconds > 60);

            if (enemyBornTimePairTmp != null && enemyBornTimePairTmp.Count() > 0)
            {
                var rd = UnityEngine.Random.Range(0, enemyBornTimePairTmp.Count());
                var beastTime = enemyBornTimePairTmp.ElementAt(rd);
                var beastId = beastTime.Key;
                var beast = enemyBeasts[beastId];
                ActiveBeastOnScreen(Team.Enemy, enemyLaneIndexs, enemyLaneFlyIndexs, enemyGate, beast, fakeLanes[beastTime.Key]);

                // var beastPref = BeastPrefs.Instance.GetBeastPref(beastTime.Key);
                // var beast = GenBeast(enemyLaneIndexs, enemyLaneFlyIndexs, enemyGate, beastPref, fakeLanes[beastTime.Key]);

                enemyBornTimePair[beastTime.Key] = DateTime.Now.AddMinutes(200);
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void ActiveBeastOnScreen(Team team, Dictionary<int, int> laneIndexs, Dictionary<int, int> laneFlyIndexs, Transform gate, BeastBase beast, int fakeLane = -1)
    {
        Transform lane = default;
        int laneIndex;

        if (beast.moveType == MoveType.Ground)
        {
            var minValue = laneIndexs.OrderBy(l => l.Value).FirstOrDefault().Value;

            var laneSameValues = laneIndexs.Where(l => l.Value == minValue);

            laneIndex = laneSameValues.ElementAt(UnityEngine.Random.Range(0, laneSameValues.Count())).Key;

            if (fakeLane != -1) laneIndex = fakeLane;

            lane = laneGround.GetChild(laneIndex);

            laneIndexs[laneIndex]++;
        }
        else
        {
            var minValue = laneFlyIndexs.OrderBy(l => l.Value).FirstOrDefault().Value;
            var laneSameValues = laneFlyIndexs.Where(l => l.Value == minValue);
            laneIndex = laneSameValues.ElementAt(UnityEngine.Random.Range(0, laneSameValues.Count())).Key;
            lane = laneFly.GetChild(laneIndex);
            laneFlyIndexs[laneIndex]++;
        }

        // Debug.Log(laneIndex);
        var tmpLane = new Vector3(gate.position.x, lane.position.y, 0);
        var worldIndex = AddEntity(beast);
        beast.Active(true, team, tmpLane, laneIndex, worldIndex);
    }

    private BeastBase GenBeast2(GameObject beastPref, Team team)
    {
        var beastGo = ObjectPool.Instance.GetGameObject(beastPref, Vector3.zero, Quaternion.identity);
        var beast = beastGo.GetComponent<BeastBase>();

        Debug.Log($"beast first:{beast.beastId} {team} hp:{beast.hp} def:{beast.def} atk:{beast.atk}");

        var beastData = team == Team.My ? myMonsterDatas.FirstOrDefault(dt => dt.beastId == beast.beastId)
            : enemyMonsterDatas.FirstOrDefault(dt => dt.beastId == beast.beastId);
        var newAtk = StatHelper.GetBaseAtribute(beast.atk, beastData.curStar, beastData.curLevel, out _, out _);
        var newDef = StatHelper.GetBaseAtribute(beast.def, beastData.curStar, beastData.curLevel, out _, out _);
        var newHp = StatHelper.GetBaseAtribute(beast.hp, beastData.curStar, beastData.curLevel, out _, out _);
        BigIntCSharp newSubHp = default;
        if (beast.beastClass == BeastClass.Knight)
            newSubHp = BigIntegerHelper.BigMultiplyFloat(newHp, 0.2f);

        beast.Setup(team, 0, 0, newHp, newSubHp, newDef, newAtk, beastData.curStar);

        Debug.Log($"beast after:{beast.beastId} {team} newHp:{beast.curHp} newDef:{beast.curDef} newAtk:{beast.curAtk} star:{beast.curStar} level:{beastData.curLevel}");

        ApplyTeamBuff(beast, team == Team.My);

        return beast;
    }

    public int AddEntity(EntityBase entity)
    {
        WorldIndex++;
        AllEntities[WorldIndex] = entity;
        return WorldIndex;
    }

    public EntityBase GetEntity(int index)
    {
        if (AllEntities.ContainsKey(index))
        {
            return AllEntities[index];
        }
        return null;
    }

    public bool IsPlaying()
    {
        return currentGameState == GameState.Start;
    }

    public void GameOver(bool isWin)
    {
        if (IsGameOver()) return;

        currentGameState = isWin ? GameState.Win : GameState.Lose;

        // StopAllBeastMove();

        if (currentGameState == GameState.Win)
        {
            rewardModels = RewardHelper.GetStageRewards(PlayerData.instance.CurStage, enemyMonsterDatas);
            SaveEndGame();

            AudioManager.instance.EndBattle();
            StartCoroutine(ShowEndGameUI(isWin));
        }
        else
        {
            var camPos = CameraController.instance.transform.position;
            Camera camera = Camera.main;
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;

            if (myGateGo.GetComponent<EntityBase>().isDie
                && myGateGo.transform.position.x < camPos.x - halfWidth)
            {
                CameraController.instance.MoveToStartPos();
            }

            AudioManager.instance.EndBattle();
            StartCoroutine(ShowEndGameUI(isWin, 3f));
        }
    }

    private void SaveEndGame()
    {
        var cnt = beastInfoItems.Where(bi => bi.curTeam == Team.My && bi.isDie).Count();
        var star = 0;
        if (cnt == 0) star = 3;
        else if (cnt < 3) star = 2;
        else if (cnt >= 3) star = 1;

        // end game save
        if (PlayerData.instance.CurStage == PlayerData.instance.MaxStage)
            PlayerData.instance.MaxStage = PlayerData.instance.CurStage + 1;

        var mmd = new StageModel()
        {
            numStar = star,
            stage = PlayerData.instance.CurStage
        };
        PlayerData.instance.AddStage(mmd, save: true);
    }

    private void StopAllBeastMove()
    {
        var keys = AllEntities.Keys;
        foreach (var key in keys)
        {
            var bb = AllEntities[key] as BeastBase;
            if (bb != null && !bb.isDie)
            {
                bb.StopAll();
            }
        }
    }

    private IEnumerator ShowEndGameUI(bool isWin, float timeDelay = 2f)
    {
        yield return new WaitForSeconds(timeDelay);
        // ObjectPool.Instance.ReleaseAll();

        UIGameManager.instance.EndGame(isWin, beastInfoItems, rewardModels);
    }

    public bool IsGameOver()
    {
        return currentGameState == GameState.Lose || currentGameState == GameState.Win;
    }

    public void ApplyTeamBuff(BeastBase beast, bool isMyTeam)
    {
        var main = isMyTeam ? myTeamBuffData : enemyTeamBuffData;
        var sub = isMyTeam ? enemyTeamBuffData : myTeamBuffData;

        var perHp = 0f;
        var perDef = 0f;
        var perMoveSpeed = 0f;
        var perAtk = 0f;
        var perCritFactor = 0f;
        var perFreezeDuration = 0f;
        var perCritRate = 0f;
        var perAttackRate = 0f;

        foreach (var data in main)
        {
            var key = data.Key;

            TeamBuffHelper.RevertFromKey(key, out Element buffFor, out BeastClass buffForClass, out TeamBuff buffType);
            // var buffFor = (Element)(key / 1000);
            // var buffType = (TeamBuff)(key % 1000);

            if (buffFor == beast.element || buffFor == Element.All || buffForClass == beast.beastClass
                || (buffFor == Element.Self && beast.buffForElement == Element.Self))
            {
                switch (buffType)
                {
                    case TeamBuff.Hp:
                        perHp += data.Value;
                        break;
                    case TeamBuff.Def:
                        perDef += data.Value;
                        break;
                    case TeamBuff.MoveSpeed:
                        perMoveSpeed += data.Value;
                        break;
                    case TeamBuff.Atk:
                        perAtk += data.Value;
                        break;
                    case TeamBuff.CritFactor:
                        perCritFactor += data.Value;
                        break;
                    case TeamBuff.FreezeDuration:
                        perFreezeDuration += data.Value;
                        break;
                    case TeamBuff.ChanceSpawnFrenzy:
                        perHp += data.Value;
                        perDef += data.Value;
                        perAtk += data.Value;
                        perMoveSpeed += data.Value;
                        perAttackRate += data.Value;
                        perCritRate += data.Value;
                        perCritFactor += data.Value;
                        break;
                    case TeamBuff.MakeFreezeExplode:
                        beast.perDmgWhenFreezeExplode += data.Value;
                        break;
                    case TeamBuff.AtkPerSec:// attack rate
                        perAttackRate += data.Value;
                        break;
                }
            }
        }

        foreach (var data in sub)
        {
            var key = data.Key;
            TeamBuffHelper.RevertFromKey(key, out Element buffFor, out BeastClass buffForClass, out TeamBuff buffType);

            // var buffFor = (Element)(key / 1000);
            // var buffType = (TeamBuff)(key % 1000);

            if (buffFor == beast.element || buffFor == Element.All || buffForClass == beast.beastClass)
            {
                switch (buffType)
                {
                    case TeamBuff.EnemyHp:
                        perHp += data.Value;
                        break;
                    case TeamBuff.EnemyDef:
                        perDef += data.Value;
                        break;
                    case TeamBuff.EnemyMoveSpeed:
                        perMoveSpeed += data.Value;
                        break;
                    case TeamBuff.EnemyAtk:
                        perAtk += data.Value;
                        break;
                    case TeamBuff.EnemyCritFactor:
                        perCritFactor += data.Value;
                        break;
                    case TeamBuff.EnemyFreezeDuration:
                        perFreezeDuration += data.Value;
                        break;
                    case TeamBuff.EnemyAtkPerSec:
                        perAttackRate += data.Value;
                        break;
                }
            }
        }

        Debug.Log($"perHp:{perHp} perDef:{perDef} perMoveSpeed:{perMoveSpeed} perAtk:{perAtk} perCritFactor:{perCritFactor} perFreezeDuration:{perFreezeDuration}");

        beast.DebugAtribute("before apply team buff " + isMyTeam);

        if (perFreezeDuration != 0 && beast.element == Element.Water)
        {
            for (int k = 0; k < beast.skillDes.Count; k++)
            {
                var skillDes = beast.skillDes[k];
                if (skillDes.effect == AbilityType.Freeze)
                {
                    skillDes.chance += perFreezeDuration;
                }
            }
        }
        beast.IncrAtribute(perHp, perDef, perAtk, 0, perMoveSpeed, 0, perAttackRate, 0, perCritRate, perCritFactor, 0, 0);

        beast.DebugAtribute("after apply team buff " + isMyTeam);
    }

    public Vector3 GetRandomLanePosition()
    {
        lanePositions.Clear();
        for (int i = 0; i < laneGround.childCount; i++)
        {
            lanePositions.Add(laneGround.GetChild(i).position);
        }

        // for (int i = 0; i < laneFly.childCount; i++)
        // {
        //     lanePositions.Add(laneFly.GetChild(i).position);
        // }
        return lanePositions[UnityEngine.Random.Range(0, lanePositions.Count)];
    }

    public List<Vector3> GetPosGroundLanes()
    {
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < laneGround.childCount; i++)
        {
            pos.Add(laneGround.GetChild(i).position);
        }

        // for (int i = 0; i < laneFly.childCount; i++)
        // {
        //     lanePositions.Add(laneFly.GetChild(i).position);
        // }
        return pos;
    }

    public List<EntityBase> GetEnemiesInScreen(bool onlyBeast = false)
    {
        List<EntityBase> entities = new List<EntityBase>();

        var camPos = CameraController.instance.transform.position;
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        for (int k = 0; k < AllEntities.Count; k++)
        {
            var entity = AllEntities.ElementAt(k).Value;
            if (entity != null && !entity.isDie && entity.currentTeam == Team.Enemy && entity.transform.position.x >= camPos.x - halfWidth
                && entity.transform.position.x <= camPos.x + halfWidth)
            {
                if (onlyBeast)
                {
                    if (entity.entityType == EntityType.Beast) entities.Add(entity);
                }
                else
                    entities.Add(entity);
            }
        }
        return entities;
    }

    public List<EntityBase> GetEntityInTeam(Team team, bool onlyBeast = false)
    {
        List<EntityBase> entities = new List<EntityBase>();

        var camPos = CameraController.instance.transform.position;
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        for (int k = 0; k < AllEntities.Count; k++)
        {
            var entity = AllEntities.ElementAt(k).Value;
            if (entity != null && !entity.isDie && entity.currentTeam == team)
            {
                if (onlyBeast)
                {
                    if (entity.entityType == EntityType.Beast) entities.Add(entity);
                }
                else
                    entities.Add(entity);
            }
        }
        return entities;
    }

    public List<BeastBase> GetBeastInTeam(Team team)
    {
        var res = new List<BeastBase>();
        List<EntityBase> entities = GetEntityInTeam(team, true);
        foreach (var en in entities)
        {
            var bb = en as BeastBase;
            if (bb != null)
                res.Add(bb);
        }
        return res;
    }

    public EntityBase GetNearestEnemy()
    {
        EntityBase entityBase = null;
        float minX = float.MaxValue;

        for (int k = 0; k < AllEntities.Count; k++)
        {
            var entity = AllEntities.ElementAt(k).Value;
            if (entity != null && !entity.isDie && entity.currentTeam == Team.Enemy)
            {
                if (minX > entity.transform.position.x)
                {
                    minX = entity.transform.position.x;
                    entityBase = entity;
                }
            }
        }
        return entityBase;
    }

    IEnumerator TeamBuffByTime(bool myTeam)
    {
        var beasts = myTeam ? myBeasts : enemyBeasts;
        var opBeasts = myTeam ? enemyBeasts : myBeasts;

        var buffData = myTeam ? myTeamBuffData : enemyTeamBuffData;
        var opBuffData = myTeam ? enemyTeamBuffData : myTeamBuffData;

        while (IsPlaying())
        {
            yield return new WaitForSeconds(1f);
            var beastKeys = beasts.Keys;

            bool hasAtkPerSec = false;

            Dictionary<BeastId, float> perAtk = new Dictionary<BeastId, float>();

            foreach (var data in buffData)
            {
                var key = data.Key;
                TeamBuffHelper.RevertFromKey(key, out Element buffFor, out BeastClass buffForClass, out TeamBuff buffType);
                // Debug.Log("TeamBuffByTime:" + key + " buffFor:" + buffFor + " buffType:" + buffType);

                if (buffType == TeamBuff.AtkPerSec)
                {
                    hasAtkPerSec = true;
                    for (int k = 0; k < beastKeys.Count; k++)
                    {
                        var beast = beasts[beastKeys.ElementAt(k)];
                        if (beast.element != buffFor && buffFor != Element.All && buffForClass != beast.beastClass) continue;

                        switch (buffType)
                        {
                            case TeamBuff.AtkPerSec:
                                if (perAtk.ContainsKey(beast.beastId)) perAtk[beast.beastId] += data.Value;
                                else perAtk.Add(beast.beastId, data.Value);
                                // Debug.Log("beast:" + beast.beastId + " newAtk:" + beast.curAtk);
                                break;
                        }
                    }
                }
            }

            foreach (var data in opBuffData)
            {
                var key = data.Key;
                TeamBuffHelper.RevertFromKey(key, out Element buffFor, out BeastClass buffForClass, out TeamBuff buffType);
                // Debug.Log("TeamBuffByTime:" + key + " buffFor:" + buffFor + " buffType:" + buffType);

                if (buffType == TeamBuff.EnemyAtkPerSec)
                {
                    hasAtkPerSec = true;
                    for (int k = 0; k < beastKeys.Count; k++)
                    {
                        var beast = beasts[beastKeys.ElementAt(k)];
                        if (beast.element != buffFor && buffFor != Element.All && buffForClass != beast.beastClass) continue;

                        switch (buffType)
                        {
                            case TeamBuff.EnemyAtkPerSec:
                                if (perAtk.ContainsKey(beast.beastId)) perAtk[beast.beastId] += data.Value;
                                else perAtk.Add(beast.beastId, data.Value);
                                // Debug.Log("beast:" + beast.beastId + " newAtk:" + beast.curAtk);
                                break;
                        }
                    }
                }
            }

            if (!hasAtkPerSec) yield break;

            for (int k = 0; k < beastKeys.Count; k++)
            {
                var beast = beasts[beastKeys.ElementAt(k)];

                // var aps = beast.effectBySkill.FirstOrDefault(ebs => ebs.abilityType == AbilityType.AtkPerSec);
                // if (aps != null)
                // {
                //     if (aps.time > 0)
                //     {
                //         if (perAtk.ContainsKey(beast.beastId)) perAtk[beast.beastId] += aps.value;
                //         else perAtk.Add(beast.beastId, aps.value);
                //         aps.time -= 1;
                //     }
                //     else{
                //         beast.effectBySkill.Remove(aps);
                //     }
                // }

                if (perAtk.ContainsKey(beast.beastId))
                {
                    beast.curAtk += BigIntegerHelper.BigMultiplyFloat(beast.curAtk, perAtk[beast.beastId] / 100);
                    Debug.Log("beast:" + beast.beastId + " newAtk:" + beast.curAtk);
                }
            }
        }
    }

    #region UI
    public void UpdateBeastItemView(Team team, BeastId beastId, int moreDamage, int moreDamaged, bool isDie)
    {
        BeastInfoItem beastInfoItem = null;
        try
        {
            beastInfoItem = beastInfoItems.First(bi => bi.beastId == beastId && team == bi.curTeam);
            // if (beastInfoItem == null) return;
        }
        catch (Exception ex)
        {
            Debug.LogError("UpdateBeastItemView team:" + team + " beastId:" + beastId + " ex:" + ex.StackTrace);
        }
        // var beastInfoItem = beastInfoItems.FirstOrDefault(bi => bi.beastId == beastId && team == bi.curTeam);
        // if(beastInfoItem == null) return;

        beastInfoItem.damage += moreDamage;
        beastInfoItem.damaged += moreDamaged;
        beastInfoItem.isDie = isDie;
        // Debug.Log(beastInfoItem.beastName + " damage:" + beastInfoItem.damage + " moreDamage:" + moreDamage);
        // Debug.Log(beastInfoItem.beastName + " damaged:" + beastInfoItem.damaged + " moreDamaged:" + moreDamaged);

        if (team == Team.My)
            UIGameManager.instance.UpdateBeastInfoItems(beastInfoItems);
    }
    #endregion
}
