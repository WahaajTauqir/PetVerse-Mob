using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    [SerializeField] private GeneralManager gm;
    [Header("Mood Influence")]
    [SerializeField, Range(1f, 5f)] private float moodFactor = 2f;

    public void RealtimeActionsReaction(ActionType action)
    {
        switch (action)
        {
            case ActionType.Feed:
                if (gm.needsSystem.needs.food < 20f)
                {

                }
                else if (gm.needsSystem.needs.food > 80f)
                {

                }
                else
                {

                }
                break;

            case ActionType.Play:
                if (gm.needsSystem.needs.energy < 20f)
                {

                }
                else if (gm.needsSystem.needs.energy > 80f)
                {

                }
                else
                {

                }
                break;

            case ActionType.Groom:
                if (gm.needsSystem.needs.hygiene < 20f)
                {

                }
                else if (gm.needsSystem.needs.hygiene > 80f)
                {

                }
                else
                {

                }
                break;

            case ActionType.Sleep:
                if (gm.needsSystem.needs.energy < 20f)
                {

                }
                else if (gm.needsSystem.needs.energy > 80f)
                {

                }
                else
                {

                }
                break;

            default:
                break;
        }
    }
    
    public void RealtimeTricksReaction(TrickType trick)
    {
        var trickObj = gm.tricksSystem.tricks.Find(t => t.trickType == trick);
        if (trickObj == null) return;

        float moodScore = gm.moodSystem.GetMoodScore();

        float masteryIncrement = Mathf.Lerp(0.1f, 1.0f, Mathf.Clamp01((moodScore * moodFactor) / 100f));

        trickObj.masteryLevel += masteryIncrement;
        trickObj.masteryLevel = Mathf.Clamp(trickObj.masteryLevel, 0f, 100f);

        if (trickObj.masteryLevel >= 100f)
        {
            trickObj.isLearned = true;
        }
    }
}
