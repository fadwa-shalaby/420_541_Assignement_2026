using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private float duration = 10f;    // boost length
    [SerializeField] private float respawnDelay = 5f; // time hidden
    [SerializeField] private GameObject visuals;      // child to hide
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect; // particle prefab

    [Header("Pickup Settings")]
    [SerializeField] private string powerUpType = "Score"; // Score, speed or Jump

    private bool collected = false; // to prevent double collection
 
    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // Only the player has a PlayerPowerUps component
        PlayerPowerUps powerUps = other.GetComponentInParent<PlayerPowerUps>();
        if (powerUps == null) return;

        if (powerUpType == "Score")
            powerUps.ActivateScoreBoost(duration);
        else if (powerUpType == "Jump")
        {
            powerUps.ActivateJumpBoost(duration);
        }
        else if (powerUpType == "Speed")
        {
            powerUps.ActivateSpeedBoost(duration);
        }
 
        // Sound and particles
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
 
        // Disappear, and call Reappear() after respawnDelay seconds
        collected = true;
        if (visuals != null)
            visuals.SetActive(false);
        GetComponent<Collider>().enabled = false;
        Invoke(nameof(Reappear), respawnDelay);
    }
 
    private void Reappear()
    {
        if (visuals != null)
            visuals.SetActive(true);
        GetComponent<Collider>().enabled = true;
        collected = false;
    }
}
