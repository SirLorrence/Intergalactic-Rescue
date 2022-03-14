using AlienGame.Backend;
using AlienGame.Runtime.Gameplay.AI;
using UnityEngine;

namespace AlienGame.Runtime.Gameplay {
    public class PlayerGravityBeam : MonoBehaviour {
        #region Fields

        [SerializeField] private GameObject _holdingContainer;
        [SerializeField] private float _beamRadius = 5;
        private RaycastHit _hit;
        private const string AnimalTagName = "Animal";

        private DebugLogger _beamLog = new DebugLogger("[GRAV BEAM]");
        [SerializeField] private bool _debug;

        //[SerializeField] private List<AnimalAI> _storedAnimals = new List<AnimalAI>();

        #endregion

#if UNITY_EDITOR
        private void OnValidate() {
            _beamLog.IsEnabled = _debug;
        }

        private void OnDrawGizmos() {
            if (_debug) {
                if (Physics.SphereCast(transform.position, _beamRadius, Vector3.down, out _hit)) {
                    Gizmos.DrawRay(transform.position, Vector3.down * _hit.distance);
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireSphere(_hit.point, _beamRadius);
                }
            }
        }
#endif

        private void Awake() {
            _beamRadius = transform.position.y / 2;
        }

        private void Update() {
            if (Physics.SphereCast(transform.position, _beamRadius, Vector3.down, out _hit)) {
                _beamLog.Message($"Name: {_hit.transform.name}, Tag: {_hit.transform.tag}");
                if (_hit.transform.CompareTag(AnimalTagName)) {
                    var animAI = _hit.transform.GetComponent<AnimalAI>();
                    animAI.Extraction(transform);
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag(AnimalTagName)) {
                other.gameObject.GetComponent<AnimalAI>().Extracted();
                //_storedAnimals.Add(other.gameObject.GetComponent<AnimalAI>());
                //other.transform.position = holdingContainer.transform.position;
                //other.transform.parent = holdingContainer.transform;
            }
            //if(other.CompareTag("DropArea")){
            //    foreach (var animal in _storedAnimals){
            //        animal.Drop(other.transform);
            //        _storedAnimals.Remove(animal);
            //    }
            //}
        }
    }
}