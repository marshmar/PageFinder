using UnityEngine;

public class DashScript : BaseScript
{
    private float dashPower = 4.0f;
    private float dashWidth = 2.0f;
    private Stat dashDuration = new Stat(0.2f);
    private float dashCoolTime = 0.3f;
    private Stat dashCost = new Stat(30.0f);

    public DashScript()
    {
        scriptBehaviour = new DashBehaviour();
        if(scriptBehaviour is DashBehaviour dashBehaviour)
        {
            dashBehaviour.SetScript(this);
        }
    }

    public Stat DashCost { get => dashCost; }
    public float DashCoolTime { get => dashCoolTime; set => dashCoolTime = value; }
    public Stat DashDuration { get => dashDuration;}
    public float DashWidth { get => dashWidth; set => dashWidth = value; }
    public float DashPower { get => dashPower; set => dashPower = value; }

    public override void Upgrade(int rarity)
    {
        switch (rarity)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}
