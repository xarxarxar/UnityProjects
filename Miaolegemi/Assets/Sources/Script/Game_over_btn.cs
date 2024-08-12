using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
public class Game_over_btn : MonoBehaviour
{
    public UnityEngine.UI.Button continue_game;
    public UnityEngine.UI.Button return_to_start;
    public AudioSource back_muisc;
    public AudioClip[] musics;
    public UnityEngine.UI.Slider music_voice;
    public TMP_Dropdown choice_music;
    public UnityEngine.UI.Button setting_btn;
    public UnityEngine.UI.Button back_to_game;
    public GameObject setting_ui;
    public GameObject[] delete_queene;
    public int delete_queene_cur = 0;
    public GameObject[] first_level;
    public GameObject[] second_level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //������Ϸ
    public void restart_game()
    {
        SceneManager.LoadScene("one");
    }

    //����������
    public void return_to_starts()
    {
        SceneManager.LoadScene("start");
    }

    //�л����ִ�С
    public void change_music_voice()
    {
        back_muisc.volume = music_voice.value;
    }

    //�л�����
    public void choice_musics()
    {
        back_muisc.clip = musics[choice_music.value];
        back_muisc.Play();
    }
    
    //�����ð�ť
    public void open_setting()
    {
        setting_ui.SetActive(true);
    }

    //�ر����ð�ť
    public void close_setting()
    {
        setting_ui.SetActive(false);
    }
}
