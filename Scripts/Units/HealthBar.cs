using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private GameObject fillGM;
    private SpriteRenderer fillRenderer;

    private GameObject backgroundGM;
    private SpriteRenderer backgroundRenderer;

    private GameObject healthBarContainer;

    private Gradient gradient;

    private float maxWidth = 0.85f;

    private Vector3 offset = new Vector3(0f, -0.5f);

    public void SetHealth(float current, float maxHealth)
    {
        float healthPercent = Mathf.Clamp01(current / maxHealth);
        fillGM.transform.localScale = new Vector2(maxWidth * healthPercent, 0.2f);
        fillRenderer.color = gradient.Evaluate(healthPercent);
    }

    public void Initialize(Transform targetTransform)
    {
        gradient = new Gradient()
        {
            colorKeys = new[]
            {
                new GradientColorKey(Color.green, 1f),
                new GradientColorKey(Color.yellow, 0.6f),
                new GradientColorKey(Color.red, 0f)
            }
        };

        healthBarContainer = new GameObject("HealthBarContainer");
        healthBarContainer.transform.SetParent(targetTransform, false);
        healthBarContainer.transform.localPosition = offset;

        //—оздадим "задник" нашего хп бара
        backgroundGM = new GameObject("HealthBarBG");
        backgroundGM.transform.SetParent(healthBarContainer.transform, false);
        backgroundRenderer = backgroundGM.AddComponent<SpriteRenderer>();
        backgroundRenderer.color = Color.black;
        backgroundRenderer.sprite = GenerateBarSprite();
        backgroundGM.transform.localScale = new Vector3(maxWidth, 0.2f);
        backgroundRenderer.sortingLayerName = "UI";
        backgroundRenderer.sortingOrder = 1;

        //—оздадим саму заполн€ющую линию
        fillGM = new GameObject("HealthBarFill");
        fillGM.transform.SetParent(healthBarContainer.transform, false);
        fillRenderer = fillGM.AddComponent<SpriteRenderer>();
        fillRenderer.color = Color.green;
        fillRenderer.sprite = GenerateBarSprite();
        fillGM.transform.localScale = new Vector3(maxWidth, 0.2f);
        fillRenderer.sortingLayerName = "UI";
        fillRenderer.sortingOrder = 2;
    }

    private Sprite GenerateBarSprite()
    {
        Texture2D texture2D = new Texture2D(100, 50);
        texture2D.SetPixel(0, 0, Color.white);
        texture2D.Apply();
        return Sprite.Create(texture2D, new Rect(0, 0, 100, 50), new Vector2(0.5f, 0.5f));
    }

    public void HideHealtbar()
    {
        fillGM.SetActive(false);
        backgroundGM.SetActive(false);
    }

    public void ShowHealtbar()
    {
        fillGM.SetActive(true);
        backgroundGM.SetActive(true);
    }

}
