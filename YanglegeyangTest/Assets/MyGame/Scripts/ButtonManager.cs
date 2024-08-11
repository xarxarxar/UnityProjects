using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject startScene;//游戏开始的场景
    [Header("开始按钮")]
    public GameObject gameScene;//游戏主场景
    public GameObject cutScenes;//过场动画的UI元素
    public void StartButton()
    {
        startScene.SetActive(false);
        cutScenes.SetActive(true);
        gameScene.SetActive(true);
    }

    [Header("仓库按钮")]
    public GameObject collectScene;//仓库的场景
    public void CollectButton()
    {
        startScene.SetActive(false);
        collectScene.SetActive(true);
    }

    [Header("设置按钮")]
    public GameObject settingScene;//设置面板
    public void SettingButton()
    {
        settingScene.SetActive(true);
    }

    [Header("放弃挑战按钮")]
    public GameObject tipPanel;//提醒是否放弃跳挑战
    public void GiveUpButton()//放弃挑战按钮
    {
        tipPanel.SetActive(true);
    }

    public void ConfrimGiveUp()//确认放弃按钮
    {
        tipPanel.SetActive(false);
        cutScenes.SetActive(true);
        settingScene.SetActive(false);
        gameScene.SetActive(false);
        startScene.SetActive(true);
    }

    public void ContinueButton()//继续挑战按钮
    {
        tipPanel.SetActive(false);
    }

    public void BackToStart()//从仓库界面返回开始界面
    {
        collectScene.SetActive(false);
        startScene.SetActive(true) ;
    }
}
