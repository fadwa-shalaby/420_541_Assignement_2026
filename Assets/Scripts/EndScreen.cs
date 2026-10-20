using UnityEngine; 
using TMPro; 
  
// Shows the score the player finished the level with. 
public class EndScreen : MonoBehaviour 
{ 
    [SerializeField] private TMP_Text finalScoreText; 
  
    private void Start() 
    { 
        finalScoreText.text = $"Level Complete!\nFinal Score: {ScoreManager.FinalScore}";
    } 
} 
