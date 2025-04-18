using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System.Reflection;

namespace CastNight;

public static class StyleManager
{
    public static void Fetch(Control control)
    {
        Type type = control.GetType();
        PropertyInfo[] properties = type.GetProperties();
        List<ICssStyleRule> rules = StyleManager.Select(
                type.Name,
                [.. StyleManager.LoadSheet(type.Name), .. StyleManager.LoadSheet("App")]);
        foreach (ICssStyleRule rule in rules)
        {
            foreach (var property in rule.Style)
            {
                try
                {
                    var target = properties.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                    if (target == null) continue;

                    var prop_type = target.PropertyType;
                    var stylable_interface = prop_type.GetInterfaces()
                        .FirstOrDefault(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IStylable<>) &&
                            i.GenericTypeArguments[0] == prop_type);

                    if (stylable_interface != null)
                    {
                        var parse_method = prop_type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);

                        if (parse_method != null)
                        {
                            var parsed_value = parse_method.Invoke(null, [property.Value]);
                            target.SetValue(control, parsed_value);
                        }
                    }
                    else
                    {
                        object? value = Convert.ChangeType(property.Value, prop_type);
                        target.SetValue(control, value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: \"{ex.Message}\"");
                }
            }
        }
    }

    public static List<ICssStyleRule> Select(string selector, List<ICssStyleRule> rules)
    {
        List<ICssStyleRule> selected = new List<ICssStyleRule>();

        foreach (ICssStyleRule rule in rules)
        {
            if (rule.SelectorText == selector)
            {
                selected.Add(rule);
            }
        }

        return selected;
    }

    public static List<ICssStyleRule> LoadSheet(string name)
    {
        try
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
        }
        catch (Exception)
        {
            return [];
        }
    }
}
