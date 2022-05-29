using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // other variables
    public CharacterController characterController;

    // horizontal movement variables
    public float moveSpeed;
    public float rotationSpeed;
    [Range(0f, 1f)]
    public float rotationSmoothnessValue;

    private Vector3 moveVector;
    private Vector3 rotation = new Vector3(0f, 0f, 0f);

    // vertical movement variables
    public float gravityScale;
    [Range(0f, 1f)]
    public float floatingGravityMultiplier;
    public float initialJumpForce;
    public float jumpForceReductionScale;
    public float floatTimeLimit;

    private float floatTime = 0f;
    private float currentJumpForce;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isFloating = false;

    void Start()
    {
        currentJumpForce = initialJumpForce;
    }

    void Update()
    {
        MovementController();
        JumpController();

        characterController.Move(moveVector);

        // placing the reset here allows the player to jump at the same frame as it falls down
        ResetJumpVariables();
    }

    // function that controls horizontal movement
    void MovementController()
    {
        rotation.y += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        Quaternion target = Quaternion.Euler(rotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSmoothnessValue);

        moveVector = Input.GetAxis("Vertical") * transform.forward * moveSpeed * Time.deltaTime;
    }

    // function that controls jumping mechanic
    void JumpController()
    {
        // if the character is grounded... 
        if (characterController.isGrounded)
        {
            // ... allow the player to jump while its not jumping
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                isJumping = true;

                moveVector.y += currentJumpForce * Time.deltaTime;
                currentJumpForce -= initialJumpForce * jumpForceReductionScale;
                return;
            }
        }

        // and if the character is not on the ground (in the air)...
        else
        {
            // ... check if the character is actually jumping (not falling or floating)...
            if (isJumping && !isFalling && !isFloating)
            {
                // ... if the player releases the jump button then the character immediately falls ...
                if (Input.GetButtonUp("Jump"))
                {
                    isFalling = true;
                    moveVector.y += Physics.gravity.y * Time.deltaTime * gravityScale;
                    return;
                }

                // ... but if the character holds the jump button until the end, the character floats a bit.
                if (currentJumpForce <= 0f)
                {
                    isFloating = true;
                    moveVector.y += Physics.gravity.y * Time.deltaTime * gravityScale * floatingGravityMultiplier;
                    return;
                }


                // otherwise keep on adding jump force if the user did not do anything from the above.
                moveVector.y += currentJumpForce * Time.deltaTime;
                currentJumpForce -= initialJumpForce * jumpForceReductionScale;
                return;
            }

            // now if the character is floating apply some scaled gravity
            if (isFloating)
            {
                moveVector.y += Physics.gravity.y * Time.deltaTime * gravityScale * floatingGravityMultiplier;

                // if the float time runs out, the character starts to fall
                if (floatTime > floatTimeLimit)
                {
                    isFloating = false;
                    isFalling = true;
                    return;
                }

                floatTime += Time.deltaTime;
                return;
            }

            // this handles the falling physics
            moveVector.y += Physics.gravity.y * Time.deltaTime * gravityScale;
        }
    }

    void ResetJumpVariables()
    {
        // reset all the variables when reaching the ground
        if (characterController.isGrounded)
        {
            isJumping = false;
            isFalling = false;
            isFloating = false;
            currentJumpForce = initialJumpForce;
            floatTime = 0f;

            // and allow the player to jump again right after reaching the ground
            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;

                moveVector.y += currentJumpForce * Time.deltaTime;
                currentJumpForce -= initialJumpForce * jumpForceReductionScale;
                return;
            }
        }
    }
}
