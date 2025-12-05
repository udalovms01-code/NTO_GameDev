using UnityEngine;

public class MoveableSmoothDamp : MoveableBase
{
    private Vector3 velocity; 
    public float smoothTime = 0.3F; 
    public float maxVelocity = 10f; 
    private Vector3 currentVelocity;

    protected override void PausableUpdate()
    {
        MoveXY();
    }

    protected void MoveXY()
    {
        if (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f || velocity.magnitude > 0.01f)
        {
            Vector3 newPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref currentVelocity, smoothTime, maxVelocity, Time.deltaTime);
            velocity = (newPosition - (Vector3)transform.localPosition) / Time.deltaTime;

            if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
            {
                velocity = velocity.normalized * maxVelocity;
            }

            transform.localPosition = newPosition + velocity * Time.deltaTime;
            if (Vector3.Distance((Vector3)transform.localPosition, targetPosition) < 0.01f && velocity.magnitude < 0.01f)
            {
                transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, transform.localPosition.z);
                velocity = Vector3.zero;
            }

            //transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        }
    }
}