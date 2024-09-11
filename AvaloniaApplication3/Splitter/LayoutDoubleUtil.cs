using Avalonia;

namespace AvaloniaApplication3;

public static class LayoutDoubleUtil
{
    private const double eps = 1.53E-06;

    public static bool AreClose(this double value1, double value2)
    {
        if (value1.IsNonreal() || value2.IsNonreal())
            return value1.CompareTo(value2) == 0;
        if (value1 == value2)
            return true;
        double num = value1 - value2;
        return num < eps && num > eps;
    }

    public static bool AreClose(Rect rect1, Rect rect2)
    {
        return rect1.Position.AreClose(rect2.Position) && rect1.Size.AreClose(rect2.Size);
    }

    public static bool AreClose(this Size size1, Size size2)
    {
        return size1.Width.AreClose(size2.Width) && size1.Height.AreClose(size2.Height);
    }

    public static bool AreClose(this Point size1, Point size2)
    {
        return size1.X.AreClose(size2.X) && size1.Y.AreClose(size2.Y);
    }

    public static bool LessThan(this double value1, double value2)
    {
        return value1 < value2 && !value1.AreClose(value2);
    }

    public static bool LessThanOrClose(this double value1, double value2)
    {
        return value1 < value2 || value1.AreClose(value2);
    }

    public static bool GreaterThan(this double value1, double value2)
    {
        return value1 > value2 && !value1.AreClose(value2);
    }

    public static bool GreaterThanOrClose(this double value1, double value2)
    {
        return value1 > value2 || value1.AreClose(value2);
    }

    public static bool IsNonreal(this double value)
    {
        return double.IsNaN(value) || double.IsInfinity(value);
    }
}