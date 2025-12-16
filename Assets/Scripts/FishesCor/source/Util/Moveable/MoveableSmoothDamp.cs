using UnityEngine;

public class MoveableSmoothDamp : MoveableBase
{
    private Vector3 velocity;
    public float smoothTime = 0.3F;


    private Vector3 currentVelocity;

    protected override void PausableUpdate()
    {
        MoveXY();
    }

    protected void MoveXY()
    {
        if (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f || velocity.magnitude > 0.01f)
        {
            Vector3 newPosition = Vector3.SmoothDamp(
                transform.localPosition, targetPosition, 
                ref currentVelocity, smoothTime, 
                G.main.fishFeelConfig.dragSpeed, 
                Time.deltaTime);
            velocity = (newPosition - (Vector3)transform.localPosition) / Time.deltaTime;

            if (velocity.sqrMagnitude > G.main.fishFeelConfig.dragSpeed * G.main.fishFeelConfig.dragSpeed)
            {
                velocity = velocity.normalized * G.main.fishFeelConfig.dragSpeed;
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