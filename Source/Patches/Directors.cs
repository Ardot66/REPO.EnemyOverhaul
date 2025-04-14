using HarmonyLib;
using UnityEngine.Events;
using UnityEngine;

namespace Ardot.REPO.REPOverhaul;

public static class DirectorPatches
{
    public static void Patch()
    {
        Plugin.Harmony.Patch(
            AccessTools.Method(typeof(GameDirector), "Awake"),
            postfix: new HarmonyMethod(typeof(DirectorPatches), "AwakePostfix")
        );
    }

    public static void AwakePostfix(GameDirector __instance)
    {
        __instance.gameObject.AddComponent<OverhaulDirector>();
    }
}

public class OverhaulDirector : MonoBehaviour
{
    public static OverhaulDirector Instance;

    public bool ExtractionCompletedImpulse = new ();
    public bool ExtractionUnlockedImpulse = new ();

    public int ExtractionOverflow = 0;

    public ExtractionPoint CurrentExtraction = null;

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        ExtractionUnlockedImpulse = false;
        ExtractionCompletedImpulse = false;

        ExtractionPoint currentExtraction = (ExtractionPoint)RoundDirector.instance.Get("extractionPointCurrent");
        if(currentExtraction != CurrentExtraction)
        {
            if(CurrentExtraction == null)
                ExtractionUnlockedImpulse = true;
            else
                ExtractionCompletedImpulse = true;

            CurrentExtraction = currentExtraction;
        }

        if(ExtractionUnlockedImpulse)
        {
            AccessTools.Method(typeof(ExtractionPoint), "HaulGoalSet").Invoke(CurrentExtraction, [CurrentExtraction.haulGoal + ExtractionOverflow * 1.2f]);
            AccessTools.Method(typeof(ExtractionPoint), "SetHaulText").Invoke(CurrentExtraction, null);
            ExtractionOverflow = 0;
        }
    }

    public void DestroyExtractionPoint(ExtractionPoint extraction)
    {
        extraction.enabled = false;
        ExtractionOverflow += extraction.haulGoal;
        RoundDirector.instance.ExtractionCompleted();

        int extractionPoints = (int)RoundDirector.instance.Get("extractionPoints");
        int extractionPointsCompleted = (int)RoundDirector.instance.Get("extractionPointsCompleted");

        if(extractionPoints - extractionPointsCompleted <= 0)
        {
            // Game over logic
        }
    }
}