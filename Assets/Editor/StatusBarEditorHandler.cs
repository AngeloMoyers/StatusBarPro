using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatusBar))]
[CanEditMultipleObjects]
public class StatusBarEditorHandler : Editor
{
    SerializedProperty BarType;
    SerializedProperty SourceImage;
    SerializedProperty BarSize;

    //Custom Options
    SerializedProperty MatchDimensionsForCustomSprites;

    //Fill
    SerializedProperty FillGradient;
    SerializedProperty FillGradientColorMode;
    SerializedProperty CustomFillModeOn;
    SerializedProperty CustomFillSprite;
    SerializedProperty FillAnimMode;
    SerializedProperty FillAnimationRate;

    //Background
    SerializedProperty BackgroundColor;
    SerializedProperty CustomBackgroundModeOn;
    SerializedProperty CustomBackgroundSprite;

    //Border
    SerializedProperty BorderColor;
    SerializedProperty BorderThickness;
    SerializedProperty BorderOffset;
    SerializedProperty CustomBorderModeOn;
    SerializedProperty CustomBorderSprite;

    //Overlay
    SerializedProperty AddTextOverlay;
    SerializedProperty UseTMPro;
    SerializedProperty OverlayMode;
    SerializedProperty CustomOverlayText;

    //Incremental Tick
    SerializedProperty AddIncrementalTicks;
    SerializedProperty TickSprite;
    SerializedProperty TickMode;
    SerializedProperty TickInterval;

    void OnEnable()
    {
        BarType = serializedObject.FindProperty("BarType");
        SourceImage = serializedObject.FindProperty("SourceImage");
        BarSize = serializedObject.FindProperty("Size");

        MatchDimensionsForCustomSprites = serializedObject.FindProperty("MatchDimensionsForCustomSprites");

        FillGradient = serializedObject.FindProperty("FillGradient");
        FillGradientColorMode = serializedObject.FindProperty("FillGradientMode");
        CustomFillModeOn = serializedObject.FindProperty("CustomFill");
        CustomFillSprite = serializedObject.FindProperty("CustomFillSprite");
        FillAnimMode = serializedObject.FindProperty("FillAnimationMode");
        FillAnimationRate = serializedObject.FindProperty("FillAnimationRate");

        BackgroundColor = serializedObject.FindProperty("BackgroundColor");
        CustomBackgroundModeOn = serializedObject.FindProperty("CustomBackground");
        CustomBackgroundSprite = serializedObject.FindProperty("CustomBackgroundSprite");

        BorderColor = serializedObject.FindProperty("BorderColor");
        BorderThickness = serializedObject.FindProperty("BorderThickness");
        BorderOffset = serializedObject.FindProperty("BorderOffset");
        CustomBorderModeOn = serializedObject.FindProperty("CustomBorder");
        CustomBorderSprite = serializedObject.FindProperty("CustomBorderSprite");

        AddTextOverlay = serializedObject.FindProperty("AddTextOverlay");
        UseTMPro = serializedObject.FindProperty("UseTMPro");
        OverlayMode = serializedObject.FindProperty("OverlayMode");
        CustomOverlayText = serializedObject.FindProperty("CustomOverlayText");

        AddIncrementalTicks = serializedObject.FindProperty("AddIncrementalTicks");
        TickSprite = serializedObject.FindProperty("TickSprite");
        TickMode = serializedObject.FindProperty("TickMode");
        TickInterval = serializedObject.FindProperty("TickInterval");

        SerializedProperty maxValue = serializedObject.FindProperty("MaxValue");
        maxValue.intValue = 10;
        SerializedProperty currentValue = serializedObject.FindProperty("CurrentValue");
        currentValue.intValue = 8;

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        StatusBar bar = (StatusBar)target;
        EditorGUILayout.LabelField("Status Bar Type", EditorStyles.boldLabel);

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            BarSize.vector2IntValue = EditorGUILayout.Vector2IntField("Size", bar.Size);
            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnSizeChanged();
            }
        }

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            BarType.enumValueIndex = (int)(StatusBarType)EditorGUILayout.EnumPopup("BarType", bar.BarType);
            SourceImage.objectReferenceValue = EditorGUILayout.ObjectField("Source Image", SourceImage.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnBarTypeChanged();
            }
        }

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            MatchDimensionsForCustomSprites.boolValue = EditorGUILayout.Toggle("Custom Sprites Match Size", bar.MatchDimensionsForCustomSprites);
            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnSizeChanged();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fill", EditorStyles.boldLabel);
        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            CustomFillModeOn.boolValue = EditorGUILayout.Toggle("Custom Fill", bar.CustomFill);
            if (CustomFillModeOn.boolValue)
                CustomFillSprite.objectReferenceValue = EditorGUILayout.ObjectField("Custom Border Image", CustomFillSprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            FillGradientColorMode.enumValueIndex = (int)(GradientColorMode)EditorGUILayout.EnumPopup("Fill Gradient Color Mode", bar.FillGradientMode);
            EditorGUILayout.PropertyField(FillGradient, true, null);

            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnBarTypeChanged();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Border", EditorStyles.boldLabel);
        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            CustomBorderModeOn.boolValue = EditorGUILayout.Toggle("Custom Border", bar.CustomBorder);
            if (CustomBorderModeOn.boolValue)
                CustomBorderSprite.objectReferenceValue = EditorGUILayout.ObjectField("Custom Border Image", CustomBorderSprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            
            BorderColor.colorValue = EditorGUILayout.ColorField("Border Color", BorderColor.colorValue);

            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnBarTypeChanged();
            }
        }

        if (!CustomBorderModeOn.boolValue)
        {
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                BorderOffset.vector2IntValue = EditorGUILayout.Vector2IntField("Border Offset", bar.BorderOffset);
                BorderThickness.vector2IntValue = EditorGUILayout.Vector2IntField("Border Thickness", bar.BorderThickness);
                if (changed.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    bar.OnSizeChanged();
                }
            }
        }


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Background", EditorStyles.boldLabel);
        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            CustomBackgroundModeOn.boolValue = EditorGUILayout.Toggle("Custom Background", bar.CustomBackground);
            if (CustomBackgroundModeOn.boolValue)
                CustomBackgroundSprite.objectReferenceValue = EditorGUILayout.ObjectField("Custom Border Image", CustomBackgroundSprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            BackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", BackgroundColor.colorValue);
            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnBarTypeChanged();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            FillAnimMode.enumValueIndex = (int)(FillAnimationMode)EditorGUILayout.EnumPopup("Fill Animation Mode", bar.FillAnimationMode);

            if (FillAnimMode.enumValueIndex != (int)FillAnimationMode.Instant)
            {
                FillAnimationRate.floatValue = EditorGUILayout.FloatField("Fill Animation Rate", bar.FillAnimationRate);
            }

            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Text Overlay", EditorStyles.boldLabel);

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            AddTextOverlay.boolValue = EditorGUILayout.Toggle("Add Text Overlay", bar.AddTextOverlay);

            if (AddTextOverlay.boolValue)
            {
                UseTMPro.boolValue = EditorGUILayout.Toggle("Use TMPro", bar.UseTMPro);
                OverlayMode.enumValueIndex = (int)(OverlayTextMode)EditorGUILayout.EnumPopup("Overlay Text Mode", bar.OverlayMode);

                if (OverlayMode.enumValueIndex == (int)OverlayTextMode.Custom)
                {
                    CustomOverlayText.stringValue = EditorGUILayout.TextField("Overlay Text", bar.CustomOverlayText, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
            }

            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnOverlayChanged();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Incremental Ticks", EditorStyles.boldLabel);

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            AddIncrementalTicks.boolValue = EditorGUILayout.Toggle("Add Incremental Ticks", bar.AddIncrementalTicks);

            if (AddIncrementalTicks.boolValue)
            {
                TickSprite.objectReferenceValue = EditorGUILayout.ObjectField("Incremental Tick Sprite", TickSprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                TickMode.enumValueIndex = (int)(IncrementalTickMode)EditorGUILayout.EnumPopup("Incremental Tick Mode", bar.TickMode);
                TickInterval.floatValue = EditorGUILayout.FloatField("Incremental Tick Interval", bar.TickInterval);
            }

            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnIncrementalTicksChanged();
            }
        }

        serializedObject.ApplyModifiedProperties();
        
        if (BarType.enumValueIndex != (int)bar.BarType)
            bar.OnBarTypeChanged();

        if (BarSize.vector2IntValue != bar.Size)
            bar.OnSizeChanged();
    }
}
