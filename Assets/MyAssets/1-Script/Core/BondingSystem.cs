using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum BondStage
{
    Stranger,
    Acquaintance,
    Friend,
    Companion,
    Family
}

public class BondingSystem : MonoBehaviour
{
    [SerializeField] private GeneralManager gm;
    [Range(0, 100)] public float bondLevel = 0f;
    [Header("UI")]
    [Tooltip("Optional UI Slider that displays the bond level (0-100). Leave empty if not used.")]
    [SerializeField] private Slider bondLevelSlider;
    [Tooltip("Optional legacy UI Text to display the bond stage (e.g., Friend).")]
    // [SerializeField] private Text bondStageText;
    // [Tooltip("Optional TextMeshProUGUI to display the bond stage (preferred).")]
    [SerializeField] private TMP_Text bondStageTMP;

    [Header("Bonding Increase")]
    [SerializeField] float minMoodForBonding = 75f;
    [SerializeField] float bondInterval = 5f;

    [Header("Bonding Decay")]
    [SerializeField] float maxMoodForDecay = 50f;
    [SerializeField] float bondDecayInterval = 25f;

    private float bondTimer = 0f;
    private float bondDecayTimer = 0f;
    private float lastMoodScore = 0f;

    public BondStage bondStage = BondStage.Stranger;
    private BondStage lastBondStage;

    private void Awake()
    {
        lastMoodScore = PlayerPrefs.GetFloat("Last_MoodScore", 0f);
        LoadBondLevel();
        lastBondStage = bondStage;
        UpdateSlider();
    }

    public void UpdateBondLevel()
    {

        float mood = gm.GetMoodScore();

        if (mood > minMoodForBonding)
        {
            bondTimer += Time.deltaTime;
            if (bondTimer >= bondInterval)
            {
                bondLevel = Mathf.Clamp(bondLevel + 1f, 0f, 100f);
                bondTimer = 0f;
                SaveBondLevel();
            }
            bondDecayTimer = 0f;
        }
        else if (mood < maxMoodForDecay)
        {
            bondDecayTimer += Time.deltaTime;
            if (bondDecayTimer >= bondDecayInterval)
            {
                bondLevel = Mathf.Clamp(bondLevel - 1f, 0f, 100f);
                bondDecayTimer = 0f;
                SaveBondLevel();
            }
            bondTimer = 0f;
        }
        else
        {
            bondTimer = 0f;
            bondDecayTimer = 0f;
        }

        UpdateBondStage();
        UpdateSlider();
    }

    public void SaveBondLevel()
    {
        PlayerPrefs.SetFloat("Bond_Level", bondLevel);

        PlayerPrefs.SetFloat("Last_MoodScore", gm.GetMoodScore());

        PlayerPrefs.Save();

        gm.firebaseDataManager.FirebaseUpdateBondStage(bondLevel);
    }

    public void LoadBondLevel()
    {
        bondLevel = PlayerPrefs.GetFloat("Bond_Level", bondLevel);
        UpdateSlider();
    }

    public void ApplyElapsedBonding(float elapsedHours)
    {
        float totalSeconds = elapsedHours * 3600f;

        if (lastMoodScore > minMoodForBonding)
        {
            int intervals = Mathf.FloorToInt(totalSeconds / bondInterval);
            if (intervals > 0)
            {
                bondLevel = Mathf.Clamp(bondLevel + intervals, 0f, 100f);
            }
        }
        else if (lastMoodScore < maxMoodForDecay)
        {
            int decayIntervals = Mathf.FloorToInt(totalSeconds / bondDecayInterval);
            if (decayIntervals > 0)
            {
                bondLevel = Mathf.Clamp(bondLevel - decayIntervals, 0f, 100f);
            }
        }

        UpdateBondStage();
        SaveBondLevel();
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (bondLevelSlider == null)
            return;

        // Ensure slider min/max are set to 0-100
        if (!Mathf.Approximately(bondLevelSlider.minValue, 0f) || !Mathf.Approximately(bondLevelSlider.maxValue, 100f))
        {
            bondLevelSlider.minValue = 0f;
            bondLevelSlider.maxValue = 100f;
        }

        // Clamp and assign value
        bondLevelSlider.value = Mathf.Clamp(bondLevel, bondLevelSlider.minValue, bondLevelSlider.maxValue);
        UpdateBondStageText();
    }

    private void UpdateBondStageText()
    {
        string stageString = bondStage.ToString();

        if (bondStageTMP != null)
        {
            bondStageTMP.text = stageString;
        }

        // if (bondStageText != null)
        // {
        //     bondStageText.text = stageString;
        // }
    }

    private void UpdateBondStage()
    {
        BondStage previousStage = bondStage;
        if (bondLevel > 0f && bondLevel < 20f)
            bondStage = BondStage.Stranger;
        else if (bondLevel >= 20f && bondLevel < 40f)
            bondStage = BondStage.Acquaintance;
        else if (bondLevel >= 40f && bondLevel < 60f)
            bondStage = BondStage.Friend;
        else if (bondLevel >= 60f && bondLevel < 70f)
            bondStage = BondStage.Companion;
        else if (bondLevel >= 70f && bondLevel <= 100f)
            bondStage = BondStage.Family;
        else
            return;

        if (bondStage != lastBondStage)
        {
            gm.firebaseDataManager.FirebaseAddBondChangeInJournal(bondStage);
            lastBondStage = bondStage;
        }
    }

    public BondStage GetBondStage()
    {
        return bondStage;
    }
}