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
        TowerManager.Instance.BonusAtk.OnValueChanged += value => _bulletDamageText.text = $"{value}";
        _bulletDamageText.text = $"{TowerManager.Instance.BonusAtk.Value}";

        TowerManager.Instance.BonusAttackRate.OnValueChanged += value => _bulletSpeedText.text = $"{value}";
        _bulletSpeedText.text = $"{TowerManager.Instance.BonusAttackRate.Value}";

        TowerManager.Instance.BonusCritProb.OnValueChanged +=
            value => _bulletCriticalShotProbText.text = $"{(value * 100f).ToString("F0")}%";
        _bulletCriticalShotProbText.text = $"{(TowerManager.Instance.BonusCritProb.Value * 100f).ToString("F0")}%";

        TowerManager.Instance.BonusCritMult.OnValueChanged +=
            value => _bulletCriticalShotMultiText.text = $"{(value * 100f).ToString("F0")}%";
        _bulletCriticalShotMultiText.text = $"{(TowerManager.Instance.BonusCritMult.Value * 100f).ToString("F0")}%";

        TowerManager.Instance.BonusCap.OnValueChanged += value => _bulletCapText.text = $"{value}";
        _bulletCapText.text = $"{TowerManager.Instance.BonusCap.Value}";

        TowerManager.Instance.BonusReload.OnValueChanged += value => _bulletReloadTimeText.text = $"{value}s";
        _bulletReloadTimeText.text = $"{TowerManager.Instance.BonusReload.Value}s";
    }



}
