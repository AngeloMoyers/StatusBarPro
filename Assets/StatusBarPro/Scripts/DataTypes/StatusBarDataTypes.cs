using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum StatusBarType
{
    FillLeft,
    FillRight,
    FillTop,
    FillBottom,
    ContainerLeft,
    ContainerRight,
}

[System.Serializable]
public enum GradientColorMode
{
    Blend,
    Threshold,
    Solid
}

[System.Serializable]
public enum FillAnimationMode
{
    FixedTime,
    Speed,
    Instant
}

[System.Serializable]
public enum OverlayTextMode
{
    CurrentValue,
    CurrentAndMaxValue,
    Percent,
    CurrentValueAndPercent,
    Custom
}

[System.Serializable]
public enum IncrementalTickMode
{
    Value,
    Percent,
}

[System.Serializable]
public enum ContainerWrapDirection
{
    Up,
    Down
}

[System.Serializable]
public enum ContainerFillMode
{
    Horizontal,
    Vertical,
    Radial
}

[System.Serializable]
public class StatusBarObjectData
{
    public GameObject ContainerObject;

    public Image FillImage;
    public Image BackgroundImage;
    public Image BorderImage;
    public Image BorderMaskImage;
    public Image BarMaskImage;

    public int CurrentValue;
    public float NormalizedValue;
}
