using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Rain : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject rainEmitter;
    public AudioSource rainSound;
    public AudioSource rainSound2;

    [Header("Rain Settings")]
    public new bool enabled = false;
    public float spawnHeightOffset = 20f; // how far above the player the rain spawns
    public float checkInterval = 0.25f; // how often it checks the players position to reorient rain
    public float minRainRate = 10f; // minimum rate that rain can be
    public float maxRainRate = 2000f;
    public float minRainVelocity = -27f;
    public float maxRainVelocity = -50f;
    public float rainTurbulenceMultiplier = 0.2f;
    public float windSpeed = 10f;
    public float minAudioThreshold = 0.1f;

    private float tileSize = 50f;
    private float rainIntensity;

    // tracks all spawned emitters with their tile coordinates
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

    private float timer = 0f;

    private void Start()
    {
        if (enabled)
        {
            rainSound.Play();
            rainSound2.Play();
            // read the shape scale from the prefab's ParticleSystem to determine tileSize
            ParticleSystem ps = rainEmitter.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var shape = ps.shape;
                tileSize = shape.scale.x;
            }

            // spawn initial 3x3 grid of emitters around player position
            UpdateRainTiles(forceImmediate: true);
        }
    }

    private void Update()
    {
        if (enabled)
        {
            timer += Time.deltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                UpdateRainTiles();
                AnimateRain();
                AnimateWind();
                ChangeRainVolume();
            }
        }
    }

    void ChangeRainVolume()
    {
        float volume = ((rainIntensity - minRainRate) / (maxRainRate - minRainRate)) * (1 - minAudioThreshold) + minAudioThreshold; // 0 to 1 number for volume basd on rain rate
        setRainVolume(volume);
    }

    void setRainVolume(float volume)
    {
        rainSound.volume = volume;
        rainSound2.volume = volume;
    }

    private void AnimateRain()
    {
        float sinValue = (Mathf.Sin(Time.time * rainTurbulenceMultiplier) + 1f) / 2f;
        float newRainRate = Mathf.Lerp(minRainRate, maxRainRate, sinValue);
        float newRainVelocity = Mathf.Lerp(minRainVelocity, maxRainVelocity, sinValue);

        setRainRate(newRainVelocity, newRainRate);
    }

    void setRainRate(float rainSpeed, float rainDensity)
    {
        foreach (var kvp in activeTiles)
        {
            var ps = kvp.Value?.GetComponent<ParticleSystem>();
            if (ps == null) continue;

            // emission
            var emission = ps.emission;
            emission.rateOverTime = rainDensity;

            // velocity
            var vel = ps.velocityOverLifetime;
            vel.enabled = true;

            float vMin = Mathf.Min(rainSpeed, rainSpeed * 1.25f);
            float vMax = Mathf.Max(rainSpeed, rainSpeed * 1.25f);

            vel.y = new ParticleSystem.MinMaxCurve(vMin, vMax);

            rainIntensity = rainDensity;
        }
    }


    void AnimateWind()
    {
        float sinValue = (Mathf.Sin(Time.time * rainTurbulenceMultiplier) + 1f) / 2f;
        float newWindVelocity = Mathf.Lerp(-1 * windSpeed, windSpeed, sinValue);

        setWindSpeed(newWindVelocity, newWindVelocity);
    }

    public void setWindSpeed(float xSpeed, float zSpeed)
    {
        foreach (var kvp in activeTiles)
        {
            GameObject emitterGO = kvp.Value;
            if (emitterGO != null)
            {
                ParticleSystem ps = emitterGO.GetComponent<ParticleSystem>();
                if (ps != null)
                {

                    var velocity = ps.velocityOverLifetime;
                    ParticleSystem.MinMaxCurve xCurve = velocity.x;
                    ParticleSystem.MinMaxCurve zCurve = velocity.z;

                    xCurve.constantMin = xSpeed * 0.8f;
                    xCurve.constantMax = xSpeed;

                    zCurve.constantMin = zSpeed * 0.8f;
                    zCurve.constantMax = zSpeed;

                    velocity.x = xCurve;
                    velocity.z = zCurve;

                }
            }
        }
    }

    private void UpdateRainTiles(bool forceImmediate = false)
    {
        //compute the player's tile position in XZ 
        Vector3 playerPos = player.transform.position;
        Vector2Int centerTile = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / tileSize),
            Mathf.FloorToInt(playerPos.z / tileSize)
        );

        //build the set of desired tile coordinates for a 3×3 area
        HashSet<Vector2Int> neededTiles = new HashSet<Vector2Int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                neededTiles.Add(new Vector2Int(centerTile.x + i, centerTile.y + j));
            }
        }

        // create any missing tiles
        foreach (Vector2Int tilePos in neededTiles)
        {
            if (!activeTiles.ContainsKey(tilePos))
            {
                // compute world position for this tile
                float worldX = tilePos.x * tileSize;
                float worldZ = tilePos.y * tileSize;
                float worldY = playerPos.y + spawnHeightOffset;

                Vector3 spawnPos = new Vector3(worldX, worldY, worldZ);

                GameObject newEmitter = Instantiate(rainEmitter, spawnPos, Quaternion.identity);

                // track it
                activeTiles[tilePos] = newEmitter;
            }
        }

        // remove tiles that are no longer in the 3×3 set
        List<Vector2Int> tilesToRemove = new List<Vector2Int>();
        foreach (var kvp in activeTiles)
        {
            if (!neededTiles.Contains(kvp.Key))
            {
                tilesToRemove.Add(kvp.Key);
            }
        }

        foreach (Vector2Int tilePos in tilesToRemove)
        {
            Destroy(activeTiles[tilePos]);
            activeTiles.Remove(tilePos);
        }
    }
}
