﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This class essentially handles all input for Camera, Players AND Gameplay Logic (e.g. EndStates)
/// </summary>

public class PlayerMovement : MonoBehaviour {

    public float CameraSpeed;
    public float CameraAcceleration;

    private RobotInput[] RobotArray;
    private RobotInput.RobotType RobotInFocus;
    private int inputNumber;
    private int previousInput;
    private Vector2 inputVectors;
    private float xTime;
    private float yTime;
    private bool gameHasStarted = false;
    private bool gameIsOver = false;
    private int aliveBots=0;
    private int safeBots=0;
    private TextMesh score;

	// Use this for initialization
	void Start () {
        //Need to Retrieve Robots from Spawn and not sent out from prefab or we are doomed :/
        RobotArray = new RobotInput[0];
        score = GetComponentInChildren<TextMesh>();
	    //Nothing else to do here, right??
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Do a Full Quit and Reset
            Application.LoadLevel("Menu");
            PlayerContainer.Instance.InitialSpawning = true;
            PlayerContainer.Instance.score = 0;
            PlayerContainer.Instance.level = 0;
        }

        //Check if the EndCondition has been met
        CheckRobots();

        if (RobotArray.Length == 0)
        {
            RetrieveRobotControllers();
        }
        else
        {
            HandleRobots();
        }

        HandleOverviewCamera();
	}

    private void RetrieveRobotControllers()
    {
        RobotArray = (RobotInput[])Object.FindObjectsOfType<RobotInput>(); //Hopefully these are the ones we want, right?
        Debug.Log(RobotArray);
        Debug.Log("Initializing Player Movement...");

        inputNumber = 0;
        RobotArray[inputNumber].BroadcastMessage("ShowSelector");
        RobotArray[inputNumber].isFocus = true;
        RobotInFocus = RobotArray[inputNumber].Type;
        aliveBots = RobotArray.Length;
        gameHasStarted = true;
    }

    private void HandleRobots()
    {
        previousInput = inputNumber;
        //Get The Inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inputNumber = 0;
        }
		else if (Input.GetKeyDown(KeyCode.Alpha2) && RobotArray.Length > 1)
        {
            inputNumber = 1;
        }
		else if (Input.GetKeyDown(KeyCode.Alpha3) && RobotArray.Length > 2)
        {
            inputNumber = 2;
        }
		else if (Input.GetKeyDown(KeyCode.Alpha4) && RobotArray.Length > 3)
        {
            inputNumber = 3;
        }

        if (inputNumber == previousInput)
        {
            //Nothing to do here then, right?
            return;
        }

        RobotArray[previousInput].BroadcastMessage("HideSelector");
        RobotArray[inputNumber].BroadcastMessage("ShowSelector");

        for (int i = 0; i < RobotArray.Length; i++)
        {
            if (i == inputNumber)
            {
                RobotArray[inputNumber].isFocus = true;
                RobotInFocus = RobotArray[inputNumber].Type;
            }
            else
            {
                RobotArray[i].isFocus = false;
            }
        }
    }

    private void HandleOverviewCamera()
    {
        inputVectors.x = Input.GetAxisRaw("Horizontal") * CameraSpeed * TimeManager.GetTime(TimeType.Engine);
        inputVectors.y = Input.GetAxisRaw("Vertical") * CameraSpeed * TimeManager.GetTime(TimeType.Engine);

        //Want to have Max and Min zoom Size for Camera to avoid Aliasing
        float scrollWheel = Input.GetAxisRaw("Mouse ScrollWheel") * CameraSpeed;
        if (scrollWheel > 0)
        {
            Camera.main.orthographicSize -= 1;
        }
        else if (scrollWheel < 0)
        {
            Camera.main.orthographicSize += 1;
        }

        transform.Translate(inputVectors.x, inputVectors.y, 0);
    }

    private void CheckRobots()
    {
        if (gameHasStarted && !gameIsOver)
        {
            if (aliveBots == 0)
            {
                Debug.Log("GAME OVER");
                gameIsOver = true;
                Application.LoadLevel("GameOver");
                //Nice GUI thing here
            }
            else if (safeBots == aliveBots && safeBots!=0)
            {
                //Load the next level
                Debug.Log("Load Next Level");
                safeBots = 0;
                Application.LoadLevel("TestScene2");
                PlayerContainer.Instance.score += 100;
            }
        }
    }

    public void RegisterKilledRobot(RobotInput someBot)
    {
        aliveBots--;
    }

    public void RegisterSafeRobot(RobotInput someBot)
    {
        safeBots++;
    }

    //private void OnGUI()
    //{
    //    if (gameIsOver)
    //    {
    //        Time.timeScale = 0;
    //        GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height - 350, 100, 30), "Hard");
    //    }
    //}

}
