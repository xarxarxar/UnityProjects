namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class AIProperties : MonoBehaviour
    {
    
        private GameObject factoryProduct;
        public GameObject FactoryProduct
        {
            get { return factoryProduct; }
            set { factoryProduct = value; }
        }
        public abstract void SpawnAI(BaggageType baggageType, Vector3 spawnPosition, Quaternion Rotation, params object[] data);
    }
    
}
