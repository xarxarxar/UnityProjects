using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public GameInfo gameInfo=>GameManager.instance.gameInfo;
    public GameObject coinGet;//获取金币的提示

    public GameObject getCoinPanel;//获取金币的面板
    public Button menuAddCoinButton;//主页获取金币的按钮
    public Button meijiaAddCoinButton;//美甲获取金币的按钮


    private void Awake()
    {
        instance = this;
    }

    //获得金币
    public void GetCoin(int count)
    {
        gameInfo.coinCount += count;
    }

    //获得金币
    public void GetCoin(int baseCount,int times)
    {
        gameInfo.coinCount += baseCount* times;
        coinGet.SetActive(true);
        if (times != 1)
        {
            //coinGet.transform.Find("金币数量").GetComponent<Text>().text = $"+{baseCount}<color=red>×<size=40%>{times}</size></color>";
            coinGet.transform.Find("金币数量").GetComponent<Text>().text = $"+{baseCount * times}";
        }
        else
        {
            coinGet.transform.Find("金币数量").GetComponent<Text>().text = $"+{baseCount * times}";
        }
        
        PlayScaleSequence(coinGet);
    }



    /// <summary>
    /// 扣钱
    /// </summary>
    /// <param name="count">是否扣成功</param>
    /// <returns></returns>
    public bool ReduceCoin(int count)
    {
        if(gameInfo.coinCount< count)
        {
            return false;
        }
        gameInfo.coinCount -= count;
        GameManager.instance.UpdateCoinText();
        return true;
    }

    /// <summary>
    /// 播放缩放动画并在完成后隐藏物体。
    /// </summary>
    /// <param name="target">要操作的目标物体</param>
    /// <param name="duration">每段动画的持续时间</param>
    public void PlayScaleSequence(GameObject target, float duration = 0.2f)
    {
        Transform t = target.transform;

        // 设置初始缩放
        t.localScale = Vector3.one * 0.8f;

        // 创建序列
        Sequence sequence = DOTween.Sequence();

        // 添加动画：从 0.8 -> 1.2
        sequence.Append(t.DOScale(1.2f, duration));

        // 添加动画：从 1.2 -> 1.0
        sequence.Append(t.DOScale(1.0f, duration));

        // 停留 1 秒
        sequence.AppendInterval(1f);

        // 动画完成后隐藏目标
        sequence.OnComplete(() => target.SetActive(false));
    }
}
