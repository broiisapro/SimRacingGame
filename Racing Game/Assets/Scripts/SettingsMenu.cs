using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using TMPro;

// SettingsMenu class handles all in-game settings adjustments for the car and audio
// This component should be attached to a settings menu UI panel
public class SettingsMenu : MonoBehaviour
{
    // Controls whether the game should pause when settings menu is open
    public bool pauseGameWhileOpen = true;
    
    // Reference to the car controller component that will be modified
    public CarController car;

    // UI Slider components for each adjustable setting
    public Slider topSpeedSlider;        // Controls maximum speed of the car
    public Slider torqueSlider;          // Controls engine power
    public Slider brakeTorqueSlider;     // Controls brake strength
    public Slider steerAngleSlider;      // Controls maximum steering angle
    public Slider downforceSlider;       // Controls downward force on the car
    public Slider volumeSlider;          // Controls game audio volume
    
    // Text components to display current values of settings
    public TextMeshProUGUI topSpeedLabel;
    public TextMeshProUGUI torqueLabel;
    public TextMeshProUGUI brakeTorqueLabel;
    public TextMeshProUGUI steerAngleLabel;
    public TextMeshProUGUI downforceLabel;
    public TextMeshProUGUI volumeLabel;

    public GameObject MainMenuPanel;

    // Initializes all settings and UI elements when the menu is first loaded
    void Start()
    {
        // Set initial slider values based on car's current settings
        topSpeedSlider.value = car.m_Topspeed;
        torqueSlider.value = car.m_FullTorqueOverAllWheels;
        brakeTorqueSlider.value = car.m_BrakeTorque;
        steerAngleSlider.value = car.m_MaximumSteerAngle;
        downforceSlider.value = car.m_Downforce;
        volumeSlider.value = AudioListener.volume;

        // Add event listeners to detect when slider values change
        topSpeedSlider.onValueChanged.AddListener(UpdateTopSpeed);
        torqueSlider.onValueChanged.AddListener(UpdateTorque);
        brakeTorqueSlider.onValueChanged.AddListener(UpdateBrakeTorque);
        steerAngleSlider.onValueChanged.AddListener(UpdateSteerAngle);
        downforceSlider.onValueChanged.AddListener(UpdateDownforce);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        // Set initial volume
        UpdateVolume(volumeSlider.value);

        // Log initial values for debugging
        Debug.Log($"[INIT] Top Speed Slider: value={topSpeedSlider.value}, min={topSpeedSlider.minValue}, max={topSpeedSlider.maxValue}");
        Debug.Log($"[INIT] Torque Slider: value={torqueSlider.value}, min={torqueSlider.minValue}, max={torqueSlider.maxValue}");
        Debug.Log($"[INIT] Brake Torque Slider: value={brakeTorqueSlider.value}, min={brakeTorqueSlider.minValue}, max={brakeTorqueSlider.maxValue}");
        Debug.Log($"[INIT] Steer Angle Slider: value={steerAngleSlider.value}, min={steerAngleSlider.minValue}, max={steerAngleSlider.maxValue}");
        Debug.Log($"[INIT] Downforce Slider: value={downforceSlider.value}, min={downforceSlider.minValue}, max={downforceSlider.maxValue}");

        // Update all UI labels with initial values
        UpdateTopSpeed(topSpeedSlider.value);
        UpdateTorque(torqueSlider.value);
        UpdateBrakeTorque(brakeTorqueSlider.value);
        UpdateSteerAngle(steerAngleSlider.value);
        UpdateDownforce(downforceSlider.value);

        // Pause game if setting is enabled
        if (pauseGameWhileOpen)
        {
            Time.timeScale = 0f;
        }
    }

    // Updates the car's maximum speed and corresponding UI label
    public void UpdateTopSpeed(float value)
    {
        car.m_Topspeed = value;
        topSpeedLabel.text = $"Top Speed: {value:F0} km/h";
        Debug.Log($"[UPDATE] Top Speed set to: {value}");
    }

    // Updates the car's engine torque and corresponding UI label
    public void UpdateTorque(float value)
    {
        car.m_FullTorqueOverAllWheels = value;
        car.m_CurrentTorque = value; // Updates current torque immediately
        torqueLabel.text = $"Torque: {value:F0} Nm";
        Debug.Log($"[UPDATE] Torque set to: {value}");
    }

    // Updates the car's brake strength and corresponding UI label
    public void UpdateBrakeTorque(float value)
    {
        car.m_BrakeTorque = value;
        brakeTorqueLabel.text = $"Brake Strength: {value:F0}";
        Debug.Log($"[UPDATE] Brake Torque set to: {value}");
    }

    // Updates the car's maximum steering angle and corresponding UI label
    public void UpdateSteerAngle(float value)
    {
        car.m_MaximumSteerAngle = value;
        steerAngleLabel.text = $"Steering Angle: {value:F0}°";
        Debug.Log($"[UPDATE] Steer Angle set to: {value}");
    }

    // Updates the car's downforce and corresponding UI label
    public void UpdateDownforce(float value)
    {
        car.m_Downforce = value;
        downforceLabel.text = $"Downforce: {value:F0}";
        Debug.Log($"[UPDATE] Downforce set to: {value}");
    }

    // Updates the game's audio volume and corresponding UI label
    public void UpdateVolume(float value)
    {
        AudioListener.volume = value;
        volumeLabel.text = $"Volume: {(value * 100f):F0}%";
    }

    // Handles returning to the main menu
    public void ReturnToMainMenu()
    {
        // Resume game time if it was paused
        if (pauseGameWhileOpen)
        {
            Time.timeScale = 1f;
        }
        gameObject.SetActive(false);      // Hide settings menu
        MainMenuPanel.SetActive(true);    // Show main menu
    }
}