using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : BasePanel
{
    public override void OnEnable()
    {
        base.OnEnable();
        BattleManager.Instance.PauseGame();//‘›Õ£”Œœ∑
    }

    private void OnDisable()
    {
        BattleManager.Instance.ResumeGame();//ª÷∏¥”Œœ∑
    }
}
