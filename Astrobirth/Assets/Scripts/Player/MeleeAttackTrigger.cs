using UnityEngine;

public class MeleeAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("들어왔어");

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().TakeDamage(1);
        }
    }

}
