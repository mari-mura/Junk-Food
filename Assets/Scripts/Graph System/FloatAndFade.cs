using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatAndFade : MonoBehaviour
{
    [SerializeField] private float lifetimeSeconds = 3.0f;
    [SerializeField] private float upwardSpeed = 1.0f;
    [SerializeField] private TMP_Text popupText;

    private float elapsedSeconds = 0.0f;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private List<float> spriteStartingAlphas = new List<float>();

    private List<Renderer> meshRenderers = new List<Renderer>();
    private List<float> meshStartingAlphas = new List<float>();

    private List<TMP_Text> textComponents = new List<TMP_Text>();
    private List<float> textStartingAlphas = new List<float>();

    public void SetText(string newText)
    {
        if (popupText != null)
        {
            popupText.text = newText;
        }
    }

    public void SetText(string newText, Color newColor)
    {
        if (popupText != null)
        {
            popupText.text = newText;
            popupText.color = newColor;

            int textIndex = textComponents.IndexOf(popupText);
            if (textIndex >= 0)
            {
                textStartingAlphas[textIndex] = newColor.a;
            }
        }
    }

    private void Awake()
    {
        if (popupText == null)
        {
            popupText = GetComponentInChildren<TMP_Text>();
        }

        SpriteRenderer[] foundSprites = GetComponentsInChildren<SpriteRenderer>();
        for (int index = 0; index < foundSprites.Length; index++)
        {
            spriteRenderers.Add(foundSprites[index]);
            spriteStartingAlphas.Add(foundSprites[index].color.a);
        }

        TMP_Text[] foundTexts = GetComponentsInChildren<TMP_Text>();
        for (int index = 0; index < foundTexts.Length; index++)
        {
            textComponents.Add(foundTexts[index]);
            textStartingAlphas.Add(foundTexts[index].color.a);
        }

        Renderer[] foundRenderers = GetComponentsInChildren<Renderer>();
        for (int index = 0; index < foundRenderers.Length; index++)
        {
            Renderer currentRenderer = foundRenderers[index];

            if (currentRenderer is SpriteRenderer)
            {
                continue;
            }

            if (currentRenderer.GetComponent<TMP_Text>() != null)
            {
                continue;
            }

            if (currentRenderer.material.HasProperty("_Color"))
            {
                meshRenderers.Add(currentRenderer);
                meshStartingAlphas.Add(currentRenderer.material.color.a);
            }
        }
    }

    private void Update()
    {
        elapsedSeconds += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedSeconds / lifetimeSeconds);

        transform.position += Vector3.up * upwardSpeed * Time.deltaTime;

        SetAlphaMultiplier(1.0f - progress);

        if (elapsedSeconds >= lifetimeSeconds)
        {
            Destroy(gameObject);
        }
    }

    private void SetAlphaMultiplier(float alphaMultiplier)
    {
        for (int index = 0; index < spriteRenderers.Count; index++)
        {
            Color spriteColor = spriteRenderers[index].color;
            spriteColor.a = spriteStartingAlphas[index] * alphaMultiplier;
            spriteRenderers[index].color = spriteColor;
        }

        for (int index = 0; index < textComponents.Count; index++)
        {
            Color textColor = textComponents[index].color;
            textColor.a = textStartingAlphas[index] * alphaMultiplier;
            textComponents[index].color = textColor;
        }

        for (int index = 0; index < meshRenderers.Count; index++)
        {
            Color meshColor = meshRenderers[index].material.color;
            meshColor.a = meshStartingAlphas[index] * alphaMultiplier;
            meshRenderers[index].material.color = meshColor;
        }
    }
}
