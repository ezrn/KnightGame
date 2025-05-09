using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{

    public GameObject deathScreen;
    public static event Action OnPlayerRespawned;
    [SerializeField] public PlayerStats playerStats;
    [SerializeField] public GameObject player;
    public void doRespawn()
    {
        playerStats.setPoise(playerStats.getMinPoise());
        deathScreen.SetActive(false);
        OnPlayerRespawned?.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.transform.position = playerStats.getRespawnLocation();
    }
}
