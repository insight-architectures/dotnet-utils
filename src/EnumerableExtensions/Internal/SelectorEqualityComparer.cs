using System;
using System.Collections.Generic;

namespace InsightArchitectures.Utilities.Internal
{
    /// <summary>
    /// An equality comparer that compares objects by the result of the given selector.
    /// </summary>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <typeparam name="TValue">The type of the result of the selector to be applied to the objects to compare.</typeparam>
    public class SelectorEqualityComparer<T, TValue> : IEqualityComparer<T>
    {
        private readonly Func<T, TValue> _selector;
        private readonly IEqualityComparer<TValue> _equalityComparer;

        /// <summary>
        /// Initializes a new instance of the class <see cref="SelectorEqualityComparer{T,TValue}"/>. The default equality comparer for <typeparamref name="TValue"/> will be used.
        /// </summary>
        /// <param name="selector">A delegate used to select which values to be used to compare objects of type <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null" />.</exception>
        public SelectorEqualityComparer(Func<T, TValue> selector)
            : this(selector, EqualityComparer<TValue>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class <see cref="SelectorEqualityComparer{T,TValue}"/>. The given implementation of <see cref="IEqualityComparer{TValue}" /> will be used to compare values of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="selector">A delegate used to select which values to be used to compare objects of type <typeparamref name="T"/>.</param>
        /// <param name="equalityComparer">The equality comparer to be used when comparing values of type <typeparamref name="TValue"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="equalityComparer"/> is <see langword="null" />.</exception>
        public SelectorEqualityComparer(Func<T, TValue> selector, IEqualityComparer<TValue> equalityComparer)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
        }

        /// <summary>
        /// Determines whether the values returned when the selector is applied to both objects are equal.
        /// </summary>
        /// <inheritdoc />
        public bool Equals(T? x, T? y)
        {
            var first = x != null ? _selector(x) : default;
            var second = y != null ? _selector(y) : default;

            return _equalityComparer.Equals(first!, second!);
        }

        /// <summary>
        /// Returns a hashcode for the value returned when the selector is applied to the specified object.
        /// </summary>
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">The value obtained when applying the selector to <paramref name="obj"/> is <see langword="null" />.</exception>
        public int GetHashCode(T obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));

            var property = _selector(obj);

            _ = property ?? throw new ArgumentNullException(nameof(obj));

            return _equalityComparer.GetHashCode(property);
        }
    }
}
