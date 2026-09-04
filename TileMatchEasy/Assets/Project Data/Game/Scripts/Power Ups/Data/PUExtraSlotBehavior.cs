
using UnityEngine;

namespace Watermelon
{
    public class PUExtraSlotBehavior : PUBehavior
    {
        private PUExtraSlotSettings customSettings;

        public override void Initialise()
        {
            customSettings = (PUExtraSlotSettings)settings;
        }

        public override bool Activate()
        {
            IsBusy = true;

            LevelController.Dock.AddExtraSlot();

            return true;
        }

        public override void ResetBehavior()
        {
            IsBusy = false;
        }
    }
}
