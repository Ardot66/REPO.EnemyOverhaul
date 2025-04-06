using System.Collections.Generic;
using UnityEngine;

namespace Ardot.REPO.REPOverhaul;

public static class GnomePatches
{
    public static void GameStart()
    {
        // Reduces the damage gnomes do to items to a more reasonable amount

        GameObject gnome = Plugin.Enemies["gnome"];
        List<HurtCollider> gnomeHurtColliders = Utils.GetHurtColliders(gnome.transform);

        for(int x = 0; x < gnomeHurtColliders.Count; x++)
        {
            HurtCollider hurtCollider = gnomeHurtColliders[x];
            hurtCollider.physImpact = HurtCollider.BreakImpact.Light;
        }
    }
}