using UnityEngine;

public class ScorePickup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect; // particle prefab
    // How many points this coin is worth (change it in the Inspector)
    [SerializeField] private int points = 10;

    // Called automatically when something enters this trigger
    private void OnTriggerEnter(Collider other)
    {
        PlayerPowerUps powerUps = other.GetComponentInParent<PlayerPowerUps>();

        // Only react to the player, ignore everything else
        if (other.CompareTag("Player") || powerUps != null)
        {
            int score = powerUps != null ? powerUps.GetScoreMultiplier(points) : points;

            // Add our points through the ScoreManager singleton
            ScoreManager.Instance.AddScore(score);
            // Sound and particles
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Remove the coin from the scene: it's collected once and gone
            Destroy(gameObject);
        }
    }
}
