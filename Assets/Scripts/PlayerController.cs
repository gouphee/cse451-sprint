using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Outlets
    private Rigidbody _rb;
    
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
            _rb.AddForce(Vector3.left * 15f * Time.deltaTime, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _rb.AddForce(Vector3.right * 15f * Time.deltaTime, ForceMode.Impulse);
        }
        
        //Jump!
        if (Input.GetKey(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * 0.05f, ForceMode.Impulse);
        }
    }
}
