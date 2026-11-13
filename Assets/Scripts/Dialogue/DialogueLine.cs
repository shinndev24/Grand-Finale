using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speaker;
    public Sprite portrait;
    [TextArea(2, 5)] public string text;
}