using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInkType : MonoBehaviour
{
    #region Variables
    private InkType basicAttackInkType;
    private InkType skillInkType;
    private InkType dashInkType;
    #endregion

    #region Properties
    public InkType BasicAttackInkType
    {
        get => basicAttackInkType;
        set 
        { 
            basicAttackInkType = value;
            playerUI.SetBasicAttackInkTypeImage(basicAttackInkType);
        }
    }
    public InkType SkillInkType
    {
        get => skillInkType;
        set 
        { 
            skillInkType = value;
            playerUI.SetSkillJoystickImage(skillInkType);
        }
    }
    public InkType DashInkType { get => dashInkType; 
        set 
        { 
            dashInkType = value;
            playerUI.SetDashJoystickImage(dashInkType);
        } 
    }
    #endregion

    private PlayerUI playerUI;

    private void Awake()
    {
        playerUI = DebugUtils.GetComponentWithErrorLogging<PlayerUI>(this.gameObject, "PlayerUI");
    }
    // Start is called before the first frame update
    void Start()
    {
        basicAttackInkType = InkType.RED;
        skillInkType = InkType.RED;
        dashInkType = InkType.RED;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BasicAttackInkType = InkType.RED;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BasicAttackInkType = InkType.GREEN;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BasicAttackInkType = InkType.BLUE;
        }
    }
}
