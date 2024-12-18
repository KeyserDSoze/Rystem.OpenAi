﻿using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi
{
    internal sealed class PriceService : IOpenAiPriceService
    {
        private readonly Dictionary<string, Model> _prices;
        public PriceService(Dictionary<string, Model> prices)
        {
            _prices = prices;
        }
        public decimal CalculatePrice(ModelName modelName, params OpenAiCost[] spentRequests)
        {
            var total = 0m;
            if (_prices.TryGetValue(modelName, out var model))
            {
                foreach (var spent in spentRequests)
                {
                    var cost = model.Costs.FirstOrDefault(x => x.Kind == spent.Kind && x.UnitOfMeasure == spent.UnitOfMeasure);
                    if (cost != null)
                        total += cost.Units * spent.Units;
                }
            }
            return total;
        }
    }
}
