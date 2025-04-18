using System;

namespace CastNight;

public interface IStylable<T>
{
    public abstract static T Parse(string value);
}
