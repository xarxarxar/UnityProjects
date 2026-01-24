using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Button _leftButton;  //左边的升级按钮
    [SerializeField]
    private Button _rightButton; //右边的升级按钮
    [SerializeField]
    private GameObject _leftMask; //左边的已购买遮罩
    [SerializeField]
    private GameObject _rightMask; //右边的已购买遮罩
    [SerializeField]
    private Text _updatePriceText;  //刷新需要消耗的金币
    [SerializeField]
    private Button _refreshButton;//刷新升级属性的按钮
    [SerializeField]
    private GameObject _discountLeft;//左侧显示打折的物体
    [SerializeField]
    private GameObject _discountRight;//右侧显示打折的物体
    [SerializeField]
    private Text _discountTextLeft;//左侧显示打折的文本
    [SerializeField]
    private Text _discountTextRight;//右侧显示打折的文本
    [SerializeField] 
    private SimpleScrollSnap _simpleScrollSnapLeft;//左侧抽奖所在的区域
    [SerializeField]
    private SimpleScrollSnap _simpleScrollSnapRight;//右侧抽奖所在的区域
    [SerializeField]
    private MySlider coolDownSlider;//冷却条
    private bool isInCoolDown = false;//是否正在冷却

    /// <summary>
    /// 刷新增益
    /// </summary>
    public static event UnityAction OnRefreshBuff;

    private int  gold => CurrencyManager.Instance.Gold;
    private UpgradeManager _upgradeManager=>UpgradeManager.Instance;
    private bool isLeftStop=false;//左边的转轮是否停下
    private bool isRightStop=false;//右边的转轮是否停下
    private UpgradeBase _leftUpgrade=null;//左侧的升级属性
    private UpgradeBase _rightUpgrade=null;//左侧的升级属性

    private void OnDisable()
    {
        _refreshButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 初始化升级面板
    /// </summary>
    public void Init()
    {
        // 初始化左侧卡片
        for (int i = 0; i < _simpleScrollSnapLeft.NumberOfPanels; i++)
        {
            _simpleScrollSnapLeft.Content.GetChild(i).GetComponent<UpgradeCard>().Init();
        }
        // 初始化右侧卡片
        for (int i = 0; i < _simpleScrollSnapRight.NumberOfPanels; i++)
        {
            _simpleScrollSnapRight.Content.GetChild(i).GetComponent<UpgradeCard>().Init();
        }
        isInCoolDown = false;
        InitComponentStatus();

        CurrencyManager.OnCoinChange += OnCoinChange;
        coolDownSlider.gameObject.SetActive(false);
    }

    //初始化组件
    private void InitComponentStatus()
    {
        _leftMask.SetActive(false);
        _rightMask.SetActive(false);
        _discountLeft.SetActive(false);
        _discountRight.SetActive(false);

        _refreshButton.onClick.AddListener(RefreshUpgrade);
        _updatePriceText.text = "0";

        _leftUpgrade = _simpleScrollSnapLeft.Content
                   .GetChild(_simpleScrollSnapLeft.CenteredPanel)
                   .GetComponent<UpgradeCard>().UpgradeBase;
        _rightUpgrade = _simpleScrollSnapRight.Content
                    .GetChild(_simpleScrollSnapRight.CenteredPanel)
                    .GetComponent<UpgradeCard>().UpgradeBase;

        _leftButton.onClick.AddListener(() =>
        {
            BuyUpgradeButton(-1);
        });
        _rightButton.onClick.AddListener(() =>
        {
            BuyUpgradeButton(1);
        });

        RefreshUpgradeButtons();

    }

    /// <summary>
    /// 游戏结束后销毁升级的数据
    /// </summary>
    public void DestroyUpgrade()
    {
        _refreshButton.onClick.RemoveAllListeners();
        _leftButton.onClick.RemoveAllListeners();
        _rightButton.onClick.RemoveAllListeners();
        CurrencyManager.OnCoinChange -= OnCoinChange;
    }

    //刷新升级属性按钮
    private void RefreshUpgrade()
    {

        if (isInCoolDown)//正在冷却
        {
            AudioManager.Instance.PlaySFX("刷新福利");
            return;
        }

        if (!CurrencyManager.Instance.SpendCoin(UpgradeManager.Instance.FreshCost.Value))//刷新需要金币
        {
            AudioManager.Instance.PlaySFX("错误");
            return;
        }
        isInCoolDown = true;
        TimerUtility.Instance.Timer(5.0f, () =>{
            isInCoolDown = false;
        });
        coolDownSlider.StartCountDown(5.0f);
       
        RefreshUpgradeButtons();
        AudioManager.Instance.PlaySFX("刷新福利");
        UpgradeManager.Instance.RefreshCount.Value++;

        SetTextColor(_updatePriceText, gold >= UpgradeManager.Instance.FreshCost.Value);
        float rawValue = UpgradeManager.Instance.RefreshCount.Value *
                 (1 - (ScienceManager.Instance.GetUpgradeCountByType(ScienceEffectType.FreshCoinCount)*0.05f));

        // 四舍五入为整数
        int roundedValue = Mathf.RoundToInt(rawValue);
        // 确保至少为 1
        roundedValue = Mathf.Max(1, roundedValue);

        // 赋值
        UpgradeManager.Instance.FreshCost.Value = roundedValue;
        _updatePriceText.text = $"{UpgradeManager.Instance.FreshCost.Value}";
        OnRefreshBuff?.Invoke();
    }


    /// <summary>
    /// 刷新升级属性的按钮
    /// </summary>
    public void RefreshUpgradeButtons()
    {
        _refreshButton.interactable = false;
        _leftButton.interactable = false;
        _rightButton.interactable = false;

        _discountLeft.SetActive(false);//打折的标签隐藏
        _discountRight.SetActive(false);//打折的标签隐藏

        _leftMask.SetActive(false);//左侧的已购买遮罩隐藏
        _rightMask.SetActive(false);//右侧的已购买遮罩隐藏

        isLeftStop = false;
        isRightStop = false;

        _leftUpgrade = null;//升级属性置为null
        _rightUpgrade = null;//升级属性置为null

        // 随机速度赋值
        _simpleScrollSnapLeft.Velocity +=10000 * Vector2.up;
        _simpleScrollSnapRight.Velocity += 10000 * Vector2.up;

        // 初始化左侧卡片
        for (int i = 0; i < _simpleScrollSnapLeft.NumberOfPanels; i++)
        {
            _simpleScrollSnapLeft.Content.GetChild(i).GetComponent<UpgradeCard>().Init();
        }
        // 初始化右侧卡片
        for (int i = 0; i < _simpleScrollSnapRight.NumberOfPanels; i++)
        {
            _simpleScrollSnapRight.Content.GetChild(i).GetComponent<UpgradeCard>().Init();
        }

        // 监听左侧停止
        _simpleScrollSnapLeft.OnPanelSelected.AddListener((value) =>
        {
            isLeftStop = true;
            _simpleScrollSnapLeft.OnPanelSelected.RemoveAllListeners();
            TryCompareUpgrades(() =>
            {
                _leftUpgrade = _simpleScrollSnapLeft.Content
                    .GetChild(_simpleScrollSnapLeft.CenteredPanel)
                    .GetComponent<UpgradeCard>().UpgradeBase;
                _rightUpgrade = _simpleScrollSnapRight.Content
                    .GetChild(_simpleScrollSnapRight.CenteredPanel)
                    .GetComponent<UpgradeCard>().UpgradeBase;
                _refreshButton.interactable = true;
                _leftButton.interactable = true;
                _rightButton.interactable = true;
            });
        });

        // 监听右侧停止
        _simpleScrollSnapRight.OnPanelSelected.AddListener((value) =>
        {
            isRightStop = true;
            _simpleScrollSnapRight.OnPanelSelected.RemoveAllListeners();
            TryCompareUpgrades(() =>
            {
                _leftUpgrade = _simpleScrollSnapLeft.Content
                    .GetChild(_simpleScrollSnapLeft.CenteredPanel)
                    .GetComponent<UpgradeCard>().UpgradeBase;
                _rightUpgrade = _simpleScrollSnapRight.Content
                    .GetChild(_simpleScrollSnapRight.CenteredPanel)
                    .GetComponent<UpgradeCard>().UpgradeBase;
                _refreshButton.interactable = true;
                _leftButton.interactable = true;
                _rightButton.interactable = true;
            });
        });

        void TryCompareUpgrades(Action onFinish)
        {
            if (isLeftStop && isRightStop)
            {
                var leftCard = _simpleScrollSnapLeft.Content
                    .GetChild(_simpleScrollSnapLeft.CenteredPanel)
                    .GetComponent<UpgradeCard>();

                var rightCard = _simpleScrollSnapRight.Content
                    .GetChild(_simpleScrollSnapRight.CenteredPanel)
                    .GetComponent<UpgradeCard>();

                bool anyAnimation = false; // 是否有动画执行

                //两个东西相同
                if (leftCard.UpgradeBase.UpgradeID == rightCard.UpgradeBase.UpgradeID)
                {
                    anyAnimation = true;
                    PlayDiscountAnimation(() =>
                    {
                        int finished = 0;
                        void Check()
                        {
                            finished++;
                            if (finished >= 2)
                                onFinish?.Invoke();
                        }

                        leftCard.SetCost(UpgradeManager.Instance. SameDiscount.Value, 0.2f, Check);
                        rightCard.SetCost(UpgradeManager.Instance.SameDiscount.Value, 0.2f, Check);
                    },discount: UpgradeManager.Instance.SameDiscount.Value);
                }
                else
                {
                    float leftDiscount = GetRandomDiscount();
                    float rightDiscount = GetRandomDiscount();

                    int animCount = 0;
                    void CheckFinish()
                    {
                        animCount++;
                        if (animCount >= ((leftDiscount != 1f ? 1 : 0) + (rightDiscount != 1f ? 1 : 0)))
                        {
                            TimerUtility.Instance.Timer(0.1f, () => { onFinish?.Invoke(); });
                        }
                    }

                    if (leftDiscount != 1.0f)
                    {
                        anyAnimation = true;
                        PlayDiscountAnimation(() =>
                        {
                            leftCard.SetCost(leftDiscount, 0.2f, CheckFinish);
                        }, side: -1, discount: leftDiscount);
                    }


                    if (rightDiscount != 1.0f)
                    {
                        anyAnimation = true;
                        PlayDiscountAnimation(() =>
                        {
                            rightCard.SetCost(rightDiscount, 0.2f, CheckFinish);
                        }, side: 1, discount: rightDiscount);
                    }
                }

                if (!anyAnimation)
                {
                    onFinish?.Invoke(); // 无动画直接完成
                }

                // 防止重复监听
                _simpleScrollSnapLeft.OnPanelCentered.RemoveAllListeners();
                _simpleScrollSnapRight.OnPanelCentered.RemoveAllListeners();
            }
        }
    }

    /// <summary>
    /// 购买升级的按钮
    /// </summary>
    /// <param name="side">-1表示左侧，1表示右侧</param>
    private void BuyUpgradeButton(int side)
    {
        AudioManager.Instance.PlaySFX("购买福利");
        if (side == -1)//左边的按钮
        {
            if (_leftUpgrade==null) return;
            if (_upgradeManager.PurchaseUpgrade(_leftUpgrade))
            {
                _leftMask.SetActive(true);
                _leftUpgrade = null;//升级属性置为null
            }
        }
        if (side == 1)
        {
            if (_rightUpgrade == null) return;
            if (_upgradeManager.PurchaseUpgrade(_rightUpgrade))
            {
                _rightMask.SetActive(true);
                _rightUpgrade = null;//升级属性置为null
            }
        }
    }


    //金币变化
    private void OnCoinChange(int amount)
    {
        SetTextColor(_updatePriceText, gold >= UpgradeManager.Instance.FreshCost.Value);
        // 初始化左侧卡片
        for (int i = 0; i < _simpleScrollSnapLeft.NumberOfPanels; i++)
        {
            _simpleScrollSnapLeft.Content.GetChild(i).GetComponent<UpgradeCard>().UpdateTextColor();
        }
        // 初始化右侧卡片
        for (int i = 0; i < _simpleScrollSnapRight.NumberOfPanels; i++)
        {
            _simpleScrollSnapRight.Content.GetChild(i).GetComponent<UpgradeCard>().UpdateTextColor();
        }


    }

    //设置文本颜色
    private void SetTextColor(Text priceText, bool isEnough)
    {
        priceText.color = isEnough ? new Color32(4, 90, 57, 255) : new Color32(228,73,98,255);
    }

    //播放打折的动画
    public void PlayDiscountAnimation(Action onComplete, int side=0,float discount=0)
    {
        // 初始化缩放为 1.5（根据side选择）
        if (side <= 0) 
        {
            _discountLeft.transform.localScale = Vector3.one * 1.5f;
            _discountTextLeft.text = $"-{(1- discount)*100}%";
        }
        if (side >= 0) 
        {
            _discountRight.transform.localScale = Vector3.one * 1.5f;
            _discountTextRight.text = $"-{(1 - discount) * 100}%";
        } 

        int completedCount = 0;
        int targetCount = side == 0 ? 2 : 1;

        void OnOneComplete()
        {
            completedCount++;
            if (completedCount >= targetCount)
            {
                onComplete?.Invoke();
            }
        }

        // 创建 DOTween 序列，延迟 0.5 秒后执行动画
        DOTween.Sequence()
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                if (side <= 0)
                {
                    _discountLeft.SetActive(true);
                    _discountLeft.transform.DOScale(Vector3.one, 0.3f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(OnOneComplete);
                }

                if (side >= 0)
                {
                    _discountRight.SetActive(true);
                    _discountRight.transform.DOScale(Vector3.one, 0.3f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(OnOneComplete);
                }
            });
    }


    /// <summary>
    /// 返回打折倍数（如 0.9 表示九折，1.0 表示不打折）
    /// 默认30%概率打折，折扣范围：0.9~0.5，折扣越大概率越小
    /// </summary>
    public float GetRandomDiscount()
    {
        // 30% 概率打折
        if (UnityEngine.Random.value >= 0.3f)
        {
            return 1f; // 不打折
        }

        // 折扣列表及权重（越低折扣权重越小）
        Dictionary<float, int> discountTable = new Dictionary<float, int>
    {
        { 0.9f, 40 },
        { 0.8f, 30 },
        { 0.7f, 20 },
        { 0.6f, 7 },
        { 0.5f, 3 }
    };

        int totalWeight = discountTable.Values.Sum();
        int roll = UnityEngine.Random.Range(0, totalWeight);

        foreach (var kvp in discountTable)
        {
            roll -= kvp.Value;
            if (roll < 0)
            {
                return kvp.Key;
            }
        }

        return 1f; // 理论不会到这，但保险兜底
    }

}
