using System.Windows;
using System.Windows.Controls;

namespace Smart.Sharp.Core.Helpers
{
  /// <summary>
  /// Helper to set layout of child elements in a panel. For example setting Margin of all children of a Grid or Stackpanel.
  /// </summary>
  public class PanelLayoutHelper
  {

    #region attached properties

    #region Margin

    public static Thickness GetMargin(DependencyObject obj)
    {
      return (Thickness)obj.GetValue(MarginProperty);
    }

    public static void SetMargin(DependencyObject obj, Thickness value)
    {
      obj.SetValue(MarginProperty, value);
    }

    public static DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof(Thickness),
      typeof(PanelLayoutHelper), new UIPropertyMetadata(new Thickness(), OnMarginChanged));

    private static void OnMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Panel panel = d as Panel;
      if (panel == null)
        return;
      panel.Loaded += SetMarginOnLoaded;
    }

    private static void SetMarginOnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Panel panel = sender as Panel;
      if (panel == null)
        return;

      foreach (UIElement child in panel.Children)
      {
        FrameworkElement frameworkElement = child as FrameworkElement;
        if (frameworkElement == null)
          continue;

        frameworkElement.Margin = GetMargin(panel);
      }
    }

    #endregion

    #region ContentVerticalAlignment

    public static VerticalAlignment GetContentVerticalAlignment(DependencyObject obj)
    {
      return (VerticalAlignment)obj.GetValue(ContentVerticalAlignmentProperty);
    }

    public static void SetContentVerticalAlignment(DependencyObject obj, VerticalAlignment alignment)
    {
      obj.SetValue(ContentVerticalAlignmentProperty, alignment);
    }

    public static DependencyProperty ContentVerticalAlignmentProperty =
      DependencyProperty.RegisterAttached("ContentVerticalAlignment", typeof(VerticalAlignment),
        typeof(PanelLayoutHelper), new UIPropertyMetadata(VerticalAlignment.Top, OnContentVerticalAlignmentChanged));

    private static void OnContentVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Panel panel = d as Panel;
      if (panel == null)
        return;
      panel.Loaded += SetContentVerticalAlignmentOnLoaded;
    }

    private static void SetContentVerticalAlignmentOnLoaded(object sender, RoutedEventArgs e)
    {
      Panel panel = sender as Panel;
      if (panel == null)
        return;

      foreach (UIElement child in panel.Children)
      {
        FrameworkElement frameworkElement = child as FrameworkElement;
        if (frameworkElement == null)
          continue;

        frameworkElement.VerticalAlignment = GetContentVerticalAlignment(panel);
      }
    }

    #endregion

    #endregion

  }
}
