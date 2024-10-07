using UnityEngine;

namespace Watermelon
{
    public static class PhysicsHelper
    {
        public static readonly int LAYER_DEFAULT = LayerMask.NameToLayer("Default");
        public static readonly int LAYER_PLAYER = LayerMask.NameToLayer("Player");

        public const string TAG_PLAYER = "Player";

        public static void Init()
        {

        }
    }
}