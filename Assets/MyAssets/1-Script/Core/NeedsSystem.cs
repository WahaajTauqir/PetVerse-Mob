using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Needs class to encapsulate all needs and their logic
// --------------------------------------------------------------------------------------------------
[System.Serializable]
public class Needs
{
    [Range(0, 100)] public float food = 50f;
    [Range(0, 100)] public float energy = 50f;
    [Range(0, 100)] public float hygiene = 50f;
    [Range(0, 100)] public float wellbeing = 50f;

    [Header("Needs Decay Rates")]
    public float foodDecay = 3f;
    public float energyDecay = 2f;
    public float hygieneDecay = 1f;
    public float wellbeingDecay = 2f;

    public void UpdateNeeds(float deltaTime, float activeGameFactor)
    {
        food -= foodDecay * deltaTime * activeGameFactor;
        energy -= energyDecay * deltaTime * activeGameFactor;
        hygiene -= hygieneDecay * deltaTime * activeGameFactor;
        wellbeing -= wellbeingDecay * deltaTime * activeGameFactor;
        ClampAll();
    }

    public void UpdateNeedsForElapsed(float hours, float inactiveGameFactor)
    {
        food -= foodDecay * hours * inactiveGameFactor;
        energy -= energyDecay * hours * inactiveGameFactor;
        hygiene -= hygieneDecay * hours * inactiveGameFactor;
        wellbeing -= wellbeingDecay * hours * inactiveGameFactor;
        ClampAll();
    }

    public void ClampAll()
    {
        food = Mathf.Clamp(food, 0f, 100f);
        energy = Mathf.Clamp(energy, 0f, 100f);
        hygiene = Mathf.Clamp(hygiene, 0f, 100f);
        wellbeing = Mathf.Clamp(wellbeing, 0f, 100f);
    }
}

public class NeedsSystem : MonoBehaviour
{
    [SerializeField] GeneralManager gm;

    [Header("Needs")]
    public Needs needs = new Needs();

    [Header("Decay Factors")]
    [SerializeField] private float activeGameFactor = 1.0f;
    [SerializeField] private float inactiveGameFactor = 1.0f;

    [Header("Critical Need")]
    [SerializeField] private string criticalNeed = "Food";

    [Header("Interaction Factors")]
    [SerializeField] private float interactionFactor = 1.0f;

    private void Awake()
    {
        LoadNeeds();
    }

    public void SaveNeeds()
    {
        PlayerPrefs.SetFloat("Need_Food", needs.food);
        PlayerPrefs.SetFloat("Need_Energy", needs.energy);
        PlayerPrefs.SetFloat("Need_Hygiene", needs.hygiene);
        PlayerPrefs.SetFloat("Need_Wellbeing", needs.wellbeing);
        PlayerPrefs.Save();
    }

    public void ApplyElapsedNeeds(float elapsedHours)
    {
        needs.UpdateNeedsForElapsed(elapsedHours, inactiveGameFactor);
        SaveNeeds();
    }

    private void LoadNeeds()
    {
        needs.food = PlayerPrefs.GetFloat("Need_Food", needs.food);
        needs.energy = PlayerPrefs.GetFloat("Need_Energy", needs.energy);
        needs.hygiene = PlayerPrefs.GetFloat("Need_Hygiene", needs.hygiene);
        needs.wellbeing = PlayerPrefs.GetFloat("Need_Wellbeing", needs.wellbeing);
        PlayerPrefs.Save();
    }

    // --------------------------------------------------------------------------------------------------
    public void UpdateNeeds(float deltaTime)
    {
        needs.UpdateNeeds(deltaTime, activeGameFactor);
        UpdateDominantNeed();
        SaveNeeds();
    }

    public void UpdateDominantNeed()
    {
        float lowestValue = float.MaxValue;
        string lowest = criticalNeed;
        if (needs.food < lowestValue)
        {
            lowestValue = needs.food;
            lowest = "Food";
        }
        if (needs.energy < lowestValue)
        {
            lowestValue = needs.energy;
            lowest = "Energy";
        }
        if (needs.hygiene < lowestValue)
        {
            lowestValue = needs.hygiene;
            lowest = "Hygiene";
        }
        if (needs.wellbeing < lowestValue)
        {
            lowestValue = needs.wellbeing;
            lowest = "Wellbeing";
        }
        criticalNeed = lowest;
    }

    // --------------------------------------------------------------------------------------------------
    public void ProcessNeeds(ActionType type)
    {
        switch (type)
        {
            case ActionType.Feed:
                needs.food += 5f * interactionFactor;
                needs.energy += 4.5f * interactionFactor;
                needs.hygiene += 0f * interactionFactor;
                needs.wellbeing += 1f * interactionFactor;
                break;

            case ActionType.Play:
                needs.food -= 2f * interactionFactor;
                needs.energy -= 5f * interactionFactor;
                needs.hygiene -= 1f * interactionFactor;
                needs.wellbeing += 4f * interactionFactor;
                break;

            case ActionType.Groom:
                needs.food += 0f * interactionFactor;
                needs.energy -= 1f * interactionFactor;
                needs.hygiene += 4f * interactionFactor;
                needs.wellbeing += 3f * interactionFactor;
                break;

            case ActionType.Sleep:
                needs.food -= 2f * interactionFactor;
                needs.energy += 5f * interactionFactor;
                needs.hygiene -= 1f * interactionFactor;
                needs.wellbeing += 4f * interactionFactor;
                break;

            case ActionType.Pet:
                needs.food += 0f * interactionFactor;
                needs.energy -= 1f * interactionFactor;
                needs.hygiene += 0f * interactionFactor;
                needs.wellbeing += 4f * interactionFactor;
                break;
        }

        needs.ClampAll();
        UpdateDominantNeed();
        gm.moodSystem.CalculateMoodScore();
        SaveNeeds();
    }

    // --------------------------------------------------------------------------------------------------
    public string GetCriticalNeed()
    {
        return criticalNeed;
    }

    // --------------------------------------------------------------------------------------------------
    public float GetFood() { return needs.food; }
    public float GetEnergy() { return needs.energy; }
    public float GetHygiene() { return needs.hygiene; }
    public float GetWellbeing() { return needs.wellbeing; }
}
