namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class InstanceManager<T> : MonoBehaviour where T : InstanceManager<T>
    {
        private static volatile T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                }
                return instance;
            }
        }
    }
    
}
