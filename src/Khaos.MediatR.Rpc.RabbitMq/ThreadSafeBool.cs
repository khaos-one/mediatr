namespace Khaos.MediatR.Rpc.RabbitMq;

internal sealed class ThreadSafeBool : IEquatable<bool>, IEquatable<ThreadSafeBool>
{
    private int _flag;
        
    public ThreadSafeBool() {}

    public ThreadSafeBool(bool initialValue)
    {
        _flag = initialValue ? 1 : 0;
    }

    public bool IsSet =>
        Interlocked.CompareExchange(ref _flag, 1, 1) == 1;

    public void Set(bool value = true)
    {
        if (value)
        {
            Interlocked.CompareExchange(ref _flag, 1, 0);
        }
        else
        {
            Interlocked.CompareExchange(ref _flag, 0, 1);
        }
    }

    public bool Equals(bool other)
    {
        return IsSet == other;
    }

    public bool Equals(ThreadSafeBool? other)
    {
        if (other != null)
        {
            return IsSet == other.IsSet;
        }

        return false;
    }

    public static implicit operator ThreadSafeBool(bool value)
    {
        return new ThreadSafeBool(value);
    }

    public static implicit operator bool(ThreadSafeBool value)
    {
        return value.IsSet;
    }
}