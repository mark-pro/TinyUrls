using System.Text.RegularExpressions;

namespace TinyUrls.Prelude.Shortner;

public interface IShortnerConfig {
    ushort MaxLength => 7;
    HashSet<char> Alphabet => new("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
}

public static class IShortnerConfigExtensions {
    public static Regex GetRegex(this IShortnerConfig config) {
        var alphabet = string.Join("", config.Alphabet);
        return new Regex($"^[{alphabet}]{{{config.MaxLength}}}$");
    }
}