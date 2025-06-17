using System.Collections.Generic;
using System;

public class CommandInvoker
{
    private List<Command> commands = new List<Command>();

    public void ExecuteCommand(Command command)
    {
        command.Execute();
    }

    public virtual void AddCommand(Command command)
    {
        commands.Add(command);
    }

    public virtual void RemoveCommand(Command command)
    {
        commands.Remove(command);
    }
}

public class BuffCommandInvoker : CommandInvoker
{
    #region Variables
    private float _buffRecycleTime = 10.0f;

    private Queue<BuffCommand> _addQueue                     = new Queue<BuffCommand>();
    private Queue<BuffCommand> _inactiveQueue                = new Queue<BuffCommand>();
    private Queue<BuffCommand> _removeQueue                  = new Queue<BuffCommand>();
    private Queue<Tuple<BuffCommand, int>> _changeLevelQueue = new Queue<Tuple<BuffCommand, int>>();
                                                            
    private Dictionary<int, BuffCommand> _activeBuffs     = new Dictionary<int, BuffCommand>();
    private Dictionary<int, BuffCommand> _inactiveBuffs   = new Dictionary<int, BuffCommand>();
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    public void Update(float deltaTime)
    {
        UpdateActiveBuffLevel();
        UpdateActiveBuffs(deltaTime);
        UpdateInactiveBuffs(deltaTime);
        RemoveExpiredBuffs();

        AddNewBuffs();

        // 비활성화 큐에 있는 원소 inactiveCommands에 추가
        foreach (var command in _inactiveQueue)
        {
            _inactiveBuffs.Add(command.buffId, command);
        }
        _inactiveQueue.Clear();
    }


    #endregion

    #region Initialization
    #endregion

    #region Actions
    /// <summary>
    /// 버프 생성전에 각 Dictionary를 Find해서 Dictionary에 원소가 존재하면 재사용하기
    /// </summary>
    /// <param name="commandID"></param>
    /// <returns></returns>
    public BuffCommand FindCommand(int commandID)
    {
        if (_activeBuffs.TryGetValue(commandID, out BuffCommand buffCommand))
            return buffCommand;

        if (_inactiveBuffs.TryGetValue(commandID, out BuffCommand inactiveCommand))
            return inactiveCommand;

        return null;
    }

    public void RemoveAllBuffs()
    {
        _activeBuffs.Clear();
        _inactiveBuffs.Clear();
    }

    private void UpdateActiveBuffLevel()
    {
        foreach (var levelCommandTuple in _changeLevelQueue)
        {
            BuffCommand buffInfo = levelCommandTuple.Item1;
            int updatedLevel = levelCommandTuple.Item2;

            if (_activeBuffs.TryGetValue(buffInfo.buffId, out BuffCommand activeBuff))
            {
                if (!activeBuff.isActive) continue;

                if (activeBuff is ILevelable levelableBuff)
                {
                    levelableBuff.SetLevel(updatedLevel);
                }
            }
        }

        _changeLevelQueue.Clear();
    }

    private void UpdateActiveBuffs(float deltaTime)
    {
        // Tick effect, Time Update
        foreach (var activeBuff in _activeBuffs.Values)
        {
            if (!activeBuff.isActive) continue;

            if (activeBuff is ITickable tickableBuff)
            {
                tickableBuff.Tick(deltaTime);
            }

            if (activeBuff is ITemporary temporaryBuff)
            {
                temporaryBuff.Update(deltaTime);

                if (!activeBuff.isActive)
                {
                    _inactiveQueue.Enqueue(activeBuff);
                }
            }
        }
    }

    private void UpdateInactiveBuffs(float deltaTime)
    {
        foreach (var inactiveBuff in _activeBuffs.Values)
        {
            if (inactiveBuff is ITemporary temporaryBuff)
            {
                temporaryBuff.Update(deltaTime);
                if (temporaryBuff.ElapsedTime >= _buffRecycleTime)
                {
                    _removeQueue.Enqueue(inactiveBuff);
                }
            }
        }
    }

    private void RemoveExpiredBuffs()
    {
        foreach (var expiredBuff in _removeQueue)
        {
            expiredBuff.EndBuff();
            _activeBuffs.Remove(expiredBuff.buffId);
            _inactiveBuffs.Remove(expiredBuff.buffId);
        }

        _removeQueue.Clear();
    }

    private void AddNewBuffs()
    {
        // TODO: Implement buff recycle system
        foreach (var newBuff in _addQueue)
        {
            if (_activeBuffs.TryGetValue(newBuff.buffId, out BuffCommand buffCommand))
            {
                if (buffCommand is ITemporary temporaryBuffCommand)
                {
                    temporaryBuffCommand.ElapsedTime = 0f;
                    continue;
                }
            }

            if (_inactiveBuffs.TryGetValue(newBuff.buffId, out BuffCommand inactiveCommand))
            {
                if (inactiveCommand is ITemporary inactiveTemporaryBuffCommand)
                {
                    inactiveCommand.isActive = true;
                    inactiveTemporaryBuffCommand.ElapsedTime = 0f;
                    _inactiveBuffs.Remove(inactiveCommand.buffId);
                }
            }

            _activeBuffs.Add(newBuff.buffId, newBuff);
            newBuff.Execute();
        }
        _addQueue.Clear();
    }



    public override void AddCommand(Command command)
    {
        if (command is BuffCommand buffCommand)
        {
            _addQueue.Enqueue(buffCommand);
        }
    }

    public override void RemoveCommand(Command command)
    {
        if (command is BuffCommand buffCommand)
            _removeQueue.Enqueue(buffCommand);
    }

    public void ChangeCommandLevel(Command command, int level)
    {
        if (command is BuffCommand buffCommand)
        {
            _changeLevelQueue.Enqueue(new System.Tuple<BuffCommand, int>(buffCommand, level));
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
