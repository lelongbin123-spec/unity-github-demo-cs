using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ghi chu:
// - He thong spawn item/gameplay object tu 4 huong voi cac ti le xac suat khac nhau.
// - Luu y: can dam bao highValuePrefabs co phan tu truoc khi random de tranh loi runtime.
public enum SpawnDirection
{
    Top,
    Left,
    Right,
    Bottom
}

public class GemFallScript : MonoBehaviour
{
    [Header("Prefabs")]
    // Các prefab gameplay có thể được spawn.
    public GameObject gemPrefab;
    public GameObject badItemPrefab;
    public GameObject[] highValuePrefabs;
    public GameObject speedPowerUpPrefab;
    public GameObject obstaclePrefab;

    [Header("Spawn Settings")]
    // Khoảng thời gian giữa mỗi lần spawn.
    public float spawnInterval = 2f;
    private float timer;

    [Header("Chance")]
    // Tỉ lệ xuất hiện của từng loại item.
    public float highValueChance = 0.2f;
    public float badItemChance = 0.2f;
    public float speedPowerChance = 0.1f;
    public float obstacleChance = 0.15f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnGem();
            timer = 0;
        }
    }

    void SpawnGem()
    {
        // Chọn prefab, chọn hướng spawn và gán hướng di chuyển cho object mới.
        // 🧱 Spawn obstacle riêng
        if (obstaclePrefab != null && Random.value < obstacleChance)
        {
            Vector3 groundPos = new Vector3(Random.Range(-8f, 8f), -3.5f, 0);
            Instantiate(obstaclePrefab, groundPos, Quaternion.identity);
        }

        float random = Random.value;

        GameObject prefabToSpawn;

        // 🎯 Chọn loại item
        if (random < highValueChance)
        {
            int index = Random.Range(0, highValuePrefabs.Length);
            prefabToSpawn = highValuePrefabs[index];
        }
        else if (random < highValueChance + badItemChance)
        {
            prefabToSpawn = badItemPrefab;
        }
        else if (random < highValueChance + badItemChance + speedPowerChance)
        {
            prefabToSpawn = speedPowerUpPrefab;
        }
        else
        {
            prefabToSpawn = gemPrefab;
        }

        // 🔀 RANDOM 4 HƯỚNG CHO TẤT CẢ
        SpawnDirection direction = (SpawnDirection)Random.Range(0, 4);

        // 📍 Vị trí spawn
        Vector3 spawnPosition = Vector3.zero;

        switch (direction)
        {
            case SpawnDirection.Top:
                spawnPosition = new Vector3(Random.Range(-8f, 8f), 6f, 0);
                break;

            case SpawnDirection.Left:
                spawnPosition = new Vector3(-9f, Random.Range(-4f, 4f), 0);
                break;

            case SpawnDirection.Right:
                spawnPosition = new Vector3(9f, Random.Range(-4f, 4f), 0);
                break;

            case SpawnDirection.Bottom:
                spawnPosition = new Vector3(Random.Range(-8f, 8f), -5f, 0);
                break;
        }

        // 🚀 Spawn object
        GameObject obj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // 🧭 Hướng di chuyển
        Vector2 moveDir = Vector2.down;

        switch (direction)
        {
            case SpawnDirection.Top:
                moveDir = Vector2.down;
                break;

            case SpawnDirection.Left:
                moveDir = Vector2.right;
                break;

            case SpawnDirection.Right:
                moveDir = Vector2.left;
                break;

            case SpawnDirection.Bottom:
                moveDir = Vector2.up;
                break;
        }

        // 🎯 Gửi direction cho object
        GemMover Gemmover = obj.GetComponent<GemMover>();
        if (Gemmover != null)
        {
            Gemmover.Init(moveDir);
        }
        
    }
}
