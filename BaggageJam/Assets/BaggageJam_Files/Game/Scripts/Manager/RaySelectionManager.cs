namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using Dreamteck.Splines;
    using UnityEngine;
    using MoreMountains.NiceVibrations;
    using UnityEngine.EventSystems;
    public class RaySelectionManager : InstanceManager<RaySelectionManager>
    {
        public LayerMask layerMask;
        public Transform currentBaggage;
    
        void Start()
        {
    
        }
    
        void Update()
        {
            SelectBaggage();
        }
        bool IsTouchOverUI()
        {
            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            return EventSystem.current.IsPointerOverGameObject();
        }
        void SelectBaggage()
        {
            if (!SlotManager.Instance.CanAddBaggage())
                return;
            if (IsTouchOverUI())
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                ISelectable Selectable = null;
    
                if (hit.transform.GetComponent<SplineFollower>() != null)
                {
                    return;
                }
    
                if (currentBaggage != null)
                    Selectable = currentBaggage.transform.GetComponent<ISelectable>();
    
                if (Input.GetMouseButton(0))
                {
                    // print(hit.transform.name);
    
                    if (currentBaggage == null)
                    {
                        currentBaggage = hit.transform;
                        currentBaggage.transform.GetComponent<ISelectable>().ISelect();
                    }
                    else if (currentBaggage != hit.transform)
                    {
                        Selectable.INotSelected();
                        currentBaggage = null;
                    }
    
                }
                else if (Input.GetMouseButtonUp(0) && currentBaggage != null)
                {
                    Selectable.ISelected();
                    currentBaggage = null;
                    MMVibrationManager.Haptic(HapticTypes.LightImpact);
                }
    
            }
            else
            {
                if (currentBaggage != null)
                {
                    ISelectable Selectable = currentBaggage.GetComponent<ISelectable>();
                    Selectable.INotSelected();
    
                    currentBaggage = null;
                }
            }
        }
    }
    
}
