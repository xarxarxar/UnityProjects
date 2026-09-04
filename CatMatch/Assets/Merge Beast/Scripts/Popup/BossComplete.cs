using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MergeBeast {
    public class BossComplete : MonoBehaviour {
        [SerializeField] private GameObject ui;
        [SerializeField] private Text noteText;
        [SerializeField] private Text monsterText;
        [SerializeField] private Text soulText;
        [SerializeField] private Text gemText;
        [SerializeField] private Text medalText;
        [SerializeField] private Text boostChestText;
        [SerializeField] private UIButton btnNextStage;

        private float timeNext = 10f;
        private Text buttonText;
        private bool allowChangeScene = false;
        private int lvMonster;

        private void Awake() {
            btnNextStage.onClick.AddListener(() => Close());
            buttonText = btnNextStage.GetComponentInChildren<Text>();
        }

        private void Close() {
            UIManager.Instance.shouldLoadEnemy = true;
            UIManager.Instance.HidePopup(ui.transform, () => {                
                Scene _sceneMap = SceneManager.GetSceneByName(StringDefine.SCENE_MAP);
                if (_sceneMap.IsValid()) {
                    SceneManager.SetActiveScene(_sceneMap);
                } else {
                    SceneManager.LoadScene(StringDefine.SCENE_MAP, LoadSceneMode.Additive);
                }
                Debug.Log(GameManager.Instance.GetTimeCountVideo() + ", " + Config.TIME_VIEW_VIDEO);
                if (GameManager.Instance.GetTimeCountVideo() > Config.TIME_VIEW_VIDEO) {
                    AdsManager.Instance.ShowAds(null, "", EnumDefine.ADSTYPE.Interstitial, false);
                    GameManager.Instance.ResetTimeCountVideo();
                }
            });

        }

        public void Show(int lvMonster, string monsterName) {
            this.lvMonster = lvMonster;
            UIManager.Instance.ShowPopup(ui.transform);
            noteText.text = $"你已击败第{lvMonster}关\n<color=#55E539FF><size=60>{monsterName}</size></color>\n你获得：";
            monsterText.text = monsterName;
            string soul = DataConfig.ListSoulReward[lvMonster - 1];
            soulText.text = "+" + Utils.FormatNumber(BigInteger.Parse(soul));
            int gem = DataConfig.ListGemReward[lvMonster - 1].AsInt;            
            gemText.text = $"+{gem}";
            gemText.transform.parent.gameObject.SetActive(gem > 0);

            int medal = DataConfig.ListMedalReward[lvMonster - 1].AsInt;
            medalText.text = $"+{medal}";
            medalText.transform.parent.gameObject.SetActive(medal > 0);

            int chest = DataConfig.ListChestReward[lvMonster - 1].AsInt;
            boostChestText.text = $"+{chest}";
            boostChestText.transform.parent.gameObject.SetActive(chest > 0);

            allowChangeScene = false;
            timeNext = 10f;

        }

        private void Update() {
            if (!allowChangeScene && ui.activeInHierarchy) {
                timeNext -= Time.deltaTime;
                buttonText.text = $"下一关\n({Mathf.RoundToInt(timeNext)})";                
                if (timeNext <= 0) {
                    allowChangeScene = true;
                    Close();
                }
            }
        }



    } //end class

}