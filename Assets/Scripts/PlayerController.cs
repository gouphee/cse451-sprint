using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    // Outlets
    private Rigidbody _rb;

    // State Tracking
    public bool canJump;
    public bool canSuperJump;
    public bool canInvertGravity;
    public GroundGenerator ground;
    private Vector3 currentGravityDirection;
    private bool canRotateWorld;
    
    // Configuration
    private const float LateralMovementForce = 40f;
    private const float JumpForce = 6.0f;
    private const float SuperJumpForce = 30.0f;
    private const float GravityForce = 24f;
    private const int SuperJumpCooldown = 5;
    private const int InvertGravityCooldown = 20;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currentGravityDirection = Vector3.down;
        Physics.gravity = currentGravityDirection * GravityForce;

        StartCoroutine("ResetSuperJumpPowerup");
        StartCoroutine("ResetInvertGravityPowerup");
        canRotateWorld = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Always move forward!
        _rb.AddForce(Vector3.forward * (5f * Time.deltaTime), ForceMode.Impulse);

        //Move left or right based on player inputs
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _rb.AddForce(Quaternion.AngleAxis(-90, Vector3.forward) * currentGravityDirection * (LateralMovementForce * Time.deltaTime), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _rb.AddForce(Quaternion.AngleAxis(90, Vector3.forward) * currentGravityDirection * (LateralMovementForce * Time.deltaTime), ForceMode.Impulse);
        }

        // Jump!
        if (Input.GetKey(KeyCode.Space))
        {
            if (canJump)
            {
                canJump = false;
                _rb.AddForce(-currentGravityDirection * JumpForce, ForceMode.Impulse);
            }
        }
        
        // Super jump!
        if (Input.GetKey(KeyCode.W))
        {
            if (canSuperJump)
            {
                canSuperJump = false;
                _rb.AddForce(-currentGravityDirection * SuperJumpForce, ForceMode.Impulse);
                StartCoroutine("ResetSuperJumpPowerup");
            }
        }

        // Invert gravity!
        if (Input.GetKey(KeyCode.S))
        {
            if (canInvertGravity && canRotateWorld)
            {
                canInvertGravity = false;
                RotateWorld(180);
                StartCoroutine("ResetInvertGravityPowerup");
            }
        }

        // Ensure the player hasn't fallen off the map (end the game if so)
        Vector3 currentPosition = _rb.position;
        float x = currentPosition.x;
        float y = currentPosition.y;
        
        if (y <= -25 || y >= 25)
        {
            ground.gameOver = true;
        }
        if (x <= -25 || x >= 25)
        {
            ground.gameOver = true;
        }
    }

    IEnumerator ResetInvertGravityPowerup()
    {
        yield return new WaitForSeconds(InvertGravityCooldown);
        canInvertGravity = true;
    }

    IEnumerator ResetSuperJumpPowerup()
    {
        yield return new WaitForSeconds(SuperJumpCooldown);
        canSuperJump = true;
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, currentGravityDirection, 1.2f);
            //Debug.DrawRay(transform.position, currentGravityDirection * 1.2f, Color.red);

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        canJump = true;
                        break;
                    }
                }
            }

            Vector3 currentLeft = Quaternion.AngleAxis(-90, Vector3.forward) * currentGravityDirection;
            Vector3 currentRight = Quaternion.AngleAxis(90, Vector3.forward) * currentGravityDirection;
            Vector3 currentPosition = transform.position;
            
            RaycastHit[] leftHits = Physics.RaycastAll(currentPosition, currentLeft, 0.6f);
            RaycastHit[] rightHits = Physics.RaycastAll(currentPosition, currentRight, 0.6f);
            if (leftHits.Length > 0)
            {
                for (int i = 0; i < leftHits.Length; i++)
                {
                    RaycastHit hit = leftHits[i];

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        RotateWorld(-90);
                    }
                }
            }

            if (rightHits.Length > 0)
            {
                for (int i = 0; i < rightHits.Length; i++)
                {
                    RaycastHit hit = rightHits[i];

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        RotateWorld(90);
                    }
                }
            }
        }
    }

    // If the world is not currently rotating, rotate the world (player, camera, and gravity) by [angle] degrees
    // Uses DOTween plugin to create a smooth animation between gravity states
    void RotateWorld(int angle)
    {
        if (canRotateWorld)
        {
            canRotateWorld = false;
            currentGravityDirection = Quaternion.AngleAxis(angle, Vector3.forward) * currentGravityDirection;
            Physics.gravity = currentGravityDirection * GravityForce;
            Vector3 currentRotationVector = _rb.rotation.eulerAngles;
            Vector3 newRotationVector = new Vector3(0, 0, currentRotationVector.z + angle);
            _rb.transform.DORotate(newRotationVector, 0.65f).SetEase(Ease.Linear).OnComplete(() => { canRotateWorld = true; });
        }
    }
}
