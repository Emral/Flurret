using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistentManager : MonoBehaviour
{
    [HideInInspector] public int _bestWave = 0;
    [HideInInspector] public ulong _highScore = 0;

    public static PersistentManager instance;

    public Image FadeScreen;

    public References refs;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            _bestWave = PlayerPrefs.GetInt("BestWave", 0);
            _highScore = System.Convert.ToUInt64(PlayerPrefs.GetInt("HighScore", 0));
        }
        else
        {
            //Keep it around for button bindings.
            //Destroy(this.gameObject);
        }
    }

    public void LoadGame()
    {
        AudioManager.PlaySFX(SFX.LevelStart);
        StartCoroutine(FadeThenLoad(1));
    }

    public void LoadScene(int idx)
    {
        StartCoroutine(FadeThenLoad(idx));
    }

    public IEnumerator FadeThenLoad(int sceneIdx)
    {
        yield return FadeScreen.Fade(new Color(0, 0, 0, 0), Color.black);
        yield return new WaitForSeconds(0.25f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIdx);
        yield return new WaitForSeconds(0.25f);
        yield return FadeScreen.Fade(Color.black, new Color(0, 0, 0, 0));
    }

    public string GetHighScore()
    {
        return _highScore.ToString();
    }

    public string GetBestWave()
    {
        return _bestWave.ToString();
    }
}