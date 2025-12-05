using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public static class Utilities
    {
        private const string keys = "a0b1c2d3e4f56789";
        public static string GetRandomKey()
        {
            string key = "";
            for (int i = 0; i < 8; i++)
            {
                key += keys[Random.Range(0, keys.Length)];
            }
            return key;
        }
    }
}
