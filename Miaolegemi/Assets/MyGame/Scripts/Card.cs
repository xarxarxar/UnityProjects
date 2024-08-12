using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    //��Ϸ�����еĿ�����

    public List<Card> upCards = new List<Card>();//����ס�ÿ��ƵĿ���
    public List<Card> downCards = new List<Card>();//���ÿ��Ƹ���ס�Ŀ���

    public int level = 0;//�ÿ������ڵĲ㼶
    public bool isclickable = true;//�ÿ����Ƿ���Ե��
    public bool isUsed = false;//�ÿ����Ƿ��Ѿ����������Ҳ����˵�ÿ������·��Ŀ��Ʋ��������ȥ�����·��

    public string  type;//�ÿ����ϳ��ص����ͣ�һ�����͵ľͿ���������֮���滻��Item��

    Button thisButton ;//�ÿ����ϴ��ص�Button���

    //�����ƿ��Ե��ʱ��ִ��ĳ�������������������button״̬�Ӳ��ɵ������Ϊ���Ե��
    public bool Isclickable
    {
        get => isclickable;
        set
        {
            isclickable = value;
            if (isclickable)
            {
                ChangeButtonState(isclickable);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(Click);//Ϊ�˿�����ӵ���¼�
        thisButton.interactable = isclickable;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�����ƵĿɵ��״̬�ı�
    void ChangeButtonState(bool isclicked)
    {
        thisButton.interactable = isclicked;
    }

    //�ÿ��Ƶĵ������
    public void Click()
    {
        if(isUsed || CardManager.instance.isFailed)//����Ѿ�������˻����Ѿ�ʧ���ˣ���ֱ�ӷ���
        {
            return;
        }
        isUsed = true;//������֮�󣬸ÿ��Ʊ�Ϊ�ѵ��

        //�����ſ���һ��֮��ֻ��Ҫ���������ſ��Ƹ��ǵ������Ƿ���Ե������
        for (int i = 0; i < downCards.Count; i++)
        {
            CardManager.instance.JudgeClickable(downCards[i]);//
        }
        CardManager.instance.MoveCard(this);
    }
}
