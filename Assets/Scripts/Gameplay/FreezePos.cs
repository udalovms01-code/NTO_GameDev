using System;
using UnityEngine;

namespace Gameplay
{
    public class FreezePos : MonoBehaviour
    {
        public bool x = false;
        public bool y = false;
        public bool z = false;
        public float startX;
        public float startY;
        public float startZ;
        public bool freezeOnStart = true;
        private Vector3 newPos;
        private void Start()
        {
            SetNewStartPos();
        }

        public void FreezeAxes()
        {
            transform.position = new Vector3(
                x ? startX : transform.position.x,
                y ? startY : transform.position.y,
                z ? startZ : transform.position.z);
            //transform.position = new Vector3(transform.position.x, 0.1756734f, transform.position.z);
        }

        public void SetNewStartPos()
        {
            startX = transform.position.x;
            startY = transform.position.y;
            startZ = transform.position.z;
        }
    }
}