namespace EKStudio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using Dreamteck.Splines;
    using NaughtyAttributes;
    using UnityEngine;
    using Random = UnityEngine.Random;
    
    public class ConveyorBelt : InstanceManager<ConveyorBelt>
    {
        [SerializeField] private int totalConveyorCount;
        [SerializeField] private GameObject conveyorPrefab;
        [SerializeField] private Transform conveyorParent;
    
        public List<GameObject> allConveyor;
        public List<GameObject> allRampBaggage;
    
    
        [SerializeField] private SplineComputer rampSpline;
        [SerializeField] private SplineComputer conveyorSpline;
    
    
        public void SpawnBaggage()
        {
            if (!CanSpawnBaggage())
                return;

            BaggageType baggageType = LevelManager.Instance.baggageList[0];
            LevelManager.Instance.baggageList.RemoveAt(0);

            BaggageFactory.SpawnBaggage(baggageType, transform.position, Quaternion.Euler(90, 90, 0), out GameObject factoryProduct);
    
            factoryProduct.GetComponent<SplineFollower>().spline = rampSpline;
            factoryProduct.GetComponent<BaggageRay>().speed = Time.timeScale == 30 ? 10 : 2;
    
            allRampBaggage.Add(factoryProduct);
        }
    
        [Button]
        void RegenerateConveyor()
        {
            for (int i = 0; i < totalConveyorCount; i++)
            {
                GameObject conveyor = Instantiate(conveyorPrefab, conveyorParent);
    
                allConveyor.Add(conveyor);
    
                SplineFollower conveyorSplineFollower = conveyor.GetComponent<SplineFollower>();
    
                conveyorSplineFollower.spline = conveyorSpline;
    
                conveyorSplineFollower.SetDistance(conveyorSplineFollower.CalculateLength() / (totalConveyorCount / (i + 1f)));
            }
        }
    
        [Button]
        void ResetConveyor()
        {
            foreach (var item in allConveyor)
            {
                DestroyImmediate(item);
            }
    
            allConveyor.Clear();
        }
    
        public bool CanSpawnBaggage()
        {
            if (LevelManager.Instance.baggageList.Count <= 0) return false;

            if (allRampBaggage.Count == 1) return false;
    
            if (allConveyor[0].GetComponent<SplineFollower>().followSpeed == 0) return false;
    
            for (int i = 0; i < allConveyor.Count; i++)
            {
                if (allConveyor[i].transform.childCount == 0)
                {
                    return true;
                }
            }
            return false;
        }
    
        // public bool IsCompleteSpawn()
        // {
        //     if (allRampBaggage.Count == 1)
        //     {
        //         return false;
        //     }
    
        //     for (int i = 0; i < allConveyor.Count; i++)
        //     {
        //         if (allConveyor[i].transform.childCount == 1)
        //         {
        //             return false;
        //         }
        //     }
        //     return true;
    
        // }
    }
    
}
