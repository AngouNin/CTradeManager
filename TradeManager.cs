using System;
using cAlgo.API;
using cAlgo.API.Internals;
using System.Threading.Tasks;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TradeManagerBot : Robot
    {
        [Parameter("Vertical Position", Group = "Panel alignment", DefaultValue = VerticalAlignment.Top)]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Horizontal Position", Group = "Panel alignment", DefaultValue = HorizontalAlignment.Left)]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        // General Group
        [Parameter("Expert Name", Group = "General", DefaultValue = "Layering Trade Manager")]
        public string ExpertName { get; set; }

        [Parameter("Lot Size", Group = "General", DefaultValue = 0.01)]
        public double LotSize { get; set; }

        [Parameter("Magic Number", Group = "General", DefaultValue = 1234)]
        public int MagicNumber { get; set; }

        [Parameter("Slippage Points", Group = "General", DefaultValue = 5)]
        public int Slippage { get; set; }

        [Parameter("Buy Sell buttons for Execution", Group = "General", DefaultValue = true)]
        public bool UseButtons { get; set; }

        [Parameter("Trendlines for Execution", Group = "General", DefaultValue = true)]
        public bool UseTrendlines { get; set; }

        [Parameter("TP1", Group = "General", DefaultValue = 200)]
        public double TP1 { get; set; }

        [Parameter("TP2", Group = "General", DefaultValue = 400)]
        public double TP2 { get; set; }

        [Parameter("TP3", Group = "General", DefaultValue = 600)]
        public double TP3 { get; set; }

        [Parameter("TP4", Group = "General", DefaultValue = 1000)]
        public double TP4 { get; set; }

        [Parameter("StopLoss", Group = "General", DefaultValue = 500)]
        public double SL { get; set; }

        public enum DeleteAllPendingOrders
        {
            No,
            TP
        }

        [Parameter("Delete All Pending Orders at", Group = "General", DefaultValue = DeleteAllPendingOrders.No)]
        public DeleteAllPendingOrders DeleteAllPendingOrdersAt { get; set; }

        // Trendline Inputs Group
        public enum TrendlineColor
        {
            DarkGreen,
            Red,
            Blue,
            Yellow,
            Black
        }

        [Parameter("Line Colour for Buy", Group = "Trendline Inputs", DefaultValue = TrendlineColor.DarkGreen)]
        public TrendlineColor BuyTrendlineColor { get; set; }

        [Parameter("Line Colour for Sell", Group = "Trendline Inputs", DefaultValue = TrendlineColor.Red)]
        public TrendlineColor SellTrendlineColor { get; set; }

        [Parameter("Execute Seconds before Candle Close", Group = "Trendline Inputs", DefaultValue = 2)]
        public int ExecuteBeforeClose { get; set; }

        [Parameter("Min Trendline distance Points", Group = "Trendline Inputs", DefaultValue = 20)]
        public double MinTrendlineDistance { get; set; }

        [Parameter("Max Body of Execution Candle", Group = "Trendline Inputs", DefaultValue = 600)]
        public double MaxCandleBodySize { get; set; }

        [Parameter("Delete Trendline after execution", Group = "Trendline Inputs", DefaultValue = true)]
        public bool AutoDeleteTrendline { get; set; }

        // First Layer Group
        [Parameter("Activate Layer 1", Group = "First Layer", DefaultValue = true)]
        public bool ActivateLayer1 { get; set; }

        [Parameter("Lot Multiplier", Group = "First Layer", DefaultValue = 1)]
        public double LotMultiplierLayer1 { get; set; }

        [Parameter("Total Number of Trades for Layer 1", Group = "First Layer", DefaultValue = 3)]
        public int TradesLayer1 { get; set; }

        [Parameter("Number of Trades for TP 1", Group = "First Layer", DefaultValue = 1)]
        public int FirstLayerNumberOfTradesForTP1 { get; set; }

        [Parameter("Number of Trades for TP 2", Group = "First Layer", DefaultValue = 1)]
        public int FirstLayerNumberOfTradesForTP2 { get; set; }

        [Parameter("Number of Trades for TP 3", Group = "First Layer", DefaultValue = 1)]
        public int FirstLayerNumberOfTradesForTP3 { get; set; }

        [Parameter("Number of Trades for TP 4", Group = "First Layer", DefaultValue = 0)]
        public int FirstLayerNumberOfTradesForTP4 { get; set; }

        public enum SetBreakeven1
        {
            TP1,
            TP2,
            TP3,
            TP4
        }

        [Parameter("Set Breakeven at", Group = "First Layer", DefaultValue = SetBreakeven1.TP1)]
        public SetBreakeven1 SetBreakevenLayer1 { get; set; }

        [Parameter("Buffer Points for BE", Group = "First Layer", DefaultValue = 10)]
        public double BufferPointsBE1 { get; set; }

        // Second Layer Group
        [Parameter("Activate Layer 2", Group = "Second Layer", DefaultValue = true)]
        public bool ActivateLayer2 { get; set; }

        [Parameter("Lot Multiplier", Group = "Second Layer", DefaultValue = 1)]
        public double LotMultiplierLayer2 { get; set; }

        [Parameter("Points Distance", Group = "Second Layer", DefaultValue = 150)]
        public double Layer2Distance { get; set; }

        [Parameter("Total Number of Trades for Layer 2", Group = "Second Layer", DefaultValue = 5)]
        public int TradesLayer2 { get; set; }

        [Parameter("Number of Trades when reach first Layer", Group = "Second Layer", DefaultValue = 2)]
        public int NumberOfTradesWhenReachFirstLayer2 { get; set; }

        [Parameter("Number of Trades for TP 1", Group = "Second Layer", DefaultValue = 1)]
        public int SecondLayerNumberOfTradesForTP1 { get; set; }

        [Parameter("Number of Trades for TP 2", Group = "Second Layer", DefaultValue = 1)]
        public int SecondLayerNumberOfTradesForTP2 { get; set; }

        [Parameter("Number of Trades for TP 3", Group = "Second Layer", DefaultValue = 1)]
        public int SecondLayerNumberOfTradesForTP3 { get; set; }

        [Parameter("Number of Trades for TP 4", Group = "Second Layer", DefaultValue = 0)]
        public int SecondLayerNumberOfTradesForTP4 { get; set; }

        public enum SetBreakeven2
        {
            FirstLayer,
            TP1,
            TP2,
            TP3,
            TP4
        }

        [Parameter("Set Breakeven at", Group = "Second Layer", DefaultValue = SetBreakeven2.FirstLayer)]
        public SetBreakeven2 SetBreakevenLayer2 { get; set; }

        [Parameter("Buffer Points for BE", Group = "Second Layer", DefaultValue = 10)]
        public double BufferPointsBE2 { get; set; }

        public enum ActionForFirstLayer2
        {
            Close_at_Entry_Price,
            Nothing
        }

        [Parameter("Action for First Layer trades", Group = "Second Layer", DefaultValue = ActionForFirstLayer2.Nothing)]
        public ActionForFirstLayer2 ActionForFirstLayerTrades2 { get; set; }

        // Third Layer Group
        [Parameter("Activate Layer 3", Group = "Third Layer", DefaultValue = true)]
        public bool ActivateLayer3 { get; set; }

        [Parameter("Lot Multiplier", Group = "Third Layer", DefaultValue = 1)]
        public double LotMultiplierLayer3 { get; set; }

        [Parameter("Points Distance", Group = "Third Layer", DefaultValue = 150)]
        public double Layer3Distance { get; set; }

        [Parameter("Total Number of Trades for Layer 3", Group = "Third Layer", DefaultValue = 8)]
        public int TradesLayer3 { get; set; }

        [Parameter("Number of Trades when reach second Layer", Group = "Third Layer", DefaultValue = 4)]
        public int NumberOfTradesWhenReachSecondLayer3 { get; set; }

        [Parameter("Number of Trades when reach first Layer", Group = "Third Layer", DefaultValue = 1)]
        public int NumberOfTradesWhenReachFirstLayer3 { get; set; }

        [Parameter("Number of Trades for TP 1", Group = "Third Layer", DefaultValue = 1)]
        public int ThirdLayerNumberOfTradesForTP1 { get; set; }

        [Parameter("Number of Trades for TP 2", Group = "Third Layer", DefaultValue = 1)]
        public int ThirdLayerNumberOfTradesForTP2 { get; set; }

        [Parameter("Number of Trades for TP 3", Group = "Third Layer", DefaultValue = 1)]
        public int ThirdLayerNumberOfTradesForTP3 { get; set; }

        [Parameter("Number of Trades for TP 4", Group = "Third Layer", DefaultValue = 0)]
        public int ThirdLayerNumberOfTradesForTP4 { get; set; }

        public enum SetBreakeven3
        {
            FirstLayer,
            SecondLayer,
            TP1,
            TP2,
            TP3,
            TP4
        }

        [Parameter("Set Breakeven at", Group = "Third Layer", DefaultValue = SetBreakeven3.FirstLayer)]
        public SetBreakeven3 SetBreakevenLayer3 { get; set; }

        [Parameter("Buffer Points for BE", Group = "Third Layer", DefaultValue = 10)]
        public double BufferPointsBE3 { get; set; }

        public enum ActionForFirstLayer3
        {
            Close_at_Entry_Price,
            Close_at_Second_Layer_Entry_Price,
            TP1,
            TP2,
            TP3,
            TP4
        }

        [Parameter("Action for First Layer trades(ALL)", Group = "Third Layer", DefaultValue = ActionForFirstLayer3.Close_at_Entry_Price)]
        public ActionForFirstLayer3 ActionForFirstLayerTrades3 { get; set; }

        public enum ActionForSecondLayer3
        {
            Close_at_Entry_Price,
            Close_at_First_Layer_Entry_Price,
            TP1,
            TP2,
            TP3,
            TP4
        }

        [Parameter("Action for Second Layer trades(ALL)", Group = "Third Layer", DefaultValue = ActionForSecondLayer3.Close_at_Entry_Price)]
        public ActionForSecondLayer3 ActionForSecondLayerTrades3 { get; set; }

        protected override void OnStart()
        {
            if (UseButtons)
            {
                DrawTradePanel();
            }

            if (UseTrendlines)
            {
                DrawTrendlines();
            }
        }

        private void DrawTradePanel()
        {
            var panel = new StackPanel
            {
                Margin = 10,
                Orientation = Orientation.Vertical,
            };

            // var buyButton = CreateTradeButton("BUY", Color.Green, TradeType.Buy);
            var buyButton = CreateTradeButton("BUY", Color.Green, TradeType.Buy);
            buyButton.Style = CreateBuyButtonStyle();

            var sellButton = CreateTradeButton("SELL", Color.Red, TradeType.Sell);
            sellButton.Style = CreateSellButtonStyle();

            var closeAllButton = CreateTradeButton("CLOSE ALL", Color.Orange, null, true);
            closeAllButton.Style = CreateCloseButtonStyle();

            panel.AddChild(buyButton);
            panel.AddChild(sellButton);
            panel.AddChild(closeAllButton);

            Chart.AddControl(new Border
            {
                VerticalAlignment = PanelVerticalAlignment,
                HorizontalAlignment = PanelHorizontalAlignment,
                Child = panel,
                Style = CreatePanelStyle(),
                Margin = "20 40 20 20",
                Width = 225,
                Height = 120,
            });
        }

        private Button CreateTradeButton(string text, Color color, TradeType? tradeType, bool closeAll = false)
        {
            var button = new Button
            {
                Text = text,
                Style = CreateButtonStyle(color, color),
                Height = 25
            };

            button.Click += async args =>
            {
                if (closeAll)
                {
                    CloseAllTrades();
                }
                else if (tradeType.HasValue)
                {
                   // Fetch the updated LotSize value dynamically
                    double updatedLotSize = LotSize;
                    Print("LotSize: {0}", updatedLotSize);
                    // Normalize volume according to the symbol's rules
                    double minimumVolume = Symbol.NormalizeVolumeInUnits(1, RoundingMode.Up);
                    double volume = minimumVolume * LotSize * 100;
                    Print("Executing {0} trade with volume {1}", tradeType.Value, volume);
                    // Execute the trade with the updated LotSize
                    await ExecuteTradeAsync(tradeType.Value, volume);              
                }
            };

            return button;
        }

        private void CloseAllTrades()
        {
            foreach (var position in Positions)
            {
                ClosePosition(position);
            }
        }

        private async Task ExecuteTradeAsync(TradeType tradeType, double volume)
        {
            var position = ExecuteMarketOrder(tradeType, SymbolName, volume, "Layer1", SL, TP1);
            await Task.Delay(ExecuteBeforeClose * 1000);
            PlacePendingOrders(tradeType);
        }

        private void PlacePendingOrders(TradeType tradeType)
        {
            if (ActivateLayer2)
            {
                double layer2Price = tradeType == TradeType.Buy 
                ? Symbol.Ask + Layer2Distance * Symbol.PipSize 
                : Symbol.Bid - Layer2Distance * Symbol.PipSize;
                PlaceLimitOrder(tradeType, SymbolName, LotSize, layer2Price, "Layer2");
            }

            if (ActivateLayer3)
            {
                double layer3Price = tradeType == TradeType.Buy 
                ? Symbol.Ask + Layer3Distance * Symbol.PipSize 
                : Symbol.Bid - Layer3Distance * Symbol.PipSize;
                PlaceLimitOrder(tradeType, SymbolName, LotSize, layer3Price, "Layer3");
            }
        }

        private void DrawTrendlines()
        {
            double currentPrice = Symbol.Bid;
            double buyTrendlinePrice = currentPrice - (MinTrendlineDistance * Symbol.PipSize);
            double sellTrendlinePrice = currentPrice + (MinTrendlineDistance * Symbol.PipSize);

            var buyColor = Color.FromName(BuyTrendlineColor.ToString());
            var sellColor = Color.FromName(SellTrendlineColor.ToString());

            Chart.DrawTrendLine("BuyTrendline",
                Server.Time.AddMinutes(-10), buyTrendlinePrice,
                Server.Time.AddMinutes(10), buyTrendlinePrice, buyColor);

            Chart.DrawTrendLine("SellTrendline",
                Server.Time.AddMinutes(-10), sellTrendlinePrice,
                Server.Time.AddMinutes(10), sellTrendlinePrice, sellColor);

            Print("Trendlines Drawn: Buy at {0} ({1}), Sell at {2} ({3})", 
                buyTrendlinePrice, BuyTrendlineColor, sellTrendlinePrice, SellTrendlineColor);
        }

        protected override void OnBar()
        {
            if (!UseTrendlines) return;

            var lastClose = Bars.ClosePrices.Last(1);
            var lastOpen = Bars.OpenPrices.Last(1);
            double bodySize = Math.Abs(lastClose - lastOpen) / Symbol.PipSize;

            if (bodySize > MaxCandleBodySize)
            {
                Print("Candle body size {0} exceeds max allowed {1}", bodySize, MaxCandleBodySize);
                return;
            }

            var buyColor = Color.FromName(BuyTrendlineColor.ToString());
            var sellColor = Color.FromName(SellTrendlineColor.ToString());

            foreach (var obj in Chart.Objects)
            {
                if (obj is ChartTrendLine trendline)
                {
                    bool isBuyTrendline = trendline.Color == buyColor;
                    bool isSellTrendline = trendline.Color == sellColor;
                    double trendlinePrice = trendline.Y1;

                    if (isBuyTrendline && lastClose > trendlinePrice)
                    {
                        ExecuteTrade(TradeType.Buy);
                        if (AutoDeleteTrendline) Chart.RemoveObject(trendline.Name);
                        Print("Buy trendline-trade executed at {0}", trendlinePrice);
                    }
                    else if (isSellTrendline && lastClose < trendlinePrice)
                    {
                        ExecuteTrade(TradeType.Sell);
                        if (AutoDeleteTrendline) Chart.RemoveObject(trendline.Name);
                        Print("Sell trendline-trade executed at {0}", trendlinePrice);
                    }
                }
            }
        }

        private void ExecuteTrade(TradeType tradeType)
        {
            double updatedLotSize = LotSize;
            Print("LotSize: {0}", updatedLotSize);
            // Normalize volume according to the symbol's rules
            double minimumVolume = Symbol.NormalizeVolumeInUnits(1, RoundingMode.Up);
            double volume = minimumVolume * LotSize * 100; 
            ExecuteMarketOrder(tradeType, SymbolName, volume, MagicNumber.ToString(), SL, TP1);
            Print("Executed {0} trade with volume {1}", tradeType, volume);
            PlacePendingOrders(tradeType);

            // Check for break-even
            double currentPrice = tradeType == TradeType.Buy ? Symbol.Ask : Symbol.Bid;
            Position position = null;
            foreach (var pos in Positions)
            {
                if (pos.Label == "Layer1")
                {
                    position = pos;
                    break;
                }
            }
            if (position != null)
            {
                double entryPrice = position.EntryPrice;

                if (tradeType == TradeType.Buy && currentPrice - entryPrice > BufferPointsBE1 * Symbol.PipSize)
                {
                    AdjustForBreakEven(position, tradeType);
                }
                else if (tradeType == TradeType.Sell && entryPrice - currentPrice > BufferPointsBE2 * Symbol.PipSize)
                {
                    AdjustForBreakEven(position, tradeType);
                }
            }
            
        }

        private void AdjustForBreakEven(Position position, TradeType tradeType)
        {
            if (position != null)
            {
                double newSL = position.EntryPrice;
                ModifyPosition(position, newSL, position.TakeProfit, ProtectionType.None);
            }
        }

        private static Style CreatePanelStyle()
        {
            var style = new Style();
            style.Set(ControlProperty.BackgroundColor, Color.FromHex("#292929"), ControlState.DarkTheme);
            style.Set(ControlProperty.BackgroundColor, Color.FromHex("#FFFFFF"), ControlState.LightTheme);
            style.Set(ControlProperty.BorderColor, Color.FromHex("#3C3C3C"));
            style.Set(ControlProperty.BorderThickness, new Thickness(1));
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
            return CreateButtonStyle(Color.FromHex("#355e07"), Color.FromHex("#73d10a"));
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
            style.Set(ControlProperty.Margin, new Thickness(0, 5, 0, 5), ControlState.DarkTheme);
            style.Set(ControlProperty.Margin, new Thickness(0, 5, 0, 5), ControlState.LightTheme);
            return style;
        }
    }
}
