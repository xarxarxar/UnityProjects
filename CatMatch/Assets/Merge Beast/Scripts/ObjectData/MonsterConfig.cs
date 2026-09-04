using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "MonsterConfig", menuName = "ConfigData/MonsterConfig")]
    public class MonsterConfig : ScriptableObject
    {
        public MonsterDictionary Monster;

        public MonsterData GetMonster(EnumDefine.MONSTER monster)
        {
            if (Monster.ContainsKey(monster))
                return Monster[monster];
            return null;
        }
    }
}
