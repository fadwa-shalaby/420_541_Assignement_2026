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
        // Only react to the player, ignore everything else
        if (other.CompareTag("Player"))
        {
            // Add our points through the ScoreManager singleton
            ScoreManager.Instance.AddScore(points);
            // Sound and particles
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Remove the coin from the scene: it's collected once and gone
            Destroy(gameObject);
        }
    }
}
