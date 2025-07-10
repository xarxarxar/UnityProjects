using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPanel : BasePanel
{
    [SerializeField] private Text _enemyCountText;//敌人数量
    [SerializeField] private Text _enemyCoinProbText;//敌人掉落金币概率
    [SerializeField] private Text _enemyCoinCountText;//敌人掉落金币数量
    [SerializeField] private Text _enemySpeedText;//敌人移速


    private void Start()
    {
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged += value => _enemyCountText.text = $"{value}";
        _enemyCountText.text = $"{EnemyManager.Instance.EnemyCurrentCount.Value}";

        EnemyManager.Instance.EnemyDieCoinProb.OnValueChanged += value => _enemyCoinProbText.text = $"{(value * 100f).ToString("F0")}%";
        _enemyCoinProbText.text = $"{EnemyManager.Instance.EnemyDieCoinProb.Value}";

        EnemyManager.Instance.EnemyDieCoin.OnValueChanged += value => _enemyCoinCountText.text = $"{value}";
        _enemyCoinCountText.text = $"{EnemyManager.Instance.EnemyDieCoin.Value}";

        EnemyManager.Instance.EnemySpeed.OnValueChanged += value => _enemySpeedText.text = $"{value}";
        _enemySpeedText.text = $"{EnemyManager.Instance.EnemySpeed.Value}";
    }
}
