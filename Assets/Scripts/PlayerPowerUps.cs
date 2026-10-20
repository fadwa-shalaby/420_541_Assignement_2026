using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 1.5f; // 50% faster
 
    public float SpeedTimeLeft { get; private set; }
    public float JumpTimeLeft { get; private set; }
    public float ScoreTimeLeft { get; private set; }

    public bool HasJumpBoost => JumpTimeLeft > 0f;
    public bool HasScoreBoost => ScoreTimeLeft > 0f;
 
    private CharacterMovement movement;
 
    private void Start()
    {
        movement = GetComponent<CharacterMovement>();
    }
 
    // Called by the pickup. Collecting another boost restarts the timer.
    public void ActivateSpeedBoost(float duration)
    {
        SpeedTimeLeft = duration;
        movement.SpeedMultiplier = boostMultiplier;
    }

    public void ActivateJumpBoost(float duration)
    {
        JumpTimeLeft = duration;
    }

    public void ActivateScoreBoost(float duration)
    {
        ScoreTimeLeft = duration;
    }

    public int GetScoreMultiplier(int points)
    {
        if(HasScoreBoost)
        {
            return points * 2; // double points
        }
        return points;
    }

    private void Update()
    {
        if (SpeedTimeLeft <= 0f) return; // no boost running

        SpeedTimeLeft -= Time.deltaTime;

        if (SpeedTimeLeft <= 0f)
        {
            movement.SpeedMultiplier = 1f; // back to normal speed
        }
        
        if (JumpTimeLeft <= 0f) return; // no boost running

        JumpTimeLeft -= Time.deltaTime;

        if (JumpTimeLeft <= 0f)
        {
            JumpTimeLeft = 0f; // back to normal jump
        }

        if (ScoreTimeLeft <= 0f) return; // no boost running

        ScoreTimeLeft -= Time.deltaTime;

        if (ScoreTimeLeft <= 0f)
        {
            ScoreTimeLeft = 0f; // back to normal score
        }
    }
}
