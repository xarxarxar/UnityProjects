using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFailPanel : MonoBehaviour
{
    [SerializeField] private Button _continueButton;//继续按钮
    [SerializeField] private Text _metaCoinText;//获取了多少局外金币

    private void OnEnable()
    {
        _metaCoinText.text = $"获得局外金币{BattleManager.Instance.DiamondCount}个";
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
