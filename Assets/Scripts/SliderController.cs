using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    GameManager gameManager;
    [SerializeField] TextMeshProUGUI difficultyText;
    [SerializeField] TextMeshProUGUI difficultySeconds;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateSliderText();
    }

    public void OnSliderValueChanged() 
    {
        if (gameManager != null)
        {
            gameManager.difficulty = (int)slider.value + 2;
            UpdateSliderText();
            gameManager.SaveDifficultyLevel();
        }
    }

    void UpdateSliderText()
    {
        switch (slider.value)
        {
            case 0: difficultyText.text = "Brainrot"; difficultySeconds.text = "(10 seconds)"; break;
            case 1: difficultyText.text = "Easy"; difficultySeconds.text = "(30 seconds)"; break;
            case 2: difficultyText.text = "Normal"; difficultySeconds.text = "(45 seconds)"; break;
            case 3: difficultyText.text = "Hardcore"; difficultySeconds.text = "(60 seconds)"; break;
        }
    }
}
