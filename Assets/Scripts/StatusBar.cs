using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusBar : MonoBehaviour
{
    [SerializeField] public StatusBarType BarType;

    //ObjectLinks

    //Preferences
    [SerializeField] public Vector2Int Size;
    [SerializeField] public Sprite SourceImage;
    [SerializeField] public Color BackgroundColor;
    [SerializeField] public Color BorderColor;


    //Fill Color
    [SerializeField] public Gradient FillGradient;
    [SerializeField] public GradientColorMode FillGradientMode;
    
    //Custom Options
    [SerializeField] public bool MatchDimensionsForCustomSprites;

    [SerializeField] public bool CustomFill;
    [SerializeField] public Sprite CustomFillSprite;

    [SerializeField] public bool CustomBorder;
    [SerializeField] public Sprite CustomBorderSprite;

    [SerializeField] public bool CustomBackground;
    [SerializeField] public Sprite CustomBackgroundSprite;

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

    //Data
    [HideInInspector][SerializeField] int MaxValue;
    [HideInInspector] [SerializeField] int CurrentValue;
    float NormalizedValue;

    private void Reset()
    {
        Debug.Log("Status Bar Created");
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
        GatherObjects();
        NormalizedValue = (float)CurrentValue / (float)MaxValue;
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

            UpdateFillColor();
        }
    }

    public int SetValue(int amount)
    {
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

        return CurrentValue;
    }

    public int AdjustValue(int amount)
    {
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

        return CurrentValue;
    }

    public void OnBarTypeChanged()
    {
        InitImages();
        OnIncrementalTicksChanged();
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
    }

    private void UpdateFillColor()
    {
        if (FillImage != null)
        {
            switch (FillGradientMode)
            {
                case GradientColorMode.Blend:
                    FillImage.color = FillGradient.Evaluate(FillImage.fillAmount);
                    break;
                case GradientColorMode.Threshold:
                    {
                        int index = 0;
                        foreach (var key in FillGradient.colorKeys)
                        {
                            if (FillImage.fillAmount >= FillGradient.colorKeys[index].time)
                            {
                                index++;
                                continue;
                            }
                            FillImage.color = FillGradient.colorKeys[index].color;
                        }
                        break;
                    }
                case GradientColorMode.Solid:
                    FillImage.color = FillGradient.Evaluate(0);
                    break;
            }
        }
    }

    void InitImages()
    {
        UpdateValue();
        GatherObjects();

        if (BarType.ToString().Contains("Fill"))
        {
            if (FillImage != null)
            {
                FillImage.sprite = CustomFill ? CustomFillSprite : SourceImage;
                FillImage.type = Image.Type.Filled;
                FillImage.fillAmount = NormalizedValue;

                UpdateFillColor();
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
            if (CustomBorder && BackgroundImage != null)
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
                break;
            case StatusBarType.ContainerRight:
                break;
        }

        BorderMaskImage.enabled = !CustomBorder;
    }

    void GatherObjects()
    {
        if (FillObject == null)
        {
            FillObject = transform.Find("Background/Fill").gameObject;
            if (FillObject != null)
                FillImage = FillObject.GetComponent<Image>();
        }
        if (BackgroundObject == null)
        {
            BackgroundObject = transform.Find("Background").gameObject;
            if (BackgroundObject != null)
                BackgroundImage = BackgroundObject.GetComponent<Image>();
        }
        if (BorderObject == null)
        {
            BorderObject = transform.Find("Background/Fill/BorderMask/Border").gameObject;
            if (BorderObject != null)
                BorderImage = BorderObject.GetComponent<Image>();
        }
        if (BorderMaskObject == null)
        {
            BorderMaskObject = transform.Find("Background/Fill/BorderMask").gameObject;
            if (BorderMaskObject != null)
                BorderMaskImage = BorderMaskObject.GetComponent<Image>();
        }
        if (BarMaskObject == null)
        {
            BarMaskObject = transform.Find("Background/Fill/BarMask").gameObject;
            if (BarMaskObject != null)
                BarMaskImage = BarMaskObject.GetComponent<Image>();
        }
        if (AddTextOverlay && UseTMPro && (TMProOverlayTextObject == null || TMProOverlayText == null))
        {
            TMProOverlayTextObject = transform.Find("Background/Fill/TMPTextOverlay").gameObject;

            if (TMProOverlayTextObject != null)
                TMProOverlayText = TMProOverlayTextObject.GetComponent<TextMeshProUGUI>();
        }
        else if (AddTextOverlay &&  !UseTMPro && (OverlayTextObject == null || OverlayText == null))
        {
            OverlayTextObject = transform.Find("Background/Fill/TextOverlay").gameObject;

            if (OverlayTextObject != null)
                OverlayText = OverlayTextObject.GetComponent<Text>();
        }
    }

    public void OnOverlayChanged()
    {
        UpdateValue();
        GatherObjects();

        if (AddTextOverlay)
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
