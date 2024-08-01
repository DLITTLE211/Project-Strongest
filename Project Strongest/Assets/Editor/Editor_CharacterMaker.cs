using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Editor_CharacterMaker : EditorWindow
{
    #region Visual Related code for Window
    Texture2D _headerSection, 
              _mainSection, 
              _completedBoolSection,
              _createCharacterButtonSection;

    Color32 headerColor = new Color32((byte)15f,(byte)15f, (byte)15f, (byte)255f);
    Color32 mainSectionColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
    Color32 boolSectionColor = new Color32((byte)35f, (byte)35f, (byte)35f, (byte)255f);
    Color32 createCharacterColor = new Color32((byte)20f, (byte)20f, (byte)20f, (byte)255f);

    Rect _headerSectionRect, 
         _mainSectionRect,
         _completedBoolRect,
         _createCharacterRect;
    #endregion

    #region Boolean Criteria
    bool filledName = false;
    bool filledID = false;
    bool filledImage = false;
    bool filledMass = false;
    bool filledMoveListCount = false;
    bool filledHeight = false;
    bool filledWeight = false;
    bool filledMoveVelocity = false;
    bool filledJumpForce = false;
    bool filledInAirMoveForce = false;
    bool filledMaxHealth = false;
    bool filledMaxStun = false;
    bool filledDefenseValue = false;
    bool filledRegenRate = false;
    #endregion

    string createdCharacterFilePath = "";

    Character_Profile _newProfile;
    public Character_Profile CurrentProfile { get { return _newProfile; } }

    [MenuItem("Window/Roster/Characters/Create New Character")]
    static void CreateNewCharacter()
    {
        Editor_CharacterMaker window = (Editor_CharacterMaker)GetWindow(typeof(Editor_CharacterMaker));
        window.minSize = new Vector2(800f, 450f);
        window.maxSize = new Vector2(1600f, 900f);
        window.Show();
    }

    private void OnEnable()
    {
        createdCharacterFilePath = $"{Application.dataPath}/CharacterList";


        Debug.Log("Data Path: " + createdCharacterFilePath);
        InitTextures();
        InitData();
    }
    void InitTextures() 
    {
        _headerSection = new Texture2D(1, 1);
        _headerSection.SetPixel(0, 0, headerColor);
        _headerSection.Apply();


        _mainSection = new Texture2D(1, 1);
        _mainSection.SetPixel(0, 0, mainSectionColor);
        _mainSection.Apply();


        _completedBoolSection = new Texture2D(1, 1);
        _completedBoolSection.SetPixel(0, 0, boolSectionColor);
        _completedBoolSection.Apply();


        _createCharacterButtonSection = new Texture2D(1, 1);
        _createCharacterButtonSection.SetPixel(0, 0, createCharacterColor);
        _createCharacterButtonSection.Apply();
    }
    void InitData() 
    {
        _newProfile = (Character_Profile)ScriptableObject.CreateInstance(typeof(Character_Profile));
    }
    private void OnGUI()
    {
        CheckAllBooleans();
        DrawLayouts();
        DrawHeader();
        DrawMainCharacterSettings();
        DrawCompletedBool();
        DrawCreateCharacter();
    }


    #region Drawing Functions
    void DrawLayouts() 
    {
        _headerSectionRect.x = 0f;
        _headerSectionRect.y = 0f;
        _headerSectionRect.width = Screen.width;
        _headerSectionRect.height = 35f;
        GUI.DrawTexture(_headerSectionRect,_headerSection);


        _mainSectionRect.x = 0f;
        _mainSectionRect.y = _headerSectionRect.height + 2.5f;
        _mainSectionRect.width = Screen.width - (Screen.width /2.75f);
        _mainSectionRect.height = Screen.height - 135f;
        GUI.DrawTexture(_mainSectionRect, _mainSection);


        _completedBoolRect.x = 0f + _mainSectionRect.width;
        _completedBoolRect.y = _headerSectionRect.height + 2.5f;
        _completedBoolRect.width = Screen.width - _mainSectionRect.width;
        _completedBoolRect.height = Screen.height - 135f;
        GUI.DrawTexture(_completedBoolRect, _completedBoolSection);

        _createCharacterRect.width = Screen.width;
        _createCharacterRect.height = 75f;
        _createCharacterRect.x = 0;
        _createCharacterRect.y = (Screen.height - 20f) - _createCharacterRect.height;
        GUI.DrawTexture(_createCharacterRect, _createCharacterButtonSection);

    }
    void DrawHeader() 
    {
        GUILayout.BeginArea(_headerSectionRect);
        #region FillArea
        GUILayout.Label("Character Creator");
        #endregion
        GUILayout.EndArea();
    }
    void DrawMainCharacterSettings() 
    {
        GUILayout.BeginArea(_mainSectionRect);

        #region Character Information
        GUILayout.Label("Character Data");

        #region Identification Info
        GUILayout.Label("Character Identification Info");
        EditorGUILayout.BeginHorizontal();
        _newProfile.CharacterName = 
            (string)EditorGUILayout.TextField("Character Name:", _newProfile.CharacterName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.CharacterID = 
            (int)EditorGUILayout.IntField("Character ID:", _newProfile.CharacterID);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.CharacterProfileImage = 
            (Sprite)EditorGUILayout.ObjectField("Character Sprite:",_newProfile.CharacterProfileImage,typeof(Sprite),false);
        EditorGUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(5);

        #region Sizing Info
        GUILayout.Label("Character Sizing Info");
        EditorGUILayout.BeginHorizontal();
        _newProfile.Mass = (int)EditorGUILayout.IntField("Character Mass:",_newProfile.Mass);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.Height = (float)EditorGUILayout.FloatField("Character Height:", _newProfile.Height);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.Weight = (float)EditorGUILayout.FloatField("Character Weight:",_newProfile.Weight);
        EditorGUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(25);

        #region Movement Info
        GUILayout.Label("Character Movement Info");
        EditorGUILayout.BeginHorizontal();
        _newProfile.MoveVelocity = (float)EditorGUILayout.Slider("Move Speed:", _newProfile.MoveVelocity,1f,5f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.JumpForce = (float)EditorGUILayout.FloatField("Jump Height:", _newProfile.JumpForce);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.InAirMoveForce = (float)EditorGUILayout.FloatField("Air Move Speed:", _newProfile.InAirMoveForce);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.moveListCount = (int)EditorGUILayout.IntField("Total MoveList(s):", _newProfile.moveListCount);
        EditorGUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(25);


        #region Health/Defense Info
        GUILayout.Label("Character Health/Defense Info");
        EditorGUILayout.BeginHorizontal();
        _newProfile.MaxHealth = (float)EditorGUILayout.Slider("Max Health:", _newProfile.MaxHealth, 0f, 250f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.MaxStunValue = (float)EditorGUILayout.Slider("Max Stun:", _newProfile.MaxStunValue, 0f, 80f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.DefenseValue = (float)EditorGUILayout.Slider("Total Defense:", _newProfile.DefenseValue, 0f, 150f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _newProfile.HealthRegenRate = (float)EditorGUILayout.Slider("Health Regeneration Rate:", _newProfile.HealthRegenRate, 0f, 8f);
        EditorGUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(25);

        #endregion

        GUILayout.EndArea();
    }
    void DrawCompletedBool()
    {
        GUILayout.BeginArea(_completedBoolRect);

        #region FillArea
        GUILayout.Label("Character Creation Checklist");
        filledName = EditorGUILayout.Toggle("Name Filled", filledName);
        filledID = EditorGUILayout.Toggle("ID Filled", filledID);
        filledImage = EditorGUILayout.Toggle("Sprite Filled", filledImage);

        GUILayout.Space(25);
        filledMass = EditorGUILayout.Toggle("Mass Filled", filledMass);
        filledHeight = EditorGUILayout.Toggle("Height Filled", filledHeight);
        filledWeight = EditorGUILayout.Toggle("Weight Filled", filledWeight);

        GUILayout.Space(25);
        filledMoveVelocity = EditorGUILayout.Toggle("Move Speed Filled", filledMoveVelocity);
        filledJumpForce = EditorGUILayout.Toggle("Jump Strength Filled", filledJumpForce);
        filledInAirMoveForce = EditorGUILayout.Toggle("In Air Move Force Filled", filledInAirMoveForce);
        filledMoveListCount = EditorGUILayout.Toggle("MoveList Count Filled", filledMoveListCount);

        GUILayout.Space(25);
        filledMaxHealth = EditorGUILayout.Toggle("Health Filled", filledMaxHealth);
        filledMaxStun = EditorGUILayout.Toggle("Stun Filled", filledMaxStun);
        filledDefenseValue = EditorGUILayout.Toggle("Defense Filled", filledDefenseValue);
        filledRegenRate = EditorGUILayout.Toggle("Regen-Rate Filled", filledRegenRate);
        GUILayout.Space(25);
        #endregion

        GUILayout.EndArea();
    }
    void DrawCreateCharacter()
    {
        GUILayout.BeginArea(_createCharacterRect);

        #region FillArea

        GUILayout.Label("Confirm Character");
        if (GUILayout.Button("Create New Character", GUILayout.Width(155), GUILayout.Height(30)))
        {
            if (CheckIfFieldsFilled())
            {
                CreateCharacter();
            }
            else 
            {
                Debug.LogError("Not All Fields have been filled. Fill in all new character data!");
            }
            
        }
        #endregion

        GUILayout.EndArea();
    }
    void CheckAllBooleans() 
    {
        try
        {
            filledName = _newProfile.CharacterName.Length > 5 ? true: false;
            filledID = _newProfile.CharacterID != 0 ? true : false;
            filledImage = _newProfile.CharacterProfileImage != null ? true : false;
            filledMass = _newProfile.Mass >= 50 ? true : false;
            filledHeight = _newProfile.Height > 45f ? true : false;
            filledWeight = _newProfile.Weight >= 100f ? true : false;
            filledMoveVelocity = _newProfile.MoveVelocity >= 1f ? true : false;
            filledJumpForce = _newProfile.JumpForce >= 30f ? true : false;
            filledInAirMoveForce = _newProfile.InAirMoveForce >= 3f ? true : false;
            filledMaxHealth = _newProfile.MaxHealth >= 100f ? true : false;
            filledMaxStun = _newProfile.MaxStunValue >= 30f ? true : false;
            filledDefenseValue = _newProfile.DefenseValue >= 25f ? true : false;
            filledRegenRate = _newProfile.HealthRegenRate >= 2 ? true : false;
            filledMoveListCount = _newProfile.moveListCount > 0 ? true : false;
        }
        catch (NullReferenceException)
        {
            filledName = false;
            filledID = false;
            filledImage = false;
            filledMass = false;
            filledHeight = false;
            filledWeight = false;
            filledMoveVelocity = false;
            filledJumpForce = false;
            filledInAirMoveForce = false;
            filledMaxHealth = false;
            filledMaxStun = false;
            filledDefenseValue = false;
            filledRegenRate = false;
            filledMoveListCount = false;
        }
    }
    bool CheckIfFieldsFilled()
    {
        bool FullCheckList = filledName && filledID && filledImage && filledMass && filledHeight
            && filledWeight && filledMoveVelocity && filledJumpForce && filledInAirMoveForce && filledMaxHealth
            && filledMaxStun && filledDefenseValue && filledRegenRate && filledMoveListCount;
        if (!FullCheckList) 
        {
            return false;
        }
        return true;
    }

    void CreateCharacter() 
    {
        //Creates New Folder in Path of CharacterList
        string newChararacterFolderPath = "Assets/BAKI_STRONGEST_SHOWDOWN_ROSTER/CharacterList/";
        Directory.CreateDirectory(newChararacterFolderPath + $"{_newProfile.CharacterName}_CharacterFolder");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string newCharacterPath = newChararacterFolderPath + $"{_newProfile.CharacterName}_CharacterFolder/";
        string SO_RefOnePath = "Assets/BAKI_STRONGEST_SHOWDOWN_ROSTER/CharacterList/Character_Resources/";
        string SO_RefTwoPath = "Assets/Prefabs/Template_MobilityOptions.asset";
        string prefab_RefPath = "Assets/Prefabs/Template_Movelist.prefab";

        /*Inside Newly Created CharacterProfile:
         * 1. Creates Scriptable object off of adjusted on in character creation window
         */

        try
        {
            #region Character_Profile Scriptable Object Creation
            string newSOPath = "";
            SO_RefOnePath += $"{CurrentProfile.CharacterName}_Profile.asset";
            AssetDatabase.CreateAsset(CurrentProfile, SO_RefOnePath);
            newSOPath = AssetDatabase.GetAssetPath(CurrentProfile);
            if (AssetDatabase.CopyAsset(newSOPath, newCharacterPath + $"{CurrentProfile.CharacterName}_Profile.asset"))
            {
                Debug.Log("Character Profile Path Success");
                AssetDatabase.DeleteAsset(SO_RefOnePath);
            }
            else
            {
                Debug.Log("Character Profile Path Failed");
            }
            AssetDatabase.SaveAssets(); 
            AssetDatabase.Refresh();
            #endregion

            #region Character MoveList Creation

            for (int i = 0; i < CurrentProfile.moveListCount; i++)
            {
                if (AssetDatabase.CopyAsset(prefab_RefPath, newCharacterPath + $"{CurrentProfile.CharacterName}_MoveList{i+1}.prefab"))
                {
                    Debug.Log("Character Movelist Path Success");
                }
                else
                {
                    Debug.Log("Character Movelist Path Failed");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            #endregion

            #region Character Mobility Creation
            if (AssetDatabase.CopyAsset(SO_RefTwoPath, newCharacterPath + $"{CurrentProfile.CharacterName}_MobilityOptions.asset"))
            {
                Debug.Log("Character Movelist Path Success");
                Debug.Log("All Criteria Met. Creating New Character Data!");
                Debug.Log($"{_newProfile.CharacterName}, has been Created!");
                this.Close();
            }
            else
            {
                Debug.Log("Character Movelist Path Failed");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endregion

        }
        catch (Exception)
        {
            Debug.LogError("Failed to Create new Character. Exiting...");
            this.Close();
        }

    }
    #endregion
}
