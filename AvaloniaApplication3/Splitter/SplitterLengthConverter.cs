using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace AvaloniaApplication3;

public class SplitterLengthConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
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

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return !(destinationType != typeof (InstanceDescriptor)) || destinationType == typeof (string);
    }

    public override object ConvertFrom(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value)
    {
        if (value == null || !this.CanConvertFrom(value.GetType()))
            throw this.GetConvertFromException(value);
        if (value is string)
            return (object) SplitterLengthConverter.FromString((string) value, culture);
        double d = Convert.ToDouble(value, (IFormatProvider) culture);
        if (double.IsNaN(d))
            d = 1.0;
        return (object) new SplitterLength(d, SplitterUnitType.Stretch);
    }

    public override object ConvertTo(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value,
        Type destinationType)
    {
        if (destinationType == (Type) null)
            throw new ArgumentNullException(nameof (destinationType));
        if (value != null && value is SplitterLength length)
        {
            if (destinationType == typeof (string))
                return ToString(length, culture);
            if (destinationType.IsEquivalentTo(typeof (InstanceDescriptor)))
                return new InstanceDescriptor((MemberInfo) typeof (SplitterLength).GetConstructor(new Type[2]
                {
                    typeof (double),
                    typeof (SplitterUnitType)
                }), new object[]
                {
                    length.Value,
                    length.SplitterUnitType
                });
        }
        throw this.GetConvertToException(value, destinationType);
    }

    internal static SplitterLength FromString(string s, CultureInfo cultureInfo)
    {
        string str = s.Trim();
        double num = 1.0;
        SplitterUnitType unitType = SplitterUnitType.Stretch;
        if (str == "*")
            unitType = SplitterUnitType.Fill;
        else
            num = Convert.ToDouble(str, (IFormatProvider) cultureInfo);
        return new SplitterLength(num, unitType);
    }

    internal static string ToString(SplitterLength length, CultureInfo cultureInfo)
    {
        return length.SplitterUnitType == SplitterUnitType.Fill ? "*" : Convert.ToString(length.Value, (IFormatProvider) cultureInfo);
    }
}