// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class Enemy : Character
// {
//     private Transform player; // Reference to the player
//     private Vector2 targetPosition; // Target position to move towards
//     private List<Vector2> path = new List<Vector2>(); // List of waypoints to follow
//     public LayerMask playerLayer;
//     public int attackDamage;

//     // Grid size and other A* parameters
//     public float gridSize = 1f; // Size of each grid cell in Unity units
//     public float attackRange = 1f; // Distance within which the enemy can attack the player
//     public float attackCooldown = 1f; // Cooldown time between attacks
//     private float lastAttackTime; // Time since last attack

//     public float pathUpdateInterval = 1f; // Interval to update path to player's position

//     private Vector2 lastPlayerPosition; // To track the last known position of the player
//     private Coroutine pathUpdateCoroutine;

//     protected override void Start()
//     {
//         base.Start();
//         characterName = "Enemy"; // Set name for enemy
//         moveSpeed = 1f;
//         lastAttackTime = -attackCooldown; // Ensure the enemy can attack immediately if in range

//         // Find the player in the scene by name or by type
//         GameObject playerObject = GameObject.Find("Player"); // Replace "Player" with the exact name of your player GameObject
//         if (playerObject != null)
//         {
//             player = playerObject.transform; // Assign the Transform component of the Player
//             targetPosition = player.position; // Set initial target to player's position
//             lastPlayerPosition = player.position; // Initialize the last known position of the player
//         }
//         else
//         {
//             Debug.LogError("Player GameObject not found in the scene.");
//         }

//         // Start coroutine to periodically update path
//         pathUpdateCoroutine = StartCoroutine(UpdatePathPeriodically());
//     }

//     private IEnumerator UpdatePathPeriodically()
//     {
//         while (true)
//         {
//             if (player != null)
//             {
//                 StartCoroutine(FindPath());
//             }
//             yield return new WaitForSeconds(pathUpdateInterval); // Update path every interval
//         }
//     }

//     protected override void Update()
//     {
//         base.Update();

//         if (path.Count > 0)
//         {
//             MoveAlongPath();
//         }
//     }

//     private IEnumerator FindPath()
//     {
//         path.Clear(); // Clear previous path

//         Vector2 startPos = transform.position;
//         Vector2 endPos = player.position;

//         // Generate a simple straight path (replace this with A* if needed)
//         path.Add(startPos);
//         path.Add(endPos);

//         yield return null; // Simulate delay
//     }

//     private void MoveAlongPath()
//     {
//         if (path.Count > 0)
//         {
//             Vector2 target = path[0];
//             transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
//             m_animator.SetInteger("AnimState", 1);

//             // Check if the enemy needs to flip based on the player's position
//             if (target.x < transform.position.x)
//             {
//                 transform.rotation = Quaternion.Euler(0, 180, 0); // Rotasi 180 derajat di sumbu Y
//             }
//             else
//             {
//                 transform.rotation = Quaternion.Euler(0, 0, 0); // Rotasi kembali ke sumbu semula
//             }

//             if (Vector2.Distance(transform.position, player.position) <= attackRange)
//             {
//                 if (Time.time - lastAttackTime >= attackCooldown)
//                 {
//                     AttackPlayer();
//                     lastAttackTime = Time.time;
//                 }
//             }
//             else if (Vector2.Distance(transform.position, target) < 0.1f)
//             {
//                 path.RemoveAt(0); // Remove the current waypoint from the path
//             }
//         }
//     }


//     private void AttackPlayer()
//     {
//         m_animator.SetTrigger("Attack");

//         Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
//         if (playerCollider != null)
//         {
//             Character playerCharacter = playerCollider.GetComponent<Character>();
//             if (playerCharacter != null)
//             {
//                 playerCharacter.TakeDamage(attackDamage); // Apply damage to player
//             }
//         }
//     }

//     public override void TakeDamage(int damage)
//     {
//         base.TakeDamage(damage);

//         if (!m_isDead)
//         {
//             m_animator.SetTrigger("Hurt");
//         }
//         else
//         {
//             StartCoroutine(HandleDeath());
//         }
//     }

//     private IEnumerator HandleDeath()
//     {
//         m_animator.SetTrigger("Death");
//         yield return new WaitForSeconds(m_animator.GetCurrentAnimatorStateInfo(0).length);
//         Destroy(gameObject);
//     }

//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
//     }
// }
