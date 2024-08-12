using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Startbtn_control : MonoBehaviour
{
    public UnityEngine.UI.Button start_button;
    public UnityEngine.UI.Button setting_btn;
    public GameObject setting_panel;
    public UnityEngine.UI.Button quitgame_btn;
    public UnityEngine.UI.Button back_btn;
    public UnityEngine.UI.Slider music_voice;
    public TMP_Dropdown choice_music;
    public AudioSource back_music;
    public AudioClip[] music_clips;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //��ʼ��Ϸ��ť�¼�
    public void start_game()
    {
        SceneManager.LoadScene("one");
    }

    //�����ý����¼�
    public void open_setting()
    {
        setting_panel.SetActive(true);
    }

    //�˳���Ϸ��ť�¼�
    public void quit_game()
    {
        Application.Quit();
    }

    //�ر����ý����¼�
    public void back_to_main()
    {
        setting_panel.SetActive(false);
    }

    //�ı䱳�����������¼�
    public void change_music_voice()
    {
        back_music.volume = music_voice.value;
    }

    //�л����������¼�
    public void choice_musics()
    {
        back_music.clip = music_clips[choice_music.value];
        back_music.Play();
    }
}
