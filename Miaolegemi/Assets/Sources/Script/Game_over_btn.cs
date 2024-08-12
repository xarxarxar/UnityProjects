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

    //重新游戏
    public void restart_game()
    {
        SceneManager.LoadScene("one");
    }

    //返回主界面
    public void return_to_starts()
    {
        SceneManager.LoadScene("start");
    }

    //切换音乐大小
    public void change_music_voice()
    {
        back_muisc.volume = music_voice.value;
    }

    //切换音乐
    public void choice_musics()
    {
        back_muisc.clip = musics[choice_music.value];
        back_muisc.Play();
    }
    
    //打开设置按钮
    public void open_setting()
    {
        setting_ui.SetActive(true);
    }

    //关闭设置按钮
    public void close_setting()
    {
        setting_ui.SetActive(false);
    }
}
