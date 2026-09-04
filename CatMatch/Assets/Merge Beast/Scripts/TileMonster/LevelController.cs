using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MergeBeast;
using Observer;
using System;
using UnityEngine.Networking;

namespace Tiledom
{
    public class LevelController : MonoBehaviour
    {
        public static LevelController instance;
        [SerializeField] GameObject overlay;
        [SerializeField] Button btnBack;
        [SerializeField] Text txtGem;
        [SerializeField] Text txtTurn;
        [SerializeField] Text txtTileCoin;
        [SerializeField] Transform levelParent;

        List<LevelItem> listLevelItem = new List<LevelItem>();

        [SerializeField] BuyTurnController buyTurn;
        [SerializeField] ConfirmPlay confirmPlay;

        [Header("TOast")]
        [SerializeField] Animator toast;
        [SerializeField] Text txtToast;

        private int totalTurn;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            listLevelItem.Clear();
            foreach(Transform child in levelParent) {
                listLevelItem.Add(child.GetComponent<LevelItem>());
            }
        }

        private void Start()
        {
            btnBack.onClick.AddListener(() => BackToGame());

            this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => OnUpdateMoney());       
            this.RegisterListener(EventID.OnUpdateTileTurn, (sender, param) => OnUpdateTileTurn());     

        }

        private void OnEnable()
        {
            BroadCastReceiver.Register(StringDefine.CHANGE_MCOIN, () => {
                ChangeMCoin();
            }, this);
            Show();

        }

        private void OnDisable() {
            BroadCastReceiver.UnRegister(StringDefine.CHANGE_MCOIN);
        }

        void OnUpdateMoney() {
            txtGem.text = Utils.FormatNumber(Utils.GetCurrentRubyMoney());
        }

        void OnUpdateTileTurn() {
            totalTurn = PlayerPrefs.GetInt(StringDefine.COUNT_TILE_TURN, Config.TILE_TURN_PER_DAY);
            txtTurn.text = totalTurn.ToString();
        }

        private void BackToGame()
        {
            Scene game = SceneManager.GetSceneByName(StringDefine.SCENE_TILE);
            if (game.isLoaded)
            {
                SceneManager.UnloadSceneAsync(game);
                SoundManager.Instance.MuteMainGame(false);                

            }
        }

        public void Hide()
        {
            overlay.SetActive(false);
        }

        void ChangeMCoin() {
            int mcoin = CPlayer.GetMCoin();
            txtTileCoin.text = Utils.FormatNumber(mcoin);
        }

        public void Show()
        {
            foreach(var item in listLevelItem) {
                item.LoadData();
            }
            Header.instance.ResetTime();
            overlay.SetActive(true);
            foreach(Transform child in GameManager._Instance.mapParent) {
                Destroy(child.gameObject);
            }

            totalTurn = PlayerPrefs.GetInt(StringDefine.COUNT_TILE_TURN, Config.TILE_TURN_PER_DAY);
            DateTime lastDay = DateTime.Parse(PlayerPrefs.GetString(StringDefine.LAST_DAY_TILE, DateTime.Today.AddDays(-1).ToString()));
            DateTime today = DateTime.Today;
            int dayDiff = (today - lastDay).Days;
            if (dayDiff > 0)
            {
                totalTurn = Config.TILE_TURN_PER_DAY;
                PlayerPrefs.SetInt(StringDefine.COUNT_TILE_TURN, totalTurn);
            }
            txtTurn.text = totalTurn.ToString();
            txtGem.text = Utils.FormatNumber(Utils.GetCurrentRubyMoney());

            ChangeMCoin();
        }

        public int GetTotalTurn()
        {
            return totalTurn;
        }
        public void AddTurn(int amount)
        {
            totalTurn += amount;
            if (totalTurn < 0) totalTurn = 0;
            PlayerPrefs.SetInt(StringDefine.COUNT_TILE_TURN, totalTurn);
        }

        public void BuyTurn()
        {
            buyTurn.gameObject.SetActive(true);
        }

        public void ShowConfirmPlay() {
            confirmPlay.gameObject.SetActive(true);
        }

        public void ShowToast(string msg) {
            txtToast.text = msg;
            toast.SetTrigger("Show");
        }

        
    }
}