using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using MergeBeast;
using System.IO;

namespace Tiledom
{
    public class ConfirmQuitController : MonoBehaviour
    {
        [SerializeField] Button btnYes;
        [SerializeField] Button btnNo;

        private void Start()
        {
            btnYes.onClick.AddListener(() => Yes());
            btnNo.onClick.AddListener(() => No());
        }

        private void No()
        {
            //xoa data trong continue
            GameManager._Instance.ClearLevelData();

            Header.instance.ShowLevel();
            gameObject.SetActive(false);
        }

        private void Yes()
        {
            if (GameManager._Instance.GetProgress() >= 50) {
                if (CPlayer.currentLevel + 1 > CPlayer.levelUnlock) {
                    CPlayer.levelUnlock += 1;
                    CPlayer.SaveUnlock();
                }
            }
            GameManager._Instance.SaveDataToContinue();

            Header.instance.ShowLevel();
            gameObject.SetActive(false);
        }
    }
}
