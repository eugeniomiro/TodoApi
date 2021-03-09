using System;

namespace TodoApi.Domain.SumTypes
{
    // see: https://www.dotnetcurry.com/patterns-practices/1510/maybe-monad-csharp
    public abstract class Updated<T>
    {
        private Updated()
        { }

        public sealed class Accepted : Updated<T>
        {
            public Accepted(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        public sealed class Invalid : Updated<T> { }

        public sealed class NotFound : Updated<T> { }

        public bool TryGetValue(out T value)
        {
            if (this is Accepted a)
            {
                value = a.Value;
                return true;
            }
            value = default;
            return false;
        }

        public static implicit operator Updated<T>(Updated.UpdatedInvalid updatedInvalid)
        {
            return new Invalid();
        }

        public static implicit operator Updated<T>(Updated.UpdatedNotFound updatedNotFound)
        {
            return new NotFound();
        }
    }
    public static class Updated
    {
        public class UpdatedInvalid { }

        public class UpdatedNotFound { }

        public static UpdatedInvalid Invalid { get; } = new UpdatedInvalid();

        public static UpdatedNotFound NotFound { get; } = new UpdatedNotFound();

        public static Updated<T> Accepted<T>(T value)
        {
            return new Updated<T>.Accepted(value);
        }
    }
}
