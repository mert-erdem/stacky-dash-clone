using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [Header("Specs")]
    [SerializeField] private float speed = 10f;
    //input params
    private Vector3 mouseRootPos;
    private Vector3 input = Vector3.zero;
    private bool isMoving = false;
    private const float dragDeltaToMove = 50f;


    void Update()
    {
        if (!isMoving)
            HandleWithInput();
            
        if (isMoving && rigidbody.velocity == Vector3.zero)
            ResetInputParams();
    }

    private void Move() => rigidbody.velocity = speed * input;

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

                input = dragVec;
                input.z = input.y;
                input.y = 0;
               
                mouseRootPos = Input.mousePosition;
                isMoving = true;// player's started to moving

                Move();
            }                  
        }
    }

    //reached a corner point
    private void ResetInputParams()
    {
        input = Vector3.zero;
        isMoving = false;
        mouseRootPos = Input.mousePosition;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("StackTile"))
        {
            StackManager.Instance.CollectTile(other.gameObject);
        }

        if (other.CompareTag("BridgeTile"))
        {
            StackManager.Instance.RemoveTile(other.transform.position);
            Destroy(other.gameObject);
        }

        if(other.CompareTag("BridgeRouter"))
        {
            ChangeRoute(other.name);
        }

        
    }

    private void ChangeRoute(string routeName)
    {
        switch(routeName)
        {
            case "Right":
                ApplyNewRoute(Vector3.right);
                break;

            case "Forward":
                ApplyNewRoute(Vector3.forward);
                break;
        }
    }

    private void ApplyNewRoute(Vector3 newRoute)
    {
        rigidbody.velocity = Vector3.zero;
        input = newRoute;
        Move();
    }
}
