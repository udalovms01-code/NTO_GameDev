using System;
using UnityEngine;

namespace Gameplay
{
    public class DoorInteract : MonoBehaviour, IIteractable
    {
        public Transform doorPivot;     
        public float openAngle = 90f;   
        public float speed = 4f;

        public bool isOpen { get; private set; }
        private Quaternion closedRotation;
        private Quaternion openRotation;

        private void Start()
        {
            if (doorPivot == null)
                doorPivot = transform;

            closedRotation = doorPivot.localRotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerMovement player))
            {
                ToggleDoor(player.transform.position);
            }
        }

        public virtual void ToggleDoor(Vector3 playerPos)
        {
            Vector3 dirToPlayer = doorPivot.position - playerPos;

            float direction = Vector3.Dot(dirToPlayer, doorPivot.right) > 0 ? 1f : -1f;

            if (!isOpen)
            {
                // ВРАЩЕНИЕ ПО Z
                openRotation = closedRotation * Quaternion.Euler(0f, 0f, openAngle * direction);
            }
            else
            {
                openRotation = closedRotation;
            }

            StopAllCoroutines();
            StartCoroutine(RotateDoor());
            isOpen = !isOpen;
        }

        private System.Collections.IEnumerator RotateDoor()
        {
            Quaternion startRot = doorPivot.localRotation;
            Quaternion targetRot = openRotation;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                doorPivot.localRotation = Quaternion.Lerp(startRot, targetRot, t);
                yield return null;
            }
        }

        public void Interact()
        {
            
        }

        public void OnClick()
        {
            ToggleDoor(PlayerMovement.Instance.transform.position);
        }

        public void UnInteract()
        {
            
        }
    }
}