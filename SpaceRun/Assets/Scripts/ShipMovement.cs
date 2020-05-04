using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotatespeed;
    public float moveSpeedIncreaseAmount;
    public float rotatespeedIncreaseAmount;
    public Rigidbody rb;
    public bool matchTube;
    public float rotationSmoothing = 0.1f;
    public float rotationCatchup = 0.01f;
    LightSpeed lightSpeed;
    PathManager pm;

    private void Start()
    {
        lightSpeed = GameObject.FindWithTag("GameController").GetComponent<LightSpeed>();
        rb = GetComponent<Rigidbody>();
        pm = GameController.Instance.pathManager;
        //rb.velocity = Vector3.forward * moveSpeed;
    }
    public void Faster()
    {
        moveSpeed += moveSpeedIncreaseAmount;
        rotatespeed += rotatespeedIncreaseAmount;
    }

    Vector3 lastLocalRollVector = Vector3.zero;
    // Update is called once per frame
    void FixedUpdate()
    {
        //!lightSpeed.LighSpeedActive
        if (!lightSpeed.LighSpeedActive)
        {
            //Turning left and right
            rb.AddRelativeTorque(Vector3.up * Input.GetAxis("Horizontal") * rotatespeed);
            rb.AddRelativeTorque(Vector3.left * Input.GetAxis("Vertical") * rotatespeed);

            //Constant moving foward
            rb.velocity = transform.forward * moveSpeed;

            if (matchTube)
            {
                float minDist = float.PositiveInfinity;
                int minIndex = 0;
                for (int i = 0; i < pm.Waypoints.Count; i++)
                {
                    float distance = Vector3.Distance(transform.position, pm.Waypoints.ElementAt(i));
                    if (distance < minDist)
                    {
                        minDist = distance;
                        minIndex = i;
                    }
                }
                Vector3 nearestPoint = pm.Waypoints.ElementAt(minIndex);
                Vector3 nextPoint = pm.Waypoints.ElementAt(minIndex + 1);
                Vector3 farPoint = pm.Waypoints.ElementAt(minIndex + 2);
                Debug.Log("MinIndex: " + minIndex.ToString());
                Vector3 rollDirection = Vector3.Normalize(
                    Vector3.Cross(nextPoint - nearestPoint,
                        Vector3.Cross(nextPoint - nearestPoint, farPoint - nextPoint)));
                Vector3 localRollVector = Vector3.ProjectOnPlane(rollDirection, transform.forward);
                if(lastLocalRollVector == Vector3.zero)
                {
                    lastLocalRollVector = localRollVector;
                }
                else
                {
                    localRollVector = Vector3.Normalize(Vector3.Lerp(localRollVector, lastLocalRollVector,rotationSmoothing));

                }
                Vector3 rotation = transform.localEulerAngles;
                rotation.z += Mathf.Clamp(Vector3.SignedAngle(-transform.up, localRollVector, transform.forward)*rotationCatchup,-1f, 1f)
                    * Mathf.Abs(rb.angularVelocity.y);
                transform.localEulerAngles = rotation;
            }

        }
        else
        {
            rb.velocity = Vector3.zero;
        }

    }
}
