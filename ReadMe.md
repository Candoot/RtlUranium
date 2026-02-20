# RtlUranium 🐝

**RTL (Right-to-Left) support for [UraniumUI](https://github.com/nicknisi/UraniumUI) Material Controls in .NET MAUI.**

Fixes Title animation, label alignment, border stroke gap, and icon positioning for RTL languages like Persian, Arabic, Hebrew, and Urdu.

## Installation
```bash
dotnet add package RtlUranium

Or via NuGet Package Manager:


Install-Package RtlUranium

## Usage

### XAML Namespace

xml
xmlns:rtl="clr-namespace:RtlUranium.Controls;assembly=RtlUranium"

### Basic Usage

xml
<rtl:RtlTextField Title="شماره موبایل"
Keyboard="Numeric"
FlowDirection="RightToLeft"
Text="{Binding PhoneNumber}" />

<rtl:RtlTextField Title="رمز عبور"
IsPassword="True"
FlowDirection="RightToLeft"
Text="{Binding Password}" />

### Force RTL Mode

xml
<rtl:RtlTextField Title="نام کاربری"
ForceRtl="True"
Text="{Binding Username}" />

### With Validation

xml
<rtl:RtlTextField Title="ایمیل"
FlowDirection="RightToLeft"
Text="{Binding Email}">
<validation:RequiredValidation />
<validation:RegexValidation Pattern="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" />
</rtl:RtlTextField>

## What's Fixed?

| Issue | LTR (Original) | RTL (RtlUranium) |
|-------|----------------|-------------------|
| Title AnchorX | 0 (left) | 1 (right) |
| Title TranslationX | Positive | Negative (calculated) |
| Title HorizontalOptions | Start | End |
| Title float animation | Moves right | Moves left |
| Icon position | Left side | Right side |
| Border gap | Left-top | Right-top |

## Supported Languages

- 🇮🇷 Persian (فارسی)
- 🇸🇦 Arabic (العربية)
- 🇮🇱 Hebrew (עברית)
- 🇵🇰 Urdu (اردو)
- Any RTL language

## Requirements

- .NET 8.0+
- .NET MAUI
- UraniumUI.Material 2.x

## License

MIT


---

## 📋 نحوه استفاده در پروژه Candoot

```xml
<!-- App.xaml - xmlns -->
xmlns:rtl="clr-namespace:RtlUranium.Controls;assembly=RtlUranium"

<!-- استایل‌ها -->
<Style TargetType="rtl:RtlTextField">
    <Setter Property="TextColor"
            Value="{AppThemeBinding Light={StaticResource PrimaryText},
                                    Dark={StaticResource PrimaryDarkText}}" />
    <Setter Property="BorderColor"
            Value="{AppThemeBinding Light=#E8DCC8, Dark=#3A3020}" />
    <Setter Property="AccentColor"
            Value="{AppThemeBinding Light={StaticResource Primary},
                                    Dark={StaticResource PrimaryDark}}" />
    <Setter Property="InputBackgroundColor"
            Value="{AppThemeBinding Light=#FFFCF5, Dark=#2A2415}" />
    <Setter Property="BackgroundColor" Value="Transparent" />
    <Setter Property="FontFamily" Value="DefaultFont"/>
    <Setter Property="FontSize" Value="15"/>
    <Setter Property="FlowDirection" Value="RightToLeft"/>
    <Setter Property="ForceRtl" Value="True"/>
    <Setter Property="TitleColor"
            Value="{AppThemeBinding Light={StaticResource Primary},
                                    Dark={StaticResource PrimaryDark}}" />
    <Setter Property="AllowClear" Value="True"/>
</Style>
