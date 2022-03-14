using AlienGame.Backend;
using AlienGame.Runtime.Managers;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace AlienGame.Runtime.Gameplay.AI {
    public class AIMovement : MonoBehaviour {
        #region Fields

        private Direction _turnDirection;

        private enum Direction {
            Left = -1,
            Right = 1,
        }


        [SerializeField] private float _maxSpeed = 40;
        [SerializeField] private float _turnSpeed = 70;
        [SerializeField] private float _maxAhead = 15;
        [SerializeField] private float _maxLockOnDistance = 25;

        [SerializeField] private Transform _target;
        private Vector3 _colliderOffset;
        private Vector3 _targetDirection;

        private Rigidbody _rb;
        private BoxCollider _collider;
        public BoxCollider Collider => _collider;

        private Ray _leftDetectionRay, _rightDetectionRay, _lockOnRay;

        private GameObject _leftRayHolder, _rightRayHolder, _lockOnRayHolder;
        private LayerMask _environmentLayer, _enemyLayer;

        private bool _waitingBeforeChase;
        private float _waitedForChase;

        [Header("Debugging")] [SerializeField] private bool _debug;
        private DebugLogger _log = new DebugLogger("[AI-MOVEMENT]");

        //Overwrites

        [SerializeField] private bool _forceLock;

        #endregion

        #region Unity Messages

#if UNITY_EDITOR

        private void OnValidate() {
            _log.IsEnabled = _debug;
        }

        private void OnDrawGizmos() {
            if (_debug) {
                //Gizmos.color = Color.blue;
                //Vector3 testAhead = transform.position + Vector3.Normalize(_rb.velocity) * maxAhead;
                //Gizmos.DrawLine(transform.position, testAhead);
                // Gizmos.DrawLine(transform.position, velocity);
                _collider = GetComponentInChildren<BoxCollider>();
                var bodyWidth = _collider.size.x;
                var bodyOffset = new Vector3(bodyWidth / 1.75f, 0, 0);
                _leftRayHolder = transform.Find("LeftRay").transform.gameObject;
                _rightRayHolder = transform.Find("RightRay").transform.gameObject;
                _lockOnRayHolder = transform.Find("LockOnRay").transform.gameObject;


                if (_overrideTurn) {
                    _leftRayHolder.transform.localPosition = new Vector3(-bodyOffset.x * 1.5f, 0, 0);
                    _rightRayHolder.transform.localPosition = new Vector3(bodyOffset.x * 1.5f, 0, 0);

                    Gizmos.color = (Physics.Raycast(_leftDetectionRay, _maxAhead)) ? Color.green : Color.red;
                    Gizmos.DrawRay(_leftRayHolder.transform.position, _leftRayHolder.transform.forward * _maxAhead);

                    Gizmos.color = (Physics.Raycast(_rightDetectionRay, _maxAhead)) ? Color.blue : Color.red;
                    Gizmos.DrawRay(_rightRayHolder.transform.position, _rightRayHolder.transform.forward * _maxAhead);
                }
                else {
                    _leftRayHolder.transform.localPosition = new Vector3(-bodyOffset.x, 0, 0);
                    _rightRayHolder.transform.localPosition = new Vector3(bodyOffset.x, 0, 0);

                    Gizmos.color = (Physics.Raycast(_leftDetectionRay, _maxAhead)) ? Color.green : Color.red;
                    Gizmos.DrawRay(_leftRayHolder.transform.position, _leftRayHolder.transform.forward * _maxAhead);

                    Gizmos.color = (Physics.Raycast(_rightDetectionRay, _maxAhead)) ? Color.blue : Color.red;
                    Gizmos.DrawRay(_rightRayHolder.transform.position, _rightRayHolder.transform.forward * _maxAhead);
                }

                Gizmos.color = LockOnTarget() ? Color.red : Color.green;
                Gizmos.DrawRay(_lockOnRayHolder.transform.position,
                    _lockOnRayHolder.transform.forward * _maxLockOnDistance);
                // Gizmos.color = Color.blue;
                // Gizmos.DrawRay(_leftDetectionRay);
                //Debug.DrawRay(transform.position - bodyoffset, transform.forward);


                //Gizmos.DrawSphere(Avoidance(), 2f);
                // Gizmos.DrawLine(transform.position, GetDirectionToTarget(player.transform)
            }
        }
#endif

        #endregion

        #region Mono Functions

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponentInChildren<BoxCollider>();
            _environmentLayer = LayerMask.GetMask("Environment");
            _enemyLayer = LayerMask.GetMask("Enemy");
            _leftRayHolder = transform.Find("LeftRay").transform.gameObject;
            _rightRayHolder = transform.Find("RightRay").transform.gameObject;
            _lockOnRayHolder = transform.Find("LockOnRay").transform.gameObject;
            // ResetSpeed();
        }

        private void Start() {
            _colliderOffset = new Vector3(_collider.size.x / 1.75f, 0, 0);

            //_leftDetectionRay = new Ray(transform.position, transform.forward * 10);
            //_leftDetectionRay = Ray(transform.position, transform.forward);
        }

        private void OnEnable() {
            if (!_debug) {
                _target = GameManager.Instance.PlayerTransform;
            }
            //SetDifficulty(difficulty);
            //AIState = State.Chasing;
        }
        // void Update() => PreMovementCalc();

        void FixedUpdate() {
            if (GameManager.Instance.ActivePlayerScript.IsAlive) Movement();
        }

        #endregion

        #region Movement Handlers

        public bool _overrideTurn = false;


        public void Movement() {
            if (_target != null) _targetDirection = _target.position - transform.position;

            float increaseR = (_overrideTurn) ? 1.75f : 1f;

            _leftRayHolder.transform.localPosition = new Vector3(-_colliderOffset.x * increaseR, 0, 0);
            _rightRayHolder.transform.localPosition = new Vector3(_colliderOffset.x * increaseR, 0, 0);

            _leftDetectionRay = new Ray(_leftRayHolder.transform.position, _leftRayHolder.transform.forward);
            _rightDetectionRay = new Ray(_rightRayHolder.transform.position, _rightRayHolder.transform.forward);

            // transform.Translate(Vector3.forward * maxSpeed * Time.deltaTime);
            _rb.MovePosition(transform.position + (transform.forward * _maxSpeed * Time.deltaTime));


            if (_overrideTurn) {
                Turning((float)_turnDirection, 2.5f);
                //if (!Physics.Raycast(_leftDetectionRay, maxAhead) && !Physics.Raycast(_rightDectectionRay, maxAhead))
                if (!RayDetectionCheck(Direction.Left) && !RayDetectionCheck(Direction.Right))
                    _overrideTurn = false;
                else if (RayDetectionCheck(Direction.Left) && !RayDetectionCheck(Direction.Right))
                    _turnDirection = Direction.Right;
                else if (!RayDetectionCheck(Direction.Left) && RayDetectionCheck(Direction.Right))
                    _turnDirection = Direction.Left;
            }
            else {
                if (!LockOnTarget()) {
                    _maxSpeed -= .5f * Time.deltaTime;

                    if (RayDetectionCheck(Direction.Left)) {
                        _turnDirection = Direction.Right;
                        _overrideTurn = true;
                    }

                    if (RayDetectionCheck(Direction.Right)) {
                        _turnDirection = Direction.Left;
                        _overrideTurn = true;
                    }
                }
                else {
                    _maxSpeed += 1f * Time.deltaTime; //increase speed when locked on
                }

                if (_target != null) {
                    if (Vector3.Angle(_targetDirection, transform.forward) > 10 /*desired chase/view angle*/) {
                        var turnD = SteeringAngleDir(_targetDirection);
                        Turning(turnD);
                    }
                }
            }

            // _maxSpeed = Mathf.Clamp(_maxSpeed, initalSpeed, initalSpeed + 10);

            //_rb.velocity = Vector3.ClampMagnitude(_rb.velocity += Vector3.forward * maxVelocity * Time.deltaTime, maxSpeed);

            //_ahead = transform.position + Vector3.Normalize(_rb.velocity) * maxAhead;

            // //Calculating Forces
            // _desiredVelocity = Vector3.Normalize(_target.position - transform.position) * maxVelocity;
            // _steering = _desiredVelocity - _rb.velocity;
            // //_steering += Avoidance();

            // //Adding Forces
            // _steering = Vector3.ClampMagnitude(_steering,maxForce);
            // _steering = _steering/_rb.mass;
            // _rb.velocity = Vector3.ClampMagnitude(_rb.velocity + (_steering * Time.deltaTime), maxSpeed);

            // //Look Direction 
            // _lookRotation = SteeringRotation(_desiredVelocity);
            // transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, Time.deltaTime * 2f);
        }

        private bool RayDetectionCheck(Direction dirOverride) => dirOverride switch {
            Direction.Left => Physics.Raycast(_leftDetectionRay, _maxAhead, _environmentLayer) ||
                              Physics.Raycast(_leftDetectionRay, _maxAhead, _enemyLayer),
            Direction.Right => Physics.Raycast(_rightDetectionRay, _maxAhead, _environmentLayer) ||
                               Physics.Raycast(_rightDetectionRay, _maxAhead, _enemyLayer),
            _ => default // the process of checking both cause an noticable delay.
        };

        private bool LockOnTarget() {
            _lockOnRay = new Ray(_lockOnRayHolder.transform.position, _lockOnRayHolder.transform.forward);
            if (Physics.Raycast(_lockOnRay, out var raycastHit, _maxLockOnDistance)) {
                if (raycastHit.collider.CompareTag("Player") || _forceLock)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Checks to see which side the target is on from the forward facing vector, then returns an rotation value
        /// </summary>
        /// <param name="targetDirection">The desired target vector</param>
        /// <param name="dotAngle">The angle in degrees</param>
        /// <returns>An clamped angle value for the plane to rotate on</returns>
        private float SteeringAngleDir(Vector3 targetDirection, float dotAngle = default) {
            Vector3 crossProductVector = Vector3.Cross(transform.forward, targetDirection);
            float dir = Vector3.Dot(crossProductVector, transform.up);
            //if (dir > 0) return Mathf.Clamp(-dotAngle, -10, 0); // returns right rotation ;
            //else if (dir < 0) return Mathf.Clamp(dotAngle, 0, 10); // return left rotation;
            if (dir > 0) return 1; // returns right rotation ;
            else if (dir < 0) return -1; // return left rotation;
            else return 0; // return no pitch rotation
        }

        private void Turning(float turnDirection, float speedMultipler = 1) {
            if (turnDirection != 0) {
                //transform.localEulerAngles += Vector3.up * turnDirection * turnSpeed * speedMultipler * Time.deltaTime;
                Quaternion deltaRotation =
                    Quaternion.Euler(Vector3.up * turnDirection * _turnSpeed * speedMultipler * Time.fixedDeltaTime);
                deltaRotation.x = 0f;
                deltaRotation.z = 0f;
                _rb.MoveRotation(deltaRotation * _rb.rotation);
            }
        }

        #endregion

        #region Deprecated Code

        // [SerializeField] private bool _enableTilt;
        // [SerializeField] [Range(-1, 1)] private int _turnDir;
        // [SerializeField] private float _testOffset;

        //public enum DifficultyLevel {
        //    Easy,
        //    Medium,
        //    Hard,
        //    OtherWorldly
        //}
        // [SerializeField][ReadOnlyInspector]
        // private float max

        //public DifficultyLevel difficulty;
        //public float maxForce; //steering force
        //public float maxVelocity;
        //public float maxAvoidForce;

        // private Vector3 _desiredVelocity;
        // private Vector3 _steering;
        // private Vector3 _ahead;
        // private Quaternion _lookRotation;

        // public void IncreaseSpeed() {
        //     _maxSpeed += 5;
        //     _turnSpeed += 5;
        // }

        // public void ResetSpeed() {
        //     _maxSpeed = initalSpeed;
        //     _turnSpeed = initalTurnSpeed;
        // }

        // private void PreMovementCalc(){
        //     _targetDirection = _target.position - transform.position;

        //     var increaseR = (_overrideTurn) ? 2 : 1;

        //     _leftDetectionRay = new Ray(_rb.position - (_colliderOffset * increaseR), transform.forward);
        //     _rightDetectionRay = new Ray(_rb.position + (_colliderOffset * increaseR), transform.forward);
        // }

        ///// <summary>
        ///// This handles where the enemy is looking forward and tilting on turns
        ///// </summary>
        ///// <param name="targetDirection"></param>
        ///// <returns></returns>
        //private Quaternion SteeringRotation(Vector3 targetDirection) {
        //    float angleTowardsTarget = Vector3.Angle(transform.forward, targetDirection);
        //    float angleToRotation = SteeringAngleDir(targetDirection, angleTowardsTarget);
        //    float exaggeration = 100f;

        //    Quaternion lookDirection = transform.rotation;
        //    Quaternion targetRotation = Quaternion.Euler(lookDirection.x, lookDirection.y, (enableTilt)? angleToRotation : lookDirection.z);

        //    lookDirection = Quaternion.LookRotation(_rb.velocity);
        //    lookDirection.z = Quaternion.Lerp(lookDirection, targetRotation, (maxSpeed * exaggeration) / 2 * Time.deltaTime).z;

        //    _aiMoveLogger.Message($"Angle to player {angleTowardsTarget}");
        //    _aiMoveLogger.Message($"Steering Direction {angleToRotation}");
        //    return lookDirection;
        //}

        #endregion
    }
}