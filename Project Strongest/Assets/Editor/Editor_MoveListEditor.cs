using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
public class Editor_MoveListEditor : EditorWindow
{
    #region Visual Code For Windows
    /*private Texture2D MoveListHeaderSection;
    private Texture2D MoveListBodySection;
    private Texture2D CurrentAttackHeaderSection;
    private Texture2D CurrentAttackBodySection;
    private Texture2D CurrentAttackInformationSection;
    private Texture2D ToggleDataPresentationSection;


    private Color32 MoveListHeaderColor;
    private Color32 MoveListBodyColor;
    private Color32 CurrentAttackHeaderColor;
    private Color32 CurrentAttackBodyColor;
    private Color32 CurrentAttackInformationColor;
    private Color32 ToggleDataPresentationColor;

    private Rect MoveListHeaderRect;
    private Rect MoveListBodyRect;
    private Rect CurrentAttackHeaderRect;
    private Rect CurrentAttackBodyRect;
    private Rect CurrentAttackInformationRect;
    private Rect ToggleDataPresentationRect;*/
    public EditorMoveListObject MoveListHeaderObject;
    public EditorMoveListObject MoveListBodyObject;
    public EditorMoveListObject CurrentAttackHeaderObject;
    public EditorMoveListObject CurrentAttackBodyObject;
    public EditorMoveListObject CurrentAttackInformationObject;
    public EditorMoveListObject ToggleDataPresentationObject;
    #endregion

    #region Character Data
    private Character_MoveList moveListData;
    Character_MoveList editedMoveList;
    private GameObject characterModel;
    private Animator characterAnimator;
    #endregion

    #region Preview Window Code

    #endregion
    [MenuItem("Window/Roster/Characters/Edit Character Movelist")]
    static void OpenMoveListEditorWindow() 
    {
        Editor_MoveListEditor window = (Editor_MoveListEditor)GetWindow(typeof(Editor_MoveListEditor));
        window.minSize = new Vector2(1400f,650f);
        window.maxSize = new Vector2(2000f, 1200f);
    }
    void OnEnable()
    {
    }
    void OnGUI()
    {
        SetScreenSize();
        DrawLayouts();
    }
    void DrawLayouts()
    {
        DrawMoveListHeader();
        DrawCurrentAttackHeader();
    }
    void SetScreenSize() 
    {
        #region MoveList Header
        Color32 headerColor1 = new Color32((byte)40f, (byte)40f, (byte)40f, (byte)255f);
        Rect size1 = new Rect();
        size1.x = 0f;
        size1.y = 0f;
        size1.width = Screen.width-(Screen.width - (Screen.width / 6f));
        size1.height = 25f;
        MoveListHeaderObject = new EditorMoveListObject(new Texture2D(1, 1), headerColor1, size1);
        MoveListHeaderObject.editorTexture.SetPixel(0, 0, headerColor1);
        MoveListHeaderObject.editorTexture.Apply();
        #endregion

        #region MoveList Body
        Color32 mBodyColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
        Rect size2 = new Rect();
        size2.x = 0f;
        size2.y = 0f;
        size2.width = Screen.width / 3f;
        size2.height = 35f;
        MoveListBodyObject = new EditorMoveListObject(new Texture2D(1, 1), mBodyColor, size2);
        #endregion

        #region CA Header
        Color32 cAHColor = new Color32((byte)35f, (byte)35f, (byte)35f, (byte)255f);
        Rect size3 = new Rect();
        size3.x = 140f;
        size3.y = 0f;
        size3.width = Screen.width - (Screen.width / 2.75f);
        size3.height = 25f;
        CurrentAttackHeaderObject = new EditorMoveListObject(new Texture2D(1, 1), cAHColor, size3);
        CurrentAttackHeaderObject.editorTexture.SetPixel(0, 0, cAHColor);
        CurrentAttackHeaderObject.editorTexture.Apply();
        #endregion

        #region CA Body 
        Color32 cABColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
        Rect size4 = new Rect();
        size4.x = 0f;
        size4.y = 0f;
        size4.width = Screen.width / 3f;
        size4.height = 35f;
        CurrentAttackBodyObject = new EditorMoveListObject(new Texture2D(1, 1), cABColor, size4);
        #endregion

        #region CA Info 
        Color32 cAInfoColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
        Rect size5 = new Rect();
        size5.x = 0f;
        size5.y = 0f;
        size5.width = Screen.width / 3f;
        size5.height = 35f;
        CurrentAttackInformationObject = new EditorMoveListObject(new Texture2D(1, 1), cAInfoColor, size5);
        #endregion

        #region Toggle Data
        Color32 toggleDataColor = new Color32((byte)25f, (byte)25f, (byte)25f, (byte)255f);
        Rect size6 = new Rect();
        size6.x = 0f;
        size6.y = 0f;
        size6.width = Screen.width / 3f;
        size6.height = 35f;
        ToggleDataPresentationObject = new EditorMoveListObject(new Texture2D(1, 1), toggleDataColor, size6);
        #endregion
    }
    void DrawMoveListHeader() 
    {
        GUILayout.BeginArea(MoveListHeaderObject.editorRect);
        #region FillArea
        GUI.DrawTexture(MoveListHeaderObject.editorRect, MoveListHeaderObject.editorTexture);
        GUILayout.Label("Character MoveList");
        #endregion
        GUILayout.EndArea();
    }
    void DrawMoveListBody()
    {
        GUILayout.BeginArea(MoveListBodyObject.editorRect);
        #region FillArea
        GUI.DrawTexture(MoveListBodyObject.editorRect, MoveListBodyObject.editorTexture);

        #endregion
        GUILayout.EndArea();
    }
    void DrawCurrentAttackHeader()
    {
        GUILayout.BeginArea(CurrentAttackHeaderObject.editorRect);
        #region FillArea
        GUI.DrawTexture(CurrentAttackHeaderObject.editorRect, CurrentAttackHeaderObject.editorTexture);
        GUILayout.Label("Current Attack Page");
        #endregion
        GUILayout.EndArea();
    }
    void DrawCurrentAttackBody()
    {
        GUILayout.BeginArea(CurrentAttackBodyObject.editorRect);
        #region FillArea
        GUILayout.Label("Current Attack");
        GUI.DrawTexture(CurrentAttackBodyObject.editorRect, CurrentAttackBodyObject.editorTexture);
        #endregion
        GUILayout.EndArea();
    }
    void DrawCurrentAttackInfo()
    {
        GUILayout.BeginArea(CurrentAttackInformationObject.editorRect);
        #region FillArea
        GUILayout.Label("Current Attack Information");
        GUI.DrawTexture(CurrentAttackInformationObject.editorRect, CurrentAttackInformationObject.editorTexture);
        #endregion
        GUILayout.EndArea();
    }
    void DrawToggleDataHeader()
    {
        GUILayout.BeginArea(ToggleDataPresentationObject.editorRect);
        #region FillArea
        GUILayout.Label("Toggle Data");
        GUI.DrawTexture(ToggleDataPresentationObject.editorRect, ToggleDataPresentationObject.editorTexture);
        #endregion
        GUILayout.EndArea();
    }
    void GetMoveListData() 
    {
        editedMoveList = moveListData;
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