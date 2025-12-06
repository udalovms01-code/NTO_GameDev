using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class DoorTo2DInteract : DoorInteract
    {
        public Transform cameraPosTransform;

        private Vector3 _cameraPos;
        private Quaternion _cameraRot;
        private bool _is2d = false;

        public override void ToggleDoor(Vector3 playerPos)
        {
            if (!isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }

            base.ToggleDoor(playerPos);
        }

        private void Open()
        {
            PlayerMovement.Instance.enabled = false;
            MouseLook.Instance.SetActive(false);
            _cameraPos = MouseLook.Instance.cam.position;
            _is2d = true;
            _cameraRot = MouseLook.Instance.cam.rotation;
            MouseLook.Instance.cam.DORotate(cameraPosTransform.rotation.eulerAngles, 0.5f);
            MouseLook.Instance.cam.DOMove(cameraPosTransform.position, 0.5f);
        }

        private void Close()
        {
            PlayerMovement.Instance.enabled = true;
            MouseLook.Instance.SetActive(true);
            MouseLook.Instance.cam.DOMove(_cameraPos, 0.5f);
            MouseLook.Instance.cam.DORotate(_cameraRot.eulerAngles, 0.5f);
            _is2d = false;

        }

        private void Update()
        {
            if (_is2d && Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleDoor(PlayerMovement.Instance.transform.position);
                Close();
            }
        }

    }
}