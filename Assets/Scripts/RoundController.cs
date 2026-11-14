using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class RoundController : MonoBehaviour
{
    public enum RoundState { Ready, Running, Victory, Defeat }

    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private Transform[] route;
    [SerializeField] private TowerPlacement placement;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text statusText;

    private const int WaveSize = 3;
    private const float SpawnInterval = 2f;
    private readonly List<EnemyHealth> enemies = new List<EnemyHealth>();
    private float nextSpawnTime;

    public RoundState State { get; private set; }
    public int SpawnedCount { get; private set; }
    public int DefeatedCount { get; private set; }
    public bool HasEnded => State == RoundState.Victory || State == RoundState.Defeat;

    private void Awake()
    {
        // Restore the UI action bindings when entering Play Mode, including
        // projects that disable domain and scene reload for fast iteration.
        FindAnyObjectByType<InputSystemUIInputModule>().AssignDefaultActions();
        startButton.onClick.AddListener(StartWave);
        restartButton.onClick.AddListener(RestartRound);
        RestartRound();
    }

    public void StartWave()
    {
        if (State != RoundState.Ready) return;
        State = RoundState.Running;
        SpawnEnemy();
        RefreshUI();
    }

    private void Update()
    {
        if (State == RoundState.Running && SpawnedCount < WaveSize && Time.time >= nextSpawnTime)
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        EnemyHealth enemy = Instantiate(enemyPrefab, route[0].position, Quaternion.identity);
        enemy.GetComponent<EnemyMovement>().SetRoute(route);
        enemy.Defeated += OnDefeated;
        enemy.GetComponent<EnemyMovement>().ReachedGate += OnReachedGate;
        enemies.Add(enemy);
        SpawnedCount++;
        nextSpawnTime = Time.time + SpawnInterval;
        RefreshUI();
    }

    private void OnDefeated(EnemyHealth enemy)
    {
        if (State != RoundState.Running) return;
        enemies.Remove(enemy);
        DefeatedCount++;
        if (SpawnedCount == WaveSize && DefeatedCount == WaveSize)
            EndRound(RoundState.Victory);
        else RefreshUI();
    }

    private void OnReachedGate()
    {
        if (State == RoundState.Running) EndRound(RoundState.Defeat);
    }

    private void EndRound(RoundState result)
    {
        if (State != RoundState.Running) return;
        State = result;
        foreach (EnemyHealth enemy in enemies)
            if (enemy != null) enemy.GetComponent<EnemyMovement>().enabled = false;
        foreach (TentenTower tower in FindObjectsByType<TentenTower>()) tower.enabled = false;
        foreach (KunaiProjectile kunai in FindObjectsByType<KunaiProjectile>()) Remove(kunai.gameObject);
        RefreshUI();
    }

    public void RestartRound()
    {
        State = RoundState.Ready;
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>()) Remove(enemy.gameObject);
        foreach (TentenTower tower in FindObjectsByType<TentenTower>()) Remove(tower.gameObject);
        foreach (KunaiProjectile kunai in FindObjectsByType<KunaiProjectile>()) Remove(kunai.gameObject);
        enemies.Clear();
        SpawnedCount = 0;
        DefeatedCount = 0;
        nextSpawnTime = 0f;
        placement.ResetPlacement();
        RefreshUI();
    }

    private static void Remove(GameObject obj)
    {
        obj.SetActive(false);
        Destroy(obj);
    }

    private void RefreshUI()
    {
        startButton.interactable = State == RoundState.Ready;
        statusText.text = State == RoundState.Ready ? "Place Tenten, then Start Wave" :
            HasEnded ? State.ToString() : $"Wave 1  |  Spawned {SpawnedCount}/3  |  Defeated {DefeatedCount}/3";
    }
}
