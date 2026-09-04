using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MergeBeast;
using Observer;

namespace Tiledom {
    public class CollectPuzzle : MonoBehaviour {
        [SerializeField] Image icon;
        [SerializeField] Text txtPrizeName;
        [SerializeField] Sprite[] arrSpr;

        private void OnEnable() {
            int puzzleIndex = PlayerPrefs.GetInt(StringDefine.PUZZLE_INDEX, 0);
            if (puzzleIndex > 3) return;
            icon.sprite = arrSpr[puzzleIndex - 1];
            icon.transform.localScale = Vector3.one;
            icon.transform.DOScale(3, 0.5f);
            txtPrizeName.text = GetText(puzzleIndex);

            GetItem(puzzleIndex);

        }

        string GetText(int index) {
            if(index == 1) {
                return "宝石×50";
            } else if (index == 2) {
                return "时间跳跃2小时×1";
            } else if (index == 3) {
                return "复活×10";
            }
            return "";
        }

        void GetItem(int index) {
            if(index == 1) {
                Utils.AddRubyCoin(50);
                this.PostEvent(EventID.OnUpDateMoney);
            } else if (index == 2) {
                MergeBeast.GameManager.Instance.AddDailyBoost(EnumDefine.DailyBoost.TIME_JUMP_2H, 1);
            } else if (index == 3) {
                CPlayer.AddRevival(10);
            }
        }


    }
}
