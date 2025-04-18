using SkiaSharp;

namespace CastNight;

public class Color : IStylable<Color>
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public static Color White => new Color(255, 255, 255, 255);
    public static Color Black => new Color(0, 0, 0, 255);
    public static Color Red => new Color(255, 0, 0, 255);
    public static Color Green => new Color(0, 255, 0, 255);
    public static Color Blue => new Color(0, 0, 255, 255);

    public Color()
    { }
    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = 255;
    }
    public Color(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static Color Parse(string color)
    {
        if (color.StartsWith("rgba"))
        {
            string[] parts = color.Remove(0, 5).Replace(")", "").Split(", ");
            return new Color(Convert.ToByte(parts[0]), Convert.ToByte(parts[1]), Convert.ToByte(parts[2]), Convert.ToByte(parts[3]));
        }
        if (color.StartsWith("rgb"))
        {
            string[] parts = color.Remove(0, 4).Replace(")", "").Split(", ");
            return new Color(Convert.ToByte(parts[0]), Convert.ToByte(parts[1]), Convert.ToByte(parts[2]), 255);
        }
        switch (color.ToLower())
        {
            case "white":
                return White;
            case "black":
                return Black;
            case "red":
                return Red;
            case "green":
                return Green;
            case "blue":
                return Blue;

            default:
                return Black;
        }
    }

    public SKColor ToSkia()
    {
        return new SKColor(R, G, B, A);
    }

    public override string ToString()
    {
        return $"rgba({R}, {G}, {B}, {A})";
    }
}
