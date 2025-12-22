using System.Collections;
using TMPro;
using UnityEngine;

public class Map_LevelIndicator : MonoBehaviour
{
    public static Map_LevelIndicator Instance;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float showTime = 2f;
    [SerializeField] private float fadeTime = 0.5f;

    private Coroutine routine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        levelText.alpha = 0f;
    }

    public void Show(string text)
    {

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(text));
    }

    private IEnumerator ShowRoutine(string text)
    {
        levelText.text = text;

        yield return Fade(0f, 1f);
        yield return new WaitForSeconds(showTime);
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            levelText.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }
        levelText.alpha = to;
    }


}
