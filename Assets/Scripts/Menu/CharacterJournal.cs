using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterJournal : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;   // una tarjeta por personaje
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;

    private int index;

    private void Awake()
    {
        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);
    }

    private void OnEnable()
    {
        index = 0;
        Refresh();
    }

    public void Next() => Move(1);
    public void Previous() => Move(-1);

    private void Move(int delta)
    {
        if (cards.Count == 0) return;
        index = (index + delta + cards.Count) % cards.Count;
        Refresh();
    }

    private void Refresh()
    {
        for (int i = 0; i < cards.Count; i++)
            cards[i].SetActive(i == index);

        leftArrow.interactable = cards.Count > 1;
        rightArrow.interactable = cards.Count > 1;
    }
}