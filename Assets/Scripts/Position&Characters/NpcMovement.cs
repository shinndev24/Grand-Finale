using UnityEngine;
using UnityEngine.UI;

public class NpcMovement : MonoBehaviour
{
    public Image image;
    public int id;
    public bool isPlayer;
    public string displayName;

    public Sprite deadSprite;
    public Sprite normalSprite;

    private RectTransform rt;
    private Vector2 originalSize;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        originalSize = rt.sizeDelta;
        if (normalSprite == null) normalSprite = image.sprite;
    }

    public void MoveTo(RectTransform slot)
    {
        transform.position = slot.position;
    }

    public void SetVisible(bool visible)
    {
        image.enabled = visible;
    }

    public void SetDead(bool squashed)
    {
        image.sprite = squashed ? deadSprite : normalSprite;
    }

    // Mantiene el tamaño y la escala propios, ignorando los del slot
    public void KeepOriginalSize()
    {
        rt.localScale = Vector3.one;
        rt.sizeDelta = originalSize;
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
    }
}
