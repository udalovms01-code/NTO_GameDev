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
            base.ToggleDoor(playerPos);
            if(isOpen) return;
            PlayerMovement.Instance.enabled = false;
            MouseLook.Instance.SetActive(false);
            _cameraPos = MouseLook.Instance.cam.position;
            _is2d = true;
            _cameraRot = MouseLook.Instance.cam.rotation;
            MouseLook.Instance.cam.DORotate(cameraPosTransform.rotation.eulerAngles, 0.5f);
            MouseLook.Instance.cam.DOMove(cameraPosTransform.position, 0.5f);
        }
        
        private void Update()
        {
            if (_is2d && Input.GetKeyDown(KeyCode.Escape))
            {
                PlayerMovement.Instance.enabled = true;
                MouseLook.Instance.SetActive(true);
                MouseLook.Instance.cam.DOMove(_cameraPos, 0.5f);
                MouseLook.Instance.cam.DORotate(_cameraRot.eulerAngles, 0.5f);
                _is2d = false;
                ToggleDoor(PlayerMovement.Instance.transform.position);
            }
        }
        
    }
}