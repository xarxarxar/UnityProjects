using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeBeast {
    public class ButtonBoost : MonoBehaviour {
        [SerializeField] private Image lifeTimeImage;
        private float lifeTime;
        private float timeCount;
        private bool allowCountDown = true;
        [SerializeField] private int id;
        [SerializeField] private GameObject _objHandClick;
        private bool _stopTutorial;

        private void Start()
        {
            this.GetComponent<UIButton>()?.onClick.AddListener(() =>
            {
                _stopTutorial = true;
                _objHandClick.SetActive(false);
            });  
        }

        private void Update() {
            if (allowCountDown) {
                timeCount += Time.deltaTime;
                if (timeCount >= lifeTime) {
                    //an vi tri boost neu thoi gian het
                    timeCount = 0f;
                    gameObject.transform.parent.gameObject.SetActive(false);
                    if(id == 1) {
                        PlayerPrefs.SetString(StringDefine.BOOST_FREE_SLOT1, "");
                    } else {
                        PlayerPrefs.SetString(StringDefine.BOOST_FREE_SLOT2, "");
                    }
                }

                lifeTimeImage.fillAmount = 1f - timeCount / lifeTime;
                if(!_stopTutorial)
                    _objHandClick.SetActive(timeCount >= lifeTime / 3f);
            }
        }

        public void SetLifeTime(float time) {
            lifeTime = time;
            timeCount = 0f;
            allowCountDown = true;
            _stopTutorial = false;
        }

        public void SetTimeCount(int time) {
            timeCount = time;
        }

        public void AllowCountDown(bool allow) {
            allowCountDown = allow;
        }
    } //end class
}