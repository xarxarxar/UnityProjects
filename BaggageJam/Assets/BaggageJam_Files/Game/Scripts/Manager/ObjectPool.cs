namespace EKStudio
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using System;
    
    /// <summary>
    /// Object Pool class. 
    /// It adds the generated objects to a pool and then pulls the destroyed objects back into that pool. 
    /// If it needs to be recreated from this object later, it opens and uses the closed object in the pool.
    /// </summary>
    
    public enum PoolType
    {
        None,
        Gameobject,
        ParticleSystem
    }
    
    public class ObjectPool : MonoBehaviour
    {
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
        public static PoolType PoolTypeEnum;
    
        private GameObject _objectPoolEmptyHolder;
    
        private static GameObject _gameobjectsEmpty;
        private static GameObject _particleSystemEmpty;
    
    
        void Awake()
        {
            SetupEmpties();
        }
    
        //Create gameobjects in scene to keep pooled objects
        private void SetupEmpties()
        {
            _objectPoolEmptyHolder = new GameObject("Pooled Objects");
    
            _particleSystemEmpty = new GameObject("Particle Effects");
            _particleSystemEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    
            _gameobjectsEmpty = new GameObject("Gameobjects");
            _gameobjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        }
    
        public static GameObject SpawnObjects(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
        {
            //Check if there is a pool of spawned object
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);
    
            if (pool == null)
            {
                //If there is no pool, create one
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }
    
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();
    
            if (spawnableObj == null)
            {
                //Set parent
                GameObject parentObject = SetParentObjects(poolType);
    
                //If there is no inactive objects, spawn new one
                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
    
                if (parentObject != null)
                {
                    spawnableObj.transform.SetParent(parentObject.transform);
                }
            }
            else
            {
                //If there is an inactive object, reactive it
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }
    
            return spawnableObj;
        }
    
        //Overload of SpawnObjects
        public static GameObject SpawnObjects(GameObject objectToSpawn, Transform parentTransform)
        {
            //Check if there is a pool of spawned object
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);
    
            if (pool == null)
            {
                //If there is no pool, create one
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }
    
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();
    
            if (spawnableObj == null)
            {
                //If there is no inactive objects, spawn new one
                spawnableObj = Instantiate(objectToSpawn, parentTransform);
            }
            else
            {
                //If there is an inactive object, reactive it
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }
    
            return spawnableObj;
        }
    
        //When an objects destroy, return it to the pool
        public static void ReturnObjectToPool(GameObject obj)
        {
            string goName = obj.name.Substring(0, obj.name.Length - 7); //Cut the (Clone) part from name
    
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);
    
            if (pool == null)
            {
                Debug.LogWarning("There is something wrong about pool! " + obj.name);
            }
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }
    
        //You can make the spawn objects childs of the relevant place
        private static GameObject SetParentObjects(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.ParticleSystem:
                    return _particleSystemEmpty;
    
                case PoolType.Gameobject:
                    return _gameobjectsEmpty;
    
                case PoolType.None:
                    return null;
    
                default:
                    return null;
            }
        }
    }
    
    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
    }
    
}
