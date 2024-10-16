using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
#if UNITY_EDITOR
public class Editor_BuildGameScript : MonoBehaviour
{
    [MenuItem("Build/Make New Game Build")]
    public static void PerformBuild()
    {
        PlayerSettings.SplashScreen.show = false;
        /* Get Build Folder Path */
        var rootFolder = $"{Application.dataPath}/../";
        DateTime currentTimeOfGeneration = DateTime.Now;
        string date = $"{currentTimeOfGeneration.Month}_{currentTimeOfGeneration.Day}_{currentTimeOfGeneration.Year}";
        var buildFolder = Path.Combine(rootFolder, $"BAKI_TESTBUILD_{date}");
        /* Delete existing Build Folder if it exists */
        FileUtil.DeleteFileOrDirectory(buildFolder);

        var scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenePaths,
            locationPathName = Path.Combine(buildFolder, $"{Application.productName}.exe"),
            target = JsonConfigurator.Is32BitBuild() ? BuildTarget.StandaloneWindows : BuildTarget.StandaloneWindows64,
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

       /* string configRootPath = JsonConfigurator.GetConfigRootPath();

        if (!File.Exists(configRootPath))
        {
            //JsonConfigurator.CreateJsonConfig();
        }*/

        //FileUtil.CopyFileOrDirectory(configRootPath, Path.Combine(buildFolder, JsonConfigurator.configJson));
        //BuildAndCopyAssetBundles(rootFolder, buildFolder);
    }
}
#endif