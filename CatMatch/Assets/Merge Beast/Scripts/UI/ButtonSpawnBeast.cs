using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using DG.Tweening;
using Spine;
using System;
using Spine.Unity;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn打开 http://web3incubators.com/
//电报https://t.me/gamecode999
namespace MergeBeast {
    public class ButtonSpawnBeast : MonoBehaviour {
        private SkeletonGraphic _skeleton;
        [SerializeField] private Text priceText;
        [SerializeField] private SkeletonGraphic mainImage;
        [SerializeField] private SkeletonGraphic moveImage;

        Action cb;
        Vector2 beastPos;
        private Queue<SkeletonGraphic> _listMoveCat;

        // Start is called before the first frame update
        private void Awake() {
            moveImage.gameObject.SetActive(false);
        }

        private void Start()
        {
            _listMoveCat = new Queue<SkeletonGraphic>();
            for(int i = 0; i < 10; i++)
            {
                SkeletonGraphic skel = Instantiate(moveImage, moveImage.transform.parent);
                _listMoveCat.Enqueue(skel);
            }
        }


        public void BreakEgg(Vector2 pos)
        {
            MoveBeatToSlot(pos);
        }



        public void EnqueueEgg(EggController egg) {
           
        }

        public void BreakEgg(Action cb, Vector2 beastPos) {
            this.cb = cb;
            this.beastPos = beastPos;
            TrackEntry entry = _skeleton.AnimationState.SetAnimation(0, "expl2", false);
            entry.TimeScale = 2f;
            entry.Complete += BreakComplete;
        }

        private void BreakComplete(TrackEntry trackEntry) {
            StartCoroutine(_BreakComplete());
        }

        IEnumerator _BreakComplete() {
            Idle();
            //MoveBeatToSlot(beastPos);
            yield return new WaitForSeconds(0.05f);
            if (cb != null) cb();

        }

        public void Idle() {
            //isReady = true;
            TrackEntry entry = _skeleton.AnimationState.SetAnimation(0, "idle", true);
        }

        public void SetActive(bool active) {
            if (active) {
                priceText.color = Color.white;
            } else {
                priceText.DOColor(Color.red, 0.2f).SetLoops(3);
            }
        }


        public void MoveBeatToSlot(Vector2 pos) {
            if (moveImage == null) return;

            mainImage.gameObject.SetActive(false);

            if(_listMoveCat.Count > 0)
            {
                SkeletonGraphic skel = _listMoveCat.Dequeue();
                skel.gameObject.SetActive(true);
                skel.Skeleton.SetSkin(mainImage.Skeleton.Skin.Name);

                skel.gameObject.transform.DOMove(pos, 0.3f).OnComplete(() => {
                    skel.gameObject.SetActive(false);
                    skel.transform.position = mainImage.transform.position;
                    mainImage.gameObject.SetActive(true);
                    _listMoveCat.Enqueue(skel);
                });
            }
        }

    } //end class
}
