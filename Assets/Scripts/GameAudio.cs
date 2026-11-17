using UnityEngine;

[DisallowMultipleComponent]
public class GameAudio : MonoBehaviour
{
    [SerializeField] private RoundController round;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource attackSource;
    [SerializeField] private AudioSource cueSource;
    [SerializeField] private AudioClip kunai;
    [SerializeField] private AudioClip fireball;
    [SerializeField] private AudioClip waveStart;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;

    private void OnEnable()
    {
        round.WaveStarted += OnWaveStarted;
        round.RoundEnded += OnRoundEnded;
        round.RoundRestarted += OnRestart;
        TentenTower.ShotFired += OnShotFired;
        EnsureMusic();
    }

    private void OnDisable()
    {
        round.WaveStarted -= OnWaveStarted;
        round.RoundEnded -= OnRoundEnded;
        round.RoundRestarted -= OnRestart;
        TentenTower.ShotFired -= OnShotFired;
        musicSource.Stop();
        attackSource.Stop();
        cueSource.Stop();
    }

    private void EnsureMusic()
    {
        if (!musicSource.isPlaying) musicSource.Play();
    }

    private void OnShotFired(TentenTower.TowerKind kind)
    {
        attackSource.PlayOneShot(kind == TentenTower.TowerKind.Sasuke ? fireball : kunai);
    }

    private void OnWaveStarted() => cueSource.PlayOneShot(waveStart);

    private void OnRoundEnded(RoundController.RoundState result)
    {
        attackSource.Stop();
        cueSource.Stop();
        cueSource.PlayOneShot(result == RoundController.RoundState.Victory ? victory : defeat);
    }

    private void OnRestart()
    {
        attackSource.Stop();
        cueSource.Stop();
        EnsureMusic();
    }
}
