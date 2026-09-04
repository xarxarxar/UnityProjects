using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MergeBeast {
    public class AscendWheelReward : MonoBehaviour {
        [SerializeField] private Image iconReward;
        [SerializeField] private Transform _light1;
        [SerializeField] private Transform _light2;
        [SerializeField] private GameObject panelWheel;

        private void OnEnable() {
            iconReward.transform.localScale = Vector3.zero;
            iconReward.SetNativeSize();
            iconReward.transform.DOScale(2f, 0.3f);            
        }

        private void Update() {

            if (_light1.gameObject.activeInHierarchy) {
                _light1.Rotate(Vector3.forward * 20f * Time.deltaTime);
                _light2.Rotate(Vector3.forward * -20f * Time.deltaTime);
            }

        }

        public void Hide() {
            UIManager.Instance.HidePopup(transform, () => {
                panelWheel.SetActive(false);
                ScreenManager.Instance?.DeActiveScreen();
                
                //this.gameObject.transform.parent.gameObject.SetActive(false);                                         
            });
            
        }
    }
}