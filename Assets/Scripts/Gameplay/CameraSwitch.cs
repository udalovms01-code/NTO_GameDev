using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class CameraSwitch : IInitializable
    {
        private Transform _camera3d; 
        private Transform _camera2d;
        private Quaternion _camera3dRotation;
        private Vector3 _camera3dPosition;
        
        public void SwitchCamera(bool is3D)
        {
            if (!is3D)
            {
                _camera3dRotation = _camera3d.rotation;
                _camera3dPosition = _camera3d.position;
                _camera3d.DORotate(_camera2d.rotation.eulerAngles, 0.5f);
                _camera3d.DOMove(_camera2d.position, 0.5f).OnComplete(
                    () =>
                    {
                        _camera3d.gameObject.SetActive(false); 
                        _camera2d.gameObject.SetActive(true);
                    });
            }
            else
            {
                _camera2d.gameObject.SetActive(false);
                _camera3d.gameObject.SetActive(true);
                _camera3d.DORotate(_camera3dRotation.eulerAngles, 0.5f);
                _camera3d.DOMove(_camera3dPosition, 0.5f);
            }
            
            ChangeCamera2d(!is3D);
            ChangeCamera3d(is3D);
        }

        public void ChangeCamera2d(bool state)
        {
            Camera2dState.Instance.EnableElements(state);
        }

        public void ChangeCamera3d(bool state)
        {
            PlayerMovement.Instance.enabled = state;
            MouseLook.Instance.SetActive(state);
            Camera3dState.Instance.EnableElements(state);
        }

        public void Initialize()
        {
            _camera3d = Camera3dState.Instance.camera3d;
            _camera2d = Camera2dState.Instance.camera2d;
            _camera3dRotation = _camera3d.rotation;
            _camera3dPosition = _camera3d.position;
            ChangeCamera2d(false);
            ChangeCamera3d(true);
            _camera2d.gameObject.SetActive(false);
        }
    }
}