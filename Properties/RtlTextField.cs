// Controls/RtlTextFieldProperties.cs

using System.ComponentModel;
using System.Windows.Input;
using UraniumUI.Resources;

namespace RtlUranium.Controls;

/// <summary>
/// BindableProperties for RtlTextField.
/// Separated for cleanliness — mirrors UraniumUI's TextField properties.
/// </summary>
public partial class RtlTextField
{
    #region Text
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(RtlTextField), string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) =>
        {
            if (b is RtlTextField tf)
                tf.OnPropertyChanged(nameof(HasValue));
        });
    #endregion

    #region TextColor
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(RtlTextField),
        ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.Black));
    #endregion

    #region IsPassword
    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword), typeof(bool), typeof(RtlTextField), false,
        propertyChanged: (b, _, n) =>
        {
            if (b is RtlTextField tf && tf.EntryView != null)
                tf.EntryView.IsPassword = (bool)n;
        });
    #endregion

    #region Placeholder
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(RtlTextField), string.Empty,
        propertyChanged: (b, _, n) =>
        {
            if (b is RtlTextField tf && tf.EntryView != null)
                tf.EntryView.Placeholder = (string)n;
        });
    #endregion

    #region PlaceholderColor
    public Color PlaceholderColor
    {
        get => (Color)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor), typeof(Color), typeof(RtlTextField), Colors.Gray,
        propertyChanged: (b, _, n) =>
        {
            if (b is RtlTextField tf && tf.EntryView != null)
                tf.EntryView.PlaceholderColor = (Color)n;
        });
    #endregion

    #region Keyboard
    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard), typeof(Keyboard), typeof(RtlTextField), Keyboard.Default,
        propertyChanged: (b, _, n) =>
        {
            if (b is RtlTextField tf && tf.EntryView != null)
                tf.EntryView.Keyboard = (Keyboard)n;
        });
    #endregion

    #region MaxLength
    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength), typeof(int), typeof(RtlTextField), int.MaxValue,
        propertyChanged: (b, _, n) =>
        {
            if (b is RtlTextField tf && tf.EntryView != null)
                tf.EntryView.MaxLength = (int)n;
        });
    #endregion

    #region IsReadOnly
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly), typeof(bool), typeof(RtlTextField), false);
    #endregion

    #region ReturnCommand
    public ICommand ReturnCommand
    {
        get => (ICommand)GetValue(ReturnCommandProperty);
        set => SetValue(ReturnCommandProperty, value);
    }

    public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(
        nameof(ReturnCommand), typeof(ICommand), typeof(RtlTextField));
    #endregion

    #region ReturnCommandParameter
    public object ReturnCommandParameter
    {
        get => GetValue(ReturnCommandParameterProperty);
        set => SetValue(ReturnCommandParameterProperty, value);
    }

    public static readonly BindableProperty ReturnCommandParameterProperty = BindableProperty.Create(
        nameof(ReturnCommandParameter), typeof(object), typeof(RtlTextField));
    #endregion

    #region SelectionLength / CursorPosition
    public int SelectionLength
    {
        get => (int)GetValue(SelectionLengthProperty);
        set => SetValue(SelectionLengthProperty, value);
    }

    public static readonly BindableProperty SelectionLengthProperty = BindableProperty.Create(
        nameof(SelectionLength), typeof(int), typeof(RtlTextField), 0);

    public int CursorPosition
    {
        get => (int)GetValue(CursorPositionProperty);
        set => SetValue(CursorPositionProperty, value);
    }

    public static readonly BindableProperty CursorPositionProperty = BindableProperty.Create(
        nameof(CursorPosition), typeof(int), typeof(RtlTextField), 0);
    #endregion

    #region AllowClear
    public bool AllowClear
    {
        get => (bool)GetValue(AllowClearProperty);
        set => SetValue(AllowClearProperty, value);
    }

    public static readonly BindableProperty AllowClearProperty = BindableProperty.Create(
        nameof(AllowClear), typeof(bool), typeof(RtlTextField), false,
        propertyChanged: (b, _, _) => (b as RtlTextField)?.OnAllowClearChanged());
    #endregion

    #region DisallowClearButtonFocus
    public bool DisallowClearButtonFocus
    {
        get => (bool)GetValue(DisallowClearButtonFocusProperty);
        set => SetValue(DisallowClearButtonFocusProperty, value);
    }

    public static readonly BindableProperty DisallowClearButtonFocusProperty = BindableProperty.Create(
        nameof(DisallowClearButtonFocus), typeof(bool), typeof(RtlTextField), false);
    #endregion

    #region SelectAllTextOnFocus
    public bool SelectAllTextOnFocus
    {
        get => (bool)GetValue(SelectAllTextOnFocusProperty);
        set => SetValue(SelectAllTextOnFocusProperty, value);
    }

    public static readonly BindableProperty SelectAllTextOnFocusProperty = BindableProperty.Create(
        nameof(SelectAllTextOnFocus), typeof(bool), typeof(RtlTextField), false);
    #endregion

    #region SelectionHighlightColor
    public Color SelectionHighlightColor
    {
        get => (Color)GetValue(SelectionHighlightColorProperty);
        set => SetValue(SelectionHighlightColorProperty, value);
    }

    public static readonly BindableProperty SelectionHighlightColorProperty = BindableProperty.Create(
        nameof(SelectionHighlightColor), typeof(Color), typeof(RtlTextField),
        ColorResource.GetColor("Primary", "PrimaryDark", Colors.Purple));
    #endregion

    #region HorizontalTextAlignment
    public TextAlignment HorizontalTextAlignment
    {
        get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
        nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(RtlTextField),
        TextAlignment.Start);
    #endregion

    #region ReturnType
    public ReturnType ReturnType
    {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }

    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
        nameof(ReturnType), typeof(ReturnType), typeof(RtlTextField), ReturnType.Default,
        propertyChanged: (b, _, n) =>
        {
            if (b is RtlTextField tf && tf.EntryView != null)
                tf.EntryView.ReturnType = (ReturnType)n;
        });
    #endregion
}
