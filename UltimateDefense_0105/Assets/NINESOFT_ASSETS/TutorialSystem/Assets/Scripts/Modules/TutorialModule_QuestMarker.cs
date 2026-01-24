using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    [ExecuteAlways]
    public class TutorialModule_QuestMarker : TutorialModule
    {
        public Transform Target;
        [SerializeField] private Vector3 Offset;
        [SerializeField] private bool ShowDistanceText = true;

        private Image questMarkerImage;
        private Text distanceText;

        private Camera Cam;
        [SerializeField] private Transform PlayerTransform;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (questMarkerImage == null) questMarkerImage = GetComponent<Image>();
            if (distanceText == null) distanceText = GetComponentInChildren<Text>();

            if (gameObject.scene.rootCount !=0)
            {
                try { if (Cam == null) Cam = Camera.main; } catch { }
                try { if (PlayerTransform == null) PlayerTransform = Camera.main.transform; } catch { }
            }
            distanceText.text = "";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Init();
        }
#endif

        private void Update()
        {
            if (IsActive && questMarkerImage != null)
                UpdateMarkerPosition();
        }

        private void UpdateMarkerPosition()
        {
            if (Target == null)
            {
                if (Application.isPlaying)
                    TutorialManager.Instance.DebugLog("Quest Marker (" + gameObject.name + ")  --> Target Null !", gameObject, DebugType.Error);
                return;
            }

            float minX = questMarkerImage.GetPixelAdjustedRect().width / 2f;
            float maxX = Screen.width - minX;

            float minY = questMarkerImage.GetPixelAdjustedRect().height / 2f;
            float maxY = Screen.height - minY;

            Vector2 pos = Cam.WorldToScreenPoint(Target.position + Offset);

            if (Vector3.Dot((Target.position - Cam.transform.position), Cam.transform.forward) < 0)
            {
                if (pos.x < Screen.width / 2f)
                    pos.x = maxX;
                else
                    pos.x = minX;
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            questMarkerImage.transform.position = pos;


            if (ShowDistanceText)
            {
                float dist = ((int)Vector3.Distance(PlayerTransform.position, Target.position));
                if (dist < 0) dist = 0;
                distanceText.text = dist + "m";
            }

        }

    }


}
