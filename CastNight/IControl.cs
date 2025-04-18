using SkiaSharp;
using System.Xml.Serialization;

namespace CastNight;

public abstract class Control
{
    [XmlAttribute("Class")]
    public string? Class { get; set; }
    public List<Control> Children { get; set; } = new List<Control>();

    internal abstract void Render(SKCanvas canvas);
    protected virtual void Initialize()
    {
        StyleManager.Fetch(this);
    }
    protected virtual void Update()
    {
        foreach (Control control in Children)
            control.Update();
    }
}
