using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 按照商店的购买次数来冷却
/// </summary>
public abstract class PlayerSkillBase : MonoBehaviour
{
    public SkillData skillData;
    public int cooldownIndex;//上次商店购买的次数序号
    public SkillInBattleUI skillUI;//技能按钮

    public class SkillInBattleUI
    {
        public Button skillButton;//按钮
        public Text cooldownText;//冷却文本
        public Text descriptionText;//描述文本
        public Image skillIcon;//技能图标
        public Image cooldownMask;//冷却遮罩
    }
    /// <summary>
    /// 初始化技能
    /// </summary>
    public void Init()
    {
        //初始化skillUI
        if (skillUI == null)
            skillUI = new SkillInBattleUI();
        //获取skillUI各个组件
        if (skillUI.skillButton == null)
        {
            skillUI.skillButton = GetComponent<Button>();
        }
        if (skillUI.cooldownText == null)
        {
            skillUI.cooldownText = transform.Find("按钮显示/技能冷却Text").GetComponent<Text>();
        }
        if(skillUI.descriptionText == null)
        {
            skillUI.descriptionText = transform.Find("按钮显示/技能描述Text").GetComponent<Text>();
        }
        if(skillUI.skillIcon == null)
        {
            skillUI.skillIcon = transform.Find("按钮显示/技能logo").GetComponent<Image>();
        }
        if (skillUI.cooldownMask == null)
        {
            skillUI.cooldownMask = transform.Find("按钮显示/冷却遮罩").GetComponent<Image>();
        }

        //按钮绑定技能
        skillUI.skillButton.onClick.RemoveAllListeners();
        skillUI.skillButton.onClick.AddListener(UseSkillCommon);
        //冷却文本初始化
        skillUI.cooldownText.gameObject.SetActive(false);
        skillUI.cooldownText.text = string.Empty;
        //描述文本初始化
        skillUI.descriptionText.text = skillData.description;
        //技能图标初始化
        skillUI.skillIcon.sprite=skillData.icon;
        //冷却遮罩初始化
        skillUI.cooldownMask.gameObject.SetActive(false);
        cooldownIndex = 0;
        //监听购买升级次数的变化
        UpgradeManager.Instance.BuyUpgradeCount.OnValueChanged -= OnBuyCountChanged;
        UpgradeManager.Instance.BuyUpgradeCount.OnValueChanged += OnBuyCountChanged;
        //特殊初始化
        SpecialInit();
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    public void UseSkillCommon()
    {
        //冷却次数到了
        if (cooldownIndex <= 0)
        {
            SkillEffect();
            cooldownIndex = skillData.cooldown;//计数器重置

            //显示冷却遮罩
            skillUI.cooldownMask.gameObject.SetActive(true);
            //显示冷却文本
            skillUI.cooldownText.gameObject.SetActive(true);
            skillUI.cooldownText.text = cooldownIndex.ToString();
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 特殊初始化
    /// </summary>
    public virtual void SpecialInit()
    {

    }
    /// <summary>
    /// 技能的具体效果
    /// </summary>
    public abstract void SkillEffect();

    //商店购买次数变化
    private void OnBuyCountChanged(int value)
    {
        cooldownIndex--;
        cooldownIndex=Mathf.Max(0, cooldownIndex);
        //冷却结束
        if (cooldownIndex <= 0)
        {
            //关闭冷却遮罩
            skillUI.cooldownMask.gameObject.SetActive(false);
            //关闭冷却文本
            skillUI.cooldownText.gameObject.SetActive(true);
            skillUI.cooldownText.text = string.Empty;
        }
        else
        {
            skillUI.cooldownText.text = cooldownIndex.ToString();
        }
    }
}

[System.Serializable]
public class SkillData
{
    public string skillName;//名称
    public string description;//描述
    public int cooldown;//冷却
    public Sprite icon;//图标
}
