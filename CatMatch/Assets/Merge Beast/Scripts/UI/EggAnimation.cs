using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace MergeBeast {
    public class EggAnimation : MonoBehaviour {
        private SkeletonGraphic _skeleton;
        [SerializeField] private ButtonSpawnBeast btnSpawnScript;

        string[] listAnimation = { "expl", "expl2" };

        private void Awake() {
            _skeleton = GetComponent<SkeletonGraphic>();
        }

        private void OnEnable() {
            Idle();
        }

        public void Break() {
            int random = Random.Range(0, listAnimation.Length);
            TrackEntry entry = _skeleton.AnimationState.SetAnimation(0, listAnimation[random], false);            
            entry.Complete += BreakComplete;
        }

        private void BreakComplete(TrackEntry trackEntry) {

            //btnSpawnScript.EnqueueEgg(this);
            //gameObject.SetActive(false);
        }

        public void Idle() {
            _skeleton.AnimationState.SetAnimation(0, "idle", true);
        }
    } //end class
}