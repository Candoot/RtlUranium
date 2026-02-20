// Extensions/RtlExtensions.cs

namespace RtlUranium.Extensions;

/// <summary>
/// RTL detection helpers for MAUI VisualElements.
/// </summary>
public static class RtlExtensions
{
    /// <summary>
    /// Determines if the element's effective flow direction is RTL.
    /// Walks up the visual tree to check inherited FlowDirection.
    /// </summary>
    public static bool IsEffectivelyRtl(this VisualElement element)
    {
        if (element == null) return false;

        // ابتدا خود المان رو چک کن
        if (element.FlowDirection == FlowDirection.RightToLeft)
            return true;

        // اگه MatchParent بود، از visual tree بالا برو
        Element? current = element.Parent;
        while (current != null)
        {
            if (current is VisualElement ve)
            {
                if (ve.FlowDirection == FlowDirection.RightToLeft)
                    return true;
                if (ve.FlowDirection == FlowDirection.LeftToRight)
                    return false;
            }
            current = current.Parent;
        }

        // Fallback: CultureInfo
        var culture = System.Globalization.CultureInfo.CurrentUICulture;
        return culture.TextInfo.IsRightToLeft;
    }

    /// <summary>
    /// Safe TranslateTo that catches ObjectDisposedException.
    /// Replacement for UraniumUI's TranslateToSafely.
    /// </summary>
    public static async Task<bool> TranslateToSafe(this VisualElement element,
        double x, double y, uint length = 250, Easing? easing = null)
    {
        try
        {
            if (element?.Handler == null) return false;
            return await element.TranslateTo(x, y, length, easing);
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Safe ScaleTo that catches ObjectDisposedException.
    /// Replacement for UraniumUI's ScaleToSafely.
    /// </summary>
    public static async Task<bool> ScaleToSafe(this VisualElement element,
        double scale, uint length = 250, Easing? easing = null)
    {
        try
        {
            if (element?.Handler == null) return false;
            return await element.ScaleTo(scale, length, easing);
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
