using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
#if UNITY_EDITOR
public class Editor_BuildGameScript : MonoBehaviour
{
    [MenuItem("Build/Generate Game Build")]
    public static void PerformBuild()
    {
        string FolderDestination = "D:/GameDev/BAKI_StrongestShowdown_Builds";
        PlayerSettings.SplashScreen.show = false;
        /* Get Build Folder Path */
        var rootFolder = $"{FolderDestination}/";
        DateTime currentTimeOfGeneration = DateTime.Now;
        string date = $"{currentTimeOfGeneration.Month}_{currentTimeOfGeneration.Day}";
        var buildFolder = Path.Combine(rootFolder, $"BAKI_SS_TESTBUILD_{date}");
        /* Delete existing Build Folder if it exists */
        FileUtil.DeleteFileOrDirectory(buildFolder);

        var scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenePaths,
            locationPathName = Path.Combine(buildFolder, $"{Application.productName}.exe"),
            target =  BuildTarget.StandaloneWindows,
            options = BuildOptions.CompressWithLz4HC
        };

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        /* Check that the build succeeded and report errors if it did not */
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize} bytes.");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed.");
            throw new Exception($"Build Failed. Errors: {summary.totalErrors} Warnings: {summary.totalWarnings}");
        }
    }
    [MenuItem("Build/TestFolderPath")]
    public static void TestFolderPath()
    {
        string FolderDestination = "D:/GameDev/BAKI_StrongestShowdown_Builds";
        Debug.Log($"{FolderDestination}/../");
    }
}
#endif