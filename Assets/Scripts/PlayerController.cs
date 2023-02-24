using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Outlets
    private Rigidbody _rb;

    // State Tracking
    public bool canJump;

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
        
        Debug.DrawRay(transform.position, Vector3.down * 1.2f, Color.red);
    }


    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 1.2f);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    canJump = true;
                }
            }
        }
    }
}
