using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Spine.Unity;


namespace MergeBeast
{
    public interface IBookEnemy
    {
        void OnDownStats(int id,Vector3 position);
        void OnUpStats();
        void OnDownHP(int id, Vector3 position);
        void OnUpHP();
        void OnDownSkill(int id, Vector3 position);
        void OnUpSkill();
    }

    public class BookItemEnemy : MonoBehaviour
    {

        public Text TxtName;
        public Image ImgLootbox;

        public Image BtnGift;
        public Image BtnHP;
        public Image BtnSkill;
        public SkeletonGraphic AnimBoss;

        private IBookEnemy _iBook;
        private int _id;

        public void SetEvent(IBookEnemy iBook,int id)
        {
            this._iBook = iBook;
            this._id = id;
            BtnSkill.color = BtnHP.color = BtnGift.color = Color.white;
        }

        public void ClearEvent()
        {
            this._iBook = null;
            BtnSkill.color = BtnHP.color = BtnGift.color = Color.black;
        }

        public void OnClickDownGift()
        {
            _iBook?.OnDownStats(_id,BtnGift.transform.position);
        }

        public void OnUpClickGift()
        {
            _iBook?.OnUpStats();
        }

        public void OnClickDownHP()
        {
            _iBook?.OnDownHP(_id,BtnHP.transform.position);
        }

        public void OnUpHP()
        {
            _iBook?.OnUpHP();
        }

        public void OnDownKill()
        {
            _iBook?.OnDownSkill(_id,BtnSkill.transform.position);
        }

        public void OnUpSkill()
        {
            _iBook?.OnUpSkill();
        }
    }
}
