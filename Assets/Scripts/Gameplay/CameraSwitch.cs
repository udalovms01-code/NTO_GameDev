using Zenject;

namespace Gameplay
{
    public class CameraSwitch : IInitializable
    {
        public void SwitchCamera(bool is3D)
        {
            ChangeCamera2d(!is3D);
            ChangeCamera3d(is3D);
        }
        
        public void ChangeCamera2d(bool state)
        {
            Camera2dState.Instance.EnableElements(state);
        }

        public void ChangeCamera3d(bool state)
        {
            PlayerMovement.Instance.enabled = !state;
            MouseLook.Instance.SetActive(!state);
            Camera3dState.Instance.EnableElements(state);
        }

        public void Initialize()
        {
            SwitchCamera(true);
        }
    }
}