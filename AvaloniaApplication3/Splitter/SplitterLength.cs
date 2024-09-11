using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaApplication3;

[TypeConverter(typeof (SplitterLengthConverter))]
public struct SplitterLength : IEquatable<SplitterLength>
{
    public SplitterLength(double value)
        : this(value, SplitterUnitType.Stretch)
    {
    }

    public SplitterLength(double value, SplitterUnitType unitType)
    {
        Value = value;
        this.SplitterUnitType = unitType;
    }

    public SplitterUnitType SplitterUnitType { get; }

    public double Value { get; }

    public bool IsFill => SplitterUnitType == SplitterUnitType.Fill;

    public bool IsStretch => SplitterUnitType == SplitterUnitType.Stretch;

    public static bool operator ==(SplitterLength obj1, SplitterLength obj2)
    {
        return obj1.SplitterUnitType == obj2.SplitterUnitType && obj1.Value == obj2.Value;
    }

    public static bool operator !=(SplitterLength obj1, SplitterLength obj2) => !(obj1 == obj2);

    public override bool Equals(object? obj)
    {
        return obj is SplitterLength splitterLength && this == splitterLength;
    }

    public override int GetHashCode() => (int) ((int) Value + SplitterUnitType);

    public bool Equals(SplitterLength other) => this == other;

    public override string ToString()
    {
        return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
    }
}