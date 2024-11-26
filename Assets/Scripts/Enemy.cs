using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class Enemy : Character
{
    private Transform player;
    private Vector2 targetPosition;
    private Vector2 lastPlayerPosition;
    private List<Vector2> path = new List<Vector2>();
    public LayerMask playerLayer;
    public int attackDamage = 2;
    public float attackRange = 1f;
    public float attackCooldown = 5f;
    private float attackCooldownTimer = 0f;
    public float pathUpdateInterval = 1f;
    private Coroutine pathUpdateCoroutine;
    public float gridSize = 1f;
    private int gridWidth;
    private int gridHeight;
    public float minX = -24f;
    public float maxX = 24f;
    public float minY = -15f;
    public float maxY = 14f;
    private bool counted = false;
    private bool isPathFinding = false;

    protected override void Start()
    {
        base.Start();
        characterName = "Enemy";
        moveSpeed = 1f;
        gridWidth = Mathf.RoundToInt((maxX - minX) / gridSize);
        gridHeight = Mathf.RoundToInt((maxY - minY) / gridSize);

        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            targetPosition = player.position;
            lastPlayerPosition = player.position;
        }

        pathUpdateCoroutine = StartCoroutine(UpdatePathPeriodically());
    }

    protected override void Update()
    {
        base.Update();

        if (path.Count > 0)
        {
            MoveAlongPath();
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (attackCooldownTimer <= 0)
        {
            AttemptAttack();
        }
    }

    private Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        int col = Mathf.RoundToInt((worldPosition.x - minX) / gridSize);
        int row = Mathf.RoundToInt((worldPosition.y - minY) / gridSize);
        return new Vector2Int(col, row);
    }

    private Vector2 GridToWorld(Vector2Int gridPosition)
    {
        float x = minX + gridPosition.x * gridSize;
        float y = minY + gridPosition.y * gridSize;
        return new Vector2(x, y);
    }

    private IEnumerator UpdatePathPeriodically()
    {
        while (true)
        {
            if (player != null)
            {
                float distanceToLastPosition = Vector2.Distance(lastPlayerPosition, player.position);

                if (distanceToLastPosition > 1f)
                {
                    lastPlayerPosition = player.position;
                    yield return StartCoroutine(FindPath());
                }
            }
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    private IEnumerator FindPath()
    {
        path.Clear();

        if (isPathFinding)
            yield break;

        Vector2Int startGridPos = WorldToGrid(transform.position);
        Vector2Int goalGridPos = WorldToGrid(new Vector2(player.position.x, player.position.y - 1));

        string start = $"({startGridPos.x}, {startGridPos.y})";
        string goal = $"({goalGridPos.x}, {goalGridPos.y})";
        UnityEngine.Debug.Log("start: " + start + " goal: " + goal);
        UnityEngine.Debug.Log("gridWidth: " + gridWidth + " gridHeight: " + gridHeight);
        string gridJson = JsonConvert.SerializeObject(GetGrid());

        string pythonFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "astar_pathfinding.py");
        string arguments = $"\"{pythonFilePath}\" \"{start}\" \"{goal}\" \"{gridJson}\"";
        UnityEngine.Debug.Log("arguments: " + arguments);
        
        isPathFinding = true;

        Task<string> pathFindingTask = Task.Run(() => RunPythonProcess(arguments));

        while (!pathFindingTask.IsCompleted)
        {
            yield return null;
        }

        if (pathFindingTask.IsCompletedSuccessfully)
        {
            OnPathReceived(pathFindingTask.Result);
        }

        isPathFinding = false;
        yield return new WaitForSeconds(0.5f);
    }

    private string RunPythonProcess(string arguments)
    {
        string pythonPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Python/python.exe");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        UnityEngine.Debug.Log("Starting...");
        using (Process process = Process.Start(startInfo))
        {
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.Log(output);
                    return output;
                }
                else
                {
                    UnityEngine.Debug.Log(error);
                    return string.Empty;
                }
            }
        }

        UnityEngine.Debug.Log("Python process failed to start.");
        return string.Empty;
    }

    private void OnPathReceived(string jsonPath)
    {
        List<Vector2Int> gridPath = JsonConvert.DeserializeObject<List<Vector2Int>>(jsonPath);

        foreach (var point in gridPath)
        {
            Vector2 worldPoint = GridToWorld(point);
            path.Add(worldPoint);
        }
    }

    private void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            Vector2 target = path[0];
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            m_animator.SetInteger("AnimState", 1);

            if (target.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                AttemptAttack();
            }
            else if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                path.RemoveAt(0);
            }
        }
    }

    private int[,] GetGrid()
    {
        int[,] grid = new int[gridWidth, gridHeight];
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                grid[i, j] = 0;
            }
        }
        return grid;
    }

    private void AttemptAttack()
    {
        if (attackCooldownTimer <= 0)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            if (playerCollider != null)
            {
                AttackPlayer(playerCollider);
                attackCooldownTimer = attackCooldown;
            }
        }
    }

    private void AttackPlayer(Collider2D playerCollider)
    {
        m_animator.SetTrigger("Attack");

        Character playerCharacter = playerCollider.GetComponent<Character>();
        playerCharacter?.TakeDamage(attackDamage);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (!m_isDead)
        {
            m_animator.SetTrigger("Hurt");
        }
        else
        {
            if (!counted)
            {
                killCount++;
                counted = true;
            }
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        m_animator.SetTrigger("Death");
        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
