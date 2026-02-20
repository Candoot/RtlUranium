// Controls/RtlTextField.cs

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Text;
using Plainer.Maui.Controls;
using RtlUranium.Extensions;
using System.Windows.Input;
using UraniumUI.Converters;
using UraniumUI.Material.Controls;
using UraniumUI.Material.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.ViewExtensions;
using UraniumUI.Views;
using static System.Net.Mime.MediaTypeNames;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace RtlUranium.Controls;

/// <summary>
/// RTL-aware TextField built on top of RtlInputField.
/// 
/// This is a near-complete replica of UraniumUI's TextField,
/// but inheriting from RtlInputField instead of InputField,
/// giving us proper RTL support for:
/// 
///   ✅ Title label animation (float up/down)
///   ✅ Title label anchor point (AnchorX = 1 for RTL)
///   ✅ StrokeDashArray border gap position
///   ✅ Icon positioning
///   ✅ Entry text alignment
///   ✅ Clear button positioning
///   ✅ Placeholder alignment
/// </summary>
[ContentProperty(nameof(Validations))]
public partial class RtlTextField : RtlInputField
{
    // ─── Entry Access ──────────────────────────
    public EntryView EntryView =>
        this.FindByViewQueryIdInVisualTreeDescendants<EntryView>("EntryView");

    // ─── Content ───────────────────────────────
    public override View Content { get; set; } = new EntryView
    {
#if WINDOWS
        Margin = new Thickness(2, 0),
#else
        Margin = new Thickness(16, 0),
#endif
        BackgroundColor = Colors.Transparent,
        VerticalOptions = LayoutOptions.Center,
    };

    // ─── HasValue ──────────────────────────────
    public override bool HasValue => !string.IsNullOrEmpty(Text);

    // ─── Behaviors ─────────────────────────────
    public IList<Behavior>? EntryBehaviors => EntryView?.Behaviors;

    // ─── Events ────────────────────────────────
    public event EventHandler<TextChangedEventArgs>? TextChanged;
    public event EventHandler? Completed;

    // ─── Commands ──────────────────────────────
    public ICommand? ClearCommand { get; protected set; }

    // ═══════════════════════════════════════════
    //  Constructor
    // ═══════════════════════════════════════════
    public RtlTextField()
    {
        base.RegisterForEvents();

        var entryView = Content as EntryView;
        if (entryView == null) return;

        entryView.SetId("EntryView");

        UpdateClearIconState();

        // ── Bindings: Entry ↔ RtlTextField ──
        entryView.SetBinding(Entry.TextProperty,
            new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.TextColorProperty,
            new Binding(nameof(TextColor), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.ReturnCommandParameterProperty,
            new Binding(nameof(ReturnCommandParameter), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.ReturnCommandProperty,
            new Binding(nameof(ReturnCommand), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.SelectionLengthProperty,
            new Binding(nameof(SelectionLength), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.CursorPositionProperty,
            new Binding(nameof(CursorPosition), BindingMode.TwoWay, source: this));
        entryView.SetBinding(Entry.IsEnabledProperty,
            new Binding(nameof(IsEnabled), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.IsReadOnlyProperty,
            new Binding(nameof(IsReadOnly), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.FontSizeProperty,
            new Binding(nameof(FontSize), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.FontFamilyProperty,
            new Binding(nameof(FontFamily), BindingMode.OneWay, source: this));
        entryView.SetBinding(Entry.FontAutoScalingEnabledProperty,
            new Binding(nameof(FontAutoScalingEnabled), BindingMode.OneWay, source: this));

        // ── RTL-specific: Entry text alignment ──
        entryView.SetBinding(Entry.HorizontalTextAlignmentProperty,
            new Binding(nameof(HorizontalTextAlignment), BindingMode.OneWay, source: this));
    }

    // ═══════════════════════════════════════════
    //  Handler Changed
    // ═══════════════════════════════════════════
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            if (EntryView != null)
            {
                EntryView.TextChanged -= EntryView_TextChanged;
                EntryView.Completed -= EntryView_Completed;
                EntryView.Focused -= EntryView_Focused;
            }
        }
        else
        {
            if (EntryView != null)
            {
                EntryView.TextChanged += EntryView_TextChanged;
                EntryView.Completed += EntryView_Completed;
                EntryView.Focused += EntryView_Focused;

                ApplyAttachedProperties();
                ApplyRtlToEntry();
            }
        }
    }

    /// <summary>
    /// Applies RTL-specific settings to the underlying Entry.
    /// </summary>
    protected virtual void ApplyRtlToEntry()
    {
        if (EntryView == null) return;

        if (IsRtl)
        {
            EntryView.FlowDirection = FlowDirection.RightToLeft;
            // اگه HorizontalTextAlignment تنظیم نشده، پیش‌فرض RTL بذار
            if (HorizontalTextAlignment == TextAlignment.Start)
            {
                EntryView.HorizontalTextAlignment = TextAlignment.End;
            }
        }
    }

    protected virtual void ApplyAttachedProperties()
    {
        if (EntryView != null)
        {
            EntryProperties.SetSelectionHighlightColor(EntryView, SelectionHighlightColor);
        }
    }

    // ═══════════════════════════════════════════
    //  Event Handlers
    // ═══════════════════════════════════════════
    private void EntryView_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.OldTextValue) || string.IsNullOrEmpty(e.NewTextValue))
        {
            UpdateState();
        }

        if (e.NewTextValue != null)
        {
            CheckAndShowValidations();
        }

        TextChanged?.Invoke(this, e);
    }

    private void EntryView_Completed(object? sender, EventArgs e)
    {
        Completed?.Invoke(this, e);
    }

    private void EntryView_Focused(object? sender, FocusEventArgs e)
    {
        if (SelectAllTextOnFocus)
        {
            SelectAllText();
        }
    }

    // ═══════════════════════════════════════════
    //  Public Methods
    // ═══════════════════════════════════════════
    public void ClearValue()
    {
        if (IsEnabled)
        {
            Text = string.Empty;
        }
    }

    public void SelectAllText()
    {
        if (EntryView?.Text?.Length > 0)
        {
            EntryView.CursorPosition = 0;
            EntryView.SelectionLength = EntryView.Text.Length;

#if ANDROID
            if (EntryView.Handler?.PlatformView is Android.Widget.EditText editText)
            {
                editText.SelectAll();
            }
#endif
        }
    }

    // ═══════════════════════════════════════════
    //  Validation
    // ═══════════════════════════════════════════
    protected override object? GetValueForValidator()
    {
        return EntryView?.Text;
    }

    public override void ResetValidation()
    {
        if (EntryView != null)
        {
            EntryView.Text = string.Empty;
        }
        base.ResetValidation();
    }

    // ═══════════════════════════════════════════
    //  Clear Icon
    // ═══════════════════════════════════════════
    protected virtual void OnClearTapped()
    {
        if (EntryView != null)
        {
            EntryView.Text = string.Empty;
        }
    }

    protected virtual void OnAllowClearChanged()
    {
        UpdateClearIconState();
    }

    protected virtual void UpdateClearIconState()
    {
        if (endIconsContainer is null) return;

        var existing = endIconsContainer
            .FindByViewQueryIdInVisualTreeDescendants<StatefulContentView>("ClearIcon");

        if (AllowClear)
        {
            if (existing == null)
            {
                var iconClear = CreateIconClear();
                endIconsContainer.Add(iconClear);
            }
        }
        else
        {
            if (existing != null)
            {
                endIconsContainer.Remove(existing);
            }
        }
    }

    protected virtual View CreateIconClear()
    {
        var contentView = new StatefulContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            IsVisible = true,
            Padding = new Thickness(5, 0),
            Margin = new Thickness(0, 0, 5, 0),
            TappedCommand = new Command(OnClearTapped),
            Content = new Path
            {
                StyleClass = new[] { "TextField.ClearIcon" },
                Data = UraniumShapes.X,
                Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray)
                    .WithAlpha(.5f),
            }
        };

        contentView.SetId("ClearIcon");
        contentView.SetBinding(StatefulContentView.IsFocusableProperty,
            new Binding(nameof(DisallowClearButtonFocus), source: this));
        contentView.SetBinding(IsVisibleProperty,
            new Binding(nameof(Text),
                converter: UraniumConverters.StringIsNotNullOrEmptyConverter,
                source: this));

        return contentView;
    }
}
