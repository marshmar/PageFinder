using UnityEngine;

public class EnemyBuff : EntityBuff
{

    private void Awake()
    {
        buffCommandInvoker = new BuffCommandInvoker();
    }

    private void Update()
    {
        buffCommandInvoker.Update(Time.deltaTime);
    }

    private void OnDisable()
    {
        buffCommandInvoker.RemoveAllBuffs();
    }

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
            Debug.Log("버프 제거");
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
}
