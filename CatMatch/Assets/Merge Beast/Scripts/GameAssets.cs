using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast {
    public class GameAssets : MonoBehaviour {
        public static GameAssets Instance;

        public List<VipConfig> listVipConfig;
        public GameObject vipBenefitRowPrf;
        

        private void Awake() {
            if(Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        

        
    }
}