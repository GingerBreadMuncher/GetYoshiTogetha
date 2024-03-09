using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    GameManager gameManager;
    [SerializeField] TextMeshProUGUI difficultyText;

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
            case 0: difficultyText.text = "Brainrot"; break;
            case 1: difficultyText.text = "Easy"; break;
            case 2: difficultyText.text = "Normal"; break;
            case 3: difficultyText.text = "Hardcore"; break;
        }
    }
}
