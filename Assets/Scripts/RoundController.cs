using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class RoundController : MonoBehaviour
{
    public enum RoundState { Ready, Running, Victory, Defeat }

    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private EnemyHealth toughEnemyPrefab;
    [SerializeField] private Transform[] route;
    [SerializeField] private TowerPlacement placement;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text ryoText;
    [SerializeField] private Text placementMessage;

    [SerializeField, Min(0)] private int defeatReward = 15;

    public const int TentenCost = 50;
    public const int ChuninUpgradeCost = 60;
    public int Ryo { get; private set; } = 100;
    public bool CanAffordTenten => Ryo >= TentenCost;
    public bool CanAfford(int cost) => Ryo >= cost;
    public bool CanManageTowers => State == RoundState.Ready || State == RoundState.Running;
    public event System.Action RyoChanged;
    public event System.Action WaveStarted;
    public event System.Action<RoundState> RoundEnded;
    public event System.Action RoundRestarted;

    public bool TryPayForChuninUpgrade()
    {
        if (!CanManageTowers || Ryo < ChuninUpgradeCost) return false;
        Ryo -= ChuninUpgradeCost;
        placementMessage.text = "";
        RefreshUI();
        return true;
    }

    public void ShowInsufficientRyo()
    {
        placementMessage.text = "Not enough Ryo.";
    }

    public bool TryPayForTenten()
    {
        return TryPayDeployment(TentenCost);
    }

    public bool TryPayDeployment(int cost)
    {
        if (!CanManageTowers || cost <= 0 || !CanAfford(cost)) return false;
        Ryo -= cost;
        placementMessage.text = "";
        RefreshUI();
        return true;
    }

    public void RefundTentenSale()
    {
        RefundTowerSale(TowerPlacement.SellRefund);
    }

    public void RefundTowerSale(int refund)
    {
        if (!CanManageTowers || refund <= 0) return;
        Ryo += refund;
        placementMessage.text = "";
        RefreshUI();
    }

    [System.Serializable]
    public class WaveSettings
    {
        public EnemyType[] spawnOrder;
        [Min(0.1f)] public float spawnInterval = 2f;

        [Min(0)] public int groupSize;
        [Min(0.1f)] public float groupInterval = 1.5f;

        public float DelayAfterSpawn(int spawnedCount)
        {
            bool groupEnded = groupSize > 0 && spawnedCount % groupSize == 0;
            return Mathf.Max(0.1f, groupEnded ? groupInterval : spawnInterval);
        }

        public WaveSettings(float interval, params EnemyType[] order)
        {
            spawnInterval = interval;
            spawnOrder = order;
        }
    }

    public enum EnemyType { Regular, Tough }

    [SerializeField] private WaveSettings[] waves =
    {
        new WaveSettings(2f, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular),
        new WaveSettings(1.6f, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough,
            EnemyType.Regular, EnemyType.Regular),
        new WaveSettings(1.2f, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough,
            EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Regular),
        new WaveSettings(0.75f, EnemyType.Tough, EnemyType.Regular, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Tough, EnemyType.Regular,
            EnemyType.Tough, EnemyType.Tough, EnemyType.Regular),
        new WaveSettings(0.5f, EnemyType.Tough, EnemyType.Regular, EnemyType.Regular,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough,
            EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough),
        new WaveSettings(0.45f,
            EnemyType.Tough, EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough),
        new WaveSettings(0.4f,
            EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Regular, EnemyType.Regular,
            EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Regular,
            EnemyType.Regular, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular),
        new WaveSettings(0.4f,
            EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular,
            EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough, EnemyType.Tough,
            EnemyType.Tough, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular, EnemyType.Regular)
        {
            groupSize = 12,
            groupInterval = 1.5f
        }
    };
    private int waveIndex;
    public int CurrentWaveNumber => waveIndex + 1;
    public int WaveCount => waves.Length;
    public int CurrentWaveSize => waves[waveIndex].spawnOrder.Length;
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
        SpawnedCount = 0;
        DefeatedCount = 0;
        State = RoundState.Running;
        SpawnEnemy();
        RefreshUI();
        WaveStarted?.Invoke();
    }

    private void Update()
    {
        if (State == RoundState.Running && SpawnedCount < CurrentWaveSize && Time.time >= nextSpawnTime)
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        EnemyType type = waves[waveIndex].spawnOrder[SpawnedCount];
        EnemyHealth enemy = Instantiate(type == EnemyType.Tough ? toughEnemyPrefab : enemyPrefab,
            route[0].position, Quaternion.identity);
        enemy.name = type == EnemyType.Tough ? "Tough Enemy" : "Regular Enemy";
        enemy.GetComponent<EnemyMovement>().SetRoute(route);
        enemy.Defeated += OnDefeated;
        enemy.GetComponent<EnemyMovement>().ReachedGate += OnReachedGate;
        enemies.Add(enemy);
        SpawnedCount++;
        nextSpawnTime = Time.time + waves[waveIndex].DelayAfterSpawn(SpawnedCount);
        RefreshUI();
    }

    private void OnDefeated(EnemyHealth enemy)
    {
        if (State != RoundState.Running) return;
        if (!enemies.Remove(enemy)) return;
        DefeatedCount++;
        Ryo += defeatReward;
        placementMessage.text = "";
        if (SpawnedCount == CurrentWaveSize && DefeatedCount == CurrentWaveSize)
        {
            if (waveIndex == waves.Length - 1)
                EndRound(RoundState.Victory);
            else
            {
                waveIndex++;
                State = RoundState.Ready;
                foreach (KunaiProjectile kunai in FindObjectsByType<KunaiProjectile>()) Remove(kunai.gameObject);
                RefreshUI();
            }
        }
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
        placement.ClearSelection();
        foreach (EnemyHealth enemy in enemies)
            if (enemy != null) enemy.GetComponent<EnemyMovement>().enabled = false;
        foreach (TentenTower tower in FindObjectsByType<TentenTower>()) tower.enabled = false;
        foreach (KunaiProjectile kunai in FindObjectsByType<KunaiProjectile>()) Remove(kunai.gameObject);
        RefreshUI();
        RoundEnded?.Invoke(result);
    }

    public void RestartRound()
    {
        State = RoundState.Ready;
        waveIndex = 0;
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>()) Remove(enemy.gameObject);
        foreach (TentenTower tower in FindObjectsByType<TentenTower>()) Remove(tower.gameObject);
        foreach (KunaiProjectile kunai in FindObjectsByType<KunaiProjectile>()) Remove(kunai.gameObject);
        enemies.Clear();
        SpawnedCount = 0;
        DefeatedCount = 0;
        nextSpawnTime = 0f;
        Ryo = 100;
        placementMessage.text = "";
        placement.ResetPlacement();
        RefreshUI();
        RoundRestarted?.Invoke();
    }

    private static void Remove(GameObject obj)
    {
        obj.SetActive(false);
        Destroy(obj);
    }

    private void RefreshUI()
    {
        startButton.interactable = State == RoundState.Ready;
        ryoText.text = $"Ryo: {Ryo}  |  Tenten: 50 Ryo  |  Sasuke: 60 Ryo";
        string wave = $"Wave {CurrentWaveNumber} of {WaveCount}";
        statusText.text = State == RoundState.Ready ? $"{wave}  |  Click an empty pad, choose Tenten or Sasuke, then press Deploy." :
            HasEnded ? $"{State}  |  {wave}" : $"{wave}  |  Spawned {SpawnedCount}/{CurrentWaveSize}  |  Defeated {DefeatedCount}/{CurrentWaveSize}";
        RyoChanged?.Invoke();
    }
}
