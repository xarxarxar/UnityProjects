using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 生成福利，每隔2分钟生成一次福利，每次福利持续10秒钟
/// </summary>
public class CreateWelfare : MonoBehaviour
{
    public Button _button;
    public Text timerText;
    public float interval = 120f;    // 每隔 2 分钟生成一次
    public float duration = 10f;     // 持续 10 秒
    private int lastRefresh=0;//上一次出现时候的刷新次数

    // Start is called before the first frame update
    void OnEnable()
    {
        UpgradeManager.Instance.RefreshCount.OnValueChanged -= SpawnWelfare;
        UpgradeManager.Instance.RefreshCount.OnValueChanged += SpawnWelfare;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickButton);
        HideThis();
    }

    //点击了这个按钮
    private void OnClickButton()
    {
        BattleUIManager.Instance.ShowWelfarePanel();
        HideThis();
    }

    //开始阶段性生成福利
    private void SpawnWelfare(int value)
    {
        if(value-lastRefresh>=7)
        {
            lastRefresh = 999;
            StartCoroutine(SpawnWelfareLoop());
        }
    }

    //将这个按钮隐藏起来
    private void HideThis()
    {
        StopAllCoroutines();
        GetComponent<Button>().enabled = false;
        transform.Find("按钮显示").gameObject.SetActive(false);
        lastRefresh = UpgradeManager.Instance.RefreshCount.Value;
    }

    private void ShowThis()
    {
        Debug.Log("显示福利");
        var button = GetComponent<Button>();
        button.enabled = true;

        RectTransform child = transform.Find("按钮显示").GetComponent<RectTransform>();
        child.gameObject.SetActive(true);

        // 清除之前的动画
        child.DOKill();
        child.localRotation = Quaternion.identity; // 归正
        child.localScale = Vector3.zero; // 从0开始放大

        // 创建序列
        Sequence seq = DOTween.Sequence();

        // 放大动画
        seq.Append(child.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

        // 无限循环左右摇动
        seq.AppendCallback(() => child.localRotation = Quaternion.identity); // 确保开始归正
        for (int i = 0; i < 3; i++) // 左右摇动三次
        {
            seq.Append(child.DORotate(new Vector3(0, 0, 20f), 0.1f));
            seq.Append(child.DORotate(new Vector3(0, 0, -20f), 0.1f));
        }

        seq.Append(child.DORotate(Vector3.zero, 0.1f)); // 归正
        seq.AppendInterval(2f); // 休息两秒
        seq.SetLoops(-1);
    }

    // 福利生成与计时合并
    private IEnumerator SpawnWelfareLoop()
    {
        // 生成福利
        ShowThis();

        float remaining = duration;
        while (remaining > 0)
        {
            timerText.text = $"{Mathf.CeilToInt(remaining)}";
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        timerText.text = ""; // 清空计时
        HideThis();
    }
}
