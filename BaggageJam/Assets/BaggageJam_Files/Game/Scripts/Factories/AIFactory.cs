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
    public static class AIFactory
    {
        //It stores all ammos types and production classes in a dictionary.
        private static Dictionary<BaggageType, Func<AIProperties>> baggageFactories = new Dictionary<BaggageType, Func<AIProperties>>
        {
            { BaggageType.Red, () => new AISpawn() },
            { BaggageType.Blue, () => new AISpawn() },
            { BaggageType.Orange, () => new AISpawn() },
            { BaggageType.Pink, () => new AISpawn() },
            { BaggageType.Green, () => new AISpawn() },
            { BaggageType.Purple, () => new AISpawn() },
        };

        //The class in which ammos are spawned.
        public static AIProperties SpawnAI(BaggageType baggageType, Vector3 spawnPosition, Quaternion Rotation, out GameObject factoryProduct)
        {
            if (baggageFactories.TryGetValue(baggageType, out var factory))
            {
                var AIProperties = factory.Invoke();
                AIProperties.SpawnAI(baggageType, spawnPosition, Rotation);
                factoryProduct = AIProperties.FactoryProduct;
                return AIProperties;
            }
            else
            {
                factoryProduct = null;
                return null;
            }
        }
    }

    //All necessary data for ammos is drawn from scriptable objects and sent to the factory for production.
    public class AISpawn : AIProperties
    {
        GameManager manager => GameManager.Instance;

        public override void SpawnAI(BaggageType baggageType, Vector3 spawnPosition, Quaternion Rotation, params object[] data)
        {
            Dictionary<BaggageType, GameObject> AIObj = new Dictionary<BaggageType, GameObject>
        {
            { BaggageType.Red,  manager.data.AllSo.AISO.AI_Red },
            { BaggageType.Blue,  manager.data.AllSo.AISO.AI_Blue },
            { BaggageType.Orange,  manager.data.AllSo.AISO.AI_Orange },
            { BaggageType.Pink,  manager.data.AllSo.AISO.AI_Pink },
            { BaggageType.Green , manager.data.AllSo.AISO.AI_Green },
            { BaggageType.Purple,  manager.data.AllSo.AISO.AI_Purple },

        };

            var spawnedAI = ObjectPool.SpawnObjects(AIObj[baggageType], spawnPosition, Rotation);
            FactoryProduct = spawnedAI;
        }
    }

}
