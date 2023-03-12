using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    // Outlets
    private Rigidbody _rb;

    // State Tracking
    public bool canJump;

    public GroundGenerator ground;

    private Vector3 currentGravityDirection = Vector3.down;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Always move forward!
        _rb.AddForce(Vector3.forward * 5f * Time.deltaTime, ForceMode.Impulse);

        //Move left or right based on player inputs
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (currentGravityDirection == Vector3.down)
            {
                _rb.AddForce(Vector3.left * 35f * Time.deltaTime, ForceMode.Impulse);
            } 
            else if (currentGravityDirection == Vector3.left)
            {
                _rb.AddForce(Vector3.up * 35f * Time.deltaTime, ForceMode.Impulse);
            }
            else if (currentGravityDirection == Vector3.right)
            {
                _rb.AddForce(Vector3.down * 35f * Time.deltaTime, ForceMode.Impulse);
            }
            else if (currentGravityDirection == Vector3.up)
            {
                _rb.AddForce(Vector3.right * 35f * Time.deltaTime, ForceMode.Impulse);
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (currentGravityDirection == Vector3.down)
            {
                _rb.AddForce(Vector3.right * 35f * Time.deltaTime, ForceMode.Impulse);
            } 
            else if (currentGravityDirection == Vector3.left)
            {
                _rb.AddForce(Vector3.down * 35f * Time.deltaTime, ForceMode.Impulse);
            }
            else if (currentGravityDirection == Vector3.right)
            {
                _rb.AddForce(Vector3.up * 35f * Time.deltaTime, ForceMode.Impulse);
            }
            else if (currentGravityDirection == Vector3.up)
            {
                _rb.AddForce(Vector3.left * 35f * Time.deltaTime, ForceMode.Impulse);
            }

        }

        //Jump!
        if (Input.GetKey(KeyCode.Space))
        {
            if (canJump)
            {
                canJump = false;
                _rb.AddForce(-currentGravityDirection * 6.0f, ForceMode.Impulse);
            }
        }
        
        //Debug.DrawRay(transform.position, Vector3.down * 1.2f, Color.red);
        float x = _rb.position.x;
        float y = _rb.position.y;
        
        if (_rb.position.y <= -25 || _rb.position.y >= 25)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ground.gameOver = true;
        }
        if (_rb.position.x <= -25 || _rb.position.x >= 25)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ground.gameOver = true;
        }
    }


    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, currentGravityDirection, 1.2f);
            Debug.DrawRay(transform.position, currentGravityDirection * 1.2f, Color.red);

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        canJump = true;
                    }
                }
            }

            Vector3 currentLeft = Vector3.left;
            if (currentGravityDirection == Vector3.left)
            {
                currentLeft = Vector3.up;
            }
            else if (currentGravityDirection == Vector3.right)
            {
                currentLeft = Vector3.down;
            }
            else if (currentGravityDirection == Vector3.up)
            {
                currentLeft = Vector3.right;
            }

            Vector3 currentRight = Vector3.right;
            if (currentGravityDirection == Vector3.left)
            {
                currentRight = Vector3.down;
            }
            else if (currentGravityDirection == Vector3.right)
            {
                currentRight = Vector3.up;
            }
            else if (currentGravityDirection == Vector3.up)
            {
                currentRight = Vector3.left;
            }
            
            RaycastHit[] leftHits = Physics.RaycastAll(transform.position, currentLeft, 0.6f);
            RaycastHit[] rightHits = Physics.RaycastAll(transform.position, currentRight, 0.6f);
            if (leftHits.Length > 0)
            {
                for (int i = 0; i < leftHits.Length; i++)
                {
                    RaycastHit hit = leftHits[i];

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        currentGravityDirection = Quaternion.AngleAxis(-90, Vector3.forward) * currentGravityDirection;
                        Physics.gravity = currentGravityDirection * 24f;
                        _rb.transform.Rotate(new Vector3(0, 0, 1), -90);
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
                        currentGravityDirection = Quaternion.AngleAxis(90, Vector3.forward) * currentGravityDirection;
                        Physics.gravity = currentGravityDirection * 24f;
                        _rb.transform.Rotate(new Vector3(0, 0, 1), 90);
                    }
                }
            }
        }
    }
}
