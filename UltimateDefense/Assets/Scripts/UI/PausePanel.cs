using UnityEngine;
using UnityEngine.UI;

public class PausePanel : BasePanel
{
    [SerializeField] private BindableButton _settingButton;//设置按钮
    [SerializeField] private BindableButton _exitButton;//退出到主菜单按钮
    public override void OnEnable()
    {
        base.OnEnable();
        BattleManager.Instance.PauseGame();//暂停游戏
        _settingButton.AddListener(GameUIManager.Instance.ShowSettingPanel);
        _exitButton.AddListener(() =>
        {
            BattleManager.Instance.EndBattle(false);
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        BattleManager.Instance.ResumeGame();//恢复游戏
        _settingButton.RemoveAllListeners();
        _exitButton.RemoveAllListeners();
    }
}
