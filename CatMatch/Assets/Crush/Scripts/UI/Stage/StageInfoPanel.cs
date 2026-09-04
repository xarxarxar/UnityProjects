using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    public Text titleTxt;
    public RectTransform enemyContainer;
    public RectTransform rewardContainer;

    public void Init(int stage)
    {
        titleTxt.text = "关卡：" + stage;

        var enemyMonsterDatas = LevelHelper.GetStageEnemies(stage);
        for (int k = 0; k < enemyContainer.childCount; k++)
        {
            var child = enemyContainer.GetChild(k);
            var prepare = child.GetComponent<PrepareItemView>();

            if (enemyMonsterDatas.Count > k)
            {
                var data = enemyMonsterDatas[k];
                var beast = BeastPrefs.Instance.GetBeastPref(data.beastId);
                var icon = beast.GetComponent<BeastBase>().icons[0];
                prepare.SetUp(PrepareSlotType.None, icon, data.curLevel.ToString(), data.curStar.ToString());
            }
            else
            {
                prepare.SetUp(PrepareSlotType.Empty, null, "", "");
            }
        }

        Rewards(stage);
    }

    public void Rewards(int stage)
    {
        var rewardModels = RewardHelper.GetCanStageRewards(stage);

        for (int k = 0; k < rewardContainer.childCount; k++)
        {
            var child = rewardContainer.GetChild(k);
            var prepare = child.GetComponent<PrepareItemView>();

            if (rewardModels.Count > k)
            {
                child.gameObject.SetActive(true);
                child.gameObject.GetComponent<RewardItemView>().SetUp(rewardModels[k]);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
