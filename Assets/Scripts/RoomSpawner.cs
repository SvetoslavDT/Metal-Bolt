using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public static int totalRoomsVisited = 1;

    public GameObject enemyPrefab;

    public Transform[] roomSpawnPoints;

    private int totalEnemiesToKill;
    private int maxEnemiesAlive;

    private int totalSpawnedSoFar = 0;
    private int currentLiveEnemies = 0;

    public bool isRoomAlerted = false;

    void Start()
    {
        totalEnemiesToKill = 2 + totalRoomsVisited;

        maxEnemiesAlive = 3 + Mathf.FloorToInt(totalRoomsVisited / 2f);

        if (maxEnemiesAlive > roomSpawnPoints.Length)
        {
            maxEnemiesAlive = roomSpawnPoints.Length;
        }

        for (int i = 0; i < maxEnemiesAlive; i++)
        {
            if (totalSpawnedSoFar < totalEnemiesToKill)
            {
                SpawnEnemy();
            }
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayGameMusicNormal();
        }
    }

    public void OnEnemyKilled(bool wasAlerted)
    {
        currentLiveEnemies--;

        if (wasAlerted)
        {
            if (totalSpawnedSoFar < totalEnemiesToKill)
            {
                Invoke("SpawnEnemy", 1f);
            }
            else if (currentLiveEnemies <= 0)
            {
                OpenRoomDoors();
            }
        }
        else
        {
            if (currentLiveEnemies <= 0)
            {
                OpenRoomDoors();
            }
        }
    }


    void SpawnEnemy()
    {
        if (enemyPrefab != null && roomSpawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, roomSpawnPoints.Length);
            Transform selectedPoint = roomSpawnPoints[randomIndex];

            GameObject newEnemy = Instantiate(enemyPrefab, selectedPoint.position, Quaternion.identity);

            if (isRoomAlerted)
            {
                EnemyAI nextEnemyAI = newEnemy.GetComponent<EnemyAI>();
                if (nextEnemyAI != null)
                {
                    nextEnemyAI.AlertEnemy();
                }
            }

            totalSpawnedSoFar++;
            currentLiveEnemies++;
        }
    }


    void OpenRoomDoors()
    {
        Debug.Log("Room cleared!.");
        isRoomAlerted = false;

        Door[] roomDoors = Object.FindObjectsByType<Door>(FindObjectsSortMode.None);

        foreach (Door door in roomDoors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SwitchToNormalMusic();
        }
    }
}
