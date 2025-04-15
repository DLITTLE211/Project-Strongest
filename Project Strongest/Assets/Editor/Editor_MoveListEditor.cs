using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor.Animations;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;

public class Editor_MoveListEditor : EditorWindow
{
    #region Visual Code For Windows
    enum MoveListEditMode {MainAttackInfoMode, MainAttackCollisionMode }
    MoveListEditMode _moveListEditState;
    public EditorMoveListObject MoveListHeaderObject;
    public EditorMoveListObject MoveListBodyObject;
    public EditorMoveListObject CurrentAttackHeaderObject;
    public EditorMoveListObject CurrentAttackBodyObject;
    public EditorMoveListObject CurrentAttackInformationObject;
    public EditorMoveListObject SaveDataObject;
    #endregion

    #region Character Data
    private Character_MoveList moveListData;
    private Character_MoveList lastMoveList;
    FullMoveList editedMoveList;
    private GameObject characterModel;
    private RuntimeAnimatorController characterAnimator;
    #endregion

    #region CompositeAttackData
    CompositeAttackData _attackData;
    Attack_BaseProperties currentCenterAttackData;
    private int highlightedAttackIndex = -1;
    bool displayCurrentAttackList = false;
    #endregion

    #region Menu Display Bools

    private int highlightedMoveIndex = -1;
    bool displaySuperAttacks = false;
    bool displayCommandGrabs = false;
    bool displayCounterAttacks = false;
    bool displayStanceAttacks = false;
    bool displayRekkaAttacks = false;
    bool displaySpecialAttacks = false;
    bool displayStringAttacks = false;
    int stringNormalCount;
    int rekkaAttackCount;
    bool displayCommandNormalAttacks = false;
    bool displayNormalAttacks = false;
    bool displayThrowAttacks = false;

    bool displayPrimaryAttackData = false;
    bool displayMainPropertyData = false;
    #endregion

    #region Animation Timeline Data
    private AnimatorOverrideController overrideController;
    private AnimationClip lastClip;
    private AnimatorController tempController;
    private AnimationClip _currentAnimClip;
    private float timelineCurrentTime = 0f;
    private int fps = 60;
    private int currentFrame;
    float init;
    float startup;
    float active;
    float inactive;
    float recoveryAmount;
    private bool isPlaying = false;
    private double lastTime;
    private bool loopPreview = false;
    [SerializeField] private float playbackSpeed = 1f;
    private Color moveEventColor = Color.cyan;
    private Color collisionBoxColor = new Color(1f, 0.5f, 0f, 0.6f); // semi-transparent orange
    private Color collisionBoxOutlineColor = Color.yellow; // fallback color
    private float markerWidth = 4f;
    int lastFrame;
    bool previewActive;
    #endregion

    #region Preview Window Code
    private PreviewRenderUtility previewUtility;
    public GameObject previewInstance;
    private Rect previewRect;
    private float previewHeight = 250f;
    bool showPreview;

    // Camera & Lighting settings
    private Vector3 previewCameraPositionOffset = new Vector3(0, 0, -10);
    private Vector3 previewCameraRotationEuler = Vector3.zero;
    private float previewCameraFOV = 30f;
    private float previewCameraNearClippingPlane = 0.1f;
    private float previewCameraFarClippingPlane = 1000f;
    private Color backgroundPreviewColor = Color.black;
    private bool showLightingSettings = true;
    private float light0Intensity;
    private float light1Intensity;

    // Panning/Zoom
    private Vector2 previewPan = Vector2.zero;
    private float previewZoom = 1f;

    // Panel widths
    private float leftPanelWidth = 200f;
    private float rightPanelWidth = 300f;
    #endregion


    [MenuItem("Window/Roster/Characters/Edit Character Movelist")]
    static void OpenMoveListEditorWindow() 
    {
        Editor_MoveListEditor window = (Editor_MoveListEditor)GetWindow(typeof(Editor_MoveListEditor));
        window.minSize = new Vector2(1600f, 1000f);
        window.maxSize = new Vector2(2400f, 1200f);
    }
    void OnEnable()
    {
        editedMoveList = null;
        lastMoveList = null;
        previewActive = false;
        ClearPreview();
        currentFrame = 0;
        lastFrame = currentFrame;
        light0Intensity = 0.35f;
        light1Intensity = 0.55f;
        previewUtility = new PreviewRenderUtility();
        previewUtility.camera.cullingMask = LayerMask.GetMask("Default", "Outlined Objects", "UI", "Player1", "Player2");
        previewUtility.cameraFieldOfView = previewCameraFOV;
        previewUtility.lights[0].intensity = light0Intensity;
        previewUtility.lights[0].color = Color.white;
        previewUtility.lights[0].transform.rotation = Quaternion.Euler(50, 50, 0);
        previewUtility.lights[1].intensity = light1Intensity;
        previewUtility.lights[1].color = Color.white;
        previewUtility.ambientColor = Color.gray;
        _attackData = null;
        characterModel = null;
        characterAnimator = null;
        currentCenterAttackData = null;
        displayCurrentAttackList = false;
    }
    private void OnDisable()
    {
        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }

        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
        if (tempController != null)
        {
            DestroyImmediate(tempController);
            tempController = null;
        }
        if (overrideController != null)
        {
            DestroyImmediate(overrideController);
            overrideController = null;
        }
        editedMoveList = null;
        previewActive = false;
        ClearPreview();
    }
    void OnGUI()
    {
        DrawLayouts();

    }
    void DrawLayouts()
    {
        SetScreenSize();
        DrawMoveListHeader();
        DrawMoveListBody(); 
        DrawCurrentAttackHeader();
        DrawCurrentAttackBody();
        DrawCurrentAttackInfo();
        DrawSaveChangesHeader();
    }
    void SetScreenSize() 
    {
        #region MoveList Header
        Color32 headerColor1 = new Color32((byte)40f, (byte)40f, (byte)40f, (byte)255f);
        Rect size1 = new Rect();
        size1.x = 0f;
        size1.y = 0f;
        size1.width = Screen.width /4f;
        size1.height = 30f;
        MoveListHeaderObject = new EditorMoveListObject(new Texture2D(1, 1), headerColor1, size1);
        MoveListHeaderObject.editorTexture.SetPixel(0, 0, headerColor1);
        MoveListHeaderObject.editorTexture.Apply();
        GUI.DrawTexture(MoveListHeaderObject.editorRect, MoveListHeaderObject.editorTexture);
        #endregion

        #region MoveList Body
        Color32 mBodyColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
        Rect size2 = new Rect();
        size2.x = 0f;
        size2.y = size1.height+10f;
        size2.width = size1.width;
        size2.height = Screen.height - (Screen.height / 10f);
        MoveListBodyObject = new EditorMoveListObject(new Texture2D(1, 1), mBodyColor, size2);
        MoveListBodyObject.editorTexture.SetPixel(0, 0, mBodyColor);
        MoveListBodyObject.editorTexture.Apply();
        GUI.DrawTexture(MoveListBodyObject.editorRect, MoveListBodyObject.editorTexture);
        #endregion

        #region CA Header
        Color32 cAHColor = new Color32((byte)75f, (byte)75f, (byte)75f, (byte)255f);
        Rect size3 = new Rect();
        size3.x = size1.width;
        size3.y = 0f;
        size3.width = (Screen.width / 1.95f);
        size3.height = 30f;
        CurrentAttackHeaderObject = new EditorMoveListObject(new Texture2D(1, 1), cAHColor, size3);
        CurrentAttackHeaderObject.editorTexture.SetPixel(0, 0, cAHColor);
        CurrentAttackHeaderObject.editorTexture.Apply();
        GUI.DrawTexture(CurrentAttackHeaderObject.editorRect, CurrentAttackHeaderObject.editorTexture);
        #endregion

        #region CA Body 
        Color32 cABColor = new Color32((byte)85f, (byte)85f, (byte)85f, (byte)255f);
        Rect size4 = new Rect();
        size4.x = size1.width;
        size4.y = size1.height + 10f;
        size4.width = (Screen.width / 1.95f);
        size4.height = Screen.height - (Screen.height / 10f);
        CurrentAttackBodyObject = new EditorMoveListObject(new Texture2D(1, 1), cABColor, size4);
        CurrentAttackBodyObject.editorTexture.SetPixel(0, 0, cABColor);
        CurrentAttackBodyObject.editorTexture.Apply();
        GUI.DrawTexture(CurrentAttackBodyObject.editorRect, CurrentAttackBodyObject.editorTexture);
        #endregion

        #region CA Info 
        Color32 cAInfoColor = new Color32((byte)45f, (byte)45f, (byte)45f, (byte)255f);
        Rect size5 = new Rect();
        size5.x = size4.x*3;
        size5.y = 0f;
        size5.width = size1.width;
        size5.height = Screen.height;
        CurrentAttackInformationObject = new EditorMoveListObject(new Texture2D(1, 1), cAInfoColor, size5);
        CurrentAttackInformationObject.editorTexture.SetPixel(0, 0, cAInfoColor);
        CurrentAttackInformationObject.editorTexture.Apply();
        GUI.DrawTexture(CurrentAttackInformationObject.editorRect, CurrentAttackInformationObject.editorTexture);
        #endregion

        #region Save Data
        Color32 toggleDataColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
        Rect size6 = new Rect();
        size6.x = 0f;
        size6.y = Screen.height/1.08f;
        size6.width = Screen.width / 4f;
        size6.height = Screen.height;
        SaveDataObject = new EditorMoveListObject(new Texture2D(1, 1), toggleDataColor, size6);
        SaveDataObject.editorTexture.SetPixel(0, 0, toggleDataColor);
        SaveDataObject.editorTexture.Apply();
        GUI.DrawTexture(SaveDataObject.editorRect, SaveDataObject.editorTexture);
        #endregion

    }
    void DrawMoveListHeader() 
    {
        GUILayout.BeginArea(MoveListHeaderObject.editorRect);
        #region FillArea
        GUILayout.Label("Character MoveList Editor");
        #endregion
        GUILayout.EndArea();
    }
    void DrawMoveListBody()
    {
        GUILayout.BeginArea(MoveListBodyObject.editorRect);
        #region FillArea
        GUILayout.Label("Full MoveList");
        moveListData = (Character_MoveList)EditorGUILayout.ObjectField(new GUIContent("Current Movelist", "Insert Movelist"), moveListData, typeof(Character_MoveList), true);
        if (moveListData != null)
        {
            FillDataOnScreen();
        }
        else 
        {
            currentCenterAttackData = null;
        }
        #endregion
        GUILayout.EndArea();
    }

    #region Left Page Information
    void FillDataOnScreen() 
    {
        GetMoveListData();
        GUILayout.Space(10);
        ShowSupersAttacks();
        GUILayout.Space(10);
        ShowCommandGrabAttacks();
        GUILayout.Space(10);
        ShowCounterAttacks();
        GUILayout.Space(10);
        ShowStanceAttacks();
        GUILayout.Space(10);
        ShowRekkaAttacks();
        GUILayout.Space(10);
        ShowSpecialAttacks();
        GUILayout.Space(10);
        ShowStringNormalAttacks();
        GUILayout.Space(10);
        ShowCommandNormalAttacks();
        GUILayout.Space(10);
        ShowNormalAttacks();
        GUILayout.Space(10);
        ShowThrowAttacks();


        GUILayout.Space(50);

        Debug.Log("Present ALL Attacks");
    }

    #region Supers Function Section
    void ShowSupersAttacks()
    {
        #region Command Grabs Display
        Debug.Log(editedMoveList.BasicSuperAttacks.Count);
        GUILayout.Label("Super Attacks");
        if (GUILayout.Button("Add New Supers Entry"))
        {
            AddBasicSuperAttacksEntry();
        }
        displaySuperAttacks = EditorGUILayout.Foldout(displaySuperAttacks, "Show Supers");
        if (displaySuperAttacks)
        {
            for (int i = 0; i < editedMoveList.BasicSuperAttacks.Count; i++)
            {
                var move = editedMoveList.BasicSuperAttacks[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.specialMoveName, style))
                {
                    DisplayBasicSuperAttacksMoveData(move);
                }
            }
        }
        #endregion
    }
    void AddBasicSuperAttacksEntry()
    {

    }
    void DisplayBasicSuperAttacksMoveData(Attack_AdvancedSpecialMove specialAttack)
    {

    }
    #endregion

    #region Command Grabs Function Section
    void ShowCommandGrabAttacks()
    {
        #region Command Grabs Display
        Debug.Log(editedMoveList.CommandThrows.Count);
        GUILayout.Label("Command Grabs");
        if (GUILayout.Button("Add New Command Grab Entry"))
        {
            AddCommandGrabEntry();
        }
        displayCommandGrabs = EditorGUILayout.Foldout(displayCommandGrabs, "Show Command Grabs");
        if (displayCommandGrabs)
        {
            for (int i = 0; i < editedMoveList.CommandThrows.Count; i++)
            {
                var move = editedMoveList.CommandThrows[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.specialMoveName, style))
                {
                    DisplayCommandGrabMoveData(move);
                }
            }
        }
        #endregion
    }
    void AddCommandGrabEntry()
    {

    }
    void DisplayCommandGrabMoveData(Attack_AdvancedSpecialMove specialAttack)
    {

    }
    #endregion

    #region Counter Attack Function Section
    void ShowCounterAttacks()
    {
        #region Counter Attack Display
        Debug.Log(editedMoveList.CounterAttacks.Count);
        GUILayout.Label("Counter Attacks");
        if (GUILayout.Button("Add New Counter Attack Entry"))
        {
            AddCounterAttackEntry();
        }
        displayCounterAttacks = EditorGUILayout.Foldout(displayCounterAttacks, "Show Counter Attacks");
        if (displayCounterAttacks)
        {
            for (int i = 0; i < editedMoveList.CounterAttacks.Count; i++)
            {
                var move = editedMoveList.CounterAttacks[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.specialMoveName, style))
                {
                    DisplayCounterAttackMoveData(move);
                }
            }
        }
        #endregion
    }
    void AddCounterAttackEntry()
    {

    }
    void DisplayCounterAttackMoveData(Attack_AdvancedSpecialMove specialAttack)
    {

    }
    #endregion

    #region Stance Function Section
    void ShowStanceAttacks()
    {
        #region Stance Attack Display
        Debug.Log(editedMoveList.stanceSpecials.Count);
        GUILayout.Label("Stance Attacks");
        if (GUILayout.Button("Add New Stance Entry"))
        {
            AddStanceAttackEntry();
        }
        displayStanceAttacks = EditorGUILayout.Foldout(displayStanceAttacks, "Show Stance Attacks");
        if (displayStanceAttacks)
        {
            for (int i = 0; i < editedMoveList.stanceSpecials.Count; i++)
            {
                var move = editedMoveList.stanceSpecials[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.StanceSpecialAttack_Name, style))
                {
                    DisplayStanceMoveData(move);
                }
            }
        }
        #endregion
    }
    void AddStanceAttackEntry()
    {

    }
    void DisplayStanceMoveData(Attack_StanceSpecialMove specialAttack)
    {

    }
    #endregion

    #region Rekka Function Section
    void ShowRekkaAttacks()
    {
        #region String Attack Display
        Debug.Log(editedMoveList.rekkaSpecials.Count);
        GUILayout.Label("Rekka Attacks");
        rekkaAttackCount = EditorGUILayout.IntField("Set Rekka Count", rekkaAttackCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
        if (GUILayout.Button("Add New Rekka Move Entry"))
        {
            AddRekkaAttackEntry();
        }
        displayRekkaAttacks = EditorGUILayout.Foldout(displayRekkaAttacks, "Show Rekka Attacks");
        if (displayRekkaAttacks)
        {
            for (int i = 0; i < editedMoveList.rekkaSpecials.Count; i++)
            {
                var move = editedMoveList.rekkaSpecials[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.RekkaSpecialAttack_Name, style))
                {
                    DisplayRekkaMoveData(move, move.RekkaSpecialAttack_Name,i);
                }
            }
            if (GUILayout.Button("Clear Rekka Attack Data"))
            {
                if (editedMoveList.rekkaSpecials.Count > 0)
                {
                    editedMoveList.rekkaSpecials.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddRekkaAttackEntry()
    {
        Attack_RekkaSpecialMove newRekkaMove = new Attack_RekkaSpecialMove();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        RekkaInput newRekkaInput = new RekkaInput();
        newRekkaMove.RekkaSpecialAttack_Name = $"New Rekka Attack Entry";
        newProperty._attackName = $"Rekka Main Attack Entry";
        propertyList.Add(newProperty);
        newRekkaMove.rekkaInput = new RekkaInput();
        newRekkaInput.mainAttackInput = new List<Attack_Input>();
        for (int i = 0; i < 3; i++)
        {
            newRekkaInput.mainAttackInput.Add(new Attack_Input("", ("").ToCharArray()));
        }
        newRekkaInput.mainAttackProperty = newProperty;
        newRekkaInput._rekkaProperties = new List<Attack_BaseProperties>();
        newRekkaInput._rekkaPortion = new List<RekkaAttack>();
        for (int i = 0; i < rekkaAttackCount; i++)
        {
            Attack_BaseProperties newRekkaProperty = new Attack_BaseProperties();
            newRekkaProperty._attackName = $"New Rekka Sub Entry{i+1}";
            Attack_BaseInput newBaseInput = new Attack_BaseInput();
            RekkaAttack newRekkaAttack = new RekkaAttack();
            newRekkaAttack.individualRekkaAttack = new Attack_BasicInput();
            newRekkaAttack.individualRekkaAttack._correctInput = new List<Attack_BaseInput>();

            newBaseInput.property = newRekkaProperty;
            newRekkaAttack.individualRekkaAttack._correctInput.Add(newBaseInput);
            newRekkaInput._rekkaPortion.Add(newRekkaAttack);
            propertyList.Add(newRekkaProperty);
            newRekkaInput._rekkaProperties.Add(newRekkaProperty);
        }
        newRekkaMove.rekkaInput = newRekkaInput;

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, null, newRekkaMove, null, null, null, null, propertyList.Count, newRekkaMove.RekkaSpecialAttack_Name);
        editedMoveList.rekkaSpecials.Insert(0, newRekkaMove);
        _attackData = newCompositeAttackData;
    }
    void DisplayRekkaMoveData(Attack_RekkaSpecialMove specialAttack,string specialName, int propertyIndex)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        propertyList.Add(specialAttack.rekkaInput.mainAttackProperty);
        for (int i = 0; i < specialAttack.rekkaInput._rekkaPortion.Count; i++) 
        {
            Attack_BaseProperties currentProperty = specialAttack.rekkaInput._rekkaPortion[i].individualRekkaAttack._correctInput[0].property;
            propertyList.Add(currentProperty);
        }
        Attack_RekkaSpecialMove newRekkaData = specialAttack;
        _attackData = new CompositeAttackData(propertyList, null, newRekkaData,null , null, null, null, propertyList.Count, specialName);

    }
    #endregion

    #region Special Move Function Section
    void ShowSpecialAttacks()
    {
        #region String Attack Display
        Debug.Log(editedMoveList.special_Simple.Count);
        GUILayout.Label("Special Move Attacks");
        if (GUILayout.Button("Add New Special Move Entry"))
        {
            AddSpecialAttackEntry();
        }
        displaySpecialAttacks = EditorGUILayout.Foldout(displaySpecialAttacks, "Show Special Move Attacks");
        if (displaySpecialAttacks)
        {
            for (int i = 0; i < editedMoveList.special_Simple.Count; i++)
            {
                var move = editedMoveList.special_Simple[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.BasicSpecialAttack_Name, style))
                {
                    DisplaySpecialMoveData(move,move.BasicSpecialAttack_Name);
                }
            }
            if (GUILayout.Button("Clear Special Attack Data"))
            {
                if (editedMoveList.special_Simple.Count > 0)
                {
                    editedMoveList.special_Simple.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddSpecialAttackEntry()
    {
        Attack_BasicSpecialMove newSpecialMove = new Attack_BasicSpecialMove();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();


        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        newProperty._attackName = $"Special Attack Entry";
        propertyList.Add(newProperty);
        newSpecialMove.property = newProperty;
        newSpecialMove.attackInput = new List<Attack_Input>();
        for (int i = 0; i < 3; i++)
        {
            newSpecialMove.attackInput.Add(new Attack_Input("", ("").ToCharArray()));
        }
        newSpecialMove.BasicSpecialAttack_Name = "New Special Attack";
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, null, null, null, newSpecialMove,null , null, propertyList.Count, newSpecialMove.BasicSpecialAttack_Name);
        editedMoveList.special_Simple.Insert(0, newSpecialMove);
        _attackData = newCompositeAttackData;
    }
    void DisplaySpecialMoveData(Attack_BasicSpecialMove specialAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { specialAttack.property};
        Attack_BasicSpecialMove newSpecialMoveData =  specialAttack;
        _attackData = new CompositeAttackData(propertyList, null, null, null, newSpecialMoveData,null , null, 1, attackName);
    }
    #endregion

    #region String Attack Function Section
    void ShowStringNormalAttacks()
    {
        #region String Attack Display
        Debug.Log(editedMoveList.stringNormalAttacks.Count);
        GUILayout.Label("String Attacks");
        stringNormalCount = EditorGUILayout.IntField("Set Attack Strings Count", stringNormalCount, GUILayout.Width(MoveListBodyObject.editorRect.width/2f), GUILayout.Height(20));
        if (GUILayout.Button("Add New String Normal Attack Entry"))
        {
            AddStringNormalAttackEntry();
        }
        displayStringAttacks = EditorGUILayout.Foldout(displayStringAttacks, "Show String Normals Attacks");
        if (displayStringAttacks)
        {
            for (int i = 0; i < editedMoveList.stringNormalAttacks.Count; i++)
            {
                var move = editedMoveList.stringNormalAttacks[i];
                if (move == null) continue;
                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.SpecialAttackName, style))
                {
                    DisplayStringNormalData(move, move.SpecialAttackName);
                }
            }
            if (GUILayout.Button("Clear String Attack Data"))
            {
                if (editedMoveList.stringNormalAttacks.Count > 0)
                {
                    editedMoveList.stringNormalAttacks.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddStringNormalAttackEntry()
    {
        List<Attack_NonSpecialAttack> newStringAttackData = new List<Attack_NonSpecialAttack>();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        Attack_NonSpecialAttack newStringEntry = new Attack_NonSpecialAttack();
        Attack_BasicInput newBasicInput = new Attack_BasicInput();
        newBasicInput._correctInput = new List<Attack_BaseInput>();
        for (int i = 0; i < stringNormalCount; i++)
        {
            Attack_BaseInput newBaseInput = new Attack_BaseInput();
            Attack_BaseProperties newProperty = new Attack_BaseProperties();
            newProperty._attackName = $"String Attack Entry({i+1})";
            newBaseInput.property = newProperty;
            newBasicInput._correctInput.Add(newBaseInput);
            propertyList.Add(newProperty);
            newStringAttackData.Add(newStringEntry);
        }
        newStringEntry.SpecialAttackName = "New String Attack";
        newStringEntry._attackInput = newBasicInput;
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, null, null, null, null, null, newStringAttackData, 1, newStringEntry.SpecialAttackName);
        editedMoveList.stringNormalAttacks.Insert(0, newStringEntry);
        _attackData = newCompositeAttackData;
    }
    void DisplayStringNormalData(Attack_NonSpecialAttack simpleAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        for (int i = 0; i < simpleAttack._attackInput._correctInput.Count; i++) 
        {
            propertyList.Add(simpleAttack._attackInput._correctInput[i].property);
        }
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>() { simpleAttack };
        _attackData = new CompositeAttackData(propertyList, null, null, null, null, null, newNonSpecialAttackData, propertyList.Count, attackName);
    }
    #endregion

    #region Command Attack Function Section
    void ShowCommandNormalAttacks()
    {
        #region Command Attack Display
        Debug.Log(editedMoveList.commandNormalAttacks.Count);
        GUILayout.Label("Command Attacks");
        if (GUILayout.Button("Add New Command Normal Attack Entry"))
        {
            AddCommandNormalAttackEntry();
        }
        displayCommandNormalAttacks = EditorGUILayout.Foldout(displayCommandNormalAttacks, "Show Command Attacks");
        if (displayCommandNormalAttacks)
        {
            for (int i = 0; i < editedMoveList.commandNormalAttacks.Count; i++)
            {
                var move = editedMoveList.commandNormalAttacks[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.SpecialAttackName, style))
                {
                    DisplayCommandNormalData(move, move.SpecialAttackName);
                }
            }
            if (GUILayout.Button("Clear Command Attack Data"))
            {
                if (editedMoveList.commandNormalAttacks.Count > 0)
                {
                    editedMoveList.commandNormalAttacks.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddCommandNormalAttackEntry()
    {
        List<Attack_NonSpecialAttack> newCommandAttackData = new List<Attack_NonSpecialAttack>();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        Attack_NonSpecialAttack newNormalEntry = new Attack_NonSpecialAttack();
        Attack_BasicInput newBasicInput = new Attack_BasicInput();
        Attack_BaseInput newBaseInput = new Attack_BaseInput();
        Attack_BaseProperties newProperty = new Attack_BaseProperties();

        newProperty._attackName = "New Command Attack Entry";
        newBaseInput.property = newProperty;
        newBasicInput._correctInput = new List<Attack_BaseInput> { newBaseInput };
        newNormalEntry._attackInput = newBasicInput;
        propertyList.Add(newProperty);
        newCommandAttackData.Add(newNormalEntry);
        newNormalEntry.SpecialAttackName = "New Command Attack";

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, null, null, null, null, null, newCommandAttackData, 1, newNormalEntry.SpecialAttackName);

        editedMoveList.commandNormalAttacks.Insert(0, newNormalEntry);
        _attackData = newCompositeAttackData;
    }
    void DisplayCommandNormalData(Attack_NonSpecialAttack simpleAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { simpleAttack._attackInput._correctInput[0].property };
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>() { simpleAttack };
        _attackData = new CompositeAttackData(propertyList, null, null, null, null, null, newNonSpecialAttackData, 1, attackName);
    }
    #endregion

    #region Normal Attack Function Section
    void ShowNormalAttacks() 
    {
        #region Normal Attack Display
        Debug.Log(editedMoveList.simpleAttacks.Count);
        GUILayout.Label("Normal Attacks");
        if (GUILayout.Button("Add New Normal Attack Entry"))
        {
            AddNewNormalAttackEntry();
        }
        displayNormalAttacks = EditorGUILayout.Foldout(displayNormalAttacks, "Show NormalAttacks");
        if (displayNormalAttacks)
        {
            for (int i = 0; i < editedMoveList.simpleAttacks.Count; i++)
            {
                var move = editedMoveList.simpleAttacks[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.SpecialAttackName, style))
                {
                    DisplaySimpleAttackData(move, move.SpecialAttackName);
                }
            }
            if (GUILayout.Button("Clear Normal Attack Data"))
            {
                if (editedMoveList.simpleAttacks.Count > 0)
                {
                    editedMoveList.simpleAttacks.RemoveAt(0);
                    _attackData = null;
                }
            }
        }

        #endregion
    }
    void AddNewNormalAttackEntry()
    {
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        Attack_NonSpecialAttack newNormalEntry = new Attack_NonSpecialAttack();
        Attack_BasicInput newBasicInput = new Attack_BasicInput();
        Attack_BaseInput newBaseInput = new Attack_BaseInput();
        Attack_BaseProperties newProperty = new Attack_BaseProperties();

        newProperty._attackName = "New Normal Attack Entry";
        newBaseInput.property = newProperty;
        newBasicInput._correctInput = new List<Attack_BaseInput> { newBaseInput };
        newNormalEntry._attackInput = newBasicInput;
        propertyList.Add(newProperty);
        newNonSpecialAttackData.Add(newNormalEntry);
        newNormalEntry.SpecialAttackName = "New Command Attack";

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList,null, null, null,null ,null, newNonSpecialAttackData, 1, newNormalEntry.SpecialAttackName);

        editedMoveList.simpleAttacks.Insert(0,newNormalEntry);
        _attackData = newCompositeAttackData;
    }
    void DisplaySimpleAttackData(Attack_NonSpecialAttack simpleAttack,string attackName) 
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { simpleAttack._attackInput._correctInput[0].property };
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>() { simpleAttack };
        _attackData = new CompositeAttackData(propertyList, null, null, null, null, null, newNonSpecialAttackData, 1, attackName);
    }
    #endregion

    #region Throw Function Section
    void ShowThrowAttacks()
    {
        #region Throw Display
        Debug.Log(editedMoveList.BasicThrows.Count);
        GUILayout.Label("Throws");
        if (GUILayout.Button("Add New Throw Entry"))
        {
            AddNewThrowEntry();
        }
        displayThrowAttacks = EditorGUILayout.Foldout(displayThrowAttacks, "Show Throws");
        if (displayThrowAttacks)
        {
            for (int i = 0; i < editedMoveList.BasicThrows.Count; i++)
            {
                var move = editedMoveList.BasicThrows[i];
                if (move == null) continue;

                var style = i == highlightedMoveIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(move.ThrowName, style))
                {
                    DisplayThrowData(move, move.ThrowName);
                }
            }
        }
        if (GUILayout.Button("Clear Throw Attack Data"))
        {
            if (editedMoveList.BasicThrows.Count > 0)
            {
                editedMoveList.BasicThrows.RemoveAt(0);
                _attackData = null;
            }
        }
        #endregion
    }
    void AddNewThrowEntry()
    {
        Attack_ThrowBase _newThrowBase = new Attack_ThrowBase();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        Attack_BasicInput newBasicInput = new Attack_BasicInput();
        Attack_BaseInput newBaseInput = new Attack_BaseInput();
        Attack_BaseProperties newProperty = new Attack_BaseProperties();

        newProperty._attackName = "New Throw Attack Entry";
        newBaseInput.property = newProperty;
        newBasicInput._correctInput = new List<Attack_BaseInput> { newBaseInput };
        _newThrowBase._attackInput = newBasicInput;
        propertyList.Add(newProperty);

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, null, null, null, null, _newThrowBase,null, 1, "");
        AttackHandler_Attack newAttackHandler = new AttackHandler_Attack();
        _newThrowBase._throwAnimation = new List<AttackHandler_Attack>() { newAttackHandler };
        editedMoveList.BasicThrows.Insert(0, _newThrowBase);
        _attackData = newCompositeAttackData;
    }
    void DisplayThrowData(Attack_ThrowBase throwAttack, string throwName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { throwAttack._attackInput._correctInput[0].property };
        _attackData = new CompositeAttackData(propertyList, null, null, null, null, throwAttack, null, 1, throwName);
    }
    #endregion




    #endregion

    #region Center Page Information
    public void DisplayAttackInfomation()
    {
        if (_attackData != null)
        {
            displayCurrentAttackList = EditorGUILayout.Foldout(displayCurrentAttackList, "Show Attack Property List");
            if (displayCurrentAttackList)
            {
                for (int i = 0; i < _attackData.basePropertyCount; i++)
                {
                    var move = _attackData.baseAttackProperties[i];
                    if (move == null) continue;
                    string attackName = _attackData.baseAttackProperties[i]._attackName;
                    var style = i == highlightedAttackIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                    if (GUILayout.Button($"{attackName}_Property {i + 1}", style, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 3f), GUILayout.Height(25)))
                    {
                        previewActive = false;
                        currentCenterAttackData = _attackData.baseAttackProperties[i];
                        currentCenterAttackData.AttackAnims = _attackData.baseAttackProperties[i].AttackAnims;
                        currentFrame = 0;
                        isPlaying = false;
                    }
                }
            }
            GUILayout.Space(50);
            if (moveListData != null && currentCenterAttackData != null)
            {
                ShowAnimationInformation();
                GUILayout.Space(10);
                DisplayAnimationTimeline();
                if (characterModel != null && characterAnimator != null)
                {
                    CreateOrUpdatePreviewInstance();
                    UpdatePreviewInstance();
                    DisplayPreviewWindow();
                    OnEditorUpdate();
                }
            }
            GUILayout.Space(25);
            if (GUILayout.Button("Clear Current Attack Info", GUILayout.Width(155), GUILayout.Height(25)))
            {
                currentCenterAttackData = null;
            }
        }
    }
    void DisplayAnimationTimeline()
    {
        #region AnimSlider
        if (currentCenterAttackData.AttackAnims != null)
        {
            if (currentCenterAttackData.AttackAnims.animClip != null)
            {
                GUILayout.Label($"Current Animation Frame: {currentFrame}");

                #region Slider Region
                float clipLength = currentCenterAttackData.AttackAnims.animClip.length;
                float currentClipLength = clipLength * fps;
                currentFrame = (int)EditorGUILayout.Slider("Animation Frame Timeline:", currentFrame, 0f, currentClipLength, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 2f), GUILayout.Height(20));


                DrawTimelineControls();
                init = (int)EditorGUILayout.Slider("Init:", init, 0f, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                startup = (int)EditorGUILayout.Slider("Startup:", startup, init, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                active = (int)EditorGUILayout.Slider("Active:", active, startup, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                inactive = (int)EditorGUILayout.Slider("Inactive:", inactive, active, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                recoveryAmount = (int)EditorGUILayout.Slider("Recovery Amount:", recoveryAmount, 0f, 100f, GUILayout.Width(500), GUILayout.Height(20));

                GUILayout.Space(25);

                #endregion
            }
        }
        #endregion
    }
    void OnEditorUpdate() 
    {
        if (isPlaying && _currentAnimClip != null)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            lastTime = currentTime;
            timelineCurrentTime += (1/60f) * playbackSpeed;
            currentFrame = (int)(timelineCurrentTime * 60f);

            if (currentFrame >= (int)(_currentAnimClip.length * 60f))
            {
                if (loopPreview)
                {
                    timelineCurrentTime %= _currentAnimClip.length;
                }
                else
                {
                    timelineCurrentTime = (int)(_currentAnimClip.length * 60f);
                    isPlaying = false;
                }
            }

            UpdatePreviewInstance();
            Repaint();
        }
        if (!isPlaying && _currentAnimClip != null)
        {
            if (lastFrame != currentFrame)
            {
                double currentTime = EditorApplication.timeSinceStartup;
                lastTime = currentTime;
                lastFrame = currentFrame;
                timelineCurrentTime = lastFrame / 60f;

                if (timelineCurrentTime >= _currentAnimClip.length)
                {
                    if (loopPreview)
                        timelineCurrentTime %= _currentAnimClip.length;
                    else
                    {
                        timelineCurrentTime = _currentAnimClip.length;
                        isPlaying = false;
                    }
                }

                UpdatePreviewInstance();
                Repaint();
            }
        }
    }
    private void DrawTimelineControls()
    {

        playbackSpeed = EditorGUILayout.FloatField(new GUIContent("Playback Speed", "Adjust playback speed multiplier"), playbackSpeed, GUILayout.Width(500), GUILayout.Height(20));
        loopPreview = EditorGUILayout.Toggle(new GUIContent("Loop Animation", "Toggle looping"), loopPreview, GUILayout.Width(500), GUILayout.Height(20));

        if (GUILayout.Button(isPlaying ? "Pause" : "Play", GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 4f), GUILayout.Height(20)))
        {
            TogglePlayback(_currentAnimClip.length);
        }

        if (GUILayout.Button("Restart", GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 4f), GUILayout.Height(20)))
        {
            RestartPlayback();
        }
    }

    private void TogglePlayback(float clipLength)
    {
        isPlaying = !isPlaying;
        lastTime = EditorApplication.timeSinceStartup;
        Animator previewAnimator = previewInstance.GetComponentInChildren<Animator>();
        if (isPlaying) 
        {
            previewAnimator.Play(_currentAnimClip.name, 0);
        }
        else 
        {
            previewAnimator.StopPlayback();
        }
        if (timelineCurrentTime >= clipLength)
        {
            timelineCurrentTime = 0f;
        }
    }
    private void RestartPlayback()
    {
        timelineCurrentTime = 0f;
        isPlaying = true;
        lastTime = EditorApplication.timeSinceStartup;
    }
    #region Preview Functions
    private void CreateOrUpdatePreviewInstance()
    {
        if (previewActive) 
        {
            return;
        }
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
        }

        if (characterModel == null)
        {
            Debug.LogError("FighterPrefab not assigned in FighterData!");
            return;
        }

        previewInstance = Instantiate(characterModel);
        previewInstance.hideFlags = HideFlags.HideAndDontSave;
        previewInstance.layer = LayerMask.GetMask("Outlined Objects");
        var animator = previewInstance.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing in FighterPrefab!");
            DestroyImmediate(previewInstance);
            return;
        }

        if (currentCenterAttackData.AttackAnims.animClip != null)
        {
            _currentAnimClip = currentCenterAttackData.AttackAnims.animClip;
            CreateAnimationController(animator);
        }

        animator.Rebind();
        animator.Update(0);
        previewUtility.AddSingleGO(previewInstance);
        ResetPreviewCamera();
        previewActive = true;
    }
    void ResetPreviewCamera()
    {
        previewUtility.camera.transform.position = Vector3.zero;
        previewUtility.camera.transform.rotation = Quaternion.identity;
    }
    void DisplayPreviewWindow() 
    {
        previewRect = new Rect();
        previewRect.x = CurrentAttackBodyObject.editorRect.width/3.5f;
        previewRect.y = CurrentAttackBodyObject.editorRect.height/2.015f;
        previewRect.width = 500f;
        previewRect.height = 500f;
        showPreview = EditorGUILayout.Foldout(showPreview, "Preview Settings");
        if (showPreview)
        {
            EditorGUI.indentLevel++;
            previewCameraFOV = EditorGUILayout.Slider("Camera FOV", previewCameraFOV, 10, 90);
            previewCameraPositionOffset = EditorGUILayout.Vector3Field("Camera Offset", previewCameraPositionOffset);
            EditorGUI.indentLevel--;
        }
        DrawPreview(previewRect);
        Repaint();
    }
    void UpdatePreviewInstance() 
    {
        if (currentCenterAttackData.AttackAnims.animClip == null || previewInstance == null)
            return;

        var animator = previewInstance.GetComponentInChildren<Animator>();
        if (animator == null)
            return;

        if (animator.HasState(0, Animator.StringToHash("PreviewState")))
        {
            float normalizedTime = Mathf.Clamp01(timelineCurrentTime / currentCenterAttackData.AttackAnims.animClip.length);
            animator.Play("PreviewState", 0, normalizedTime);
            animator.speed = 0;
            animator.Update(0);
        }
        else
        {
            Debug.LogWarning("PreviewState not found in animator controller!");
        }
    }
    void DrawPreview(Rect _rectSize) 
    {
        if (previewInstance == null)
            return;

        try
        {
            previewUtility.BeginPreview(_rectSize, GUIStyle.none);
            SetupPreviewCamera();
            DrawPreviewInstanceUtil();
            //DrawCollisionBoxesInPreview();
            previewUtility.camera.Render();
            GUI.DrawTexture(_rectSize, previewUtility.EndPreview(), ScaleMode.StretchToFill, false);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error drawing preview: " + ex.Message);
            previewUtility.Cleanup();
            throw;
        }
    }
    private Bounds ComputeBounds(GameObject go)
    {
        Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
        foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }
    private void SetupPreviewCamera()
    {
        Bounds bounds = ComputeBounds(previewInstance);
        Vector3 center = bounds.center;

        previewUtility.camera.transform.position = center + new Vector3(0, 0, -10) + previewCameraPositionOffset - new Vector3(previewPan.x, previewPan.y, 0);
        previewUtility.camera.transform.rotation = Quaternion.Euler(previewCameraRotationEuler);
        previewUtility.camera.fieldOfView = previewCameraFOV / previewZoom;
        previewUtility.camera.nearClipPlane = previewCameraNearClippingPlane;
        previewUtility.camera.farClipPlane = previewCameraFarClippingPlane;
        previewUtility.camera.backgroundColor = backgroundPreviewColor;
    }

    private void DrawPreviewInstanceUtil()
    {
        foreach (Renderer r in previewInstance.GetComponentsInChildren<Renderer>())
        {
            Mesh mesh = null;
            Material mat = null;

            if (r is MeshRenderer mr)
            {
                MeshFilter mf = mr.GetComponent<MeshFilter>();
                if (mf != null)
                    mesh = mf.sharedMesh;
                mat = mr.sharedMaterial;
            }
            else if (r is SkinnedMeshRenderer smr)
            {
                mesh = new Mesh();
                smr.BakeMesh(mesh);
                mat = smr.sharedMaterial;
            }

            if (mesh != null && mat != null)
                previewUtility.DrawMesh(mesh, r.transform.localToWorldMatrix, mat, 0);
        }
    }
    private void CreateAnimationController(Animator animator)
    {
        if (tempController != null)
        {
            DestroyImmediate(tempController);
        }
        if (overrideController != null)
        {
            DestroyImmediate(overrideController);
        }

        tempController = new AnimatorController();

        if (tempController.layers.Length == 0)
            tempController.AddLayer("Base Layer");

        var stateMachine = tempController.layers[0].stateMachine;
        if (stateMachine == null)
        {
            stateMachine = new AnimatorStateMachine();
            tempController.layers[0].stateMachine = stateMachine;
        }

        var state = stateMachine.AddState("PreviewState");
        state.motion = _currentAnimClip;

        overrideController = new AnimatorOverrideController(tempController)
        {
            ["PreviewState"] = _currentAnimClip
        };

        animator.runtimeAnimatorController = overrideController;
    }
    void ClearPreview()
    {
        timelineCurrentTime = 0f;
        isPlaying = false;
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
    }
    /*private void DrawCollisionBoxesInPreview()
    {
        if (fighterController?.CharacterData?.collisionConfig == null || selectedAnimationClip == null)
            return;

        int currentFrame = Mathf.FloorToInt(timelineCurrentTime * fighterController.CharacterData.collisionConfig.fps);
        Mesh cubeMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        foreach (var entry in fighterController.CharacterData.collisionConfig.collisionEntries)
        {
            if (entry.stateName != selectedState)
                continue;

            foreach (var box in entry.collisionBoxes)
            {
                if (currentFrame < box.activeFrameStart || currentFrame > box.activeFrameEnd)
                    continue;

                Transform parent = GetParentTransformInPreview(box.parentName);
                Vector3 position = parent.TransformPoint(box.offset);
                Vector3 size = new Vector3(box.size.x, box.size.y, 0.02f);

                previewSolidMaterial.color = box.GetColor();
                previewUtility.DrawMesh(cubeMesh,
                    Matrix4x4.TRS(position, Quaternion.identity, size),
                    previewSolidMaterial, 0);

                previewWireMaterial.color = box.WireColor;
                DrawWireframeCube(position, size);
            }
        }
    }*/
    #endregion
    void ShowAnimationInformation()
    {
        characterModel = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player Object"), characterModel, typeof(GameObject), true, GUILayout.Width(500), GUILayout.Height(20));
        characterAnimator = (RuntimeAnimatorController)EditorGUILayout.ObjectField(new GUIContent("Object Animator"), characterAnimator, typeof(RuntimeAnimatorController), true, GUILayout.Width(500), GUILayout.Height(20));
        if (currentCenterAttackData.AttackAnims != null)
        {
            _currentAnimClip = currentCenterAttackData.AttackAnims.animClip != null ? currentCenterAttackData.AttackAnims.animClip : null;
            if (lastClip != _currentAnimClip)
            {
                if (_currentAnimClip != null)
                {
                    lastClip = _currentAnimClip;
                    init = currentCenterAttackData.AttackAnims._frameData.init;
                    startup = currentCenterAttackData.AttackAnims._frameData.startup;
                    active = currentCenterAttackData.AttackAnims._frameData.active;
                    inactive = currentCenterAttackData.AttackAnims._frameData.inactive;
                    recoveryAmount = currentCenterAttackData.AttackAnims._frameData.recoveryAmount;
                }
            }
            _currentAnimClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Current Attack Animation:"), _currentAnimClip, typeof(AnimationClip), true, GUILayout.Width(500), GUILayout.Height(20));
        }
        else
        {
            _currentAnimClip = null;
            _currentAnimClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Current Attack Animation:"), _currentAnimClip, typeof(AnimationClip), true, GUILayout.Width(500), GUILayout.Height(20));
        }
    }
    #endregion

    #region Right Page Information
    void ShowRightPageAttackInformation() 
    {
        GUILayout.Space(50);
        displayPrimaryAttackData = EditorGUILayout.Foldout(displayPrimaryAttackData, "Show Primary Data");
        if (displayPrimaryAttackData)
        {
            DisplayAdvanceSpecialData();
            DisplayStanceData();
            DisplayRekkaData();
            DisplaySpecialData();
            DisplayNormalData();
            DisplayThrowData();
        }
        displayMainPropertyData = EditorGUILayout.Foldout(displayMainPropertyData, "Show Attack Property Data");
        if (displayMainPropertyData)
        {
            GUILayout.Label("General Attack Information");

            currentCenterAttackData._attackName = (string)EditorGUILayout.TextField("Attack Name:", currentCenterAttackData._attackName);
            currentCenterAttackData.rawAttackDamage = (float)EditorGUILayout.FloatField("Raw Damage:", currentCenterAttackData.rawAttackDamage);
            currentCenterAttackData.counterHitDamageMult = (float)EditorGUILayout.FloatField("Counter Hit Multiplier:", currentCenterAttackData.counterHitDamageMult);

            GUILayout.Space(25);
            Attack_StunValues stunValues = currentCenterAttackData.attackMainStunValues;
            stunValues.hitstunValue = (int)EditorGUILayout.FloatField("Hit Stun:", stunValues.hitstunValue);
            stunValues.blockStunValue = (int)EditorGUILayout.FloatField("Block Stun:", stunValues.blockStunValue);
            stunValues.hitstopValue = (int)EditorGUILayout.FloatField("Hit Stop:", stunValues.hitstopValue);
            stunValues.blockStopValue = (int)EditorGUILayout.FloatField("Block Stop:", stunValues.blockStopValue);

            GUILayout.Space(25);
            currentCenterAttackData.hitLevel = (HitLevel)EditorGUILayout.EnumFlagsField("Hit Level", currentCenterAttackData.hitLevel);

            GUILayout.Space(25);
            currentCenterAttackData._meterRequirement = (int)EditorGUILayout.FloatField("Meter Requirement:", currentCenterAttackData._meterRequirement);
            currentCenterAttackData._meterAwardedOnHit = (int)EditorGUILayout.FloatField("Meter Awarded:", currentCenterAttackData._meterAwardedOnHit);
            currentCenterAttackData.attackScalingPercent = (int)EditorGUILayout.FloatField("Attack Scaling:", currentCenterAttackData.attackScalingPercent);

            currentCenterAttackData.dashCancelable = (bool)EditorGUILayout.Toggle("Dash Cancelable:", currentCenterAttackData.dashCancelable);
            currentCenterAttackData.JumpCancelable = (bool)EditorGUILayout.Toggle("Jump Cancelable:", currentCenterAttackData.JumpCancelable);

            GUILayout.Space(25);
            currentCenterAttackData._airInfo = (AirAttackInfo)EditorGUILayout.EnumFlagsField("Air Properties", currentCenterAttackData._airInfo);

            Attack_CancelInfo _cancelProperty = currentCenterAttackData.cancelProperty;
            GUILayout.Space(25);
            _cancelProperty.CurrentLevel = (Cancel_State)EditorGUILayout.EnumFlagsField("Attack Cancel Level", _cancelProperty.CurrentLevel);
            _cancelProperty.nextAvailableAttackRoute = (Cancel_State)EditorGUILayout.EnumFlagsField("Next Cancel State", _cancelProperty.nextAvailableAttackRoute);
            currentCenterAttackData._moveType = (MoveType)EditorGUILayout.EnumPopup("Move Type", currentCenterAttackData._moveType);

            GUILayout.Space(25);
            Horizontal_KnockBack LateralKnockBackData = currentCenterAttackData.LateralKB_Data;
            Vertical_KnockBack VerticalKnockBackData = currentCenterAttackData.VerticalKB_Data;

            LateralKnockBackData.Hit_HKB_Level = (Attack_KnockBack_Lateral)EditorGUILayout.EnumPopup("Hit Knockback", LateralKnockBackData.Hit_HKB_Level);
            LateralKnockBackData.Hit_Value = (int)EditorGUILayout.FloatField("Knockback Value:", LateralKnockBackData.Hit_Value);
            LateralKnockBackData.Block_HKB_Level = (Attack_KnockBack_Lateral)EditorGUILayout.EnumPopup("Block Knockback", LateralKnockBackData.Block_HKB_Level);
            LateralKnockBackData.Block_Value = (int)EditorGUILayout.FloatField("Block Knockback Value:", LateralKnockBackData.Block_Value);
            GUILayout.Space(25);
            VerticalKnockBackData.Hit_VKB_Level = (Attack_KnockBack_Vertical)EditorGUILayout.EnumPopup("Hit KnockUp/Down", VerticalKnockBackData.Hit_VKB_Level);
            VerticalKnockBackData.Hit_Value = (int)EditorGUILayout.FloatField("KnockUp Value:", VerticalKnockBackData.Hit_Value);
            VerticalKnockBackData.Block_VKB_Level = (Attack_KnockBack_Vertical)EditorGUILayout.EnumPopup("Block KnockUp/Down", VerticalKnockBackData.Block_VKB_Level);
            VerticalKnockBackData.Block_Value = (int)EditorGUILayout.FloatField("Block KnockUp Value:", VerticalKnockBackData.Block_Value);
            currentCenterAttackData.KnockDown = (Attack_KnockDown)EditorGUILayout.EnumPopup("Knockdown", currentCenterAttackData.KnockDown);
        }
       
    }
    #region Specific Attack Data Display Code
    void DisplayAdvanceSpecialData() 
    {
        if (_attackData.advancedInputData != null)
        {
            GUILayout.Label("Advanced Input Primary Data");
        }
    }
    void DisplayStanceData()
    {
        if (_attackData.stanceInputData != null)
        {
            GUILayout.Label("Stance Primary Data");
        }
    }
    void DisplayRekkaData()
    {
        if (_attackData.rekkaAttackData != null)
        {
            GUILayout.Label("Rekka Primary Data");
            _attackData.rekkaAttackData.RekkaSpecialAttack_Name = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.rekkaAttackData.RekkaSpecialAttack_Name);
            for (int i = 0; i < _attackData.rekkaAttackData.rekkaInput.mainAttackInput.Count; i++)
            {
                _attackData.rekkaAttackData.rekkaInput.mainAttackInput[i].attackString = (string)EditorGUILayout.TextField($"Rekka Attack Input_{i + 1}:", _attackData.rekkaAttackData.rekkaInput.mainAttackInput[i].attackString);
            }
            _attackData.rekkaAttackData.LeewayTime = (int)EditorGUILayout.FloatField($"Leeway Time Between Attacks:", _attackData.rekkaAttackData.LeewayTime);
            if (_attackData.followUpAttackIndex > 0)
            {
                string currentHighlightedRekkaInput = _attackData.rekkaAttackData.rekkaInput._rekkaPortion[_attackData.followUpAttackIndex].individualRekkaAttack._correctInput[0]._correctSequence;
                currentHighlightedRekkaInput = (string)EditorGUILayout.TextField("Rekka Individual Input:", currentHighlightedRekkaInput);
            }
        }
    }
    void DisplaySpecialData()
    {
        if (_attackData.specialInputData != null)
        {
            GUILayout.Label("Special Primary Data");
            _attackData.SpecialAttackName = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.SpecialAttackName);
            for (int i = 0; i < 3; i++) 
            {
                _attackData.specialInputData.attackInput[i].attackString = (string)EditorGUILayout.TextField($"Special Attack Input_{i+1}:", _attackData.specialInputData.attackInput[i].attackString);
            }
        }
    }
    void DisplayNormalData()
    {
        if (_attackData.normalAttackData != null)
        {
            GUILayout.Label("Normal Primary Data");
            GUILayout.Label($"Normal Attack Count: {_attackData.normalAttackData.Count}");
            _attackData.SpecialAttackName = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.SpecialAttackName);
            _attackData.normalAttackData[0]._attackInput._correctInput[0]._correctSequence = (string)EditorGUILayout.TextField("Attack Button Sequence:", _attackData.normalAttackData[0]._attackInput._correctInput[0]._correctSequence);

        }
    }
    void DisplayThrowData()
    {
        if (_attackData.throwInputData != null)
        {
            GUILayout.Label("Throw Primary Data");
            GUILayout.Label($"Throw Attack Count: {_attackData.throwInputData._attackInput._correctInput.Count}");
            _attackData.SpecialAttackName = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.SpecialAttackName);
            _attackData.throwInputData._attackInput._correctInput[0]._correctSequence = (string)EditorGUILayout.TextField("Attack Button Sequence:", _attackData.throwInputData._attackInput._correctInput[0]._correctSequence);
            AnimationClip throwAnim = _attackData.throwInputData._throwAnimation[0] != null ? _attackData.throwInputData._throwAnimation[0].animClip : null;
            throwAnim = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Current Attack Animation:"), throwAnim, typeof(AnimationClip), true, GUILayout.Width(CurrentAttackInformationObject.editorRect.width), GUILayout.Height(20));
        }
    }
    #endregion
    void ShowRightPageCollisionInformation()
    {
        GUILayout.Space(50);
        GUILayout.Label("Collision Information");
        AttackHandler_Attack attackAnim = currentCenterAttackData.AttackAnims;

        attackAnim.attackType = (HitBoxType)EditorGUILayout.EnumPopup("Attack Type:", attackAnim.attackType);
        attackAnim.hb_placement = (Vector3)EditorGUILayout.Vector3Field("Hitbox Placement", attackAnim.hb_placement);
        attackAnim.hb_orientation = (Vector3)EditorGUILayout.Vector3Field("Hitbox Orientation", attackAnim.hb_orientation);
        attackAnim.hb_size = (Vector2)EditorGUILayout.Vector2Field("Hitbox size", attackAnim.hb_size);

        GUILayout.Space(25);
        attackAnim.hurtType = (HurtBoxType)EditorGUILayout.EnumPopup("Hurtbox Type:", attackAnim.hurtType);
        attackAnim.hb_placement = (Vector3)EditorGUILayout.Vector3Field("Hurtbox Placement", attackAnim.hu_placement);
        attackAnim.hb_orientation = (Vector3)EditorGUILayout.Vector3Field("Hurtbox Orientation", attackAnim.hu_orientation);
        attackAnim.hb_size = (Vector2)EditorGUILayout.Vector2Field("Hurtbox size", attackAnim.hu_size);

        GUILayout.Space(25);
        HitCount _hitCount = attackAnim._hitCount;
        _hitCount._startCount = (int)EditorGUILayout.FloatField("Initial Hit Count:", _hitCount._startCount);
        _hitCount._startRefreshRate = (int)EditorGUILayout.FloatField("Initial Refresh Rate:", _hitCount._startRefreshRate);

    }
    #endregion

    #region Show Window Data
    void DrawCurrentAttackHeader()
    {
        GUILayout.BeginArea(CurrentAttackHeaderObject.editorRect);
        #region FillArea
        #endregion
        GUILayout.EndArea();
    }
    void DrawCurrentAttackBody()
    {
        GUILayout.BeginArea(CurrentAttackBodyObject.editorRect);
        #region FillArea
        if (moveListData != null)
        {
            GUILayout.Label("Current Attack");
            if (_attackData != null)
            {
                DisplayAttackInfomation();
            }
        }
        #endregion
        GUILayout.EndArea();
    }
    void DrawCurrentAttackInfo()
    {
        GUILayout.BeginArea(CurrentAttackInformationObject.editorRect);
        #region FillArea
        GUILayout.Label("Current Attack Information");
        if(currentCenterAttackData != null) 
        {
            if(_moveListEditState == MoveListEditMode.MainAttackInfoMode) 
            {
                ShowRightPageAttackInformation();
            }
            if(_moveListEditState == MoveListEditMode.MainAttackCollisionMode) 
            {
                ShowRightPageCollisionInformation();
            }
        }
        GUILayout.Space(145);
        if (moveListData != null)
        {
            _moveListEditState = (MoveListEditMode)GUILayout.Toolbar((int)_moveListEditState, new[] { "Current Attack Info Editor", "Current Attack Collision Editor" }, GUILayout.Width(CurrentAttackInformationObject.editorRect.width/1f), GUILayout.Height(50));
        }
        #endregion
        GUILayout.EndArea();
    }
    void DrawSaveChangesHeader()
    {
        GUILayout.BeginArea(SaveDataObject.editorRect);
        #region FillArea
        //GUILayout.Label("Toggle Data");
        if (moveListData != null)
        {
            if (GUILayout.Button("Save Changes?", GUILayout.Width(SaveDataObject.editorRect.width/3f), GUILayout.Height(SaveDataObject.editorRect.height/20f)))
            {
                SaveMoveListChanges();
            }
        }
        #endregion
        GUILayout.EndArea();
    }
    #endregion

    void SaveMoveListChanges() 
    {
        try 
        {
            moveListData.SetFullMoveListData(editedMoveList);
            Debug.Log($"MoveList Changes ARE SAVED.");
        }
        catch (Exception e) 
        {
            Debug.LogError($"MoveList Changes ARE NOT SAVED. ERROR: {e}");
        }
        
    }
    void GetMoveListData() 
    {
        if (editedMoveList != null)
        {
            if (lastMoveList != moveListData)
            {
                FullMoveList newMoveList = null;
                newMoveList = moveListData.GetFullMoveList();
                editedMoveList = newMoveList;
            }
        }
        if (editedMoveList == null)
        {
            FullMoveList newMoveList = null;
            newMoveList = moveListData.GetFullMoveList();
            editedMoveList = newMoveList;
        }
    } 
}
[Serializable]
public class EditorMoveListObject 
{
    public Texture2D editorTexture;
    public Color32 editorColor;
    public Rect editorRect;
    public EditorMoveListObject(Texture2D _texture,Color32 _color, Rect _rect) 
    {
        editorTexture = _texture;
        editorColor = _color;
        editorRect = _rect;
    }
}
[Serializable]
public class CompositeAttackData 
{
    public List<Attack_BaseProperties> baseAttackProperties;

    public int basePropertyCount;
    public Attack_AdvancedSpecialMove advancedInputData;
    public Attack_RekkaSpecialMove rekkaAttackData;
    public Attack_StanceSpecialMove stanceInputData;
    public Attack_BasicSpecialMove specialInputData;
    public Attack_ThrowBase throwInputData;
    public List<Attack_NonSpecialAttack> normalAttackData;
    public string SpecialAttackName;
    public int followUpAttackIndex;
    public CompositeAttackData(
        List<Attack_BaseProperties> _baseProperties, 
        Attack_AdvancedSpecialMove _advancedInputData = null,
        Attack_RekkaSpecialMove _rekkaData = null,
        Attack_StanceSpecialMove _stanceData = null,
        Attack_BasicSpecialMove _specialData = null,
        Attack_ThrowBase _throwData = null,
        List<Attack_NonSpecialAttack> _nonSpecialData = null
        ,int _basePropertyCount = 0,
        string _specialName = "",
        int _followUpIndex = -1) 
    {
        baseAttackProperties = _baseProperties;
        advancedInputData = _advancedInputData;
        rekkaAttackData = _rekkaData;
        stanceInputData = _stanceData;
        specialInputData = _specialData;
        throwInputData = _throwData;
        normalAttackData = _nonSpecialData;
        if(baseAttackProperties.Count > 0) 
        {
            basePropertyCount = baseAttackProperties.Count;
        }
        else 
        {
            basePropertyCount = _basePropertyCount;
        }
        SpecialAttackName = _specialName;
        followUpAttackIndex = _followUpIndex;
    }
}