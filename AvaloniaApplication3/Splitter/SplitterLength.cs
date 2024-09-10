using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaApplication3;

[TypeConverter(typeof (SplitterLengthConverter))]
public struct SplitterLength : IEquatable<SplitterLength>
{
    private double unitValue;
    private SplitterUnitType unitType;

    public SplitterLength(double value)
        : this(value, SplitterUnitType.Stretch)
    {
    }

    public SplitterLength(double value, SplitterUnitType unitType)
    {
        this.unitValue = value;
        this.unitType = unitType;
    }

    public SplitterUnitType SplitterUnitType => this.unitType;

    public double Value => this.unitValue;

    public bool IsFill => this.SplitterUnitType == SplitterUnitType.Fill;

    public bool IsStretch => this.SplitterUnitType == SplitterUnitType.Stretch;

    public static bool operator ==(SplitterLength obj1, SplitterLength obj2)
    {
        return obj1.SplitterUnitType == obj2.SplitterUnitType && obj1.Value == obj2.Value;
    }

    public static bool operator !=(SplitterLength obj1, SplitterLength obj2) => !(obj1 == obj2);

    public override bool Equals(object obj)
    {
        return obj is SplitterLength splitterLength && this == splitterLength;
    }

    public override int GetHashCode() => (int) ((int) this.unitValue + this.unitType);

    public bool Equals(SplitterLength other) => this == other;

    public override string ToString()
    {
        return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
    }
}