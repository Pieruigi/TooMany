using UnityEngine;

public class ScreenshotTaker : SingletonPersistent<ScreenshotTaker>
{
    public KeyCode screenshotKey = KeyCode.E; // tasto che vuoi usare
    public string folderName = "Screenshots"; // cartella dove salvare

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(screenshotKey))
        {
            string folderPath = Application.dataPath + "/" + folderName;

            // Create folder if needed
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            // File name with date
            string fileName = $"screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            string fullPath = folderPath + "/" + fileName;

            ScreenCapture.CaptureScreenshot(fullPath);
            Debug.Log("Screenshot salvato in: " + fullPath);
        }
#endif
    }
}
