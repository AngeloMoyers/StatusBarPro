using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusBarHandler : MonoBehaviour
{
    [SerializeField] public StatusBarType BarType;

    //ObjectLinks
    GameObject StatusBarObject; 
    GameObject StatusBarPrefab;

    //Preferences
    [SerializeField] public Vector2Int Size;
    [SerializeField] public Sprite SourceImage;
    [SerializeField] public Color BackgroundColor;
    [SerializeField] public Color BorderColor;


    //Fill Color
    [SerializeField] public Gradient FillGradient;
    [SerializeField] public Color FillGradientSolidColor;
    [SerializeField] public GradientColorMode FillGradientMode;
    
    //Custom Options
    [SerializeField] public bool MatchDimensionsForCustomSprites;

    [SerializeField] public bool CustomFill;
    [SerializeField] public Sprite CustomFillSprite;
    [SerializeField] public Vector2 CustomFillOffset;
    [SerializeField] public Vector2 CustomFillSize;

    [SerializeField] public bool CustomBorder;
    [SerializeField] public Sprite CustomBorderSprite;
    [SerializeField] public Vector2 CustomBorderOffset;
    [SerializeField] public Vector2 CustomBorderSize;

    [SerializeField] public bool CustomBackground;
    [SerializeField] public Sprite CustomBackgroundSprite;
    [SerializeField] public Vector2 CustomBackgroundOffset;
    [SerializeField] public Vector2 CustomBackgroundSize;

    //Animation
    [SerializeField] public FillAnimationMode FillAnimationMode;
    [SerializeField] public float FillAnimationRate;
    float TargetEndAnimationTime;

    //Text Overlay
    [SerializeField] public bool AddTextOverlay;
    [SerializeField] public bool UseTMPro;
    [SerializeField] public OverlayTextMode OverlayMode;
    [SerializeField] public Vector2 OverlayTextPosition;
    [SerializeField] public string CustomOverlayText;

    GameObject TMProOverlayTextObject;
    GameObject OverlayTextObject;

    TextMeshProUGUI TMProOverlayText;
    Text OverlayText;

    //ValueIncrementTicks
    [SerializeField] public bool AddIncrementalTicks;
    [SerializeField] public Sprite TickSprite;
    [SerializeField] public Vector2 TickSize;
    [SerializeField] public Color TickColor;
    [SerializeField] public IncrementalTickMode TickMode;
    [SerializeField] public float TickInterval;

    //Fill##########################################
    GameObject FillObject;
    GameObject BackgroundObject;
    GameObject BorderMaskObject;
    GameObject BorderObject;
    GameObject BarMaskObject;

    Image FillImage;
    Image BackgroundImage;
    Image BorderMaskImage;
    Image BorderImage;
    Image BarMaskImage;

    [SerializeField] public Vector2Int BorderThickness;
    [SerializeField] public Vector2Int BorderOffset;

    //End Fill#####################################
    //Container####################################
    [SerializeField] public int ValuePerContainer;
    [SerializeField] public ContainerFillMode ContFillMode;
    [SerializeField] public Vector2 ContainerOffset;
    [SerializeField] public int ContainersPerRow;
    [SerializeField] public bool WrapContainers;
    [SerializeField] public ContainerWrapDirection WrapDirection;
    [SerializeField] public Vector2 WrapOffset;

    //Dictionary<GameObject, StatusBar> ContainerMap;
    //End Container################################

    //Data
    [HideInInspector][SerializeField] public int MaxValue;
    [HideInInspector] [SerializeField] public int CurrentValue;
    float NormalizedValue;
    int LastValue;

    List<StatusBarObjectData> ContainerData;

    private void Reset()
    {
        Debug.Log("Status Bar Created");
    }

    private GameObject GetStatusBarPrefab()
    {
        if (StatusBarPrefab == null)
            StatusBarPrefab = Resources.Load<GameObject>("Prefabs/StatusBar");

        return StatusBarPrefab;
    }

    public void SetTo()
    {
        GameObject InField = GameObject.Find("InputAmount");
        if (InField != null)
        {
            InputField inputAmount = InField.GetComponentInChildren<InputField>();
            if (inputAmount != null)
            {
                SetValue(Int32.Parse(inputAmount.text.ToString()));
            }
        }
    }

    public void Raise()
    {
        GameObject InField = GameObject.Find("InputAmount");
        if (InField != null)
        {
            InputField inputAmount = InField.GetComponentInChildren<InputField>();
            if (inputAmount != null)
            {
                AdjustValue(Int32.Parse(inputAmount.text.ToString()));
            }
        }
    }

    public void Lower()
    {
        GameObject InField = GameObject.Find("InputAmount");
        if (InField != null)
        {
            InputField inputAmount = InField.GetComponentInChildren<InputField>();
            if (inputAmount != null)
            {
                AdjustValue(-Int32.Parse(inputAmount.text.ToString()));
            }
        }
    }

    private void Awake()
    {
        InitImages();
    }

    void Start()
    {
    }

    void Update()
    {
        AnimateFill();
    }

    void AnimateFill()
    {
        if (BarType.ToString().Contains("Container") && ContainerData != null)
        {
            float totalNorms = 0f;
            for (int i = 0; i < ContainerData.Count; i++)
            {
                if (ContainerData[i].FillImage != null)
                    totalNorms += ContainerData[i].FillImage.fillAmount;
            }
            float currentHPRepresentation = totalNorms / (float)ContainerData.Count;
            
            int it = LastValue > CurrentValue? ContainerData.Count - 1 : 0;
            int targetNum = LastValue > CurrentValue ? -1 : (ContainerData.Count);
            while (it != targetNum && LastValue != CurrentValue)
            {
                Image Fill = ContainerData[it].FillImage;
                float normalizedValue = ContainerData[it].NormalizedValue;

                if (Fill != null && !Mathf.Approximately(normalizedValue, Fill.fillAmount))
                {
                    switch (FillAnimationMode)
                    {
                        case FillAnimationMode.FixedTime:
                            {
                                float differenceBetweenCurrentAndTarget = Mathf.Abs(currentHPRepresentation - NormalizedValue);
                                float timeLeft = TargetEndAnimationTime - Time.time;
                                float amountToMoveThisFrame = (differenceBetweenCurrentAndTarget * (MaxValue / ValuePerContainer) / (timeLeft)) * Time.deltaTime;

                                if (normalizedValue < Fill.fillAmount)
                                {
                                    Fill.fillAmount -= amountToMoveThisFrame;

                                    if (Fill.fillAmount < normalizedValue)
                                        Fill.fillAmount = normalizedValue;
                                }
                                else if (normalizedValue > Fill.fillAmount)
                                {
                                    Fill.fillAmount += amountToMoveThisFrame;

                                    if (Fill.fillAmount > normalizedValue)
                                        Fill.fillAmount = normalizedValue;
                                }

                                if (Mathf.Approximately(Fill.fillAmount, normalizedValue))
                                    Fill.fillAmount = normalizedValue;

                                break;
                            }
                        case FillAnimationMode.Speed:
                            {
                                if (normalizedValue < Fill.fillAmount)
                                {
                                    Fill.fillAmount -= Time.deltaTime * FillAnimationRate;

                                    if (Fill.fillAmount < normalizedValue)
                                        Fill.fillAmount = normalizedValue;
                                }
                                else
                                {
                                    Fill.fillAmount += Time.deltaTime * FillAnimationRate;

                                    if (Fill.fillAmount > normalizedValue)
                                        Fill.fillAmount = normalizedValue;
                                }
                                break;
                            }
                        case FillAnimationMode.Instant:
                            {
                                Fill.fillAmount = normalizedValue;
                                break;
                            }
                    }

                    if (FillGradientMode != GradientColorMode.Solid)
                    {
                        foreach (var cont in ContainerData)
                        {
                            UpdateFillColor(cont.FillImage, currentHPRepresentation);
                        }
                    }
                    break;
                }
                if (LastValue < CurrentValue)
                {
                    it++;
                }
                else if (LastValue > CurrentValue)
                {
                    it--;
                }
            }
        }
        else
        {
            if (FillImage != null && !Mathf.Approximately(NormalizedValue, FillImage.fillAmount))
            {
                switch (FillAnimationMode)
                {
                    case FillAnimationMode.FixedTime:
                        {
                            float differenceBetweenCurrentAndTarget = Mathf.Abs(FillImage.fillAmount - NormalizedValue);
                            float timeLeft = TargetEndAnimationTime - Time.time;
                            float amountToMoveThisFrame = (differenceBetweenCurrentAndTarget / timeLeft) * Time.deltaTime;

                            if (NormalizedValue < FillImage.fillAmount)
                            {
                                FillImage.fillAmount -= amountToMoveThisFrame;

                                if (FillImage.fillAmount < NormalizedValue)
                                    FillImage.fillAmount = NormalizedValue;
                            }
                            else if (NormalizedValue > FillImage.fillAmount)
                            {
                                FillImage.fillAmount += amountToMoveThisFrame;

                                if (FillImage.fillAmount > NormalizedValue)
                                    FillImage.fillAmount = NormalizedValue;
                            }

                            if (Mathf.Approximately(FillImage.fillAmount, NormalizedValue))
                                FillImage.fillAmount = NormalizedValue;

                            break;
                        }
                    case FillAnimationMode.Speed:
                        {
                            if (NormalizedValue < FillImage.fillAmount)
                            {
                                FillImage.fillAmount -= Time.deltaTime * FillAnimationRate;

                                if (FillImage.fillAmount < NormalizedValue)
                                    FillImage.fillAmount = NormalizedValue;
                            }
                            else
                            {
                                FillImage.fillAmount += Time.deltaTime * FillAnimationRate;

                                if (FillImage.fillAmount > NormalizedValue)
                                    FillImage.fillAmount = NormalizedValue;
                            }
                            break;
                        }
                    case FillAnimationMode.Instant:
                        {
                            FillImage.fillAmount = NormalizedValue;
                            break;
                        }

                }
                UpdateFillColor(FillImage, FillImage.fillAmount);
            }
        }
    }

    public int SetValue(int amount)
    {
        LastValue = CurrentValue;
        CurrentValue = amount;

        if (CurrentValue < 0)
            CurrentValue = 0;
        if (CurrentValue > MaxValue)
            CurrentValue = MaxValue;

        UpdateValue();

        if (FillAnimationMode == FillAnimationMode.FixedTime)
        {
            TargetEndAnimationTime = Time.time + FillAnimationRate;
        }

        if (BarType.ToString().Contains("Container"))
        {
            if (ContainerData != null)
            {
                for (int i = 0; i < ContainerData.Count; i++)
                {
                    if (i == CurrentValue / ValuePerContainer)
                        ContainerData[i].CurrentValue = CurrentValue % ValuePerContainer;
                    else if (i < CurrentValue / ValuePerContainer)
                        ContainerData[i].CurrentValue = ValuePerContainer;
                    else
                        ContainerData[i].CurrentValue = 0;

                    ContainerData[i].NormalizedValue = (float)ContainerData[i].CurrentValue/ (float)ValuePerContainer;
                }
            }
        }

        OnOverlayChanged();

        return CurrentValue;
    }

    public int AdjustValue(int amount)
    {
        LastValue = CurrentValue;
        CurrentValue += amount;

        if (CurrentValue < 0)
            CurrentValue = 0;
        if (CurrentValue > MaxValue)
            CurrentValue = MaxValue;

        UpdateValue();

        if (FillAnimationMode == FillAnimationMode.FixedTime)
        {
            TargetEndAnimationTime = Time.time + FillAnimationRate;
        }

        if (BarType.ToString().Contains("Container"))
        {
            if (ContainerData != null)
            {
                for (int i = 0; i < ContainerData.Count; i++)
                {
                    if (i == CurrentValue / ValuePerContainer)
                        ContainerData[i].CurrentValue = CurrentValue % ValuePerContainer;
                    else if (i < CurrentValue / ValuePerContainer)
                        ContainerData[i].CurrentValue = ValuePerContainer;
                    else
                        ContainerData[i].CurrentValue = 0;

                    ContainerData[i].NormalizedValue = (float)ContainerData[i].CurrentValue / (float)ValuePerContainer;
                }
            }
        }

        OnOverlayChanged();

        return CurrentValue;
    }

    public void OnBarTypeChanged()
    {
        InitImages();
        OnIncrementalTicksChanged();
        OnOverlayChanged();
    }

    public void OnSizeChanged()
    {
        UpdateValue();
        GatherObjects();

        if (!CustomFill || MatchDimensionsForCustomSprites)
        {
            if (FillObject != null)
            {
                RectTransform rect = FillObject.GetComponent<RectTransform>();
                if (rect != null)
                    rect.sizeDelta = Size;
            }
        }
        if (!CustomBackground || MatchDimensionsForCustomSprites)
        {
            if (BackgroundObject != null)
            {
                RectTransform rect = BackgroundObject.GetComponent<RectTransform>();
                if (rect != null)
                    rect.sizeDelta = Size;
            }
        }
        if (!CustomBorder || MatchDimensionsForCustomSprites)
        {
            if (BorderObject != null)
            {
                RectTransform rect = BorderObject.GetComponent<RectTransform>();
                if (rect != null)
                    rect.sizeDelta = Size;
            }
            if (BorderMaskObject != null)
            {
                RectTransform rect = BorderMaskObject.GetComponent<RectTransform>();
                if (rect != null)
                {
                    Vector2 newSize = new Vector2(Size.x - BorderThickness.x, Size.y - BorderThickness.y);
                    rect.sizeDelta = newSize;
                    rect.localPosition = new Vector2(BorderOffset.x, BorderOffset.y);
                }
            }
        }
        if (BarMaskObject != null)
        {
            RectTransform rect = BarMaskObject.GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = Size;
        }

        if (BarType.ToString().Contains("Container"))
        {
            if (ContainerData != null)
            {
                for (int i = 0; i < ContainerData.Count; i++)
                {
                    if (ContainerData[i].ContainerObject != null)
                    {
                        RectTransform tform = ContainerData[i].ContainerObject.GetComponent<RectTransform>();
                        tform.sizeDelta = Size;
                    }
                    if (ContainerData[i].FillImage != null)
                    {
                        RectTransform tform = ContainerData[i].FillImage.GetComponent<RectTransform>();
                        tform.sizeDelta = Size;
                    }
                    if (ContainerData[i].BorderMaskImage != null)
                    {
                        RectTransform tform = ContainerData[i].BorderMaskImage.GetComponent<RectTransform>();
                        Vector2 newSize = new Vector2(Size.x - BorderThickness.x, Size.y - BorderThickness.y);
                        tform.sizeDelta = newSize;
                        tform.localPosition = new Vector2(BorderOffset.x, BorderOffset.y);
                    }
                    if (ContainerData[i].BorderImage != null)
                    {
                        RectTransform tform = ContainerData[i].BorderImage.GetComponent<RectTransform>();
                        tform.sizeDelta = Size;
                    }
                    if (ContainerData[i].BackgroundImage != null)
                    {
                        RectTransform tform = ContainerData[i].BackgroundImage.GetComponent<RectTransform>();
                        tform.sizeDelta = Size;
                    }
                    if (ContainerData[i].BarMaskImage != null)
                    {
                        RectTransform tform = ContainerData[i].BarMaskImage.GetComponent<RectTransform>();
                        tform.sizeDelta = Size;
                    }
                }
            }

            OnBarTypeChanged();
        }
    }

    private void UpdateFillColor(Image img, float normalizedValue)
    {
        if (img != null)
        {
            img.color = GetFillColor(normalizedValue);
        }
    }

    private Color GetFillColor(float normalizedValue)
    {
        Color color = Color.white;
        switch (FillGradientMode)
        {
            case GradientColorMode.Blend:
                color = FillGradient.Evaluate(normalizedValue);
                break;
            case GradientColorMode.Threshold:
                {
                    if (normalizedValue >= 1)
                    {
                        return FillGradient.colorKeys[FillGradient.colorKeys.Length - 1].color;
                    }
                    int index = 0;
                    foreach (var key in FillGradient.colorKeys)
                    {
                        if (normalizedValue >= FillGradient.colorKeys[index].time)
                        {
                            index++;
                            continue;
                        }
                        color = FillGradient.colorKeys[index].color;
                    }
                    break;
                }
            case GradientColorMode.Solid:
                color = FillGradientSolidColor;
                break;
        }

        return color;
    }

    private void SetupImages(ref StatusBarObjectData data)
    {
        if (data.FillImage != null)
        {
            data.FillImage.sprite = CustomFill ? CustomFillSprite : SourceImage;
            data.FillImage.fillAmount = data.NormalizedValue;

            RectTransform tform = data.FillImage.GetComponent<RectTransform>();
            tform.sizeDelta = Size;

           //UpdateFillColor(); TODO: GetFillColor()
        }

        //Background Image
        if (CustomBackground && data.BackgroundImage != null)
        {
            data.BackgroundImage.sprite = CustomBackgroundSprite;
            data.BackgroundImage.color = BackgroundColor;

            RectTransform tform = data.BackgroundImage.GetComponent<RectTransform>();
            tform.sizeDelta = Size;
        }
        else if (data.BackgroundImage != null)
        {
            data.BackgroundImage.sprite = SourceImage;
            data.BackgroundImage.type = Image.Type.Simple;
            data.BackgroundImage.color = BackgroundColor;

            RectTransform tform = data.BackgroundImage.GetComponent<RectTransform>();
            tform.sizeDelta = Size;
        }

        //Border Image
        if (CustomBorder && data.BorderImage != null)
        {
            data.BorderImage.sprite = CustomBorderSprite;
            data.BorderImage.color = BorderColor;

            RectTransform tform = data.BorderImage.GetComponent<RectTransform>();
            tform.sizeDelta = Size;
        }
        else
        {
            if (data.BorderImage != null)
            {
                data.BorderImage.sprite = SourceImage;
                data.BorderImage.type = Image.Type.Simple;
                data.BorderImage.color = BorderColor;

                RectTransform tform = data.BorderImage.GetComponent<RectTransform>();
                tform.sizeDelta = Size;
            }

            if (BorderMaskImage != null)
            {
                data.BorderMaskImage.sprite = SourceImage;

                RectTransform tform = data.BorderMaskImage.GetComponent<RectTransform>();
                Vector2 newSize = new Vector2(Size.x - BorderThickness.x, Size.y - BorderThickness.y);
                tform.sizeDelta = newSize;
                tform.localPosition = new Vector2(BorderOffset.x, BorderOffset.y);
            }
        }
        if (BarMaskImage != null)
        {
            data.BarMaskImage.sprite = SourceImage;
            data.BarMaskImage.type = Image.Type.Simple;
        }
    }

    void InitImages()
    {
        UpdateValue();
        GatherObjects();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject container = transform.GetChild(i).gameObject;
            if (container != StatusBarObject && container.name.Contains("Container"))
            {
                DestroyImmediate(container);
            }    
        }


        if (BarType.ToString().Contains("Fill"))
        {
            if (FillImage != null)
            {
                FillImage.sprite = CustomFill ? CustomFillSprite : SourceImage;
                FillImage.type = Image.Type.Filled;
                FillImage.fillAmount = NormalizedValue;

                UpdateFillColor(FillImage, NormalizedValue);
            }

            //Background Image
            if (CustomBackground && BackgroundImage != null)
            {
                BackgroundImage.sprite = CustomBackgroundSprite;
                BackgroundImage.color = BackgroundColor;
            }
            else if (BackgroundImage != null)
            {
                BackgroundImage.sprite = SourceImage;
                BackgroundImage.type = Image.Type.Simple;
                BackgroundImage.color = BackgroundColor;
            }

            //Border Image
            if (CustomBorder && BorderImage != null)
            {
                BorderImage.sprite = CustomBorderSprite;
                BorderImage.color = BorderColor;
            }
            else
            {
                if (BorderImage != null)
                {
                    BorderImage.sprite = SourceImage;
                    BorderImage.type = Image.Type.Simple;
                    BorderImage.color = BorderColor;
                }

                if (BorderMaskImage != null)
                {
                    BorderMaskImage.sprite = SourceImage;
                }
            }
            if (BarMaskImage != null)
            {
                BarMaskImage.sprite = SourceImage;
                BarMaskImage.type = Image.Type.Simple;
            }
        }

        switch (BarType)
        {
            case StatusBarType.FillLeft:
                {
                    if (FillImage != null)
                    {
                        FillImage.fillMethod = Image.FillMethod.Horizontal;
                        FillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                    }

                    break;
                }
            case StatusBarType.FillRight:
                {
                    if (FillImage != null)
                    {
                        FillImage.fillMethod = Image.FillMethod.Horizontal;
                        FillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                    }

                    break;
                }

            case StatusBarType.FillTop:
                {
                    if (FillImage != null)
                    {
                        FillImage.fillMethod = Image.FillMethod.Vertical;
                        FillImage.fillOrigin = (int)Image.OriginVertical.Top;
                    }

                    break;
                }
            case StatusBarType.FillBottom:
                {
                    if (FillImage != null)
                    {
                        FillImage.fillMethod = Image.FillMethod.Vertical;
                        FillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                    }

                    break;
                }
            case StatusBarType.ContainerLeft:
                {
                    if (MaxValue > 0 && ValuePerContainer > 0)
                    {
                        if (ContainerData == null)
                            ContainerData = new List<StatusBarObjectData>();
                        ContainerData.Clear();

                        int ContainerCount = (int)(MaxValue / ValuePerContainer);

                        StatusBarObjectData[] ContainerDataArray = new StatusBarObjectData[ContainerCount];
                        for (int i = 0; i < ContainerCount; i++)
                        {
                            ContainerDataArray[i] = new StatusBarObjectData();
                        }

                        int rowCount = 0;
                        for (int i = 0; i < ContainerCount; i++)
                        {
                            GameObject container;
                            if (i == 0)
                            {
                                container = transform.GetChild(0).gameObject;
                            }
                            else
                            {
                                container = Instantiate(GetStatusBarPrefab());
                                container.name = "Container";
                            }
                            RectTransform tform = container.GetComponent<RectTransform>();
                            tform.sizeDelta = Size;
                            GatherObjects(container.transform, ref ContainerDataArray[i]);

                            ContainerDataArray[i].ContainerObject = container;
                            if (i == CurrentValue / ValuePerContainer)
                                ContainerDataArray[i].CurrentValue = CurrentValue % ValuePerContainer;
                            else if ( i < CurrentValue / ValuePerContainer)
                                ContainerDataArray[i].CurrentValue = ValuePerContainer;
                            else
                                ContainerDataArray[i].CurrentValue = 0;


                            ContainerDataArray[i].NormalizedValue = (float)ContainerDataArray[i].CurrentValue / (float)ValuePerContainer;

                            SetupImages(ref ContainerDataArray[i]);

                            if (ContainerDataArray[i].FillImage != null)
                            {
                                switch (ContFillMode)
                                {
                                    case ContainerFillMode.Horizontal:
                                        ContainerDataArray[i].FillImage.fillMethod = Image.FillMethod.Horizontal;
                                        ContainerDataArray[i].FillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                                        break;
                                    case ContainerFillMode.Vertical:
                                        ContainerDataArray[i].FillImage.fillMethod = Image.FillMethod.Vertical;
                                        ContainerDataArray[i].FillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                                        break;
                                    case ContainerFillMode.Radial:
                                        ContainerDataArray[i].FillImage.fillMethod = Image.FillMethod.Radial360;
                                        ContainerDataArray[i].FillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                                        ContainerDataArray[i].FillImage.fillClockwise = true;
                                        break;
                                }

                                ContainerDataArray[i].FillImage.fillAmount = ContainerDataArray[i].NormalizedValue;
                                UpdateFillColor(ContainerDataArray[i].FillImage, NormalizedValue);
                            }
                        }

                        ContainerData = new List<StatusBarObjectData>(ContainerDataArray);


                        int it = 0;
                        for (int i = 0; i < ContainerData.Count; i++)
                        {
                            GameObject container = ContainerData[i].ContainerObject;
                           container.transform.SetParent(transform, false);

                            if (it > 0 && ContainersPerRow > 0 && it % ContainersPerRow == 0)
                            {
                                if (!WrapContainers)
                                    break;

                                rowCount++;
                            }

                            if (container != gameObject && ContainersPerRow != 0)
                            {
                                switch (WrapDirection)
                                {
                                    case ContainerWrapDirection.Up:
                                        container.transform.localPosition = new Vector3((it % ContainersPerRow) * (Size.x + ContainerOffset.x), -(it * ContainerOffset.y) + (rowCount * (Size.y + WrapOffset.y)), 0);

                                        break;
                                    case ContainerWrapDirection.Down:
                                        container.transform.localPosition = new Vector3((it % ContainersPerRow) * (Size.x + ContainerOffset.x), (it * ContainerOffset.y) + (rowCount * -(Size.y + WrapOffset.y)), 0);
                                        break;
                                }
                            }
                            it++;
                        }
                    }
                }
                break;
            case StatusBarType.ContainerRight:
                {
                    if (MaxValue > 0 && ValuePerContainer > 0)
                    {
                        if (ContainerData == null)
                            ContainerData = new List<StatusBarObjectData>();
                        ContainerData.Clear();

                        int ContainerCount = (int)(MaxValue / ValuePerContainer);

                        StatusBarObjectData[] ContainerDataArray = new StatusBarObjectData[ContainerCount];
                        for (int i = 0; i < ContainerCount; i++)
                        {
                            ContainerDataArray[i] = new StatusBarObjectData();
                        }

                        int rowCount = 0;
                        for (int i = 0; i < ContainerCount; i++)
                        {
                            GameObject container;
                            if (i == 0)
                            {
                                container = transform.GetChild(0).gameObject;
                            }
                            else
                            {
                                container = Instantiate(GetStatusBarPrefab());
                                container.name = "Container";
                            }
                            RectTransform tform = container.GetComponent<RectTransform>();
                            tform.sizeDelta = Size;
                            GatherObjects(container.transform, ref ContainerDataArray[i]);

                            ContainerDataArray[i].ContainerObject = container;
                            if (i == CurrentValue / ValuePerContainer)
                                ContainerDataArray[i].CurrentValue = CurrentValue % ValuePerContainer;
                            else if (i < CurrentValue / ValuePerContainer)
                                ContainerDataArray[i].CurrentValue = ValuePerContainer;
                            else
                                ContainerDataArray[i].CurrentValue = 0;


                            ContainerDataArray[i].NormalizedValue = (float)ContainerDataArray[i].CurrentValue / (float)ValuePerContainer;

                            SetupImages(ref ContainerDataArray[i]);

                            if (ContainerDataArray[i].FillImage != null)
                            {
                                switch (ContFillMode)
                                {
                                    case ContainerFillMode.Horizontal:
                                        ContainerDataArray[i].FillImage.fillMethod = Image.FillMethod.Horizontal;
                                        ContainerDataArray[i].FillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                                        break;
                                    case ContainerFillMode.Vertical:
                                        ContainerDataArray[i].FillImage.fillMethod = Image.FillMethod.Vertical;
                                        ContainerDataArray[i].FillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                                        break;
                                    case ContainerFillMode.Radial:
                                        ContainerDataArray[i].FillImage.fillMethod = Image.FillMethod.Radial360;
                                        ContainerDataArray[i].FillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                                        ContainerDataArray[i].FillImage.fillClockwise = false;
                                        break;
                                }

                                ContainerDataArray[i].FillImage.fillAmount = ContainerDataArray[i].NormalizedValue;
                                UpdateFillColor(ContainerDataArray[i].FillImage, NormalizedValue);
                            }
                        }

                        ContainerData = new List<StatusBarObjectData>(ContainerDataArray);


                        int it = 0;
                        for (int i = 0; i < ContainerData.Count; i++)
                        {
                            GameObject container = ContainerData[i].ContainerObject;
                            container.transform.SetParent(transform, false);

                            if (it > 0 && ContainersPerRow > 0 && it % ContainersPerRow == 0)
                            {
                                if (!WrapContainers)
                                    break;

                                rowCount++;
                            }

                            if (container != gameObject && ContainersPerRow != 0)
                            {
                                switch (WrapDirection)
                                {
                                    case ContainerWrapDirection.Up:
                                        container.transform.localPosition = new Vector3(-(it % ContainersPerRow) * (Size.x + ContainerOffset.x), -(it * ContainerOffset.y) + (rowCount * (Size.y + WrapOffset.y)), 0);

                                        break;
                                    case ContainerWrapDirection.Down:
                                        container.transform.localPosition = new Vector3(-(it % ContainersPerRow) * (Size.x + ContainerOffset.x), (it * ContainerOffset.y) + (rowCount * -(Size.y + WrapOffset.y)), 0);
                                        break;
                                }
                            }
                            it++;
                        }
                    }
                }
                break;
        }

        BorderMaskImage.enabled = !CustomBorder;
    }

    void GatherObjects()
    {
        if (StatusBarObject == null)
        {
            StatusBarObject = transform.Find("StatusBar").gameObject;
        }
        if (FillObject == null)
        {
            FillObject = transform.Find("StatusBar/Background/Fill").gameObject;
            if (FillObject != null)
                FillImage = FillObject.GetComponent<Image>();
        }
        if (BackgroundObject == null)
        {
            BackgroundObject = transform.Find("StatusBar/Background").gameObject;
            if (BackgroundObject != null)
                BackgroundImage = BackgroundObject.GetComponent<Image>();
        }
        if (BorderObject == null)
        {
            BorderObject = transform.Find("StatusBar/Background/Fill/BorderMask/Border").gameObject;
            if (BorderObject != null)
                BorderImage = BorderObject.GetComponent<Image>();
        }
        if (BorderMaskObject == null)
        {
            BorderMaskObject = transform.Find("StatusBar/Background/Fill/BorderMask").gameObject;
            if (BorderMaskObject != null)
                BorderMaskImage = BorderMaskObject.GetComponent<Image>();
        }
        if (BarMaskObject == null)
        {
            BarMaskObject = transform.Find("StatusBar/Background/Fill/BarMask").gameObject;
            if (BarMaskObject != null)
                BarMaskImage = BarMaskObject.GetComponent<Image>();
        }
        if (AddTextOverlay && UseTMPro && (TMProOverlayTextObject == null || TMProOverlayText == null))
        {
            TMProOverlayTextObject = transform.Find("StatusBar/Background/Fill/TMPTextOverlay").gameObject;

            if (TMProOverlayTextObject != null)
                TMProOverlayText = TMProOverlayTextObject.GetComponent<TextMeshProUGUI>();
        }
        else if (AddTextOverlay &&  !UseTMPro && (OverlayTextObject == null || OverlayText == null))
        {
            OverlayTextObject = transform.Find("StatusBar/Background/Fill/TextOverlay").gameObject;

            if (OverlayTextObject != null)
                OverlayText = OverlayTextObject.GetComponent<Text>();
        }
    }

    void GatherObjects(Transform target, ref StatusBarObjectData objectData)
    {
        GameObject Object = target.Find("Background/Fill").gameObject;
        if (FillObject != null)
            objectData.FillImage = Object.GetComponent<Image>();

        Object = target.Find("Background").gameObject;
        if (BackgroundObject != null)
            objectData.BackgroundImage = Object.GetComponent<Image>();

        Object = target.Find("Background/Fill/BorderMask/Border").gameObject;
        if (BorderObject != null)
            objectData.BorderImage = Object.GetComponent<Image>();

        Object = target.Find("Background/Fill/BorderMask").gameObject;
        if (BorderMaskObject != null)
            objectData.BorderMaskImage = Object.GetComponent<Image>();

        Object = target.Find("Background/Fill/BarMask").gameObject;
        if (BarMaskObject != null)
            objectData.BarMaskImage = Object.GetComponent<Image>();
    }

    public void OnOverlayChanged()
    {
        UpdateValue();
        GatherObjects();

        if (AddTextOverlay && !BarType.ToString().Contains("Container"))
        {
            if (UseTMPro)
            {
                if (TMProOverlayTextObject != null)
                {
                    RectTransform rect = TMProOverlayTextObject.GetComponent<RectTransform>();
                    rect.localPosition = OverlayTextPosition;
                }
            }
            else
            {
                if (OverlayTextObject != null)
                {
                    RectTransform rect = OverlayTextObject.GetComponent<RectTransform>();
                    rect.localPosition = OverlayTextPosition;
                }
            }

            if (TMProOverlayTextObject != null)
            {
                TMProOverlayTextObject.SetActive(UseTMPro);
            }
            if (OverlayTextObject != null)
            {
                OverlayTextObject.SetActive(!UseTMPro);
            }

            switch (OverlayMode)
            {
                case OverlayTextMode.CurrentValue:
                    if (UseTMPro)
                    {
                        if (TMProOverlayText != null)
                        {
                            TMProOverlayText.text = CurrentValue.ToString();
                        }
                    }
                    else
                    {
                        if (OverlayText != null)
                        {
                            OverlayText.text = CurrentValue.ToString();
                        }
                    }
                    break;
                case OverlayTextMode.CurrentAndMaxValue:
                    if (UseTMPro)
                    {
                        if (TMProOverlayText != null)
                        {
                            TMProOverlayText.text = CurrentValue.ToString() + "/" + MaxValue.ToString();
                        }
                    }
                    else
                    {
                        if (OverlayText != null)
                        {
                            OverlayText.text = CurrentValue.ToString() + "/" + MaxValue.ToString();
                        }
                    }
                    break;
                case OverlayTextMode.Percent:
                    if (UseTMPro)
                    {
                        if (TMProOverlayText != null)
                        {
                            TMProOverlayText.text = string.Format("{0:F1} ", (NormalizedValue * 100f).ToString()).Trim() + "%";
                        }
                    }
                    else
                    {
                        if (OverlayText != null)
                        {
                            OverlayText.text = string.Format("{0:F1} ", (NormalizedValue * 100f).ToString()).Trim() + "%";
                        }
                    }
                    break;
                case OverlayTextMode.CurrentValueAndPercent:
                    if (UseTMPro)
                    {
                        if (TMProOverlayText != null)
                        {
                            TMProOverlayText.text = CurrentValue.ToString() + "(" + string.Format("{0:F1} ", (NormalizedValue * 100f).ToString()).Trim() + "%)";
                        }
                    }
                    else
                    {
                        if (OverlayText != null)
                        {
                            OverlayText.text = CurrentValue.ToString() + "(" + string.Format("{0:F1} ", (NormalizedValue * 100f).ToString()).Trim() + "%)";
                        }
                    }
                    break;
                case OverlayTextMode.Custom:
                    if (UseTMPro)
                    {
                        if (TMProOverlayText != null)
                            TMProOverlayText.text = CustomOverlayText;
                    }
                    else
                    {
                        if (OverlayText != null)
                            OverlayText.text = CustomOverlayText;
                    }
                    break;
            }
        }
        else
        {
            if (TMProOverlayTextObject != null)
            {
                TMProOverlayTextObject.SetActive(false);
            }
            if (OverlayTextObject != null)
            {
                OverlayTextObject.SetActive(false);
            }
        }
    }
    
    public void OnIncrementalTicksChanged()
    {
        GatherObjects();

        if (BarMaskObject != null)
        {
            for (int i = BarMaskObject.transform.childCount; i > 0; --i)
                DestroyImmediate(BarMaskObject.transform.GetChild(0).gameObject);
        }

        if (AddIncrementalTicks)
        {
            if (TickInterval > 0)
            {
                switch (TickMode)
                {
                    case IncrementalTickMode.Percent:
                        {
                            HandleIncrementalTicksPercent();
                        }
                        break;
                    case IncrementalTickMode.Value:
                        {
                            HandleIncrementalTickValue();
                        }
                        break;
                }
            }
        }
    }

    private void HandleIncrementalTicksPercent()
    {
        if (BarType == StatusBarType.FillLeft)
        {
            float range = FillImage.rectTransform.rect.width;
            float multiplier = range / 100f;

            int count = (int)TickInterval - 50;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(count * multiplier, 0, tform.localPosition.z);

                count += (int)TickInterval;
            } while (count < 50);
        }
        else if (BarType == StatusBarType.FillRight)
        {
            float range = FillImage.rectTransform.rect.width;
            float multiplier = range / 100f;

            int count = 50 - (int)TickInterval;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(count * multiplier, 0, tform.localPosition.z);

                count -= (int)TickInterval;
            } while (count > -50);
        }
        else if (BarType == StatusBarType.FillBottom)
        {
            float range = FillImage.rectTransform.rect.height;
            float multiplier = range / 100f;

            int count = (int)TickInterval - 50;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(0, count * multiplier, tform.localPosition.z);

                count += (int)TickInterval;
            } while (count < 50);
        }
        else if (BarType == StatusBarType.FillTop)
        {
            float range = FillImage.rectTransform.rect.height;
            float multiplier = range / 100f;

            int count = 50 - (int)TickInterval;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(0, count * multiplier, tform.localPosition.z);

                count -= (int)TickInterval;
            } while (count > -50);
        }
    }

    private void HandleIncrementalTickValue()
    {
        if (BarType == StatusBarType.FillLeft)
        {
            float range = FillImage.rectTransform.rect.width;
            float multiplier = range / 100f;

            int incrementAmount = (int)((TickInterval / MaxValue) * 100);
            int count = incrementAmount - 50;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(count * multiplier, 0, tform.localPosition.z);

                count += incrementAmount;
            } while (count < 50);
        }
        else if (BarType == StatusBarType.FillRight)
        {
            if (MaxValue > 0)
            {
                float range = FillImage.rectTransform.rect.width;
                float multiplier = range / 100f;

                int incrementAmount = (int)((TickInterval / MaxValue) * 100);
                int count = 50 - incrementAmount;
                do
                {
                    GameObject newTick = CreateTick();
                    RectTransform tform = newTick.GetComponent<RectTransform>();
                    tform.localPosition = new Vector3(count * multiplier, 0, tform.localPosition.z);

                    count -= incrementAmount;
                } while (count > -50);
            }
        }
        else if (BarType == StatusBarType.FillBottom)
        {
            float range = FillImage.rectTransform.rect.height;
            float multiplier = range / 100f;

            int incrementAmount = (int)((TickInterval / MaxValue) * 100);
            int count = incrementAmount - 50;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(0, count * multiplier, tform.localPosition.z);

                count += incrementAmount;
            } while (count < 50);
        }
        else if (BarType == StatusBarType.FillTop)
        {
            float range = FillImage.rectTransform.rect.height;
            float multiplier = range / 100f;

            int incrementAmount = (int)((TickInterval / MaxValue) * 100);
            int count = 50 - incrementAmount;
            do
            {
                GameObject newTick = CreateTick();
                RectTransform tform = newTick.GetComponent<RectTransform>();
                tform.localPosition = new Vector3(0, count * multiplier, tform.localPosition.z);

                count -= incrementAmount;
            } while (count > -50);
        }
    }

    private GameObject CreateTick()
    {
        GameObject Tick = new GameObject("Tick");
        Tick.transform.parent = BarMaskObject.transform;
        RectTransform rectTransform = Tick.AddComponent<RectTransform>();
        Image TickImage = Tick.AddComponent<Image>();

        rectTransform.sizeDelta = TickSize;
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);

        TickImage.sprite = TickSprite;
        TickImage.color = TickColor;

        return Tick;
    }
    void UpdateValue()
    {
        NormalizedValue = (float)CurrentValue / (float)MaxValue;
    }

    private void SetInitialValues()
    {
        MaxValue = 10;
        CurrentValue = 8;
    }
}
