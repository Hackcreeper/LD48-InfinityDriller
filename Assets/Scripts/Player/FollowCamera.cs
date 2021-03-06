using UnityEngine;

namespace Player
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;

        private void Update()
        {
            transform.position = target.position + offset;
        }
    }
}