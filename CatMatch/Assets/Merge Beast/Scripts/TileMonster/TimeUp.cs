using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeBeast;
using UnityEngine.UI;
using System;

namespace Tiledom
{
    public class TimeUp : MonoBehaviour
    {
        [SerializeField] Button btnClose;
        [SerializeField] Button btnWatchVideo;

        private void Start() {
            btnClose.onClick.AddListener(() => Hide());
            btnWatchVideo.onClick.AddListener(() => WatchVideo());
        }


        private void WatchVideo()
        {
            AdsManager.Instance.ShowAds(() => {                
                Header.instance.BuyTime();
                gameObject.SetActive(false);
            });
        }

        private void Hide()
        {
            CPlayer.AddMCoin(GameManager._Instance.listTrash.Count);
            GameManager._Instance.ClearLevelData();
            LevelController.instance.Show();
            gameObject.SetActive(false);
        }
    }
}