using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MergeBeast;
using DG.Tweening;

namespace Tiledom {

    public class PrizeController : MonoBehaviour {
        public static PrizeController Instance;
        private void Awake() {
            if (Instance == null) {
                Instance = this;

                foreach(Transform child in pieceParent) {
                    listPiece.Add(child.gameObject);
                }
            }


        }
        [SerializeField] private Image pieceFly;
        [SerializeField] private Transform pieceParent;
        [SerializeField] private Button btn;
        [SerializeField] private Animator animator;
        [SerializeField] private CollectPuzzle popupCollectPuzzle;

        private List<GameObject> listPiece = new List<GameObject>();

        private int totalPiece;
        public bool canMove = false;
        public float speed;

        private void OnEnable() {
            totalPiece = PlayerPrefs.GetInt(StringDefine.TOTAL_PIECE, 0);
            ShowPiece();

        }


        private void Update() {
            if(canMove) {                
                pieceFly.transform.position = Vector2.MoveTowards(pieceFly.transform.position, pieceParent.transform.position, speed);

                if (Vector2.Distance(pieceFly.transform.position, pieceParent.transform.position) <= 0.2f) {
                    canMove = false;
                    pieceFly.transform.position = pieceParent.transform.position;
                    pieceFly.gameObject.SetActive(false);
                    ShowPiece();
                }
            }
        }

        void ShowFullPiece() {
            foreach(var item in listPiece) {
                item.SetActive(true);
            }
        }       

        public void CollectPiece(Vector2 startPos) {
            pieceFly.gameObject.SetActive(true);
            pieceFly.transform.position = startPos;
            pieceFly.sprite = GetSpriteByIndex();
            canMove = true;

            totalPiece++;
            PlayerPrefs.SetInt(StringDefine.TOTAL_PIECE, totalPiece);
        }

        void ShowPiece() {
            foreach (var item in listPiece) {
                item.SetActive(false);
            }

            if (totalPiece > 0) {
                int mod = totalPiece % 9;
                if (totalPiece >= 9) {
                    ShowFullPiece();
                    btn.interactable = true;
                    animator.SetBool("scale", true);
                } else {
                    btn.interactable = false;
                    animator.SetBool("scale", false);
                    for (int i = 0; i < listPiece.Count; i++) {
                        if (i < mod) {
                            listPiece[i].SetActive(true);
                        }
                    }
                }

            }
        }


        private Sprite GetSpriteByIndex() {
            int index = totalPiece % 9;
            return listPiece[index].GetComponent<Image>().sprite;
        }

        public void CollectItem() {
            btn.interactable = false;
            animator.SetBool("scale", false);
            totalPiece -= 9;
            PlayerPrefs.SetInt(StringDefine.TOTAL_PIECE, totalPiece);

            int currentPuzzle = PlayerPrefs.GetInt(StringDefine.PUZZLE_INDEX, 0);
            currentPuzzle++;
            PlayerPrefs.SetInt(StringDefine.PUZZLE_INDEX, currentPuzzle);

            btn.transform.DOShakePosition(2f, 10f, 30).OnComplete(() => {
                popupCollectPuzzle.gameObject.SetActive(true);

                //check lai cac manh ghep
                ShowPiece();
            });

            

            

        }


    }
}