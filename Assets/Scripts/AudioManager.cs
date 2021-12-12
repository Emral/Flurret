using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SFX
{
    None = -1,
    ButtonClick, // ok
    Pause, // ok
    Pauseloop, // ok
    Resume, // ok
    PlayerShoot, // ok
    EnemyShoot, // ok
    PlayerHarm, // ok
    PlayerKill, // ok
    PlayerKillForever, // ok
    EnemyHarm, // ok
    EnemyKill, // ok
    BossKill, // ok
    FlyBuzz, // ok
    BonusItem, // ok
    LevelStart, // ok
    LevelWin, // ok
    LaserWindup, // ok
    LaserShoot, // ok
    LaserShootSmall, // ok
    RoomBonk, // ok
    RoomCry, // ok
    Quadcopter, // ok
    BalloonPop, // ok
    LaserCry, // ok
}

[System.Serializable]
public class SoundEffect
{
    public SFX type;
    public AudioClip clip;
    public bool looping;
}

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;

    public AudioSource sfxSourcePrefab;

    private List<AudioSource> _instantiatedSoundPool = new List<AudioSource>();
    private Dictionary<SFX, float> _cooldowns = new Dictionary<SFX, float>();

    public static List<SoundEffect> soundEffects;

    private static AudioManager _instance;

    private float _audioMaxVolume;

    private float _cooldownMax = 0.10f;
    private float _volumeLerpDirection = 1;

    private bool _musicPaused = false;

    // Start is called before the first frame update
    private void Start()
    {
        soundEffects = PersistentManager.instance.refs.sounds;
        _instance = this;
        _audioMaxVolume = musicSource.volume;
    }

    private void Update()
    {
        if (_musicPaused)
        {
            return;
        }

        foreach (AudioSource src in _instantiatedSoundPool)
        {
            if (!src.isPlaying)
            {
                src.gameObject.SetActive(false);
            }
        }

        float nextVolume = Mathf.Clamp(musicSource.volume + _volumeLerpDirection * Manager.deltaTime, 0, _audioMaxVolume);
        if (nextVolume != musicSource.volume)
        {
            musicSource.volume = nextVolume;
        }
    }

    public static void PlayMusic(AudioClip clip, bool skipFade = false)
    {
        _instance.musicSource.clip = clip;
        _instance._volumeLerpDirection = 1;
        if (skipFade)
        {
            _instance.musicSource.volume = _instance._audioMaxVolume;
        }
        _instance.musicSource.Play();
    }

    public static void PauseMusic()
    {
        _instance.musicSource.Pause();
        _instance._musicPaused = true;
        foreach (AudioSource s in _instance._instantiatedSoundPool)
        {
            if (s.gameObject.activeSelf)
            {
                s.Pause();
            }
        }
    }

    public static void UnpauseMusic()
    {
        _instance.musicSource.UnPause();
        _instance._musicPaused = false;
        foreach (AudioSource s in _instance._instantiatedSoundPool)
        {
            if (s.gameObject.activeSelf)
            {
                s.UnPause();
            }
        }
    }

    public static void StopMusic()
    {
        _instance.musicSource.Stop();
    }

    public static void FadeIn()
    {
        _instance._volumeLerpDirection = 1;
    }

    public static void FadeOut()
    {
        _instance._volumeLerpDirection = -1;
    }

    public static AudioSource PlaySFX(SFX type)
    {
        if (type == SFX.None)
        {
            return null;
        }

        if (!_instance._cooldowns.ContainsKey(type))
        {
            _instance._cooldowns.Add(type, 0);
        }
        else if (Time.time < _instance._cooldowns[type] + _instance._cooldownMax)
        {
            return null;
        }

        _instance._cooldowns[type] = Time.time;
        AudioSource freeSource = _instance._instantiatedSoundPool.FirstOrDefault(k => k.gameObject.activeSelf == false);

        if (freeSource == default)
        {
            freeSource = _instance.AddNewToPool();
        }

        freeSource.gameObject.SetActive(true);

        SoundEffect effect = soundEffects.First(s => s.type == type);
        freeSource.clip = effect.clip;
        freeSource.loop = effect.looping;
        freeSource.Play();

        return freeSource;
    }

    private AudioSource AddNewToPool()
    {
        AudioSource newSource = Instantiate(sfxSourcePrefab, transform);
        _instantiatedSoundPool.Add(newSource);
        return newSource;
    }
}