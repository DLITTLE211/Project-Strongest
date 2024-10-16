using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

/*
 * This class will re-create the config.json file. Yes, "Configurator" is a word.
 * 
 * This is what the config.json file looks like for reference:
 *
 * {
 *    "displayName": "{Application.productName}",
 *    "executable": "{Application.productName}.exe",
 *    "configurations":
 *     {
 *         "mobile2x3":[""],
 *         "verticalScreen":["-popupwindow -screen-height 1920 -screen-width 1080"]
 *     }
 *     "juristiction":"MN"
 * }
 * 
 */

public static class JsonConfigurator
{
    public const string configJson = "config.json";

    public static string GetConfigRootPath()
    {
        var rootFolder = $"{Application.dataPath}/../";
        return Path.Combine(rootFolder, configJson);
    }


    public static void CreateJsonConfig(string juristiction = "ND", bool hasVerticalScreen = true, string verticalScreenParameters = "", bool hasMobile2x3 = false, string hasMobile2x3Parameters = "", bool is32BitBuild = false, string gameType = "ETAB")
    {
        string configRootPath = GetConfigRootPath();

        /* Delete the current config file if it exists. */
        if (File.Exists(configRootPath))
        {
            FileUtil.DeleteFileOrDirectory(configRootPath);
        }

        string displayName = Application.productName;

        /* 
         * Auto-generate config JSON file
         *
         * Here I am using String Builder so that people do not have to install the Newtonsoft JSON package.
         * 
         */
        if (!File.Exists(configRootPath))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"displayName\": \"");
            sb.Append(displayName);
            sb.Append("\",");
            sb.Append("\"executable\": \"");
            sb.Append(displayName);
            sb.Append(".exe\",");
            sb.Append("\"configurations\": {");

            if (hasVerticalScreen)
            {
                sb.Append("\"verticalScreen\": [\"");
                sb.Append(verticalScreenParameters);
                sb.Append("\"]");
            }
            if (hasVerticalScreen && hasMobile2x3)
            {
                sb.Append(",");
            }
            if (hasMobile2x3)
            {
                sb.Append("\"mobile2x3\": [\"");
                sb.Append(hasMobile2x3Parameters);
                sb.Append("\"]");
            }
            sb.Append("},");

            if (is32BitBuild)
            {
                sb.Append("\"is32BitBuild\": " + is32BitBuild.ToString().ToLower() + ",");
            }

            sb.Append("\"juristiction\": \"");
            sb.Append(juristiction);
            sb.Append("\", ");
            sb.Append($"\"gameType\": \"{gameType}\"");
            sb.Append("}");

            File.WriteAllText(configRootPath, sb.ToString());
        }
    }

    /* We are using both a string JSON file in these methods because the exact layout of the JSON object depends on which screen configurations are used. */
    public static bool JsonConfigHasVerticalScreen()
    {
        string jsonString = File.ReadAllText(GetConfigRootPath());
        if (jsonString.IndexOf("verticalScreen") == -1)
        {
            return false;
        }

        return true;
    }

    public static bool JsonConfigHasMobile2x3()
    {
        string jsonString = File.ReadAllText(GetConfigRootPath());
        if (jsonString.IndexOf("mobile2x3") == -1)
        {
            return false;
        }

        return true;
    }

    public static bool Is32BitBuild()
    {
        string jsonString = File.ReadAllText(GetConfigRootPath());
        if (jsonString.IndexOf("is32BitBuild") == -1)
        {
            return false;
        }

        JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(jsonString);

        return jsonConfig.is32BitBuild;
    }
}
