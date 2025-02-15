using System.Collections.Generic;
using UnityEngine;

public class RiddleData : ScriptableObject
{
    public int stageNum;
    public string positiveConversation;
    public string neagativeConversation;
    public string problem;
    public string[] options = new string[3];
    public List<string> conversations = new List<string>();
}
