using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Script", menuName = "Scriptable Object/ScriptData")]
public class ScriptData : ScriptableObject
{

    public enum ScriptType { BASICATTACK, DASH, SKILL, MAGIC, PASSIVE }

    [Header("# Main Info")]
    public int scriptId;
    public string scriptName;
    public string scriptDesc;
    public Sprite scriptIcon;
    public Sprite scriptBG;
    public ScriptType scriptType;
    public InkType inkType;
    public int price;
    public int level;

    [Header("# Level Data")]
    public float[] percentages;


}
