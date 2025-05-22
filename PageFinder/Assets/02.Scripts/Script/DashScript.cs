using UnityEngine;

public class DashScript : BaseScript
{
    public DashScript()
    {
        scriptBehaviour = new DashBehaviour();
    }
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
