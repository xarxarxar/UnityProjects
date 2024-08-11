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
    public int card_type = 0;//��������0�����ܲ�1������2����3������
    public bool card_is_enable = true;//�˿����Ƿ�ʹ��
    public GameObject systemevent;
    public GameObject game_over_ui;
    private int delete_count = 3;//����N�ſ�Ƭ��ɾ��
    public int init_max_delete = 3;//������unity�����ʼ���������
    private void Awake()
    {
        //��ʼ��ɾ��������
        delete_count = init_max_delete;
        //Ϊ������ӵ���¼�
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
        if(card_is_enable == false)//��������ѱ�ʹ���������ƶ��˿���
        {
            return;
        }
        for(int i = 0; i < systemevent.GetComponent<Game_over_btn>().first_level.Length; i++)//�����һ��Ŀ��Ʊ�ʹ�ã���Ӧ�����µĿ�������Ϊ����ʹ�ã������޷�ʹ���²㿨��
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
        for(int i = 0; i < systemevent.GetComponent<card_slot>().solt_enable.Length; i++)//��鵱ǰ�Ƿ��п��ÿ���
        {
            if (systemevent.GetComponent<card_slot>().solt_enable[i] == null)//��⵽��i�����ۿ���
            {
                transform.DOMove(systemevent.GetComponent<card_slot>().slot[i].transform.position, 0.8f);//�ƶ����Ƶ��ÿ���λ��
                systemevent.GetComponent<card_slot>().solt_enable[i] = this.gameObject;//���ô˿����ѱ�ռ��
                GetComponent<UnityEngine.UI.Button>().enabled = false;//���ô˿����ѱ�ʹ��
                switch(card_type)//��¼���࿨���ڿ����е�����
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
                switch (card_type)//�����࿨���ڿ����е������Ƿ��Ѵﵽ��Ҫɾ��������
                {
                    case 0:
                        if(systemevent.GetComponent<card_slot>().carrot == delete_count)//����Ѿ��ﵽִ��һ�³���
                        {
                            int k = 0;
                            for (int j = 0; j < systemevent.GetComponent<card_slot>().solt_enable.Length; j++)
                            {
                                if (k >= delete_count)//��N�Ŵ��࿨�Ʒ���ɾ�����У��Է���ɾ��֮ǰ������ͬ���Ϳ��Ƶ�����ɾ�¼����ͬ�࿨�ƣ��ﵽ�������ٽ����࿨�Ʒ���ɾ�����У�������ɾ������ټ�������
                                {
                                    break;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j] == null)//������п��ƣ���鿨���Ƿ��ڳ��ϣ����û��������
                                {
                                    continue;
                                }
                                if (systemevent.GetComponent<card_slot>().solt_enable[j].GetComponent<Card_event>().card_type == card_type)//������г��ϵĿ��ƣ��������ڿ����ϵĴ��࿨�ư�˳�����N�ŵ�ɾ��������
                                {
                                    systemevent.GetComponent<Game_over_btn>().delete_queene[systemevent.GetComponent<Game_over_btn>().delete_queene_cur] = (systemevent.GetComponent<card_slot>().solt_enable[j]);//�ѿ��Ʒ���ɾ������
                                    systemevent.GetComponent<Game_over_btn>().delete_queene_cur++;//ɾ�����м�1
                                    systemevent.GetComponent<card_slot>().solt_enable[j] = null;//���øÿ��ۿ���
                                    k++;//����ɾ���Ķ��е�����+1
                                }
                            }
                            Invoke("delay_change_img", 1f);//1s��ִ��sprite�任��ʵ����ʧ����Ч��
                            Invoke("delay_delete", 1.1f);//�Ӷ�����ɾ����N�����ƣ������������
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
                card_is_enable = false;//���ÿ����ѱ�ʹ��
                for (int w = 0; w < systemevent.GetComponent<card_slot>().all_cards.Length; w++)//������п����Ƿ����ã����û��ȫ�����ã���ִ�к���ĳ���ֱ�ӷ��غ���
                {
                    if (systemevent.GetComponent<card_slot>().all_cards[w] != null)
                    {
                        if (systemevent.GetComponent<card_slot>().all_cards[w].GetComponent<Card_event>().card_is_enable != false)
                        {
                            return;
                        }
                    }
                }
                for (int e = 0; e < systemevent.GetComponent<card_slot>().solt_enable.Length; e++)//������п��ƶ���ʹ��,��鿨�����Ƿ��п��ƣ�����о��ж������Ϸʧ�ܣ����û�����ж���һ����Ϸʤ��
                {
                    if (systemevent.GetComponent<card_slot>().solt_enable[e] == false)
                    {
                        systemevent.GetComponent<card_slot>().gameover_text.text = "��Ӯ��!";
                        game_over_ui.SetActive(true);
                        return;
                    }
                }
                systemevent.GetComponent<card_slot>().gameover_text.text = "������!";
                game_over_ui.SetActive(true);
                return;
            }
        }
        systemevent.GetComponent<card_slot>().gameover_text.text = "������!";
        game_over_ui.SetActive(true);
    }

    public void delay_delete()
    {
        for (int i = 0; i < delete_count; i++)//ɾ��ɾ�������е�ǰN������ָ���ʵ�壬������N������ɾ��������,����ָ�����,����һ����������û�ж������ǰ���һ�黹û��ɾ��������ͳ�������Ҫ�µ���Ҫɾ����һ�飬�µ�һ��������У��ȴ�ǰһ��ɾ������ٽ���ɾ������
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
        for (int i = 0; i < delete_count; i++)//��ɾ�������е�ǰN������Ҫɾ���Ķ����sprite������ʧ����sprite��ʵ����ʧ����
        {
            systemevent.GetComponent<Game_over_btn>().delete_queene[i].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("disappear");
        }
    }
}
