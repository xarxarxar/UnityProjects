using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : ManagerBase<PanelManager>,IManager
{
    public List<BasePanel> OutPanels= new List<BasePanel>();
    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;
    }

    public override void Init()
    {
        for (int i = 0; i < OutPanels.Count; i++)
        {
            OutPanels[i].Init();
        }
    }
}
