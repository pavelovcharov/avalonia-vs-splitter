using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace AvaloniaApplication3;

public class SplitterLengthConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        switch (Type.GetTypeCode(sourceType))
        {
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.String:
                return true;
            default:
                return false;
        }
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return !(destinationType != typeof (InstanceDescriptor)) || destinationType == typeof (string);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (!CanConvertFrom(value.GetType()))
            throw GetConvertFromException(value);
        if (value is string)
            return FromString((string) value, culture);
        double d = Convert.ToDouble(value, culture);
        if (double.IsNaN(d))
            d = 1.0;
        return new SplitterLength(d, SplitterUnitType.Stretch);
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == null)
            throw new ArgumentNullException(nameof (destinationType));
        if (value != null && value is SplitterLength length)
        {
            if (destinationType == typeof (string))
                return ToString(length, culture);
            if (destinationType.IsEquivalentTo(typeof (InstanceDescriptor)))
                return new InstanceDescriptor(typeof (SplitterLength).GetConstructor(new Type[2]
                {
                    typeof (double),
                    typeof (SplitterUnitType)
                }), new object[]
                {
                    length.Value,
                    length.SplitterUnitType
                });
        }
        throw GetConvertToException(value, destinationType);
    }

    private const string StarStr = "*";
    internal static SplitterLength FromString(string s, CultureInfo? cultureInfo)
    {
        string str = s.Trim();
        double num;
        SplitterUnitType unitType = SplitterUnitType.Stretch;
        if (str.EndsWith(StarStr))
        {
            unitType = SplitterUnitType.Fill;
            var unitTypeLength = StarStr.Length;
            if(str.Length == unitTypeLength)
                num = 1;
            else {
                var valueStr = str.Substring(0, str.Length - unitTypeLength);
                num = Convert.ToDouble(valueStr, cultureInfo);
            }
        }
        else
        {
            num = Convert.ToDouble(str, cultureInfo);
        }

        return new SplitterLength(num, unitType);
    }

    internal static string ToString(SplitterLength length, CultureInfo? cultureInfo)
    {
        return length.SplitterUnitType == SplitterUnitType.Fill ? $"*{(length.Value.AreClose(1) ? string.Empty : length.Value)}" : Convert.ToString(length.Value, cultureInfo);
    }
}