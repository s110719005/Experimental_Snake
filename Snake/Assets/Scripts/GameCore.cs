using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance;
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

    public void EndGame()
    {
        TakeScreenShot();
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
        string folderPath = Application.dataPath + "/screenshots/" + "GradientSnake.png";
        if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
            System.IO.Directory.CreateDirectory(folderPath);  // it will get created
        ScreenCapture.CaptureScreenshot(folderPath, 2);
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
