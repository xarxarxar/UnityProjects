using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private static Player _instance;

    // 单例模式，确保全局只有一个玩家实例
    public static Player instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Player>();  // 如果没有实例，查找场景中的Player对象
            }

            return _instance;
        }
    }

    public GuideLine guideLine;  // 引导线，用于显示小球发射的轨迹
    [SerializeField] private GameObject center;  // 小球发射器的中心位置
    [HideInInspector] public Vector3 nextPosition;  // 小球的下一个发射位置

    [HideInInspector] public List<Ball> activeBall = new List<Ball>();  // 当前激活的小球列表
    [HideInInspector] public List<Ball> inBall = new List<Ball>();  // 所有处于场中的小球列表
    [HideInInspector] public List<BlockBase> addBallBlock = new List<BlockBase>();  // 添加小球的模块

    [HideInInspector] public bool isFirst = false;  // 是否是第一次发射小球
    [HideInInspector] public GameObject shotRot;  // 发射旋转的对象
    [HideInInspector] public bool isReturnBall = false;  // 是否正在回收小球
    [SerializeField] private ParticleSystem fxGet;  // 获取小球时的粒子效果
    [SerializeField] private TextMeshProUGUI textGetBallCount;  // 显示获得小球数量的文本

    [HideInInspector] public GameObject[] balls;  // 所有可选的小球预设
    [HideInInspector] public GameObject selectBall;  // 当前选中的小球预设

    public int ballMaxCount = 1;  // 每次最多发射的小球数量
    public int ballCount = 1;  // 当前剩余的小球数量

    private int num = 0;  // 一个计数变量，可能用于其他逻辑

    /// <summary>
    /// 初始化玩家数据
    /// </summary>
    public void SetData()
    {
        // 设置选中的小球并创建小球池
        selectBall = balls[GameData.SelectBallNum];
        center.GetComponent<SpriteRenderer>().sprite = selectBall.GetComponent<Ball>().spriteBall.sprite;
        PoolManager.CreatePool(selectBall, 50, false, 0);

        // 创建一个空对象用于控制发射旋转
        shotRot = new GameObject();
        shotRot.name = "ShotRot";
        nextPosition = guideLine.transform.position;  // 获取引导线的位置
        nextPosition.y = transform.position.y;  // 保持Y轴位置不变
        CtrUI.instance.SetBallCount(ballCount);  // 更新UI上的小球数量显示
    }

    #region // 小球发射
    /// <summary>
    /// 发射小球
    /// </summary>
    public void ShotBall()
    {
        // 设置回收小球按钮可见
        CtrUI.instance.SetReturnBallButton(true);
        isReturnBall = false;

        // 设置发射旋转对象的位置和旋转
        shotRot.transform.position = guideLine.transform.position;
        shotRot.transform.rotation = guideLine.transform.rotation;

        // 锁定游戏状态，防止其他操作
        CtrGame.instance.IsLock = true;

        // 启动发射协程
        StartCoroutine(ShotBallCo());
        guideLine.GuidelineOff();  // 隐藏引导线
    }
    #endregion

    #region // 发射小球协程
    /// <summary>
    /// 发射小球的协程
    /// </summary>
    IEnumerator ShotBallCo()
    {
        center.SetActive(false);  // 隐藏小球发射中心

        Vector3 shotpos = guideLine.transform.position;  // 获取发射位置

        // 循环发射多个小球
        for (int i = 0; i < ballMaxCount; i++)
        {
            // 播放发射音效
            CtrGame.instance.ShotSound();

            // 从池中生成小球
            Ball ball = PoolManager.Spawn(selectBall, shotpos, Quaternion.identity).GetComponent<Ball>();
            ballCount -= 1;  // 剩余小球数量减一
            CtrUI.instance.SetBallCount(ballCount);  // 更新UI显示

            if (i == 0)
            {
                ball.isFirst = true;  // 标记第一次发射的小球
            }

            activeBall.Add(ball);  // 将小球加入激活列表
            ball.SetData(1);  // 设置小球的伤害值为1
            yield return new WaitForSeconds(0.035f);  // 发射间隔

            if (ballCount < 0)
            {
                ballCount = 0;  // 确保小球数量不为负
            }
        }

        // 等待所有小球发射完毕
        yield return new WaitForSeconds(0.035f);
        CtrUI.instance.textBallCount.DOFade(0f, 0.1f).SetEase(Ease.OutCubic);  // 渐变消失
        StartCoroutine(CheckTurnCo());  // 检查是否完成回合
    }
    #endregion

    #region // 检查回合是否结束
    /// <summary>
    /// 检查当前回合是否结束
    /// </summary>
    IEnumerator CheckTurnCo()
    {
        while (activeBall.Count > 0)  // 等待所有小球结束
        {
            yield return null;
        }

        StartCoroutine(ReadyPlayerCo());  // 准备下一回合
    }
    #endregion

    #region // 设置下一个发射位置
    /// <summary>
    /// 设置下一个发射位置的X坐标
    /// </summary>
    public void SetNextPositionX(float posX)
    {
        nextPosition.x = posX;
        Transform guideLineTransform;
        (guideLineTransform = guideLine.transform).DOMoveX(posX, 0f);  // 更新引导线的X坐标
        guideLineTransform.rotation = Quaternion.identity;  // 重置引导线的旋转
        center.gameObject.SetActive(true);  // 显示发射中心

        // 播放回收音效
        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_ball_comback);
    }
    #endregion

    #region // 准备下一回合
    /// <summary>
    /// 准备玩家进入下一回合
    /// </summary>
    IEnumerator ReadyPlayerCo()
    {
        CtrUI.instance.SetReturnBallButton(false);  // 隐藏回收按钮
        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_ball_comback);
        CtrUI.instance.textBallCount.DOFade(1f, 0.1f).SetEase(Ease.OutCubic);  // 显示小球数量
        CtrUI.instance.textBallCount.transform.DOMoveX(nextPosition.x, 0f);  // 更新UI位置

        // 初始化小球数量
        ballCount = ballMaxCount;
        CtrUI.instance.SetBallCount(ballMaxCount);  // 更新UI

        // 播放获取小球的动画和粒子效果
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].transform.DOKill();
            addBallBlock[i].transform.DOMove(nextPosition, 0.1f);
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_ball_comback);
            fxGet.Play();  // 播放粒子效果
        }

        yield return new WaitForSeconds(0.15f);  // 等待动画完成

        // 显示新增的小球数量
        if (addBallBlock.Count > 0)
        {
            textGetBallCount.text = $"+{addBallBlock.Count}";
            textGetBallCount.transform.DOMove(nextPosition, 0f);
            textGetBallCount.DOFade(1f, 0f);
            textGetBallCount.transform.DOMoveY(0.5f, 0.2f).SetEase(Ease.OutCubic).SetRelative(true);
            textGetBallCount.DOFade(0f, 1f).SetEase(Ease.Linear).SetDelay(0.2f);
        }

        // 删除添加的小球块
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].Destory();
        }

        // 更新小球数量
        ballCount = ballMaxCount += addBallBlock.Count;
        CtrUI.instance.SetBallCount(ballMaxCount);

        // 清空添加的小球模块列表
        addBallBlock.Clear();

        // 清空入场的小球
        inBall.Clear();
        isFirst = false;
        CtrGame.instance.NextTurn();  // 进入下一个回合
    }
    #endregion

    #region // 继续玩家回合
    /// <summary>
    /// 继续玩家回合
    /// </summary>
    public void ContinuePlayer()
    {
        StartCoroutine(ContinuePlayerCo());
    }

    IEnumerator ContinuePlayerCo()
    {
        CtrUI.instance.SetReturnBallButton(false);  // 隐藏回收按钮
        CtrUI.instance.textBallCount.DOFade(1f, 0.1f).SetEase(Ease.OutCubic);  // 显示小球数量

        // 初始化小球数量
        ballCount = ballMaxCount;

        // 更新UI显示
        CtrUI.instance.SetBallCount(ballMaxCount);

        // 播放小球增加动画
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].transform.DOKill();
            addBallBlock[i].transform.DOMove(nextPosition, 0.15f);
            fxGet.Play();
        }

        yield return new WaitForSeconds(0.15f);

        // 显示新增的小球数量
        if (addBallBlock.Count > 0)
        {
            textGetBallCount.text = $"+{addBallBlock.Count}";
            textGetBallCount.transform.DOMove(nextPosition, 0f);
            textGetBallCount.DOFade(1f, 0f);
            textGetBallCount.transform.DOMoveY(0.5f, 0.2f).SetEase(Ease.OutCubic).SetRelative(true);
            textGetBallCount.DOFade(0f, 1f).SetEase(Ease.Linear).SetDelay(0.2f);
        }

        // 删除添加的小球块
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].Destory();
        }

        // 更新小球数量
        ballCount = ballMaxCount += addBallBlock.Count;
        CtrUI.instance.SetBallCount(ballMaxCount);

        // 清空添加的小球模块
        addBallBlock.Clear();

        // 清空入场的小球
        inBall.Clear();
        isFirst = false;

        // 重置游戏状态并进入下一回合
        CtrGame.instance.isGameOver = false;
        CtrGame.instance.NextTurnMoveEnd();
    }
    #endregion

    #region // 回收小球
    /// <summary>
    /// 回收小球
    /// </summary>
    public void ReturnBall()
    {
        StopAllCoroutines();  // 停止所有协程
        CtrUI.instance.SetReturnBallButton(false);  // 隐藏回收按钮
        StartCoroutine(ReturnBallCo());
    }

    IEnumerator ReturnBallCo()
    {
        // 回收所有活跃的小球
        for (int i = 0; i < activeBall.Count; i++)
        {
            activeBall[i].ReturnBall();
        }

        yield return new WaitForSeconds(0.25f);  // 等待回收完成
        activeBall.Clear();  // 清空活跃小球列表
        SetNextPositionX(nextPosition.x);  // 更新下一发射位置

        StartCoroutine(ReadyPlayerCo());  // 准备下一回合
    }
    #endregion
}
