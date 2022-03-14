// using UnityEngine;
//
// namespace AlienGame.Runtime.Gameplay.Pick_ups.Power_Ups {
//     public class CoinMagnet : Item {
//         [SerializeField] private float magnetRange;
//         public override ItemType itemType => ItemType.CoinMagnet;
//
//         public override void OnItemActive() {
//             ActivateMagnet();
//         }
//
//         private void ActivateMagnet() {
//             GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
//
//             foreach (GameObject coin in coins) {
//                 if (Vector3.Distance(playerPosition.position, coin.transform.position) < magnetRange)
//                     coin.GetComponent<Item>().AttractOnSpawn = true;
//             }
//         }
//     }
// }