using System;
using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi
{
    public sealed class CostResult<T>
    {
        public T Result { get; }
        internal List<Func<decimal>> Calculations { get; } = new List<Func<decimal>>();
        internal CostResult(T result, Func<decimal> calculate)
        {
            Result = result;
            Calculations.Add(calculate);
        }
        public decimal CalculateCost()
            => Calculations.Sum(x => x.Invoke());
    }
}
