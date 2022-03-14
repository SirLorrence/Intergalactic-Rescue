using AlienGame.Backend;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace AlienGame.Runtime.Gameplay.Controls {
    public class InputController : MonoBehaviour {
        #region Field

        [ReadOnlyInspector] [SerializeField] private Vector2 _moveValue;
        private Vector2 _touchPoint; // will rename this 
        [ReadOnlyInspector] [SerializeField] private bool _isTouched;
        public Vector2 MoveValue => _moveValue;

        #endregion


        private void Awake() {
            EnhancedTouchSupport.Enable();
            _isTouched = false;
        }
#if UNITY_EDITOR
        private void OnEnable() => TouchSimulation.Enable();
        private void OnDisable() => TouchSimulation.Disable();

#endif

        private void Update() {
            if (EnhancedTouch.Touch.activeFingers.Count == 1)
                ProcessTouchInformation(EnhancedTouch.Touch.activeFingers[0].currentTouch);
        }

        private void ProcessTouchInformation(EnhancedTouch.Touch currentTouch) {
            if (currentTouch.phase == TouchPhase.Began) {
                _touchPoint = currentTouch.startScreenPosition;
                _isTouched = true;
            }

            if (currentTouch.phase == TouchPhase.Moved) OnDrag(currentTouch.screenPosition);
            if (currentTouch.phase == TouchPhase.Ended) {
                _isTouched = false;
                _moveValue = Vector2.zero;
            }
        }

        private void OnDrag(Vector2 currentPosition) {
            if (_isTouched) {
                var touchDelta = currentPosition - _touchPoint;
                _moveValue = Vector2.ClampMagnitude(touchDelta, 1.0f);
                Debug.DrawLine(_touchPoint, currentPosition, Color.red);
            }
        }
    }
}