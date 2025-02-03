using System;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System.Collections.Generic;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TradeManager : Robot
    {
        [Parameter("Vertical Position", Group = "Panel alignment", DefaultValue = VerticalAlignment.Top)]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Horizontal Position", Group = "Panel alignment", DefaultValue = HorizontalAlignment.Left)]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        [Parameter("Expert Name", Group = "Default trade parameters", DefaultValue = "Layering Trade Manager")]
        public double ExpertName { get; set; }

        [Parameter("Lots Size", Group = "Default trade parameters", DefaultValue = 0.01)]
        public double DefaultLots { get; set; }

        [Parameter("Magic Number", Group = "Default trade parameters", DefaultValue = 1234)]
        public double MagicNumber { get; set; }

        [Parameter("Slippage Point", Group = "Default trade parameters", DefaultValue = 5)]
        public double SlippagePoint { get; set; }

        [Parameter("Buy Sell buttons for Execution", Group = "Default trade parameters", DefaultValue = true)]
        public bool BuySellButtonsForExecution { get; set; }

        [Parameter("Trendlines for Execution", Group = "Default trade parameters", DefaultValue = false)]
        public bool TrendlinesForExecution { get; set; }

        [Parameter("Take Profit 1", Group = "Default trade parameters", DefaultValue = 200)]
        public double DefaultTakeProfitPips1 { get; set; }

        [Parameter("Take Profit 2", Group = "Default trade parameters", DefaultValue = 400)]
        public double DefaultTakeProfitPips2 { get; set; }

        [Parameter("Take Profit 3", Group = "Default trade parameters", DefaultValue = 600)]
        public double DefaultTakeProfitPips3 { get; set; }

        [Parameter("Take Profit 4", Group = "Default trade parameters", DefaultValue = 1000)]
        public double DefaultTakeProfitPips4 { get; set; }

        [Parameter("Stop Loss", Group = "Default trade parameters", DefaultValue = 500)]
        public double DefaultStopLossPips { get; set; }


        protected override void OnStart()
        {
            // Draw Buy Button
            DrawPanel();

            // Draw Sell Button
            // DrawSellButton();
        }

        // Function to draw the Buy Button
        private void DrawPanel()
        {
            var tradingPanel = new TradingPanel(this, Symbol, DefaultLots, DefaultStopLossPips, DefaultTakeProfitPips1);

            var border = new Border 
            {
                VerticalAlignment = PanelVerticalAlignment,
                HorizontalAlignment = PanelHorizontalAlignment,
                Style = Styles.CreatePanelBackgroundStyle(),
                Margin = "20 40 20 20",
                Width = 225,
                Child = tradingPanel
            };

            Chart.AddControl(border);
        }

        // // Function to draw the Sell Button
        // private void DrawSellButton()
        // {
        //     // Clear existing buttons (if any)
        //     Chart.RemoveObject(SellButtonName);

        //     // Draw a rectangle as the Sell button
        //     Chart.DrawRectangle(SellButtonName, ChartIconType.Rectangle, ChartIconPosition.TopRight, ButtonWidth, ButtonHeight, Colors.Red);

        //     // Add the text "SELL" to the button
        //     ChartObjects.DrawText(SellButtonName + "_Label", "SELL", ChartIconPosition.TopRight, Colors.White);
        // }

        // public void OnBar()
        // {
        //     // Check if the Buy Button was clicked
        //     if (IsButtonClicked(BuyButtonName))
        //     {
        //         ExecuteBuyTrade();
        //     }

        //     // Check if the Sell Button was clicked
        //     if (IsButtonClicked(SellButtonName))
        //     {
        //         ExecuteSellTrade();
        //     }
        // }

        // // Function to detect if a button was clicked
        // private bool IsButtonClicked(string buttonName)
        // {
        //     // Get the button object
        //     var button = ChartObjects.GetObject(buttonName);

        //     if (button != null)
        //     {
        //         // Check if the mouse click occurred within the button's boundaries
        //         var mousePosition = Chart.MousePosition;
        //         if (mousePosition.X >= button.Left && mousePosition.X <= button.Right &&
        //             mousePosition.Y >= button.Top && mousePosition.Y <= button.Bottom)
        //         {
        //             return true;
        //         }
        //     }

        //     return false;
        // }

        // // Define Layer 1 execution logic
        // private void ExecuteBuyTrade()
        // {
        //     double entryPrice = Symbol.Bid;
        //     double lotSize = 0.01; // User-defined lot size
        //     double pointsDistance = 150; // User-defined points distance for subsequent layers
        //     int numberOfLayers = 3; // Number of layers (you can adjust based on user input)

        //     // Execute Layer 1 buy trade
        //     var buyOrder = ExecuteMarketOrder(TradeType.Buy, Symbol, lotSize, "Layer1Buy");

        //     // Place pending orders for Layer 2, Layer 3, etc.
        //     for (int i = 1; i <= numberOfLayers; i++)
        //     {
        //         double pendingPrice = entryPrice + (i * pointsDistance * Symbol.TickSize); // Calculate the price for each layer
        //         ExecutePendingOrder(TradeType.Buy, Symbol, lotSize, pendingPrice, "Layer" + i);
        //     }

        //     // Optional: Apply stop loss and take profit for Layer 1
        //     double stopLoss = entryPrice - (500 * Symbol.TickSize); // SL for Layer 1
        //     double takeProfit = entryPrice + (200 * Symbol.TickSize); // TP for Layer 1
        //     ModifyOrder(buyOrder, stopLoss, takeProfit);
        // }

        // private void ExecuteSellTrade()
        // {
        //     double entryPrice = Symbol.Ask;
        //     double lotSize = 0.01; // User-defined lot size
        //     double pointsDistance = 150; // User-defined points distance for subsequent layers
        //     int numberOfLayers = 3; // Number of layers (you can adjust based on user input)

        //     // Execute Layer 1 sell trade
        //     var sellOrder = ExecuteMarketOrder(TradeType.Sell, Symbol, lotSize, "Layer1Sell");

        //     // Place pending orders for Layer 2, Layer 3, etc.
        //     for (int i = 1; i <= numberOfLayers; i++)
        //     {
        //         double pendingPrice = entryPrice - (i * pointsDistance * Symbol.TickSize); // Calculate the price for each layer
        //         ExecutePendingOrder(TradeType.Sell, Symbol, lotSize, pendingPrice, "Layer" + i);
        //     }

        //     // Optional: Apply stop loss and take profit for Layer 1
        //     double stopLoss = entryPrice + (500 * Symbol.TickSize); // SL for Layer 1
        //     double takeProfit = entryPrice - (200 * Symbol.TickSize); // TP for Layer 1
        //     ModifyOrder(sellOrder, stopLoss, takeProfit);
        // }

        // // Function to execute a pending order (buy/sell)
        // private void ExecutePendingOrder(TradeType tradeType, Symbol symbol, double lotSize, double price, string label)
        // {
        //     // Create the pending order
        //     var pendingOrder = PendingOrders.PlaceOrder(tradeType, symbol, lotSize, price, label);
        // }

        // // Function to modify an order (to add SL/TP)
        // private void ModifyOrder(Order order, double stopLoss, double takeProfit)
        // {
        //     // Modify the order with the defined stop loss and take profit
        //     order.ModifyStopLoss(stopLoss);
        //     order.ModifyTakeProfit(takeProfit);
        // }

        public class TradingPanel : CustomControl
        {
            private const string LotsInputKey = "LotsKey";
            private const string TakeProfitInputKey = "TPKey";
            private const string StopLossInputKey = "SLKey";
            private readonly IDictionary<string, TextBox> _inputMap = new Dictionary<string, TextBox>();
            private readonly Robot _robot;
            private readonly Symbol _symbol;

            public TradingPanel(Robot robot, Symbol symbol, double defaultLots, double defaultStopLossPips, double defaultTakeProfitPips)
            {
                _robot = robot;
                _symbol = symbol;
                AddChild(CreateTradingPanel(defaultLots, defaultStopLossPips, defaultTakeProfitPips));
            }

            private ControlBase CreateTradingPanel(double defaultLots, double defaultStopLossPips, double defaultTakeProfitPips)
            {
                var mainPanel = new StackPanel();

                var header = CreateHeader();
                mainPanel.AddChild(header);

                var contentPanel = CreateContentPanel(defaultLots, defaultStopLossPips, defaultTakeProfitPips);
                mainPanel.AddChild(contentPanel);

                return mainPanel;
            }

            private ControlBase CreateHeader()
            {
                var headerBorder = new Border 
                {
                    BorderThickness = "0 0 0 1",
                    Style = Styles.CreateCommonBorderStyle()
                };

                var header = new TextBlock 
                {
                    Text = "Quick Trading Panel",
                    Margin = "10 7",
                    Style = Styles.CreateHeaderStyle()
                };

                headerBorder.Child = header;
                return headerBorder;
            }

            private StackPanel CreateContentPanel(double defaultLots, double defaultStopLossPips, double defaultTakeProfitPips)
            {
                var contentPanel = new StackPanel 
                {
                    Margin = 10
                };
                var grid = new Grid(4, 3);
                grid.Columns[1].SetWidthInPixels(5);

                var sellButton = CreateTradeButton("SELL", Styles.CreateSellButtonStyle(), TradeType.Sell);
                grid.AddChild(sellButton, 0, 0);

                var buyButton = CreateTradeButton("BUY", Styles.CreateBuyButtonStyle(), TradeType.Buy);
                grid.AddChild(buyButton, 0, 2);

                var lotsInput = CreateInputWithLabel("Quantity (Lots)", defaultLots.ToString("F2"), LotsInputKey);
                grid.AddChild(lotsInput, 1, 0, 1, 3);

                var stopLossInput = CreateInputWithLabel("Stop Loss (Pips)", defaultStopLossPips.ToString("F1"), StopLossInputKey);
                grid.AddChild(stopLossInput, 2, 0);

                var takeProfitInput = CreateInputWithLabel("Take Profit (Pips)", defaultTakeProfitPips.ToString("F1"), TakeProfitInputKey);
                grid.AddChild(takeProfitInput, 2, 2);

                var closeAllButton = CreateCloseAllButton();
                grid.AddChild(closeAllButton, 3, 0, 1, 3);

                contentPanel.AddChild(grid);

                return contentPanel;
            }

            private Button CreateTradeButton(string text, Style style, TradeType tradeType)
            {
                var tradeButton = new Button 
                {
                    Text = text,
                    Style = style,
                    Height = 25
                };

                tradeButton.Click += args => ExecuteMarketOrderAsync(tradeType);

                return tradeButton;
            }

            private ControlBase CreateCloseAllButton()
            {
                var closeAllBorder = new Border 
                {
                    Margin = "0 10 0 0",
                    BorderThickness = "0 1 0 0",
                    Style = Styles.CreateCommonBorderStyle()
                };

                var closeButton = new Button 
                {
                    Style = Styles.CreateCloseButtonStyle(),
                    Text = "Close All",
                    Margin = "0 10 0 0"
                };

                closeButton.Click += args => CloseAll();
                closeAllBorder.Child = closeButton;

                return closeAllBorder;
            }

            private Panel CreateInputWithLabel(string label, string defaultValue, string inputKey)
            {
                var stackPanel = new StackPanel 
                {
                    Orientation = Orientation.Vertical,
                    Margin = "0 10 0 0"
                };

                var textBlock = new TextBlock 
                {
                    Text = label
                };

                var input = new TextBox 
                {
                    Margin = "0 5 0 0",
                    Text = defaultValue,
                    Style = Styles.CreateInputStyle()
                };

                _inputMap.Add(inputKey, input);

                stackPanel.AddChild(textBlock);
                stackPanel.AddChild(input);

                return stackPanel;
            }

            private void ExecuteMarketOrderAsync(TradeType tradeType)
            {
                var lots = GetValueFromInput(LotsInputKey, 0);
                if (lots <= 0)
                {
                    _robot.Print(string.Format("{0} failed, invalid Lots", tradeType));
                    return;
                }

                var stopLossPips = GetValueFromInput(StopLossInputKey, 0);
                var takeProfitPips = GetValueFromInput(TakeProfitInputKey, 0);

                _robot.Print(string.Format("Open position with: LotsParameter: {0}, StopLossPipsParameter: {1}, TakeProfitPipsParameter: {2}", lots, stopLossPips, takeProfitPips));

                var volume = _symbol.QuantityToVolumeInUnits(lots);
                _robot.ExecuteMarketOrderAsync(tradeType, _symbol.Name, volume, "Trade Panel Sample", stopLossPips, takeProfitPips);
            }

            private double GetValueFromInput(string inputKey, double defaultValue)
            {
                double value;

                return double.TryParse(_inputMap[inputKey].Text, out value) ? value : defaultValue;
            }

            private void CloseAll()
            {
                foreach (var position in _robot.Positions)
                    _robot.ClosePositionAsync(position);
            }
        }

        public static class Styles
        {
            public static Style CreatePanelBackgroundStyle()
            {
                var style = new Style();
                style.Set(ControlProperty.CornerRadius, 3);
                style.Set(ControlProperty.BackgroundColor, GetColorWithOpacity(Color.FromHex("#292929"), 0.85m), ControlState.DarkTheme);
                style.Set(ControlProperty.BackgroundColor, GetColorWithOpacity(Color.FromHex("#FFFFFF"), 0.85m), ControlState.LightTheme);
                style.Set(ControlProperty.BorderColor, Color.FromHex("#3C3C3C"), ControlState.DarkTheme);
                style.Set(ControlProperty.BorderColor, Color.FromHex("#C3C3C3"), ControlState.LightTheme);
                style.Set(ControlProperty.BorderThickness, new Thickness(1));

                return style;
            }

            public static Style CreateCommonBorderStyle()
            {
                var style = new Style();
                style.Set(ControlProperty.BorderColor, GetColorWithOpacity(Color.FromHex("#FFFFFF"), 0.12m), ControlState.DarkTheme);
                style.Set(ControlProperty.BorderColor, GetColorWithOpacity(Color.FromHex("#000000"), 0.12m), ControlState.LightTheme);
                return style;
            }

            public static Style CreateHeaderStyle()
            {
                var style = new Style();
                style.Set(ControlProperty.ForegroundColor, GetColorWithOpacity("#FFFFFF", 0.70m), ControlState.DarkTheme);
                style.Set(ControlProperty.ForegroundColor, GetColorWithOpacity("#000000", 0.65m), ControlState.LightTheme);
                return style;
            }

            public static Style CreateInputStyle()
            {
                var style = new Style(DefaultStyles.TextBoxStyle);
                style.Set(ControlProperty.BackgroundColor, Color.FromHex("#1A1A1A"), ControlState.DarkTheme);
                style.Set(ControlProperty.BackgroundColor, Color.FromHex("#111111"), ControlState.DarkTheme | ControlState.Hover);
                style.Set(ControlProperty.BackgroundColor, Color.FromHex("#E7EBED"), ControlState.LightTheme);
                style.Set(ControlProperty.BackgroundColor, Color.FromHex("#D6DADC"), ControlState.LightTheme | ControlState.Hover);
                style.Set(ControlProperty.CornerRadius, 3);
                return style;
            }

            public static Style CreateBuyButtonStyle()
            {
                return CreateButtonStyle(Color.FromHex("#009345"), Color.FromHex("#10A651"));
            }

            public static Style CreateSellButtonStyle()
            {
                return CreateButtonStyle(Color.FromHex("#F05824"), Color.FromHex("#FF6C36"));
            }

            public static Style CreateCloseButtonStyle()
            {
                return CreateButtonStyle(Color.FromHex("#F05824"), Color.FromHex("#FF6C36"));
            }

            private static Style CreateButtonStyle(Color color, Color hoverColor)
            {
                var style = new Style(DefaultStyles.ButtonStyle);
                style.Set(ControlProperty.BackgroundColor, color, ControlState.DarkTheme);
                style.Set(ControlProperty.BackgroundColor, color, ControlState.LightTheme);
                style.Set(ControlProperty.BackgroundColor, hoverColor, ControlState.DarkTheme | ControlState.Hover);
                style.Set(ControlProperty.BackgroundColor, hoverColor, ControlState.LightTheme | ControlState.Hover);
                style.Set(ControlProperty.ForegroundColor, Color.FromHex("#FFFFFF"), ControlState.DarkTheme);
                style.Set(ControlProperty.ForegroundColor, Color.FromHex("#FFFFFF"), ControlState.LightTheme);
                return style;
            }

            private static Color GetColorWithOpacity(Color baseColor, decimal opacity)
            {
                var alpha = (int)Math.Round(byte.MaxValue * opacity, MidpointRounding.AwayFromZero);
                return Color.FromArgb(alpha, baseColor);
            }
        }

    }
}