using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Iniscribed")]
    public TextMeshProUGUI uitLevel;
    public TextMeshProUGUI uitShots;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level; // Current level
    public int levelMax; // Maximum levels
    public int shotsTaken; // Number of shots taken
    public GameObject castle; // Current castle object
    public GameMode mode = GameMode.idle; // Current game mode
    public GameObject gameOverPanel; // Game Over UI panel
    public Button playAgainButton; // Button to restart the game

    // Start is called before the first frame update
    void Start()
    {
        S = this; // Define the singleton

        level = 0; // Initialize level
        shotsTaken = 0;

        levelMax = castles.Length; // Set max levels based on the castles length
        StartLevel(); // Start the first level

        gameOverPanel.SetActive(false); // Hide Game Over panel at the start
        playAgainButton.onClick.AddListener(RestartGame);
    }

    void StartLevel()
    {
        // Get rid of old castle if one exists
        if (castle != null)
        {
            Destroy(castle);
        }

        // Destroy old projectiles if they exist
        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI(); // Update the UI with current level and shots

        mode = GameMode.playing; // Set game mode to playing
        FollowCam.SWITCH_VIEW(FollowCam.eView.both); // Zoom out to show both
    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    void Update()
    {
        UpdateGUI(); // Update GUI every frame
        
        // Check for level end
        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd; // Set mode to level end

            FollowCam.SWITCH_VIEW(FollowCam.eView.both); // Zoom out to show both

            // Start the next level in 2 sec
            Invoke("NextLevel", 2f);
        }

    }

    void NextLevel()
    {
        level++; // Increment level
        if (level >= levelMax) // Check if exceeding max levels
        {

            GameOver();
            level = 0; // Reset to first level
            shotsTaken = 0; // Reset shots taken
            
        }
        StartLevel(); // Start the new level
    }

    static public void SHOT_FIRED()
    {
        S.shotsTaken++; // Increment shots taken
    }

    static public GameObject GET_CASTLE()
    {
        return S.castle; // Get current castle
    }

    public void GameOver()
    {
    
        if (gameOverPanel.activeSelf) return; // Prevent re-triggering the Game Over panel

        gameOverPanel.SetActive(true); // Show the Game Over panel
        Time.timeScale = 0; // Stop the game time
    }

    // Method to restart the game
    void RestartGame()
    {
        Time.timeScale = 1; // Resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void RemoveProjectiles()
    {
        Projectile.DESTROY_PROJECTILES(); // Call your method to remove projectiles
        Debug.Log("All projectiles removed."); // Optional: Log for debugging
    }
}
