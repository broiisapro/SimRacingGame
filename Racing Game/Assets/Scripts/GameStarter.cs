using UnityEngine;
using TMPro;

public class GameStarter : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject car;
    public Transform carStartPoint;
    public TextMeshProUGUI DeathText;

    void Start()
    {
        // Game will remain paused until StartGame() is called
    }

    //called from the main menu button
    public void FreshGame()
    {
        if (car != null && carStartPoint != null)
        {
            car.transform.position = carStartPoint.position;
            car.transform.rotation = carStartPoint.rotation;

            Rigidbody rb = car.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Resume game time
        Time.timeScale = 1f;

        // Hide the home panel
        if (homePanel != null)
            homePanel.SetActive(false);

        // Notify other scripts to begin
        MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.SendMessage("StartGame", SendMessageOptions.DontRequireReceiver);
        }
    }

    void Update()
    {
        if (car != null && car.transform.position.y < -10f)
        {
            // Pause the game
            Time.timeScale = 0f;

            // Show the home panel
            if (homePanel != null)
                homePanel.SetActive(true);

            // Reset the car position
            car.transform.position = carStartPoint.position;
            car.transform.rotation = carStartPoint.rotation;
            DeathText.gameObject.SetActive(true);
        }
    }
}