using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The class that produces all particles that can be produced.
/// There is a main production class called ParticleFactory, and it provides the production of particles where necessary by pulling the specific data of the particle types.
/// </summary>

namespace EKStudio
{
    public static class ParticleFactory
    {
        //It stores all particle types and production classes in a dictionary.
        private static Dictionary<ParticleType, Func<ParticleProperties>> particleFactories = new Dictionary<ParticleType, Func<ParticleProperties>>
        {
            { ParticleType.BaggageMerge, () => new ParticleMerge() },
        };

        //The class in which particles are spawned.
        public static ParticleProperties SpawnParticle(ParticleType particleType, Vector3 spawnPosition, Transform parent)
        {
            if (particleFactories.TryGetValue(particleType, out var factory))
            {
                var particle = factory.Invoke();
                particle.SpawnParticle(particleType, spawnPosition, parent);
                return particle;
            }
            else
            {
                return null;
            }
        }
    }

    //All necessary data for particles is drawn from scriptable objects and sent to the factory for production.
    public class ParticleMerge : ParticleProperties
    {
        GameManager manager => GameManager.Instance;

        public override void SpawnParticle(ParticleType particleType, Vector3 spawnPosition, Transform parent)
        {
            var spawnedParticle = ObjectPool.SpawnObjects(manager.data.AllSo.ParticleSO.baggageMerge_Part, spawnPosition, Quaternion.identity);

            DOVirtual.DelayedCall(3, () => spawnedParticle.SetActive(false));
        }
    }

}
