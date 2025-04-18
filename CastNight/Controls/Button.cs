using SkiaSharp;
using System.Xml.Serialization;

namespace CastNight.Controls;

public class Button : Control
{
    [XmlIgnore]
    public Size Size { get; set; } = new Size(100, 100);
    [XmlAttribute("Size")]
    public string SizeSerialized
    {
        get => Size.ToString()!;
        set
        {
            Size = Size.Parse(value);
        }
    }

    [XmlIgnore]
    public Color BackgroundColor { get; set; } = Color.White;
    [XmlAttribute("BackgroundColor")]
    public string BackgroundColorSerialized
    {
        get => BackgroundColor.ToString()!;
        set
        {
            BackgroundColor = Color.Parse(value);
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    internal override void Render(SKCanvas canvas)
    {
        using (SKPaint paint = new SKPaint
        {
            Color = BackgroundColor.ToSkia(),
            IsAntialias = true
        })
        {
            canvas.DrawRect(100, 100, Size.Width, Size.Height, paint);
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
