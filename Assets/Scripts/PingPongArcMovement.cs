using UnityEngine;

public class PingPongArcMovement2D : MonoBehaviour
{
    [Header("Path Settings")]
    [Tooltip("Distance between left and right points on X.")]
    public float horizontalDistance = 5f;

    [Tooltip("Vertical offset of the upper path relative to the starting height.")]
    public float jumpHeight = 3f;

    [Header("Speed Settings")]
    public float moveSpeed = 2f;
    public float jumpDuration = 1f;

    [Header("Arc Shape")]
    [Tooltip("How far sideways the jump arc bends on X. Positive value bends outwards.")]
    public float arcHorizontalBend = 1f; // Keep this positive for outward bend

    private Vector3 leftGround;
    private Vector3 rightGround;
    private Vector3 leftHigh;
    private Vector3 rightHigh;

    private enum Phase
    {
        GroundLeftToRight,
        JumpUp,
        HighRightToLeft,
        JumpDown
    }

    private Phase phase = Phase.GroundLeftToRight;

    // Bezier data
    private Vector3 jumpStart;
    private Vector3 jumpEnd;
    private Vector3 jumpControl;
    private float jumpT = 0f;

    private bool facingRight = true;

    private void Start()
    {
        // Define path points based on initial position
        leftGround = transform.position;
        rightGround = leftGround + Vector3.right * horizontalDistance;

        leftHigh = leftGround + Vector3.up * jumpHeight;
        rightHigh = rightGround + Vector3.up * jumpHeight;
    }

    private void Update()
    {
        switch (phase)
        {
            case Phase.GroundLeftToRight:
                MoveHorizontal(leftGround, rightGround, true, Phase.JumpUp);
                break;

            case Phase.JumpUp:
                DoArc(Phase.HighRightToLeft);
                break;

            case Phase.HighRightToLeft:
                MoveHorizontal(rightHigh, leftHigh, false, Phase.JumpDown);
                break;

            case Phase.JumpDown:
                DoArc(Phase.GroundLeftToRight);
                break;
        }

        ApplyFlip();
    }

    // ---------- Movement ----------

    private void MoveHorizontal(Vector3 from, Vector3 to, bool movingRight, Phase nextPhase)
    {
        facingRight = movingRight;

        transform.position = Vector3.MoveTowards(transform.position, to, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, to) < 0.01f)
        {
            // Start next phase if it's a jump
            if (nextPhase == Phase.JumpUp)
            {
                // JumpUp: From rightGround to rightHigh.
                // The arc should bend towards the right (positive X direction) to "lean out".
                StartJump(rightGround, rightHigh, /*arc direction*/ 1f);
            }
            else if (nextPhase == Phase.JumpDown)
            {
                // JumpDown: From leftHigh to leftGround.
                // The arc should bend towards the left (negative X direction) to "lean out".
                StartJump(leftHigh, leftGround, /*arc direction*/ -1f);
            }

            phase = nextPhase;
        }
    }

    private void StartJump(Vector3 start, Vector3 end, float arcDirection)
    {
        jumpStart = start;
        jumpEnd = end;
        jumpT = 0f;

        // --- FIXED ARC LOGIC FOR OUTWARD BEND ---
        // P0 (start) and P2 (end) are on a vertical line (same X).
        // To make the arc bend OUTWARDS (like the red drawing), the control point (P1)
        // needs to be shifted in the *same direction* as the overall movement 
        // during the horizontal travel phase, or in the direction *away* from the center.

        // For JumpUp (rightGround to rightHigh):
        //   - start.x is the X position.
        //   - We want to bend the arc to the RIGHT (positive X)
        //   - So, arcDirection should be positive (1f).
        //   - controlX = start.x + arcHorizontalBend * 1f;

        // For JumpDown (leftHigh to leftGround):
        //   - start.x is the X position.
        //   - We want to bend the arc to the LEFT (negative X)
        //   - So, arcDirection should be negative (-1f).
        //   - controlX = start.x + arcHorizontalBend * -1f;

        // This is exactly what the `arcDirection` parameter passed into `StartJump` controls now.
        float controlX = start.x + arcHorizontalBend * arcDirection;

        // Y: The control point should still be above the higher of P0/P2 for the vertical curve.
        float controlY = Mathf.Max(start.y, end.y) + jumpHeight * 0.5f;

        jumpControl = new Vector3(controlX, controlY, start.z);

        // We face the direction of the arc for this jump
        facingRight = arcDirection > 0f;
    }

    private void DoArc(Phase nextPhase)
    {
        jumpT += Time.deltaTime / jumpDuration;
        float t = Mathf.Clamp01(jumpT);

        // Quadratic Bezier: B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        Vector3 p0 = jumpStart;
        Vector3 p1 = jumpControl;
        Vector3 p2 = jumpEnd;

        float oneMinusT = 1f - t;
        Vector3 pos =
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;

        transform.position = pos;

        if (t >= 1f)
        {
            phase = nextPhase;
        }
    }

    // ---------- Visual Facing ----------

    private void ApplyFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}