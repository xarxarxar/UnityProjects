namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using Dreamteck.Splines;
    using UnityEngine;
    
    public class BaggageRay : MonoBehaviour
    {
        public SplineFollower SplineFollower => GetComponent<SplineFollower>();
        float duration;
        public float speed;
    
        void Update()
        {
            RayCheck();
        }
        void RayCheck()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, 1f))
            {
                if (hit.transform.tag == TagHolder.Conveyor_Tag && hit.transform.childCount == 1)
                {
                    SplineFollower.followSpeed = 0;
                }
                else if (hit.transform.gameObject.layer == 6)
                {
                    SplineFollower.followSpeed = 0;
                }
                else if (!TimeStop())
                {
                    SplineFollower.followSpeed = speed;
                }
            }
            else
            {
                duration += Time.deltaTime;
                if (duration > .2f && !TimeStop())
                {
                    SplineFollower.followSpeed = speed;
                    duration = 0;
                }
            }
        }
    
        void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.up, Color.red);
        }
    
        bool TimeStop()
        {
            return ConveyorBelt.Instance.allConveyor[0].GetComponent<SplineFollower>().followSpeed == 0;
        }
    }
    
}
