//================
// Deprecated Code - doesn't really effect gameplay & know well designed
//================
//
//
// using System;
// using System.Collections;
// using System.Linq;
// using AlienGame.Backend;
// using UnityEngine;
//
// namespace AlienGame.Runtime.Gameplay {
//     public class PlayerRadar : MonoBehaviour {
//         #region Fields
//
//         [Header("Radar Settings")] [SerializeField]
//         private GameObject _arrowPrefab;
//
//         [Tooltip("The distance needed to be picked up by the radar")] [SerializeField]
//         private float _detectionRadius = 100;
//
//         [Tooltip("The distance for the arrow is place away from the ship")] [SerializeField]
//         private float _arrowFromShipRatio = 10;
//
//         [Range(0, 5)] [SerializeField] private int _arrowCount = 1;
//         [SerializeField] private LayerMask _enemyLayer;
//
//         private WaitForSeconds _delayTimer = new WaitForSeconds(0.1f);
//         private GameObject[] _arrows;
//         [SerializeField]private Collider[] _nearbyEnemies;
//
//         [SerializeField] private bool _debug;
//         private DebugLogger _log = new DebugLogger("[ENEMY_RADAR]");
//
//         #endregion
//
//         private void Awake() => CreateArrows();
//
// #if UNITY_EDITOR
//         private void OnValidate() {
//             _log.IsEnabled = _debug;
//         }
//
//         private void OnDrawGizmos() {
//             if(_debug) Gizmos.DrawWireSphere(transform.position, _detectionRadius);
//         }
// #endif
//
//         #region Radar Controllers
//
//         public void StartRadar() {
//             _log.Message("Started Enemy Radar");
//             StartCoroutine(UpdateArrows());
//         }
//         
//         public void StopRadar() {
//             _log.Message("Stopped Enemy Radar");
//             StopAllCoroutines();
//         }
//
//         #endregion
//
//         #region Update
//
//         private IEnumerator UpdateArrows() {
//             while (true) {
//                 _nearbyEnemies = GetNearbyEnemies();
//                 _log.Message($"Number of Objects {_nearbyEnemies.Length}");
//                 if (_nearbyEnemies.Length != 0) SetArrows(_nearbyEnemies);
//                 else {
//                     _log.Message("No Enemies");
//                     _arrows.ToList().ForEach(arrow => arrow.SetActive(false));
//                 }
//         
//                 yield return null;
//             }
//         }
//
//         #endregion
//
//         #region Arrow Management
//
//         private void CreateArrows() {
//             _arrows = new GameObject[_arrowCount];
//             for (int i = 0; i < _arrowCount; i++) {
//                 var arrow = Instantiate(_arrowPrefab, transform);
//                 _arrows[i] = arrow;
//                 arrow.SetActive(false);
//                 arrow.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, _arrowFromShipRatio);
//             }
//         }
//
//         private void SetArrows(Collider[] objectsToLookAt) {
//             for (int i = 0; i < _arrows.Length; i++) {
//                 if (i >= objectsToLookAt.Length) _arrows[i].SetActive(false);
//                 else SetActiveArrow(i, objectsToLookAt[i]);
//             }
//         }
//
//         private void SetActiveArrow(int index, Collider objectToLookAt) {
//             var arrow = _arrows[index];
//             arrow.SetActive(true);
//             arrow.transform.position = transform.position;
//             arrow.transform.LookAt(objectToLookAt.transform.position);
//         }
//
//         #endregion
//
//         #region Enemy Management
//
//         private Collider[] GetNearbyEnemies() {
//             Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, _detectionRadius, _enemyLayer);
//
//             nearbyEnemies = nearbyEnemies.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude)
//                 .ToArray();
//             nearbyEnemies = nearbyEnemies.Where(x => x.GetComponent<Renderer>().isVisible == false).ToArray();
//             return nearbyEnemies;
//         }
//
//         #endregion
//     }
// }