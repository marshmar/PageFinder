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

    [SerializeField] private Sprite[] stickerIcons;
    [Header("Background")]
    [SerializeField] private Sprite[] scriptBackgrounds;
    [SerializeField] private Sprite[] stickerBackgrounds;

    public void Map(ref Sprite scriptIcon, ref Sprite scriptBackground, string inkType, string type, int scriptId)
    {
        // 체감 온도
        if (scriptId == 5)
        {
            scriptBackground = scriptBackgrounds[0];
            scriptIcon = passiveScriptIcons[0];
        }
        // 억센 덩쿨
        else if (scriptId == 9)
        {
            scriptBackground = scriptBackgrounds[1];
            scriptIcon = passiveScriptIcons[1];
        }
        // 초목의 기운
        else if (scriptId == 10)
        {
            scriptBackground = scriptBackgrounds[1];
            scriptIcon = passiveScriptIcons[1];
        }
        // 물 절약
        else if (scriptId == 14)
        {
            scriptBackground = scriptBackgrounds[2];
            scriptIcon = passiveScriptIcons[2];
        }
        // 깊은 우물
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

    public Sprite GetScriptBackground(InkType inkType)
    {
        Sprite backGround = null;
        switch (inkType)
        {
            case InkType.RED:
                backGround = scriptBackgrounds[0];
                break;
            case InkType.GREEN:
                backGround = scriptBackgrounds[1];
                break;
            case InkType.BLUE:
                backGround = scriptBackgrounds[2];
                break;
        }

        return backGround;
    }

    public Sprite GetStickerBackground()
    {
        return stickerBackgrounds[0];
    }

    public Sprite GetScriptIconByID(int scriptID)
    {
        Sprite icon = null;
        return icon;
    }

    public Sprite GetScriptIconByScriptTypeAndInkType(NewScriptData.ScriptType scriptType, InkType inkType)
    {
        Sprite scriptIcon = null;

        switch (inkType)
        {
            case InkType.RED:
                if (scriptType == NewScriptData.ScriptType.BasicAttack)
                {
                    scriptIcon = scriptIconReds[0];
                }
                else if (scriptType == NewScriptData.ScriptType.Dash)
                {
                    scriptIcon = scriptIconReds[1];
                }
                else if (scriptType == NewScriptData.ScriptType.Skill)
                {
                    scriptIcon = scriptIconReds[2];
                }

                break;
            case InkType.GREEN:
                if (scriptType == NewScriptData.ScriptType.BasicAttack)
                {
                    scriptIcon = scriptIconGreens[0];
                }
                else if (scriptType == NewScriptData.ScriptType.Dash)
                {
                    scriptIcon = scriptIconGreens[1];
                }
                else if (scriptType == NewScriptData.ScriptType.Skill)
                {
                    scriptIcon = scriptIconGreens[2];
                }
                break;
            case InkType.BLUE:
                if (scriptType == NewScriptData.ScriptType.BasicAttack)
                {
                    scriptIcon = scriptIconBlues[0];
                }
                else if (scriptType == NewScriptData.ScriptType.Dash)
                {
                    scriptIcon = scriptIconBlues[1];
                }
                else if (scriptType == NewScriptData.ScriptType.Skill)
                {
                    scriptIcon = scriptIconBlues[2];
                }
                break;
        }
        return scriptIcon;  
    }

    public Sprite GetStickerIconByID(int stickerID)
    {
        switch (stickerID)
        {
            case 101:
                return stickerIcons[0];
            case 102:
                return stickerIcons[1];
            case 103:
                return stickerIcons[2];
            case 104:
                return stickerIcons[3];
            case 105:
                return stickerIcons[4];
            case 106:
                return stickerIcons[5];
            case 107:
                return stickerIcons[6];
            case 108:
                return stickerIcons[7];
            case 109:
                return stickerIcons[8];
        }

        return null;
    }
}
