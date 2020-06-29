using Simulation;
using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour {
    // Start is called before the first frame update
    bool _screenshotCheck;

    IEnumerator  Start() {
        var config = LoadJson();
        Debug.Log(JsonUtility.ToJson(config));
        for (int i = 0; i < 1000; i++) {
            config.BuildScene();
            //DynamicGI.UpdateEnvironment();
            StartCoroutine("CaptureScreen");
            //yield return new WaitUntil(() => !_screenshotCheck);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    IEnumerator CaptureScreen() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        var folderPath = "Screenshots/";
        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        var screenshotName =
            "Screenshot_" +
            System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
            "-1.png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName));
        yield return new WaitForSecondsRealtime(0.4F);
         screenshotName =
            "Screenshot_" +
            System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
            "-2.png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName));
        yield return new WaitForSecondsRealtime(0.3F);
        screenshotName =
            "Screenshot_" +
            System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
            "-3.png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName));
        //wenta mal omak enta dafa3 7aga men geebak
        var objects = FindObjectsOfType<GameObject>();
        foreach (var g in objects) {
            if (g.name != "root")
                Destroy(g.gameObject);
        }
        _screenshotCheck = true;
    }

    static SceneDefinition LoadJson() {
        const string path = "Config/config";
        var targetFile = Resources.Load<TextAsset>(path);
        var config = ScriptableObject.CreateInstance<SceneDefinition>();
        config.Load(targetFile.text);
        return config;
    }

    // Update is called once per frame
    void Update() {
    }
}