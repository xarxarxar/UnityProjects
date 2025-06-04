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
        GameEntrance.OnDateUpdate += OnDateUpdate;
        GameEntrance.OnPlayerInfoSync += OnPlayerInfoSync;
        GameEntrance.OnLoadSceneProgress += OnLoadSceneProgress;
    }

    //日期更新事件
    private void OnDateUpdate()
    {
        mLoadingSlider.value = 0.25f;
        mLoadingText.text = "信息获取中...";
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
        GameEntrance.OnDateUpdate -= OnDateUpdate;
        GameEntrance.OnPlayerInfoSync -= OnPlayerInfoSync;
        GameEntrance.OnLoadSceneProgress -= OnLoadSceneProgress;
    }
}
