using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;

namespace CastNight;

public static class StyleManager
{
    public static List<ICssStyleRule> LoadSheet(string name)
    {
        string cnss = App.ReadResource(name + ".cnss");
        CssParserOptions config = new CssParserOptions
        {
            IsIncludingUnknownDeclarations = true,
            IsToleratingInvalidSelectors = true,
            IsIncludingUnknownRules = true
        };
        CssParser parser = new CssParser(config);
        ICssStyleSheet stylesheet = parser.ParseStyleSheet(cnss);
        return stylesheet.Rules.OfType<ICssStyleRule>().ToList();

        // foreach (ICssStyleRule rule in stylesheet.Rules.OfType<ICssStyleRule>())
        // {
        //     if (rule.SelectorText == "Window")
        //     {
        //         foreach (var property in rule.Style)
        //         {
        //             if (property.Name.ToLower() == "backgroundcolor")
        //             {
        //                 BackgroundColor = Color.Parse(property.Value);
        //             }
        //         }
        //     }
        // }
    }
}
