using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VideoGameManager : MonoBehaviour
{
    public List<GameObject> Weapons=new List<GameObject>();
    public GameObject bigBallPrefab;//大招的球
    public List<Color32> roleColors = new List<Color32>();
    public GameObject currentReward = null;
    public List<BaseRole> circleRoles=new List<BaseRole>();
    public RoleUI roleUIPrefab;//英雄状态UI 
    public GameObject CantGetWeaponPrefab;//无法拾取武器的标识
    public GameObject CantUseBigPrefab;//无法使用大招的标识

    public Transform roleUIParent;

    public float ballSpeed=20;
    private float kniefHoldTime = 5;
    private float kniefRotateSpeed = 180;

    public Text ballSpeedText;
    public Text kniefHoldTimeText;
    public Text kniefRotateSpeedText;

    public bool gameEnd = false;

    public static VideoGameManager instance;
    //流光相关
    public RectTransform canvasTransform; // UI Canvas
    public GameObject flyLightPrefab; // 一个流光粒子图标预制体（UI Image）
    private readonly Queue<GameObject> flyLightPool = new Queue<GameObject>();

    public float KniefHoldTime { 
        get => kniefHoldTime;
        set
        {
            kniefHoldTime = value;
            kniefHoldTimeText.text = KniefHoldTime.ToString("F1");
        }
    }

    public float KniefRotateSpeed 
    { 
        get => kniefRotateSpeed; 
        set
        {
            kniefRotateSpeed = value;
            kniefRotateSpeedText.text = Mathf.RoundToInt(KniefRotateSpeed).ToString();
        }
    }

    public static UnityAction<GameObject> OnBallGenerate;//大招的小球生成

    void Awake()
    {
        instance=this;
    }

    private void Start()
    {
        for(int i = 0;i< circleRoles.Count; i++)
        {
            RoleUI roleUI = Instantiate(roleUIPrefab,roleUIParent);
            roleUI.nameText.text = circleRoles[i].RoleName;
            roleUI.roleImage.sprite = circleRoles[i].roleImage.sprite;
            circleRoles[i].roleUI = roleUI;
            circleRoles[i].roleColor = roleColors[i];
        }

        Invoke(nameof(SpawnWeapons), 3);
        Invoke(nameof(SpawnBigBall), 3);
        StartCoroutine(ChangeValue());
        ballSpeedText.text = ballSpeed.ToString("F1");
        kniefHoldTimeText.text = KniefHoldTime.ToString("F1");
        kniefRotateSpeedText.text = Mathf.RoundToInt(KniefRotateSpeed).ToString();
    }


    public void RoleDie(BaseRole role)
    {
        circleRoles.Remove(role);
        if (circleRoles.Count == 1)
        {
            gameEnd = true;
            circleRoles[0].transform.Find("Canvas").GetComponent<Canvas>().sortingOrder=99;

            circleRoles[0].transform.DOMove(Vector3.zero, 1.5f);

            // 缩放到 (20,20,20) 用时2秒（假设是等比放大）
            circleRoles[0].transform.DOScale(Vector3.one * 20f, 1.5f);
        }
    }

    public void SpawnWeapons()
    {
        StartCoroutine(SpawnWeaponsCoroutine());
    }

    private IEnumerator SpawnWeaponsCoroutine()
    {
        int index = Random.Range(0, Weapons.Count); // 包含0，不包含Rewards.Count
        currentReward = Weapons[index];
        if (currentReward.activeSelf) yield break;
        Vector2 randomPos = new Vector2(Random.Range(-9f, 13f), Random.Range(-13f, 13.5f));
        currentReward.transform.SetParent(null); // 解除父子关系
        currentReward.transform.position = randomPos; // 设置为世界坐标
        currentReward.SetActive(false);
        yield return new WaitForSeconds(Random.Range(1.0f,2.0f));
        currentReward.SetActive(true);
        yield break;
    }

    public void SpawnBigBall()
    {
        StartCoroutine(SpawnBigBallCoro());
    }

    private IEnumerator SpawnBigBallCoro()
    {
        while (true)
        {
            Vector2 randomPos = new Vector2(Random.Range(-9f, 13f), Random.Range(-13f, 13.5f));
            GameObject tmpBall= Instantiate(bigBallPrefab, randomPos,Quaternion.identity);
            OnBallGenerate?.Invoke(tmpBall);
            yield return new WaitForSeconds(4);
        }
    }

    private IEnumerator ChangeValue()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            ballSpeed += 0.1f;
            ballSpeedText.text= ballSpeed.ToString("F1");

            yield return new WaitForSeconds(2.0f);
        }
    }

    /// <summary>
    /// 播放流光特效。
    /// </summary>
    /// <param name="screenStart">屏幕起点坐标。</param>
    /// <param name="screenEnd">屏幕终点坐标。</param>
    /// <param name="count">本次播放的流光数量。</param>
    public void PlayFlyEffect(Vector3 screenStart, Vector3 screenEnd, int count = 3)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject flyCoin = GetFlyLight();
            PrepareFlyLight(flyCoin, screenStart);

            // 计算中间控制点（弧线弯曲点）
            Vector3 midPoint = (screenStart + screenEnd) / 2f;

            // 添加随机偏移，使每个轨迹略有不同
            float horizontalOffset = Random.Range(-100f, 100f); // 左右
            float verticalOffset = Random.Range(100f, 200f);    // 向上更高一点

            midPoint += new Vector3(horizontalOffset, verticalOffset, 0f);

            // 设置路径
            Vector3[] path = new Vector3[] { screenStart, midPoint, screenEnd };

            // 使用 DoTween 路径飞行
            flyCoin.transform
                .DOPath(path, 0.6f, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    ReturnFlyLight(flyCoin);
                    // 可触发粒子、音效等
                });
        }
    }

    /// <summary>
    /// 从流光对象池取出一个可用对象，池为空时才创建新对象。
    /// </summary>
    /// <returns>可用于播放飞行动画的流光对象。</returns>
    private GameObject GetFlyLight()
    {
        if (flyLightPool.Count > 0)
        {
            GameObject pooledFlyLight = flyLightPool.Dequeue();
            pooledFlyLight.SetActive(true);
            return pooledFlyLight;
        }

        return Instantiate(flyLightPrefab, canvasTransform);
    }

    /// <summary>
    /// 重置流光对象的父节点、位置和变换状态。
    /// </summary>
    /// <param name="flyCoin">需要播放的流光对象。</param>
    /// <param name="screenStart">屏幕起点坐标。</param>
    private void PrepareFlyLight(GameObject flyCoin, Vector3 screenStart)
    {
        flyCoin.transform.DOKill();
        flyCoin.transform.SetParent(canvasTransform, false);
        flyCoin.transform.localRotation = flyLightPrefab.transform.localRotation;
        flyCoin.transform.localScale = flyLightPrefab.transform.localScale;
        flyCoin.transform.position = screenStart;
    }

    /// <summary>
    /// 停止流光旧 Tween，并把对象放回池中等待复用。
    /// </summary>
    /// <param name="flyCoin">需要回收的流光对象。</param>
    private void ReturnFlyLight(GameObject flyCoin)
    {
        if (flyCoin == null)
        {
            return;
        }

        flyCoin.transform.DOKill();
        flyCoin.SetActive(false);
        flyLightPool.Enqueue(flyCoin);
    }
}
