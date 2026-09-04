using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeBeast;
using SimpleJSON;

namespace Tiledom
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] Button btnResume;
        [SerializeField] Button btnQuit;
        [SerializeField] ConfirmQuitController confirmQuitController;
        [SerializeField] Text txtRemainTime;
        // Start is called before the first frame update

        private void OnEnable() {
            txtRemainTime.text = $"剩余时间\n{Header.instance.GetTimeRemain()}秒";
        }
        void Start()
        {
            btnResume.onClick.AddListener(() => Resume());
            btnQuit.onClick.AddListener(() => Quit());
        }

        private void Resume()
        {
            Header.instance.Resume();
        }

        private void Quit() {
            string path = SaveSystem.TILE_CONTINUE + CPlayer.currentLevel + ".json";
            string data = SaveSystem.Load(path);
            Debug.LogError(data);
            if (data == null) {
                confirmQuitController.gameObject.SetActive(true);
                gameObject.SetActive(false);
            } else {
                JSONNode jsonData = JSON.Parse(data);
                int complete = jsonData["complete"].AsInt;
                if (complete == 100) {
                    //quit luon ma ko can hoi
                    Header.instance.ShowLevel();
                    gameObject.SetActive(false);
                } else {
                    confirmQuitController.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }

        }
    }
}