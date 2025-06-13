using UnityEngine;
using System.Collections.Generic;

public class PlayerInputInvoker : MonoBehaviour
{
    #region Variables
    private List<InputCommand> addList = new List<InputCommand>();
    private List<InputCommand> removeList = new List<InputCommand>();
    private List<InputCommand> inputList = new List<InputCommand>();
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        UpdateCommands();

        RemoveCommand();
        EnqueueCommand();
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    private void RemoveCommand()
    {
        inputList.RemoveAll(input => input.IsExpired);
    }

    public void AddInputCommand(InputCommand input)
    {
        addList.Add(input);
    }

    private void UpdateCommands()
    {
        InputCommand excuteCommand = null;

        foreach(InputCommand input in inputList)
        {
            if (Time.time >= input.Timestamp + input.ExpirationTime)
            {
                input.IsExpired = true;
                continue;
            }

            if (!input.IsExcuteable()) continue;

            if (excuteCommand == null || input.Priority > excuteCommand.Priority)
                excuteCommand = input;
        }

        if (excuteCommand != null)
        {
            excuteCommand.Execute();
            inputList.Clear();
        }
    }

    private void EnqueueCommand()
    {
        while (addList.Count > 0)
        {
            var input = addList[0];
            inputList.Add(input);
            addList.RemoveAt(0);
        }

        addList.Clear();
    }

    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion


}
