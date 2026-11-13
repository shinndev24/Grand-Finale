using System.Collections.Generic;
using UnityEngine;

public class SlotPresenter : MonoBehaviour
{
    public List<RectTransform> slots;
    public List<NpcMovement> characters;

    private Dictionary<int, NpcMovement> byId = new();

    void Awake()
    {
        foreach (var c in characters) byId[c.id] = c;
    }

    public void PlaceCharacters(Dictionary<int, int> slotToCharacter)
    {
        foreach (var kv in slotToCharacter)
        {
            var ch = byId[kv.Value];
            ch.SetVisible(true);
            ch.MoveTo(slots[kv.Key - 1]);
        }
    }

    public void RevealDeath(int id)
    {
        byId[id].SetVisible(false);
    }

    public void HideCharacter(int id)
    {
        byId[id].SetVisible(false);
    }

    public void ShowAll() 
    { 
        foreach (var c in characters) c.SetVisible(true); 
    }
}

