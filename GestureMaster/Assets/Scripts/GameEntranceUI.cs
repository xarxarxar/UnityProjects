using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEntranceUI : MonoBehaviour
{
    [SerializeField]private Slider mLoadingSlider;//加载的slider
    [SerializeField]private Text mLoadingText;//加载时显示的text

    private void Start()
    {
        Debug.Log("GameEntranceUI脚本运行");
        GameEntrance.OnGetingPlayerWechatInfo += OnGetingPlayerWechatInfo;
        GameEntrance.OnGetPlayerWechatInfo += OnGetPlayerWechatInfo;
        GameEntrance.OnDateUpdate += OnDateUpdate;
        GameEntrance.OnPlayerInfoSync += OnPlayerInfoSync;
        GameEntrance.OnLoadSceneProgress += OnLoadSceneProgress;
    }

    private void OnGetingPlayerWechatInfo()
    {
        mLoadingSlider.gameObject.SetActive(true);
        mLoadingSlider.value = 0;
        mLoadingText.text = "获取玩家信息...";
    }

    private void OnGetPlayerWechatInfo()
    {
        mLoadingSlider.gameObject.SetActive(true);
        mLoadingSlider.value = 0.2f;
        mLoadingText.text = "获取玩家信息完成...";
    }

    //日期更新事件
    private void OnDateUpdate()
    {
        mLoadingSlider.value = 0.4f;
        mLoadingText.text = "信息同步中...";
    }

    //玩家信息同步事件
    private void OnPlayerInfoSync()
    {
        mLoadingSlider.value = 0.5f;
        mLoadingText.text = "玩家信息同步中...";
    }

    //场景加载事件
    private void OnLoadSceneProgress(float progress)
    {
        mLoadingSlider.value = 0.5f+ progress/2;
        mLoadingText.text = "场景加载中...";
    }

    private void OnDestroy()
    {
        GameEntrance.OnGetingPlayerWechatInfo -= OnGetingPlayerWechatInfo;
        GameEntrance.OnGetPlayerWechatInfo -= OnGetPlayerWechatInfo;
        GameEntrance.OnDateUpdate -= OnDateUpdate;
        GameEntrance.OnPlayerInfoSync -= OnPlayerInfoSync;
        GameEntrance.OnLoadSceneProgress -= OnLoadSceneProgress;
    }
}
