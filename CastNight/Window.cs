using System.Xml;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;

namespace CastNight;

public abstract class Window
{
    private NativeWindowSettings native_settings;
    private GameWindow window;
    private GRContext grcontext;

    public string Title { get; set; } = "CastNight Application";
    public Size Size { get; set; } = new Size(720, 480);
    public Color BackgroundColor { get; set; } = Color.White;

    public Window()
    {
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
            this.Size = new Size(args.Size.X, args.Size.Y);
        };

        window.RenderFrame += (args) =>
        {
            window.Title = Title;
            OnRender();

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

                    using (SKPaint paint = new SKPaint
                    {
                        Color = SKColors.Red,
                        IsAntialias = true
                    })
                    {
                        canvas.DrawRect(100, 100, 200, 150, paint);
                    }

                    surface.Canvas.Flush();
                }
            }

            window.SwapBuffers();

            OnUpdate();
        };
    }
    protected void InitializeComponents()
    {
        string cnxml = App.ReadResource(this.GetType().Name + ".cnxml");
        XmlDocument cnxml_doc = new XmlDocument();
        cnxml_doc.LoadXml(cnxml);
        XmlNode window_node = cnxml_doc.GetElementsByTagName("Window")[0]!;
        foreach (XmlAttribute attr in window_node.Attributes!)
        {
            if (attr.Name == "Title")
            {
                this.Title = attr.Value;
            }
            else if (attr.Name == "Size")
            {
                this.Size = Size.Parse(attr.Value);
                window.Size = new Vector2i(Size.Width, Size.Height);
            }
        }

        string cnss = App.ReadResource(this.GetType().Name + ".cnss");
        CssParserOptions config = new CssParserOptions
        {
            IsIncludingUnknownDeclarations = true,
            IsToleratingInvalidSelectors = true,
            IsIncludingUnknownRules = true
        };
        CssParser parser = new CssParser(config);
        ICssStyleSheet stylesheet = parser.ParseStyleSheet(cnss);
        foreach (ICssStyleRule rule in stylesheet.Rules.OfType<ICssStyleRule>())
        {
            if (rule.SelectorText == "Window")
            {
                foreach (var property in rule.Style)
                {
                    if (property.Name.ToLower() == "backgroundcolor")
                    {
                        BackgroundColor = Color.Parse(property.Value);
                    }
                }
            }
        }
    }

    public virtual void OnUpdate()
    { }
    public virtual void OnRender()
    { }

    public void Show()
    {
        window.Run();
    }

    public void Close()
    {
        window.Close();
    }
}
