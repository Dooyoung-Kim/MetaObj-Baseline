using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointConnection : MonoBehaviour
{
    public string thisPart;
    public string targetPart;
    public bool isConnected;

    void Awake()
    {
        isConnected = false;
    }

    public void connectJoint(GameObject other)
    {
        Rigidbody rbThis = transform.parent.gameObject.GetComponent<Rigidbody>();
        Rigidbody rbOther = other.transform.parent.gameObject.GetComponent<Rigidbody>();

        if (rbThis == null || rbOther == null)
        {
            Debug.LogWarning("���� �θ� Rigidbody�� �����ϴ�.");
            return;
        }

        if (rbThis.mass <= rbOther.mass)
        {
            Quaternion normalizedRotation = Quaternion.Normalize(other.transform.parent.rotation);
            transform.parent.rotation = normalizedRotation;

            Vector3 posOffset = other.transform.position - transform.position;
            transform.parent.position += posOffset;

            FixedJoint fj = transform.parent.gameObject.GetComponent<FixedJoint>();
            if (fj == null)
            {
                FixedJoint joint = transform.parent.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = rbOther;
                joint.breakForce = Mathf.Infinity;
                joint.breakTorque = Mathf.Infinity;
                joint.massScale = 1000;
                joint.enableCollision = false;
            }
            else
            {
                if (fj.connectedBody != rbOther)
                {
                    FixedJoint joint = transform.parent.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = rbOther;
                    joint.breakForce = Mathf.Infinity;
                    joint.breakTorque = Mathf.Infinity;
                    joint.massScale = 1000;
                    joint.enableCollision = false;
                }
            }
        }
        else
        {
            Quaternion normalizedRotation = Quaternion.Normalize(transform.parent.rotation);
            other.transform.parent.rotation = normalizedRotation;

            Vector3 posOffset = transform.position - other.transform.position;
            other.transform.parent.position += posOffset;

            FixedJoint fj = other.transform.parent.gameObject.GetComponent<FixedJoint>();
            if (fj == null)
            {
                FixedJoint joint = other.transform.parent.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = rbThis;
                joint.breakForce = Mathf.Infinity;
                joint.breakTorque = Mathf.Infinity;
                joint.massScale = 1000;
                joint.enableCollision = false;
            }
            else
            {
                if (fj.connectedBody != rbThis)
                {
                    FixedJoint joint = other.transform.parent.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = rbThis;
                    joint.breakForce = Mathf.Infinity;
                    joint.breakTorque = Mathf.Infinity;
                    joint.massScale = 1000;
                    joint.enableCollision = false;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "joint")
        {
            JointConnection otherJoint = other.gameObject.GetComponent<JointConnection>();
            if (otherJoint.thisPart == targetPart && otherJoint.targetPart == thisPart)
            {
                if (!isConnected && !(otherJoint.isConnected))
                {
                    connectJoint(other.gameObject);
                    isConnected = true;
                    otherJoint.isConnected = true;

                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                    }
                }
            }
        }
    }
}
