using System.Collections;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [Header("Components")]
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    [Header("Specs")]
    [SerializeField] private float speed = 10f;
    //input params
    private Vector3 mouseRootPos;
    private Vector3 input = Vector3.zero;
    private bool isMoving = false;
    private const float dragDeltaToMove = 50f;
    private bool isStuck = false;


    void Update()
    {
        if (!isMoving)
            HandleWithInput();
            
        if (isMoving && rigidbody.velocity == Vector3.zero)
            ResetInputParams();
    }

    private void Move()
    {
        rigidbody.velocity = speed * input;
    }

    #region Input Handling
    private void HandleWithInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mouseRootPos = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            var dragVec = Input.mousePosition - mouseRootPos;

            if(dragVec.magnitude >= dragDeltaToMove)
            {
                dragVec.Normalize();

                if (Mathf.Abs(dragVec.x) >= Mathf.Abs(dragVec.y))
                    dragVec.y = 0;
                else
                    dragVec.x = 0;

                Vector3 newInput = dragVec;
                newInput.z = newInput.y;
                newInput.y = 0;

                if (isStuck && newInput != Vector3.back)
                    return;// player can not move without required amount of tile
                else if (newInput == Vector3.back)
                    isStuck = false;

                input = newInput;
               
                mouseRootPos = Input.mousePosition;
                isMoving = true;

                Move();
            }                  
        }
    }

    //reached a corner point
    private void ResetInputParams()
    {
        animator.SetBool("IsJumping", false);
        rigidbody.velocity = Vector3.zero;
        isMoving = false;
        mouseRootPos = Input.mousePosition;
    }
    #endregion

    #region Bridge Routing
    private void HandleWithBridge(GameObject trigger)
    {
        animator.SetBool("IsJumping", false);

        bool bridgePassed = StackManager.Instance.RemoveTile(trigger.transform.position);

        if (!bridgePassed)// player couldn't passed a bridge
        {
            if(GameManager.Instance.EnteredMiniGame)// player stopped at mini game phase
            {
                HandleWithFinish();
            }
            isStuck = true;
            ResetInputParams();
        }
        else
        {
            Destroy(trigger);
            isStuck = false;// player can move on the bridge
        }
    }

    private void ChangeRoute(string routeName)
    {
        switch (routeName)
        {
            case "Left":
                ApplyNewRoute(Vector3.left);
                break;

            case "Right":
                ApplyNewRoute(Vector3.right);
                break;

            case "Forward":
                ApplyNewRoute(Vector3.forward);
                break;

            case "Back":
                ApplyNewRoute(Vector3.back);
                break;
        }
    }
    private void ApplyNewRoute(Vector3 newRoute)
    {
        rigidbody.velocity = Vector3.zero;
        input = newRoute;
        Move();
    }   
    #endregion

    // level end
    private void Stop()
    {
        rigidbody.velocity = Vector3.zero;
        enabled = false;
    }

    private void HandleWithMiniGame()
    {
        GameManager.ActionMiniGame?.Invoke();
        animator.SetTrigger("MiniGame");
        visual.rotation = Quaternion.Euler(0, 90f, 0);
    }

    private void HandleWithFinish()
    {
        GameManager.ActionLevelPassed?.Invoke();
        animator.SetTrigger("Finish");
        visual.rotation = Quaternion.Euler(0, 180f, 0);
        Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // IN GAME PHASE
        if(other.CompareTag("StackTile"))
        {          
            StackManager.Instance.CollectTile(other.gameObject);
            animator.SetBool("IsJumping", true);
        }
        if (other.CompareTag("BridgeTile"))
        {
            HandleWithBridge(other.gameObject);            
        }
        if(other.CompareTag("BridgeRouter"))
        {
            ChangeRoute(other.name);
        }
        // MINI GAME PHASE
        if(other.CompareTag("MiniGame"))
        {
            HandleWithMiniGame();
        }
        if(other.CompareTag("Multiplier"))
        {
            //print(other.transform.parent.name);
            // apply multiplier's value
        }
        if(other.CompareTag("Finish"))
        {
            HandleWithFinish();
        }
    }
}
