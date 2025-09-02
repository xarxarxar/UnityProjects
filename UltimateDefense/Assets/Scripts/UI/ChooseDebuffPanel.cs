using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class ChooseDebuffPanel : MonoBehaviour
{
    public static event UnityAction<Debuff> OnDebuffChooseEnd;//debuff选择完毕
    [SerializeField] private Text _titleText;//标题
    [SerializeField]private Debuff _debuff;
    [SerializeField]private Button startChallengeButton;//开始挑战按钮
    [SerializeField]private Button _giveupChallengeButton;//放弃挑战按钮
    //[SerializeField]private DebuffStruct _debuffStructPrefab;//debuffStruct预制体
    //[SerializeField]private List<DebuffStruct>  _debuffStructs=new List<DebuffStruct>();//debuffStruct预制体
    [SerializeField]private Transform _debuffStructParent;//debuffStruct预制体生成的父物体
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
        //if (DataManager.Instance.PlayerInfo.PassCount.Value == 0)//如果通关次数为0，则直接跳过这一步
        //{
        //    OnDebuffChooseEnd?.Invoke(new Debuff(_debuff));
        //    gameObject.SetActive(false);
        //}
        _debuff = new Debuff();
        _chooseCount = 0;
        //_titleText.text = $"您已通关了{DataManager.Instance.PlayerInfo.PassCount.Value}次\r\n需抽取{DataManager.Instance.PlayerInfo.PassCount.Value}个Debuff再进行挑战";

        SetButtonStatus(startChallengeButton, false);
        InitAllDebuffStructs();//初始化debuffstruct
        startChallengeButton.onClick.AddListener(StartChallengeButton);
        _giveupChallengeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            GameUIManager.Instance.ShowMainMenu();
        });

        DebuffStruct.OnChangeDebuffStruct += OnChangeDebuffStruct;
    }

    private void OnDisable()
    {
        debuffStructs.Clear();
        foreach (Transform child in _debuffStructParent)
        {
            Destroy(child.gameObject);
        }


        startChallengeButton.onClick.RemoveAllListeners();
        DebuffStruct.OnChangeDebuffStruct -= OnChangeDebuffStruct;
    }

    //开始挑战
    private void StartChallengeButton()
    {
        //OnDebuffChooseEnd?.Invoke(new Debuff(_debuff));
        //gameObject.SetActive(false);

        PlayAnim();
    }

    //初始化debuffstruct
    private void InitAllDebuffStructs()
    {
        
    }

    //debuffstruct改变时
    private void OnChangeDebuffStruct(DebuffType debuffType,int delta)
    {
        switch (debuffType)
        {
            case DebuffType.AddHP:
                _debuff.AddHP = Mathf.Max(0, _debuff.AddHP + delta);
                break;
            case DebuffType.AddSpeed:
                _debuff.AddSpeed = Mathf.Max(0, _debuff.AddSpeed + delta);
                break;
            case DebuffType.AddCount:
                _debuff.AddCount = Mathf.Max(0, _debuff.AddCount + delta);
                break;
            case DebuffType.DamageNullified:
                _debuff.DamageNullified = Mathf.Max(0, _debuff.DamageNullified + delta);
                break;
        }
        _chooseCount = _debuff.AddHP + _debuff.AddSpeed + _debuff.AddCount+ _debuff.DamageNullified;

        //设置开始挑战按钮的状态
        SetButtonStatus(startChallengeButton, _chooseCount >= DataManager.Instance.PlayerInfo.PassCount.Value);
        
    }

    //设置按钮的状态
    private void SetButtonStatus(Button button,bool active)
    {

        
    }

    //播放抽取动画
    private void PlayAnim()
    {
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
            1.0f
        ).SetEase(Ease.OutQuad);

        // 2. target 渐显
        Tween fadeTween = target.DOFade(1f, 1.0f).SetEase(Ease.Linear);

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
            CanvasGroup canvasGroup = img.GetComponent<CanvasGroup>();

            // 透明到 0
            canvasGroup.alpha = 0;
            img.gameObject.SetActive(false);
            completedCount++;

            // 等所有 images 完成
            if (completedCount == debuffStructs.Count)
            {
                PlayTargetAnimation();
            }
        });
    }

    private void PlayTargetAnimation()
    {
        // 先把 rotation 归零，避免累计误差
        target.rectTransform.localRotation = Quaternion.identity;

        // 创建序列
        Sequence seq = DOTween.Sequence();

        // 摇三次（每次左右一次）
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
        _debuffStructParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(400, 100);
        Dictionary<DebuffStruct, int> chosenCount = new Dictionary<DebuffStruct, int>();
        Vector3 flyEffectEndPos = Vector3.zero;

        for (int i = 0; i < 10; i++)
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

            if (!chosenCount.ContainsKey(selected))
            {
                chosenCount[selected] = 0;
                DebuffStruct tmpDebuff = Instantiate(selected, _debuffStructParent);
                tmpDebuff.gameObject.SetActive(true);
                tmpDebuff.GetComponent<CanvasGroup>().alpha = 1.0f;
                tmpDebuff._debuffType = selected._debuffType;
                Canvas.ForceUpdateCanvases();
                flyEffectEndPos = tmpDebuff.transform.position;
            }
            else
            {
                foreach (Transform child in _debuffStructParent)
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
            yield return StartCoroutine(GameUIManager.Instance.PlayFlyEffectAsync(screenStart, screenEnd, 3,1.0f/3));

            chosenCount[selected]++;
            Debug.Log($"选中: {selected._debuffType}, 当前次数 = {chosenCount[selected]}");
        }
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
    /// 敌人增加10%的移速的个数
    /// </summary>
    public int AddSpeed;

    /// <summary>
    /// 敌人增加10%的数量的个数
    /// </summary>
    public int AddCount;

    /// <summary>
    /// 敌人免疫伤害次数
    /// </summary>
    public int DamageNullified;

    public Debuff()
    {
        AddHP = 0; AddSpeed=0; AddCount = 0;
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

    public static string GetDescription(DebuffType type)
    {
        switch (type)
        {
            case DebuffType.AddHP:
                return "敌人生命值增加 10%";
            case DebuffType.AddSpeed:
                return "敌人移动速度增加 10%";
            case DebuffType.AddCount:
                return "敌人数量增加 10%";
            case DebuffType.DamageNullified:
                return "敌人免疫伤害次数";
            default:
                return "未知效果";
        }
    }
}