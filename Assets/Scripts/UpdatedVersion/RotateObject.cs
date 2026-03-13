using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float rotationSpeed = 100f;

    // Choose the axis of rotation (default is Vector3.up for Y-axis rotation)
    public Vector3 rotationAxis = Vector3.up;

    // If you want the object to rotate around a point, specify it here
    public Transform rotationPoint;

    void Update()
    {
        // If there is a rotation point, rotate around that point
        if (rotationPoint != null)
        {
            // Rotate around the point with the specified axis and speed
            transform.RotateAround(rotationPoint.position, rotationAxis, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Rotate around the object's own axis
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}
