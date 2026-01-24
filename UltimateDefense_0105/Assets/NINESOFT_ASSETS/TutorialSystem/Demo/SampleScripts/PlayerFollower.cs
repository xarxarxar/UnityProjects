using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM.Samples
{

    public class PlayerFollower : MonoBehaviour
    {
        public Transform player;
        private Vector3 offset;
        void Start()
        {
            offset = transform.position - player.position;
        }


        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, player.position + offset, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation, Time.deltaTime * .5f);
        }
    }

}