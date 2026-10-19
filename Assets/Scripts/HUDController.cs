using UnityEngine;
using TMPro; // TMP_Text
using UnityEngine.UI;
 
// Updates the HUD every frame from the ScoreManager and PlayerPowerUps.
public class HUDController : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Health")]
    [SerializeField] private CharacterMovement player;
    [SerializeField] private Graphic[] healthBarImages;
 
    private void Update()
    {
        UpdateScore();
        UpdateHealth();
    }
 
    private void UpdateScore()
    {
        // The Score property from Lab 4: anyone can read it
        scoreText.text = "Score: " + ScoreManager.Instance.Score;
    }

    private void UpdateHealth()
    {
        for (int i = 0; i < healthBarImages.Length; i++)
        {
            healthBarImages[i].gameObject.SetActive(i < player.CurrentHealth);
        }
    }
 
}
