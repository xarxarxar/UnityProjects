using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Spine.Unity;

namespace MergeBeast
{
    public class BeastItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
    {

        private BeastData _item;
        protected BeastData _myData {
            set
            {
                PlayerPrefs.SetInt($"{MyTag}{MyIndex}", value != null ? value.ID : StringDefine.NULL);
                _item = value;
            }
            get
            {
                return _item;
            }
        }

        protected bool _canMerge;
        protected bool _isDraging;
        protected bool _canDrag;
        protected bool _firstTouch;

       //    [SerializeField] protected Image IconBeast;
        [SerializeField] protected SkeletonGraphic _skelCat;
        [SerializeField] protected Image ImgSlot;
        [SerializeField] protected Text TxtLevel;
        [SerializeField] private Sprite _spriteOpen;
        [SerializeField] private Sprite _spriteLock;
        [SerializeField] private Sprite _spriteHasBeast;
        

        protected UnityEngine.Vector2 Offset;
        protected SkeletonGraphic _skelDrag;

        protected Color COLOR_FADE = new Color(1f, 1f, 1f, 0.6f);
        protected Color COLOR_LIGHT = new Color(1f, 1f, 1f, 1f);
        protected string MyTag;
        protected int MyIndex;
        protected BigInteger _myDps;

        public UIButton _btnUnlock;


        public void OnDrag(PointerEventData eventData)
        {
            this.OnDragBeast(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.OnHandDownBeast(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.OnHandleUpBeast(eventData);
        }

        public virtual void SetBeast(BeastData data)
        {
            // Debug.Log("GHEP BEAST THANH CONG");
            if (data == null) return;
            StartCoroutine(IESetBeast(data));
        }

        private IEnumerator IESetBeast(BeastData data)
        {
            yield return new WaitForEndOfFrame();
            int maxbeast = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST, 0);
            if (data.ID > maxbeast && data.ID > PlayerPrefs.GetInt(StringDefine.HIGHER_LV_BEAST_ASCEND, 0))
            {
                GameManager.Instance.levelBeastDiff = data.ID - maxbeast;

                PlayerPrefs.SetInt(StringDefine.HIGHEST_LEVEL_BEAST, data.ID);
                ScreenManager.Instance?.ActiveScreen(EnumDefine.SCREEN.NEW_BEAST);
            }
            int currentShopBeast = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST);
            if (data.ID > currentShopBeast)
            {
                PlayerPrefs.SetInt(StringDefine.CURRENT_SHOP_BEAST, data.ID);
            }

            this._myData = data;
            this._skelCat.Skeleton.SetSkin(data.Level.ToString("D3"));
            this._skelCat.transform.localScale = Vector3.one * 1.3f;
            this._skelCat.color = COLOR_LIGHT;
            this._canMerge = true;
            this.TxtLevel.text = data.Level.ToString();
            this.ImgSlot.sprite = _spriteHasBeast;
        }

        public BeastData GetData()
        {
            return _myData;
        }

        public bool CanMerge()
        {
            return _canMerge;
        }

        public bool IsDraging()
        {
            return _isDraging;
        }

        public void SetCanMerge(bool merge)
        {
            this._canMerge = merge;
        }

        public UnityEngine.Vector3 GetPositionBeast()
        {
            return _skelCat.transform.position ;
        }

        public UnityEngine.Vector3 GetPositionSlot()
        {
            //return Camera.main.ScreenToWorldPoint(this.transform.position);
            return this.transform.position;
        }


        public void OnUpgradeBeast()
        {
            int rootID = _myData.ID;
            var nextBeast = GameManager.Instance.GetNextBeast(_myData.ID);
            //check xem co dc level merge ko
            int percentLevelMerge = PlayerPrefs.GetInt(StringDefine.LEVEL_LEVEL_MERGE, 0);
            int random = new System.Random().Next(1, 100);
            if(random <= percentLevelMerge) {
                //dc merge them 1 cap
                nextBeast = GameManager.Instance.GetNextBeast(nextBeast.ID);
                UIManager.Instance.ShowNotify("额外1级");
            }
            this.SpawnBeast(nextBeast);

            this._skelCat.transform.localScale = Vector3.zero;
            this.TxtLevel.text = nextBeast.Level.ToString();// string.Empty;
            this.TxtLevel.color = new Color(1f, 1f, 1f, 0f);

            UIManager.Instance.ShowEffectMerge(_skelCat.transform.position + new Vector3(0, 0.3f), rootID, () =>
            {
                this.TxtLevel.color = new Color(1f, 1f, 1f, 1f);
                if (this._myData != null)
                {
                    this._skelCat.transform.localScale = UnityEngine.Vector3.one * 1.3f;
                    PoolManager.Instance.PlayFXBeastMerge(_myData.ID, this.GetPositionSlot());
                }
                SoundManager.Instance?.PlaySound(EnumDefine.SOUND.BEAST_MERGE);
                UIManager.Instance?.AddMoneyMerge();
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.Merge,EnumDefine.Mission.ReachStar);


                
                if (_skelDrag != null) {
                    _skelDrag.transform.localScale = Vector3.zero;
                } else {
                    _skelDrag = GameManager.Instance.GetBeastDrag();
                    _skelDrag.transform.localScale = Vector3.zero;
                }
                GameManager.Instance.GetBeastDrag().transform.localScale = Vector3.zero;
                GameManager.Instance.GetBeastDrag2().transform.localScale = Vector3.zero;
            });
        }

        /// <summary>
        /// khi spawn 1 beast, thì tạm thời ẩn hình ảnh của beast ở slot, khi nào move beast tới thì lại show ra
        /// </summary>
        public void HideBeastIconWhenSpawn() {
            _skelCat.transform.localScale = Vector3.zero;
            Invoke("ShowBeastIconWhenSpawn", 0.3f);
        }

        public void ShowBeastIconWhenSpawn() {
            _skelCat.transform.localScale = Vector3.one * 1.3f;
            this.SetBeast(_myData);
        }

        public virtual void OnActiveSuggestMerge(bool active)
        {
           
        }

        public virtual void OnDragBeast(PointerEventData eventData)
        {
            this._skelDrag.transform.position = (UnityEngine.Vector2)Camera.main.ScreenToWorldPoint(eventData.position) - Offset;
        }

        public virtual void OnHandDownBeast(PointerEventData eventData)
        {
            _canDrag = GameManager.Instance.SomeBeastDraging();
            if (!_canMerge || _canDrag) return;

            this.Offset = Camera.main.ScreenToWorldPoint(eventData.position) - _skelCat.transform.position;
            this._skelDrag = GameManager.Instance.GetBeastDrag();
            this._skelDrag.Skeleton.SetSkin(_myData.Level.ToString("D3"));
            this._skelDrag.transform.localScale = UnityEngine.Vector3.zero;
            this._isDraging = true;
            this._firstTouch = true;
        }

        public virtual void OnHandleUpBeast(PointerEventData eventData)
        {
            
            BeastItem item = this.GetRaycastLine<BeastItem>(eventData.position);

            if (item != null && item.enabled)
            {
                if (item == this)
                {
                    StartCoroutine(IEMove(_skelDrag.transform, _skelCat.transform.position, () =>
                    {
                        this.ReSetDrag();
                    }));
                }
                else
                {
                    if (item.GetData() != null)
                    {
                        if (item.CanMerge()) // Dang trang thai tranning,co the merge
                        {
                            // Merge Cung Loai
                            if (item.GetData().ID == this._myData.ID)
                            {
                                if (item.GetData().ID == (int)Config.MAX_MONSTER) {
                                    //swap
                                    var tmp = item.GetData();
                                    item.SetBeast(_myData);
                                    this.SetBeast(tmp);
                                    this._skelDrag.transform.localScale = Vector3.zero;
                                    this._skelCat.color = this.COLOR_LIGHT;

                                    StartCoroutine(IEScale(_skelCat.transform));
                                } else {
                                    StartCoroutine(IEMove(_skelDrag.transform, item.GetPositionBeast(), () => {
                                        this._skelCat.transform.localScale = Vector3.zero;
                                        this._skelCat.transform.localScale = Vector3.zero;
                                        this._myData = null;
                                        this._canMerge = false;
                                        this.TxtLevel.text = string.Empty;
                                        this.ImgSlot.sprite = _spriteOpen;
                                        item.OnUpgradeBeast();
                                        this.CallBackAfterMove();
                                        
                                    }));
                                }
                            }
                            else // Swap cho
                            {
                                var tmp = item.GetData();
                                item.SetBeast(_myData);
                                this.SetBeast(tmp);
                                this._skelDrag.transform.localScale = Vector3.zero;
                                this._skelCat.color = this.COLOR_LIGHT;

                                StartCoroutine(IEScale(_skelCat.transform ));

                            }
                        }
                        else // Slot nay dang trang thai Battle
                        {
                            StartCoroutine(IEMove(_skelDrag.transform, _skelCat.transform.position, () =>
                            {
                                this.ReSetDrag();
                            }));
                        }
                    }
                    else // Slot ko co chua beast,move toi day
                    {
                        StartCoroutine(IEMove(_skelDrag.transform, item.GetPositionBeast(), () =>
                        {
                            this._skelDrag.transform.localScale = Vector3.zero;
                            this._skelCat.transform.localScale = Vector3.zero;
                            item.SetBeast(_myData);
                            this._myData = null;
                            this.ImgSlot.sprite = _spriteOpen;
                            _canMerge = false;
                            this.TxtLevel.text = string.Empty;
                            this.CallBackAfterMove();
                        }));
                    }
                }                

            }
            else // Ko co item nao tai cho tha tay
            {
                StartCoroutine(IEMove(_skelDrag.transform, _skelCat.transform.position, () =>
                {
                    this.ReSetDrag();
                }));
            }            
        }
        public virtual void SpawnBeast(BeastData data) { }

        protected T GetRaycastLine<T>(Vector2 position) where T : BeastItem
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = position;
            List<RaycastResult> list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, list);
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T component = list[i].gameObject.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return null;
        }

        protected IEnumerator IEMove(Transform root,Vector3 target,UnityAction callback = null)
        {
            while((target - root.position).magnitude > 5f)
            {
                   root.position = Vector3.Lerp(root.position, target, 20f * Time.deltaTime);
                yield return null;
            }

            root.position = target;
            callback?.Invoke();
        }

        protected IEnumerator IEScale(Transform root,UnityAction callback = null)
        {
            root.localScale = Vector3.one * 1.7f;
            root.transform.DOScale(Vector3.one, 0.25f);
            yield return new WaitForSeconds(0.25f);
            root.transform.localScale = Vector3.one * 1.3f;
            callback?.Invoke();
        }

        public virtual void AddEvtAttack(UnityAction<BigInteger,int> action) { }
        public BigInteger MyDPS() { return _myDps; }

        public virtual BigInteger RootDPS() { return 0; }

        protected virtual void ReSetDrag()
        {
            _skelDrag.transform.localScale = Vector3.zero;
            _skelCat.transform.localScale = Vector3.one * 1.3f;
            _skelCat.color = COLOR_LIGHT;
        }

        protected virtual void CallBackAfterMove() { }

        public virtual void SetEmptySlot()
        {
            this._myData = null;
            this._canMerge = false;
            this._skelCat.transform.localScale = Vector3.zero;
            this.ImgSlot.sprite = _spriteOpen;
            TxtLevel.text = "";
        }

        public void SetStateSlot(bool open,int index)
        {
            ImgSlot.sprite = open ? _spriteOpen : _spriteLock;
            ImgSlot.SetNativeSize();
            this.enabled = open;
            this.TxtLevel.text = string.Empty;
            this._skelCat.transform.localScale = Vector3.zero;
            this.MyIndex = index;
        }

        public virtual void OnAscendBeast()
        {
            this.SetEmptySlot();
        }

    }
}
