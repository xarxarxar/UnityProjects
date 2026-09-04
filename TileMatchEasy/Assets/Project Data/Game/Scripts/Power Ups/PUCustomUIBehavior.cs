using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class PUCustomUIBehavior : PUUIBehavior
    {
        [Group("Refs")]
        [SerializeField] TextMeshProUGUI titleText;

        protected override void ApplyVisuals()
        {
            base.ApplyVisuals();

            PUCustomSettings customSettings = (PUCustomSettings)settings;

            titleText.text = customSettings.Title;
        }
    }
}
