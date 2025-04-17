using HarmonyLib;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;

namespace Ardot.REPO.EnemyOverhaul;

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
    public bool AllExtractionPointsCompleted = false;

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
            if(currentExtraction != null && ((ExtractionPoint.State)currentExtraction.Get("currentState") != ExtractionPoint.State.Active || (bool)currentExtraction.Get("stateStart")))
                goto Break;

            if(CurrentExtraction == null)
                ExtractionUnlockedImpulse = true;
            else
                ExtractionCompletedImpulse = true;

            CurrentExtraction = currentExtraction;
            AllExtractionPointsCompleted = (bool)RoundDirector.instance.Get("allExtractionPointsCompleted");
        }

        Break:

        if(ExtractionUnlockedImpulse)
        {
            AccessTools.Method(typeof(ExtractionPoint), "HaulGoalSet").Invoke(CurrentExtraction, [(int)RoundDirector.instance.Get("haulGoal") / (int)RoundDirector.instance.Get("extractionPoints") + ExtractionOverflow * 12 / 10]);
            AccessTools.Method(typeof(ExtractionPoint), "SetHaulText").Invoke(CurrentExtraction, null);
            ExtractionOverflow = 0;
        }
    }

    public void DestroyExtractionPoint(ExtractionPoint extraction)
    {
        extraction.enabled = false;
        ExtractionOverflow += extraction.haulGoal;
        RoundDirector.instance.Set("extractionPointActive", false);
        RoundDirector.instance.ExtractionPointsUnlock();
        RoundDirector.instance.ExtractionCompleted();

        const int DestructionFee = 5;

		SemiFunc.StatSetRunCurrency(SemiFunc.StatGetRunCurrency() - DestructionFee);
        CurrencyUI.instance.Show();
        CurrencyUI.instance.Set("showTimer", 2f);

        int extractionPoints = (int)RoundDirector.instance.Get("extractionPoints");
        int extractionPointsCompleted = (int)RoundDirector.instance.Get("extractionPointsCompleted");

        if(extractionPoints - extractionPointsCompleted <= 0)
            GameOver();
        else
        {
            SemiFunc.UIBigMessage($"EXTRACTION DESTROYED\n${DestructionFee * 1000} FINED", "{!}", 25f, Color.red, Color.red);
            BigMessageUI.instance.Set("bigMessageTimer", 3f);
        }
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    public IEnumerator GameOverCoroutine()
    {
        SemiFunc.UIBigMessage($"ESSENTIAL ASSETS DAMAGED", "{!}", 30f, Color.red, Color.red);
        BigMessageUI.instance.Set("bigMessageTimer", 3f);

        yield return new WaitForSeconds(6f);

        for(int x = 0; x < GameDirector.instance.PlayerList.Count; x++)
            GameDirector.instance.PlayerList[x].PlayerDeath(-1);

        yield break;
    }
}