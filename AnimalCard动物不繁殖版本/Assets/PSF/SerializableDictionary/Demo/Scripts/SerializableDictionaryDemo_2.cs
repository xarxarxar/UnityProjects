using SerializableDictionary.Scripts;
using UnityEngine;

namespace SerializableDictionary.Demo.Scripts
{
    [CreateAssetMenu(menuName = "SerializableDictionaryDemo/SerializableDictionaryDemo_2", fileName = "SerializableDictionaryDemo_2")]
    public class SerializableDictionaryDemo_2 : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<Material, bool> _areMaterialsTransparent;
        [SerializeField] private SerializableDictionary<GameObject, Mesh> _prefabMeshes;
    }
}