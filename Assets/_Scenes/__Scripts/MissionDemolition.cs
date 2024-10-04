using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";


    // Start is called before the first frame update
    void Start()
    {
        S = this;// define the slington

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        //Get rid of old castle if one exists
        if(castle != null)
        {
            Destroy(castle);
        }

        //destroy old projectiles if they exist
        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        //reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
        //Zoom out to show both
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;

    }



    void Update()
    {
        UpdateGUI();
        
        //check for level end
        if((mode== GameMode.playing) && Goal.goalMet)
        {
            mode = GameMode.levelEnd;

            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            //start the next level in 2 sec
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            level = 0;
            shotsTaken = 0;
        }
        StartLevel();
    }

    static public void SHOT_FIRED()
    {
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }









}
