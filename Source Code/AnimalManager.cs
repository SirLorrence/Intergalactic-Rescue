using System.Collections;
using System.Collections.Generic;
using AlienGame.Backend;
using AlienGame.Runtime.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;


namespace AlienGame.Runtime.Managers {
    public class AnimalManager : MonoBehaviour {
        #region Fields

        [Header("Settings")]
        [SerializeField] private float _delayToRespawn;
        [SerializeField] private List<AnimalSpawner> _spawners = new List<AnimalSpawner>();

        private WaitForSeconds _respawnDelay;
        private int _currentAnimalCount;
        [SerializeField] private bool _debug;
        private DebugLogger _log = new DebugLogger("[ANIMAL-MANAGER]");

        #endregion

        #region Unity Messages

        private void Awake() {
            //_spawnPoints = GameObject.FindGameObjectsWithTag("animalSpawnPoint");
            _respawnDelay = new WaitForSeconds(_delayToRespawn);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            _log.IsEnabled = _debug;
        }
#endif

        #endregion

        #region Start and End Game

        public void StartManager() {
            StartCoroutine(Respawn());

            //animalspawns.add(_spawnpoints[0].transform.position, 0f);
            //for (int index = 1; index < _spawnpoints.length; index++) {
            //    animalspawns.add(_spawnpoints[index].transform.position, time.time + timetorespawn);
            //StartCoroutine(ChangeWave());
        }

        public void EndManager() {
            _spawners.Clear();
            // animalSpawns.Clear();
            StopAllCoroutines();

            //StopCoroutine(ChangeWave());
            //_wave = 0;
            //_timePassedInWave = 0;
        }

        #endregion

        #region Spawning Process

        public void AddToList(AnimalSpawner spawner) {
            _spawners.Add(spawner);
        }

        private IEnumerator Respawn() {
            _log.Message("Respawn started...");
            while (true) {
                GameObject animal = ObjectPool.Instance.GetObjectFromPool("Animal");
                if (animal != null) {
                    yield return _respawnDelay;
                    if (AvailableSpawners() != 0) {
                        Vector3 spawnLocation = GetSpawnLocation();
                        Spawn(spawnLocation, animal);
                    }
                }
                else {
                    _log.Error("No Animal found in Object Pool");
                }

                yield return null;
                //Vector3 spawnPosition = GetSpawnLocation();

                /*
                 IF any animals are avaible in the pool
                    get spawn poition 
                 
                 */


                //if ((useRespawn == false) && (spawnPosition == new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue))) {
                //    StopCoroutine(Respawn());
                //    break;
                //}

                //if (spawnPosition != new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue)) {
                //Spawn(spawnPosition);
                //}

                //yield return _respawnDelay;
            }
        }

        private int AvailableSpawners() {
            int count = 0;
            foreach (var spawner in _spawners) {
                if (!spawner.IsOccupied)
                    count++;
            }

            return count;
        }

        private Vector3 GetSpawnLocation() {
            List<Vector3> points = new List<Vector3>();

            foreach (var spawner in _spawners) {
                if (!spawner.IsOccupied)
                    points.Add(spawner.Point);
            }

            if (points.Count <= 1) {
                _log.Message("No points");
            }

            return points[Random.Range(0, points.Count)];

            //if (_currentAnimalCount >= maxAnimalCount)
            //    return new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);

            //foreach (KeyValuePair<Vector3, float> entry in animalSpawns)
            //{
            //    if (entry.Value < Time.time)
            //    {
            //        return entry.Key;
            //    }
            //}
            //return new Vector3(Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
        }

        #endregion

        #region Spawn and Collection

        public void Spawn(Vector3 position, GameObject animalObject) {
            _log.Message("Spawning Animal");
            animalObject.transform.position = position;
            animalObject.SetActive(true);

            //_currentAnimalCount++;


            //animalSpawns.Remove(position);
            //SetAnimal(animal);


            //GameObject animal = ObjectPool.Instance.GetObjectFromPool("Animal");
            //if (animal == null) {
            //    Log.Error("No Animal found in Object Pool");
            //    return;
            //}
        }

        //TODO
        //Set up a method on animals to pass the originalPosition from it to here when it is collected to enter the spawn back into the Dictionary
        //public void AnimalCollected(Vector3 emptySpawn) {
        //    if (useRespawn == true) {
        //        animalSpawns.Add(emptySpawn, Time.time + timeToRespawn);
        //        _currentAnimalCount--;
        //    }
        //}

        #endregion

        #region Set Animals and Waves

        //TODO
        //Set up a method on animals to tell it what animal it is
        private void SetAnimal(GameObject animal) {
            //float animalResult = UnityEngine.Random.Range(0, 1 + animalWaveInfo[_wave].largeAnimalChance + animalWaveInfo[_wave].mediumAnimalChance + animalWaveInfo[_wave].smallAnimalChance);


            //if (animalResult <= animalWaveInfo[_wave].smallAnimalChance)
            //{
            //    animal.GetComponent<MeshFilter>().mesh = smallAnimalMesh;
            //    animal.GetComponent<Renderer>().material = smallAnimalMaterial;
            //}
            //else if (animalResult <= animalWaveInfo[_wave].mediumAnimalChance + animalWaveInfo[_wave].smallAnimalChance)
            //{
            //    animal.GetComponent<MeshFilter>().mesh = mediumAnimalMesh;
            //    animal.GetComponent<Renderer>().material = mediumAnimalMaterial;
            //}
            //else if (animalResult <= animalWaveInfo[_wave].largeAnimalChance + animalWaveInfo[_wave].mediumAnimalChance + animalWaveInfo[_wave].smallAnimalChance)
            //{
            //    animal.GetComponent<MeshFilter>().mesh = largeAnimalMesh;
            //    animal.GetComponent<Renderer>().material = largeAnimalMaterial;
            //}
        }

        //private IEnumerator ChangeWave()
        //{
        //    while (true)
        //    {
        //        //_timePassedInWave += 1;
        //        //if(_timePassedInWave >= animalWaveInfo[_wave].waveDuration)
        //        //{
        //        //    _wave++;
        //        //    _timePassedInWave = 0;
        //        //}
        //        //if (_wave >= animalWaveInfo.Length)
        //        //    StopCoroutine(ChangeWave());
        //        //yield return waveDelay;
        //    }

        //}

        #endregion

        #region Deprecated Code

        // public bool _useRespawn = false;
        //public float timeToRespawn;
        // public int maxAnimalCount;
        //public AnimalWaveInformation[] animalWaveInfo;
        //[Header("Assignable")]
        //public Mesh smallAnimalMesh;
        //public Mesh mediumAnimalMesh;
        //public Mesh largeAnimalMesh;
        //public Material smallAnimalMaterial;
        //public Material mediumAnimalMaterial;
        //public Material largeAnimalMaterial;
        //private WaitForSeconds waveDelay = new WaitForSeconds(1);
        //private int _wave = 0;
        //private int _timePassedInWave;
        //private GameObject[] _spawnPoints;
        // private Dictionary<Vector3, float> animalSpawns = new Dictionary<Vector3, float>(); // Vector 3, Float

        #endregion
    }

    #region Structs

    // [Serializable]
    // public struct AnimalWaveInformation {
    //     public int waveDuration;
    //     public int smallAnimalChance;
    //     public int mediumAnimalChance;
    //     public int largeAnimalChance;
    // }

    #endregion
}