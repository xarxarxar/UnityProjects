using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Linq;
using System.IO;
using DG.Tweening;
using UnityEngine.Networking;
using System;
using MergeBeast;
using Observer;

namespace Tiledom
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager _Instance;
        public Camera cam;
        public TileLootConfig lootConfig;
        public Transform mapParent;
        public Transform trashParent; //khi cac tile dc move khoi map se nam o day
        public bool canClick = true;
        public List<TileController> listTrash = new List<TileController>();
        [SerializeField] Button btnUndo;
        [SerializeField] Text undoText;
        [SerializeField] Button btnCommission;
        [SerializeField] Text commissionText;
        [SerializeField] Button btnShuffle;
        [SerializeField] Text shuffleText;
        [SerializeField] List<LootItem> listLootItem;


        [Header("==== Prefab ===")]
        public GameObject tileLayerPrefab;
        public GameObject tilePrefab;
        public GameObject tileMovePrefab;

        [SerializeField] Sprite[] listSprite;

        //script
        public SlotController slotController;
        [SerializeField] GameOverController gameOverController;
        [SerializeField] LevelComplete levelComplete;
        //[SerializeField] FastBuyController fastBuyController;        

        // [SerializeField] GameObject completeGameObj;
        // [SerializeField] Button btnRate;

        private List<int> listType = new List<int>(); //danh sach type
        public List<TileController> listAllTile = new List<TileController>();
        public bool isNewGame;
        public JSONArray listItem;
        private Vector2[] arrStartPos = {
            new Vector2(-1080, 0),
            new Vector2(50, 1920),
            new Vector2(0, 1080),
            new Vector2(50, -1920)
        };

        List<Transform> listLayer;
        private float timeCount = 0f;
        private int point = 0; //so diem con lai
        private int totalPoint; //tong so diem cua ban choi
        private int myPoint;//so diem cua minh
        private bool allowCheckPoint = true;

        float deltaTime = 0f;
        float timeToShowFps = 0.5f;
        float timeCountVideo = 0f;
        private float timeCountClick = 0f;

        public JSONNode jsonData;

        [HideInInspector] public List<int> listItemRewarded = new List<int>(); //danh sach loot an dc, neu loot da an roi thi se ko dc show nua



        private void OnEnable()
        {
            BroadCastReceiver.Register(StringDefine.CHANGE_ITEM, () =>
            {
                undoText.text = CPlayer.undoAmount.ToString();
                commissionText.text = CPlayer.commissionAmount.ToString();
                shuffleText.text = CPlayer.shuffleAmount.ToString();
            }, this);

            BroadCastReceiver.Register(StringDefine.CHANGE_THEME, () =>
            {
                foreach (var item in listAllTile)
                {
                    item.ChangeTheme();
                }
                foreach (var item in listTrash)
                {
                    item.ChangeTheme();
                }
            }, this);


        }

        private void OnDisable()
        {
            BroadCastReceiver.UnRegister(StringDefine.CHANGE_ITEM);
            BroadCastReceiver.UnRegister(StringDefine.MATCH_TILE);
            BroadCastReceiver.UnRegister(StringDefine.CHANGE_THEME);
        }

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
            //PlayerPrefs.DeleteAll();


            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (AdsManager.Instance != null)
            {

            }
            // if (SoundManager.Instance != null)
            //     SoundManager.Instance.MuteMainGame(true);
            //Debug.LogError("================== DeviceID: " + SystemInfo.deviceUniqueIdentifier);
        }

        private void Start()
        {
            btnUndo.onClick.AddListener(() => Undo());
            btnCommission.onClick.AddListener(() => Commision());
            btnShuffle.onClick.AddListener(() => Shuffle());

            undoText.text = CPlayer.undoAmount.ToString();
            commissionText.text = CPlayer.commissionAmount.ToString();
            shuffleText.text = CPlayer.shuffleAmount.ToString();

            // if (SoundController._Instance != null) SoundController._Instance.PlayGamePlayBG();

            // btnRate.onClick.AddListener(() => ShowRate());
        }

        private void Update()
        {
            //if (allowCheckPoint) {
            //    timeCount += Time.deltaTime;
            //    if (timeCount > 10) {
            //        timeCount = 0;
            //        allowCheckPoint = false;
            //        //point--;

            //    }
            //}
            timeCountClick += Time.deltaTime;
            if (timeCountClick >= 1f)
            {
                timeCountClick = 0f;
                canClick = true;
            }

        }

        public float GetTimeCountVideo()
        {
            return timeCountVideo;
        }

        public void ResetTimeCountVideo()
        {
            timeCountVideo = 0f;
        }

        public void StartGame()
        {
            listItemRewarded.Clear();

            PlayerPrefs.SetString(StringDefine.LAST_DAY_TILE, DateTime.Today.ToString());

            listAllTile.Clear();
            listTrash.Clear();
            listType.Clear();
            foreach (Transform child in mapParent)
            {
                Destroy(child.gameObject);
            }

            slotController.Reset();
            foreach (Transform child in slotController.slotParent)
            {
                Destroy(child.gameObject);
            }

            mapParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -814);
            Debug.Log("is new game: " + isNewGame);
            if (isNewGame)
            {
                StartCoroutine(_LoadMapData());
            }
            else
            {
                LoadContinue();
            }


        }

        void ShowLootItem()
        {
            for (int i = 0; i < listLootItem.Count; i++)
            {
                listLootItem[i].gameObject.SetActive(i < listItem.Count);
            }
            for (int i = 0; i < listItem.Count; i++)
            {
                JSONNode item = listItem[i].AsObject;
                int type = item["type"].AsInt;
                bool done = item["isDone"].AsBool;
                listLootItem[i].UpdateIcon(type);
                listLootItem[i].SetDone(done);

                if (isNewGame)
                {
                    int amount = GetTotalTileInList(type, listAllTile);
                    if (done)
                    {
                        listLootItem[i].UpdateText($"{amount}/{amount}");
                    }
                    else
                    {
                        listLootItem[i].UpdateText($"0/{amount}");
                    }
                }
                else
                {
                    int totalPiece = CountLootPiece(type, listTrash) + CountLootPiece(type, listAllTile);
                    int ownedPiece = CountLootPiece(type, listTrash);
                    listLootItem[i].UpdateText($"{ownedPiece}/{totalPiece}");
                }


            }
        }

        /// <summary>
        /// Dem so manh ghep cua loot
        /// </summary>
        /// <returns></returns>
        int CountLootPiece(int type, List<TileController> list)
        {
            int count = 0;
            foreach (var item in list)
            {
                if (item.GetTileType() == type)
                {
                    count++;
                }
            }
            return count;
        }

        private IEnumerator _LoadMapData()
        {
            //kiem tra trong folder                        

            int currentLevel = CPlayer.currentLevel;
            string path = SaveSystem.TILE_ORIGIN + currentLevel + ".json";

            // var loadingRequest = UnityWebRequest.Get(path);
            // // Debug.Log("== =======start load map: " + TimeUtil.TimeStampSecond);
            // yield return loadingRequest.SendWebRequest();
            // // Debug.Log("== ===========end load map: " + TimeUtil.TimeStampSecond);
            // if (loadingRequest.isHttpError || loadingRequest.isNetworkError)
            // {
            //     Debug.Log("Het Map");
            //     yield break;
            // }

            // JSONNode json = JSON.Parse(loadingRequest.downloadHandler.text);
            string data = SaveSystem.Load(path);
            jsonData = new JSONObject();
            if (data != null)
            {
                jsonData = JSON.Parse(data);
            }

            //Debug.LogError(jsonData.ToString());
            int totalLayer = jsonData["layers"].AsInt;
            JSONArray arrTile = jsonData["tiles"].AsArray;
            JSONArray arrRate = jsonData["rate"].AsArray;
            listItem = jsonData["items"].AsArray;



            //tinh point
            totalPoint = arrTile.Count / 3;
            point = totalPoint;
            BroadCastReceiver.Broadcast(StringDefine.CHANGE_CURRENT_POINT, point);
            //gen list type
            GenListType(arrRate, arrTile.Count);

            //check list
            //tinh phan chenh lech giua 2 list, vi du khi lam tron list type = 57, con total = 60
            int diff = Mathf.Abs(arrTile.Count - listType.Count);
            int temp = 0;
            int typeR = arrRate[0]["type"].AsInt;
            while (true)
            {
                if (diff == 0) break;

                if (temp % 3 == 0)
                {
                    temp = 0;
                    int randomType = UnityEngine.Random.Range(0, arrRate.Count);
                    typeR = arrRate[randomType]["type"].AsInt;

                }
                listType.Add(typeR);
                temp++;
                diff--;
            }

            //shuffer
            listType = listType.OrderBy(c => System.Guid.NewGuid()).ToList();
            //listType.Sort();

            listLayer = new List<Transform>();
            //create layer parent
            Vector2 finalPos = Vector2.zero;
            Vector2 startPos = Vector2.zero;
            for (int i = 0; i < totalLayer; i++)
            {

                if (i % 2 == 0)
                {
                    finalPos = Vector2.zero;
                }
                else
                {
                    finalPos = new Vector2(50, -50);
                }
                startPos = arrStartPos[i % 4];

                GameObject layerParent = Instantiate(tileLayerPrefab, mapParent);
                layerParent.name = i.ToString();
                listLayer.Add(layerParent.transform);

                layerParent.GetComponent<RectTransform>().anchoredPosition = startPos;

            }

            //create tile 
            for (int i = 0; i < arrTile.Count; i++)
            {
                JSONNode tileData = arrTile[i].AsObject;
                float x = tileData["x"].AsFloat;
                float y = tileData["y"].AsFloat;
                int layer = tileData["layer"].AsInt;

                GameObject go = Instantiate(tilePrefab, listLayer[layer]);
                go.name = i.ToString();
                TileController script = go.GetComponent<TileController>();// PoolController._Instance.GetTile();                
                //script.transform.parent = listLayer[layer];
                //script.gameObject.SetActive(true);
                //script.transform.localScale = Vector3.one;
                script.SetUp(i, listType[i], layer, new Vector2(x, y));
                //script.SetUp(i, listType[i], layer, new Vector2(100 * i, 0));

                listAllTile.Add(script);
            }


            ShowLootItem();
            StartCoroutine(_ShowMapAnimation());

            //check map chan hay map le
            RePosition();
            yield return new WaitForSeconds(0.3f);
            Header.instance.allowCount = true;



        }

        int GetTotalLayerContinue(JSONArray list1, JSONArray list2, JSONArray list3)
        {
            int layer = list3[0]["layer"].AsInt;
            List<int> layerId = new List<int>();
            layerId.Add(layer);
            foreach (var item in list1)
            {
                int l = item.Value["layer"].AsInt;
                if (!layerId.Contains(l))
                {
                    layerId.Add(l);
                }
            }

            foreach (var item in list2)
            {
                int l = item.Value["layer"].AsInt;
                if (!layerId.Contains(l))
                {
                    layerId.Add(l);
                }
            }

            foreach (var item in list3)
            {
                int l = item.Value["layer"].AsInt;
                if (!layerId.Contains(l))
                {
                    layerId.Add(l);
                }
            }
            return layerId.Count;
        }

        void LoadContinue()
        {
            string path = $"{SaveSystem.TILE_CONTINUE}{CPlayer.currentLevel}.json";

            string data = SaveSystem.Load(path);
            if (data != null)
            {

                jsonData = JSON.Parse(SaveSystem.Load(path));
                if (jsonData != null)
                {
                    int remainTime = jsonData["remainTime"].AsInt;
                    Header.instance.SetTime(remainTime);
                    Header.instance.allowCount = true;

                    JSONArray tileMoves = jsonData["tileMoves"].AsArray;
                    JSONArray listTrash = jsonData["listTrash"].AsArray;
                    JSONArray listAllTile = jsonData["listAllTile"].AsArray;
                    listItem = jsonData["items"].AsArray;

                    //tao layer
                    int totalLayer = GetTotalLayerContinue(tileMoves, listTrash, listAllTile);
                    listLayer = new List<Transform>();
                    //create layer parent
                    Vector2 finalPos = Vector2.zero;
                    Vector2 startPos = Vector2.zero;
                    for (int i = 0; i < totalLayer; i++)
                    {

                        if (i % 2 == 0)
                        {
                            finalPos = Vector2.zero;
                        }
                        else
                        {
                            finalPos = new Vector2(50, -50);
                        }
                        startPos = arrStartPos[i % 4];

                        GameObject layerParent = Instantiate(tileLayerPrefab, mapParent);
                        layerParent.name = i.ToString();
                        listLayer.Add(layerParent.transform);

                        layerParent.GetComponent<RectTransform>().anchoredPosition = startPos;

                    }




                    //trash
                    for (int i = 0; i < listTrash.Count; i++)
                    {
                        JSONNode tile = listTrash[i].AsObject;

                        GameObject trash = Instantiate(tilePrefab, trashParent);
                        trash.SetActive(false);
                        TileController script = trash.GetComponent<TileController>();
                        script.SetUp(tile["id"].AsInt, tile["type"].AsInt, tile["layer"],
                        new Vector2(tile["x"].AsInt, tile["y"].AsInt));
                        this.listTrash.Add(script);
                    }



                    //all tile
                    for (int i = 0; i < listAllTile.Count; i++)
                    {
                        JSONNode tile = listAllTile[i].AsObject;

                        GameObject tileController = Instantiate(tilePrefab, listLayer[tile["layer"].AsInt]);
                        TileController script = tileController.GetComponent<TileController>();
                        script.SetUp(tile["id"].AsInt, tile["type"].AsInt, tile["layer"],
                        new Vector2(tile["x"].AsInt, tile["y"].AsInt));
                        this.listAllTile.Add(script);
                    }

                    //tile move
                    slotController.Reset();
                    for (int i = 0; i < tileMoves.Count; i++)
                    {
                        JSONNode tile = tileMoves[i].AsObject;

                        TileController tileController = GetTileInListById(tile["id"].AsInt, this.listTrash);
                        AddSlot(tileController, false);
                    }

                    ShowLootItem();

                    StartCoroutine(_ShowMapAnimation());

                    //check map chan hay map le
                    RePosition();

                }
            }
        }

        /// <summary>
        /// Tính lại vị trí của mapParent để cho map luôn nằm ở giữa 
        /// </summary>
        private void RePosition()
        {
            float min = 0;
            float max = 0;
            float top = 0;
            float bottom = 0;
            for (int i = 0; i < listAllTile.Count; i++)
            {
                TileController tile = listAllTile[i];
                int layer = tile.GetLayer();
                var x = tile.GetPos().x;
                var y = tile.GetPos().y;

                if (layer % 2 == 1)
                {
                    x += 50;
                    y -= 50;
                }

                if (y < bottom)
                {
                    bottom = y;
                }
                else if (y > top)
                {
                    top = y;
                }


                if (x < min)
                {
                    min = x;
                }
                else if (x > max)
                {
                    max = x;
                }
            }


            float distance = Mathf.Abs(max) - Mathf.Abs(min);
            float distanceHeight = Mathf.Abs(top) - Mathf.Abs(bottom);

            RectTransform mapRtf = mapParent.GetComponent<RectTransform>();
            Vector2 newPos = new Vector2(-distance / 2 * 1.14f, mapRtf.anchoredPosition.y - distanceHeight / 2 * 1.14f);
            mapRtf.anchoredPosition = newPos;
        }

        IEnumerator _ShowMapAnimation()
        {
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer[i].GetComponent<RectTransform>().DOAnchorPos(i % 2 == 0 ? Vector2.zero : new Vector2(50, -50), 0.3f);
            }
            yield return new WaitForSeconds(0.5f);
            foreach (var script in listAllTile)
            {
                script.CheckParent();
            }

        }

        private void GenListType(JSONArray arr, int totalTile)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                JSONNode note = arr[i].AsObject;
                int type = note["type"].AsInt;
                int rate = note["rate"].AsInt;

                int numOfTile = totalTile * rate / 100;
                int phanDu = numOfTile % 3; //phần dư cho 3
                if (phanDu == 0)
                {
                    //data ok
                    AddListType(type, numOfTile);
                }
                else
                {
                    AddListType(type, numOfTile - phanDu);
                }
            }

        }

        private void AddListType(int type, int num)
        {
            for (int i = 0; i < num; i++)
            {
                listType.Add(type);
            }
        }

        public void AddSlot(TileController tile, bool addListTrash = true)
        {

            slotController.AddSlot(tile);
            if (addListTrash)
                listTrash.Add(tile);

            //chi check doi voi nhung tile > 20, vi > 20 la loot
            if (tile.GetTileType() >= 20)
            {
                //check trong list item, neu item nao co type trung voi tile.gettiletype thi moi check
                for (int i = 0; i < listItem.Count; i++)
                {
                    JSONNode item = listItem[i].AsObject;
                    if (item["type"] == tile.GetTileType() && !listLootItem[i].GetDone())
                    {
                        int totalPiece = CountLootPiece(tile.GetTileType(), listTrash) + CountLootPiece(tile.GetTileType(), listAllTile);
                        int ownedPiece = CountLootPiece(tile.GetTileType(), listTrash);
                        listLootItem[i].UpdateText($"{ownedPiece}/{totalPiece}");
                    }
                }

            }

            //check collect item
            for (int i = 0; i < listItem.Count; i++)
            {
                JSONNode item = listItem[i].AsObject;
                //neu item chua dc collect thi moi check                
                if (!item["isDone"].AsBool)
                {
                    bool done = IsDoneCollectType(item["type"].AsInt);
                    if (done)
                    {
                        listLootItem[i].SetDone(true);
                        item["isDone"] = true;

                        //luu vao list origin luon, de tránh farm

                        string originPath = SaveSystem.TILE_ORIGIN + CPlayer.currentLevel + ".json";
                        JSONNode json = JSON.Parse(SaveSystem.Load(originPath));
                        json["items"] = listItem;
                        SaveSystem.Save(originPath, json.ToString());
                        CollectLoot(item["type"].AsInt);
                    }
                }
            }

            CheckParentTile();
        }

        public void CheckParentTile()
        {
            foreach (var item in listAllTile)
            {
                if (item != null)
                {
                    item.CheckParent();
                }
            }
        }

        public void Restart()
        {
            //DestroyAllTile();
            UnityEngine.SceneManagement.SceneManager.LoadScene(StringDefine.SCENE_TILE,
            UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        public void GameOver()
        {
            gameOverController.gameObject.SetActive(true);
        }

        public void RemoveTileFromList(TileController tile)
        {
            if (listAllTile.Contains(tile))
            {
                listAllTile.Remove(tile);
            }

            //check game finish
            if (listAllTile.Count == 0)
            {
                //listTrash.Clear();             
                foreach (Transform child in slotController.slotParent)
                {
                    child.gameObject.SetActive(false);
                }
                StartCoroutine(_DelayWithAction(1f, () => levelComplete.gameObject.SetActive(true)));

            }
        }

        IEnumerator _DelayWithAction(float time, System.Action cb)
        {
            yield return new WaitForSeconds(time);
            if (cb != null) cb();
        }

        private void Undo(bool checkData = true)
        {
            if (checkData)
            { //normal thi true, khi mà ấn chơi lại thì ko cần check số lượng                
                if (CPlayer.undoAmount <= 0)
                {
                    // FastBuyController._Instance.Show(EnumData.ItemType.Undo);
                    return;
                }

                if (!canClick) return;
            }

            TileMove lastTile = slotController.GetLastTileMove();
            if (lastTile == null)
            {
                Debug.Log("ko co tile nao");
                return;
            }

            int id = lastTile.GetId();
            int layer = lastTile.GetLayer();

            //tim trong trash de lay id
            TileController tile = GetTileInListById(id, listTrash);

            if (tile == null) return;

            //an last tile
            lastTile.gameObject.SetActive(false);
            //remove lasttile khoi list
            slotController.RemoveLastTile();

            //set tile nay cung vi tri voi tile o duoi khay
            tile.gameObject.SetActive(true);
            tile.transform.position = lastTile.transform.position;

            //dua tile nay ve parent ban dau
            tile.transform.parent = listLayer[layer];
            tile.MoveToOrigin();

            //xoa tile nay khoi trash va dua vao list
            if (listTrash.Contains(tile))
            {
                listTrash.Remove(tile);
            }
            if (!listAllTile.Contains(tile))
            {
                listAllTile.Add(tile);
            }

            StartCoroutine(_DelayWithAction(0.3f, () =>
            {
                CheckParentTile();
            }));

            if (checkData)
                CPlayer.AddUndo(-1);

        }

        private void Commision()
        {
            if (CPlayer.commissionAmount <= 0)
            {
                // FastBuyController._Instance.Show(EnumData.ItemType.Commission);               
                return;
            }

            if (!canClick)
            {
                Debug.LogError("can not click");
                return;
            }
            if (slotController.listTileMove.Count >= 7 || slotController.listTileMove.Count == 0)
            {
                Debug.LogError("so tile ko du dieu kien");
                return;
            }

            canClick = false;
            timeCountClick = 0f;
            if (slotController.listTileMove.Count <= 5)
            {
                //lay random tile
                int random = UnityEngine.Random.Range(0, slotController.listTileMove.Count);
                TileMove tileMove = slotController.listTileMove[random];
                int type = tileMove.GetTileType();

                //kiem tra trong listTilemove xem co bao nhieu type nhu type vua random
                int countSameType = slotController.CountType(type);

                //tinh số tile còn thiếu
                int needTile = 3 - countSameType;

                //tìm các tile cần trong list all tile
                List<TileController> listNeeded = new List<TileController>();
                foreach (var item in listAllTile)
                {
                    //add vao lisst needed
                    if (item.GetTileType() == type)
                    {
                        listNeeded.Add(item);
                    }
                    //khi du? roi thi break
                    if (listNeeded.Count >= needTile) break;
                }
                for (int i = 0; i < listNeeded.Count; i++)
                {
                    int k = i;
                    StartCoroutine(_DelayWithAction(0.5f * k, () =>
                    {
                        //Debug.Log(i);
                        AddSlot(listNeeded[k]);
                        if (k == listNeeded.Count - 1)
                        {
                            canClick = true;
                        }
                    }));
                }

            }
            else
            {
                //con 1 slot thi phai kiem tra xem co 2 cai ko
                TileMove t = slotController.CountTwoType();
                if (t != null)
                {
                    //có 2 type trùng nhau => ok
                    TileController tileController = GetTileInListByType(t.GetTileType(), listAllTile);
                    AddSlot(tileController);
                    StartCoroutine(_DelayWithAction(0.5f, () => canClick = true));
                }
                else
                {
                    //khong co type nao trung nhau thi ko dùng đc
                    canClick = true;
                    Debug.LogError("khong co type nao trung nhau");
                    return;
                }

            }

            CPlayer.AddCommission(-1);
        }

        public void Shuffle(bool checkData = true)
        {

            if (checkData)
            {
                if (CPlayer.shuffleAmount <= 0)
                {
                    // FastBuyController._Instance.Show(EnumData.ItemType.Shuffle);
                    return;
                }
                if (!canClick) return;
            }

            List<int> newType = new List<int>();
            for (int i = 0; i < listAllTile.Count; i++)
            {
                newType.Add(listAllTile[i].GetTileType());
            }

            //shuffle list type
            newType = newType.OrderBy(c => System.Guid.NewGuid()).ToList();

            for (int i = 0; i < listAllTile.Count; i++)
            {
                TileController tile = listAllTile[i];
                tile.ChangeType(newType[i]);
            }

            if (checkData)
                CPlayer.AddShuffle(-1);
        }

        public void RestartWhenGameOver()
        {
            // SoundController._Instance.PlayGamePlayBG();
            canClick = false;
            Header.instance.allowCount = true;

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(_DelayWithAction(0.3f * i, () => Undo(false)));
            }

            StartCoroutine(_DelayWithAction(1f, () =>
            {
                Shuffle(false);
                canClick = true;
            }));
        }


        private TileController GetTileInListById(int id, List<TileController> list)
        {
            foreach (var item in list)
            {
                if (item.GetId() == id) return item;
            }
            return null;
        }

        private TileController GetTileInListByType(int type, List<TileController> list)
        {
            foreach (var item in list)
            {
                if (item.GetTileType() == type) return item;
            }
            return null;
        }


        public Sprite GetSpriteBeast(int type)
        {
            return listSprite[type];
        }

        private bool IsDoneCollectType(int type)
        {
            foreach (var item in listAllTile)
            {
                if (item.GetTileType() == type)
                {
                    return false;
                }
            }
            return true;
        }


        public void SaveDataToContinue()
        {
            //check data cu, nếu có data cũ thì kiểm tra xem rewarded = true hay false, 
            //neu ko co data cũ, thì có nghĩa la lần đầu lưu game => rewarded = false luôn
            bool rewarded = false;
            string path = SaveSystem.TILE_CONTINUE + CPlayer.currentLevel + ".json";
            string prevData = SaveSystem.Load(path);
            if (prevData == null)
            {
                //chua tung luu data
                rewarded = false;
            }
            else
            {
                JSONNode prevJson = JSON.Parse(prevData);
                if (prevJson["rewarded"].AsBool)
                {
                    rewarded = true; //neu = true thi khi ra level ko check nua, vi da nhan roi
                }
            }

            JSONNode data = new JSONObject();
            data["remainTime"] = Header.instance.GetTimeCount();
            float percent = (float)listTrash.Count / (listTrash.Count + listAllTile.Count);
            data["complete"] = Mathf.RoundToInt(percent * 100);
            data["rewarded"] = rewarded;
            JSONArray arrAllTile = new JSONArray();
            data["listAllTile"] = arrAllTile;



            foreach (var item in listAllTile)
            {
                JSONNode tile = new JSONObject();
                tile["id"] = item.GetId();
                tile["type"] = item.GetTileType();
                tile["layer"] = item.GetLayer();
                tile["x"] = item.GetPos().x;
                tile["y"] = item.GetPos().y;

                arrAllTile.Add(tile);
            }

            JSONArray arrTrash = new JSONArray();
            data["listTrash"] = arrTrash;

            foreach (var item in listTrash)
            {
                JSONNode tile = new JSONObject();
                tile["id"] = item.GetId();
                tile["type"] = item.GetTileType();
                tile["layer"] = item.GetLayer();
                tile["x"] = item.GetPos().x;
                tile["y"] = item.GetPos().y;

                arrTrash.Add(tile);
            }

            JSONArray arrMove = new JSONArray();
            data["tileMoves"] = arrMove;
            foreach (var item in slotController.listTileMove)
            {
                JSONNode tile = new JSONObject();
                tile["id"] = item.GetId();
                tile["type"] = item.GetTileType();
                tile["index"] = item.GetIndex();
                tile["layer"] = item.GetLayer();
                tile["x"] = item.GetFinalPos().x;
                tile["y"] = item.GetFinalPos().y;
                arrMove.Add(tile);
            }

            JSONArray arrItem = new JSONArray();
            data["items"] = GameManager._Instance.listItem;

            SaveSystem.Save($"{SaveSystem.TILE_CONTINUE}{CPlayer.currentLevel}.json", data.ToString());
        }

        public void ClearLevelData()
        {
            string path = $"{SaveSystem.TILE_CONTINUE}{CPlayer.currentLevel}.json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            //save trang thai cua cac item vao origin            
            //string origin = $"{SaveSystem.TILE_ORIGIN}{CPlayer.currentLevel}.json";
            //SaveSystem.Save(origin, GameManager._Instance.jsonData.ToString());
        }

        void CollectLoot(int type)
        {
            listItemRewarded.Add(type);

            int index = CPlayer.currentLevel - 1;
            ListLootItemData listData = lootConfig.list[index];
            for (int i = 0; i < listData.listItem.Count; i++)
            {
                LootItemData itemData = listData.listItem[i];
                if (type != (int)itemData.lootType) continue;
                switch (itemData.lootType)
                {
                    case EnumDefine.TileLoot.Soul:
                        UIManager.Instance.UpdateMoneyCoin(MergeBeast.GameManager.Instance.TotalDamage() * itemData.amount * 60, false);
                        break;
                    case EnumDefine.TileLoot.AutoMerge:
                        MergeBeast.GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.AUTO_MERGE_1, itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Dps:
                        MergeBeast.GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.DAMAGE_BOOST_1, itemData.amount);
                        break;
                    case EnumDefine.TileLoot.SpawnTime:
                        MergeBeast.GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.SPAWN_FASTER_1, itemData.amount);
                        break;
                    case EnumDefine.TileLoot.BoostChest:
                        MergeBeast.GameManager.Instance.AddDailyBoost(EnumDefine.DailyBoost.BOOST_CHEST, itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Gem:
                        Utils.AddRubyCoin(itemData.amount);
                        this.PostEvent(EventID.OnUpDateMoney);
                        break;
                    case EnumDefine.TileLoot.Star:
                        Utils.AddStar(itemData.amount);
                        break;
                    case EnumDefine.TileLoot.MedalMerge:
                        Utils.AddMedalMerge(itemData.amount, true);
                        this.PostEvent(EventID.OnUpdateAutoMergeMedal);
                        break;
                    case EnumDefine.TileLoot.Robin:

                        break;
                    case EnumDefine.TileLoot.MCoin:
                        CPlayer.AddMCoin(itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Undo:
                        CPlayer.AddUndo(itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Commit:
                        CPlayer.AddCommission(itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Shuffle:
                        CPlayer.AddShuffle(itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Revive:
                        CPlayer.AddRevival(itemData.amount);
                        break;
                    case EnumDefine.TileLoot.Chaos:
                        break;

                }
                LevelController.instance.ShowToast($"获得{itemData.lootType}×{itemData.amount}");
            }
        }

        public int GetProgress()
        {
            float percent = (float)listTrash.Count / (listTrash.Count + listAllTile.Count);
            return Mathf.RoundToInt(percent * 100);
        }

        public int GetTotalTileInList(int type, List<TileController> list)
        {
            int count = 0;
            foreach (var item in list)
            {
                if (item.GetTileType() == type)
                {
                    count++;
                }
            }
            return count;
        }

        #region record
        public void SaveRecordToOrigin(int time, int level)
        {
            JSONNode json = JSON.Parse(SaveSystem.Load($"{SaveSystem.TILE_ORIGIN}{level}.json"));
            json["record"] = time;
            SaveSystem.Save($"{SaveSystem.TILE_ORIGIN}{level}.json", json.ToString());

        }

        public bool HasRecord(int level)
        {
            JSONNode json = JSON.Parse(SaveSystem.Load($"{SaveSystem.TILE_ORIGIN}{level}.json"));
            return json.HasKey("record");
        }

        public int GetRecord(int level)
        {
            JSONNode json = JSON.Parse(SaveSystem.Load($"{SaveSystem.TILE_ORIGIN}{level}.json"));
            return json["record"].AsInt;
        }

        #endregion


    } //end class
}