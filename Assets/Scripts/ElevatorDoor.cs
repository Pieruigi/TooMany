using UnityEngine;

public class AutoElevatorDoors : MonoBehaviour
{
    [Header("Door References")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Door Positions (local X)")]
    public float leftOpenX;     
    public float rightOpenX;    
    public float leftClosedX;   
    public float rightClosedX;  

    [Header("Block Position")]
    [Tooltip("X position where the doors stop if blocked (if equal to closed → no block).")]
    public float leftBlockX;
    public float rightBlockX;

    [Header("Settings")]
    public float speed = 2f;           // door movement speed
    public float waitTime = 1f;        // time to wait when fully open before closing again

    private enum DoorState { Opening, OpenWait, Closing }
    private DoorState state = DoorState.Opening;
    private float timer;

    [Header("Audio")]
    [SerializeField]
    AudioSource audioSource;

    void Update()
    {
        switch (state)
        {
            case DoorState.Opening:
                // Move both doors towards open positions
                MoveDoor(leftDoor, leftOpenX);
                MoveDoor(rightDoor, rightOpenX);

                if (DoorsAtTarget(leftOpenX, rightOpenX))
                {
                    state = DoorState.OpenWait;
                    timer = waitTime;
                }
                break;

            case DoorState.OpenWait:
                // Wait a little while before trying to close
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = DoorState.Closing;
                    audioSource.Play();
                }
                    
                break;

            case DoorState.Closing:
                // Try to close doors, but stop at block if defined
                float targetL = leftBlockX != leftClosedX ? leftBlockX : leftClosedX;
                float targetR = rightBlockX != rightClosedX ? rightBlockX : rightClosedX;

                MoveDoor(leftDoor, targetL);
                MoveDoor(rightDoor, targetR);

                // If blocked → reopen immediately
                if (targetL == leftBlockX || targetR == rightBlockX)
                {
                    if (DoorsAtTarget(leftBlockX, rightBlockX))
                        state = DoorState.Opening;
                }
                else if (DoorsAtTarget(leftClosedX, rightClosedX))
                {
                    // If fully closed → open again after wait
                    state = DoorState.Opening;
                }
                break;
        }
    }

    // Smoothly move a single door toward its target X position
    private void MoveDoor(Transform door, float targetX)
    {
        Vector3 pos = door.localPosition;
        pos.x = Mathf.MoveTowards(pos.x, targetX, speed * Time.deltaTime);
        door.localPosition = pos;
    }

    // Check if both doors have reached their target positions
    private bool DoorsAtTarget(float lx, float rx)
    {
        return Mathf.Approximately(leftDoor.localPosition.x, lx) &&
               Mathf.Approximately(rightDoor.localPosition.x, rx);
    }
}
