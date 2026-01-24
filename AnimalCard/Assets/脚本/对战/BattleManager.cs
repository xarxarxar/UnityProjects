using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public Slider CountDownSlider;
    public bool isEnd = false;//游戏是否结束
    public bool isSuccess=false;//是否成功抓到动物
    public static event UnityAction<bool> OnBattleEnd;//游戏结束，是否成功抓到宠物

    public GameObject CountDownSliderPanel;
    public RoleControl PlayerRoleControl;//自己
    public NpcControl NpcControl;

    public Role PlayerRole;
    public Role NpcRole;

    private float maxDuration = 1;
    private float currentTime = 0;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// 初始化对战管理器
    /// </summary>
    /// <param name="player">玩家的动物数据</param>
    /// <param name="npc">npc的动物数据</param>
    /// <param name="duration">本局时长</param>
    public void Init(AnimalBaseData player,AnimalBaseData npc,float duration)
    {
        isEnd=false;
        currentTime =duration;
        maxDuration=duration;
        transform.GetChild(0).gameObject.SetActive(true);
        CountDownSliderPanel.SetActive(true);
        StartCoroutine(CountDownIe());
        PlayerRoleControl.Init();
        NpcControl.Init();
    }

    private void GameEnd(bool success)
    {
        isEnd = true;
        CardEffectManager.instance.StopAllEffects();
        transform.GetChild(0).gameObject.SetActive(false);
        CountDownSliderPanel.SetActive(false);
        GameManager.Instance. BattleEndPanel.SetActive(true);
        
        OnBattleEnd?.Invoke(success);
    }


    private IEnumerator CountDownIe()
    {
        while (currentTime >= 0)
        {
            currentTime-=Time.deltaTime;
            CountDownSlider.value = currentTime / maxDuration;
            yield return null;
        }
        GameEnd(false);
        yield break;
    }
}
