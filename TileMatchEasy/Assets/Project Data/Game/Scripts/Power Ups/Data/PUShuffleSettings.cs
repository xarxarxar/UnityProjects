using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "PU Shuffle Settings", menuName = "Content/Power Ups/PU Shuffle Settings")]
    public class PUShuffleSettings : PUCustomSettings
    {
        [LineSpacer("Animation")]
        [SerializeField] float scaleTime = 0.5f;
        public float ScaleTime => scaleTime;

        [SerializeField] float scaleMinDelay = 0.05f;
        public float ScaleMinDelay => scaleMinDelay;

        [SerializeField] float scaleMaxDelay = 0.4f;
        public float ScaleMaxDelay => scaleMaxDelay;

        [SerializeField] Ease.Type scaleEasingType = Ease.Type.BackOut;
        public Ease.Type ScaleEasingType => scaleEasingType;

        public override void Initialise()
        {

        }
    }
}
