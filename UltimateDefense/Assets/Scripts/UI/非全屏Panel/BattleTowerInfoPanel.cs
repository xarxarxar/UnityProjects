using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTowerInfoPanel : BasePanel
{
    [SerializeField] private Text _bulletDamageText;//×Óµ¯ÉËº¦
    [SerializeField] private Text _bulletSpeedText;//×Óµ¯¹¥ËÙ
    [SerializeField] private Text _bulletCriticalShotProbText;//×Óµ¯±©»÷¸ÅÂÊ
    [SerializeField] private Text _bulletCriticalShotMultiText;//±©»÷ÉËº¦±¶ÂÊ
    [SerializeField] private Text _bulletCapText;//µ¯¼ÐÈÝÁ¿
    [SerializeField] private Text _bulletReloadTimeText;//»»µ¯Ê±³¤

    private void Start()
    {
        TowerManager.Instance.GlobalAttackBonus.OnValueChanged += value => _bulletDamageText.text = $"{value}";
        _bulletDamageText.text = $"{TowerManager.Instance.GlobalAttackBonus.Value}";

        TowerManager.Instance.GlobalAttackSpeedMultiplier.OnValueChanged += value => _bulletSpeedText.text = $"{value}";
        _bulletSpeedText.text = $"{TowerManager.Instance.GlobalAttackSpeedMultiplier.Value}";

        TowerManager.Instance.GlobalCriticalShotProb.OnValueChanged +=
            value => _bulletCriticalShotProbText.text = $"{(value * 100f).ToString("F0")}%";
        _bulletCriticalShotProbText.text = $"{(TowerManager.Instance.GlobalCriticalShotProb.Value * 100f).ToString("F0")}%";

        TowerManager.Instance.GlobalCriticalMultiplier.OnValueChanged +=
            value => _bulletCriticalShotMultiText.text = $"{(value * 100f).ToString("F0")}%";
        _bulletCriticalShotMultiText.text = $"{(TowerManager.Instance.GlobalCriticalMultiplier.Value * 100f).ToString("F0")}%";

        TowerManager.Instance.GlobalIncreaseBulletCap.OnValueChanged += value => _bulletCapText.text = $"{value}";
        _bulletCapText.text = $"{TowerManager.Instance.GlobalIncreaseBulletCap.Value}";

        TowerManager.Instance.GlobalReloadTime.OnValueChanged += value => _bulletReloadTimeText.text = $"{value}s";
        _bulletReloadTimeText.text = $"{TowerManager.Instance.GlobalReloadTime.Value}s";
    }



}
