using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeBeast;

namespace Tiledom {
    public class TileMove : MonoBehaviour {


        [SerializeField] Image icon;

        public bool isMatch = false;
        public bool shouldMatch = false;

        public int type;
        private int layer;
        private int id;
        public Vector2 finalPos;
        public int index = -1;
        private float speed;
        public bool canMove = true;
        private bool callEvent = false;
        private Vector3 currentScale = Vector3.one;

        private RectTransform rtf;

        private void Awake() {
            rtf = GetComponent<RectTransform>();
        }
        public void SetUp(int id, int type, int layer) {
            this.id = id;
            this.type = type;
            this.layer = layer;
            icon.sprite = GameManager._Instance.GetSpriteBeast(type);
            icon.SetNativeSize();
            icon.transform.localScale = new Vector2(0.9f, 0.9f);
        }

        private void Update() {
            if (index >= 0 && canMove) {
                //transform.localPosition = Vector2.MoveTowards(transform.localPosition, finalPos, Time.deltaTime * 100f);
                rtf.anchoredPosition = Vector2.MoveTowards(rtf.anchoredPosition, finalPos, speed);
                if(Vector2.Distance(rtf.anchoredPosition, finalPos) < 2) {
                    rtf.anchoredPosition = finalPos;
                    
                    if (callEvent) {
                        //khi move tu tren xuong moi goi event, move thu tu trong hang ko goi event
                        BroadCastReceiver.Broadcast(StringDefine.MOVE_DONE);
                        callEvent = false;
                    }

                    //canMove = false;
                }
            }


            if(isMatch) {                
                shouldMatch = false;
                if(currentScale.x < 0.5f) {
                    isMatch = false;                    
                    Destroy(gameObject);
                }
                float delta = Time.deltaTime * 4f;
                currentScale -= new Vector3(delta, delta, delta);
                transform.localScale = currentScale;

            }
        }


        public void SetIndex(int index) {
            this.index = index;
            if (index >= 0) {                
                canMove = true;
                finalPos = new Vector2(-300f + 100 * index, 0);

                float distance = Vector2.Distance(rtf.anchoredPosition, finalPos);                

                speed = SetSpeed(distance, finalPos);
            }
        }

        private float SetSpeed(float distance, Vector2 finalPos) {
            float speed = 10f;

            if (rtf.anchoredPosition.y == finalPos.y) {
                //move trong row
                speed = 10f;
                callEvent = false;
            } else {
                //move den khay
                speed = 40f;
                callEvent = true;
            }
                    
            return speed;
        }

        public Vector3 GetFinalPos() {
            return finalPos;
        }

        public int GetTileType() {
            return type;
        }
        public int GetIndex() {
            return index;
        }

        public int GetLayer() {
            return layer;
        }
        public int GetId() {
            return id;
        }
    }
}