using System.Xml.Serialization;

namespace CastNight;

public static class StructureManager
{
    public static T Load<T>(string name) where T : Control
    {
        string cnxml = App.ReadResource(name + ".cnxml");
        XmlSerializer serializer = CreateSerializer(typeof(T));
        using StringReader reader = new StringReader(cnxml);
        T control = (T)serializer.Deserialize(reader)!;
        return control;
    }

    public static XmlSerializer CreateSerializer(Type rootType)
    {
        Type base_type = typeof(Control);

        Type[] derived_types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t != base_type && base_type.IsAssignableFrom(t))
            .ToArray();

        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
        XmlAttributes children_attrs = new XmlAttributes();

        foreach (Type dt in derived_types)
        {
            children_attrs.XmlElements.Add(new XmlElementAttribute(dt.Name, dt));
        }

        overrides.Add(base_type, nameof(Control.Children), children_attrs);

        XmlRootAttribute root_attr = new XmlRootAttribute(rootType.Name);

        return new XmlSerializer(rootType, overrides, derived_types, root_attr, "");
    }
}
