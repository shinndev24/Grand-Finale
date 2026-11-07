using UnityEngine;
using UnityEngine.UI;

public class NpcMovement : MonoBehaviour
{
    public Image image;
    public int id;  
    public bool isPlayer;

    public void MoveTo(RectTransform slot)
    {
        transform.position = slot.position;
    }

    public void SetVisible(bool visible)
    {
        image.enabled = visible;
    }
}
