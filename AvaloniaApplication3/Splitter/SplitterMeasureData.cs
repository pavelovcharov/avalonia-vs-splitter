using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication3;

public class SplitterMeasureData
{
    public SplitterMeasureData(Control element)
    {
        Element = element;
        AttachedLength = SplitterPanel.GetSplitterLength(element);
    }

    public static IList<SplitterMeasureData> FromElements(IList elements)
    {
        List<SplitterMeasureData> splitterMeasureDataList = new List<SplitterMeasureData>(elements.Count);
        foreach (Control element in elements)
        {
            if (element != null)
                splitterMeasureDataList.Add(new SplitterMeasureData(element));
        }
        return splitterMeasureDataList;
    }

    public Control Element { get; private set; }

    public SplitterLength AttachedLength { get; set; }

    public bool IsMinimumReached { get; set; }

    public bool IsMaximumReached { get; set; }

    public Rect MeasuredBounds { get; set; }
}