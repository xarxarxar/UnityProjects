using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast {
    public class EggController : MonoBehaviour {

        List<EggSlice> listEggSlice = new List<EggSlice>();
        private Animator anim;

        private void Awake() {
            anim = GetComponent<Animator>();
            for (int i = 0; i < transform.childCount; i++) {
                listEggSlice.Add(transform.GetChild(i).GetComponent<EggSlice>());
            }
        }

        public void Break() {
            //anim.SetTrigger("expl");
            for(int i = 0; i < listEggSlice.Count; i++) {
                listEggSlice[i].Break();
            }
            StartCoroutine(_Destroy());
        }

        public void BackToNormal() {
            for(int i = 0; i < listEggSlice.Count; i++) {
                listEggSlice[i].BackToNormal();
            }
        }

        IEnumerator _Destroy() {
            yield return new WaitForSeconds(1f);
            BackToNormal();
            UIManager.Instance.btnSpawnScriptAnimation.EnqueueEgg(this);
            gameObject.SetActive(false);
        }
    } //end class
}