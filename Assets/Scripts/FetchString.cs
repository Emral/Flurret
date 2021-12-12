using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchString : MonoBehaviour
{
    public TMPro.TMP_Text hiScore;
    public TMPro.TMP_Text waves;

    // Start is called before the first frame update
    private void Start()
    {
        waves.SetText(PersistentManager.instance.GetBestWave());
        hiScore.SetText(PersistentManager.instance.GetHighScore());
    }
}