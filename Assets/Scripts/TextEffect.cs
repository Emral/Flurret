using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    public TMPro.TMP_Text text;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(ScoreMove());
    }

    public void SetScore(int score)
    {
        text.text = score.ToString();
    }

    private IEnumerator ScoreMove()
    {
        yield return new WaitForSeconds(0.3f);
        yield return gameObject.DespawnFlicker(text);
    }
}