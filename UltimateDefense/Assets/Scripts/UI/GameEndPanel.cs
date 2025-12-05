using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] private Text _titleText;//挑战成功还是失败的Text
    [SerializeField] private Text _descriptionText;//描述一下
    [SerializeField] private Text _progressText;//完成了多少进度的 Text
    [SerializeField] private Slider _progressSlider;//完成了多少进度的 slider

    [SerializeField] private BindableButton _continueButton;//继续按钮
    [SerializeField] private BindableButton _doubleButton;//奖励翻倍按钮

    [SerializeField] private RewardStruct _diamondReward;//钻石奖励
    [SerializeField] private RewardStruct _crownReward;//王冠
    [SerializeField] private CollectionTextEndBattle _collection;//收集的文字
    [SerializeField] private Transform _collectionParent;//收集的文字的父物体
    private List<CollectionTextEndBattle> signleCollection=new List<CollectionTextEndBattle>();//单次战斗生成的收藏文字prefab
    


    private void OnEnable()
    {
        for (int i = signleCollection.Count - 1; i >= 0; i--)
        {
            CollectionTextEndBattle tmp = signleCollection[i];
            Destroy(tmp.gameObject);
            signleCollection.RemoveAt(i);
        }
    }

    public void Init(bool isSuccess)
    {
        gameObject.SetActive(true);
        _doubleButton.gameObject.SetActive(true);

        if (isSuccess)
        {
            _crownReward.gameObject.SetActive(true);
            BattleManager.Instance.CrownCount = 1;
            _titleText.text = $"挑战成功";
            _descriptionText.text = $"你击败了所有敌人";
            _progressSlider.value = 1 ;
            AudioManager.Instance.PlaySFX("胜利");
        }
        else
        {
            _crownReward.gameObject.SetActive(false);
            BattleManager.Instance.CrownCount = 0;
            _titleText.text = $"挑战失败";
            _descriptionText.text = $"你被击败了";
            _progressSlider.value = (WaveManager.Instance.CurrentRound - 1) / (float)WaveManager.Instance.MaxRound;
            AudioManager.Instance.PlaySFX("失败");
        }

        _progressText.text = $"{_progressSlider.value*100}%";
        _crownReward.Init(BattleManager.Instance.CrownCount);//若胜利，则获得一个王冠
        _diamondReward.Init(BattleManager.Instance.DiamondCount);//设置钻石的数量
        foreach(var kv in BattleManager.Instance.CollectionSignleBattle)
        {
            CollectionTextEndBattle tmp = Instantiate(_collection,_collectionParent);
            tmp.SetCollectionText(kv.Key,CollectionManager.Instance.GetColorByIndex(kv.Key), kv.Value);
            signleCollection.Add(tmp);
        }

        Debug.Log($"isSuccess is {isSuccess},CrownCount is {BattleManager.Instance.CrownCount}");
    }

    private void Start()
    {
        _continueButton.AddListener(ContinueButton);
        _doubleButton.AddListener(DoubleButton);
    }


    //继续按钮的点击事件
    private void ContinueButton()
    {
        gameObject.SetActive(false);
        BattleUIManager.Instance.HideBattleScene();
        GameUIManager.Instance.ShowMainMenu();//返回到主菜单
    }

    private void DoubleButton()
    {
        WeChatManager.ShareApp(() =>
        {
            _doubleButton.gameObject.SetActive(false);

            DataManager.Instance.PlayerInfo.DailyTask.ShareCount.Value += 1;
            //王冠和钻石的显示数量翻倍
            UIUtils.PlayNumberAnimation(_crownReward.countText, BattleManager.Instance.CrownCount * 2,0.5f);
            UIUtils.PlayNumberAnimation(_diamondReward.countText, BattleManager.Instance.DiamondCount * 2,0.5f);
            for(int i=0;i<signleCollection.Count;i++)
            {
                UIUtils.PlayNumberAnimation(signleCollection[i].CountText, signleCollection[i].Count *2,0.5f);
                DataManager.Instance.PlayerInfo.MonthlyCollection[signleCollection[i].index] += signleCollection[i].Count;
            }
            //再额外给予玩家一倍的奖励
            MetaCurrencyManager.Instance.AddMetaCoin(RewardType.Diamond, BattleManager.Instance.DiamondCount);
            MetaCurrencyManager.Instance.AddMetaCoin(RewardType.Crown, BattleManager.Instance.CrownCount);

            DataManager.Instance.SavePlayerInfo();
        });
    }

}
