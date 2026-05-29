using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LAS
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        
        public static event Action<Vector2> MoveInput;
        public static event Action<bool> JumpInput;
        
        public static event Action SubmitInput;

        private void OnEnable()
        {
            EnableInput();
            Subscribe();
        }

        private void OnDisable()
        {
            DisableInput();
            Unsubscribe();
        }

        public void EnableInput()
        {
            _inputActions.Enable();
        }

        public void DisableInput()
        {
            _inputActions.Disable();
        }

        private void Subscribe()
        {
            _inputActions["Move"].started += OnMove;
            _inputActions["Move"].performed += OnMove;
            _inputActions["Move"].canceled += OnMove;
            
            _inputActions["Jump"].started += OnJump;
            _inputActions["Jump"].canceled += OnStopJump;
            
            _inputActions["Submit"].performed += OnSubmit;
        }

        private void Unsubscribe()
        {
            _inputActions["Move"].started -= OnMove;
            _inputActions["Move"].performed -= OnMove;
            _inputActions["Move"].canceled -= OnMove;
            
            _inputActions["Jump"].started -= OnJump;
            _inputActions["Jump"].canceled -= OnStopJump;
            
            _inputActions["Submit"].performed -= OnSubmit;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput?.Invoke(context.ReadValue<Vector2>());
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            JumpInput?.Invoke(true);
        }

        private void OnStopJump(InputAction.CallbackContext context)
        {
            JumpInput?.Invoke(false);
        }
        
        //UI
        private void OnSubmit(InputAction.CallbackContext context)
        {
            SubmitInput?.Invoke();
        }
    }
}
