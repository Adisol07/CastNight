using SkiaSharp;
using System.Xml.Serialization;

namespace CastNight.Controls;

public class Button : Control
{
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

    }

    internal override void Render(SKCanvas canvas)
    {
        using (SKPaint paint = new SKPaint
        {
            Color = BackgroundColor.ToSkia(),
            IsAntialias = true
        })
        {
            canvas.DrawRect(100, 100, 200, 150, paint);
        }
    }

    protected override void Update()
    {

    }
}
