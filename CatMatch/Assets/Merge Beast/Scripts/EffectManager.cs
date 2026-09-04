using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MergeBeast {
    public class EffectManager : MonoBehaviour {
        public static EffectManager Instance;
       
        [SerializeField] Transform gemPosition;
        [SerializeField] Transform soulPosition;

        [Header("Prefab")]
        [SerializeField] GameObject gemRewardPrefab;

        private void Awake() {
            if(Instance == null) {
                Instance = this;
            }
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.R)) {
                PoolManager.Instance.PlayFXEnemyDead(new UnityEngine.Vector3(-0.25f, 2.2f, -1f));
            }
        }

        public void CollectGem(Vector2 startPos, float timeShow = 0.5f, float timeMove = 0.5f) {
            gemRewardPrefab.SetActive(true);
            gemRewardPrefab.transform.position = startPos;
            gemRewardPrefab.GetComponent<GetGemEffect>().Move(gemPosition.position, timeShow, timeMove);
        }
        
    }
}