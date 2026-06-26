using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The class that produces all particles that can be produced.
/// There is a main production class called ParticleFactory, and it provides the production of particles where necessary by pulling the specific data of the particle types.
/// </summary>

namespace EKStudio
{
    public static class BaggageFactory
    {
        //It stores all ammos types and production classes in a dictionary.
        private static Dictionary<BaggageType, Func<BaggageProperties>> baggageFactories = new Dictionary<BaggageType, Func<BaggageProperties>>
        {
            { BaggageType.Red, () => new BaggageSpawn() },
            { BaggageType.Blue, () => new BaggageSpawn() },
            { BaggageType.Orange, () => new BaggageSpawn() },
            { BaggageType.Pink, () => new BaggageSpawn() },
            { BaggageType.Green, () => new BaggageSpawn() },
            { BaggageType.Purple, () => new BaggageSpawn() },
        };

        //The class in which ammos are spawned.
        public static BaggageProperties SpawnBaggage(BaggageType baggageType, Vector3 spawnPosition, Quaternion Rotation, out GameObject factoryProduct)
        {
            if (baggageFactories.TryGetValue(baggageType, out var factory))
            {
                var baggageProperties = factory.Invoke();
                baggageProperties.SpawnBaggage(baggageType, spawnPosition, Rotation);
                factoryProduct = baggageProperties.FactoryProduct;
                return baggageProperties;
            }
            else
            {
                factoryProduct = null;
                return null;
            }
        }
    }

    //All necessary data for ammos is drawn from scriptable objects and sent to the factory for production.
    public class BaggageSpawn : BaggageProperties
    {
        GameManager manager => GameManager.Instance;

        public override void SpawnBaggage(BaggageType baggageType, Vector3 spawnPosition, Quaternion Rotation, params object[] data)
        {
            Dictionary<BaggageType, GameObject> baggageObj = new Dictionary<BaggageType, GameObject>
        {
            { BaggageType.Red,  manager.data.AllSo.BaggageSO.Baggage_Red },
            { BaggageType.Blue,  manager.data.AllSo.BaggageSO.Baggage_Blue },
            { BaggageType.Orange,  manager.data.AllSo.BaggageSO.Baggage_Orange },
            { BaggageType.Pink,  manager.data.AllSo.BaggageSO.Baggage_Pink },
            { BaggageType.Green,  manager.data.AllSo.BaggageSO.Baggage_Green },
            { BaggageType.Purple,  manager.data.AllSo.BaggageSO.Baggage_Purple },
        };

            var spawnedBaggage = ObjectPool.SpawnObjects(baggageObj[baggageType], spawnPosition, Rotation);
            FactoryProduct = spawnedBaggage;
        }
    }

}
