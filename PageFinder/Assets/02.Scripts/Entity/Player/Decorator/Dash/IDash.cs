using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DashDecorator
{
    public float DashPower { get; set; }
    public float DashDuration { get; set; }
    public float DashCooltime { get; set; }
    public float DashWidth { get; set; }

    public float DashCost { get; set; }

    public void DashMovement(Player playerScr, Vector3? dir = null);
    public void EndDash(Player playerScr);
    public IEnumerator DashCoroutine
        (
        Vector3? dashDir, 
        Player playerScr
        );

    public void GenerateInkMark(PlayerInk playerInkScr, Player playerScr);
    public IEnumerator ExtraEffectCoroutine(Component component);
}
