using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MergeBeast {
    public class FreeBoostPopupAnim : MonoBehaviour {
        Action cb = null;
        [SerializeField] FreeBoostCtrl ctl;

        public void SetAction(Action cb) {
            this.cb = cb;
        }

        public void Action() {
            if (cb != null)
                ctl.DelayAction(this.cb);
        }
    } //class 
}