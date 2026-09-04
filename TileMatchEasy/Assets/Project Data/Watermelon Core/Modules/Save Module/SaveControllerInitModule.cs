using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Save Controller Settings", Core = true)]
    public class SaveControllerInitModule : InitModule
    {
        [SerializeField] bool autoSave;

        public override void CreateComponent(Initialiser Initialiser)
        {
            SaveController.Initialise(autoSave);
        }

        public SaveControllerInitModule()
        {
            moduleName = "Save Controller Settings";
        }
    }
}
