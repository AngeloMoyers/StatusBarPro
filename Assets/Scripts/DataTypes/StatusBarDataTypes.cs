using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum StatusBarType
{
    FillLeft,
    FillRight,
    FillCenterHorizontal,
    ContainerFullLeft,
    ContainerFullRight,
    ContainerPartialLeft,
    ContainerPartialRight
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
