using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    [HideInInspector] public Player playerInstance;
    [HideInInspector] public Camera mainCam;

    public References refs;

    public static int enemyWaveCountAddition;
    public static float enemyHPAddition;

    private Vector2 _backgroundSpeed = Vector2.zero;
    private Vector2 _backgroundOffset = Vector2.zero;
    private bool _isPaused = false;

    public static float deltaTime = 0;

    private AudioSource pauseLoop;

    private Coroutine _nextWaveCor;

    [HideInInspector] public PowerupStage FrontShot = PowerupStage.Low;
    [HideInInspector] public PowerupStage SideShot = PowerupStage.Off;
    [HideInInspector] public PowerupStage BackShot = PowerupStage.Off;
    [HideInInspector] public PowerupStage Shields = PowerupStage.Off;
    [HideInInspector] public PowerupStage Zany = PowerupStage.Off;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;
    }

    public ParticleSystem SpawnEffect(EffectType type, Vector3 position)
    {
        ParticleSystem prt = refs.GetEffect(type);
        ParticleSystem prtInstance = Instantiate(prt, position, Quaternion.identity);
        prtInstance.Play();
        return prt;
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;

        if (Input.GetButtonDown("Pause"))
        {
            if (_isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        //uiReferences.PauseOverlay.SetActive(true);
        _isPaused = true;
        Time.timeScale = 0;
        //AudioManager.PlaySFX(SFX.Pause);
        //pauseLoop = AudioManager.PlaySFX(SFX.Pauseloop);
        //AudioManager.PauseMusic();
    }

    public void UnpauseGame()
    {
        //uiReferences.PauseOverlay.SetActive(false);
        _isPaused = false;
        //pauseLoop.Stop();
        //AudioManager.PlaySFX(SFX.Resume);
        Time.timeScale = 1;
        //AudioManager.UnpauseMusic();
    }

    public bool GetIsPaused()
    {
        return _isPaused;
    }
}