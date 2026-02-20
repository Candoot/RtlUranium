// Controls/RtlInputField.cs

using Microsoft.Maui.Controls.Shapes;
using RtlUranium.Extensions;
using UraniumUI.Material.Controls;
using UraniumUI.ViewExtensions;
using UraniumUI.Resources;

namespace RtlUranium.Controls;

/// <summary>
/// RTL-aware InputField that fixes:
/// 1. Title label animation direction for RTL languages
/// 2. StrokeDashArray calculation for RTL border gap
/// 3. Title label alignment (AnchorX, HorizontalOptions)
/// 4. Icon positioning in RTL mode
/// 
/// Inherits from InputField directly — NO Reflection needed!
/// All base members (labelTitle, border, imageIcon, etc.) are `protected`
/// in the original UraniumUI source code.
/// </summary>
[ContentProperty(nameof(Content))]
public partial class RtlInputField : InputField
{
    // ─── Constants (same as base, but accessible here) ───
    // FirstDash = 6 (internal const in InputField)
    // MaxCornerRadius = 24 (internal const in InputField)
    private const double _firstDash = 6;
    private const double _maxCornerRadius = 24;

    // ─── Cached RTL state ───
    private bool _isRtl;
    private bool _rtlResolved;

    /// <summary>
    /// Force RTL mode regardless of FlowDirection detection.
    /// Useful when you want to ensure RTL behavior.
    /// </summary>
    public bool ForceRtl
    {
        get => (bool)GetValue(ForceRtlProperty);
        set => SetValue(ForceRtlProperty, value);
    }

    public static readonly BindableProperty ForceRtlProperty = BindableProperty.Create(
        nameof(ForceRtl), typeof(bool), typeof(RtlInputField), false,
        propertyChanged: (b, _, _) =>
        {
            if (b is RtlInputField rtl)
            {
                rtl._rtlResolved = false; // re-evaluate
                rtl.UpdateState();
            }
        });

    /// <summary>
    /// Resolves whether we're in RTL mode.
    /// Caches the result until layout changes.
    /// </summary>
    protected bool IsRtl
    {
        get
        {
            if (!_rtlResolved)
            {
                _isRtl = ForceRtl || this.IsEffectivelyRtl();
                _rtlResolved = true;
            }
            return _isRtl;
        }
    }

    public RtlInputField()
    {
    }

    // ─── Override: Fix Title alignment after handler is set ───
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
        {
            Dispatcher.Dispatch(FixTitleRtlAlignment);
        }
    }

    // ─── Override: Re-evaluate RTL on size change ───
    protected override async void OnSizeAllocated(double width, double height)
    {
        _rtlResolved = false; // FlowDirection might have changed

        base.OnSizeAllocated(width, height);

        // base already does Task.Delay(100) + InitializeBorder()
        // We add RTL fix after that
        await Task.Delay(120);
        FixTitleRtlAlignment();
    }

    /// <summary>
    /// Fixes the Title label's HorizontalOptions, FlowDirection, and AnchorX for RTL.
    /// </summary>
    protected virtual void FixTitleRtlAlignment()
    {
        if (labelTitle == null) return;

        if (IsRtl)
        {
            labelTitle.FlowDirection = FlowDirection.RightToLeft;
            labelTitle.HorizontalOptions = LayoutOptions.End;
            labelTitle.HorizontalTextAlignment = TextAlignment.End;
        }
        else
        {
            labelTitle.FlowDirection = FlowDirection.LeftToRight;
            labelTitle.HorizontalOptions = LayoutOptions.Start;
            labelTitle.HorizontalTextAlignment = TextAlignment.Start;
        }
    }

    /// <summary>
    /// Core RTL fix: Override UpdateState to handle RTL title animation.
    /// 
    /// In the original UraniumUI code, UpdateState() only handles LTR:
    ///   - AnchorX = 0 (left anchor)
    ///   - TranslateX = positive (move right)
    ///   - StrokeDashArray gap on the left side
    ///
    /// For RTL we need:
    ///   - AnchorX = 1 (right anchor)
    ///   - TranslateX = negative (move left, or calculated for right side)
    ///   - StrokeDashArray gap position adjusted
    /// </summary>
    protected override void UpdateState()
    {
        if (Content is null) return;
        if (border == null || labelTitle == null) return;

        if (border.StrokeDashArray == null ||
            border.StrokeDashArray.Count == 0 ||
            labelTitle.Width <= 0)
        {
            return;
        }

        // اگه RTL نیست → از base استفاده کن
        if (!IsRtl)
        {
            base.UpdateState();
            return;
        }

        // ═══════════════════════════════════════════
        //  RTL-specific UpdateState
        // ═══════════════════════════════════════════

        using (border.Batch())
        using (labelTitle.Batch())
        {
            if (HasValue || Content.IsFocused)
            {
                // ── State: Focused or Has Value ──
                // Title should float up and scale down

                UpdateOffset(0.01);

                labelTitle.CancelAnimations();

                // برای RTL: نقطه anchor سمت راسته
                labelTitle.AnchorX = 1;

                // محاسبه X: فاصله از سمت راست
                var cornerClamped = Math.Max(10, Math.Min(CornerRadius, _maxCornerRadius));
                var baseOffset = cornerClamped - 10;

                // در RTL، لیبل از سمت راست شروع میشه
                // پس TranslationX منفی میشه (حرکت به سمت چپ نسبت به anchor)
                double x = -(labelTitle.Width - baseOffset);

                if (HasValue)
                {
                    // بدون انیمیشن (مثلاً وقتی صفحه لود میشه و مقدار داره)
                    labelTitle.TranslationX = x;
                    labelTitle.TranslationY = -25;
                    labelTitle.Scale = 0.8;
                }
                else
                {
                    // با انیمیشن (وقتی فوکوس میگیره)
                    labelTitle.TranslateToSafe(x, -25, 90, Easing.BounceOut);
                    labelTitle.ScaleToSafe(0.8, 90);
                }
            }
            else
            {
                // ── State: Unfocused & Empty ──
                // Title should return to normal position inside the border

                var dashArray = border.StrokeDashArray;
                var offsetToGo = dashArray[0] + dashArray[1] + _firstDash;

                UpdateOffset(offsetToGo);

                labelTitle.CancelAnimations();

                // در RTL: آیکون سمت راسته
                labelTitle.AnchorX = 1;

                double iconWidth = imageIcon.IsValueCreated ? imageIcon.Value.Width : 0;
                double x = -iconWidth; // حرکت به سمت چپ به اندازه آیکون

                labelTitle.TranslateToSafe(x, 0, 90, Easing.BounceOut);
                labelTitle.ScaleToSafe(1, 90);
            }
        }
    }
}
