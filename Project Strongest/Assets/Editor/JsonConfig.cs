[System.Serializable]
public class JsonConfig
{
    public string displayName;
    public string executable;
    public ScreenConfigurations configurations;
    public string jurisdiction;
    public bool is32BitBuild;
    public string gameType;
}

[System.Serializable]
public class ScreenConfigurations
{
    public string[] verticalScreen;
    public string[] mobile2x3;
}

