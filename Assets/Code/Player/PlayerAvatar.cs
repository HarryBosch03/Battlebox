using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BoschingMachine.Kinematics;
using BoschingMachine.Bipedal;

namespace BoschingMachine
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAvatar : Biped
    {
        [SerializeField] InputActionAsset inputAsset;
        [SerializeField] float mouseSensivity;

        [Space]
        [SerializeField] EntityMovement movement;

        InputActionMap playerMap;
        InputAction moveAction;
        InputAction jumpAction;

        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            playerMap = inputAsset.FindActionMap("Player");
            playerMap.Enable();

            moveAction = playerMap.FindAction("Movement");
            jumpAction = playerMap.FindAction("Jump");
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void FixedUpdate()
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            Vector3 moveDirection = transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);
            movement.Move(rb, moveDirection, GetFlag(jumpAction));
        }

        protected override void Update()
        {
            if (Mouse.current != null)
            {
                LookRotation += Mouse.current.delta.ReadValue() * mouseSensivity;
            }

            base.Update();
        }

        protected bool GetFlag(InputAction action) => action.ReadValue<float>() > 0.5f;
    }
}
