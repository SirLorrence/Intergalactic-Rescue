using System.Collections;
using AlienGame.Runtime.Managers;
using UnityEngine;

namespace AlienGame.Runtime.Gameplay.AI {
    public class AIAgent : MonoBehaviour {
        //[SerializeField] private bool isDead;
        // [ReadOnlyInspector] [SerializeField] private bool _hasIncreased = false;

        #region Fields

        private GameObject _body;
        private GameObject _engineVFX;
        private GameObject _explosionVFX;

        private AIMovement _aiMovement;
        private EnemyManager _enemyManager;

        #endregion

        #region Mono Functions

        private void Awake() {
            _body = transform.Find("ShipBody").gameObject;
            _engineVFX = transform.Find("Exhaust").gameObject;
            _explosionVFX = transform.Find("Explosion").gameObject;
            _aiMovement = GetComponent<AIMovement>();
        }

        void Start() {
            _enemyManager = FindObjectOfType<EnemyManager>();
        }

        // private void OnEnable() {
        //     StartCoroutine(SpawnShip());
        // }

        private void OnCollisionEnter(Collision other) {
            if (!other.collider.CompareTag("Player"))
                StartCoroutine(DeathSequence());
        }

        #endregion

        #region Spawn Handler

        public void SpawnShip() => StartCoroutine(OnSpawn());
        private IEnumerator OnSpawn() {
            var mesh = _body.GetComponent<MeshRenderer>();
            _aiMovement.Collider.enabled = false;
            var counter = 0;
            while (true) {
                mesh.enabled = !mesh.enabled;
                yield return new WaitForSeconds(.25f);
                counter += 1;
                if (counter >= 5)
                    break;
                yield return null;
            }
            mesh.enabled = true;
            _aiMovement.Collider.enabled = true;
        }

        #endregion

        #region Death Handlers

        private IEnumerator DeathSequence() {
            OnDeath();
            var explosionEmition = _explosionVFX.GetComponent<ParticleSystem>();
            while (explosionEmition.isEmitting) {
                yield return null;
            }

            OnReset();
            if (_enemyManager != null) _enemyManager.EnemyDeath(this.gameObject);
            else gameObject.SetActive(false);
        }

        private void OnDeath() {
            //Play effect
            _explosionVFX.SetActive(true);
            _engineVFX.SetActive(false);
            AudioManager.Instance.PlayAudio(AudioTypes.SFX_ENEMY_EXPLOSION);
            _body.SetActive(false);
            _aiMovement.enabled = false;
            //add to score
            GameManager.Instance.IncreaseEnemyDestroyedCount();
        }


        private void OnReset() {
            _body.SetActive(true);
            _engineVFX.SetActive(true);
            _explosionVFX.SetActive(false);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _aiMovement.enabled = true;

            //isDead = false;
        }

        #endregion


        // private void Update() {
        //    var timer = Mathf.FloorToInt(_enemyManager.TimeInGame);
        //     if (timer % 10 == 0 && timer != 0) {
        //         if (!hasIncreased) {
        //             _aiMovement.IncreaseSpeed();
        //             hasIncreased = true;
        //         }
        //     }
        //     else hasIncreased = false;
        // }
    }
}