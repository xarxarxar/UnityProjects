using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace MergeBeast
{
    public class BeastTraining : BeastItem
    {
        [SerializeField] private GameObject ObjSuggestMerge;

        private List<string> _listCatAnim;

        // Use this for initialization
        void Awake()
        {
            MyTag = StringDefine.BEAST_TRANING_INDEX;
            _listCatAnim = new List<string>()
            {
                "Minigame_Belly_Rub","Minigame_Belly_Rub_Pre","Minigame_Brush_3","Sit_Bottom","Sit_Knead","Sit_Lazy","Sit_Lick_Hand","Sit_Lick_Hand_Tick","Sit_Lick_Leg"
            };

            StartCoroutine(IECatAction());
        }

        public override void OnHandDownBeast(PointerEventData eventData)
        {

            _canDrag = GameManager.Instance.SomeBeastDraging();
            if (!_canMerge || _canDrag) return;

            base.OnHandDownBeast(eventData);
        }


        public override void OnDragBeast(PointerEventData eventData)
        {
            if (!_canMerge || _canDrag) return;

            base.OnDragBeast(eventData);

            if (_firstTouch)
            {
                _skelCat.color = base.COLOR_FADE;
                _skelDrag.transform.localScale = Vector3.one * 1.3f;
                GameManager.Instance.OnSuggestMerge(this);
                _firstTouch = false;
            }
        }


        public override void OnHandleUpBeast(PointerEventData eventData)
        {
            base._isDraging = false;
            if (_canDrag) return;
            if (!_canMerge) return;

            GameManager.Instance.OnDeActiveSuggestMerge();
            base.OnHandleUpBeast(eventData);
        }

        public override void SpawnBeast(BeastData data)
        {
            base._myData = data;
        //    base.SetBeast(data);
            //Khi spawn beast thi tam thoi an icon di
            base.HideBeastIconWhenSpawn();
        }


        public override void OnActiveSuggestMerge(bool active)
        {
            this.ObjSuggestMerge.SetActive(active);
        }

        public override void SetEmptySlot()
        {
            base.SetEmptySlot();
        }

        public void ShowTipUnlock()
        {
            if (PlayerPrefs.GetInt($"{StringDefine.TRANING_SLOT}{MyIndex - 1}") == StringDefine.NULL)
            {
                UIManager.Instance.ShowTipUnlockSlot(MyIndex < 15, MyIndex, Camera.main.ScreenToWorldPoint(GetPositionSlot()));
            }
        }

        public void HideTipUnlock()
        {
            UIManager.Instance?.HideTipUnlockSlot();
        }

        private IEnumerator IECatAction()
        {
            yield return new WaitForEndOfFrame();
            int count = 0;
            int loop = Random.Range(0, 4);
            var anim = _skelCat.AnimationState.SetAnimation(0, _listCatAnim[Random.Range(0, _listCatAnim.Count)], true);
            anim.Complete += (t) =>
            {
                count++;
                if(count >= loop)
                    _skelCat.AnimationState.SetAnimation(0, "Sit_Idle", true);
            };

            yield return new WaitForSeconds(Random.Range(7.5f, 15.5f));
            StartCoroutine(IECatAction());
        }
    }
}
