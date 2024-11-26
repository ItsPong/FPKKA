using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : Character
{
    public int attackDamage = 20; 
    public float attackRange = 1.5f;  
    public Transform attackPoint;
    public LayerMask enemyLayer; 

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (m_isDead) return; 

        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        
        transform.Translate(new Vector3(moveX, moveY, 0), Space.World); 

        if (moveX > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0); 
        else if (moveX < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0); 

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_animator.SetTrigger("Attack1");
            PerformAttack();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            m_animator.SetTrigger("Attack2");
            PerformAttack();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            m_animator.SetTrigger("Attack3");
            PerformAttack();
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon || Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Epsilon)
        {
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_animator.SetInteger("AnimState", 0);
        }
    }

    private void PerformAttack()
    {
        if (attackPoint == null) return; 

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Character enemyCharacter = enemy.GetComponent<Character>();
            if (enemyCharacter != null)
            {
                enemyCharacter.TakeDamage(attackDamage);
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        if (m_isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            if (m_animator != null)
            {
                m_animator.SetTrigger("Hurt");
            }
        }

        UpdateUI();
    }

    protected override void Die()
    {
        m_isDead = true; 

        if (m_animator != null)
        {
            m_animator.SetTrigger("Death");
            StartCoroutine(HandleDeathAndLoadScene());
            
            m_animator.ResetTrigger("Hurt");
            m_animator.ResetTrigger("Attack1");
            m_animator.ResetTrigger("Attack2");
            m_animator.ResetTrigger("Attack3");
        }
    }

    private IEnumerator HandleDeathAndLoadScene()
    {
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetInt("KillCount", killCount);
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
