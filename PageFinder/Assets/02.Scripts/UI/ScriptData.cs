using UnityEngine;

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

    public void CopyData(ScriptData other)
    {
        this.scriptId = other.scriptId;
        this.scriptName = other.scriptName;
        this.scriptDesc = other.scriptDesc;
        this.scriptIcon = other.scriptIcon;
        this.scriptBG = other.scriptBG;
        this.scriptType = other.scriptType;
        this.inkType = other.inkType;
        this.price = other.price;
        this.level = other.level;

        percentages = new float[3] { other.percentages[0], other.percentages[1], other.percentages[2] };
    }
}