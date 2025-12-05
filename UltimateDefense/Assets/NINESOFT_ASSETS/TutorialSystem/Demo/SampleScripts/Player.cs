using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NINESOFT.TUTORIAL_SYSTEM;

namespace NINESOFT.TUTORIAL_SYSTEM.Samples
{
    public class Player : MonoBehaviour
    {
        private const float SPEED = 7f;
        public CharacterController CharacterController;

        public TutorialEventTrigger MyTutorialEventTrigger;
     
        private void Update()
        {
            Vector3 direction = transform.forward * Input.GetAxisRaw("Vertical");
            transform.Rotate(0f, Input.GetAxisRaw("Horizontal") * .5f, 0f);
            CharacterController.Move(direction * Time.deltaTime * SPEED);

            if (direction != Vector3.zero)
            {
                MyTutorialEventTrigger.CallMyEvent();
            }

        }

        public void ResetRotation()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}