using System.Collections.Generic;
using NUnit.Framework.Constraints;
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
    // 삽입/비활성화/삭제 시 대기큐
    private Queue<BuffCommand> addQueue = new Queue<BuffCommand>();
    private Queue<BuffCommand> inactiveQueue = new Queue<BuffCommand>();
    private Queue<BuffCommand> removeQueue = new Queue<BuffCommand>();

    private Dictionary<int, BuffCommand> activeCommands = new Dictionary<int, BuffCommand>();
    private Dictionary<int, BuffCommand> inactiveCommands = new Dictionary<int, BuffCommand>();

    private float buffRecycleTime = 10.0f;

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

    public void Update(float deltaTime)
    {
        // 액티브 커맨드 업데이트
        // activeCommands에 있는 타입이 TemporaryBuffCommand일 경우에만 tick함수 실행, 그외에는 continue
        if(activeCommands.Count > 0)
        {
            foreach (var command in activeCommands.Values)
            {
                if (!command.active) continue;

                if (command is TemporaryBuffCommand temporaryBuffCommand)
                {
                    temporaryBuffCommand.Tick(deltaTime);
                    if (!temporaryBuffCommand.active)
                    {
                        inactiveQueue.Enqueue(temporaryBuffCommand);
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
                if(command is TemporaryBuffCommand temporaryBuffCommand)
                {
                    temporaryBuffCommand.Tick(deltaTime);
                    if(temporaryBuffCommand.ElapsedTime >= buffRecycleTime)
                    {
                        removeQueue.Enqueue(temporaryBuffCommand);
                    }
                }
            }
        }

        // Remove 큐에 있는 원소 제거
        foreach(var command in removeQueue)
        {
            activeCommands.Remove(command.buffID);
            inactiveCommands.Remove(command.buffID);
        }
        removeQueue.Clear();
        
        // Add 큐에 있는 원소 activeCommands에 추가
        // Add 큐에서 버프 추가할 시에 이미 버프가 존재하면 지속시간 초기화 하고 continue
        foreach(var command in addQueue)
        {
            if(activeCommands.TryGetValue(command.buffID, out BuffCommand buffCommand))
            {
                if(buffCommand is TemporaryBuffCommand temporaryBuffCommand)
                {
                    temporaryBuffCommand.ElapsedTime = 0f;
                    continue;
                }
            }

            if(inactiveCommands.TryGetValue(command.buffID, out BuffCommand inactiveCommand))
            {
                if(inactiveCommand is TemporaryBuffCommand inactiveTemporaryBuffCommand)
                {
                    inactiveTemporaryBuffCommand.active = true;
                    inactiveTemporaryBuffCommand.ElapsedTime = 0f;
                    inactiveCommands.Remove(inactiveTemporaryBuffCommand.buffID);
                }
            }

            activeCommands.Add(command.buffID, command);
            command.Execute();
        }
        addQueue.Clear();

        // 비활성화 큐에 있는 원소 inactiveCommands에 추가
        foreach(var command in inactiveQueue)
        {
            inactiveCommands.Add(command.buffID, command);
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
}
