using SkiaSharp;

namespace CastNight;

public abstract class Control
{
    public List<Control> Children { get; set; } = new List<Control>();

    protected abstract void Initialize();
    internal abstract void Render(SKCanvas canvas);
    protected virtual void Update()
    {
        foreach (Control control in Children)
            control.Update();
    }
}
