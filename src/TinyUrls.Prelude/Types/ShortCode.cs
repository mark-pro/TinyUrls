using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TinyUrls.Types;

[JsonConverter(typeof(ShortCodeTypeConverter))]
public readonly struct ShortCodeType : IParsable<ShortCodeType>, IEquatable<ShortCodeType> {

    private readonly ReadOnlyMemory<char> _value = new ();
    internal ShortCodeType(ReadOnlyMemory<char> value) => _value = value;
    public static implicit operator string(ShortCodeType value) => value.ToString();
    public override string ToString() => _value.ToString();

    public static ShortCodeType Parse(string s, IFormatProvider? _) =>
        ShortCode.FromString(s);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ShortCodeType result) =>
        s is not null ?
            (result = ShortCode.FromString(s)) is {} :
            (result = default) is {};
    
    public static bool operator ==(ShortCodeType left, ShortCodeType right) =>
        left._value.Span.SequenceEqual(right._value.Span);

    public static bool operator !=(ShortCodeType left, ShortCodeType right) => !(left == right);
    
    public bool Equals(ShortCodeType other) =>
        _value.Equals(other._value);

    public override bool Equals(object? obj) =>
        obj is ShortCodeType other && Equals(other);

    public override int GetHashCode() =>
        _value.GetHashCode();
}

public static class ShortCode {
    public static ShortCodeType FromString(string value) =>
        new(value.AsMemory());
}

internal sealed class ShortCodeTypeConverter : JsonConverter<ShortCodeType> {
    public override ShortCodeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ShortCode.FromString(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ShortCodeType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
} 