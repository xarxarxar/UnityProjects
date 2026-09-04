using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "IAPConfig", menuName = "ConfigData/IAPConfig")]
    public class IAPConfig : ScriptableObject
    {

        public List<IAPPack> Packs;

        public IAPPack GetPackIAP(int id)
        {
            return Packs.Find(x => x.ID == id);
        }

        public IAPPack GetPackIAPByProductId(string productId) {
            return Packs.Find(x => x.PurchaseID.Equals(productId));
        }
    }
}
