using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NINESOFT.TUTORIAL_SYSTEM
{
    
    public class CollisionListener : MonoBehaviour
    {
        public StringAction OnCollision;
        public StringAction OnTrigger;

        public StringAction OnCollision2D;
        public StringAction OnTrigger2D;

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision?.Invoke(collision.transform.tag);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTrigger?.Invoke(other.tag);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollision2D?.Invoke(collision.transform.tag);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnTrigger2D?.Invoke(collision.tag);
        }

    }
}