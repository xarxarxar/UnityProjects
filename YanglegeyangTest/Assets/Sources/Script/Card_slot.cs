using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class card_slot : MonoBehaviour
{
    public int carrot = 0;
    public int grass = 0;
    public int corn = 0;
    public int bell = 0;
    public GameObject[] slot;
    public GameObject[] solt_enable;
    public GameObject[] all_cards;
    public GameObject game_over_ui;
    public TMP_Text gameover_text;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (game_over_ui.active == false)//��Ҳ���������û�������ˣ������������ܾ��У���������,Ӧ���ǲ���ִ����
        {
            for (int i = 0; i < all_cards.Length; i++)
            {
                if (all_cards[i] != null)
                {
                    return;
                }
            }
            for(int i = 0; i < solt_enable.Length; i++)
            {
                if (solt_enable[i] != null)
                {
                    return;
                }
            }
            gameover_text.text = "��Ӯ�ˣ�";
            game_over_ui.SetActive(true);
            Debug.Log("you win");
        }
    }
}
