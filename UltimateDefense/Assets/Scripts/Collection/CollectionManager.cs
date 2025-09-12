using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Enemy.OnEnemyDie += OnEnemyDie;
    }

    private void OnEnemyDie(Enemy enemy)
    {
        // 基础概率：0.1% = 0.001
        float probability = 0.001f;
        // 通关次数每 +1，概率 +0.05% (0.0005)
        probability += (DataManager.Instance.PlayerInfo.PassCount.Value - 1) * 0.0005f;

        // 敌人等级每 +5，概率 +0.05%
        probability += (WaveManager.Instance.CurrentRound / 5) * 0.0005f;

        probability= Mathf.Min(probability, 0.01f);

        // 随机触发
        if (Random.value < probability)
        {
            Debug.Log("触发成功！");
        }


    }

}
