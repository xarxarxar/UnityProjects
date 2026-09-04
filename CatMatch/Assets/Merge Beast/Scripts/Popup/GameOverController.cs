using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MergeBeast {

    public class GameOverController : MonoBehaviour {
        [SerializeField] private UIButton btnClose;

        private void Awake() {
            btnClose.onClick.AddListener(() => Close());
        }

        private void Close() {
            UIManager.Instance.HidePopup(transform, () => {
                SceneManager.LoadScene(2);
            });
        }
    }
}