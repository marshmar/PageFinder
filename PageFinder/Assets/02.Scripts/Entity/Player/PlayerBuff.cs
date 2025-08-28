using UnityEngine;
using System.Collections.Generic;

public class PlayerBuff : EntityBuff, IListener
{
    #region Variables
    // Hashing
    private Player _player;
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _player = this.GetComponentSafe<Player>();

        buffCommandInvoker = new BuffCommandInvoker();
    }

    private void Start()
    {
        AddListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void Update()
    {
        buffCommandInvoker.Update(Time.deltaTime);
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    public override void AddBuff(int buffID)
    {
        BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffID);
        if (buffCommand == null)
        {
            BuffData buffData = new BuffData();
            buffCommand = BuffGenerator.Instance.CreateBuffCommand(in buffData);
            buffCommandInvoker.AddCommand(buffCommand);
        }
    }

    public void AddBuff(in BuffData buffData)
    {
        BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffData.buffId);
        if (buffCommand == null)
        {
            buffCommand = BuffGenerator.Instance.CreateBuffCommand(in buffData);
            buffCommandInvoker.AddCommand(buffCommand);
        }
    }

    public override void RemoveBuff(int buffID)
    {
        BuffCommand command = buffCommandInvoker.FindCommand(buffID);
        if (command != null)
        {
            buffCommandInvoker.RemoveCommand(command);
        }
    }

    public override void ChangeBuffLevel(int buffID, int level)
    {
        BuffCommand command = buffCommandInvoker.FindCommand(buffID);
        if (command != null)
        {
            buffCommandInvoker.ChangeCommandLevel(command, level);
        }
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.AddBuff, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Create_Script, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.AddBuff, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Create_Script, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.AddBuff:
                {
                    var buffId = (int)Param;
                    BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffId);
                    if (buffCommand is null)
                    {
                        BuffData buffData = new BuffData();
                        buffCommand = BuffGenerator.Instance.CreateBuffCommand(in buffData);
                    }
                    buffCommandInvoker.AddCommand(buffCommand);
                    break;
                }
        }
    }
    #endregion



    

    

}
