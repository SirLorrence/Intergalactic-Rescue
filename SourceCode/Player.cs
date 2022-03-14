using AlienGame.Backend;
using AlienGame.Runtime.Gameplay.Controls;
using AlienGame.Runtime.Managers;
using UnityEngine;

namespace AlienGame.Runtime.Gameplay {
    [RequireComponent(typeof(InputController), typeof(Rigidbody))]
    public class Player : MonoBehaviour {
        #region Fields

        [Header("Player Settings")] [SerializeField]
        private float _speed = 250;

        [SerializeField] private float _turnSpeed = 720;

        [SerializeField] private GameObject _explosionVFX;
        [SerializeField] private GameObject _fireVFX;

        //Debugging and Player information
        private DebugLogger _log = new DebugLogger("Player");
        [SerializeField] private bool _debug;
        [ReadOnlyInspector] [SerializeField] private bool _isAlive;
        [ReadOnlyInspector] [SerializeField] private bool _abilityActive;

        [ReadOnlyInspector] [SerializeField] private float _abilityTime;

        private Vector3 _offset;
        private Rigidbody _rigidbody;
        private InputController _inputController;
        private PlayerCamera _playerCamera;

        #endregion

        #region Properties

        public float Speed {
            get => _speed;
            set => _speed = value;
        }

        public bool IsAlive => _isAlive;

        #endregion

        #region Start Methods

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _inputController = GetComponent<InputController>();
            _playerCamera = FindObjectOfType<PlayerCamera>();
            _explosionVFX.SetActive(false);
            _fireVFX.SetActive(false);

            // _playerRadar = GetComponent<PlayerRadar>();
        }

        private void Start() => OnSpawn();
#if UNITY_EDITOR
        private void OnValidate() {
            _log.IsEnabled = _debug;
        }
#endif

        #endregion

        #region Mono Update Methods

        private void FixedUpdate() => Movement();
        private void OnCollisionEnter(Collision other) => OnDeath();

        #endregion

        #region Player Actions

        private void OnSpawn() {
            _playerCamera.CameraSetUp(this.transform);
            _isAlive = true;
            _rigidbody.useGravity = false;

            // _playerRadar.StartRadar();
        }

        private void Movement() {
            if (_isAlive) {
                Vector3 movement = new Vector3(_inputController.MoveValue.x, 0, _inputController.MoveValue.y)
                    .normalized;

                _rigidbody.AddForce(movement * (_speed * 50 * Time.deltaTime));
                // _playerCamera.IsMoving = movement != Vector3.zero;
                if (movement != Vector3.zero) {
                    var rot = Quaternion.LookRotation(_rigidbody.velocity.normalized);
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
                }
            }
        }

        private void OnDeath() {
            if (!_isAlive)
                return;
            _isAlive = false;
            _rigidbody.useGravity = true;
            _playerCamera.OnPlayerDeath();
            AudioManager.Instance.PlayAudio(AudioTypes.SFX_PLAYER_DEATH);
            _explosionVFX.SetActive(true);
            _fireVFX.SetActive(true);
            GameManager.Instance.OnDeath();
        }

        #endregion

        #region Deprecated Code

        // [ReadOnlyInspector] [SerializeField] private ItemType _currentType;
        // private PlayerRadar _playerRadar;
        //[ReadOnlyInspector][SerializeField]  private float _multiplier = 1;
        //[Header("Camera Settings")] [SerializeField]
        //private Vector3 cameraPosition = new Vector3(0, 10, -15);
        //[SerializeField] private float cameraRotation = 30;
        // [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();

        /*
        ================
        Power Up Handlers
        ================
        
        public void CheckActiveAbility(ItemType itemType, float time, Item item) {
            if (time <= 0) item.OnItemActive();
            else if (_abilityActive && itemType == _currentType) _abilityTime += time;
            else {
                _abilityTime += time;
                _currentType = itemType;
                StartCoroutine(ActivatePowerUp(item));
            }
        }
        
        IEnumerator ActivatePowerUp(Item powerItem) {
            _abilityActive = true;
            while (_abilityTime > 0) {
                powerItem.OnItemActive();
                _abilityTime -= Time.deltaTime;
                yield return null;
            }
        
            powerItem.OnItemCompleted();
            _abilityActive = false;
        }
         */

        /*
         ================
         Spawn Handler
         ================
         
        private void OnSpawn() {
            _playerCamera.CameraSetUp(this.transform);
            _radar.StartRadar();
            _isAlive = true;
            _rigidbody.useGravity = false;
        }
        */

        #endregion
    }
}