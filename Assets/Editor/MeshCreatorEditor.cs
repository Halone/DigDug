using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(MeshCreator))]
public class MeshCreatorEditor : Editor {
    private Action doAction;
    private const string PATH_TEXTURES          = "Textures/";
    private const string NAME_FILE_ENEMY_PLONG  = "EnemyPlongeur";
    private const string NAME_FILE_ENEMY_DRAG   = "EnemyDragon";
    private const string NAME_FILE_PLAYER       = "Player";

    private List<Texture2D> m_Textures = new List<Texture2D>();
    private const int NbrTexturePerLine = 4;
    private int countTexture = 0;
    private bool l_Foldout;
    private Vector2 m_Texture1;
    private Vector2 m_Texture2;
    private Texture m_SpritePlongeur;
    private Texture m_SpriteDragon;
    private Texture m_SpritePlayer;
    private bool m_WorldHasBeenGenerated;
    private bool m_WorldHasBeenSaved;
    private bool m_UnitsHaveBeenPlaced;
    private string m_SelectedUnit;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        MeshCreator l_MeshCreator = (MeshCreator)target;
        GUILayout.Label("");
        GUILayout.Label("Never Save the scene, pls");
        GUILayout.Label("");

        if (doAction == null) SetModeDisplayTexture();
        doAction();
    }

    private void FillTexture(MeshCreator p_Instance)
    {
        MeshRenderer l_MeshRenderer = p_Instance.GetComponent<MeshRenderer>();
        if (l_MeshRenderer != null)
        {
            Texture2D l_Texture = l_MeshRenderer.sharedMaterial.mainTexture as Texture2D;
            if (l_Texture != null)
            {
                int l_Width = l_Texture.width;
                int l_Height = l_Texture.width;

                int l_Width1 = 64;
                int l_Height1 = 64;

                for(int x = 0; x < l_Width; x += l_Width1)
                {
                    for (int y = 0; y < l_Height; y += l_Height1)
                    {
                        var colors = l_Texture.GetPixels(x, y, l_Width1, l_Height1);
                        Texture2D l_NewTexture = new Texture2D(l_Width1, l_Height1);
                        l_NewTexture.SetPixels(colors);
                        l_NewTexture.Apply();

                        m_Textures.Add(l_NewTexture);
                    }
                }

            }
        }
    }

    #region StateMachine
    #region Set Mode
    private void SetModeDisplayTexture()
    {
        m_Texture1 = Vector3.left;
        m_Texture2 = Vector3.left;
        m_UnitsHaveBeenPlaced = ((MeshCreator)target).AreUnitsPlaced();
        doAction = DoActionDisplayTexture;
    }

    private void SetModeDisplayGenerationButton()
    {
        m_WorldHasBeenGenerated = false;
        doAction = DoActionDisplayGenerationButton;
    }

    private void SetModeDisplayUnits()
    {
        m_WorldHasBeenSaved = false;
        doAction = DoActionDisplayUnits;
    }
    #endregion
    #region Do Action
    private void DoActionDisplayTexture()
    {
        if (m_WorldHasBeenSaved ||m_UnitsHaveBeenPlaced)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Save units positions or ");
            if (GUILayout.Button("Save")) 
            {
                ((MeshCreator)target).SaveLevel();
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Choose 2 Texture for the map !");
        l_Foldout = EditorGUILayout.Foldout(l_Foldout, "Textures");
        if (l_Foldout)
        {
            EditorGUILayout.BeginHorizontal();
            int count = 0;
            for (int i = 0; i < m_Textures.Count; i++)
            {
                if (GUILayout.Button(m_Textures[i]))
                {
                    if (countTexture % 2 == 0) m_Texture1 = new Vector2(Mathf.Floor(i / 16), (i % 16));
                    else if (countTexture % 2 == 1) m_Texture2 = new Vector2(Mathf.Floor(i / 16), (i % 16));
                    countTexture++;
                }
                if (count % NbrTexturePerLine == 1)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                count++;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (m_Texture1.x != Vector2.left.x && m_Texture2.x != Vector2.left.x) SetModeDisplayGenerationButton();
        if (m_Textures.Count == 0) FillTexture(((MeshCreator)target));
    }

    private void DoActionDisplayGenerationButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate World"))
        {
            ((MeshCreator)target).BuildMap(m_Texture1, m_Texture2);
            m_WorldHasBeenGenerated = true;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Return To Texture Selection"))
        {
            SetModeDisplayTexture();
        }
        EditorGUILayout.EndHorizontal();

        if (m_WorldHasBeenGenerated) SetModeDisplayUnits();
    }

    private void DoActionDisplayUnits()
    {
        //Enemies Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(m_SpritePlongeur))
        {
            ((MeshCreator)target).AddUnitAt(new Vector3(0, 11, 0), NAME_FILE_ENEMY_PLONG);
        }
        if (GUILayout.Button(m_SpriteDragon))
        {
            ((MeshCreator)target).AddUnitAt(new Vector3(0, 10, 0), NAME_FILE_ENEMY_DRAG);
        }
        EditorGUILayout.EndHorizontal();

        //Player Button
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(m_SpritePlayer))
        {
            ((MeshCreator)target).AddPlayerAt(new Vector3(5, 10, 0));
        }
        EditorGUILayout.EndHorizontal();

        //Save Map
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            ((MeshCreator)target).SaveLevel();
        }
        if (GUILayout.Button("Return To Texture Selection"))
        {
            SetModeDisplayTexture();
            m_WorldHasBeenSaved = true;
        }
        EditorGUILayout.EndHorizontal();

        if(m_WorldHasBeenSaved) SetModeDisplayTexture();

        if (!m_SpritePlongeur) m_SpritePlongeur = Resources.Load(PATH_TEXTURES + NAME_FILE_ENEMY_PLONG) as Texture;
        if (!m_SpriteDragon) m_SpriteDragon     = Resources.Load(PATH_TEXTURES + NAME_FILE_ENEMY_DRAG) as Texture;
        if (!m_SpritePlayer) m_SpritePlayer     = Resources.Load(PATH_TEXTURES + NAME_FILE_PLAYER) as Texture;
    }
    #endregion
    #endregion
}
