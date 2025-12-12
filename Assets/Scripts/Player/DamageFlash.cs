using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Image overlay;
    [SerializeField] private float flashDuration = 0.2f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (overlay != null)
            overlay.color = new Color(1, 0, 0, 0); // transparent
    }

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / flashDuration);
            overlay.color = new Color(1, 0, 0, alpha);
            yield return null;
        }
        overlay.color = new Color(1, 0, 0, 0);
    }
}
