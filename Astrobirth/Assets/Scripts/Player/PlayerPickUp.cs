using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerPickUp : MonoBehaviour
{
    // 플레이어가 든 3개의 아이템을 들고있게될 플레이어 아이템 시스템. 

    // 일단 플레이어가 아이템을 줍게 하자.

    // 효율적이냐? 나도 모른다. 주말동안 일이 많았다. (작업 못함)

    private PlayerController playerController;      // 플레이어 컨트롤러

    [Header("줍기 설정")]

    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;

    private PickupItem targetItem;

    private bool isPickingUp = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickingUp) return;

        // Player 주변에서 가장 가까운 아이템을 찾는다
        targetItem = FindClosestItem();

        if (targetItem == null) return;

        Keyboard keyboard = Keyboard.current;

        // 가까운 아이템이 있을 떄 E키로 줍는다 
        // 마우스로 그 아이템을 겨눴을 때 하고싶은데.. 당장은 이렇게 해놓자. 나중에 바꾸는 걸로

        if (keyboard.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PickupAnimation());
        }
    }

    private PickupItem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        PickupItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemCollider in colliders)
        {
            PickupItem item = itemCollider.GetComponentInParent<PickupItem>();

            if (item == null) continue;
            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        return closestItem;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        // 현재 찾은 아이템을 저장한다

        PickupItem itemToCollect = targetItem;


        /*
        // 아이템 방향으로 캐릭터를 돌려준다 - 아무래도 당장은 필요 없을 것 같다.
        Vector3 itemDirection = itemToCollect.transform.position - transform.position;
        itemDirection.y = 0f;

        
        if (itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }
        */



        if (itemToCollect.needDelay && itemToCollect != null)
        {
            // 줍기 상태로 변경하여 이동을 막는다 - 만약 주울 때 멈춰야 한다면
            //playerController.ChangeState(PlayerState.Interact);

            // 전환선 없ㅎ이 PickUp01 상태로 이동한다 \
            //animator.CrossFade("PickUp01", 0.1f);


            // 손이 아이템에 닿는 시점까지 기다린다

            yield return new WaitForSeconds(pickupMoment);

            // 실제로 아이템을 획득한다 

            if (itemToCollect != null)
            {
                //itemToCollect.Collect();
            }

            // 남은 애니메이션 시간을 기다린다 \
            yield return new WaitForSeconds(pickupDuration - pickupMoment);


            // 이동 BlendTree로 돌아간다 (지금은 사용 X)
            //animator.CrossFade("Blend Tree", 0.1f);

            // 노말 상태로 돌아가 이동을 허용한다
            //playerController.ChangeState(PlayerState.Normal);
        }
        else if (itemToCollect != null)
        {
            //itemToCollect.Collect();
        }



        targetItem = null;

        isPickingUp = false;

    }
}
