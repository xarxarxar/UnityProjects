using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast {
    public class NewBeastEffect : MonoBehaviour {
        [SerializeField] GameObject mergeObj;
        [SerializeField] GameObject rotateObj;
        [SerializeField] GameObject lighting;
        void OnEnable() {
            mergeObj.SetActive(false);
            rotateObj.SetActive(false);
            lighting.SetActive(true);

        }

        public void ShowMergeObj() {
            mergeObj.SetActive(true);
            rotateObj.SetActive(true);
            lighting.SetActive(false);
        }

        public void ShowRotate() {
            mergeObj.SetActive(false);
            rotateObj.SetActive(true);
        }
    }
}