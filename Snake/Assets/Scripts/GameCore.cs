using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;
    [SerializeField]
    SnakeManager snakeManager;
    [SerializeField]
    private bool isTitle;
    private GridObject startGridObject;
    private GridObject exitGridObject;
    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if(isTitle)
        {
            GenerateStartMenu();
        }
        snakeManager.GenerateSnake();
    }

    private void GenerateStartMenu()
    {
        var gridObjects = GridManager.Instance.CurrentGrid.GridObjects;
        startGridObject = gridObjects[8, 7];
        exitGridObject = gridObjects[8, 2];
        GridManager.Instance.SetGridColor(startGridObject, Color.black);
        GridManager.Instance.SetGridColor(exitGridObject, Color.black);
        GridManager.Instance.SetGridBoolValue(startGridObject, true);
        GridManager.Instance.SetGridBoolValue(exitGridObject, true);
    }

    public void MenuCollisionCheck(GridObject collideGridObject)
    {
        if(startGridObject == null || exitGridObject == null) { return; }
        if(collideGridObject == startGridObject)
        {
            StartGame();
        }
        else if(collideGridObject == exitGridObject)
        {
            ExitGame();
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void StartGame()
    {
        startGridObject = null;
        exitGridObject = null;
        SceneManager.LoadScene("GameScene");
        Destroy(this);
    }

    public void EndGame()
    {
        TakeScreenShot();
        SceneManager.LoadScene("TitleScene");
    }

    private void TakeScreenShot()
    {
#if UNITY_EDITOR
        string folderPath = "Assets/Screenshots/"; // the path of your project folder

        if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
            System.IO.Directory.CreateDirectory(folderPath);  // it will get created
        
        var screenshotName =
                                "Screenshot_" +
                                //System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                "GradientSnake.png"; // put youre favorite data format here
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName),2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
        Debug.Log(folderPath + screenshotName); // You get instant feedback in the console
#else
        string folderPath = Application.streamingAssetsPath + "/screenshots/";
        if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
            System.IO.Directory.CreateDirectory(folderPath);  // it will get created
        var screenshotName =
                                "Screenshot_" +
                                //System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                "GradientSnake.png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName),2);
        //UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
