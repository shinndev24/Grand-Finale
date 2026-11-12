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

    void Awake()
    {
        rt = GetComponent<RectTransform>();
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
}
