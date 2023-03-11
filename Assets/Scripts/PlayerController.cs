using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Outlets
    private Rigidbody _rb;

    // State Tracking
    public bool canJump;

    public GroundGenerator ground;

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
            _rb.AddForce(Vector3.left * 35f * Time.deltaTime, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _rb.AddForce(Vector3.right * 35f * Time.deltaTime, ForceMode.Impulse);
        }

        //Jump!
        if (Input.GetKey(KeyCode.Space))
        {
            if (canJump)
            {
                canJump = false;
                _rb.AddForce(Vector3.up * 6.0f, ForceMode.Impulse);
            }
        }
        
        //Debug.DrawRay(transform.position, Vector3.down * 1.2f, Color.red);
        if (_rb.position.y <= -25)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ground.gameOver = true;
        }
    }


    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 1.2f);
            Debug.DrawRay(transform.position, Vector3.down * 1.2f);

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

            RaycastHit[] leftHits = Physics.RaycastAll(transform.position, Vector3.left, 0.6f);
            RaycastHit[] rightHits = Physics.RaycastAll(transform.position, Vector3.right, 0.6f);
            if (leftHits.Length > 0)
            {
                for (int i = 0; i < leftHits.Length; i++)
                {
                    RaycastHit hit = leftHits[i];

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        Physics.gravity = new Vector3(-9.8f, 0, 0);
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
                        Physics.gravity = new Vector3(9.8f, 0, 0);
                    }
                }
            }
        }
    }
}
