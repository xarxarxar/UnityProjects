using UnityEngine;

namespace Watermelon.IAPStore
{
    /// <summary>
    /// ��Ʒ����ʵ������ӿ�
    /// </summary>
    public interface IIAPStoreOffer
    {
        public void Init();
        public GameObject GameObject { get; }
        public float Height { get; }
    }
}