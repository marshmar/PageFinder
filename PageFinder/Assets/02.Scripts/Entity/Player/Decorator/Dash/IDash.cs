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

    public void DashMovement(PlayerUtils playerUtils, Vector3? dir = null);
    public void EndDash(PlayerUtils playerUtils);
    public IEnumerator DashCoroutine
        (
        Vector3? dashDir,
        PlayerUtils playerUtils,
        PlayerAnim playerAnim,
        PlayerState playerState
        );

    public void GenerateInkMark(PlayerInkType playerInkType, PlayerUtils playerUtils);
    public IEnumerator ExtraEffectCoroutine(Component component);
}
