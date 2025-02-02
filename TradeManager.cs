using System;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TradeManager : Robot
    {
        // Define button positions and text
        private const string BuyButtonName = "BuyButton";
        private const string SellButtonName = "SellButton";

        // Define the button size and positions
        private const int ButtonWidth = 100;
        private const int ButtonHeight = 30;
        private const int ButtonMargin = 10;

        public void OnStart()
        {
            // Draw Buy Button
            DrawBuyButton();

            // Draw Sell Button
            DrawSellButton();
        }

        // Function to draw the Buy Button
        private void DrawBuyButton()
        {
            // Clear existing buttons (if any)
            ChartObjects.RemoveObject(BuyButtonName);

            // Draw a rectangle as the Buy button
            ChartObjects.DrawRectangle(BuyButtonName, StaticPosition.TopLeft, 0, ButtonWidth, ButtonHeight, Colors.Green);

            // Add the text "BUY" to the button
            ChartObjects.DrawText(BuyButtonName + "_Label", "BUY", StaticPosition.TopLeft, 0, Colors.White);
        }

        // Function to draw the Sell Button
        private void DrawSellButton()
        {
            // Clear existing buttons (if any)
            ChartObjects.RemoveObject(SellButtonName);

            // Draw a rectangle as the Sell button
            ChartObjects.DrawRectangle(SellButtonName, StaticPosition.TopRight, 0, ButtonWidth, ButtonHeight, Colors.Red);

            // Add the text "SELL" to the button
            ChartObjects.DrawText(SellButtonName + "_Label", "SELL", StaticPosition.TopRight, 0, Colors.White);
        }

        public void OnBar()
        {
            // Check if the Buy Button was clicked
            if (IsButtonClicked(BuyButtonName))
            {
                ExecuteBuyTrade();
            }

            // Check if the Sell Button was clicked
            if (IsButtonClicked(SellButtonName))
            {
                ExecuteSellTrade();
            }
        }

        // Function to detect if a button was clicked
        private bool IsButtonClicked(string buttonName)
        {
            // Get the button object
            var button = ChartObjects.GetObject(buttonName);

            if (button != null)
            {
                // Check if the mouse click occurred within the button's boundaries
                var mousePosition = Chart.MousePosition;
                if (mousePosition.X >= button.Left && mousePosition.X <= button.Right &&
                    mousePosition.Y >= button.Top && mousePosition.Y <= button.Bottom)
                {
                    return true;
                }
            }

            return false;
        }

        // Define Layer 1 execution logic
        private void ExecuteBuyTrade()
        {
            double entryPrice = Symbol.Bid;
            double lotSize = 0.01; // User-defined lot size
            double pointsDistance = 150; // User-defined points distance for subsequent layers
            int numberOfLayers = 3; // Number of layers (you can adjust based on user input)

            // Execute Layer 1 buy trade
            var buyOrder = ExecuteMarketOrder(TradeType.Buy, Symbol, lotSize, "Layer1Buy");

            // Place pending orders for Layer 2, Layer 3, etc.
            for (int i = 1; i <= numberOfLayers; i++)
            {
                double pendingPrice = entryPrice + (i * pointsDistance * Symbol.TickSize); // Calculate the price for each layer
                ExecutePendingOrder(TradeType.Buy, Symbol, lotSize, pendingPrice, "Layer" + i);
            }

            // Optional: Apply stop loss and take profit for Layer 1
            double stopLoss = entryPrice - (500 * Symbol.TickSize); // SL for Layer 1
            double takeProfit = entryPrice + (200 * Symbol.TickSize); // TP for Layer 1
            ModifyOrder(buyOrder, stopLoss, takeProfit);
        }

        private void ExecuteSellTrade()
        {
            double entryPrice = Symbol.Ask;
            double lotSize = 0.01; // User-defined lot size
            double pointsDistance = 150; // User-defined points distance for subsequent layers
            int numberOfLayers = 3; // Number of layers (you can adjust based on user input)

            // Execute Layer 1 sell trade
            var sellOrder = ExecuteMarketOrder(TradeType.Sell, Symbol, lotSize, "Layer1Sell");

            // Place pending orders for Layer 2, Layer 3, etc.
            for (int i = 1; i <= numberOfLayers; i++)
            {
                double pendingPrice = entryPrice - (i * pointsDistance * Symbol.TickSize); // Calculate the price for each layer
                ExecutePendingOrder(TradeType.Sell, Symbol, lotSize, pendingPrice, "Layer" + i);
            }

            // Optional: Apply stop loss and take profit for Layer 1
            double stopLoss = entryPrice + (500 * Symbol.TickSize); // SL for Layer 1
            double takeProfit = entryPrice - (200 * Symbol.TickSize); // TP for Layer 1
            ModifyOrder(sellOrder, stopLoss, takeProfit);
        }

        // Function to execute a pending order (buy/sell)
        private void ExecutePendingOrder(TradeType tradeType, Symbol symbol, double lotSize, double price, string label)
        {
            // Create the pending order
            var pendingOrder = PendingOrders.PlaceOrder(tradeType, symbol, lotSize, price, label);
        }

        // Function to modify an order (to add SL/TP)
        private void ModifyOrder(Order order, double stopLoss, double takeProfit)
        {
            // Modify the order with the defined stop loss and take profit
            order.ModifyStopLoss(stopLoss);
            order.ModifyTakeProfit(takeProfit);
        }

    }
}