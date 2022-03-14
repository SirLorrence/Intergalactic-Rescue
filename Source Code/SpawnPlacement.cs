// ========================================
// Created by: Laurence Sadler (SirLorrence @ github)
// ========================================

using System.Collections.Generic;
using AlienGame.Backend;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AlienGame.Runtime.Gameplay {
    public class SpawnPlacement : MonoBehaviour {
        #region Fields

        [Header("Settings")] [SerializeField] private int _gridX = 10;
        [SerializeField] private int _gridZ = 10;
        [SerializeField] private int _offset = 100; // spacing 
        [SerializeField] private float _spawnRadiusSize = 20;
        [SerializeField] private int _amountOfObjects = 25;
        [SerializeField] private GameObject _spawnPrefab;
        [ReadOnlyInspector] [SerializeField] private List<Vector3> _spawnLocations;
        [SerializeField] private bool _debug;
        private DebugLogger _log = new DebugLogger("[SPAWN-PLACEMENT]");
        
        private Vector3[,] _locationData;
        private LayerMask _layerMask;
        

        #endregion

#if UNITY_EDITOR
        private void OnValidate() {
            _log.IsEnabled = _debug;
            _layerMask = LayerMask.GetMask("Environment");
        }

        private void OnDrawGizmosSelected() {
            for (int x = 0; x < _gridX; x++) {
                for (int z = 0; z < _gridZ; z++) {
                    Vector3 pos = new Vector3(transform.position.x + x * _offset, 0, z * _offset);
                    Gizmos.color = (Physics.CheckSphere(pos, _spawnRadiusSize, _layerMask)) ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(pos, _spawnRadiusSize);
                }
            }
        }
#endif
        private void Awake() {
            _layerMask = LayerMask.GetMask("Environment");
            _spawnLocations = new List<Vector3>();
        }

        private void Start() {
            GeneratePlacement();
        }

        /*
        1a. Add all points to data array
        1b. Add all available points to List
        2. Get random, index and get that from the list
        3. Find/check the vector 3 position to match with the random point selected
        4. In the surround locations around the point, find and remove theses from the list if not already.
        */
        public void GeneratePlacement() {
            _locationData = new Vector3[_gridX + 1, _gridZ + 1];
            for (int x = 0; x < _gridX; x++) {
                for (int z = 0; z < _gridZ; z++) {
                    Vector3 point = new Vector3(transform.position.x + x * _offset, 0, z * _offset);
                    _locationData[x, z] = point;
                    if (!Physics.CheckSphere(point, _spawnRadiusSize, _layerMask))
                        _spawnLocations.Add(point);
                }
            }

            PlaceObjects();
            _log.Message("Done.....");
            
        }

        void PlaceObjects() {
            // amountOfObjects = Mathf.FloorToInt(_spawnLocations.Count / 3.0f);
            for (int i = 0; i < _amountOfObjects; i++) {
                if (_spawnLocations.Count <= 1)
                    break;
                var randomIndex = Random.Range(0, _spawnLocations.Count);
                var selectedLocation = _spawnLocations[randomIndex];
                // Quaternion randomRotation = new Quaternion(0, Random.rotation.y, 0, 0);
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
                GameObject spawnObject = Instantiate(_spawnPrefab, selectedLocation, randomRotation);
                spawnObject.transform.SetParent(this.transform);
                CheckAndRemovePoints(selectedLocation);
                // _spawnLocations.RemoveAt(randomIndex);
            }
        }

        void CheckAndRemovePoints(Vector3 pointToCompare) {
            for (int x = 0; x < _gridX; x++) {
                for (int z = 0; z < _gridZ; z++) {
                    if (pointToCompare == _locationData[x, z]) {
                        DeleteSurrounding(x, z);
                        break;
                    }
                }
            }
        }

        void DeleteSurrounding(int coor1, int coor2) {
            bool WithinDataSet(int x, int y) {
                return x <= _locationData.GetLength(0) && y <= _locationData.GetLength(1);
            }

            void DeleteFunction(Vector3 point) {
                var index = _spawnLocations.IndexOf(point);
                if (index >= 0)
                    _spawnLocations.RemoveAt(index);
            }

            //(c1 + col,c2 - row)
            for (int col = -1; col <= 1; col++) {
                for (int row = -1; row <= 1; row++) {
                    var pointX = coor1 + col;
                    var pointY = coor2 - row;
                    if (WithinDataSet(pointX, pointY)) {
                        var dataPoint = _locationData[pointX, pointY];
                        if (_spawnLocations.Contains(dataPoint)) {
                            DeleteFunction(dataPoint);
                        }
                    }
                }
            }

            // if (_spawnLocations.Contains(_locationData[(coor1 + 1), (coor2 + 1) - 1])) {
            //     var point = _locationData[(coor1 + 1), (coor2 + 1) - 1];
            //     DeleteFunction(point);
            // }
            // for (int i = 0; i < 2; i++) {
            //     if (_spawnLocations.Contains(_locationData[(coor1 + 1) - i, (coor2 + 1)])) {
            //         var point = _locationData[(coor1 + 1) - i, (coor2 + 1)];
            //         DeleteFunction(point);
            //     }
            //
            //     if (_spawnLocations.Contains(_locationData[(coor1 - 1), (coor2 ) + i])) {
            //         var point = _locationData[(coor1 - 1), (coor2 - 1) + i];
            //         DeleteFunction(point);
            //     }
            // }
            //
            //
            //
            // for (int i = 0; i < 3; i++) {
            //     if (_spawnLocations.Contains(_locationData[(coor1 - 1) + i, (coor2 - 1)])) {
            //         var point = _locationData[(coor1 - 1) + i, (coor2 - 1)];
            //         DeleteFunction(point);
            //     }
            // }
        }
    }
}