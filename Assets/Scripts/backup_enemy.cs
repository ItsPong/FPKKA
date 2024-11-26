// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Newtonsoft.Json;
// using UnityEditor.Scripting.Python;

// public class Enemy : Character
// {
//     private Transform player;
//     private Vector2 targetPosition;
//     private List<Vector2> path = new List<Vector2>();
//     public LayerMask playerLayer;
//     public int attackDamage = 2;
//     public float attackRange = 1f;
//     public float attackCooldown = 5f;
//     private float attackCooldownTimer = 0f;
//     public float pathUpdateInterval = 1f;
//     private Vector2 lastPlayerPosition;
//     private Coroutine pathUpdateCoroutine;
//     public float gridSize = 0.01f;
//     private int gridWidth;
//     private int gridHeight;
//     public float minX = -24f;
//     public float maxX = 24f;
//     public float minY = -15f;
//     public float maxY = 14f;
//     private bool counted = false;

//     private static bool isPythonRunning = false;

//     protected override void Start()
//     {
//         base.Start();
//         characterName = "Enemy";
//         moveSpeed = 1f;
//         gridWidth = Mathf.RoundToInt((maxX - minX) / gridSize);
//         gridHeight = Mathf.RoundToInt((maxY - minY) / gridSize);

//         GameObject playerObject = GameObject.Find("Player");
//         if (playerObject != null)
//         {
//             player = playerObject.transform;
//             targetPosition = player.position;
//             lastPlayerPosition = player.position;
//         }

//         pathUpdateCoroutine = StartCoroutine(UpdatePathPeriodically());
//     }

//     protected override void Update()
//     {
//         base.Update();

//         if (path.Count > 0)
//         {
//             MoveAlongPath();
//         }

//         // Reduce the cooldown timer over time
//         if (attackCooldownTimer > 0)
//         {
//             attackCooldownTimer -= Time.deltaTime;
//         }

//         if (attackCooldownTimer <= 0)
//         {
//             AttemptAttack();
//         }
//     }

//     private Vector2Int WorldToGrid(Vector2 worldPosition)
//     {
//         int col = Mathf.RoundToInt((worldPosition.x - minX) / gridSize);
//         int row = Mathf.RoundToInt((worldPosition.y - minY) / gridSize);
//         return new Vector2Int(col, row);
//     }

//     private Vector2 GridToWorld(Vector2Int gridPosition)
//     {
//         float x = minX + gridPosition.x * gridSize;
//         float y = minY + gridPosition.y * gridSize;
//         return new Vector2(x, y);
//     }

//     private IEnumerator UpdatePathPeriodically()
//     {
//         while (true)
//         {
//             if (player != null && Vector2.Distance(transform.position, player.position) > attackRange)
//             {
//                 yield return StartCoroutine(FindPath());
//             }
//             yield return new WaitForSeconds(pathUpdateInterval);
//         }
//     }

//     private IEnumerator FindPath()
//     {
//         path.Clear();
        
//         Vector2Int startGridPos = WorldToGrid(transform.position); 
//         Vector2Int goalGridPos = WorldToGrid(new Vector2(player.position.x, player.position.y-1));     

//         string start = $"({startGridPos.x}, {startGridPos.y})";
//         string goal = $"({goalGridPos.x}, {goalGridPos.y})";

//         string gridJson = JsonConvert.SerializeObject(GetGrid());
//         string pythonModulePath = Application.dataPath + "/Scripts"; 
//         string resultFilePath = Application.dataPath + $"/path_result_{GetInstanceID()}.json";

//         while (isPythonRunning)
//         {
//             yield return null;
//         }

//         isPythonRunning = true;

//         string pythonCode = $@"
// import sys
// import json
// sys.path.append(r'{pythonModulePath.Replace("\\", "\\\\")}')
// from astar_pathfinding import find_path

// start = {start}
// goal = {goal}
// grid = {gridJson}

// path = find_path(start, goal, grid)

// with open(r'{resultFilePath.Replace("\\", "\\\\")}', 'w') as f:
//     json.dump(path, f)
// ";
//         isPythonRunning = false;

//         PythonRunner.RunString(pythonCode);

//         yield return new WaitForSeconds(0.1f);

//         if (System.IO.File.Exists(resultFilePath))
//         {
//             string pathJson = System.IO.File.ReadAllText(resultFilePath);
//             OnPathReceived(pathJson);
//         }
//     }

//     void OnPathReceived(string jsonPath)
//     {
//         string processedJson = jsonPath.Replace("\\\"", "\"").Trim('"');
        
//         Debug.Log("Processed JSON Path: " + processedJson);

//         try
//         {
//             List<Vector2Int> gridPath = JsonConvert.DeserializeObject<List<Vector2Int>>(processedJson);
            
//             foreach (var point in gridPath)
//             {
//                 Vector2 worldPoint = GridToWorld(point);
//                 path.Add(worldPoint);
//                 Debug.Log("Path Point: " + worldPoint);
//             }
//         }
//         catch (JsonSerializationException e)
//         {
//             Debug.LogError("JSON deserialization failed: " + e.Message);
//         }
//     }

//     private void MoveAlongPath()
//     {
//         if (path.Count > 0)
//         {
//             Vector2 target = path[0];
//             transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
//             m_animator.SetInteger("AnimState", 1);

//             if (target.x < transform.position.x)
//             {
//                 transform.rotation = Quaternion.Euler(0, 180, 0); 
//             }
//             else
//             {
//                 transform.rotation = Quaternion.Euler(0, 0, 0); 
//             }

//             if (Vector2.Distance(transform.position, player.position) <= attackRange)
//             {
//                 AttemptAttack();
//             }
//             else if (Vector2.Distance(transform.position, target) < 0.1f)
//             {
//                 path.RemoveAt(0); 
//             }
//         }
//     }

//     private int[,] GetGrid()
//     {
//         int[,] grid = new int[gridWidth, gridHeight];
//         for (int i = 0; i < gridWidth; i++)
//         {
//             for (int j = 0; j < gridHeight; j++)
//             {
//                 grid[i, j] = 0;
//             }
//         }
//         return grid;
//     }

//     private void AttemptAttack()
//     {
//         if (attackCooldownTimer <= 0)
//         {
//             Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
//             if (playerCollider != null)
//             {
//                 AttackPlayer(playerCollider);
//                 attackCooldownTimer = attackCooldown;
//             }
//         }
//     }

//     private void AttackPlayer(Collider2D playerCollider)
//     {
//         m_animator.SetTrigger("Attack");

//         Character playerCharacter = playerCollider.GetComponent<Character>();
//         playerCharacter?.TakeDamage(attackDamage);
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
//             if (!counted) {
//                 killCount++;
//                 counted = true;
//             }
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
