// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chess;

/// <summary>
/// Minimal enum-like base class to replace SmartEnum dependency
/// Provides Name, Value properties and FromValue factory method
/// </summary>
public abstract class SimpleEnum<T> where T : SimpleEnum<T>
{
    private static readonly Dictionary<int, T> ValueMap = new();
    private static readonly Dictionary<string, T> NameMap = new();

    /// <summary>
    /// The name of this enum value
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The numeric value of this enum value
    /// </summary>
    public int Value { get; }

    protected SimpleEnum(string name, int value)
    {
        Name = name;
        Value = value;

        // Register this instance for FromValue lookups
        var type = GetType();
        if (!ValueMap.ContainsKey(value) && type == typeof(T))
        {
            ValueMap[value] = (T)(object)this;
            NameMap[name] = (T)(object)this;
        }
    }

    /// <summary>
    /// Get enum value by its numeric value
    /// </summary>
    public static T? FromValue(int value)
    {
        return ValueMap.TryGetValue(value, out var result) ? result : null;
    }

    /// <summary>
    /// Get enum value by its name
    /// </summary>
    public static T? FromName(string name)
    {
        return NameMap.TryGetValue(name, out var result) ? result : null;
    }

    /// <summary>
    /// Get all enum values
    /// </summary>
    public static IEnumerable<T> GetValues()
    {
        return ValueMap.Values;
    }

    public override string ToString() => Name;

    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is SimpleEnum<T> other)
            return Value == other.Value;
        return false;
    }

    public static bool operator ==(SimpleEnum<T>? a, SimpleEnum<T>? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Value == b.Value;
    }

    public static bool operator !=(SimpleEnum<T>? a, SimpleEnum<T>? b)
    {
        return !(a == b);
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
