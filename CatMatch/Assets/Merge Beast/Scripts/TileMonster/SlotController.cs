using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MergeBeast;

namespace Tiledom {
    public class SlotController : MonoBehaviour {

        private int maxSlot = 7;
        private int currentIndex = 0;
        public Transform slotParent;

        public List<TileMove> listMatch = new List<TileMove>();
        public List<TileMove> listTileMove = new List<TileMove>();
        public List<TileMove> listSequence = new List<TileMove>();

        private void OnEnable() {
            BroadCastReceiver.Register(StringDefine.MOVE_DONE, () => {
                Check();
            }, this);
        }

        private void OnDisable() {
            BroadCastReceiver.UnRegister(StringDefine.MOVE_DONE);
        }


        //private void Update() {            
        //    if(ShouldCheck()) {
        //        foreach(var item in listMatch) {
        //            item.isMatch = true;
        //        }
        //        listMatch.Clear();
        //    }
        //}

        /// <summary>
        /// Tạo ra 1 slot ảo và add vào list đồng thời ẩn tile thật đi
        /// </summary>
        /// <param name="tile"></param>
        public void AddSlot(TileController tile) {
            if (listTileMove.Count >= 7) return;
            //tao ra 1 tile ao? trung voi vi tru cua tile click
            GameObject tileMove = Instantiate(GameManager._Instance.tileMovePrefab, transform);
            TileMove tileMoveScript = tileMove.GetComponent<TileMove>();
            tileMove.transform.parent = slotParent;
            tileMove.transform.position = tile.transform.position;
            tileMoveScript.SetUp(tile.GetId(), tile.GetTileType(), tile.GetLayer());


            //an tile that di
            tile.transform.parent = GameManager._Instance.trashParent;
            tile.gameObject.SetActive(false);
            GameManager._Instance.RemoveTileFromList(tile);

            //move tile 
            //kiem tra xem co type nay chua, neu chua co thi move binh thuong, neu co roi thi move den vi tri cung type
            int sameTypeIndex = GetSameTypeIndex(tileMoveScript);
            if (sameTypeIndex == -1) {
                //khong co tile trung nhau, move ve ben phai
                tileMoveScript.SetIndex(currentIndex);
                listTileMove.Add(tileMoveScript);
            } else {
                int finalIndex = sameTypeIndex + 1;
                //co tile trung, move den vi tri sat' ben canh
                tileMoveScript.SetIndex(finalIndex);
                listTileMove.Insert(finalIndex, tileMoveScript);

                //move nhung tile phai sang 1 don vi
                for (int i = finalIndex + 1; i < listTileMove.Count; i++) {
                    TileMove script = listTileMove[i];
                    int prevIndex = script.GetIndex();
                    listTileMove[i].SetIndex(prevIndex + 1);
                }
            }
            listSequence.Add(tileMoveScript);

            currentIndex++;

        }

        public void Reset() {
            listSequence.Clear();
            listTileMove.Clear();
            currentIndex = 0;
        }

        private bool ShouldCheck() {
            if (listMatch.Count < 3) return false;
            foreach (var item in listMatch) {
                if (item.shouldMatch) {
                    if (item.canMove) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sau khi slot ảo move tới vị trí khay thì Check match
        /// </summary>
        private void Check() {
            int countMatchTile = 0;
            if (listTileMove.Count == 0) return;

            int typeCheck = listTileMove[0].GetTileType();
            int endOfThree = 0; //vi tri cuoi cung trong 3 vi tri match
            for (int i = 0; i < listTileMove.Count; i++) {
                TileMove tileMove = listTileMove[i];
                if (tileMove.GetTileType() == typeCheck) {
                    countMatchTile++;
                    if (countMatchTile == 3) {
                        endOfThree = i;
                        break;
                    }
                } else {
                    countMatchTile = 1;
                    typeCheck = tileMove.GetTileType();
                }
            }

            //xoa tile trung nhau
            if (countMatchTile == 3) {
                BroadCastReceiver.Broadcast(StringDefine.MATCH_TILE);
                listMatch.Clear();
                //tao ra list chua cac tile trung nhau
                //listMatch = new List<TileMove>();
                int firstOfThree = endOfThree - 2; //tinh vi tri dau tien cua danh sach cac tile trung nhau

                //add cac tile vao list
                for (int i = firstOfThree; i <= endOfThree; i++) {
                    listMatch.Add(listTileMove[i]);
                }

                //remove cac tile nay khoi list ban dau
                for (int i = 0; i < 3; i++) {
                    TileMove t = listTileMove[firstOfThree];
                    listTileMove.RemoveAt(firstOfThree);
                    listSequence.Remove(t);
                }

                //gan lai vi tri currentIndex de move nhung tile sau
                currentIndex = listTileMove.Count;


                //destroy nhung tile trung nhau
                for (int i = 0; i < listMatch.Count; i++) {
                    listMatch[i].isMatch = true;
                    //listMatch[i].SetIndex(-1);

                }
                Sapxep(endOfThree, 3);


            }


            //check game over
            if (listTileMove.Count >= 7) {
                bool hasMatch = false;
                foreach (var item in listTileMove) {
                    if (item.shouldMatch) hasMatch = true;
                }

                if (!hasMatch) {
                    GameManager._Instance.GameOver();
                }
            }

            

        }

        private void Sapxep(int last, int step) {
            for (int i = 0; i < listTileMove.Count; i++) {
                int prevIndex = listTileMove[i].GetIndex();
                if (prevIndex > last) {
                    listTileMove[i].SetIndex(prevIndex - step);
                }
            }
        }

        /// <summary>
        /// Tìm vị trí lớn nhất của các tile có trùng type
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Nếu = -1 thì ko có type đó trong list</returns>
        private int GetSameTypeIndex(TileMove tile) {
            int index = -1;
            for (int i = 0; i < listTileMove.Count; i++) {
                TileMove t = listTileMove[i];
                if (t.GetTileType() == tile.GetTileType() && t != tile) {
                    index = i;
                }
            }
            return index;
        }

        public TileMove GetLastTileMove() {
            if (listSequence.Count == 0) return null;
            return listSequence[listSequence.Count - 1];
        }

        public void RemoveLastTile() {
            TileMove last = GetLastTileMove();
            if(last != null) {
                if(listTileMove.Contains(last))
                    listTileMove.Remove(last);
                if(listSequence.Contains(last))
                    listSequence.Remove(last);
                Destroy(last.gameObject);
                currentIndex--;

                //sap xep lai danh sach
                Sapxep(last.GetIndex(), 1);
            }


        }

        /// <summary>
        /// Đêm xem trong danh sách có bao nhiêu type
        /// </summary>
        /// <param name="type">Type cần check</param>
        /// <returns></returns>
        public int CountType(int type) {
            int count = 0;
            foreach (var item in listTileMove) {
                if(item.GetTileType() == type) {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Kiểm tra trong list xem có 2 type trùng nhau ko
        /// </summary>
        /// <returns></returns>
        public TileMove CountTwoType() {
            int count = 1;
            for(int i = 1; i < listTileMove.Count; i++) {
                TileMove tileMove = listTileMove[i];
                if(listTileMove[i].GetTileType() == listTileMove[i-1].GetTileType()) {
                    count++;
                    if(count >= 2) {
                        return tileMove;
                    }
                }
            }
            return null;
        }
    } //end class
}