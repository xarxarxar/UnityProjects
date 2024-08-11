using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using TMPro;

public class Card_event : MonoBehaviour
{
    public int card_type = 0;//卡牌类型0：胡萝卜1：玉米2：草3：铃铛
    public bool card_is_enable = true;//此卡牌是否被使用
    public GameObject systemevent;
    public GameObject game_over_ui;
    private int delete_count = 3;//集齐N张卡片后删除
    public int init_max_delete = 3;//方便在unity里面初始化最大卡牌数
    private void Awake()
    {
        //初始化删除卡牌数
        delete_count = init_max_delete;
        //为卡牌添加点击事件
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(move_to);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void move_to()
    {
        if(card_is_enable == false)//如果卡牌已被使用则不能再移动此卡牌
        {
            return;
        }
        for(int i = 0; i < systemevent.GetComponent<Game_over_btn>().first_level.Length; i++)//如果第一层的卡牌被使用，对应在其下的卡牌设置为可以使用，否则无法使用下层卡牌
        {
            if (systemevent.GetComponent<Game_over_btn>().first_level[i] == null)
            {
                continue;
            }
            if (gameObject.Equals(systemevent.GetComponent<Game_over_btn>().first_level[i].gameObject)){
                systemevent.GetComponent<Game_over_btn>().second_level[i].GetComponent<Card_event>().card_is_enable = true;
                break;
            }
        }
        for(int i = 0; i < systemevent.GetComponent<card_slot>().solt_enable.Length; i++)//检查当前是否有可用卡槽
        {
            if (systemevent.GetComponent<card_slot>().solt_enable[i] == null)//检测到第i个卡槽可用
            {
                transform.DOMove(systemevent.GetComponent<card_slot>().slot[i].transform.position, 0.8f);//移动卡牌到该卡槽位置
                systemevent.GetComponent<card_slot>().solt_enable[i] = this.gameObject;//设置此卡槽已被占用
                GetComponent<UnityEngine.UI.Button>().enabled = false;//设置此卡牌已被使用
                switch(card_type)//记录此类卡牌在卡槽中的数量
                {
                    case 0:
                    systemevent.GetComponent<card_slot>().carrot++;
                        break;
                    case 1:
                        systemevent.GetComponent<card_slot>().grass++;
                        break;
                    case 2:
                        systemevent.GetComponent<card_slot>().corn++;
                        break;
                    case 3:
                        systemevent.GetComponent<card_slot>().bell++;
                        break;
                    default:
                        break;
                }
                switch (card_type)//检测此类卡牌在卡槽中的数量是否已达到需要删除的数量
                {
                    case 0:
                        if(systemevent.GetComponent<card_slot>().carrot == delete_count)//如果已经达到执行一下程序
                        {
                            int k = 0;
                            for (int j = 0; j < systemevent.GetComponent<card_slot>().solt_enable.Length; j++)
                            {
                                if (k >= delete_count)//把N张此类卡牌放入删除队列，以防在删除之前加入了同类型卡牌导致误删新加入的同类卡牌，达到数量后不再将此类卡牌放入删除队列，待队列删除完成再继续加入
                                {
                                    break;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j] == null)//检查所有卡牌，检查卡牌是否还在场上，如果没有则跳过
                                {
                                    continue;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j].GetComponent<Card_event>().card_type == card_type)//检查所有场上的卡牌，将所有在卡槽上的此类卡牌按顺序添加N张到删除队列中
                                {
                                    systemevent.GetComponent<Game_over_btn>().delete_queene[systemevent.GetComponent<Game_over_btn>().delete_queene_cur] = (systemevent.GetComponent<card_slot>().solt_enable[j]);//把卡牌放入删除队列
                                    systemevent.GetComponent<Game_over_btn>().delete_queene_cur++;//删除队列加1
                                    systemevent.GetComponent<card_slot>().solt_enable[j] = null;//设置该卡槽可用
                                    k++;//加入删除的队列的数量+1
                                }
                            }
                            Invoke("delay_change_img", 1f);//1s后执行sprite变换，实现消失动画效果
                            Invoke("delay_delete", 1.1f);//从队列中删除这N个卡牌，并销毁其对象
                            systemevent.GetComponent<card_slot>().carrot = 0;
                        }
                        break;
                    case 1:
                        if (systemevent.GetComponent<card_slot>().grass == delete_count)
                        {
                            int k = 0;
                            for (int j = 0; j < systemevent.GetComponent<card_slot>().solt_enable.Length; j++)
                            {
                                if (k >= delete_count)
                                {
                                    break;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j] == null)
                                {
                                    continue;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j].GetComponent<Card_event>().card_type == card_type)
                                {
                                    systemevent.GetComponent<Game_over_btn>().delete_queene[systemevent.GetComponent<Game_over_btn>().delete_queene_cur] = (systemevent.GetComponent<card_slot>().solt_enable[j]);
                                    systemevent.GetComponent<Game_over_btn>().delete_queene_cur++;
                                    systemevent.GetComponent<card_slot>().solt_enable[j] = null;
                                    k++;
                                }
                            }
                            Invoke("delay_change_img", 1f);
                            Invoke("delay_delete", 1.1f);
                            systemevent.GetComponent<card_slot>().grass = 0;
                        }
                        break;
                    case 2:
                        if (systemevent.GetComponent<card_slot>().corn == delete_count)
                        {
                            int k = 0;
                            for (int j = 0; j < systemevent.GetComponent<card_slot>().solt_enable.Length; j++)
                            {
                                if (k >= delete_count)
                                {
                                    break;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j] == null)
                                {
                                    continue;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j].GetComponent<Card_event>().card_type == card_type)
                                {
                                    systemevent.GetComponent<Game_over_btn>().delete_queene[systemevent.GetComponent<Game_over_btn>().delete_queene_cur] = (systemevent.GetComponent<card_slot>().solt_enable[j]);
                                    systemevent.GetComponent<Game_over_btn>().delete_queene_cur++;
                                    systemevent.GetComponent<card_slot>().solt_enable[j] = null;
                                    k++;
                                }
                            }
                            Invoke("delay_change_img", 1f);
                            Invoke("delay_delete", 1.1f);
                            systemevent.GetComponent<card_slot>().corn = 0;
                        }
                        break;
                    case 3:
                        if (systemevent.GetComponent<card_slot>().bell == delete_count)
                        {
                            int k = 0;
                            for (int j = 0; j < systemevent.GetComponent<card_slot>().solt_enable.Length; j++)
                            {
                                if (k >= delete_count)
                                {
                                    break;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j] == null)
                                {
                                    continue;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j].GetComponent<Card_event>().card_type == card_type)
                                {
                                    systemevent.GetComponent<Game_over_btn>().delete_queene[systemevent.GetComponent<Game_over_btn>().delete_queene_cur] = (systemevent.GetComponent<card_slot>().solt_enable[j]);
                                    systemevent.GetComponent<Game_over_btn>().delete_queene_cur++;
                                    systemevent.GetComponent<card_slot>().solt_enable[j] = null;
                                    k++;
                                }
                            }
                            Invoke("delay_change_img", 1f);
                            Invoke("delay_delete", 1.1f);
                            systemevent.GetComponent<card_slot>().bell = 0;
                        }
                        break;
                    default:
                        break;
                }
                card_is_enable = false;//设置卡牌已被使用
                for (int w = 0; w < systemevent.GetComponent<card_slot>().all_cards.Length; w++)//检查所有卡牌是否被启用，如果没有全部启用，不执行后面的程序直接返回函数
                {
                    if (systemevent.GetComponent<card_slot>().all_cards[w] != null)
                    {
                        if (systemevent.GetComponent<card_slot>().all_cards[w].GetComponent<Card_event>().card_is_enable != false)
                        {
                            return;
                        }
                    }
                }
                for (int e = 0; e < systemevent.GetComponent<card_slot>().solt_enable.Length; e++)//如果所有卡牌都被使用,检查卡槽中是否还有卡牌，如果有就判定玩家游戏失败，如果没有则判定玩家获得游戏胜利
                {
                    if (systemevent.GetComponent<card_slot>().solt_enable[e] == false)
                    {
                        systemevent.GetComponent<card_slot>().gameover_text.text = "你赢了!";
                        game_over_ui.SetActive(true);
                        return;
                    }
                }
                systemevent.GetComponent<card_slot>().gameover_text.text = "你输了!";
                game_over_ui.SetActive(true);
                return;
            }
        }
        systemevent.GetComponent<card_slot>().gameover_text.text = "你输了!";
        game_over_ui.SetActive(true);
    }

    public void delay_delete()
    {
        for (int i = 0; i < delete_count; i++)//删除删除队列中的前N个对象指向的实体，并将这N个对象删除出队列,队列指向归零,但不一定队列里面没有对象，如果前面的一组还没有删除，后面就出现了需要新的需要删除的一组，新的一组会加入队列，等待前一组删除完毕再进行删除操作
        {
            Destroy(systemevent.GetComponent<Game_over_btn>().delete_queene[0]);
            for (int k = 0; k < systemevent.GetComponent<Game_over_btn>().delete_queene_cur; k++)
            {
                systemevent.GetComponent<Game_over_btn>().delete_queene[k] = systemevent.GetComponent<Game_over_btn>().delete_queene[k + 1];
            }
            systemevent.GetComponent<Game_over_btn>().delete_queene_cur--;
        }
    }

    public void delay_change_img()
    {
        for (int i = 0; i < delete_count; i++)//将删除队列中的前N个马上要删除的对象的sprite换成消失动画sprite，实现消失动画
        {
            systemevent.GetComponent<Game_over_btn>().delete_queene[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("disappear");
        }
    }
}
