using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI
/// </summary>
public class StartPanel : MonoBehaviour
{
    #region 私有变量
    [SerializeField]
    private Button _startBattleButton;//开始挑战按钮
    #endregion

    #region 公共变量
    /// <summary>
    /// 开始挑战按钮被点击
    /// </summary>
    public static event UnityAction OnSatrtBattle;
    #endregion

    #region 私有方法
    private void Start()
    {
        _startBattleButton.onClick.AddListener(StartBattleButton);
    }

    /// <summary>
    /// 开始挑战按钮的点击方法
    /// </summary>
    private void StartBattleButton()
    {
        Debug.Log("点击开始挑战按钮");
        OnSatrtBattle?.Invoke();//开始挑战按钮被点击
        GameUIManager.Instance.HideMainMenu();//隐藏主菜单
        GameUIManager.Instance.ShowChooseDebuffPanel();
        gameObject.SetActive(false);
    }
    #endregion
}
