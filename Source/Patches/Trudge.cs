using UnityEngine;

namespace Ardot.REPO.REPOverhaul;

public static class TrudgePatches
{
    public static void Patch()
    {

    }

    public static bool StartPrefix(EnemySlowWalker __instance)
    {
        return false;
    }
}

public class TrudgeOverride : MonoBehaviour
{

}