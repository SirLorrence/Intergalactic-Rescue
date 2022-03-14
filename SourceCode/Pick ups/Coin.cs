// using AlienGame.Runtime.Managers;
// using UnityEngine;
//
// namespace AlienGame.Runtime.Gameplay.Pick_ups {
//     public class Coin : Item {
//         private int _value = 1;
//         public override ItemType itemType => ItemType.Coin;
//         public override void OnItemActive() => GameManager.Instance.AddCoins(_value);
//
//         public override void OnTriggerEnter(Collider other) {
//             if (other.CompareTag("Player")) {
//                 OnItemActive();
//                 StopAllCoroutines();
//                 Destroy(gameObject);
//             }
//         }
//     }
// }