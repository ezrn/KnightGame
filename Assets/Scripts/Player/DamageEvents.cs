using UnityEngine;

public class DamageEvents : MonoBehaviour
{
    [Header("Damage Event Settings")]
    public float poiseRegenerationInterval = 0.1f;
    public float poiseRegenerationAmount = 1f;

    public PlayerStats playerStats;
    public GameObject deathScreen;

    private float timer = 0f;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        deathScreen.SetActive(false);
    }

    void Update()
    {
        DoPoiseRegeneration();
        CheckDeath();
        
    }

    void DoPoiseRegeneration()
    {
        timer += Time.deltaTime;
        if (timer >= poiseRegenerationInterval)
        {
            timer = 0f;
            playerStats.HealPoise(poiseRegenerationAmount);
        }
    }

    public void CheckDeath()
    {
        if (playerStats.getPoise() > playerStats.getMaxPoise())
        {
            deathScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
