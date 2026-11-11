using System.Collections.Generic;
using UnityEngine;

public class SlotPresenter : MonoBehaviour
{
    public List<RectTransform> slots;
    public List<NpcMovement> characters;

    private Dictionary<int, NpcMovement> byId = new();
    public bool HasId(int id) => byId.ContainsKey(id);


    void Awake()
    {
        foreach (var c in characters) byId[c.id] = c;
    }

    public void PlaceCharacters(Dictionary<int, int> slotToCharacter)
    {
        foreach (var kv in slotToCharacter)
        {
            int slot = kv.Key;
            var ch = byId[kv.Value];
            ch.SetVisible(true);
            var rt = ch.GetComponent<RectTransform>();
            rt.SetParent(slots[slot - 1], false);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    public void RevealDeath(int id)
    {
        byId[id].SetDead(true);
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

