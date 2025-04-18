using System;

namespace CastNight;

public class Size
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Size(int w, int h)
    {
        Width = w;
        Height = h;
    }

    public static Size Parse(string value)
    {
        string[] parts = value.Split(" ");
        return new Size(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
    }
}
