using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlienGame.Backend;
using AlienGame.Runtime.Gameplay.AI;

namespace AlienGame.Runtime.Managers {
    public class EnemyManager : MonoBehaviour {
        #region Fields

        [Header("Settings")] [SerializeField] int _numberOfSpawnPoints = 8;

        //[Header("Custom Wave Lengths")] 
        //public WaveInformation[] waveInformation;
        private LayerMask _environmentLayer;

        public Action<GameObject> OnEnemyDeath;


        private int _enemyCount;

        //public int _waveNumber;
        [ReadOnlyInspector] [SerializeField] private float _timeInGame;

        [SerializeField] private int _maxDistanceFromPlayer = 75;
        // private int _minDistanceFromPlayer = 20;
        private int _sqrMinDistanceFromPlayer;

        private WaitForSeconds _delay = new WaitForSeconds(3f);
        private WaitForSeconds _initialDelay = new WaitForSeconds(2.5f);

        [ReadOnlyInspector] [SerializeField] private Transform playerPosition;

        // public float TimeInGame {
        //     get => _timeInGame;
        // }

        [Header("Debugging")] [SerializeField] private bool debug = false;
        private DebugLogger Log = new DebugLogger("[ENEMYMANAGER]");

        #endregion


    #if UNITY_EDITOR
        private void OnValidate() {
            Log.IsEnabled = debug;
        }

        private void OnDrawGizmos() {
            if (debug) {
                Gizmos.color = Color.red;
                if (playerPosition != null) {
                    Gizmos.DrawWireSphere(playerPosition.position, _maxDistanceFromPlayer);

                    //circle circumference formula
                    for (int i = 0; i < _numberOfSpawnPoints; i++) {
                        float angle = i            * (2 * (float) Math.PI / _numberOfSpawnPoints);
                        float x = Mathf.Cos(angle) * _maxDistanceFromPlayer;
                        float z = MathF.Sin(angle) * _maxDistanceFromPlayer;
                        Vector3 temp;
                        temp.x = playerPosition.position.x + x;
                        temp.y = playerPosition.position.y;
                        temp.z = playerPosition.position.z + z;
                        Gizmos.color = (Physics.CheckSphere(temp, 2f, _environmentLayer)) ? Color.gray : Color.green;
                        Gizmos.DrawSphere(temp, 2f);
                    }
                }
            }
        }
    #endif

        #region Updates

        public void Awake() {
            _environmentLayer = LayerMask.GetMask("Environment");
            //_sqrMinDistanceFromPlayer = _minDistanceFromPlayer * _minDistanceFromPlayer;
        }

        private void Start() {
            if (debug) {
                playerPosition = GameManager.Instance.PlayerTransform;
                StartEnemyManager();
            }
        }

        private void OnEnable() {
            OnEnemyDeath += EnemyDeath;
        }

        private void OnDisable() {
            OnEnemyDeath -= EnemyDeath;
        }


        public void StartEnemyManager() {
            if (playerPosition == null) playerPosition = GameManager.Instance.PlayerTransform;
            Log.Message("Started Enemy Manager");
            ResetStats();
            StartUpdates();
        }

        private void StartUpdates() {
            StartCoroutine(SpawnEnemyUpdate());
            StartCoroutine(TimerUpdate());
            //StartCoroutine(CheckDifficultyUpdate());
        }

        private void ResetStats() {
            _enemyCount = 0;
            //_waveNumber = 0;
            _timeInGame = 0;
        }

        public void StopEnemyManager() {
            Log.Message("Stopped Enemy Manager");
            StopAllCoroutines();
        }

        #endregion

        #region Spawning and Managing Enemies

        //  Todo 
        //Interact With enemy to set diffuculty
        private IEnumerator SpawnEnemyUpdate() {
            yield return _initialDelay;

            while (true) {
                if (CanSpawnEnemy()) {
                    SpawnEnemy();
                }

                yield return _delay;
            }

            //yield return null;
        }

        private IEnumerator TimerUpdate() {
            while (true) {
                _timeInGame += Time.deltaTime;
                yield return null;
            }
        }
        //private IEnumerator CheckDifficultyUpdate()
        //{
        //    yield return _initialDelay;

        //    while (true)
        //    {
        //        if (IsWaveCompleted())
        //        {
        //            IncreaseWaveDifficulty();
        //        }
        //        yield return _delay;
        //    }

        //    yield return null;
        //}

        public void EnemyDeath(GameObject enemy) {
            _enemyCount--;
            ObjectPool.Instance.SetObjectInPool(enemy);
            Log.Message($"Enemy died, {_enemyCount} ");
        }

        public bool CanSpawnEnemy() => _enemyCount < 10;

        public void SpawnEnemy() {
            GameObject enemy = ObjectPool.Instance.GetObjectFromPool("Enemies"); //hard coded but whatever

            if (enemy == null) {
                Log.Message("No Enemy found in Object Pool");
                return;
            }

            enemy.transform.position = GetSpawnPoints();
            //enemy.transform.position = GetSpawnpoint();
            enemy.SetActive(true);
            enemy.GetComponent<AIAgent>().SpawnShip();
            _enemyCount++;
            Log.Message($"Enemy Spawned, {_enemyCount} ");
        }

        //public Vector3 GetSpawnpoint() {
        //    var checkpointsInRange = Physics.OverlapSphere(playerTrans.position, _maxDistanceFromPlayer);
        //    //var checkpointsInRange = Physics.OverlapSphere(GameManager.Instance.playerTransform.position, _maxDistanceFromPlayer, spawnpointMask);
        //    List<Transform> checkpointsToSpawn = new List<Transform>();

        //    foreach (var checkpoint in checkpointsInRange) {
        //        if (checkpoint.transform.position.sqrMagnitude > _sqrMinDistanceFromPlayer)
        //            checkpointsToSpawn.Add(checkpoint.transform);
        //    }

        //    if (checkpointsToSpawn.Count == 0) {
        //        Log.Error($"No checkpoints in range at {GameManager.Instance.playerTransform.position} ");
        //        return Vector3.zero;
        //    }

        //    return checkpointsToSpawn[UnityEngine.Random.Range(0, checkpointsToSpawn.Count)].position;
        //}

        private Vector3 GetSpawnPoints() {
            List<Vector3> avaiblePoints = new List<Vector3>();
            for (int i = 0; i < _numberOfSpawnPoints; i++) {
                //circle circumference formula
                float angle = i            * (2 * (float) Mathf.PI / _numberOfSpawnPoints);
                float x = Mathf.Cos(angle) * _maxDistanceFromPlayer;
                float z = Mathf.Sin(angle) * _maxDistanceFromPlayer;

                Vector3 spawnPoint;
                spawnPoint.x = playerPosition.position.x + x;
                spawnPoint.y = playerPosition.position.y + 2f;
                spawnPoint.z = playerPosition.position.z + z;

                // only add it the an avaible points if is not being blocked by anything
                if (!Physics.CheckSphere(spawnPoint, 2f, _environmentLayer))
                    avaiblePoints.Add(spawnPoint);
            }

            return avaiblePoints[UnityEngine.Random.Range(0, avaiblePoints.Count)];
        }

        //public bool IsWaveCompleted() => _timeSinceStartOfGame + waveInformation[_waveNumber].waveDuration < Time.time;
        //public void IncreaseWaveDifficulty()
        //{
        //    _waveNumber++;

        //    Log.Message($"Wave Increased, Current is {_waveNumber}");
        //}

        #endregion
    }

    #region Structs

    [Serializable]
    public struct WaveInformation {
        public int waveDuration;
        public int maxEnemies;
    }

    #endregion
}