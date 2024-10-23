using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Dailogue/Dialogue Container")]
public class DialogueContainer : ScriptableObject
{
    [TextArea(5,10)]
    public string[] dialogues;
    public bool isInControl;
}
