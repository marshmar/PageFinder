using System.Collections.Generic;
using UnityEngine;

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
    // 삽입/비활성화/삭제/업데이트/레벨변화 시 대기큐
    private Queue<BuffCommand> addQueue = new Queue<BuffCommand>();
    private Queue<BuffCommand> inactiveQueue = new Queue<BuffCommand>();
    private Queue<BuffCommand> removeQueue = new Queue<BuffCommand>();
    private Queue<System.Tuple<BuffCommand, int>> changeLevelQueue = new Queue<System.Tuple<BuffCommand, int>>();

    private Dictionary<int, BuffCommand> activeCommands = new Dictionary<int, BuffCommand>();
    private Dictionary<int, BuffCommand> inactiveCommands = new Dictionary<int, BuffCommand>();

    private float buffRecycleTime = 10.0f;

    public Dictionary<int, BuffCommand> ActiveCommands { get => activeCommands; set => activeCommands = value; }
    public Dictionary<int, BuffCommand> InactiveCommands { get => inactiveCommands; set => inactiveCommands = value; }

    private List<BuffCommand> allBuffCommands;

    public override void AddCommand(Command command)
    {
        if(command is BuffCommand buffCommand)
        {
            addQueue.Enqueue(buffCommand);
        }
    }

    public override void RemoveCommand(Command command)
    {
        if(command is BuffCommand buffCommand)
            removeQueue.Enqueue(buffCommand);
    }

    public void ChangeCommandLevel(Command command, int level)
    {
        if (command is BuffCommand buffCommand)
        {
            changeLevelQueue.Enqueue(new System.Tuple<BuffCommand, int>(buffCommand, level));
        }
    }

    public void Update(float deltaTime)
    {
        // Update Active Commands
        if(activeCommands.Count > 0)
        {
            // Level Change
            foreach (var levelCommandTuple in changeLevelQueue)
            {
                BuffCommand command = levelCommandTuple.Item1;
                int level = levelCommandTuple.Item2;

                if (activeCommands.TryGetValue(command.buffId, out BuffCommand buffCommand))
                {
                    if (!buffCommand.active) continue;

                    if (buffCommand is ILevelable levelBuffCommand)
                    {
                        levelBuffCommand.SetLevel(level);
                    }
                }
            }
            changeLevelQueue.Clear();

            // Tick effect, Time Update
            foreach (var command in activeCommands.Values)
            {
                if (!command.active) continue;
                
                if (command is ITickable tickBuffCommanbd)
                {
                    tickBuffCommanbd.Tick(deltaTime);
                }

                if (command is ITemporary temporaryBuffCommand)
                {
                    temporaryBuffCommand.Update(deltaTime);

                    if (!command.active)
                    {
                        inactiveQueue.Enqueue(command);
                    }
                }

            }
        }

        // 인액티브 커맨드 업데이트
        // 인액티브 커맨드의 미사용 시간이 buffRecycleTime 이상이면 RemoveQueue에 추가
        if(inactiveCommands.Count > 0)
        {
            foreach(var command in inactiveCommands.Values)
            {
                if(command is ITemporary temporaryBuffCommand)
                {
                    temporaryBuffCommand.Update(deltaTime);
                    if(temporaryBuffCommand.ElapsedTime >= buffRecycleTime)
                    {
                        removeQueue.Enqueue(command);
                    }
                }
            }
        }



        // Remove 큐에 있는 원소 제거
        foreach(var command in removeQueue)
        {
            command.EndBuff();
            activeCommands.Remove(command.buffId);
            inactiveCommands.Remove(command.buffId);
        }
        removeQueue.Clear();
        
        // Add 큐에 있는 원소 activeCommands에 추가
        // Add 큐에서 버프 추가할 시에 이미 버프가 존재하면 지속시간 초기화 하고 continue
        foreach(var command in addQueue)
        {
            if (activeCommands.TryGetValue(command.buffId, out BuffCommand buffCommand))
            {
                if(buffCommand is ITemporary temporaryBuffCommand)
                {
                    temporaryBuffCommand.ElapsedTime = 0f;
                    continue;
                }
            }

            if(inactiveCommands.TryGetValue(command.buffId, out BuffCommand inactiveCommand))
            {
                if(inactiveCommand is ITemporary inactiveTemporaryBuffCommand)
                {
                    inactiveCommand.active = true;
                    inactiveTemporaryBuffCommand.ElapsedTime = 0f;
                    inactiveCommands.Remove(inactiveCommand.buffId);
                }
            }

            //스크립트 교체
            if (activeCommands.ContainsKey(command.buffId))
            {
                activeCommands[command.buffId].EndBuff();
                activeCommands[command.buffId] = command;
                command.Execute();
            }
            else
            {
                activeCommands.Add(command.buffId, command);
                command.Execute();
            }
        }
        addQueue.Clear();

        // 비활성화 큐에 있는 원소 inactiveCommands에 추가
        foreach(var command in inactiveQueue)
        {
            inactiveCommands.Add(command.buffId, command);
        }
        inactiveQueue.Clear();
    }

    /// <summary>
    /// 버프 생성전에 각 Dictionary를 Find해서 Dictionary에 원소가 존재하면 재사용하기
    /// </summary>
    /// <param name="commandID"></param>
    /// <returns></returns>
    public BuffCommand FindCommand(int commandID)
    {
        if (activeCommands.TryGetValue(commandID, out BuffCommand buffCommand))
            return buffCommand;

        if(inactiveCommands.TryGetValue(commandID,out BuffCommand inactiveCommand))
            return inactiveCommand;

        return null;
    }

    public void ClearBuff()
    {
        activeCommands.Clear();
        inactiveCommands.Clear();
    }
}
