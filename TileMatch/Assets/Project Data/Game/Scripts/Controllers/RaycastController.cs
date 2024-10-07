using System;
using UnityEngine;

namespace Watermelon
{
    public class RaycastController : MonoBehaviour
    {
        private static bool isActive;

        public static event SimpleCallback OnInputActivated;

        private bool isPopupOpened;

        public void Initialise()
        {
            isActive = true;

            UIController.OnPoupWindowStateChanged += OnPopupStateChanged;
        }

        private void OnPopupStateChanged(IPopupWindow popupWindow, bool state)
        {
            isPopupOpened = state;
        }

        private void Update()
        {
            if (!isActive || !LevelController.IsRaycastEnabled) return;

            if (Input.GetMouseButtonDown(0) && !IsRaycastBlockedByUI())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    IClickableObject clickableObject = hit.transform.GetComponent<IClickableObject>();
                    if (clickableObject != null)
                    {
                        if (LevelController.IsLevelLoaded)
                        {
                            clickableObject.OnObjectClicked();
                        }
                    }
                }
            }
        }

        private bool IsRaycastBlockedByUI()
        {
            return isPopupOpened;
        }

        public static void Disable()
        {
            isActive = false;
        }

        public static void Enable()
        {
            isActive = true;

            OnInputActivated?.Invoke();
        }

        public void ResetControl()
        {

        }
    }
}
