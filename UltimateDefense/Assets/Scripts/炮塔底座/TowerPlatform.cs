using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlatform : MonoBehaviour
{
    [SerializeField]private SpriteRenderer _spriteRenderer;


    //下面是函数
    public void OnEnable()
    {
        BattleManager.OnStartBattle += Init;
    }

    //初始化
    protected virtual void Init()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        TowerPlatformDataManager t = TowerPlatformDataManager.Instance;
        _spriteRenderer.sprite = t.GetCurrentSkin(t.GetCurrentTowerPlatformData()).sprite;
    }
}
