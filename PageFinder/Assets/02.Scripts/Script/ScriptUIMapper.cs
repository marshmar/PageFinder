using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptUIMapper : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private Sprite[] scriptIconReds;
    [SerializeField] private Sprite[] scriptIconGreens;
    [SerializeField] private Sprite[] scriptIconBlues;
    [SerializeField] private Sprite[] passiveScriptIcons;

    [Header("Background")]
    [SerializeField] private Sprite[] scriptBackgrounds;

    public void Map(ref Sprite scriptIcon, ref Sprite scriptBackground, string inkType, string type, int scriptId)
    {
        // ü�� �µ�
        if (scriptId == 5)
        {
            scriptBackground = scriptBackgrounds[0];
            scriptIcon = passiveScriptIcons[0];
        }
        // �＾ ����
        else if (scriptId == 9)
        {
            scriptBackground = scriptBackgrounds[1];
            scriptIcon = passiveScriptIcons[1];
        }
        // �ʸ��� ���
        else if (scriptId == 10)
        {
            scriptBackground = scriptBackgrounds[1];
            scriptIcon = passiveScriptIcons[1];
        }
        // �� ����
        else if (scriptId == 14)
        {
            scriptBackground = scriptBackgrounds[2];
            scriptIcon = passiveScriptIcons[2];
        }
        // ���� �칰
        else if (scriptId == 15)
        {
            scriptBackground = scriptBackgrounds[2];
            scriptIcon = passiveScriptIcons[3];
        }
        else
        {
            switch (inkType)
            {
                case "RED":
                    scriptBackground = scriptBackgrounds[0];
                    if (type == "BASICATTACK")
                    {
                        scriptIcon = scriptIconReds[0];

                    }
                    else if (type == "DASH")
                    {
                        scriptIcon = scriptIconReds[1];
                    }
                    else if (type == "SKILL")
                    {
                        scriptIcon = scriptIconReds[2];
                    }

                    break;
                case "GREEN":
                    scriptBackground = scriptBackgrounds[1];
                    if (type == "BASICATTACK")
                    {
                        scriptIcon = scriptIconGreens[0];

                    }
                    else if (type == "DASH")
                    {
                        scriptIcon = scriptIconGreens[1];
                    }
                    else if (type == "SKILL")
                    {
                        scriptIcon = scriptIconGreens[2];
                    }
                    break;
                case "BLUE":
                    scriptBackground = scriptBackgrounds[2];
                    if (type == "BASICATTACK")
                    {
                        scriptIcon = scriptIconBlues[0];
                    }
                    else if (type == "DASH")
                    {
                        scriptIcon = scriptIconBlues[1];
                    }
                    else if (type == "SKILL")
                    {
                        scriptIcon = scriptIconBlues[2];
                    }
                    break;
            }
        }
    }

}
