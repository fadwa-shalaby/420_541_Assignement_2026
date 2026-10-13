using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CharacterMovement playerMovement = other.GetComponent<CharacterMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(1);
            }
        }
    }
}