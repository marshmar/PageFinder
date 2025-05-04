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
    // ����/��Ȱ��ȭ/����/������Ʈ/������ȭ �� ���ť
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

        // �ξ�Ƽ�� Ŀ�ǵ� ������Ʈ
        // �ξ�Ƽ�� Ŀ�ǵ��� �̻�� �ð��� buffRecycleTime �̻��̸� RemoveQueue�� �߰�
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



        // Remove ť�� �ִ� ���� ����
        foreach(var command in removeQueue)
        {
            command.EndBuff();
            activeCommands.Remove(command.buffId);
            inactiveCommands.Remove(command.buffId);
        }
        removeQueue.Clear();
        
        // Add ť�� �ִ� ���� activeCommands�� �߰�
        // Add ť���� ���� �߰��� �ÿ� �̹� ������ �����ϸ� ���ӽð� �ʱ�ȭ �ϰ� continue
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

            //��ũ��Ʈ ��ü
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

        // ��Ȱ��ȭ ť�� �ִ� ���� inactiveCommands�� �߰�
        foreach(var command in inactiveQueue)
        {
            inactiveCommands.Add(command.buffId, command);
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

    public void ClearBuff()
    {
        activeCommands.Clear();
        inactiveCommands.Clear();
    }
}
