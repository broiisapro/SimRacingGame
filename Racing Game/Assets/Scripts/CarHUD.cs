using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarHUD : MonoBehaviour
{
    public Rigidbody carRigidbody;
    public Transform carTransform;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI gForceText;
    public TextMeshProUGUI otherStatsText;

    public GameObject settingsPanel;
    public KeyCode toggleSettingsKey = KeyCode.Escape;

    private Vector3 lastVelocity;
    private float gForce;
    private float smoothedGForce = 0f;

    void Start()
    {
        // Initialize lastVelocity in Start
        lastVelocity = carRigidbody.linearVelocity;
    }

    void Update()
    {
        UpdateSpeed();
        UpdateGForce();

        if (Input.GetKeyDown(toggleSettingsKey))
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }

    void UpdateSpeed()
    {
        float speed = carRigidbody.linearVelocity.magnitude * 3.6f; // m/s to km/h
        speedText.text = $"Speed: {speed:F1} km/h";
    }

    void UpdateGForce()
    {
        Vector3 acceleration = (carRigidbody.linearVelocity - lastVelocity) / Time.deltaTime;
        // Add safety check for NaN and infinity
        float rawGForce = Mathf.Abs(acceleration.magnitude / 9.81f);
        if (!float.IsNaN(rawGForce) && !float.IsInfinity(rawGForce))
        {
            smoothedGForce = Mathf.Lerp(smoothedGForce, rawGForce, 0.01f);
            gForce = smoothedGForce;
        }
        gForceText.text = $"G-Force: {gForce:F2} G";
        lastVelocity = carRigidbody.linearVelocity;
    }
}