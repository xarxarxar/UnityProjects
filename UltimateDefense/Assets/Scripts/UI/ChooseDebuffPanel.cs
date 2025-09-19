using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseDebuffPanel : MonoBehaviour
{
    public static event UnityAction<Debuff> OnDebuffChooseEnd;//debuff选择完毕
    [SerializeField] private Text _titleText;//标题
    [SerializeField]private Debuff _debuff;
    [SerializeField]private Button startChooseButton;//开始抽取按钮
    [SerializeField]private Button startChallengeButton;//开始挑战按钮
    [SerializeField]private Button _giveupChallengeButton;//放弃挑战按钮
    //[SerializeField]private DebuffStruct _debuffStructPrefab;//debuffStruct预制体
    //[SerializeField]private List<DebuffStruct>  _debuffStructs=new List<DebuffStruct>();//debuffStruct预制体
    [SerializeField]private Transform _debuffStructParent;//debuffStruct预制体生成的父物体
    [SerializeField]private Transform _debuffStructRealParent;//debuffStruct预制体最终的父物体
    [SerializeField]private List<DebuffStruct> debuffStructs=new List<DebuffStruct>();//所有的DebuffStruct
    private int _chooseCount = 0;//选择debuff的个数，个数不能大于通关次数

    //动画
    private int completedCount = 0;//完成的动画个数
    [SerializeField] private Image target;//卡片的目标位置
    private float duration = 1.0f;           // 飞行时长
    private Vector2 horizontalOffsetRange = new Vector2(-100f, 100f);//卡片飞行的轨迹偏移
    private Vector2 verticalOffsetRange = new Vector2(100f, 200f);//卡片飞行的轨迹偏移
    [SerializeField] private Sprite boxOpen; // 宝箱打开的sprite
    [SerializeField] private Sprite boxClosed; // 宝箱关闭的sprite

    private void OnEnable()
    {
        if (DataManager.Instance.PlayerInfo.PassCount.Value == 0)//如果通关次数为0，则直接跳过这一步
        {
            OnDebuffChooseEnd?.Invoke(new Debuff(_debuff));
            gameObject.SetActive(false);
            return;
        }
        _debuff = new Debuff();
        _chooseCount = 0;
        _titleText.text = $"您已通关了{DataManager.Instance.PlayerInfo.PassCount.Value}次\r\n需抽取{DataManager.Instance.PlayerInfo.PassCount.Value}个Debuff再进行挑战";

        _debuffStructParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(430, 100);
        Canvas.ForceUpdateCanvases();
        startChooseButton.gameObject.SetActive(true);
        startChallengeButton.gameObject.SetActive(false);

        startChooseButton.GetComponent<Transform>().localScale = Vector3.one;
        startChallengeButton.GetComponent<Transform>().localScale = Vector3.one;
        _giveupChallengeButton.GetComponent<Transform>().localScale = Vector3.one;


        startChooseButton.onClick.AddListener(StartChooseButton);
        startChallengeButton.onClick.AddListener(StartChallenge);
        _giveupChallengeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            GameUIManager.Instance.ShowMainMenu();
        });

    }

    private void OnDisable()
    {
        _debuffStructParent.gameObject.SetActive(true);
        foreach (Transform child in _debuffStructRealParent)
        {
            Destroy(child.gameObject);
        }

        startChooseButton.onClick.RemoveAllListeners();
        startChallengeButton.onClick.RemoveAllListeners();
        _giveupChallengeButton.onClick.RemoveAllListeners();
        startChooseButton.gameObject.SetActive(true);
        startChallengeButton.gameObject.SetActive(false);
    }

    private void StartChallenge()
    {
        OnDebuffChooseEnd?.Invoke(_debuff);
        gameObject.SetActive(false);
    }

    //开始抽取debuff
    private void StartChooseButton()
    {
        _debuffStructParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(430, 100);
        Canvas.ForceUpdateCanvases();

        PlayAnim();
    }


    //播放抽取动画
    private void PlayAnim()
    {
        startChooseButton.gameObject.SetActive(false);
        _giveupChallengeButton.gameObject.SetActive(false);
        target.gameObject.SetActive(true);
        target.sprite = boxClosed;

        GridLayoutGroup grid = _debuffStructParent.GetComponent<GridLayoutGroup>();
        Vector2 currentSize = grid.cellSize;
        float targetX = 130f;

        // target 初始透明
        Color c = target.color;
        target.color = new Color(c.r, c.g, c.b, 0f);

        Sequence seq = DOTween.Sequence();

        // 1. cellSize.x 动画
        Tween gridTween = DOTween.To(
            () => grid.cellSize.x,
            x => grid.cellSize = new Vector2(x, currentSize.y),
            targetX,
            0.5f
        ).SetEase(Ease.OutQuad);

        // 2. target 渐显
        Tween fadeTween = target.DOFade(1f, 0.5f).SetEase(Ease.Linear);

        // 同时进行
        seq.Join(gridTween);
        seq.Join(fadeTween);

        // 动画完成后再执行 FlyImage
        seq.OnComplete(() =>
        {
            completedCount = 0;

            foreach (var img in debuffStructs)
            {
                FlyImage(img.GetComponent<RectTransform>());
            }
        });
    }

    private void FlyImage(RectTransform img)
    {
        Vector3 screenStart = img.position;
        Vector3 screenEnd = target.transform.position;

        // 计算中间控制点（弧线弯曲点）
        Vector3 midPoint = (screenStart + screenEnd) / 2f;

        // 添加随机偏移，使每个轨迹略有不同
        float horizontalOffset = Random.Range(horizontalOffsetRange.x, horizontalOffsetRange.y);
        float verticalOffset = Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);
        midPoint += new Vector3(horizontalOffset, verticalOffset, 0f);

        // 设置路径（三点曲线）
        Vector3[] path = new Vector3[] { screenStart, midPoint, screenEnd };

        // 拿到当前 sizeDelta
        Vector2 currentSize = img.sizeDelta;


        // 并行动画：路径移动 + 渐隐
        Sequence seq = DOTween.Sequence();
        seq.Join(// 并行动画：路径移动
        img.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad));
        

        seq.OnComplete(() =>
        {
            completedCount++;

            // 等所有 images 完成
            if (completedCount == debuffStructs.Count)
            {
                _debuffStructParent.gameObject.SetActive(false);
                PlayTargetAnimation();
            }
        });
    }

    //播放最终选择的动画
    private void PlayTargetAnimation()
    {
        // 先把 rotation 归零，避免累计误差
        target.rectTransform.localRotation = Quaternion.identity;

        // 创建序列
        Sequence seq = DOTween.Sequence();

        // 摇五次（每次左右一次）
        for (int i = 0; i < 5; i++)
        {
            seq.Append(target.rectTransform.DORotate(new Vector3(0, 0, 20f), 0.1f));
            seq.Append(target.rectTransform.DORotate(new Vector3(0, 0, -20f), 0.1f));
        }

        // 最后归位并切换 sprite
        seq.AppendCallback(() =>
        {
            target.rectTransform.localRotation = Quaternion.identity;
            if (boxOpen != null)
                target.sprite = boxOpen;

            StartCoroutine(ChooseDebuffsCoroutine());
        });
    }
    //抽取debuff
    private IEnumerator ChooseDebuffsCoroutine()
    {
        Dictionary<DebuffStruct, int> chosenCount = new Dictionary<DebuffStruct, int>();
        Vector3 flyEffectEndPos = Vector3.zero;

        for (int i = 0; i < DataManager.Instance.PlayerInfo.PassCount.Value; i++)
        {
            List<DebuffStruct> available = debuffStructs.FindAll(d =>
            {
                chosenCount.TryGetValue(d, out int current);
                return current < d.MAXCOUNT;
            });

            if (available.Count == 0)
            {
                Debug.LogWarning("没有可供选择的 DebuffStruct 了");
                break;
            }

            DebuffStruct selected = available[Random.Range(0, available.Count)];

            switch (selected._debuffType)
            {
                case DebuffType.AddSpeed:
                    _debuff.AddSpeed++;
                    break;
                case DebuffType.AddHP:
                    _debuff.AddHP++;
                    break;
                case DebuffType.InitialMoneyDecrease:
                    _debuff.InitialMoneyDecrease++;
                    break;
                case DebuffType.DamageNullified:
                    _debuff.DamageNullified++;
                    break;
                case DebuffType.CrystalMaxHpDecrease:
                    _debuff.CrystalMaxHpDecrease++;
                    break;
                case DebuffType.EliteEnemyCount:
                    _debuff.EliteEnemyCount++;
                    break;
            }

            if (!chosenCount.ContainsKey(selected))
            {
                chosenCount[selected] = 0;
                DebuffStruct tmpDebuff = Instantiate(selected, _debuffStructRealParent);
                tmpDebuff.gameObject.SetActive(true);
                tmpDebuff.GetComponent<CanvasGroup>().alpha = 1.0f;
                tmpDebuff._debuffType = selected._debuffType;
                Canvas.ForceUpdateCanvases();
                flyEffectEndPos = tmpDebuff.transform.position;
            }
            else
            {
                foreach (Transform child in _debuffStructRealParent)
                {
                    DebuffStruct debuff = child.GetComponent<DebuffStruct>();
                    if (debuff != null && debuff._debuffType == selected._debuffType)
                    {
                        flyEffectEndPos = debuff.transform.position;
                        debuff._countText.text = (chosenCount[selected] + 1).ToString();
                    }
                }
            }

            Vector3 screenStart = RectTransformUtility.WorldToScreenPoint(null, target.transform.position);
            Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(null, flyEffectEndPos);

            // 等待飞行特效完成
            yield return StartCoroutine(GameUIManager.Instance.PlayFlyEffectAsync(screenStart, screenEnd, 3,0.3f));
            AudioManager.Instance.PlaySFX("获得金币");

            chosenCount[selected]++;
        }

        startChallengeButton.gameObject.SetActive(true);
        _giveupChallengeButton.gameObject.SetActive(true);
        target.gameObject.SetActive(false);
    }
}

/// <summary>
/// 通关一次之后选择的debuff
/// </summary>
[System.Serializable]
public class Debuff
{
    /// <summary>
    /// 敌人增加10%的血量的个数
    /// </summary>
    public int AddHP;

    /// <summary>
    /// 初始金币减少10%的个数
    /// </summary>
    public int InitialMoneyDecrease;

    /// <summary>
    /// 精英怪数量增加10%的个数
    /// </summary>
    public int EliteEnemyCount;

    /// <summary>
    /// 城墙初始最大生命值减少10%的个数
    /// </summary>
    public int CrystalMaxHpDecrease;

    /// <summary>
    /// 敌人增加10%的速度
    /// </summary>
    public int AddSpeed;

    /// <summary>
    /// 敌人免疫伤害次数
    /// </summary>
    public int DamageNullified;


    public Debuff()
    {
        AddHP = 0; InitialMoneyDecrease = 0; EliteEnemyCount = 0; CrystalMaxHpDecrease = 0; AddSpeed = 0;
        DamageNullified = 0;
    }

    //拷贝一份
    public Debuff (Debuff debuff)
    {
        var fields = typeof(Debuff).GetFields();
        foreach (var field in fields)
        {
            field.SetValue(this, field.GetValue(debuff));
        }
    }

}