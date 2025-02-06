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
        public double StopLoss { get; set; }

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

        [Parameter("Total Number of Trades for Layer 1", Group = "First Layer", DefaultValue = 5)]
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

        [Parameter("Total Number of Trades for Layer 2", Group = "Second Layer", DefaultValue = 6)]
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

        [Parameter("Total Number of Trades for Layer 3", Group = "Third Layer", DefaultValue = 11)]
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
            // Create trade buttons if enabled
            if (UseButtons)
                DrawTradePanel();

            // Optionally monitor for trendlines if enabled
            if (UseTrendlines)
                MonitorTrendlines();
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

            button.Click += args =>
            {
                if (closeAll)
                {
                    CloseAllTrades();
                }
                else if (tradeType.HasValue == true)
                {
                    if (tradeType.Value == TradeType.Buy)
                    {
                        ExecuteBuyTrades();
                    }
                    else if (tradeType.Value == TradeType.Sell)
                    {
                        ExecuteSellTrades();
                    }
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

        private void ExecuteBuyTrades()
        {
            // if (!IsTradeAllowed(TradeType.Buy)) return;
            // Fetch the updated LotSize value dynamically
            double updatedLotSize = LotSize;
            Print("LotSize:=> {0}", updatedLotSize);
            // Normalize volume according to the symbol's rules
            double minimumVolume = Symbol.NormalizeVolumeInUnits(1, RoundingMode.Up);
            double volume = minimumVolume * LotSize * 100;
            Print("Executing Buy trade with volume {1}", volume);

            // Layer 1 
            for (int i = 1; i <= TradesLayer1; i++)
            {
                var tradeResult = ExecuteMarketOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer1, MagicNumber.ToString());
                if (tradeResult.IsSuccessful)
                {
                    var position = tradeResult.Position;
                    // Handle SL and TPs
                    SetStopLossAndTakeProfits(position, (int)TP1, (int)TP2, (int)TP3, (int)TP4, (int)StopLoss);
                }
            }

            // Layer 2
            for (int i = 1; i <= TradesLayer2; i++)
            {
                double layer2Price = Symbol.Ask + Layer2Distance * Symbol.PipSize;
                PlaceLimitOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer2, layer2Price, "Layer2");
            }

            // Layer 3
            for (int i = 1; i <= TradesLayer3; i++)
            {
                double layer3Price = Symbol.Ask + Layer3Distance * Symbol.PipSize; // Adjust for Layer 3
                PlaceLimitOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer3, layer3Price, "Layer3");
            }
        }

        private void ExecuteSellTrades()
        {
            // if (!IsTradeAllowed(TradeType.Sell)) return;

            // Fetch the updated LotSize value dynamically
            double updatedLotSize = LotSize;
            Print("LotSize: {0}", updatedLotSize);
            Print("Symbol: {0}", Symbol);
            // Normalize volume according to the symbol's rules
            double minimumVolume = Symbol.NormalizeVolumeInUnits(1, RoundingMode.Up);
            double volume = minimumVolume * LotSize * 100;
            Print("Executing Sell trade with volume {1}", volume);

            // Layer 1 
            for (int i = 1; i <= TradesLayer1; i++)
            {
                var tradeResult = ExecuteMarketOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer1, MagicNumber.ToString());
                if (tradeResult.IsSuccessful)
                {
                    var position = tradeResult.Position;
                    // Handle SL and TPs
                    SetStopLossAndTakeProfits(position, (int)TP1, (int)TP2, (int)TP3, (int)TP4, (int)StopLoss);
                }
            }

            // Layer 2
            for (int i = 1; i <= TradesLayer2; i++)
            {
                double layer2Price = Symbol.Bid - Layer2Distance * Symbol.PipSize;
                PlaceLimitOrder(TradeType.Sell, SymbolName, volume*LotMultiplierLayer2, layer2Price, "Layer2");
            }

            // Layer 3
            for (int i = 1; i <= TradesLayer3; i++)
            {
                double layer3Price = Symbol.Bid - Layer3Distance * Symbol.PipSize; // Adjust for Layer 3
                PlaceLimitOrder(TradeType.Sell, SymbolName, volume*LotMultiplierLayer3, layer3Price, "Layer3");
            }
        }

        private void SetStopLossAndTakeProfits(Position position, int tp1, int tp2, int tp3, int tp4, int stopLoss)
        {
            double slPrice = position.EntryPrice - stopLoss * Symbol.PipSize;
            ModifyPosition(position, slPrice, position.EntryPrice + tp1 * Symbol.PipSize, ProtectionType.None);
            
            // Set additional TPs logic
        }

        private bool IsTradeAllowed(TradeType tradeType)
        {
            double lastClose = Bars.ClosePrices.Last(1);
            var lastCandle = Bars.LastBar;

            double bodySize = Math.Abs(lastCandle.Close - lastCandle.Open);

            // Validate body size criteria
            if (bodySize > MaxCandleBodySize || bodySize < MinTrendlineDistance)
                return false;

            // Additional checks can go here based on your requirements

            return true;
        }

        private void MonitorTrendlines()
        {
            // Implement logic to listen for trendline crossings and trigger trades
        }

        protected override void OnTick()
        {
            // Periodically check conditions for managing existing positions and pending orders
            ManageOpenPositions();
        }

        private void ManageOpenPositions()
        {
            // Implement the logic to check existing positions, trigger closings, and manage SL/TP adjustments based on layers
        }

        // Implement additional methods and logic as per your requirements

        protected override void OnStop()
        {
            // Cleanup actions if needed, such as closing orders or removing buttons
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
