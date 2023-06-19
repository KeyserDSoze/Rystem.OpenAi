using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class GroceryFunction : IOpenAiChatFunction
    {
        public const string GroceryLabel = "get_list_of_food_to_buy";
        public string Name => GroceryLabel;

        public string Description => "Get the price for the food you have to buy";

        public Type Input => typeof(GroceryRequest);

        public async Task<object> WrapAsync(string message)
        {
            var request = System.Text.Json.JsonSerializer.Deserialize<GroceryRequest>(message);
            if (request == null)
                await Task.Delay(0);
            var response = new GroceryResponse
            {
                Food = new List<GroceryInnerResponse>()
            };
            decimal price = 1;
            if (request.Vegetables != null)
                foreach (var food in request.Vegetables)
                    AddFood(food);
            if (request.Fish != null)
                foreach (var food in request.Fish)
                    AddFood(food);
            if (request.Meat != null)
                foreach (var food in request.Meat)
                    AddFood(food);
            void AddFood(string name)
            {
                response.Food.Add(new GroceryInnerResponse
                {
                    Name = name,
                    Description = "it's in stock",
                    Price = price++,
                    Supermaket = price % 2 == 0 ? "Lidl" : "Coop"
                });
            }
            return response;
        }
    }
}
