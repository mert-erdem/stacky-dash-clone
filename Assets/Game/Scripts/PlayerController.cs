using System;
using System.Collections;
using System.Collections.Generic;
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
    private float dragDeltaToMove = 100f;


    void Update()
    {
        if (!isMoving)
            HandleWithInput();

        if (isMoving && rigidbody.velocity == Vector3.zero)
            ResetInputParams();
    }

    private void Move() => rigidbody.velocity = input * speed;

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

            if(dragVec.magnitude > dragDeltaToMove)
            {
                dragVec.Normalize();

                if (Mathf.Abs(dragVec.x) > Mathf.Abs(dragVec.y))
                    dragVec.y = 0;
                else
                    dragVec.x = 0;

                input = dragVec;
                input.z = input.y;
                input.y = 0;

                Move();

                mouseRootPos = Input.mousePosition;
                isMoving = true;// player's started to moving
            }                  
        }
    }

    //reached a corner point
    private void ResetInputParams()
    {
        isMoving = false;
        mouseRootPos = Input.mousePosition;
    }
    #endregion
}
