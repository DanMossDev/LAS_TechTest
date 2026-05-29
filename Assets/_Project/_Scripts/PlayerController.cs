using Unity.VisualScripting;
using UnityEngine;

namespace LAS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TerrainManager _terrainManager;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _gravity = 0.5f;

        [SerializeField] private float _jumpSpeed = 2;

        [SerializeField] private Transform _playerVisuals;
        [SerializeField] private float _rotationAirSpeed = 100.0f;
        [SerializeField] private float _rotationGroundSpeed = 10.0f;
        [SerializeField] private float _groundedDistance = 0.1f;

        private float _rotationInput;
        private float _timeInAir;
        private bool _isGrounded;

        private Vector2 _movement;

        private void OnEnable()
        {
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            InputManager.MoveInput += OnMove;
            InputManager.JumpInput += OnJump;
        }

        private void UnsubscribeFromInput()
        {
            InputManager.MoveInput -= OnMove;
            InputManager.JumpInput -= OnJump;
        }

        private void OnMove(Vector2 direction)
        {
            _rotationInput = direction.x;
        }

        private void OnJump(bool held)
        {
            if (held && _isGrounded)
                _movement.y = _jumpSpeed;
            else if (_movement.y > 0)
                _movement.y /= 5.0f;
        }
        
        private void Update()
        {
            _movement.x = Time.deltaTime * _speed;
            
            float x = transform.position.x + _movement.x;
            float groundY = _terrainManager.GetTerrainHeightAtX(x);

            if (groundY >= transform.position.y && _movement.y <= 0)
            {
                transform.position = new Vector3(x, groundY, 0);
                _movement.y = 0;
            }
            else
            {
                _movement.y -= _gravity * Time.deltaTime;
                transform.position = new Vector3(x, Mathf.Max(transform.position.y + _movement.y, groundY), 0);
            }
            
            _isGrounded = (transform.position.y - groundY) < _groundedDistance;

            if (_isGrounded)
            {
                _timeInAir = 0;
                LerpTransformUp(_terrainManager.GetNormalAtPosition(x));
            }
            else
            {
                _timeInAir += Time.deltaTime;
                Rotate();
            }
        }

        private void Rotate()
        {
            _playerVisuals.localEulerAngles = new Vector3(0, 0, _playerVisuals.localEulerAngles.z - _rotationAirSpeed * _rotationInput * Time.deltaTime);
        }
        
        private void LerpTransformUp(Vector3 targetUp)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_playerVisuals.forward, targetUp);
            _playerVisuals.rotation = Quaternion.Slerp(_playerVisuals.rotation, targetRotation, Time.deltaTime * _rotationGroundSpeed);
        }
    }
}