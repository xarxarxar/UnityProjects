using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeBeast;
using SimpleJSON;

namespace Tiledom
{
    public class ConfirmPlay : MonoBehaviour
    {
        [SerializeField] Text txtProgress;
        [SerializeField] Text txtTime;
        [SerializeField] Button btnRestart;
        [SerializeField] Button btnContinue;

        private void OnEnable()
        {
            
            // btnContinue.gameObject.SetActive(AdsManager.Instance.IsLoaded(EnumDefine.ADSTYPE.Rewarded));
            string dataPath = SaveSystem.TILE_CONTINUE + CPlayer.currentLevel + ".json";
            JSONNode jsonData = JSON.Parse(SaveSystem.Load(dataPath));
            int complete = jsonData["complete"].AsInt;
            int time = jsonData["remainTime"].AsInt;
            txtProgress.text = $"关卡进度<color=#eeb230>{complete}</color>%";
            txtTime.text = $"剩余时间<color=#eeb230>{time}</color>秒";
        }

        private void Start()
        {
            btnRestart.onClick.AddListener(() => RestartGame());
            btnContinue.onClick.AddListener(() => ContinueGame());
        }

        private void ContinueGame()
        {
            AdsManager.Instance.ShowAds(() => {
                LevelController.instance.Hide();
                GameManager._Instance.isNewGame = false;
                GameManager._Instance.StartGame();
                gameObject.SetActive(false);
                //LevelController.instance.AddTurn(-1);
            }, "", EnumDefine.ADSTYPE.Rewarded, true, EnumDefine.GameType.Tile);
            
        }

        private void RestartGame()
        {
            GameManager._Instance.isNewGame = true;
            GameManager._Instance.StartGame();            
            LevelController.instance.Hide();
            gameObject.SetActive(false);
            //LevelController.instance.AddTurn(-1);
        }
    }
}