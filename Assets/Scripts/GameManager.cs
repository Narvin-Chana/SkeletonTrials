using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject skeletonPrefab;
    public bool ableToGoToNextLevel;
    public bool shouldSelectPowerUp;
    public RunePillar[] pillars;

    private int level;

    private int skeletonsToSpawnCounter;
    private float skeletonSpawnDelay;
    private float skeletonSpawnDelayCounter;
    private int skeletonHealth;

    public int enemiesLeftCounter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void StartLevel()
    {
        TopDownCharacterController.Instance.PlayerStats.Health =
            TopDownCharacterController.Instance.PlayerStats.MaxHealth;
        UIManager.Instance.UpdateHearts();
        ableToGoToNextLevel = false;
        level++;
        skeletonsToSpawnCounter = 2 * level;
        skeletonSpawnDelay = 2.0f;
        skeletonHealth = 3 + level / 3;
    }

    private void Start()
    {
        TopDownCharacterController.Instance.PlayerStats.Health =
            TopDownCharacterController.Instance.PlayerStats.MaxHealth;
        UIManager.Instance.UpdateHearts();
        pillars[1].ActivatePillar(PowerUpType.Attack);
    }

    private void Update()
    {
        if (skeletonSpawnDelayCounter < 0 && skeletonsToSpawnCounter > 0)
        {
            SpawnSkeleton();
            skeletonSpawnDelayCounter = skeletonSpawnDelay;
        }
        else
        {
            skeletonSpawnDelayCounter -= Time.deltaTime;
        }

        if (enemiesLeftCounter == 0 && !shouldSelectPowerUp && level > 0 && !ableToGoToNextLevel)
        {
            SpawnPowerUps();
        }
    }

    private void SpawnSkeleton()
    {
        skeletonsToSpawnCounter--;
        enemiesLeftCounter++;
        // Spawn a skeleton at a random location inside the map
        GameObject skeleton = Instantiate(skeletonPrefab, GetRandomSpawnLocation(), Quaternion.identity);
        skeleton.GetComponent<EnemyController>().health = skeletonHealth;
    }

    private void SpawnPowerUps()
    {
        shouldSelectPowerUp = true;
        // Select three (valid) random power ups to assign to the three altars
        List<PowerUpType> powerUps = new(3);

        List<PowerUpType> powerUpPool = TopDownCharacterController.Instance.PlayerStats.GetPossiblePowerUpPool();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, powerUpPool.Count);
            powerUps.Add(powerUpPool[randomIndex]);
            powerUpPool.RemoveAt(randomIndex);
        }

        foreach (RunePillar runePillar in pillars)
        {
            runePillar.ActivatePillar(powerUps[0]);
            powerUps.Remove(powerUps[0]);
        }
    }

    public void SelectPowerUpDone()
    {
        shouldSelectPowerUp = false;
        // Deactivate all pillars
        foreach (RunePillar runePillar in pillars)
        {
            runePillar.DeactivatePillar();
        }

        // Allow the player to start the next level
        ableToGoToNextLevel = true;
    }

    private static Vector2 GetRandomSpawnLocation()
    {
        float x;
        float y;
        do
        {
            x = Random.Range(-10, 12);
            y = Random.Range(-4.5f, 2);
        } while (Vector3.Distance(new Vector3(x, y, 0), TopDownCharacterController.Instance.transform.position) < 1.0f);

        return new Vector2(x, y);
    }
}