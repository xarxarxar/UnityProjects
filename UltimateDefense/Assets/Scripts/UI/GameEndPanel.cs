using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] private Text _titleText;//挑战成功还是失败的Text
    [SerializeField] private Text _descriptionText;//描述一下
    [SerializeField] private Text _metaCoinText;//获取了多少局外金币
    [SerializeField] private Text _progressText;//完成了多少进度的 Text
    [SerializeField] private Slider _progressSlider;//完成了多少进度的 slider

    [SerializeField] private Button _continueButton;//继续按钮
    [SerializeField] private Button _doubleButton;//奖励翻倍按钮
    
    

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
        }
        else
        {
            _titleText.text = $"挑战失败";
            _descriptionText.text = $"你被击败了";
            _progressSlider.value = WaveManager.Instance.CurrentRound/100f;
        }

        _progressText.text = $"{_progressSlider.value*100}%";
        _metaCoinText.text = $"{BattleManager.Instance.MetaCoinCount}";
    }

    private void Start()
    {
        _continueButton.onClick.AddListener(ContinueButton);
    }


    //继续按钮的点击事件
    private void ContinueButton()
    {
        gameObject.SetActive(false);
        BattleUIManager.Instance.HideBattleScene();
        GameUIManager.Instance.ShowMainMenu();//返回到主菜单
    }


    
}
