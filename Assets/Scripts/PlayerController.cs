using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    // Outlets
    private Rigidbody _rb;
    public Image superJumpImage;
    public Image invertGravityImage;
    public TMP_Text scoreText;

    // State Tracking
    public bool canJump;
    public bool canSuperJump;
    public bool canInvertGravity;
    public PlatformGenerator ground;
    private Vector3 currentGravityDirection;
    private bool canRotateWorld;
    private float startTime;
    public float score;
    private float currentVelocity;

    public bool isPaused;
    public bool gameOver;
    
    // Configuration
    private const float InitialForwardForce = 10f;
    private const float ForwardForceIncreaseRate = 0.01f;
    private const float LateralMovementForce = 30f;
    private const float JumpForce = 12.0f;
    private const float SuperJumpForce = 20.0f;
    private const float GravityForce = 24f;
    private const int SuperJumpCooldown = 5;
    private const int InvertGravityCooldown = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currentGravityDirection = Vector3.down;
        Physics.gravity = currentGravityDirection * GravityForce;
        startTime = Time.time;

        StartCoroutine("ResetSuperJumpPowerup");
        StartCoroutine("ResetInvertGravityPowerup");
        canRotateWorld = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            return;
        }

        if (gameOver)
        {
            MenuController.instance.ShowGameOver();
            return;
        }
        
        // Always move forward!
        Vector3 newVelocity = _rb.velocity;
        newVelocity.z = InitialForwardForce + ForwardForceIncreaseRate * (Time.time - startTime);
        _rb.velocity = (newVelocity.z > 20f) ? Vector3.forward * 20f : newVelocity;
        currentVelocity = _rb.velocity.z;
        
        // Update UI based on status of powerups
        superJumpImage.color = canSuperJump ? Color.green : Color.grey;
        invertGravityImage.color = canInvertGravity ? Color.green : Color.grey;

        //Move left or right based on player inputs
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _rb.AddForce(Quaternion.AngleAxis(-90, Vector3.forward) * currentGravityDirection * (LateralMovementForce * Time.deltaTime), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _rb.AddForce(Quaternion.AngleAxis(90, Vector3.forward) * currentGravityDirection * (LateralMovementForce * Time.deltaTime), ForceMode.Impulse);
        }
        
        // Pause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuController.instance.ShowPauseMenu();
        }

        // Jump!
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                canJump = false;
                _rb.AddForce(-currentGravityDirection * JumpForce, ForceMode.Impulse);
            }
        }
        
        // Super jump!
        if (Input.GetKeyDown(KeyCode.W))
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

        if (y <= -0.5 || y >= 16.5)
        {
            gameOver = true;
        }
        if (x <= -8.5 || x >= 8.5)
        {
            gameOver = true;
        }
        
        // Update score and GUI
        score += (currentVelocity * 3) * Time.deltaTime;
        scoreText.text = ((int)score).ToString();
    }

    IEnumerator ResetInvertGravityPowerup()
    {
        invertGravityImage.fillAmount = 0;
        invertGravityImage.DOFillAmount(100, InvertGravityCooldown);
        yield return new WaitForSeconds(InvertGravityCooldown);
        canInvertGravity = true;
    }

    IEnumerator ResetSuperJumpPowerup()
    {
        superJumpImage.fillAmount = 0;
        superJumpImage.DOFillAmount(100, SuperJumpCooldown);
        yield return new WaitForSeconds(SuperJumpCooldown);
        canSuperJump = true;
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            gameOver = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
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
