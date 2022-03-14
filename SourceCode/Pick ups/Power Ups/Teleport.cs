// using UnityEngine;
//
// namespace AlienGame.Runtime.Gameplay.Pick_ups.Power_Ups {
//     public class Teleport : Item {
//         public int minMapX = -100;
//         public int minMapZ = -100;
//         public int maxMapX = 100;
//         public int maxMapZ = 100;
//         public float teleportHeight;
//
//         public override ItemType itemType => ItemType.Teleport;
//
//         public override void OnItemActive() {
//         }
//
//         public override void OnItemCompleted() {
//             Vector3 position = PickDestination();
//             player.transform.position = position;
//             //EnemyManager.Instance.DespawnAll();
//         }
//
//         private Vector3 PickDestination() {
//             System.Random rand = new System.Random();
//             for (int teleportAttempt = 0; teleportAttempt < 100; teleportAttempt++) {
//                 Vector3 position =
//                     new Vector3(rand.Next(minMapX, maxMapX), teleportHeight, rand.Next(minMapZ, maxMapZ));
//                 if (CheckCollision(position)) {
//                     return position;
//                 }
//             }
//
//             return Vector3.zero;
//         }
//
//         private bool CheckCollision(Vector3 position) {
//             int amountOfHitColliders = Physics.OverlapSphereNonAlloc(position, 2f, null);
//
//             return amountOfHitColliders == 0;
//         }
//     }
// }