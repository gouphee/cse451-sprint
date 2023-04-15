using UnityEngine;

public class ClampVelocity : MonoBehaviour
{
    public float maxVerticalVelocity = 0.1f; // Adjust this value to limit the vertical velocity
    private Rigidbody _rb;
    private PlayerController _playerController;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!_playerController.canJump && !_playerController.canSuperJump)
        {
            Vector3 currentVelocity = _rb.velocity;
            currentVelocity.y = Mathf.Clamp(currentVelocity.y, -maxVerticalVelocity, maxVerticalVelocity);
            _rb.velocity = currentVelocity;
        }
    }
}
