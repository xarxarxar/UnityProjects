using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] private Text _titleText;//挑战成功还是失败的Text
    [SerializeField] private Text _descriptionText;//描述一下
    //[SerializeField] private Text _metaCoinText;//获取了多少钻石
    [SerializeField] private Text _progressText;//完成了多少进度的 Text
    [SerializeField] private Slider _progressSlider;//完成了多少进度的 slider

    [SerializeField] private BindableButton _continueButton;//继续按钮
    [SerializeField] private BindableButton _doubleButton;//奖励翻倍按钮

    [SerializeField] private RewardStruct _diamondReward;//钻石奖励
    [SerializeField] private RewardStruct _crownReward;//王冠

    private void OnEnable()
    {
        
    }

    public void Init(bool isSuccess)
    {
        gameObject.SetActive(true);
        if(isSuccess )
        {
            _titleText.text = $"挑战成功";
            _descriptionText.text = $"你击败了所有敌人";
            _progressSlider.value = 1 ;
            _crownReward.Init(1);//若胜利，则获得一个王冠
            AudioManager.Instance.PlaySFX("胜利");
        }
        else
        {
            _titleText.text = $"挑战失败";
            _descriptionText.text = $"你被击败了";
            _progressSlider.value = WaveManager.Instance.CurrentRound/ WaveManager.Instance.MaxRound*1f;
            _crownReward.gameObject.SetActive(false);//否则不获得王冠
            AudioManager.Instance.PlaySFX("失败");
        }

        _progressText.text = $"{_progressSlider.value*100}%";
        _diamondReward.Init(BattleManager.Instance.DiamondCount);//设置钻石的数量
    }

    private void Start()
    {
        _continueButton.AddListener(ContinueButton);
    }


    //继续按钮的点击事件
    private void ContinueButton()
    {
        gameObject.SetActive(false);
        BattleUIManager.Instance.HideBattleScene();
        GameUIManager.Instance.ShowMainMenu();//返回到主菜单

    }



}
