namespace TinyUrls;

public readonly struct Option<T> where T : class {
    private readonly T? _value = default;
    private Option(T? value) => _value = value;
    private bool IsSome => _value is not null;
    private bool IsNone => _value is null;
    public static implicit operator Option<T>(T? value) => new(value);
    public static Option<T> Some(T value) => new(value);
    public static Option<T> None => new(null);
    public Option<T> Do(Action<T> f) {
        if (IsSome) f(_value!);
        return new(_value);
    }
    public Option<TOut> Map<TOut>(Func<T, TOut> f) where TOut : class =>
        IsSome ? f(_value!) : Option<TOut>.None;
    public T IfNone(T defaultValue) => IsNone ? defaultValue : _value!;
}