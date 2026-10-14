using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMelee : MonoBehaviour
{
    public bool canMeleeAttack = false;

    public GameObject meleeAttackRange;

    public float fadeTime = 0.3f;
    public float lastAttackTime = -999f;
    public float attackCooldown = 0.5f;


    public void OnAttack(InputValue inputValue)
    {
        if (!canMeleeAttack) return;
        if (Time.timeScale == 0f) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        meleeAttackRange.gameObject.SetActive(true);

    }

    private void Update()
    {

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if(keyboard.digit1Key.wasPressedThisFrame)
        {
            canMeleeAttack = false;
        }
        else if(keyboard.digit2Key.wasPressedThisFrame)
        {
            canMeleeAttack = true;
        }

        if(meleeAttackRange == null) return;

        if(Time.time > lastAttackTime + fadeTime)
        {
            meleeAttackRange.gameObject.SetActive(false);
        }
    }
}
