using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponSystem : MonoBehaviour
{
    [Header("플레이어 아이템")]

    private PlayerController playerController;



    public bool isHoldingThirdItem = false;

    private int currentWeaponNum = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            ChangeItemHolding(1);
        }
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            ChangeItemHolding(2);
        }
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            if(isHoldingThirdItem)
                ChangeItemHolding(3);
        }


    }

    private void ChangeItemHolding(int itemNum)
    {
        currentWeaponNum = itemNum;
    }
}
