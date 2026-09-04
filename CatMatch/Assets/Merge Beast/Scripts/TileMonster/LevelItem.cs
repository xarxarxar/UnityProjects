using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using MergeBeast;
using System;
using UnityEngine.Networking;

namespace Tiledom
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private int level;
        [SerializeField] Text txtLevel;
        [SerializeField] Transform rewardParent;
        [SerializeField] Sprite lootAvaliableSpr;
        [SerializeField] Sprite lootNotAvaliableSpr;
        [SerializeField] Slider slider;
        [SerializeField] Text txtSlider;
        [SerializeField] Button btnEnter;
        [SerializeField] Text txtRecord;

        JSONNode jsonData;//json cua continue

        int complete;
        private bool canClick = true;

        private void Awake()
        {
            slider.minValue = 0;
            slider.maxValue = 100;
        }

        private void Start()
        {
            btnEnter.onClick.AddListener(() => PlayGame());
        }
        public void LoadData()
        {
            Debug.LogError("level unlock: " + CPlayer.levelUnlock);
            //check co the unlock level ko
            if(level <= CPlayer.levelUnlock) {
                btnEnter.gameObject.SetActive(true);
            } else {
                btnEnter.gameObject.SetActive(false);
            }


            txtLevel.text = $"等级 {level}";
            string path = $"{SaveSystem.TILE_CONTINUE}{level}.json";

            string data = SaveSystem.Load(path);
            if (data != null)
            {
                jsonData = JSON.Parse(SaveSystem.Load(path));
                if (jsonData != null)
                {
                    int complete = jsonData["complete"].AsInt;
                    this.complete = complete;
                    slider.minValue = 0;
                    slider.maxValue = 100;
                    slider.value = complete;
                    txtSlider.text = $"{complete}%";                    
                    JSONArray items = jsonData["items"].AsArray;
                    LoadLootImage(items);

                    if(complete == 100) {
                        slider.gameObject.SetActive(false);
                        //rewardParent.gameObject.SetActive(false);
                        txtRecord.text = $"纪录<color=#eeb230>{GameManager._Instance.GetRecord(level)}</color>秒";
                    } else {
                        txtRecord.text = "";
                    }

                    //CHECK KHI HOAN THANH GAME, BAY MANH GHEP
                    bool rewarded = jsonData["rewarded"].AsBool;
                    if(!rewarded && complete == 100) {                        
                        GameManager._Instance.StartCoroutine(_CheckGameFinish(items));                        
                    }
                   

                }
            }
            else
            {

                LoadOrigin();
            }

        }

        IEnumerator _CheckGameFinish(JSONArray items) {
            canClick = false;
            yield return new WaitForSeconds(0.5f);
            canClick = true;
            bool isDone = true;
            for(int i = 0; i < items.Count; i++) {
                if(!items[i]["isDone"].AsBool) {
                    isDone = false;
                    break;
                }
            }
            if(isDone) {
                PrizeController.Instance.CollectPiece(transform.position);
                //save lai data 
                jsonData["rewarded"] = true;
                string path = SaveSystem.TILE_CONTINUE + CPlayer.currentLevel + ".json";
                SaveSystem.Save(path, jsonData.ToString());
            }
            
        }

        void LoadOrigin()
        {
            
            string path = SaveSystem.TILE_ORIGIN + level + ".json";            
            JSONNode json = JSON.Parse(SaveSystem.Load(path));            
            complete = 0;
            slider.value = 0;
            txtSlider.text = $"{complete}%";

            JSONArray items = json["items"].AsArray;
            
            LoadLootImage(items);

        }

        void LoadLootImage(JSONArray items) {
            for (int i = 0; i < rewardParent.childCount; i++)
            {
                Image img = rewardParent.GetChild(i).GetComponent<Image>();

                img.gameObject.SetActive(i < items.Count);
                if (i < items.Count)
                {
                    img.sprite = GameManager._Instance.GetSpriteBeast(items[i]["type"].AsInt);
                    GameObject tick = img.transform.GetChild(0).gameObject;
                    tick.SetActive(items[i]["isDone"].AsBool);
                }
            }
        }

        private void PlayGame()
        {
            if (!canClick) return;
            CPlayer.currentLevel = level;
            if(complete == 100) {
                //vao choi va ko mat luot               
                LevelController.instance.Hide();
                GameManager._Instance.isNewGame = true;
                GameManager._Instance.StartGame();
            } else {
                if (LevelController.instance.GetTotalTurn() > 0) {                    
                    
                    if (complete == 0) {
                        LevelController.instance.Hide();
                        GameManager._Instance.isNewGame = true;
                        GameManager._Instance.StartGame();
                        LevelController.instance.AddTurn(-1);
                    } else {
                        LevelController.instance.ShowConfirmPlay();
                    }

                } else {
                    if(complete == 0) {
                        LevelController.instance.BuyTurn();
                    } else {
                        LevelController.instance.ShowConfirmPlay();
                    }
                    
                }
            }
            


        }


    }
}