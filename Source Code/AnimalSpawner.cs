using UnityEngine;
using AlienGame.Runtime.Managers;
using AlienGame.Backend;

namespace AlienGame.Runtime.Gameplay {
    public class AnimalSpawner : MonoBehaviour {
        #region Fields/Properties

        [ReadOnlyInspector] [SerializeField] private bool _isOccupied;
        private Vector3 _point;
        private BoxCollider _box;
        private AnimalManager _animalManager;
        private DebugLogger _log = new DebugLogger("[Animal Spawner]");
        [SerializeField] private bool _debug;

        public Vector3 Point => _point;
        public bool IsOccupied => _isOccupied;

        #endregion

#if UNITY_EDITOR
        private void OnValidate() {
            _log.IsEnabled = _debug;
        }
#endif
        void Start() {
            _box = GetComponent<BoxCollider>();
            _animalManager = FindObjectOfType<AnimalManager>();
            _point = transform.Find("Spawn Point").transform.position;
            if (_animalManager != null)
                _animalManager.AddToList(this);
            else
                _log.Error("Animal manager not found");
        }

        private void OnTriggerStay(Collider other) {
            if (other.CompareTag("Animal"))
                _isOccupied = true;

            _log.Message($"Tag Name: {other.tag}");
        }

        private void OnTriggerExit(Collider other) {
            _isOccupied = false;
        }
    }
}