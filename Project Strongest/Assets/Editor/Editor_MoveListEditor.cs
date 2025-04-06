using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
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
    public EditorMoveListObject ToggleEditObject;
    #endregion

    #region Character Data
    private Character_MoveList moveListData;
    FullMoveList editedMoveList;
    int index;
    GenericMenu NormalFullList; 
    private GameObject characterModel;
    private Animator characterAnimator;
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
    bool displayCommandNormalAttacks = false;
    bool displayNormalAttacks = false;
    bool displayThrowAttacks = false;
    #endregion

    #region Preview Window Code

    #endregion


    [MenuItem("Window/Roster/Characters/Edit Character Movelist")]
    static void OpenMoveListEditorWindow() 
    {
        Editor_MoveListEditor window = (Editor_MoveListEditor)GetWindow(typeof(Editor_MoveListEditor));
        window.minSize = new Vector2(1600f, 1000f);
        window.maxSize = new Vector2(1600f, 1000f);
    }
    void OnEnable()
    {
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
        DrawToggleDataHeader();
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
        size3.width = 1000f;
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
        size4.width = size3.width-200f;
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
        size5.width = 400f;
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
        size6.y = Screen.height/1.05f;
        size6.width = Screen.width/3f;
        size6.height = Screen.height;
        SaveDataObject = new EditorMoveListObject(new Texture2D(1, 1), toggleDataColor, size6);
        SaveDataObject.editorTexture.SetPixel(0, 0, toggleDataColor);
        SaveDataObject.editorTexture.Apply();
        GUI.DrawTexture(SaveDataObject.editorRect, SaveDataObject.editorTexture);
        #endregion

        #region Toggle Header
        Color32 toggleColor = new Color32((byte)75f, (byte)75f, (byte)75f, (byte)255f);
        Rect size7 = new Rect();
        size7.x = size4.x * 3;
        size7.y = Screen.height / 1.05f;
        size7.width = Screen.width/4f;
        size7.height = Screen.height;
        ToggleEditObject = new EditorMoveListObject(new Texture2D(1, 1), toggleColor, size7);
        ToggleEditObject.editorTexture.SetPixel(0, 0, cAHColor);
        ToggleEditObject.editorTexture.Apply();
        GUI.DrawTexture(ToggleEditObject.editorRect, ToggleEditObject.editorTexture);
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
        #endregion
        GUILayout.EndArea();
    }
    void FillDataOnScreen() 
    {
        GetMoveListData();
        ShowSupersAttacks();
        ShowCommandGrabAttacks();
        ShowCounterAttacks();
        ShowStanceAttacks();
        ShowRekkaAttacks();
        ShowSpecialAttacks();
        ShowStringNormalAttacks();
        ShowCommandNormalAttacks();
        ShowNormalAttacks();
        ShowThrowAttacks();
        Debug.Log("Present ALL Attacks");
    }

    #region Supers Function Section
    void ShowSupersAttacks()
    {
        #region Command Grabs Display
        Debug.Log(editedMoveList.CommandThrows.Count);
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
                    DisplayRekkaMoveData(move);
                }
            }
        }
        #endregion
    }
    void AddRekkaAttackEntry()
    {

    }
    void DisplayRekkaMoveData(Attack_RekkaSpecialMove specialAttack)
    {

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
                    DisplaySpecialMoveData(move);
                }
            }
        }
        #endregion
    }
    void AddSpecialAttackEntry()
    {

    }
    void DisplaySpecialMoveData(Attack_Special_Base specialAttack)
    {

    }
    #endregion

    #region String Attack Function Section
    void ShowStringNormalAttacks()
    {
        #region String Attack Display
        Debug.Log(editedMoveList.stringNormalAttacks.Count);
        GUILayout.Label("String Attacks");
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
                    DisplayStringNormalData(move);
                }
            }
        }
        #endregion
    }
    void AddStringNormalAttackEntry()
    {

    }
    void DisplayStringNormalData(Attack_NonSpecialAttack simpleAttack)
    {

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
                    DisplayCommandNormalData(move);
                }
            }
        }
        #endregion
    }
    void AddCommandNormalAttackEntry()
    {

    }
    void DisplayCommandNormalData(Attack_NonSpecialAttack simpleAttack)
    {

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
                    DisplaySimpleAttackData(move);
                }
            }
        }
        #endregion
    }
    void AddNewNormalAttackEntry()
    {

    }
    void DisplaySimpleAttackData(Attack_NonSpecialAttack simpleAttack) 
    {

    }
    #endregion

    #region Throw Function Section
    void ShowThrowAttacks()
    {
        #region Throw Display
        Debug.Log(editedMoveList.simpleAttacks.Count);
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
                    DisplayThrowData(move);
                }
            }
        }
        #endregion
    }
    void AddNewThrowEntry()
    {

    }
    void DisplayThrowData(Attack_ThrowBase throwAttack)
    {

    }
    #endregion

    #region Show Window Data
    void DrawCurrentAttackHeader()
    {
        GUILayout.BeginArea(CurrentAttackHeaderObject.editorRect);
        #region FillArea
        if (moveListData != null)
        {
        }
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
            //newString = (string)EditorGUILayout.TextField("Character Name:", newString);
        }
        #endregion
        GUILayout.EndArea();
    }
    void DrawCurrentAttackInfo()
    {
        GUILayout.BeginArea(CurrentAttackInformationObject.editorRect);
        #region FillArea
        GUILayout.Label("Current Attack Information");
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
            if (GUILayout.Button("Save Changes?", GUILayout.Width(155), GUILayout.Height(30)))
            {
                SaveMoveListChanges();
            }
        }
        #endregion
        GUILayout.EndArea();
    }
    void DrawToggleDataHeader()
    {
        GUILayout.BeginArea(ToggleEditObject.editorRect);
        #region FillArea
        if (moveListData != null)
        {
            _moveListEditState = (MoveListEditMode)GUILayout.Toolbar((int)_moveListEditState, new[] { "Current Attack Info Editor", "Current Attack Collision Editor" });
            //if (GUILayout.Button("Save Changes?", GUILayout.Width(155), GUILayout.Height(30)))
            //{

            //}
        }
        #endregion
        GUILayout.EndArea();
    }
    #endregion

    void SaveMoveListChanges() 
    {
        Debug.Log("MoveList Changes Saved");
    }
    void GetMoveListData() 
    {
        FullMoveList newMoveList = null;
        editedMoveList = moveListData.GetFullMoveList(newMoveList);
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