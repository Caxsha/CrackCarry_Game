using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidbody3D;

    [SerializeField]
    ConfigurableJoint mainJoint;

    //Input
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    //Controller settings
    float maxSpeed = 3;

    //States
    bool isGrounded = false;

    //Raycasts
    RaycastHit[] raycastHits = new RaycastHit[10];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpButtonPressed = true;
            Debug.Log("Jump button pressed");
        }    
    }

    void FixedUpdate()
    {
        // Ïðîâåðÿåì, ñòîèò ëè ïåðñîíàæ íà çåìëå
        isGrounded = false;

        int numberOfHits = Physics.SphereCastNonAlloc(rigidbody3D.position, 0.2f, transform.up * -1, raycastHits, 0.5f);

        for (int i = 0; i < numberOfHits; i++)
        {
            if (raycastHits[i].transform.root == transform)
                continue;

            isGrounded = true;

            break;
        }

        if (!isGrounded)
            rigidbody3D.AddForce(Vector3.down * 10);

        float inputMagnitued = moveInputVector.magnitude;

        if (inputMagnitued != 0)
        {
            Quaternion desiredDerection = Quaternion.LookRotation(
                new Vector3(moveInputVector.x, 0, moveInputVector.y * -1),
                transform.up
                );

            mainJoint.targetRotation = Quaternion.RotateTowards(
                mainJoint.targetRotation, desiredDerection, Time.fixedDeltaTime * 300
                );

            // ÎÃÐÀÍÈ×ÈÂÀÅÌ ÃÎÐÈÇÎÍÒÀËÜÍÓÞ ÑÊÎÐÎÑÒÜ
            Vector3 horizontalVelocity = new Vector3(rigidbody3D.velocity.x, 0, rigidbody3D.velocity.z);

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                rigidbody3D.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rigidbody3D.velocity.y;
            }

            // ÄÂÈÆÅÍÈÅ ÏÅÐÑÎÍÀÆÀ
            Vector3 localVelocifyVsForward = transform.forward * Vector3.Dot(transform.forward, rigidbody3D.velocity);

            float localForwardVelocity = localVelocifyVsForward.magnitude;

            if (localForwardVelocity < maxSpeed)
            {
                rigidbody3D.AddForce(transform.forward * inputMagnitued * 30);
            }
        }

        if (isGrounded && isJumpButtonPressed)
        {
            rigidbody3D.AddForce(Vector3.up * 20, ForceMode.Impulse);

            isJumpButtonPressed = false;
        }
            
    }
}
