using System;
using System.Collections;
using System.Collections.Generic;
using Observer;
using UnityEngine;
using UnityEngine.UI;

namespace MergeBeast
{
    public class SelectBattleController : BaseScreen
    {
        [SerializeField] Button btnBack;
        public Image redDotIcon;

        private void Start()
        {
            btnBack.onClick.AddListener(() => ScreenManager.Instance.DeActiveScreen());

            if (redDotIcon != null) redDotIcon.gameObject.SetActive(RewardHelper.CheckHasChapterRewards());
        }

        private void Awake()
        {
            this.RegisterListener(EventID.BackFromMap, (sender, param) => BackToMap());
        }

        private void BackToMap()
        {
            if (!gameObject.activeSelf) return;
            if (redDotIcon != null) redDotIcon.gameObject.SetActive(RewardHelper.CheckHasChapterRewards());
        }
    }
}