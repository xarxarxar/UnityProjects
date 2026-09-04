using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeBeast;
using UnityEngine.SceneManagement;

namespace Tiledom
{
    public class GameOverController : MonoBehaviour
    {        
        [SerializeField] Button btnQuit;
        [SerializeField] Button btnBuyTime;
        [SerializeField] Button btnWatchAds;
        [SerializeField] Button btnContinue;

        [SerializeField] Transform itemParent;
        [SerializeField] GameObject txtNote;
        [SerializeField] Text txtProgress;
        [SerializeField] Text txtTotalTime;
        [SerializeField] Text txtRecord;
        [SerializeField] List<GameObject> listItem;

        [SerializeField] Button btnX2MCoin;
        [SerializeField] Text txtMcoin;

        private int mCoin;

        private void OnEnable() {
            foreach (var item in listItem) {
                item.SetActive(false);
            }

            if(GameManager._Instance.GetProgress() >= 50) {
                if(CPlayer.currentLevel + 1 > CPlayer.levelUnlock) {
                    CPlayer.levelUnlock += 1;
                    CPlayer.SaveUnlock();
                }
            }

            txtProgress.text = $"{GameManager._Instance.GetProgress()}%";
            txtTotalTime.text = $"时间{Header.instance.GetTimeFinish()}秒"; //use
            if(GameManager._Instance.HasRecord(CPlayer.currentLevel)) {
                txtRecord.text = $"纪录{GameManager._Instance.GetRecord(CPlayer.currentLevel)}秒"; 
            } else {
                txtRecord.text = "无记录";
            }
            Header.instance.allowCount = false;

            if(CPlayer.GetRevival() > 0) {
                btnContinue.gameObject.SetActive(true);
                btnContinue.GetComponentInChildren<Text>().text = $"使用\n({CPlayer.GetRevival()})";
                btnWatchAds.gameObject.SetActive(false);
            } else {
                btnContinue.gameObject.SetActive(false);
                btnWatchAds.gameObject.SetActive(AdsManager.Instance.IsLoaded(EnumDefine.ADSTYPE.Rewarded));
            }
            

            txtNote.SetActive(GameManager._Instance.listItemRewarded.Count > 0);
            for (int i = 0; i < GameManager._Instance.listItemRewarded.Count; i++) {
                int type = GameManager._Instance.listItemRewarded[i];
                GameObject go = listItem[i];
                go.SetActive(true);
                Image icon = go.transform.GetChild(0).GetComponent<Image>();
                Text txtAmount = go.transform.GetChild(1).GetComponent<Text>();

                icon.sprite = GameManager._Instance.GetSpriteBeast(type);

                txtAmount.text = GameManager._Instance.lootConfig.GetAmountBuyType(CPlayer.currentLevel, type).ToString();
                if (type == (int)EnumDefine.TileLoot.Soul) {
                    txtAmount.text += "p";
                }

            }

            mCoin = GameManager._Instance.listTrash.Count;
            txtMcoin.text = mCoin.ToString();
            btnX2MCoin.gameObject.SetActive(AdsManager.Instance.IsLoaded());
        }

        void GetX2MCoin() {
            AdsManager.Instance.ShowAds(() => {
                btnX2MCoin.gameObject.SetActive(false);
                mCoin *= 2;
                txtMcoin.text = mCoin.ToString();
            }, "", EnumDefine.ADSTYPE.Rewarded, true, EnumDefine.GameType.Tile);
        }
        void Start() {
            //btnRestart.onClick.AddListener(() => RestartGame());
            btnQuit.onClick.AddListener(() => BackToLevel());
            // btnBuyTime.onClick.AddListener(() => BuyTime());
            btnWatchAds.onClick.AddListener(() => WatchAdsToContinue());
            btnContinue.onClick.AddListener(() => UseReviveToContinue());
            btnX2MCoin.onClick.AddListener(() => GetX2MCoin());
        }

        private void BackToLevel()
        {
            //xoa current level
            GameManager._Instance.ClearLevelData();

            CPlayer.AddMCoin(mCoin);
            LevelController.instance.Show();
            gameObject.SetActive(false);
        }

        void Hide() {
            gameObject.SetActive(false);
        }

        private void RestartGame()
        {
            // Debug.Log(GameManager._Instance.jsonData.ToString());
            string path = SaveSystem.TILE_ORIGIN + CPlayer.currentLevel + ".json";
            SaveSystem.Save(path, GameManager._Instance.jsonData.ToString());
            GameManager._Instance.isNewGame = true;
            GameManager._Instance.StartGame();
            CPlayer.AddMCoin(mCoin);
            Hide();

        }


        private void WatchAdsToContinue() {
            AdsManager.Instance.ShowAds(() => {
                GameManager._Instance.RestartWhenGameOver();
                Hide();
            }, null, EnumDefine.ADSTYPE.Rewarded, true, EnumDefine.GameType.Tile);
            
        }

        private void UseReviveToContinue() {
            CPlayer.AddRevival(-1);
            GameManager._Instance.RestartWhenGameOver();
            Hide();
        }
    }
}