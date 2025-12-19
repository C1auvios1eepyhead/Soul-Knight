using UnityEngine;
using System.Reflection;

public class Map_PlayerSpeedBoost : MonoBehaviour
{
    private float endTime;
    private float multiplier = 1.5f;

    private Component playerMovement;
    private FieldInfo speedField;       // private float speed;
    private FieldInfo currentSpeedField; // private float currentSpeed;
    private FieldInfo usingDashField;    // private bool usingDash;

    private float originalSpeed;
    private bool applied;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogWarning("[Map_PlayerSpeedBoost] PlayerMovement not found on Player.");
            enabled = false;
            return;
        }

        var t = playerMovement.GetType();
        speedField = t.GetField("speed", BindingFlags.Instance | BindingFlags.NonPublic);
        currentSpeedField = t.GetField("currentSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        usingDashField = t.GetField("usingDash", BindingFlags.Instance | BindingFlags.NonPublic);

        if (speedField == null || currentSpeedField == null || usingDashField == null)
        {
            Debug.LogWarning("[Map_PlayerSpeedBoost] Reflection fields not found. Check PlayerMovement variable names.");
            enabled = false;
        }
    }

    public void Refresh(float newMultiplier, float duration)
    {
        multiplier = Mathf.Max(1f, newMultiplier);
        endTime = Time.time + duration;

        if (!applied)
        {
            originalSpeed = (float)speedField.GetValue(playerMovement);
            ApplyBoost();
            applied = true;
        }
        else
        {
            // It is already accelerating. Only the refresh time (not cumulative)
        }
    }

    private void ApplyBoost()
    {
        float boostedSpeed = originalSpeed * multiplier;
        speedField.SetValue(playerMovement, boostedSpeed);

        bool usingDash = (bool)usingDashField.GetValue(playerMovement);
        if (!usingDash)
        {
            currentSpeedField.SetValue(playerMovement, boostedSpeed);
        }
        // If player is on Dash, make it so that it automatically returns to speed (which is now boostedSpeed) after Dash ends.
    }

    private void Update()
    {
        if (!applied) return;

        if (Time.time > endTime)
        {
            // restore
            speedField.SetValue(playerMovement, originalSpeed);

            bool usingDash = (bool)usingDashField.GetValue(playerMovement);
            if (!usingDash)
            {
                currentSpeedField.SetValue(playerMovement, originalSpeed);
            }

            Destroy(this); // Clean the components
        }
    }
}
