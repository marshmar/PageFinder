using UnityEngine;
using System.Collections.Generic;

public class PlayerInputInvoker : MonoBehaviour
{
    #region Variables
    private List<InputCommand> _addList      = new List<InputCommand>();
    private List<InputCommand> _removeList   = new List<InputCommand>();
    private List<InputCommand> _inputList    = new List<InputCommand>();
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
        _inputList.RemoveAll(input => input.IsExpired);
    }

    public void AddInputCommand(InputCommand input)
    {
        _addList.Add(input);
    }

    private void UpdateCommands()
    {
        InputCommand excuteCommand = null;

        foreach(InputCommand input in _inputList)
        {
            if (Time.time >= input.Timestamp + input.ExpirationTimeSec)
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
            _inputList.Clear();
        }
    }

    private void EnqueueCommand()
    {
        while (_addList.Count > 0)
        {
            var input = _addList[0];
            _inputList.Add(input);
            _addList.RemoveAt(0);
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
    #endregion
}
