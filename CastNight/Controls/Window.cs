using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;
using System.Xml.Serialization;

namespace CastNight.Controls;

[XmlRoot("Window")]
public abstract class Window : Control
{
    private NativeWindowSettings? native_settings;
    private GameWindow? window;
    private GRContext? grcontext;
    private Size size = new Size(720, 480);

    [XmlAttribute("Title")]
    public string Title { get; set; } = "CastNight Application";

    [XmlIgnore]
    public Size Size
    {
        get => size;
        set
        {
            size = value;
            if (window != null)
                window.Size = new Vector2i(size.Width, size.Height);
        }
    }
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
    public Color BackgroundColor { get; set; } = Color.Black;
    [XmlAttribute("BackgroundColor")]
    public string BackgroundColorSerialized
    {
        get => BackgroundColor.ToString()!;
        set
        {
            BackgroundColor = Color.Parse(value);
        }
    }

    protected override void Update()
    {
        OnUpdate();
        base.Update();
    }
    internal override void Render(SKCanvas canvas)
    { }
    protected override void Initialize()
    {
        base.Initialize();

        native_settings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(Size.Width, Size.Height),
            Title = Title,
            IsEventDriven = true
        };

        window = new GameWindow(GameWindowSettings.Default, native_settings);
        grcontext = GRContext.CreateGl();

        window.Resize += (args) =>
        {
            if (window.MouseState.IsButtonDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Button1))
                this.Size = new Size(args.Size.X, args.Size.Y);
        };

        window.RenderFrame += (args) =>
        {
            window.Title = Title;

            Update();

            GL.Viewport(0, 0, window.Size.X, window.Size.Y);
            GL.ClearColor(BackgroundColor.R / 255f, BackgroundColor.G / 255f, BackgroundColor.B / 255f, BackgroundColor.A / 255f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GRGlFramebufferInfo fb_info = new GRGlFramebufferInfo(0, SKColorType.Rgba8888.ToGlSizedFormat());

            using (GRBackendRenderTarget render_target = new GRBackendRenderTarget(window.Size.X, window.Size.Y, 0, 8, fb_info))
            {
                using (SKSurface surface = SKSurface.Create(grcontext, render_target, SKColorType.Rgba8888))
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);

                    foreach (Control control in Children)
                    {
                        control.Render(canvas);
                    }

                    surface.Canvas.Flush();
                }
            }

            window.SwapBuffers();
        };
    }

    public virtual void OnUpdate()
    { }

    public void Show()
    {
        Initialize();
        window?.Run();
    }

    public void Close()
    {
        window?.Close();
    }
}
