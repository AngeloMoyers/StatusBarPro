using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatusBarHandler))]
[CanEditMultipleObjects]
public class StatusBarEditorHandler : Editor
{
    SerializedProperty BarType;
    SerializedProperty SourceImage;
    SerializedProperty BarSize;

    //Custom Options
    SerializedProperty MatchDimensionsForCustomSprites;

    //Data
    SerializedProperty CurrentValue;
    SerializedProperty MaxValue;

    //Fill
    bool ShowFill = true;
    SerializedProperty FillGradient;
    SerializedProperty FillGradientSolidColor;
    SerializedProperty FillGradientColorMode;
    SerializedProperty CustomFillModeOn;
    SerializedProperty CustomFillSprite;
    bool ShowAnimation = true;
    SerializedProperty FillAnimMode;
    SerializedProperty FillAnimationRate;

    //Background
    bool ShowBackground = true;
    SerializedProperty BackgroundColor;
    SerializedProperty CustomBackgroundModeOn;
    SerializedProperty CustomBackgroundSprite;

    //Border
    bool ShowBorder = true;
    SerializedProperty BorderColor;
    SerializedProperty BorderThickness;
    SerializedProperty BorderOffset;
    SerializedProperty CustomBorderModeOn;
    SerializedProperty CustomBorderSprite;

    //Overlay
    bool ShowOverlay = true;
    SerializedProperty AddTextOverlay;
    SerializedProperty UseTMPro;
    SerializedProperty OverlayMode;
    SerializedProperty OverlayTextPosition;
    SerializedProperty CustomOverlayText;

    //Incremental Tick
    bool ShowIncrementalTicks = true;
    SerializedProperty AddIncrementalTicks;
    SerializedProperty TickSprite;
    SerializedProperty TickColor;
    SerializedProperty TickSize;
    SerializedProperty TickMode;
    SerializedProperty TickInterval;

    //Container#############################
    bool ShowContainer = true;
    SerializedProperty ValuePerContainer;
    SerializedProperty ContFillMode;
    SerializedProperty ContainerOffset;
    SerializedProperty ContainersPerRow;
    SerializedProperty WrapContainers;
    SerializedProperty WrapDirection;
    SerializedProperty WrapOffset;

    void OnEnable()
    {
        BarType = serializedObject.FindProperty("BarType");
        SourceImage = serializedObject.FindProperty("SourceImage");
        BarSize = serializedObject.FindProperty("Size");

        MatchDimensionsForCustomSprites = serializedObject.FindProperty("MatchDimensionsForCustomSprites");

        FillGradient = serializedObject.FindProperty("FillGradient");
        FillGradientColorMode = serializedObject.FindProperty("FillGradientMode");
        FillGradientSolidColor = serializedObject.FindProperty("FillGradientSolidColor");
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
        OverlayTextPosition = serializedObject.FindProperty("OverlayTextPosition");
        CustomOverlayText = serializedObject.FindProperty("CustomOverlayText");

        AddIncrementalTicks = serializedObject.FindProperty("AddIncrementalTicks");
        TickSprite = serializedObject.FindProperty("TickSprite");
        TickColor = serializedObject.FindProperty("TickColor");
        TickSize = serializedObject.FindProperty("TickSize");
        TickMode = serializedObject.FindProperty("TickMode");
        TickInterval = serializedObject.FindProperty("TickInterval");

        ValuePerContainer = serializedObject.FindProperty("ValuePerContainer");
        ContFillMode = serializedObject.FindProperty("ContFillMode");
        ContainerOffset = serializedObject.FindProperty("ContainerOffset");
        ContainersPerRow = serializedObject.FindProperty("ContainersPerRow");
        WrapContainers = serializedObject.FindProperty("WrapContainers");
        WrapDirection = serializedObject.FindProperty("WrapDirection");
        WrapOffset = serializedObject.FindProperty("WrapOffset");

        MaxValue = serializedObject.FindProperty("MaxValue");
        CurrentValue = serializedObject.FindProperty("CurrentValue");

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        GUIStyle FoldoutStyle = new GUIStyle(EditorStyles.foldout);
        FoldoutStyle.fontStyle = FontStyle.Bold;

        serializedObject.Update();

        StatusBarHandler bar = (StatusBarHandler)target;
        EditorGUILayout.LabelField("Status Bar Type", EditorStyles.boldLabel);

        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            BarSize.vector2IntValue = EditorGUILayout.Vector2IntField("Size", bar.Size);
            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnSizeChanged();
                bar.OnIncrementalTicksChanged();
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
        EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
        using (var changed = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.BeginHorizontal();
            var defaultLabelWidth = EditorGUIUtility.labelWidth;
            var defaultFieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = 80;
            EditorGUIUtility.fieldWidth = 20;
            MaxValue.intValue = EditorGUILayout.IntField("Max Value", bar.MaxValue);
            CurrentValue.intValue = EditorGUILayout.IntField("Current Value", bar.CurrentValue);
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            EditorGUIUtility.fieldWidth = defaultFieldWidth;
            EditorGUILayout.EndHorizontal();

            if (changed.changed)
            {
                serializedObject.ApplyModifiedProperties();
                bar.OnBarTypeChanged();
            }
        }

        if (bar.BarType.ToString().Contains("Container"))
        {
            EditorGUILayout.Space();
            ShowContainer = EditorGUILayout.Foldout(ShowContainer, "Container", FoldoutStyle);
            if (ShowContainer)
            {

                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    ValuePerContainer.intValue = EditorGUILayout.IntField("Value Per Container", bar.ValuePerContainer);
                    ContFillMode.enumValueIndex = (int)(ContainerFillMode)EditorGUILayout.EnumPopup("Fill Mode", bar.ContFillMode);
                    ContainerOffset.vector2Value = EditorGUILayout.Vector2Field("Container Offset", bar.ContainerOffset);
                    ContainersPerRow.intValue = EditorGUILayout.IntField("Containers Per Row", bar.ContainersPerRow);
                    WrapDirection.enumValueIndex = (int)(ContainerWrapDirection)EditorGUILayout.EnumPopup("Wrap Direction", bar.WrapDirection);
                    WrapContainers.boolValue = EditorGUILayout.Toggle("Wrap Containers", bar.WrapContainers);

                    if (WrapContainers.boolValue)
                    {
                        WrapOffset.vector2Value = EditorGUILayout.Vector2Field("Wrap Offset", bar.WrapOffset);
                    }

                    if (changed.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                        bar.OnBarTypeChanged();
                        bar.OnIncrementalTicksChanged();
                    }
                }
            }
        }

        EditorGUILayout.Space();
        ShowFill = EditorGUILayout.Foldout(ShowFill, "Fill", FoldoutStyle);
        if (ShowFill)
        {
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                CustomFillModeOn.boolValue = EditorGUILayout.Toggle("Custom Fill", bar.CustomFill);
                if (CustomFillModeOn.boolValue)
                    CustomFillSprite.objectReferenceValue = EditorGUILayout.ObjectField("Custom Border Image", CustomFillSprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                FillGradientColorMode.enumValueIndex = (int)(GradientColorMode)EditorGUILayout.EnumPopup("Fill Gradient Color Mode", bar.FillGradientMode);
                if (FillGradientColorMode.enumValueIndex == (int)GradientColorMode.Solid)
                {
                    FillGradientSolidColor.colorValue = EditorGUILayout.ColorField("Color", bar.FillGradientSolidColor);
                }
                else
                {
                    EditorGUILayout.PropertyField(FillGradient, true, null);
                }

                if (changed.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    bar.OnBarTypeChanged();
                }
            }
        }

        EditorGUILayout.Space();
        ShowBorder = EditorGUILayout.Foldout(ShowBorder, "Border", FoldoutStyle);
        if (ShowBorder)
        {
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
        }


        EditorGUILayout.Space();
        ShowBackground = EditorGUILayout.Foldout(ShowBackground, "Background", FoldoutStyle);
        if (ShowBackground)
        {
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
        }

        EditorGUILayout.Space();
        ShowAnimation = EditorGUILayout.Foldout(ShowAnimation, "Animation", FoldoutStyle);
        if (ShowAnimation)
        {
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
        }

        if (BarType.enumValueIndex != (int)StatusBarType.ContainerLeft && BarType.enumValueIndex != (int)StatusBarType.ContainerRight)
        {
            EditorGUILayout.Space();
            ShowOverlay = EditorGUILayout.Foldout(ShowOverlay, "Text Overlay", FoldoutStyle);
            if (ShowOverlay)
            {

                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    AddTextOverlay.boolValue = EditorGUILayout.Toggle("Add Text Overlay", bar.AddTextOverlay);

                    if (AddTextOverlay.boolValue)
                    {
                        UseTMPro.boolValue = EditorGUILayout.Toggle("Use TMPro", bar.UseTMPro);
                        OverlayMode.enumValueIndex = (int)(OverlayTextMode)EditorGUILayout.EnumPopup("Overlay Text Mode", bar.OverlayMode);
                        OverlayTextPosition.vector2Value = EditorGUILayout.Vector2Field("Overlay Text Position", bar.OverlayTextPosition);

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
            }

            EditorGUILayout.Space();
            ShowIncrementalTicks = EditorGUILayout.Foldout(ShowIncrementalTicks, "Incremental Ticks", FoldoutStyle);
            if (ShowIncrementalTicks)
            {
                using (var changed = new EditorGUI.ChangeCheckScope())
                {
                    AddIncrementalTicks.boolValue = EditorGUILayout.Toggle("Add Incremental Ticks", bar.AddIncrementalTicks);

                    if (AddIncrementalTicks.boolValue)
                    {
                        TickSprite.objectReferenceValue = EditorGUILayout.ObjectField("Incremental Tick Sprite", TickSprite.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        TickColor.colorValue = EditorGUILayout.ColorField("Tick Color", bar.TickColor);
                        TickSize.vector2Value = EditorGUILayout.Vector2Field("Tick Dimensions", bar.TickSize);
                        TickMode.enumValueIndex = (int)(IncrementalTickMode)EditorGUILayout.EnumPopup("Incremental Tick Mode", bar.TickMode);
                        TickInterval.floatValue = EditorGUILayout.FloatField("Incremental Tick Interval", bar.TickInterval);
                    }

                    if (changed.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                        bar.OnIncrementalTicksChanged();
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        
        if (BarType.enumValueIndex != (int)bar.BarType)
            bar.OnBarTypeChanged();

        if (BarSize.vector2IntValue != bar.Size)
            bar.OnSizeChanged();
    }
}
