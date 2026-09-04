using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MergeBeast;

namespace Tiledom
{
    public class Header : MonoBehaviour
    {
        public static Header instance;
        public Slider timerSlider;
        private float timeCount;
        public bool allowCount = false;
        private float totalTime = 300f;
        
        [SerializeField] Button btnPause;

        [Header("Reference Script")]
        [SerializeField] TimeUp timeUpController;
        [SerializeField] PauseController pauseController;

        private float totalTimeUse = 0f;
        

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        void Start()
        {
            timerSlider.minValue = 0;
            timerSlider.maxValue = 1;
            timerSlider.value = 1;
            timeCount = totalTime;

            totalTimeUse = totalTime;
            
            btnPause.onClick.AddListener(() => Pause());

        }

        public void ResetTime() {            
            timeCount = totalTime;
            timerSlider.value = 1;
        }

        public void SetTime(int time) {            
            timeCount = time;
        }

        public void ShowLevel()
        {
            LevelController.instance.Show();
        }

        private void Pause() {
            //tranh truong hop chua xuat hien popup ma nguoi choi click vao nut pause sau do se gay ra loi
            if (GameManager._Instance.listAllTile.Count == 0) return;
            allowCount = false;
            pauseController.gameObject.SetActive(true);
        }

        public void Resume() {
            allowCount = true;
            pauseController.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (allowCount)
            {                
                timeCount -= Time.deltaTime;
                timerSlider.value = timeCount / totalTime;
                if (timeCount <= 0)
                {
                    //game over
                    allowCount = false;
                    TimeUp();
                }
            }



        }

        void TimeUp()
        {
            timeUpController.gameObject.SetActive(true);
        }

        public void BuyTime()
        {
            timeCount += 60f;
            totalTimeUse += 60f;
            allowCount = true;
        }

        public int GetTimeCount() {
            return Mathf.RoundToInt(timeCount);
        }

        public int GetTimeRemain() {
            return Mathf.RoundToInt(timeCount);
        }

        public int GetTotalTimeUse() {
            return Mathf.RoundToInt(totalTimeUse);
        }

        public int GetTimeFinish() {
            return Mathf.RoundToInt(totalTimeUse - timeCount);
        }
    }
}