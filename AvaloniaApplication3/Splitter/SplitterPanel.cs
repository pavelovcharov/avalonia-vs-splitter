using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace AvaloniaApplication3;

public class SplitterPanel : Panel
{
    public static readonly AttachedProperty<SplitterLength> SplitterLengthProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, SplitterLength>("SplitterLength",
            new SplitterLength(100.0));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SplitterPanel, Orientation>(nameof(Orientation), Orientation.Vertical);

    // public static readonly StyledProperty<bool> ShowResizePreviewProperty = AvaloniaProperty.Register<SPl(nameof (ShowResizePreview), typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.BooleanFalse));
    public static readonly AttachedProperty<double> MinimumLengthProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, double>("MinimumLength", 0);

    public static readonly AttachedProperty<double> MaximumLengthProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, double>("MaximumLength", double.MaxValue);

    private static readonly AttachedProperty<double> ActualSplitterLengthProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, double>("ActualSplitterLength", 0);

    private static readonly AttachedProperty<int> IndexProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, int>("Index", -1);

    private static readonly AttachedProperty<bool> IsFirstProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, bool>("IsFirst", false);

    private static readonly AttachedProperty<bool> IsLastProperty =
        AvaloniaProperty.RegisterAttached<SplitterPanel, AvaloniaObject, bool>("IsLast", false);
    // public static readonly DependencyProperty ActualSplitterLengthProperty = SplitterPanel.ActualSplitterLengthPropertyKey.DependencyProperty;
    // public static readonly DependencyProperty IndexProperty = SplitterPanel.IndexPropertyKey.DependencyProperty;
    // public static readonly DependencyProperty IsFirstProperty = SplitterPanel.IsFirstPropertyKey.DependencyProperty;
    // public static readonly DependencyProperty IsLastProperty = SplitterPanel.IsLastPropertyKey.DependencyProperty;
    // private SplitterResizePreviewWindow currentPreviewWindow;

    static SplitterPanel()
    {
        // FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterPanel)));
        AffectsParentMeasure<SplitterPanel>(SplitterLengthProperty);
        AffectsParentArrange<SplitterPanel>(SplitterLengthProperty);
        AffectsMeasure<SplitterPanel>(OrientationProperty);
    }

    public SplitterPanel()
    {
        Thumb.DragStartedEvent.Raised.Subscribe(new AnonymousObserver<(object, RoutedEventArgs)>(x =>
        {
            _ = x;
            OnSplitterDragStarted(x.Item1, (VectorEventArgs)x.Item2);
        }));
        // this.AddHandler(Thumb.DragStartedEvent, (Delegate) new DragStartedEventHandler(this.OnSplitterDragStarted));
        // AutomationProperties.SetAutomationId((DependencyObject) this, nameof (SplitterPanel));
    }

    // private bool IsShowingResizePreview => this.currentPreviewWindow != null;

    public static double GetActualSplitterLength(AvaloniaObject element)
    {
        return element.GetValue(ActualSplitterLengthProperty);
    }

    protected static void SetActualSplitterLength(AvaloniaObject element, double value)
    {
        element.SetValue(ActualSplitterLengthProperty, (object) value);
    }

    public static int GetIndex(AvaloniaObject element)
    {
        return element.GetValue(IndexProperty);
    }

    public static bool GetIsFirst(AvaloniaObject element)
    {
        return element.GetValue(IsFirstProperty);
    }

    protected static void SetIsFirst(AvaloniaObject element, bool value)
    {
        element.SetValue(IsFirstProperty, value);
    }

    public static bool GetIsLast(AvaloniaObject element)
    {
        return element.GetValue(IsLastProperty);
    }

    protected static void SetIsLast(AvaloniaObject element, bool value)
    {
        element.SetValue(IsLastProperty, value);
    }

    protected static void SetIndex(AvaloniaObject element, int value)
    {
        element.SetValue(IndexProperty, (object) value);
    }

    public static SplitterLength GetSplitterLength(AvaloniaObject element)
    {
        return element != null
            ? element.GetValue(SplitterLengthProperty)
            : throw new ArgumentNullException(nameof(element));
    }

    public static void SetSplitterLength(AvaloniaObject element, SplitterLength value)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));
        element.SetValue(SplitterLengthProperty, (object) value);
    }

    public static double GetMinimumLength(AvaloniaObject element)
    {
        return element != null
            ? element.GetValue(MinimumLengthProperty)
            : throw new ArgumentNullException(nameof(element));
    }

    public static void SetMinimumLength(AvaloniaObject element, double value)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));
        element.SetValue(MinimumLengthProperty, (object) value);
    }

    public static double GetMaximumLength(AvaloniaObject element)
    {
        return element != null
            ? element.GetValue(MaximumLengthProperty)
            : throw new ArgumentNullException(nameof(element));
    }

    public static void SetMaximumLength(AvaloniaObject element, double value)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));
        element.SetValue(MaximumLengthProperty, (object) value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, (object) value);
    }

    // public bool ShowResizePreview
    // {
    //   get => (bool) this.GetValue(SplitterPanel.ShowResizePreviewProperty);
    //   set => this.SetValue(SplitterPanel.ShowResizePreviewProperty, Boxes.Box(value));
    // }

    private void UpdateIndices()
    {
        int count = Children.Count;
        int num = count - 1;
        for (int index = 0; index < count; ++index)
        {
            Visual internalChild = Children[index];
            SetIndex(internalChild, index);
            SetIsFirst(internalChild, index == 0);
            SetIsLast(internalChild, index == num);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateIndices();
        return Measure(availableSize, Orientation,
            SplitterMeasureData.FromElements(Children), true);
    }

    private static Size MeasureNonreal(Size availableSize, Orientation orientation, IEnumerable<SplitterMeasureData> measureData, bool remeasureElements)
    {
        double num1 = 0.0;
        double num2 = 0.0;
        foreach (SplitterMeasureData splitterMeasureData in measureData)
        {
            if (remeasureElements)
                splitterMeasureData.Element.Measure(availableSize);
            if (orientation == Orientation.Horizontal)
            {
                num1 += splitterMeasureData.Element.DesiredSize.Width;
                num2 = Math.Max(num2, splitterMeasureData.Element.DesiredSize.Height);
            }
            else
            {
                num1 = Math.Max(num1, splitterMeasureData.Element.DesiredSize.Width);
                num2 += splitterMeasureData.Element.DesiredSize.Height;
            }
        }

        Rect rect = new Rect(0.0, 0.0, num1, num2);
        foreach (SplitterMeasureData splitterMeasureData in measureData)
        {
            if (orientation == Orientation.Horizontal)
            {
                rect = rect.WithWidth(splitterMeasureData.Element.DesiredSize.Width);
                splitterMeasureData.MeasuredBounds = rect;
                rect = rect.Translate(new Vector(rect.Width, 0));
            }
            else
            {
                rect.WithHeight(splitterMeasureData.Element.DesiredSize.Height);
                splitterMeasureData.MeasuredBounds = rect;
                rect.Translate(new Vector(0, rect.Height));
            }
        }

        return new Size(num1, num2);
    }

    public static Size Measure(
        Size availableSize,
        Orientation orientation,
        IEnumerable<SplitterMeasureData> measureData,
        bool remeasureElements)
    {
        double valueStretch = 0.0;
        double valueFill = 0.0;
        double minFill = 0.0;
        double minStretch = 0.0;
        if (orientation == Orientation.Horizontal && LayoutDoubleUtil.IsNonreal(availableSize.Width) ||
            orientation == Orientation.Vertical && LayoutDoubleUtil.IsNonreal(availableSize.Height))
            return MeasureNonreal(availableSize, orientation, measureData, remeasureElements);
        foreach (SplitterMeasureData splitterMeasureData in measureData)
        {
            SplitterLength attachedLength = splitterMeasureData.AttachedLength;
            double minimumLength = GetMinimumLength(splitterMeasureData.Element);
            if (attachedLength.IsStretch)
            {
                valueStretch += attachedLength.Value;
                minStretch += minimumLength;
            }
            else
            {
                valueFill += attachedLength.Value;
                minFill += minimumLength;
            }

            splitterMeasureData.IsMinimumReached = false;
            splitterMeasureData.IsMaximumReached = false;
        }

        double minTotal = minStretch + minFill;
        double width = availableSize.Width;
        double height = availableSize.Height;
        double availableLength = orientation == Orientation.Horizontal ? width : height;
        double remainsForFill = valueFill == 0.0 ? 0.0 : Math.Max(0.0, availableLength - valueStretch);
        double remainsForStretch = remainsForFill == 0.0 ? availableLength : valueStretch;
        if (minTotal <= availableLength)
        {
            foreach (SplitterMeasureData splitterMeasureData in measureData)
            {
                SplitterLength attachedLength = splitterMeasureData.AttachedLength;
                double maximumLength = GetMaximumLength(splitterMeasureData.Element);
                if (attachedLength.IsStretch && (valueStretch == 0.0 ? 0.0 : attachedLength.Value / valueStretch * remainsForStretch) > maximumLength)
                {
                    splitterMeasureData.IsMaximumReached = true;
                    if (valueStretch == attachedLength.Value)
                    {
                        valueStretch = maximumLength;
                        splitterMeasureData.AttachedLength = new SplitterLength(maximumLength);
                    }
                    else
                    {
                        valueStretch -= attachedLength.Value;
                        splitterMeasureData.AttachedLength = new SplitterLength(valueStretch);
                        valueStretch += valueStretch;
                    }

                    remainsForFill = valueFill == 0.0 ? 0.0 : Math.Max(0.0, availableLength - valueStretch);
                    remainsForStretch = remainsForFill == 0.0 ? availableLength : valueStretch;
                }
            }

            if (remainsForFill < minFill)
            {
                remainsForFill = minFill;
                remainsForStretch = availableLength - remainsForFill;
            }

            foreach (SplitterMeasureData splitterMeasureData in measureData)
            {
                SplitterLength attachedLength = splitterMeasureData.AttachedLength;
                double minimumLength = GetMinimumLength(splitterMeasureData.Element);
                if (attachedLength.IsFill)
                {
                    if ((valueFill == 0.0 ? 0.0 : attachedLength.Value / valueFill * remainsForFill) < minimumLength)
                    {
                        splitterMeasureData.IsMinimumReached = true;
                        remainsForFill -= minimumLength;
                        valueFill -= attachedLength.Value;
                    }
                }
                else if ((valueStretch == 0.0 ? 0.0 : attachedLength.Value / valueStretch * remainsForStretch) <
                         minimumLength)
                {
                    splitterMeasureData.IsMinimumReached = true;
                    remainsForStretch -= minimumLength;
                    valueStretch -= attachedLength.Value;
                }
            }
        }

        Size availableSize1 = new Size(width, height);
        Rect rect = new Rect(0.0, 0.0, width, height);
        foreach (SplitterMeasureData splitterMeasureData in measureData)
        {
            SplitterLength attachedLength = splitterMeasureData.AttachedLength;
            double num9 = splitterMeasureData.IsMinimumReached
                ? GetMinimumLength(splitterMeasureData.Element)
                : (!attachedLength.IsFill
                    ? (valueStretch == 0.0 ? 0.0 : attachedLength.Value / valueStretch * remainsForStretch)
                    : (valueFill == 0.0 ? 0.0 : attachedLength.Value / valueFill * remainsForFill));
            if (remeasureElements)
                SetActualSplitterLength(splitterMeasureData.Element, num9);
            if (orientation == Orientation.Horizontal)
            {
                availableSize1 = availableSize1.WithWidth(num9);
                splitterMeasureData.MeasuredBounds = new Rect(rect.Left, rect.Top, num9, rect.Height);
                rect = rect.Translate(new Vector(num9, 0));
                if (remeasureElements)
                    splitterMeasureData.Element.Measure(availableSize1);
            }
            else
            {
                availableSize1 = availableSize1.WithHeight(num9);
                splitterMeasureData.MeasuredBounds = new Rect(rect.Left, rect.Top, rect.Width, num9);
                rect = rect.Translate(new Vector(0, num9));
                if (remeasureElements)
                    splitterMeasureData.Element.Measure(availableSize1);
            }
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Rect finalRect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
        foreach (Control internalChild in Children)
        {
            if (internalChild != null)
            {
                double actualSplitterLength = GetActualSplitterLength(internalChild);
                if (Orientation == Orientation.Horizontal)
                {
                    finalRect = finalRect.WithWidth(actualSplitterLength);
                    internalChild.Arrange(finalRect);
                    finalRect = finalRect.Translate(new Vector(actualSplitterLength, 0));
                }
                else
                {
                    finalRect = finalRect.WithHeight(actualSplitterLength);
                    internalChild.Arrange(finalRect);
                    finalRect = finalRect.Translate(new Vector(0, actualSplitterLength));
                }
            }
        }

        return finalSize;
    }

    private void OnSplitterDragStarted(object? sender, VectorEventArgs args)
    {
        if (!(sender is GridSplitter originalSource))
            return;
        args.Handled = true;
        originalSource.DragDelta +=  OnSplitterResized;
        originalSource.DragCompleted += OnSplitterDragCompleted;
        // if (!this.ShowResizePreview)
        //     return;
        // this.currentPreviewWindow = new SplitterResizePreviewWindow();
        // this.currentPreviewWindow.Show((UIElement) originalSource);
    }

    private void OnSplitterDragCompleted(object? sender, VectorEventArgs args)
    {
        if (!(sender is GridSplitter grip))
            return;
        args.Handled = true;
        // if (this.IsShowingResizePreview)
        // {
        //     this.currentPreviewWindow.Hide();
        //     this.currentPreviewWindow = (SplitterResizePreviewWindow) null;
        //     Point logicalPoint =
        //         DpiAwareness.DeviceToLogicalPoint((Visual) grip, new Point(args.HorizontalChange, args.VerticalChange));
        //     this.CommitResize(grip, logicalPoint.X, logicalPoint.Y);
        // }

        grip.DragDelta -= OnSplitterResized;
        grip.DragCompleted -= OnSplitterDragCompleted;
    }
    //
    private void OnSplitterResized(object? sender, VectorEventArgs args)
    {
        if (!(sender is GridSplitter grip))
            return;
        args.Handled = true;
        // if (this.IsShowingResizePreview)
        //     this.TrackResizePreview(grip, args.HorizontalChange, args.VerticalChange);
        // else
            CommitResize(grip, args.Vector.X, args.Vector.Y);
    }
    //
    private void CommitResize(GridSplitter grip, double horizontalChange, double verticalChange)
    {
        if (!GetResizeIndices(grip, out int _, out var resizeIndex1, out var resizeIndex2))
            return;
        double pixelAmount = Orientation == Orientation.Horizontal ? horizontalChange : verticalChange;
        ResizeChildren(resizeIndex1, resizeIndex2, pixelAmount);
    }
    
    // private void TrackResizePreview(
    //     SplitterGrip grip,
    //     double horizontalChange,
    //     double verticalChange)
    // {
    //     int gripIndex;
    //     int resizeIndex1;
    //     int resizeIndex2;
    //     if (!this.GetResizeIndices(grip, out gripIndex, out resizeIndex1, out resizeIndex2))
    //         return;
    //     double pixelAmount = this.Orientation == Orientation.Horizontal ? horizontalChange : verticalChange;
    //     IList<SplitterMeasureData> measureData = SplitterMeasureData.FromElements((IList) this.Children);
    //     this.ResizeChildrenCore(measureData[resizeIndex1], measureData[resizeIndex2], pixelAmount);
    //     SplitterPanel.Measure(this.RenderSize, this.Orientation, (IEnumerable<SplitterMeasureData>) measureData, false);
    //     Point point = grip.TransformToAncestor((Visual) this).Transform(new Point(0.0, 0.0));
    //     if (this.Orientation == Orientation.Horizontal)
    //         point.X += measureData[gripIndex].MeasuredBounds.Width - this.Children[gripIndex].RenderSize.Width;
    //     else
    //         point.Y += measureData[gripIndex].MeasuredBounds.Height - this.Children[gripIndex].RenderSize.Height;
    //     Point screen = this.PointToScreen(point);
    //     this.currentPreviewWindow.Move((double) (int) screen.X, (double) (int) screen.Y);
    // }

    private bool GetResizeIndices(GridSplitter grip, out int gripIndex, out int resizeIndex1, out int resizeIndex2)
    {
        for (int index = 0; index < Children.Count; ++index)
        {
            if (Children[index].IsVisualAncestorOf(grip))
            {
                gripIndex = index;
                switch (grip.ResizeBehavior)
                {
                    case GridResizeBehavior.CurrentAndNext:
                        resizeIndex1 = index;
                        resizeIndex2 = index + 1;
                        break;
                    case GridResizeBehavior.PreviousAndCurrent:
                        resizeIndex1 = index - 1;
                        resizeIndex2 = index;
                        break;
                    case GridResizeBehavior.PreviousAndNext:
                        resizeIndex1 = index - 1;
                        resizeIndex2 = index + 1;
                        break;
                    default:
                        throw new InvalidOperationException("BasedOnAlignment is not a valid resize behavior");
                }
    
                return resizeIndex1 >= 0 && resizeIndex2 >= 0 && resizeIndex1 < Children.Count &&
                       resizeIndex2 < Children.Count;
            }
        }
    
        gripIndex = -1;
        resizeIndex1 = -1;
        resizeIndex2 = -1;
        return false;
    }

    internal void ResizeChildren(int index1, int index2, double pixelAmount)
    {
        SplitterMeasureData child1 = new SplitterMeasureData(Children[index1]);
        SplitterMeasureData child2 = new SplitterMeasureData(Children[index2]);
        if (!ResizeChildrenCore(child1, child2, pixelAmount))
            return;
        SetSplitterLength(child1.Element, child1.AttachedLength);
        SetSplitterLength(child2.Element, child2.AttachedLength);
        InvalidateMeasure();
    }

    private bool ResizeChildrenCore(
        SplitterMeasureData child1,
        SplitterMeasureData child2,
        double pixelAmount)
    {
        Visual element1 = child1.Element;
        Visual element2 = child2.Element;
        SplitterLength attachedLength1 = child1.AttachedLength;
        SplitterLength attachedLength2 = child2.AttachedLength;
        double actualSplitterLength1 = GetActualSplitterLength(element1);
        double actualSplitterLength2 = GetActualSplitterLength(element2);
        double num1 = Math.Max(0.0,
            Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength1 + pixelAmount));
        double num2 = Math.Max(0.0,
            Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength2 - pixelAmount));
        double minimumLength1 = GetMinimumLength(element1);
        double minimumLength2 = GetMinimumLength(element2);
        if (minimumLength1 + minimumLength2 > num1 + num2)
            return false;
        if (num1 < minimumLength1)
        {
            num2 -= minimumLength1 - num1;
            num1 = minimumLength1;
        }

        if (num2 < minimumLength2)
        {
            num1 -= minimumLength2 - num2;
            num2 = minimumLength2;
        }

        if (attachedLength1.IsFill && attachedLength2.IsFill || attachedLength1.IsStretch && attachedLength2.IsStretch)
        {
            child1.AttachedLength =
                new SplitterLength(num1 / (num1 + num2) * (attachedLength1.Value + attachedLength2.Value),
                    attachedLength1.SplitterUnitType);
            child2.AttachedLength =
                new SplitterLength(num2 / (num1 + num2) * (attachedLength1.Value + attachedLength2.Value),
                    attachedLength1.SplitterUnitType);
        }
        else if (attachedLength1.IsFill)
            child2.AttachedLength = new SplitterLength(num2, SplitterUnitType.Stretch);
        else
            child1.AttachedLength = new SplitterLength(num1, SplitterUnitType.Stretch);

        return true;
    }
}