using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public string characterName = "Character";
    protected static int killCount = 0;
    protected int maxHealth = 1000;
    protected int currentHealth;
    public float moveSpeed = 5f;
    protected Animator m_animator;
    protected bool m_isDead = false;

    public Text nameText;
    public Text healthText;
    public Text killCountText;

    protected virtual void Start()
    {
        currentHealth = 100;

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            characterName = PlayerPrefs.GetString("PlayerName");
        }

        UpdateUI();

        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogWarning($"{characterName} is missing an Animator component.");
        }
    }

    protected virtual void Update()
    {
    }

    public virtual void TakeDamage(int damage)
    {
        if (m_isDead) return; 

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateUI();
    }

    protected void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health Points: " + currentHealth;

        if (nameText != null)
            nameText.text = characterName;

        if (killCountText != null)
            killCountText.text = "Kills: " + killCount;
    }

    protected virtual void Die()
    {
        m_isDead = true;
        if (m_animator != null)
        {
            m_animator.SetTrigger("Death");
        }
    }

    public virtual void RecoverHealth() {}
}
