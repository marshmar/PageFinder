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
    // ����/��Ȱ��ȭ/���� �� ���ť
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
        // ��Ƽ�� Ŀ�ǵ� ������Ʈ
        // activeCommands�� �ִ� Ÿ���� TemporaryBuffCommand�� ��쿡�� tick�Լ� ����, �׿ܿ��� continue
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

        // �ξ�Ƽ�� Ŀ�ǵ� ������Ʈ
        // �ξ�Ƽ�� Ŀ�ǵ��� �̻�� �ð��� buffRecycleTime �̻��̸� RemoveQueue�� �߰�
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

        // Remove ť�� �ִ� ���� ����
        foreach(var command in removeQueue)
        {
            activeCommands.Remove(command.buffID);
            inactiveCommands.Remove(command.buffID);
        }
        removeQueue.Clear();
        
        // Add ť�� �ִ� ���� activeCommands�� �߰�
        // Add ť���� ���� �߰��� �ÿ� �̹� ������ �����ϸ� ���ӽð� �ʱ�ȭ �ϰ� continue
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

        // ��Ȱ��ȭ ť�� �ִ� ���� inactiveCommands�� �߰�
        foreach(var command in inactiveQueue)
        {
            inactiveCommands.Add(command.buffID, command);
        }
        inactiveQueue.Clear();
    }

    /// <summary>
    /// ���� �������� �� Dictionary�� Find�ؼ� Dictionary�� ���Ұ� �����ϸ� �����ϱ�
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
