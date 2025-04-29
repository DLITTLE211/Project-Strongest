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
    enum MoveListEditMode { MainAttackInfoMode, MainAttackCollisionMode }
    MoveListEditMode _moveListEditState;
    public EditorMoveListObject MoveListHeaderObject;
    public EditorMoveListObject MoveListBodyObject;
    public EditorMoveListObject CurrentAttackHeaderObject;
    public EditorMoveListObject CurrentAttackBodyObject;
    public EditorMoveListObject CurrentAttackInformationObject;
    public EditorMoveListObject SaveDataObject;
    #endregion

    #region Character Data
    Character_MoveList moveListData;
    private Character_MoveList lastMoveList;
    FullMoveList editedMoveList;
    private GameObject characterModel;
    private RuntimeAnimatorController characterAnimator;
    #endregion

    #region CompositeAttackData
    CompositeAttackData _attackData;
    Attack_BaseProperties currentCenterAttackData;
    private int highlightedAttackIndex = -1;
    private int subAttackDataIndex;
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
    int superFollowUpCount;
    int commandGrabFollowUpCount;
    int counterFollowUpCount;
    int stanceAttackCount;
    int stanceKillCount;
    int rekkaAttackCount;
    int stringNormalCount;
    bool displayCommandNormalAttacks = false;
    bool displayNormalAttacks = false;
    bool displayThrowAttacks = false;
    List<DisplayExtraFramePoint> attackAnimExtraPoints;

    bool displayPrimaryAttackData = false;
    bool displayMainPropertyData = false;
    #endregion

    #region Animation Timeline Data
    private AttackHandler_Attack newAttackAnim;
    private AnimatorOverrideController overrideController;
    private AnimationClip lastClip;
    private AnimatorController tempController;
    private AnimationClip _currentAnimClip;
    private float timelineCurrentTime = 0f;
    private int fps = 60;
    private int currentFrame;
    int init;
    int startup;
    int active;
    int inactive;
    int recoveryAmount;
    private bool isPlaying = false;
    private double lastTime;
    private bool loopPreview = false;
    [SerializeField] private float playbackSpeed = 1f;
    private Material hitboxMat;
    private Material hurtboxMat;
    private Color moveEventColor = Color.cyan;
    private Color collisionBoxColor = new Color(1f, 0.5f, 0f, 0.6f); // semi-transparent orange
    private Color collisionBoxOutlineColor = Color.yellow; // fallback color
    private float markerWidth = 4f;
    int lastFrame;
    bool previewActive;
    byte alphaLevel = 127;
    #endregion

    #region Preview Window Code
    private PreviewRenderUtility previewUtility;
    public GameObject previewInstance;
    private Rect previewRect;
    private float previewHeight = 250f;
    private float verticalBias;
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
    List<Color> hitboxColorList = new List<Color>();
    List<Color> hurtboxColorList = new List<Color>();
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
        StartFunctionCalls();
    }
    void AddHitboxColorList()
    {

        hitboxColorList.Add(new Color32(0,255,0, alphaLevel));
        hitboxColorList.Add(new Color32(255, 0, 0, alphaLevel));
        hitboxColorList.Add(new Color32(0, 0, 255, alphaLevel));
        hitboxColorList.Add(new Color32(255, 255, 255, alphaLevel));
        hitboxColorList.Add(new Color32(125, 125, 125, alphaLevel));
        hitboxColorList.Add(new Color32(0, 255, 255, alphaLevel));
        hitboxColorList.Add(new Color32(255, 255, 0, alphaLevel));
        hitboxColorList.Add(new Color32(0, 0, 0, alphaLevel));
        hitboxColorList.Add(new Color32(255, 0, 255, alphaLevel));
    }
    void AddHurtboxColorList() 
    {

        hurtboxColorList.Add( new Color32(255, 0, 170, alphaLevel));
        hurtboxColorList.Add( new Color32(102, 222, 255, alphaLevel));
        hurtboxColorList.Add( new Color32(197, 255, 102, alphaLevel));
        hurtboxColorList.Add( new Color32(236, 220, 188, alphaLevel));
        hurtboxColorList.Add( new Color32(155, 97, 52, alphaLevel));
        hurtboxColorList.Add( new Color32(57, 207, 255, alphaLevel));
        hurtboxColorList.Add( new Color32(135, 135, 135, alphaLevel));
        hurtboxColorList.Add( new Color32(255, 255, 255, alphaLevel));
        hurtboxColorList.Add( new Color32(0, 0, 0, alphaLevel));
        hurtboxColorList.Add( new Color32(188, 106, 106, alphaLevel));
        hurtboxColorList.Add( new Color32(2, 150, 90, alphaLevel));
        hurtboxColorList.Add( new Color32(210, 3, 45, alphaLevel));
    }
    void StartFunctionCalls()
    {
        AddHitboxColorList();
        verticalBias = 0f;
        AddHurtboxColorList();
        newAttackAnim = null;
        attackAnimExtraPoints = new List<DisplayExtraFramePoint>();
        extraFramePointCount = 0;
        lastCount = 0;
        showExtraFramePointVariables = false;
        editedMoveList = null;
        lastClip = null;
        _currentAnimClip = null;
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
        size1.width = Screen.width / 4f;
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
        size2.y = size1.height + 10f;
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
        size5.x = size4.x * 3;
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
        size6.y = Screen.height / 1.08f;
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
        hitboxMat = (Material)EditorGUILayout.ObjectField("HitBoxMat", hitboxMat, typeof(Material), true);
        hurtboxMat = (Material)EditorGUILayout.ObjectField("HurtBoxMat", hurtboxMat, typeof(Material), true);
        moveListData = (Character_MoveList)EditorGUILayout.ObjectField("Current Movelist", moveListData, typeof(Character_MoveList), true);
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
    Vector2 moveListScrollWheelPos;
    void FillDataOnScreen()
    {
        moveListScrollWheelPos = EditorGUILayout.BeginScrollView(moveListScrollWheelPos, GUILayout.Width(MoveListBodyObject.editorRect.width), GUILayout.Height(800));
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
        EditorGUILayout.EndScrollView();
        GUILayout.Space(50);
    }

    #region Supers Function Section
    void ShowSupersAttacks()
    {
        #region Super Display
        GUILayout.Label("Super Attacks");
        superFollowUpCount = EditorGUILayout.IntField("Super FollowUp Count", superFollowUpCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
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
                    DisplayBasicSuperAttacksMoveData(move, move.specialMoveName);
                }
            }
            if (GUILayout.Button("Clear Super Attack Data"))
            {
                if (editedMoveList.BasicSuperAttacks.Count > 0)
                {
                    editedMoveList.BasicSuperAttacks.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddBasicSuperAttacksEntry()
    {
        Attack_AdvancedSpecialMove newAdvancedAttackData = new Attack_AdvancedSpecialMove();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> animationFollowupList = new List<AttackHandler_Attack>();

        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        newProperty._attackName = $"Special Attack Entry";
        propertyList.Add(newProperty);
        newAdvancedAttackData.property = newProperty;
        newAdvancedAttackData.attackInput = new List<Attack_Input>();
        newAdvancedAttackData._customAnimation = new List<AttackHandler_Attack>();
        for (int i = 0; i < 3; i++)
        {
            newAdvancedAttackData.attackInput.Add(new Attack_Input("", ("").ToCharArray()));
        }
        animationFollowupList.Add(newAdvancedAttackData.property.AttackAnims);
        for (int i = 0; i < superFollowUpCount; i++)
        {
            AttackHandler_Attack newAttackHandler = new AttackHandler_Attack();
            newAttackHandler._frameData = new FrameData();
            animationFollowupList.Add(newAttackHandler);
            newAttackHandler._hitCount = new HitCount();
            newAdvancedAttackData._customAnimation.Add(newAttackHandler);
        }
        newAdvancedAttackData.specialMoveName = "New Super Attack Data";
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, animationFollowupList, newAdvancedAttackData, null, null, null, null, null, propertyList.Count, newAdvancedAttackData.specialMoveName);
        editedMoveList.BasicSuperAttacks.Insert(0, newAdvancedAttackData);
        _attackData = newCompositeAttackData;
    }
    void DisplayBasicSuperAttacksMoveData(Attack_AdvancedSpecialMove specialAttack, string specialAttackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { specialAttack.property };
        List<AttackHandler_Attack> animationFollowupList = new List<AttackHandler_Attack>();
        animationFollowupList.Add(specialAttack.property.AttackAnims);
        for (int i = 0; i < specialAttack._customAnimation.Count; i++) 
        {
            animationFollowupList.Add(specialAttack._customAnimation[i]);
        }
        Attack_AdvancedSpecialMove newAdvancedMoveData = specialAttack;
        newAdvancedMoveData.specialMoveName = specialAttackName;
        _attackData = new CompositeAttackData(propertyList, animationFollowupList,newAdvancedMoveData, null, null, null, null, null, 1, newAdvancedMoveData.specialMoveName);
    }
    #endregion

    #region Command Grabs Function Section
    void ShowCommandGrabAttacks()
    {
        #region Command Grabs Display
        GUILayout.Label("Command Grabs");
        commandGrabFollowUpCount = EditorGUILayout.IntField("CommandGrab FollowUp Count", commandGrabFollowUpCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
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
                    DisplayCommandGrabMoveData(move, move.specialMoveName);
                }
            }
            if (GUILayout.Button("Clear Command Grab Attack Data"))
            {
                if (editedMoveList.CommandThrows.Count > 0)
                {
                    editedMoveList.CommandThrows.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddCommandGrabEntry()
    {
        Attack_AdvancedSpecialMove newAdvancedAttackData = new Attack_AdvancedSpecialMove();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> animationFollowupList = new List<AttackHandler_Attack>();
        newAdvancedAttackData._customAnimation = new List<AttackHandler_Attack>();

        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        newProperty._attackName = $"Special Attack Entry";
        propertyList.Add(newProperty);
        newAdvancedAttackData.property = newProperty;
        newAdvancedAttackData.attackInput = new List<Attack_Input>();
        for (int i = 0; i < 3; i++)
        {
            newAdvancedAttackData.attackInput.Add(new Attack_Input("", ("").ToCharArray()));
        }
        animationFollowupList.Add(newAdvancedAttackData.property.AttackAnims);
        for (int i = 0; i < commandGrabFollowUpCount; i++)
        {
            AttackHandler_Attack newAttackHandler = new AttackHandler_Attack();
            newAttackHandler._frameData = new FrameData();
            newAttackHandler._hitCount = new HitCount();
            animationFollowupList.Add(newAttackHandler);
            newAdvancedAttackData._customAnimation.Add(newAttackHandler);
        }
        newAdvancedAttackData.specialMoveName = "New Command Grab Attack Data";
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, animationFollowupList, newAdvancedAttackData, null, null, null, null, null, propertyList.Count, newAdvancedAttackData.specialMoveName);
        editedMoveList.CommandThrows.Insert(0, newAdvancedAttackData);
        _attackData = newCompositeAttackData;
    }
    void DisplayCommandGrabMoveData(Attack_AdvancedSpecialMove specialAttack, string specialAttackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { specialAttack.property };
        List<AttackHandler_Attack> animationFollowupList = new List<AttackHandler_Attack>();
        animationFollowupList.Add(specialAttack.property.AttackAnims);
        for (int i = 0; i < specialAttack._customAnimation.Count; i++)
        {
            animationFollowupList.Add(specialAttack._customAnimation[i]);
        }
        Attack_AdvancedSpecialMove newAdvancedMoveData = specialAttack;
        newAdvancedMoveData.specialMoveName = specialAttackName;
        _attackData = new CompositeAttackData(propertyList, animationFollowupList, newAdvancedMoveData, null, null, null, null, null, 1, newAdvancedMoveData.specialMoveName);
    }
    #endregion

    #region Counter Attack Function Section
    void ShowCounterAttacks()
    {
        #region Counter Attack Display
        GUILayout.Label("Counter Attacks");
        counterFollowUpCount = EditorGUILayout.IntField("Counter FollowUp Count", counterFollowUpCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
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
                    DisplayCounterAttackMoveData(move, move.specialMoveName);
                }
            }
            if (GUILayout.Button("Clear Counter Attack Data"))
            {
                if (editedMoveList.CounterAttacks.Count > 0)
                {
                    editedMoveList.CounterAttacks.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddCounterAttackEntry()
    {
        Attack_AdvancedSpecialMove newAdvancedAttackData = new Attack_AdvancedSpecialMove();
        newAdvancedAttackData._customAnimation = new List<AttackHandler_Attack>();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> animationFollowupList = new List<AttackHandler_Attack>();

        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        newProperty._attackName = $"Special Attack Entry";
        propertyList.Add(newProperty);
        newAdvancedAttackData.property = newProperty;
        newAdvancedAttackData.attackInput = new List<Attack_Input>();
        for (int i = 0; i < 3; i++)
        {
            newAdvancedAttackData.attackInput.Add(new Attack_Input("", ("").ToCharArray()));
        }
        animationFollowupList.Add(newAdvancedAttackData.property.AttackAnims);
        for (int i = 0; i < counterFollowUpCount; i++)
        {
            AttackHandler_Attack newAttackHandler = new AttackHandler_Attack();
            newAttackHandler._frameData = new FrameData();
            newAttackHandler._hitCount = new HitCount();
            animationFollowupList.Add(newAttackHandler);
            newAdvancedAttackData._customAnimation.Add(newAttackHandler);
        }
        newAdvancedAttackData.specialMoveName = "New Counter Attack Data";
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList,animationFollowupList ,newAdvancedAttackData, null, null, null, null, null, propertyList.Count, newAdvancedAttackData.specialMoveName);
        editedMoveList.CounterAttacks.Insert(0, newAdvancedAttackData);
        _attackData = newCompositeAttackData;
    }
    void DisplayCounterAttackMoveData(Attack_AdvancedSpecialMove specialAttack, string specialAttackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { specialAttack.property };
        Attack_AdvancedSpecialMove newAdvancedMoveData = specialAttack;

        List<AttackHandler_Attack> animationFollowupList = new List<AttackHandler_Attack>();
        animationFollowupList.Add(specialAttack.property.AttackAnims);
        for (int i = 0; i < specialAttack._customAnimation.Count; i++)
        {
            animationFollowupList.Add(specialAttack._customAnimation[i]);
        }
        newAdvancedMoveData.specialMoveName = specialAttackName;
       _attackData = new CompositeAttackData(propertyList,animationFollowupList ,newAdvancedMoveData, null, null, null, null, null, 1, newAdvancedMoveData.specialMoveName);
    }
    #endregion

    #region Stance Function Section
    void ShowStanceAttacks()
    {
        #region Stance Attack Display
        GUILayout.Label("Stance Attacks");
        if (GUILayout.Button("Add New Stance Entry"))
        {
            AddStanceAttackEntry();
        }
        stanceAttackCount = EditorGUILayout.IntField("Set Stance Attack Count", stanceAttackCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
        stanceKillCount = EditorGUILayout.IntField("Set Stance Kill Count", stanceKillCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
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
                    DisplayStanceMoveData(move, move.StanceSpecialAttack_Name);
                }
            }
            if (GUILayout.Button("Clear Stance Attack Data"))
            {
                if (editedMoveList.stanceSpecials.Count > 0)
                {
                    editedMoveList.stanceSpecials.RemoveAt(0);
                    _attackData = null;
                }
            }
        }
        #endregion
    }
    void AddStanceAttackEntry()
    {
        Attack_StanceSpecialMove newStanceMove = new Attack_StanceSpecialMove();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>(); 
        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        StanceInput newStanceInput = new StanceInput();
        List<Attack_Input> _totalStanceInputs = new List<Attack_Input>();
        newStanceMove.StanceSpecialAttack_Name = $"New Stance Attack Entry";
        newProperty._attackName = $"Stance Main Attack Entry";
        propertyList.Add(newProperty);
        newStanceMove.stanceInput = new StanceInput();
        newStanceMove.stanceInput._stanceInput = new List<Attack_Input>();
        for (int i = 0; i < 3; i++)
        {
            _totalStanceInputs.Add(new Attack_Input("", ("").ToCharArray()));
        }
        newStanceMove.stanceStartProperty = newProperty;

        attackAnimList.Add(newStanceMove.stanceStartProperty.AttackAnims);
        StanceAttack newStanceAttack = new StanceAttack();
        newStanceAttack._stanceButtonInput = new Attack_BasicInput();
        newStanceAttack._stanceButtonInput._correctInput = new List<Attack_BaseInput>();
        for (int i = 0; i < stanceAttackCount; i++)
        {
            Attack_BaseProperties newstanceProperty = new Attack_BaseProperties();
            newstanceProperty._attackName = $"New Stance Attack Sub Entry{i + 1}";
            Attack_BaseInput newBaseInput = new Attack_BaseInput();

            newBaseInput.property = newstanceProperty;
            newStanceAttack._stanceButtonInput._correctInput.Add(newBaseInput);
            newStanceInput.stanceAttack = newStanceAttack;
            propertyList.Add(newstanceProperty);
            attackAnimList.Add(newStanceAttack._stanceButtonInput._correctInput[0].property.AttackAnims);
        }
        StanceAttack newStanceKill = new StanceAttack();
        newStanceKill._stanceButtonInput = new Attack_BasicInput();
        newStanceKill._stanceButtonInput._correctInput = new List<Attack_BaseInput>();
        for (int i = 0; i < stanceKillCount; i++)
        {
            Attack_BaseProperties newstanceProperty = new Attack_BaseProperties();
            newstanceProperty._attackName = $"New Stance Kill Sub Entry{i + 1}";
            Attack_BaseInput newBaseInput = new Attack_BaseInput();

            newBaseInput.property = newstanceProperty;
            newStanceKill._stanceButtonInput._correctInput.Add(newBaseInput);
            newStanceInput.stanceKill = newStanceKill;
            propertyList.Add(newstanceProperty);
            attackAnimList.Add(newStanceKill._stanceButtonInput._correctInput[0].property.AttackAnims);
        }
        newStanceInput._stanceInput = _totalStanceInputs;
        newStanceMove.stanceInput = newStanceInput;

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList, null, null, newStanceMove, null, null, null, propertyList.Count, newStanceMove.StanceSpecialAttack_Name);
        editedMoveList.stanceSpecials.Insert(0, newStanceMove);
        _attackData = newCompositeAttackData;
    }
    void DisplayStanceMoveData(Attack_StanceSpecialMove specialAttack, string specialMoveName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        propertyList.Add(specialAttack.stanceStartProperty);
        attackAnimList.Add(specialAttack.stanceStartProperty.AttackAnims);
        if (specialAttack.stanceInput.stanceAttack._stanceButtonInput._correctInput.Count > 0)
        {
            Attack_BaseProperties currentAttackProperty = specialAttack.stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property;
            propertyList.Add(currentAttackProperty);
            attackAnimList.Add(currentAttackProperty.AttackAnims);
        }
        if (specialAttack.stanceInput.stanceKill._stanceButtonInput._correctInput.Count > 0)
        {
            Attack_BaseProperties currentKillProperty = specialAttack.stanceInput.stanceKill._stanceButtonInput._correctInput[0].property;
            propertyList.Add(currentKillProperty);
            attackAnimList.Add(currentKillProperty.AttackAnims);
        }

        Attack_StanceSpecialMove mewStanceData = specialAttack;
        mewStanceData.StanceSpecialAttack_Name = specialMoveName;
        _attackData = new CompositeAttackData(propertyList, attackAnimList, null, null, mewStanceData, null, null, null, propertyList.Count, mewStanceData.StanceSpecialAttack_Name);
    }
    #endregion

    #region Rekka Function Section
    void ShowRekkaAttacks()
    {
        #region String Attack Display
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
                    DisplayRekkaMoveData(move, move.RekkaSpecialAttack_Name, i);
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
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
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
        attackAnimList.Add(newRekkaInput.mainAttackProperty.AttackAnims);
        newRekkaInput._rekkaPortion = new List<RekkaAttack>();
        for (int i = 0; i < rekkaAttackCount; i++)
        {
            Attack_BaseProperties newRekkaProperty = new Attack_BaseProperties();
            newRekkaProperty._attackName = $"New Rekka Sub Entry{i + 1}";
            Attack_BaseInput newBaseInput = new Attack_BaseInput();
            RekkaAttack newRekkaAttack = new RekkaAttack();
            newRekkaAttack.individualRekkaAttack = new Attack_BasicInput();
            newRekkaAttack.individualRekkaAttack._correctInput = new List<Attack_BaseInput>();

            newBaseInput.property = newRekkaProperty;
            newRekkaAttack.individualRekkaAttack._correctInput.Add(newBaseInput);
            newRekkaInput._rekkaPortion.Add(newRekkaAttack);
            propertyList.Add(newRekkaProperty);
            newRekkaInput._rekkaProperties.Add(newRekkaProperty);
            attackAnimList.Add(newRekkaInput._rekkaProperties[i].AttackAnims);
        }
        newRekkaMove.rekkaInput = newRekkaInput;

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList, null, newRekkaMove, null, null, null, null, propertyList.Count, newRekkaMove.RekkaSpecialAttack_Name);
        editedMoveList.rekkaSpecials.Insert(0, newRekkaMove);
        _attackData = newCompositeAttackData;
    }
    void DisplayRekkaMoveData(Attack_RekkaSpecialMove specialAttack, string specialName, int propertyIndex)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        propertyList.Add(specialAttack.rekkaInput.mainAttackProperty);
        attackAnimList.Add(specialAttack.rekkaInput.mainAttackProperty.AttackAnims);
        for (int i = 0; i < specialAttack.rekkaInput._rekkaPortion.Count; i++)
        {
            Attack_BaseProperties currentProperty = specialAttack.rekkaInput._rekkaPortion[i].individualRekkaAttack._correctInput[0].property;
            propertyList.Add(currentProperty);
            attackAnimList.Add(currentProperty.AttackAnims);
        }
        Attack_RekkaSpecialMove newRekkaData = specialAttack;
        newRekkaData.RekkaSpecialAttack_Name = specialName;
        _attackData = new CompositeAttackData(propertyList, attackAnimList, null, newRekkaData, null, null, null, null, propertyList.Count, newRekkaData.RekkaSpecialAttack_Name);
    }
    #endregion

    #region Special Move Function Section
    void ShowSpecialAttacks()
    {
        #region String Attack Display
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
                    DisplaySpecialMoveData(move, move.BasicSpecialAttack_Name);
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
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();


        Attack_BaseProperties newProperty = new Attack_BaseProperties();
        newProperty._attackName = $"Special Attack Entry";
        propertyList.Add(newProperty);
        newSpecialMove.property = newProperty;
        newSpecialMove.attackInput = new List<Attack_Input>();
        for (int i = 0; i < 3; i++)
        {
            newSpecialMove.attackInput.Add(new Attack_Input("", ("").ToCharArray()));
        }
        attackAnimList.Add(newProperty.AttackAnims);
        newSpecialMove.BasicSpecialAttack_Name = "New Special Attack";
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList,null, null, null, newSpecialMove, null, null, propertyList.Count, newSpecialMove.BasicSpecialAttack_Name);
        editedMoveList.special_Simple.Insert(0, newSpecialMove);
        _attackData = newCompositeAttackData;
    }
    void DisplaySpecialMoveData(Attack_BasicSpecialMove specialAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { specialAttack.property };
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        Attack_BasicSpecialMove newSpecialMoveData = specialAttack;
        attackAnimList.Add(newSpecialMoveData.property.AttackAnims);
        specialAttack.BasicSpecialAttack_Name = attackName;
        _attackData = new CompositeAttackData(propertyList, attackAnimList, null, null, null, newSpecialMoveData, null, null, 1, specialAttack.BasicSpecialAttack_Name);
    }
    #endregion

    #region String Attack Function Section
    void ShowStringNormalAttacks()
    {
        #region String Attack Display
        GUILayout.Label("String Attacks");
        stringNormalCount = EditorGUILayout.IntField("Set Attack Strings Count", stringNormalCount, GUILayout.Width(MoveListBodyObject.editorRect.width / 2f), GUILayout.Height(20));
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
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        Attack_NonSpecialAttack newStringEntry = new Attack_NonSpecialAttack();
        Attack_BasicInput newBasicInput = new Attack_BasicInput();
        newBasicInput._correctInput = new List<Attack_BaseInput>();
        for (int i = 0; i < stringNormalCount; i++)
        {
            Attack_BaseInput newBaseInput = new Attack_BaseInput();
            Attack_BaseProperties newProperty = new Attack_BaseProperties();
            newProperty._attackName = $"String Attack Entry({i + 1})";
            newBaseInput.property = newProperty;
            newBasicInput._correctInput.Add(newBaseInput);
            propertyList.Add(newProperty);
            newStringAttackData.Add(newStringEntry);
            attackAnimList.Add(newBaseInput.property.AttackAnims);
        }
        newStringEntry.SpecialAttackName = "New String Attack";
        newStringEntry._attackInput = newBasicInput;
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList,null, null, null, null, null, newStringAttackData, 1, newStringEntry.SpecialAttackName);
        editedMoveList.stringNormalAttacks.Insert(0, newStringEntry);
        _attackData = newCompositeAttackData;
    }
    void DisplayStringNormalData(Attack_NonSpecialAttack simpleAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>();
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        for (int i = 0; i < simpleAttack._attackInput._correctInput.Count; i++)
        {
            propertyList.Add(simpleAttack._attackInput._correctInput[i].property);
            attackAnimList.Add(simpleAttack._attackInput._correctInput[i].property.AttackAnims);
        }
        simpleAttack.SpecialAttackName = attackName;
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>() { simpleAttack };
        _attackData = new CompositeAttackData(propertyList, attackAnimList, null, null, null, null, null, newNonSpecialAttackData, propertyList.Count, simpleAttack.SpecialAttackName);
    }
    #endregion

    #region Command Attack Function Section
    void ShowCommandNormalAttacks()
    {
        #region Command Attack Display
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
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
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
        attackAnimList.Add(newCommandAttackData[0]._attackInput._correctInput[0].property.AttackAnims);
        newNormalEntry.SpecialAttackName = "New Command Attack";

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList,null, null, null, null, null, newCommandAttackData, 1, newNormalEntry.SpecialAttackName);

        editedMoveList.commandNormalAttacks.Insert(0, newNormalEntry);
        _attackData = newCompositeAttackData;
    }
    void DisplayCommandNormalData(Attack_NonSpecialAttack simpleAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { simpleAttack._attackInput._correctInput[0].property };
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>() { simpleAttack };
        attackAnimList.Add(newNonSpecialAttackData[0]._attackInput._correctInput[0].property.AttackAnims);
        simpleAttack.SpecialAttackName = attackName;
        _attackData = new CompositeAttackData(propertyList, attackAnimList,null, null, null, null, null, newNonSpecialAttackData, 1, simpleAttack.SpecialAttackName);
    }
    #endregion

    #region Normal Attack Function Section
    void ShowNormalAttacks()
    {
        #region Normal Attack Display
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
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
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
        attackAnimList.Add(newNonSpecialAttackData[0]._attackInput._correctInput[0].property.AttackAnims);
        newNormalEntry.SpecialAttackName = "New Command Attack";

        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList, null, null, null, null, null, newNonSpecialAttackData, 1, newNormalEntry.SpecialAttackName);

        editedMoveList.simpleAttacks.Insert(0, newNormalEntry);
        _attackData = newCompositeAttackData;
    }
    void DisplaySimpleAttackData(Attack_NonSpecialAttack simpleAttack, string attackName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { simpleAttack._attackInput._correctInput[0].property };
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        List<Attack_NonSpecialAttack> newNonSpecialAttackData = new List<Attack_NonSpecialAttack>() { simpleAttack };
        attackAnimList.Add(newNonSpecialAttackData[0]._attackInput._correctInput[0].property.AttackAnims);
        simpleAttack.SpecialAttackName = attackName;
        _attackData = new CompositeAttackData(propertyList, attackAnimList,null, null, null, null, null, newNonSpecialAttackData, 1, simpleAttack.SpecialAttackName);
    }
    #endregion

    #region Throw Function Section
    void ShowThrowAttacks()
    {
        #region Throw Display
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
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        Attack_BasicInput newBasicInput = new Attack_BasicInput();
        Attack_BaseInput newBaseInput = new Attack_BaseInput();
        Attack_BaseProperties newProperty = new Attack_BaseProperties();

        newProperty._attackName = "New Throw Attack Entry";
        newBaseInput.property = newProperty;
        newBasicInput._correctInput = new List<Attack_BaseInput> { newBaseInput };
        _newThrowBase._attackInput = newBasicInput;
        propertyList.Add(newProperty);

        AttackHandler_Attack newAttackHandler = new AttackHandler_Attack();
        _newThrowBase._throwAnimation = new List<AttackHandler_Attack>() { newAttackHandler };
        attackAnimList.Add(newProperty.AttackAnims);
        for(int i = 0; i < _newThrowBase._throwAnimation.Count; i++) 
        {
            attackAnimList.Add(_newThrowBase._throwAnimation[i]);
        }
        editedMoveList.BasicThrows.Insert(0, _newThrowBase);
        _newThrowBase.ThrowName = "New Main Throw Data";
        CompositeAttackData newCompositeAttackData = new CompositeAttackData(propertyList, attackAnimList, null, null, null, null, _newThrowBase, null, 1, _newThrowBase.ThrowName);
        _attackData = newCompositeAttackData;
    }
    void DisplayThrowData(Attack_ThrowBase throwAttack, string throwName)
    {
        List<Attack_BaseProperties> propertyList = new List<Attack_BaseProperties>() { throwAttack._attackInput._correctInput[0].property };
        List<AttackHandler_Attack> attackAnimList = new List<AttackHandler_Attack>();
        attackAnimList.Add(throwAttack._attackInput._correctInput[0].property.AttackAnims);
        for(int i = 0; i < throwAttack._throwAnimation.Count; i++) 
        {
            attackAnimList.Add(throwAttack._throwAnimation[0]);
        }
        throwAttack.ThrowName = throwName;
        _attackData = new CompositeAttackData(propertyList, attackAnimList,null, null, null, null, throwAttack, null, 1, throwAttack.ThrowName);
    }
    #endregion

    #endregion

    #region Center Page Information

    Vector2 attackPropertyAnimView;
    public void DisplayAttackInfomation()
    {
        if (_attackData != null)
        {
            displayCurrentAttackList = EditorGUILayout.Foldout(displayCurrentAttackList, "Show Attack Property List");
            if (displayCurrentAttackList)
            {
                attackPropertyAnimView = EditorGUILayout.BeginScrollView(attackPropertyAnimView, GUILayout.Width(CurrentAttackBodyObject.editorRect.width/2f), GUILayout.Height(250));

                for (int i = 0; i < _attackData.basePropertyCount; i++)
                {
                    var move = _attackData.baseAttackProperties[i];
                    if (move == null) continue;
                    string attackName = _attackData.baseAttackProperties[i]._attackName;
                    var style = i == highlightedAttackIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                    if (GUILayout.Button($"{attackName}_Property {i + 1}", style, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 3f), GUILayout.Height(25)))
                    {
                        previewActive = false;
                        /*if (i > 0)
                        {
                            subAttackDataIndex = i;
                        }
                        else { subAttackDataIndex = -1; }*/
                        subAttackDataIndex = i;
                        currentCenterAttackData = _attackData.baseAttackProperties[i];
                        currentFrame = 0;
                        isPlaying = false;
                    }
                }
                for (int i = 0; i < _attackData.baseAttackAnimations.Count; i++)
                {
                    var move = _attackData.baseAttackAnimations[i];
                    if (move == null) continue;
                    var style = i == highlightedAttackIndex ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                    string attackName = _attackData.baseAttackAnimations[i].animClip != null ? _attackData.baseAttackAnimations[i].animClip.name : $"BaseAnim_{i+1}";
                    if (GUILayout.Button($"Override to {attackName}", style, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 2.15f), GUILayout.Height(25)))
                    {
                        newAttackAnim = _attackData.baseAttackAnimations[i];
                        _currentAnimClip = newAttackAnim.animClip;
                        currentFrame = 0;
                        isPlaying = false;
                    }
                }
                EditorGUILayout.EndScrollView();
                if (moveListData != null && currentCenterAttackData != null)
                {
                    if (characterModel != null && characterAnimator != null)
                    {
                        DisplayPreviewWindow();
                    }
                }
            }
            if (moveListData != null && currentCenterAttackData != null)
            {
                ShowAnimationInformation(newAttackAnim);
                GUILayout.Space(10);
                DisplayAnimationTimeline(newAttackAnim);

                if (characterModel != null && characterAnimator != null)
                {
                    CreateOrUpdatePreviewInstance(newAttackAnim);
                    UpdatePreviewInstance(newAttackAnim);
                    OnEditorUpdate();
                }
            }
            GUILayout.Space(25);
            if (GUILayout.Button("Clear Current Attack Info", GUILayout.Width(155), GUILayout.Height(25)))
            {
                currentCenterAttackData = null;
                newAttackAnim = null;
                _attackData = null;
                _currentAnimClip = null;
            }
        }
    }
    void DisplayAnimationTimeline(AttackHandler_Attack newAttackAnim)
    {
        #region AnimSlider
        if (newAttackAnim != null)
        {
            if (newAttackAnim.animClip != null)
            {
                GUILayout.Label($"Current Animation Frame: {currentFrame}");

                #region Slider Region
                float clipLength = newAttackAnim.animClip.length;
                float currentClipLength = clipLength * fps;
                currentFrame = (int)EditorGUILayout.Slider("Animation Frame Timeline:", currentFrame, 0f, currentClipLength, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 2f), GUILayout.Height(20));


                DrawTimelineControls();
                init = (int)EditorGUILayout.Slider("Init:", init, 0f, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                startup = (int)EditorGUILayout.Slider("Startup:", startup, init, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                active = (int)EditorGUILayout.Slider("Active:", active, startup, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                inactive = (int)EditorGUILayout.Slider("Inactive:", inactive, active, currentClipLength, GUILayout.Width(500), GUILayout.Height(20));
                recoveryAmount = (int)EditorGUILayout.Slider("Recovery Amount:", recoveryAmount, 0f, 100f, GUILayout.Width(500), GUILayout.Height(20));
                newAttackAnim._frameData.init = init;
                newAttackAnim._frameData.startup = startup;
                newAttackAnim._frameData.active = active;
                newAttackAnim._frameData.inactive = inactive;
                newAttackAnim._frameData.recoveryAmount = recoveryAmount;
                FrameData curFrameData = newAttackAnim._frameData;
                if (curFrameData != null)
                {
                    List<ExtraFrameHitPoints> curExtraPoints = curFrameData._extraPoints;
                    if (curExtraPoints != null)
                    {
                        if (curExtraPoints.Count > 0)
                        {
                            extraFramePointCount = newAttackAnim._frameData._extraPoints.Count;
                        }
                    }
                }
                DisplayExtraFramePoints(newAttackAnim);

                GUILayout.Space(25);

                #endregion
            }
        }
        #endregion
    }
    int extraFramePointCount;
    bool showExtraFramePointVariables;
    Vector2 extraFramePointScrollWheel;
    int lastCount;
    void DisplayExtraFramePoints(AttackHandler_Attack attackAnim)
    {
        extraFramePointCount = (int)EditorGUILayout.Slider("ExtraFramePoint Count:", extraFramePointCount, 0, 15, GUILayout.Width(500), GUILayout.Height(20));
        if (lastCount != extraFramePointCount)
        {
            List<DisplayExtraFramePoint> newHitPointList = new List<DisplayExtraFramePoint>();
            if (attackAnim._frameData._extraPoints != null)
            {
                if (attackAnim._frameData._extraPoints.Count > 0)
                {
                    if (extraFramePointCount > lastCount)
                    {
                        if (attackAnim._frameData._extraPoints.Count > 0)
                        {
                            for (int i = 0; i < extraFramePointCount; i++)
                            {
                                newHitPointList.Add(new DisplayExtraFramePoint(false, attackAnim._frameData._extraPoints[i]));
                            }
                            attackAnimExtraPoints = newHitPointList;
                            attackAnim._frameData._extraPoints = new List<ExtraFrameHitPoints>();
                            for (int i = 0; i < newHitPointList.Count; i++)
                            {
                                attackAnim._frameData._extraPoints.Add(newHitPointList[i]._extraFrameHitPoint);
                            }
                        }
                        else
                        {
                            int finalAdditionCount = extraFramePointCount - attackAnim._frameData._extraPoints.Count;
                            for (int i = 0; i < finalAdditionCount; i++)
                            {
                                DisplayExtraFramePoint newDisplayPoint = new DisplayExtraFramePoint(false, new ExtraFrameHitPoints());
                                attackAnimExtraPoints.Add(newDisplayPoint);
                                attackAnim._frameData._extraPoints.Add(new ExtraFrameHitPoints());
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < extraFramePointCount; i++)
                        {
                            newHitPointList.Add(new DisplayExtraFramePoint(false, attackAnim._frameData._extraPoints[i]));
                        }
                        attackAnimExtraPoints = newHitPointList;
                        attackAnim._frameData._extraPoints = new List<ExtraFrameHitPoints>();
                        for (int i = 0; i < newHitPointList.Count; i++)
                        {
                            attackAnim._frameData._extraPoints.Add(newHitPointList[i]._extraFrameHitPoint);
                        }
                    }
                }
                else
                {
                    attackAnimExtraPoints = new List<DisplayExtraFramePoint>();
                    attackAnim._frameData._extraPoints = new List<ExtraFrameHitPoints>();
                    for (int i = 0; i < extraFramePointCount; i++)
                    {
                        DisplayExtraFramePoint newDisplayPoint = new DisplayExtraFramePoint(false, new ExtraFrameHitPoints());
                        attackAnimExtraPoints.Add(newDisplayPoint);
                        attackAnim._frameData._extraPoints.Add(attackAnimExtraPoints[i]._extraFrameHitPoint);
                    }
                }
            }
            else
            {
                attackAnim._frameData._extraPoints = new List<ExtraFrameHitPoints>();
                attackAnimExtraPoints = new List<DisplayExtraFramePoint>();
                for (int i = 0; i < extraFramePointCount; i++)
                {
                    attackAnim._frameData._extraPoints.Add(new ExtraFrameHitPoints());
                    DisplayExtraFramePoint newDisplayPoint = new DisplayExtraFramePoint(false, new ExtraFrameHitPoints());
                    attackAnimExtraPoints.Add(newDisplayPoint);
                }
            }
            lastCount = extraFramePointCount;
        }
        showExtraFramePointVariables = EditorGUILayout.Foldout(showExtraFramePointVariables, "Show Extra Frame Point Variables");
        if (showExtraFramePointVariables)
        {
            extraFramePointScrollWheel = EditorGUILayout.BeginScrollView(extraFramePointScrollWheel, GUILayout.Width(850), GUILayout.Height(250));
            if (attackAnim._frameData._extraPoints != null && attackAnim._frameData._extraPoints.Count > 0)
            {
                for (int i = 0; i < attackAnimExtraPoints.Count; i++)
                {
                    GUILayout.Label($"Extra Frame Point {i + 1}");
                    attackAnimExtraPoints[i].isDisplayed = EditorGUILayout.Foldout(attackAnimExtraPoints[i].isDisplayed, $"Display Extra Frame Point {i+1}");
                    if (attackAnimExtraPoints[i].isDisplayed)
                    {
                        DisplayIndividualExtraPoint(attackAnimExtraPoints[i]._extraFrameHitPoint);
                        attackAnim._frameData._extraPoints[i] = attackAnimExtraPoints[i]._extraFrameHitPoint;
                    }
                    GUILayout.Label("______________________________________________________________________________________________________________________________________________________");
                    GUILayout.Space(25);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
    void DisplayIndividualExtraPoint(ExtraFrameHitPoints framePointI) 
    {
        framePointI.hitFramePoints = (int)EditorGUILayout.FloatField("Hit Frame Point:", framePointI.hitFramePoints,GUILayout.Width(500), GUILayout.Height(20));
        framePointI.call = (HitPointCall)EditorGUILayout.EnumFlagsField("Individual Frame Call:", framePointI.call, GUILayout.Width(500), GUILayout.Height(20));
        if(framePointI.awaitEnum == null) 
        { 
            framePointI.awaitEnum = new AwaitClass();
        }
        framePointI.awaitEnum.keyRef = (WaitingEnumKey)EditorGUILayout.EnumPopup("On Await End Call:", framePointI.awaitEnum.keyRef, GUILayout.Width(500), GUILayout.Height(20));
        framePointI.awaitEnum.awaitingCheck = (HitPointCall)EditorGUILayout.EnumFlagsField("On Await End Call:", framePointI.awaitEnum.awaitingCheck, GUILayout.Width(500), GUILayout.Height(20));
        framePointI.Force = (float)EditorGUILayout.FloatField("Force:", framePointI.Force, GUILayout.Width(500), GUILayout.Height(20));
        framePointI.projectileSpeed = (float)EditorGUILayout.FloatField("Projectile Speed:", framePointI.projectileSpeed, GUILayout.Width(500), GUILayout.Height(20));
        framePointI.camPos = EditorGUILayout.Vector3Field("Camera Position:", framePointI.camPos);
        framePointI.camRotation = EditorGUILayout.Vector3Field("Camera Rotation:", framePointI.camRotation);
        framePointI.snapMovement = (bool)EditorGUILayout.Toggle("Snap Movement:", framePointI.snapMovement);
        if (framePointI.customDamage == null) 
        {
            framePointI.customDamage = new CustomDamageField();
            framePointI.customDamage.customDamageFieldStunValues = new Attack_StunValues();
        }
        framePointI.customDamage.rawAttackDamage = (float)EditorGUILayout.FloatField("Raw Damage:", framePointI.customDamage.rawAttackDamage, GUILayout.Width(500), GUILayout.Height(20));
        framePointI.customDamage.counterHitDamageMult = (float)EditorGUILayout.FloatField("Counter Hit Multiplier:", framePointI.customDamage.counterHitDamageMult, GUILayout.Width(500), GUILayout.Height(20));
        framePointI.customDamage.isScaling = (bool)EditorGUILayout.Toggle("Is Scaling:", framePointI.customDamage.isScaling);
        framePointI.customDamage.isFinalAttack = (bool)EditorGUILayout.Toggle("Is Final Attack:", framePointI.customDamage.isFinalAttack);

        GUILayout.Space(25);
        Attack_StunValues stunValues = currentCenterAttackData.attackMainStunValues;
        stunValues.hitstunValue = (int)EditorGUILayout.Slider("Hit Stun:", stunValues.hitstunValue,0,50, GUILayout.Width(500), GUILayout.Height(20));
        stunValues.blockStunValue = (int)EditorGUILayout.Slider("Block Stun:", stunValues.blockStunValue, 0, 50, GUILayout.Width(500), GUILayout.Height(20));
        stunValues.hitstopValue = (int)EditorGUILayout.Slider("Hit Stop:", stunValues.hitstopValue, 0, 50, GUILayout.Width(500), GUILayout.Height(20));
        stunValues.blockStopValue = (int)EditorGUILayout.Slider("Block Stop:", stunValues.blockStopValue, 0, 50, GUILayout.Width(500), GUILayout.Height(20));

        framePointI._hurtboxType = (HurtBoxType)EditorGUILayout.EnumPopup("Hurt Box Type:", framePointI._hurtboxType, GUILayout.Width(500), GUILayout.Height(20));
        if (framePointI.customDamage.lateralKBP == null)
        {
            framePointI.customDamage.lateralKBP = new Horizontal_KnockBack();
        }
        if (framePointI.customDamage.verticalKBP == null)
        {
            framePointI.customDamage.verticalKBP = new Vertical_KnockBack();
        }
        Horizontal_KnockBack LateralKnockBackData = framePointI.customDamage.lateralKBP;
        Vertical_KnockBack VerticalKnockBackData = framePointI.customDamage.verticalKBP;

        LateralKnockBackData.Hit_HKB_Level = (Attack_KnockBack_Lateral)EditorGUILayout.EnumPopup("Hit Knockback", LateralKnockBackData.Hit_HKB_Level);
        LateralKnockBackData.Hit_Value = (float)EditorGUILayout.FloatField("Knockback Value:", LateralKnockBackData.Hit_Value);
        LateralKnockBackData.Block_HKB_Level = (Attack_KnockBack_Lateral)EditorGUILayout.EnumPopup("Block Knockback", LateralKnockBackData.Block_HKB_Level);
        LateralKnockBackData.Block_Value = (float)EditorGUILayout.FloatField("Block Knockback Value:", LateralKnockBackData.Block_Value);
        GUILayout.Space(25);
        VerticalKnockBackData.Hit_VKB_Level = (Attack_KnockBack_Vertical)EditorGUILayout.EnumPopup("Hit KnockUp/Down", VerticalKnockBackData.Hit_VKB_Level);
        VerticalKnockBackData.Hit_Value = (float)EditorGUILayout.FloatField("KnockUp Value:", VerticalKnockBackData.Hit_Value);
        VerticalKnockBackData.Block_VKB_Level = (Attack_KnockBack_Vertical)EditorGUILayout.EnumPopup("Block KnockUp/Down", VerticalKnockBackData.Block_VKB_Level);
        VerticalKnockBackData.Block_Value = (float)EditorGUILayout.FloatField("Block KnockUp Value:", VerticalKnockBackData.Block_Value);
        framePointI.customDamage.KnockDown = (Attack_KnockDown)EditorGUILayout.EnumPopup("Knockdown", framePointI.customDamage.KnockDown);
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

            UpdatePreviewInstance(newAttackAnim);
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
            }
            UpdatePreviewInstance(newAttackAnim);
            Repaint();
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
    private void CreateOrUpdatePreviewInstance(AttackHandler_Attack newAttackAnim)
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

        if (newAttackAnim.animClip != null)
        {
            _currentAnimClip = newAttackAnim.animClip;
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
        previewRect.x = CurrentAttackBodyObject.editorRect.width/1.95f;
        previewRect.y = CurrentAttackBodyObject.editorRect.height/2.95f;
        previewRect.width = 500f;
        previewRect.height = 500f;
        showPreview = EditorGUILayout.Foldout(showPreview, "Preview Settings");
        if (showPreview)
        {
            EditorGUI.indentLevel++;
            previewCameraFOV = EditorGUILayout.Slider("Camera FOV", previewCameraFOV, 10, 90,GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 1.95f), GUILayout.Height(20));
            previewCameraPositionOffset = EditorGUILayout.Vector3Field("Camera Offset", previewCameraPositionOffset, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 1.15f), GUILayout.Height(20));
            EditorGUI.indentLevel--;
            GUILayout.Space(50);
        }
        DrawPreview(previewRect);
        Repaint();
    }
    void UpdatePreviewInstance(AttackHandler_Attack newAttackAnim) 
    {
        if (newAttackAnim != null) 
        {
            if (newAttackAnim.animClip == null || previewInstance == null)
                return;

            var animator = previewInstance.GetComponentInChildren<Animator>();
            if (animator == null)
                return;

            if (animator.HasState(0, Animator.StringToHash("PreviewState")))
            {
                float normalizedTime = Mathf.Clamp01(timelineCurrentTime / newAttackAnim.animClip.length);
                animator.Play("PreviewState", 0, normalizedTime);
                animator.speed = 0;
                animator.Update(0);
            }
            else
            {
                Debug.LogWarning("PreviewState not found in animator controller!");
            }
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
            if (_moveListEditState ==  MoveListEditMode.MainAttackCollisionMode) 
            {
                DrawCollisionBoxesInPreview();
            }
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

    private void DrawCollisionBoxesInPreview()
    {
        if (newAttackAnim != null && _currentAnimClip != null)
        {
            Mesh cubeMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            string modelInPreviewName = $"{characterModel.name}(Clone)";
            if (currentFrame >= newAttackAnim._frameData.startup && currentFrame <= (newAttackAnim._frameData.inactive+ newAttackAnim._frameData.recoveryAmount))
            {
                Vector3 position = new Vector3(newAttackAnim.hu_placement.x, newAttackAnim.hu_placement.y + 1, newAttackAnim.hu_placement.z);// parent.TransformPoint(newAttackAnim.hu_placement);
                Vector3 size = new Vector3(newAttackAnim.hu_size.x, newAttackAnim.hu_size.y, 0.02f);

                previewUtility.DrawMesh(cubeMesh, Matrix4x4.TRS(position, Quaternion.identity, size), hurtboxMat, 0);
                Color32 hurtboxColor = hurtboxColorList[(int)newAttackAnim.hurtType];
                hurtboxMat.color = hurtboxColor;
                DrawWireframeCube(newAttackAnim.hu_placement, size,hurtboxMat);
            }

            if (currentFrame >= newAttackAnim._frameData.active && currentFrame <= newAttackAnim._frameData.inactive)
            {
                Vector3 position = new Vector3(newAttackAnim.hb_placement.x, newAttackAnim.hb_placement.y + 1, newAttackAnim.hb_placement.z);
                Vector3 size = new Vector3(newAttackAnim.hb_size.x, newAttackAnim.hb_size.y, 0.02f);

                previewUtility.DrawMesh(cubeMesh, Matrix4x4.TRS(position, Quaternion.identity, size), hitboxMat, 0);
                Color32 hitboxColor = hitboxColorList[(int)newAttackAnim.attackType];
                hitboxMat.color = hitboxColor;
                DrawWireframeCube(newAttackAnim.hb_placement, size,hitboxMat);
            }
        }
    }
    private void DrawWireframeCube(Vector3 position, Vector3 size, Material curMat)
    {
        Vector3 half = size * 0.5f;
        Vector3[] vertices = new Vector3[]
        {
            position + new Vector3(-half.x, -half.y, -half.z),
            position + new Vector3(half.x, -half.y, -half.z),
            position + new Vector3(half.x, half.y, -half.z),
            position + new Vector3(-half.x, half.y, -half.z),
            position + new Vector3(-half.x, -half.y, half.z),
            position + new Vector3(half.x, -half.y, half.z),
            position + new Vector3(half.x, half.y, half.z),
            position + new Vector3(-half.x, half.y, half.z)
        };

        int[] lines = { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };

        GL.PushMatrix();
        GL.MultMatrix(previewUtility.camera.worldToCameraMatrix);
        curMat.SetPass(0);
        GL.Begin(GL.LINES);
        foreach (int i in lines)
        {
            GL.Vertex(vertices[i]);
        }
        GL.End();
        GL.PopMatrix();
    }
    #endregion
    void ShowAnimationInformation(AttackHandler_Attack newAttackAnim)
    {
        characterModel = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Player Object"), characterModel, typeof(GameObject), true, GUILayout.Width(500), GUILayout.Height(20));
        characterAnimator = (RuntimeAnimatorController)EditorGUILayout.ObjectField(new GUIContent("Object Animator"), characterAnimator, typeof(RuntimeAnimatorController), true, GUILayout.Width(500), GUILayout.Height(20));
        _currentAnimClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Current Attack Animation:"), _currentAnimClip, typeof(AnimationClip), true, GUILayout.Width(CurrentAttackBodyObject.editorRect.width / 1.95f), GUILayout.Height(20));
        if(_currentAnimClip != null) 
        {
            newAttackAnim.animClip = _currentAnimClip;
        }
        if (newAttackAnim != null)
        {
            if (_currentAnimClip != null)
            {
                if (lastClip != _currentAnimClip)
                {
                    lastClip = _currentAnimClip;
                    _currentAnimClip = newAttackAnim.animClip;
                    init = newAttackAnim._frameData.init;
                    startup = newAttackAnim._frameData.startup;
                    active = newAttackAnim._frameData.active;
                    inactive = newAttackAnim._frameData.inactive;
                    recoveryAmount = newAttackAnim._frameData.recoveryAmount;
                }
            }
            else
            {
                lastClip = _currentAnimClip;
                _currentAnimClip = newAttackAnim.animClip;
                init = newAttackAnim._frameData.init;
                startup = newAttackAnim._frameData.startup;
                active = newAttackAnim._frameData.active;
                inactive = newAttackAnim._frameData.inactive;
                recoveryAmount = newAttackAnim._frameData.recoveryAmount;
            }
            /*if (_currentAnimClip != newAttackAnim.animClip)
            {
                if (newAttackAnim.animClip != null)
                {
                    _currentAnimClip = newAttackAnim.animClip;
                }
                else 
                {
                    if(_currentAnimClip != null) 
                    {
                        newAttackAnim.animClip = _currentAnimClip;
                    }
                }
            }
            else
            {
                if (_currentAnimClip != null)
                {
                    newAttackAnim.animClip = _currentAnimClip;
                }
            }*/

        }
    }
    #endregion

    #region Right Page Information
    Vector2 rightPageScrollVector;
    void ShowRightPageAttackInformation() 
    {
        GUILayout.Space(50);
        rightPageScrollVector = EditorGUILayout.BeginScrollView(rightPageScrollVector, GUILayout.Width(CurrentAttackInformationObject.editorRect.width), GUILayout.Height(900));
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
            DisplayMainInformation(currentCenterAttackData);
        }
        EditorGUILayout.EndScrollView();
    }
    void DisplayMainInformation(Attack_BaseProperties newProperty) 
    {
        newProperty._attackName = (string)EditorGUILayout.TextField("Attack Name:", newProperty._attackName);
        newProperty.rawAttackDamage = (float)EditorGUILayout.FloatField("Raw Damage:", newProperty.rawAttackDamage);
        newProperty.counterHitDamageMult = (float)EditorGUILayout.FloatField("Counter Hit Multiplier:", newProperty.counterHitDamageMult);

        GUILayout.Space(25);
        Attack_StunValues stunValues = newProperty.attackMainStunValues;
        stunValues.hitstunValue = (int)EditorGUILayout.Slider("Hit Stun:", stunValues.hitstunValue, 0, 50);
        stunValues.blockStunValue = (int)EditorGUILayout.Slider("Block Stun:", stunValues.blockStunValue, 0, 50);
        stunValues.hitstopValue = (int)EditorGUILayout.Slider("Hit Stop:", stunValues.hitstopValue, 0, 50);
        stunValues.blockStopValue = (int)EditorGUILayout.Slider("Block Stop:", stunValues.blockStopValue, 0, 50);

        GUILayout.Space(25);
        newProperty.hitLevel = (HitLevel)EditorGUILayout.EnumFlagsField("Hit Level", newProperty.hitLevel);

        GUILayout.Space(25);
        newProperty._meterRequirement = (int)EditorGUILayout.FloatField("Meter Requirement:", newProperty._meterRequirement);
        newProperty._meterAwardedOnHit = (int)EditorGUILayout.FloatField("Meter Awarded:", newProperty._meterAwardedOnHit);
        newProperty.attackScalingPercent = (int)EditorGUILayout.FloatField("Attack Scaling:", newProperty.attackScalingPercent);

        newProperty.dashCancelable = (bool)EditorGUILayout.Toggle("Dash Cancelable:", newProperty.dashCancelable);
        newProperty.JumpCancelable = (bool)EditorGUILayout.Toggle("Jump Cancelable:", newProperty.JumpCancelable);

        GUILayout.Space(25);
        newProperty._airInfo = (AirAttackInfo)EditorGUILayout.EnumFlagsField("Air Properties", newProperty._airInfo);

        Attack_CancelInfo _cancelProperty = newProperty.cancelProperty;
        GUILayout.Space(25);
        _cancelProperty.CurrentLevel = (Cancel_State)EditorGUILayout.EnumFlagsField("Attack Cancel Level", _cancelProperty.CurrentLevel);
        _cancelProperty.nextAvailableAttackRoute = (Cancel_State)EditorGUILayout.EnumFlagsField("Next Cancel State", _cancelProperty.nextAvailableAttackRoute);
        newProperty._moveType = (MoveType)EditorGUILayout.EnumPopup("Move Type", newProperty._moveType);

        GUILayout.Space(25);
        Horizontal_KnockBack LateralKnockBackData = newProperty.LateralKB_Data;
        Vertical_KnockBack VerticalKnockBackData = newProperty.VerticalKB_Data;

        LateralKnockBackData.Hit_HKB_Level = (Attack_KnockBack_Lateral)EditorGUILayout.EnumPopup("Hit Knockback", LateralKnockBackData.Hit_HKB_Level);
        LateralKnockBackData.Hit_Value = (float)EditorGUILayout.FloatField("Knockback Value:", LateralKnockBackData.Hit_Value);
        LateralKnockBackData.Block_HKB_Level = (Attack_KnockBack_Lateral)EditorGUILayout.EnumPopup("Block Knockback", LateralKnockBackData.Block_HKB_Level);
        LateralKnockBackData.Block_Value = (float)EditorGUILayout.FloatField("Block Knockback Value:", LateralKnockBackData.Block_Value);
        GUILayout.Space(25);
        VerticalKnockBackData.Hit_VKB_Level = (Attack_KnockBack_Vertical)EditorGUILayout.EnumPopup("Hit KnockUp/Down", VerticalKnockBackData.Hit_VKB_Level);
        VerticalKnockBackData.Hit_Value = (float)EditorGUILayout.FloatField("KnockUp Value:", VerticalKnockBackData.Hit_Value);
        VerticalKnockBackData.Block_VKB_Level = (Attack_KnockBack_Vertical)EditorGUILayout.EnumPopup("Block KnockUp/Down", VerticalKnockBackData.Block_VKB_Level);
        VerticalKnockBackData.Block_Value = (float)EditorGUILayout.FloatField("Block KnockUp Value:", VerticalKnockBackData.Block_Value);
        newProperty.KnockDown = (Attack_KnockDown)EditorGUILayout.EnumPopup("Knockdown", newProperty.KnockDown);
    }
    #region Specific Attack Data Display Code
    void DisplayAdvanceSpecialData() 
    {
        if (_attackData.advancedInputData != null)
        {
            GUILayout.Label("Advanced Input Primary Data");
            _attackData.advancedInputData.specialMoveName = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.advancedInputData.specialMoveName);
            for (int i = 0; i < 3; i++)
            {
                _attackData.advancedInputData.attackInput[i].attackString = (string)EditorGUILayout.TextField($"(Advanced)Special Attack Input_{i + 1}:", _attackData.advancedInputData.attackInput[i].attackString);
            }
        }
    }
    void DisplayStanceData()
    {
        if (_attackData.stanceInputData != null)
        {
            GUILayout.Label("Stance Primary Data");
            _attackData.stanceInputData.StanceSpecialAttack_Name = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.stanceInputData.StanceSpecialAttack_Name);
            for (int i = 0; i < _attackData.stanceInputData.stanceInput._stanceInput.Count; i++)
            {
                _attackData.stanceInputData.stanceInput._stanceInput[i].attackString = (string)EditorGUILayout.TextField($"Stance Attack Input_{i + 1}:", _attackData.stanceInputData.stanceInput._stanceInput[i].attackString);
            }
            _attackData.stanceInputData.stanceHeldTime = (int)EditorGUILayout.FloatField($"Stance Held Time:", _attackData.stanceInputData.stanceHeldTime);

            if (_attackData.stanceInputData.stanceInput.stanceAttack._stanceButtonInput._correctInput.Count > 0)
            {
                string stanceAttackString = _attackData.stanceInputData.stanceInput.stanceAttack._stanceButtonInput._correctInput[0]._correctSequence;
                //DisplayMainInformation(_attackData.stanceInputData.stanceInput.stanceAttack._stanceButtonInput._correctInput[0].property);
            }
            if (_attackData.stanceInputData.stanceInput.stanceKill._stanceButtonInput._correctInput.Count > 0)
            {
                string stanceAttackString = _attackData.stanceInputData.stanceInput.stanceKill._stanceButtonInput._correctInput[0]._correctSequence;
                //DisplayMainInformation(_attackData.stanceInputData.stanceInput.stanceKill._stanceButtonInput._correctInput[0].property);
            }
        }
    }
    bool displaySubAttackData;
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
            if (subAttackDataIndex > 0)
            {
                displaySubAttackData = (bool)EditorGUILayout.Foldout(displaySubAttackData,"Display Sub Rekka Data");
                if (displaySubAttackData)
                {
                    string currentHighlightedRekkaInput = _attackData.rekkaAttackData.rekkaInput._rekkaPortion[subAttackDataIndex - 1].individualRekkaAttack._correctInput[0]._correctSequence;
                    currentHighlightedRekkaInput = (string)EditorGUILayout.TextField("Rekka Individual Input:", currentHighlightedRekkaInput);
                    DisplayMainInformation(_attackData.rekkaAttackData.rekkaInput._rekkaPortion[subAttackDataIndex - 1].individualRekkaAttack._correctInput[0].property);
                }
            }
        }
    }
    void DisplaySpecialData()
    {
        if (_attackData.specialInputData != null)
        {
            GUILayout.Label("Special Primary Data");
            _attackData.specialInputData.BasicSpecialAttack_Name = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.specialInputData.BasicSpecialAttack_Name);
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
            _attackData.normalAttackData[0].SpecialAttackName = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.normalAttackData[0].SpecialAttackName);
            _attackData.normalAttackData[0]._attackInput._correctInput[subAttackDataIndex]._correctSequence = (string)EditorGUILayout.TextField("Attack Button Sequence:", _attackData.normalAttackData[0]._attackInput._correctInput[subAttackDataIndex]._correctSequence);
        }
    }
    void DisplayThrowData()
    {
        if (_attackData.throwInputData != null)
        {
            GUILayout.Label("Throw Primary Data");
            GUILayout.Label($"Throw Attack Count: {_attackData.throwInputData._attackInput._correctInput.Count}");
            _attackData.throwInputData.ThrowName = (string)EditorGUILayout.TextField("Current Attack Name:", _attackData.throwInputData.ThrowName);
           
                _attackData.throwInputData._attackInput._correctInput[0]._correctSequence = (string)EditorGUILayout.TextField("Attack Button Sequence:", _attackData.throwInputData._attackInput._correctInput[0]._correctSequence);
            
            /*AnimationClip throwAnim = _attackData.throwInputData._throwAnimation[0] != null ? _attackData.throwInputData._throwAnimation[0].animClip : null;

            throwAnim = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Current Attack Animation:"), throwAnim, typeof(AnimationClip), true, GUILayout.Width(CurrentAttackInformationObject.editorRect.width), GUILayout.Height(20));*/
        }
    }
    #endregion
    void ShowRightPageCollisionInformation()
    {
        GUILayout.Space(50);
        GUILayout.Label("Collision Information");
        //verticalBias = (float)EditorGUILayout.Slider("Vertical Bias:", verticalBias, 0f, 15f, GUILayout.Width(CurrentAttackInformationObject.editorRect.width / 2f), GUILayout.Height(20));
        if(newAttackAnim != null) { 
        newAttackAnim.attackType = (HitBoxType)EditorGUILayout.EnumPopup("Attack Type:", newAttackAnim.attackType);
        newAttackAnim.hb_placement = (Vector3)EditorGUILayout.Vector3Field("Hitbox Placement", newAttackAnim.hb_placement);
        newAttackAnim.hb_orientation = (Vector3)EditorGUILayout.Vector3Field("Hitbox Orientation", newAttackAnim.hb_orientation);
        newAttackAnim.hb_size = (Vector2)EditorGUILayout.Vector2Field("Hitbox size", newAttackAnim.hb_size);

        GUILayout.Space(25);
        newAttackAnim.hurtType = (HurtBoxType)EditorGUILayout.EnumPopup("Hurtbox Type:", newAttackAnim.hurtType);
        newAttackAnim.hu_placement = (Vector3)EditorGUILayout.Vector3Field("Hurtbox Placement", newAttackAnim.hu_placement);
        newAttackAnim.hu_orientation = (Vector3)EditorGUILayout.Vector3Field("Hurtbox Orientation", newAttackAnim.hu_orientation);
        newAttackAnim.hu_size = (Vector2)EditorGUILayout.Vector2Field("Hurtbox size", newAttackAnim.hu_size);

        GUILayout.Space(25);
        HitCount _hitCount = newAttackAnim._hitCount;
        _hitCount._startCount = (int)EditorGUILayout.FloatField("Initial Hit Count:", _hitCount._startCount);
        _hitCount._startRefreshRate = (int)EditorGUILayout.FloatField("Initial Refresh Rate:", _hitCount._startRefreshRate);
            }
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
        //GUILayout.Space(145);
        if (moveListData != null)
        {
            _moveListEditState = (MoveListEditMode)GUILayout.Toolbar((int)_moveListEditState, new[] { "Current Attack Info Editor", "Current Attack Collision Editor" }, GUILayout.Width(CurrentAttackInformationObject.editorRect.width/1f), GUILayout.Height(SaveDataObject.editorRect.height / 20f));
           
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
            EditorUtility.SetDirty(moveListData);
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
                lastMoveList = moveListData;
                newMoveList = moveListData.GetFullMoveList();
                editedMoveList = newMoveList;
                return;
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
public class DisplayExtraFramePoint
{
    public bool isDisplayed;
    public ExtraFrameHitPoints _extraFrameHitPoint;
    public DisplayExtraFramePoint(bool _displayed, ExtraFrameHitPoints _hitPoint)
    {
        isDisplayed = _displayed;
        _extraFrameHitPoint = _hitPoint;
    }
}
[Serializable]
public class CompositeAttackData
{
    public List<Attack_BaseProperties> baseAttackProperties;
    public List<AttackHandler_Attack> baseAttackAnimations;

    public int basePropertyCount;
    public Attack_AdvancedSpecialMove advancedInputData;
    public Attack_RekkaSpecialMove rekkaAttackData;
    public Attack_StanceSpecialMove stanceInputData;
    public Attack_BasicSpecialMove specialInputData;
    public Attack_ThrowBase throwInputData;
    public List<Attack_NonSpecialAttack> normalAttackData;
    public string SpecialAttackName;
    public CompositeAttackData(
        List<Attack_BaseProperties> _baseProperties,
        List<AttackHandler_Attack> _baseAttackAnimations,
        Attack_AdvancedSpecialMove _advancedInputData = null,
        Attack_RekkaSpecialMove _rekkaData = null,
        Attack_StanceSpecialMove _stanceData = null,
        Attack_BasicSpecialMove _specialData = null,
        Attack_ThrowBase _throwData = null,
        List<Attack_NonSpecialAttack> _nonSpecialData = null
        ,int _basePropertyCount = 0,
        string _specialName = "") 
    {
        baseAttackProperties = _baseProperties;
        baseAttackAnimations = _baseAttackAnimations;
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
    }
}