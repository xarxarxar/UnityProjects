namespace EKStudio
{
    using UnityEngine;
    
    
    public abstract class ParticleProperties
    {
        public abstract void SpawnParticle(ParticleType particleType, Vector3 spawnPosition, Transform parent = null);
    
    }
    
}
