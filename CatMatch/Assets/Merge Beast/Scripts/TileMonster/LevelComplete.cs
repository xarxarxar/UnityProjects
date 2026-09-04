using System;
using System.Collections;
using System.Collections.Generic;
using MergeBeast;
using UnityEngine;
using UnityEngine.UI;

namespace Tiledom
{
    public class LevelComplete : MonoBehaviour
    {
        [SerializeField] Text txtLevel;
        [SerializeField] Button btnReplay;
        [SerializeField] Button btnNext;
        [SerializeField] Text txtTime;
        [SerializeField] Transform itemParent;
        [SerializeField] GameObject txtNote;
        [SerializeField] Text txtRecord;
        [SerializeField] List<GameObject> listItem;

        [SerializeField] Button btnX2MCoin;
        [SerializeField] Text txtMcoin;

        private int mCoin;

        private void OnEnable() {          
            
            if(CPlayer.currentLevel + 1 >  CPlayer.levelUnlock) {
                CPlayer.levelUnlock += 1;
                CPlayer.SaveUnlock();
            }

            foreach(var item in listItem) {
                item.SetActive(false);
            }
            txtLevel.text = "完成关卡 " + CPlayer.currentLevel;
            Header.instance.allowCount = false;
            GameManager._Instance.SaveDataToContinue();
            txtTime.text = $"时间{Header.instance.GetTimeFinish()}秒";

            if(GameManager._Instance.HasRecord(CPlayer.currentLevel)) {
                int record = GameManager._Instance.GetRecord(CPlayer.currentLevel);
                int timePlay = Header.instance.GetTimeFinish();
                if (timePlay < record) {
                    GameManager._Instance.SaveRecordToOrigin(timePlay, CPlayer.currentLevel);
                    txtRecord.text = $"纪录{timePlay}秒";
                } else {
                    txtRecord.text = $"纪录{record}秒";
                }
            } else {
                //neu ko co record
                GameManager._Instance.SaveRecordToOrigin(Header.instance.GetTimeFinish(), CPlayer.currentLevel);
                txtRecord.text = $"纪录{Header.instance.GetTimeFinish()}秒";
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
                if(type == (int) EnumDefine.TileLoot.Soul) {
                    txtAmount.text += "p";
                }
                
            }
            mCoin = GameManager._Instance.listTrash.Count;
            //show mcoin
            txtMcoin.text = mCoin.ToString();
            CPlayer.AddMCoin(mCoin);
            btnX2MCoin.gameObject.SetActive(AdsManager.Instance.IsLoaded());
        }

        void GetX2MCoin() {
            AdsManager.Instance.ShowAds(() => {
                btnX2MCoin.gameObject.SetActive(false);
                CPlayer.AddMCoin(mCoin);
                txtMcoin.text = (mCoin * 2).ToString();
            }, "", EnumDefine.ADSTYPE.Rewarded, true, EnumDefine.GameType.Tile);
        }

        void Start() {
            btnX2MCoin.onClick.AddListener(() => GetX2MCoin());
            btnReplay.onClick.AddListener(() => ReplayGame());
            btnNext.onClick.AddListener(() => Hide());
        }

        private void Hide()
        {
            LevelController.instance.Show();            
            gameObject.SetActive(false);
        }

        private void ReplayGame()
        {
            //luu lai data khi da thay doi items, du lieu ko thay doi gi, chi co items la thay doi
            string path = SaveSystem.TILE_ORIGIN + CPlayer.currentLevel + ".json";
            SaveSystem.Save(path, GameManager._Instance.jsonData.ToString());
            GameManager._Instance.isNewGame = true;
            GameManager._Instance.StartGame();
            Header.instance.ResetTime();
            gameObject.SetActive(false);
        }
    }
}