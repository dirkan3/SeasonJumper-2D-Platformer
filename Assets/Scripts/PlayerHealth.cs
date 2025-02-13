using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private static PlayerHealth _instance;
    public int maxHealth = 5;
    public int currentHealth;

    public bool IsFullHealth => currentHealth >= maxHealth;
    public bool IsDead => currentHealth <= 0;

    public Animator animator;
    public HealthUI healthUI;
    private SpriteRenderer SpriteRenderer;
    AudioManager audioManager;

    public static event Action OnPlayerDied;

    private bool _isImmune = false;
    private float immunityDuration = 1.5f;

    private bool shouldResetHealth = false;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SpriteRenderer = GetComponent<SpriteRenderer>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {

        if (GameController.Instance != null && GameController.Instance.storedPlayerHealth > 0)
        {
            currentHealth = GameController.Instance.storedPlayerHealth;
        }
        else
        {
            currentHealth = maxHealth;
            if (GameController.Instance != null)
            {
                GameController.Instance.storedPlayerHealth = maxHealth;
            }
        }

        if (healthUI == null)
        {
            healthUI = FindObjectOfType<HealthUI>();
        }

        if (healthUI != null)
        {
            healthUI.SetMaxHearts(maxHealth);
            healthUI.UpdateHeart(currentHealth);
        }
    }

    #region PlayerHealth

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            return;
        }

        StartCoroutine(InitializeHealthUIWithChecks());
    }

    private IEnumerator InitializeHealthUIWithChecks()
    {
        while (GameController.Instance == null || GameController.Instance.playerHealth == null || healthUI == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        healthUI.SetMaxHearts(maxHealth);
        healthUI.UpdateHeart(currentHealth);

        if (shouldResetHealth)
        {
            ResetHealth();
            shouldResetHealth = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isImmune) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
        }
        Trap trap = collision.GetComponent<Trap>();
        
        if(trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
            StartCoroutine(Invunerable());
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0);
        audioManager.PlaySFX(audioManager.hit);

        animator.SetBool("isHurt", true);
        StartCoroutine(ResetHurtAfterDelay());

        if (healthUI != null)
        {
            healthUI.UpdateHeart(currentHealth);
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.storedPlayerHealth = currentHealth;
        }

        if (currentHealth <= 0)
        {
            audioManager.PlaySFX(audioManager.death);
            OnPlayerDied?.Invoke();
        }
    }

    private IEnumerator ResetHurtAfterDelay()
    {
        yield return new WaitForSeconds(1f);  
        animator.SetBool("isHurt", false);  
    }


    private IEnumerator Invunerable()
    {
        _isImmune = true; 
        yield return new WaitForSeconds(immunityDuration); 
        _isImmune = false;
    }


    public void ResetHealth()
    {
        currentHealth = maxHealth;
        // Update the UI here only once.
        if (healthUI == null)
        {
            healthUI = FindObjectOfType<HealthUI>();
            if (healthUI == null)
            {
                return;
            }
        }

        healthUI.ResetUI(maxHealth);
    }

    public void TriggerHealthReset()
    {
        shouldResetHealth = true; 
    }
    #endregion

    #region addHealth

    public void AddHealth(int amount)
    {
        if (!IsFullHealth)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            UpdateHealthUI();
            if (GameController.Instance != null)
            {
                GameController.Instance.storedPlayerHealth = currentHealth;
            }
        }
    }

    private void UpdateHealthUI()
    {
        FindObjectOfType<HealthUI>().UpdateHeart(currentHealth);
    }

    #endregion
}
