using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDash
{
    public float DashPower { get; set; }
    public float DashDuration { get; set; }
    public float DashCooltime { get; set; }
    public float DashWidth { get; set; }

    public float DashCost { get; set; }
    public bool IsDashing { get; set; }

    public void DashMovement(Player playerScr, Vector3? dir = null);
    public void EndDash(Player playerScr);
    public IEnumerator DashCoroutine
        (
        Vector3? dashDir, 
        PlayerAttackController playerAttackControllerScr,
        PlayerController playerControllerScr,
        Player playerScr
        );

    public void GenerateInkMark(PlayerInk playerInkScr, Player playerScr);
}
