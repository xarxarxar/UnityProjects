using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MergeBeast;
namespace Tiledom {
    public class TileController : MonoBehaviour {
        [SerializeField] Image icon;
        [SerializeField] GameObject inActiveImage;
        [SerializeField] UIButton btn;


        private RectTransform rtf;
        public int type;
        public int layer;
        public int id;
        public Vector2 originPos;

        public bool hasParent;

        private void Awake() {
            rtf = GetComponent<RectTransform>();

            //btn.onClick.AddListener(() => OnClick());            
        }
        public void SetUp(int id, int type, int layer, Vector2 position) {
            this.originPos = position;
            this.id = id;
            this.type = type;
            this.layer = layer;
            rtf.anchoredPosition = position;
            //inActiveImage.SetActive(layer < 2);
            ChangeTheme();
            icon.SetNativeSize();
            icon.transform.localScale = new Vector2(.75f, .75f);
            inActiveImage.SetActive(false);            
            //Debug.LogError("check parent: " +  delennhau(new Vector2(-400, -400), new Vector2(-400, -300)));
        }

        public void ChangeTheme() {
            icon.sprite = GameManager._Instance.GetSpriteBeast(type);
        }

        public void OnClick() {
            GameManager._Instance.AddSlot(this);
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(EnumDefine.SOUND.TileClick);
        }

        public void CheckParent() {
            if (layer == GameManager._Instance.mapParent.childCount - 1) return;
            if (HasParent()) {
                inActiveImage.SetActive(true);
            } else {
                inActiveImage.SetActive(false);
            }

        }

        //private bool HasParent() {
        //    for (int i = layer + 1; i < GameManager._Instance.mapParent.childCount; i++) {


        //        Transform layerParent = GameManager._Instance.mapParent.GetChild(i);
        //        foreach (Transform otherTile in layerParent) {
        //            float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(transform.position),
        //             Camera.main.ScreenToWorldPoint(otherTile.position));

        //            //float distance = Vector2.Distance(transform.position, otherTile.position);

        //            // Debug.Log(distance);
        //            if (distance <= .8f) {
        //                //if (gameObject.name.Equals("35")) {
        //                //    Debug.LogError(otherTile.name + ", distance: " + distance);
        //                //}
        //                hasParent = true;
        //                return true;
        //            }
        //        }
        //    }
        //    hasParent = false;
        //    return false;
        //}

        private bool HasParent() {
            for(int i = layer + 1; i < GameManager._Instance.mapParent.childCount; i++) {
                Transform layerParent = GameManager._Instance.mapParent.GetChild(i);                
                foreach(RectTransform otherTile in layerParent) {
                    int layerParentIndex = int.Parse(layerParent.name);
                    if (layer % 2 == 0) {
                        //neu minh la layer chan, thi doi voi nhung layer chan, chi can check nhung thang cung vi tri, ko can check xung quanh
                        
                        if (layerParentIndex % 2 == 0) {
                            if (rtf.anchoredPosition == otherTile.anchoredPosition) {
                                return true;
                            }
                        } else {
                            if (CheckParentEvenLayer(rtf.anchoredPosition, otherTile.anchoredPosition, otherTile)) {
                                return true;
                            }
                        }
                    } else {
                        //neu minh la layer le?
                        if(layerParentIndex % 2 != 0) {
                            if (rtf.anchoredPosition == otherTile.anchoredPosition) {
                                return true;
                            }
                        } else {
                            if (CheckParentOddLayer(rtf.anchoredPosition, otherTile.anchoredPosition)) {
                                return true;
                            }
                        }
                        
                    }
                    
                }
            }
            return false;
        }

        private bool CheckParentEvenLayer(Vector2 currentPos, Vector2 otherPos, RectTransform otherTile) {
            int startX = (int)currentPos.x - 100;
            int startY = (int)currentPos.y + 100;
            for(int i =  startX; i <= currentPos.x; i += 100) {
                for(int j = (int)currentPos.y; j <= startY; j += 100) {
                    //if (gameObject.name.Equals("78")) {
                    //    Debug.LogError("current: " + currentPos + ", pos check: " + new Vector2(i, j) + ", other pos: " + otherPos + ", " + otherTile.name);
                    //}
                    if (otherPos == new Vector2(i, j)) {
                        //Debug.LogError("parent: " + otherTile.name);
                        return true; 
                    }
                }
            }
            return false;
        }

        private bool CheckParentOddLayer(Vector2 currentPos, Vector2 otherPos) {
            int startX = (int)currentPos.x;
            int endX = (int)currentPos.x + 100;

            int startY = (int)currentPos.y - 100;
            int endY = (int)currentPos.y;            
            for(int i = startX; i <= endX; i+= 100) {
                for(int j = startY; j <= endY; j += 100) {
                    //if (gameObject.name.Equals("78")) {
                    //    Debug.LogError("current: " + currentPos + ", pos check: " + new Vector2(i, j) + ", other pos: " + otherPos);
                    //}
                    if (otherPos == new Vector2(i, j)) return true;
                }
            }
            return false;
        }

        public int GetTileType() {
            return type;
        }
        public int GetLayer() {
            return layer;
        }
        public Vector2 GetPos() {
            return originPos;
        }
        public int GetId() {
            return id;
        }

        public void MoveToOrigin() {
            transform.localScale = Vector3.one;
            btn.transform.localScale = Vector3.one;
            rtf.DOAnchorPos(originPos, 0.3f);
        }

        public void ChangeType(int type) {
            this.type = type;
            ChangeTheme();
            icon.SetNativeSize();
        }
    } //end class
}