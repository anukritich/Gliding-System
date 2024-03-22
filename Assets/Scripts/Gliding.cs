using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlidingSystem : MonoBehaviour
{
    [SerializeField] private float BaseSpeed = 30f;
    [SerializeField] private float MaxThrustSpeed = 400;
    [SerializeField] private float MinThrustSpeed = 60;
    [SerializeField] private float ThrustFactor = 80;
    [SerializeField] private float DragFactor = 1;
    [SerializeField] private float MinDrag;
    [SerializeField] private float RotationSpeed = 5;
    [SerializeField] private float LowPercent = 0.1f, HighPercent = 1;

    private float CurrentThrustSpeed;
    private float TiltValue, LerpValue;

    private Transform CameraTransform;
    private Rigidbody Rb;

    private void Start()
    {
        CameraTransform = Camera.main.transform.parent;
        Rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GlidingMovement();
    }

    private void Update()
    {
       
        ManageRotation();
    }

    private void GlidingMovement()
    {
        float pitchInDeg = transform.eulerAngles.x % 360;
        float pitchInRads = transform.eulerAngles.x * Mathf.Deg2Rad;
        float mappedPitch = -Mathf.Sin(pitchInRads);
        float offsetMappedPitch = Mathf.Cos(pitchInRads) * DragFactor;
        float accelerationPercent = pitchInDeg >= 300f ? LowPercent : HighPercent;
        Vector3 glidingForce = -Vector3.right * CurrentThrustSpeed;

        CurrentThrustSpeed += mappedPitch * accelerationPercent * ThrustFactor * Time.deltaTime;
        CurrentThrustSpeed = Mathf.Clamp(CurrentThrustSpeed, 0, MaxThrustSpeed);

        if (Rb.velocity.magnitude >= MinThrustSpeed)
        {
            Rb.AddRelativeForce(glidingForce);
            Rb.drag = Mathf.Clamp(offsetMappedPitch, MinDrag, DragFactor);
        }
        else
        {
            CurrentThrustSpeed = 0;
        }

        Debug.Log(CurrentThrustSpeed);
    }
    private void ManageRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        

        if (mouseX == 0 && mouseY == 0)
        {
            TiltValue = Mathf.Lerp(TiltValue, 0, LerpValue);
            LerpValue += Time.deltaTime;
        }
        else
        {
            LerpValue = 0;
        }

        Quaternion targetRotation = Quaternion.Euler(CameraTransform.eulerAngles.x, CameraTransform.eulerAngles.y, TiltValue);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }
}

