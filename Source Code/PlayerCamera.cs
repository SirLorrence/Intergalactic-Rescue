using Cinemachine;
using UnityEngine;

namespace AlienGame.Runtime.Gameplay {
    public class PlayerCamera : MonoBehaviour {
        // private float cameraBaseFOV;
        // [SerializeField] private float zoomOutTime;
        // public bool IsMoving { get; set; }
        private CinemachineVirtualCamera _cinCam;

        private void OnEnable() {
            _cinCam = GetComponentInChildren<CinemachineVirtualCamera>();
            // cameraBaseFOV = _cinCam.m_Lens.FieldOfView;
        }

        public void CameraSetUp(Transform playerPosition) => _cinCam.Follow = playerPosition;
        public void OnPlayerDeath() => _cinCam.Follow = default;

        // private void Update() {
        //     if (IsMoving) {
        //         if (_cinCam.m_Lens.FieldOfView <= cameraBaseFOV + 10) {
        //             _cinCam.m_Lens.FieldOfView = Mathf.Lerp(_cinCam.m_Lens.FieldOfView, cameraBaseFOV + 10,
        //                 Time.deltaTime * zoomOutTime);
        //         }
        //     }
        //     else {
        //         if (_cinCam.m_Lens.FieldOfView >= cameraBaseFOV) {
        //             _cinCam.m_Lens.FieldOfView = Mathf.Lerp(_cinCam.m_Lens.FieldOfView, cameraBaseFOV,
        //                 Time.deltaTime * zoomOutTime);
        //         }
        //     }
        // }
    }
}