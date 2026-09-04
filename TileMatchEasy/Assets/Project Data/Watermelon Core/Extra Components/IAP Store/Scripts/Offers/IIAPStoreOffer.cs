using UnityEngine;

namespace Watermelon.IAPStore
{
    /// <summary>
    /// 商品必须实现这个接口
    /// </summary>
    public interface IIAPStoreOffer
    {
        public void Init();
        public GameObject GameObject { get; }
        public float Height { get; }
    }
}