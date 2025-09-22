using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TestDebugger : MonoBehaviour
{
    [SerializeField] GeneralManager gm;

    [SerializeField] TextMeshProUGUI criticalNeed;
    [SerializeField] TextMeshProUGUI dominantEmotion;
    [SerializeField] TextMeshProUGUI personalityTrait;
    [SerializeField] TextMeshProUGUI bondStage;
    [SerializeField] Slider daysSlider;

    [SerializeField] TextMeshProUGUI daysLabelText;

    [Header("Simulation Settings")]
    [SerializeField] float totalSimulationMinutes = 7f; 
    private float elapsedTime = 0f;
    private int currentDay = 1;
    private bool isSimulating = true;

    void UpdateValues()
    {
        criticalNeed.text = gm.needsSystem.GetCriticalNeed();
        dominantEmotion.text = gm.emotionSystem.GetDominantEmotion().ToString();
        personalityTrait.text = gm.personalitySystem.GetPetPersonality();
        bondStage.text = gm.bondingSystem.GetBondStage().ToString();
    }
    
    void Update()
    {
        UpdateValues();
        
        if (!isSimulating) return;
        elapsedTime += Time.deltaTime;
        float minutesPassed = elapsedTime / 60f;
        float totalSeconds = totalSimulationMinutes * 60f;
        float progress = Mathf.Clamp01(elapsedTime / totalSeconds);
        float dayFloat = 1f + progress * 6f; // 1 to 7
        int newDay = Mathf.Clamp(Mathf.FloorToInt(dayFloat), 1, 7);

        if (daysSlider != null)
            daysSlider.value = dayFloat;


        if (newDay != currentDay)
        {
            currentDay = newDay;
            UpdateDayLabel(currentDay);
        }

        if (minutesPassed >= totalSimulationMinutes)
        {
            isSimulating = false;
        }
    }

    private void UpdateDayLabel(int day)
    {
        if (daysLabelText != null)
            daysLabelText.text = $"{day}";
    }
}
