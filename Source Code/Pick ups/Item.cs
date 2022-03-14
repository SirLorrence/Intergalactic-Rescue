// using System.Collections;
// using AlienGame.Runtime.Managers;
// using UnityEngine;
//
// namespace AlienGame.Runtime.Gameplay.Pick_ups {
//     public abstract class Item : MonoBehaviour {
//         #region Fields
//
//         [Header("Item settings")] [SerializeField]
//         protected float _abilityTimeLimit;
//
//         [Header("Attraction Settings")] [SerializeField]
//         private float _itemRadius = 5;
//
//         [SerializeField] private float _attractSpeed = 4;
//         [SerializeField] private bool _attractOnSpawn = true;
//
//         [SerializeField] protected Transform playerPosition;
//         [SerializeField] protected Player player;
//         protected GameManager gameManager;
//
//         #endregion
//
//         #region Properties
//
//         public abstract ItemType itemType { get; }
//
//         public bool AttractOnSpawn {
//             get => _attractOnSpawn;
//             set => _attractOnSpawn = value;
//         }
//
//         #endregion
//
//         #region Pass down Methods
//
//         /// <summary>
//         /// This method is called While the items timer is active. It will stop being called when the items timer has reached zero.
//         /// </summary>
//         public abstract void OnItemActive();
//
//         /// <summary>
//         /// This method is called after the items timer has reached zero.
//         /// </summary>
//         public virtual void OnItemCompleted() {
//         }
//
//         #endregion
//
//         private void Start() {
//             gameManager = GameManager.Instance;
//             SetReferences();
//             if (player == null || playerPosition == null) {
//                 gameManager.FindPlayerReferences();
//                 SetReferences();
//             }
//
//             StartCoroutine((_attractOnSpawn) ? AttractToPlayer() : CheckRange());
//         }
//
//
//         private void SetReferences() {
//             player = gameManager.ActivePlayerScript;
//             playerPosition = gameManager.playerTransform;
//         }
//
//         #region Attraction handler
//
//         IEnumerator CheckRange() {
//             while (true) {
//                 if (Vector3.Distance(playerPosition.position, transform.position) < _itemRadius || _attractOnSpawn)
//                     StartCoroutine(AttractToPlayer());
//                 yield return 0;
//             }
//         }
//
//         IEnumerator AttractToPlayer() {
//             StopCoroutine(CheckRange());
//             //if in range, face player and move toward them
//             var position = transform.position;
//             transform.rotation = Quaternion.LookRotation((playerPosition.position - position));
//             position = Vector3.Lerp(position, playerPosition.position, Time.deltaTime * _attractSpeed);
//             transform.position = position;
//             yield return 0;
//         }
//
//         #endregion
//
//         public virtual void OnTriggerEnter(Collider other) {
//             if (other.CompareTag("Player")) {
//                 StopAllCoroutines();
//                 // player.CheckActiveAbility(itemType, _abilityTimeLimit, this);
//                 gameObject.SetActive(false);
//                 // Activate();
//             }
//         }
//
//         private void OnDrawGizmosSelected() {
//             Gizmos.DrawWireSphere(transform.position, _itemRadius);
//         }
//     }
// }