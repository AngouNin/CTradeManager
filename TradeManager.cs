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
                        Print("Buy trade button click!");
                        ExecuteBuyTrades();
                    }
                    else if (tradeType.Value == TradeType.Sell)
                    {
                        Print("Sell trade button click!");
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
            Print("Symbol: {0}", Symbol);
            // Normalize volume according to the symbol's rules
            double minimumVolume = Symbol.NormalizeVolumeInUnits(1, RoundingMode.Up);
            double volume = minimumVolume * LotSize * 100;
            Print("Executing Buy trade with volume {0}", volume);
            // Layer 1 
            Print("Total  Number of Trades for Layer 1 ==> {0}", TradesLayer1);
            for (int i = 1; i <= TradesLayer1; i++)
            {
                var tradeResult = ExecuteMarketOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer1, MagicNumber.ToString());
                Print("Trade Result: {0}", tradeResult);
                if (tradeResult.IsSuccessful)
                {
                    var position = tradeResult.Position;
                    // Handle SL and TPs
                    SetStopLossAndTakeProfits(position, (int)TP1, (int)TP2, (int)TP3, (int)TP4, (int)StopLoss);
                }
            }

            // Layer 2
            Print("Total  Number of Trades for Layer 2 ==> {0}", TradesLayer2);
            for (int i = 1; i <= TradesLayer2; i++)
            {
                double layer2Price = Symbol.Ask + Layer2Distance * Symbol.PipSize;
                PlaceLimitOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer2, layer2Price, "Layer2");
                Print("Layer 2 Price: {0}", layer2Price);
            }

            // Layer 3
            Print("Total  Number of Trades for Layer 3 ==> {0}", TradesLayer3);
            for (int i = 1; i <= TradesLayer3; i++)
            {
                double layer3Price = Symbol.Ask + Layer3Distance * Symbol.PipSize; // Adjust for Layer 3
                PlaceLimitOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer3, layer3Price, "Layer3");
                Print("Layer 3 Price: {0}", layer3Price);
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

            // Layer 1 
            Print("Total  Number of Trades for Layer 1 ==> {0}", TradesLayer1);
            for (int i = 1; i <= TradesLayer1; i++)
            {
                var tradeResult = ExecuteMarketOrder(TradeType.Buy, SymbolName, volume*LotMultiplierLayer1, MagicNumber.ToString());
                if (tradeResult.IsSuccessful)
                {
                    var position = tradeResult.Position;
                    Print("Trade Result: {0}", tradeResult);
                    // Handle SL and TPs
                    SetStopLossAndTakeProfits(position, (int)TP1, (int)TP2, (int)TP3, (int)TP4, (int)StopLoss);
                }
            }

            // Layer 2
            Print("Total  Number of Trades for Layer 2 ==> {0}", TradesLayer2);
            for (int i = 1; i <= TradesLayer2; i++)
            {
                double layer2Price = Symbol.Bid - Layer2Distance * Symbol.PipSize;
                PlaceLimitOrder(TradeType.Sell, SymbolName, volume*LotMultiplierLayer2, layer2Price, "Layer2");
                Print("Layer 2 Price: {0}", layer2Price);
            }

            // Layer 3
            Print("Total  Number of Trades for Layer 3 ==> {0}", TradesLayer3);
            for (int i = 1; i <= TradesLayer3; i++)
            {
                double layer3Price = Symbol.Bid - Layer3Distance * Symbol.PipSize; // Adjust for Layer 3
                PlaceLimitOrder(TradeType.Sell, SymbolName, volume*LotMultiplierLayer3, layer3Price, "Layer3");
                Print("Layer 3 Price: {0}", layer3Price);
            }
        }

        private void SetStopLossAndTakeProfits(Position position, int tp1, int tp2, int tp3, int tp4, int stopLoss)
        {
            if (position == null)
                return;

            double slPrice, tp1Price, tp2Price, tp3Price, tp4Price;

            if (position.TradeType == TradeType.Buy)
            {
                slPrice = position.EntryPrice - stopLoss * Symbol.PipSize;
                tp1Price = position.EntryPrice + tp1 * Symbol.PipSize;
                tp2Price = position.EntryPrice + tp2 * Symbol.PipSize;
                tp3Price = position.EntryPrice + tp3 * Symbol.PipSize;
                tp4Price = position.EntryPrice + tp4 * Symbol.PipSize;
            }
            else
            {
                slPrice = position.EntryPrice + stopLoss * Symbol.PipSize;
                tp1Price = position.EntryPrice - tp1 * Symbol.PipSize;
                tp2Price = position.EntryPrice - tp2 * Symbol.PipSize;
                tp3Price = position.EntryPrice - tp3 * Symbol.PipSize;
                tp4Price = position.EntryPrice - tp4 * Symbol.PipSize;
            }

            // Modify main position with SL and the first TP
            ModifyPosition(position, slPrice, tp1Price, ProtectionType.None);

            // Create partial close orders for other TPs
            int remainingVolume = (int)position.VolumeInUnits;
            int volumeForTP1 = (int)(remainingVolume * FirstLayerNumberOfTradesForTP1 / TradesLayer1);
            int volumeForTP2 = (int)(remainingVolume * FirstLayerNumberOfTradesForTP2 / TradesLayer1);
            int volumeForTP3 = (int)(remainingVolume * FirstLayerNumberOfTradesForTP3 / TradesLayer1);
            int volumeForTP4 = (int)(remainingVolume * FirstLayerNumberOfTradesForTP4 / TradesLayer1);

            if (volumeForTP2 > 0)
                PlaceTakeProfitOrder(position.TradeType, volumeForTP2, tp2Price, "TP2");

            if (volumeForTP3 > 0)
                PlaceTakeProfitOrder(position.TradeType, volumeForTP3, tp3Price, "TP3");

            if (volumeForTP4 > 0)
                PlaceTakeProfitOrder(position.TradeType, volumeForTP4, tp4Price, "TP4");
        }

        private void PlaceTakeProfitOrder(TradeType tradeType, int volume, double price, string label)
        {
            if (tradeType == TradeType.Buy)
                PlaceLimitOrder(TradeType.Sell, SymbolName, volume, price, label);
            else
                PlaceLimitOrder(TradeType.Buy, SymbolName, volume, price, label);
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
            // Create trendlines for buying and selling
            double currentPrice = Symbol.Bid;
            double buyTrendlinePrice = currentPrice - (MinTrendlineDistance * Symbol.PipSize);
            double sellTrendlinePrice = currentPrice + (MinTrendlineDistance * Symbol.PipSize);

            var buyColor = Color.FromName(BuyTrendlineColor.ToString());
            var sellColor = Color.FromName(SellTrendlineColor.ToString());

            var buyTrendline = Chart.DrawTrendLine("BuyTrendline",
                Server.Time.AddMinutes(-10), buyTrendlinePrice,
                Server.Time.AddMinutes(10), buyTrendlinePrice, buyColor);

            var sellTrendline = Chart.DrawTrendLine("SellTrendline",
                Server.Time.AddMinutes(-10), sellTrendlinePrice,
                Server.Time.AddMinutes(10), sellTrendlinePrice, sellColor);

            Print("Trendlines Drawn: Buy at {0} ({1}), Sell at {2} ({3})", 
                buyTrendlinePrice, BuyTrendlineColor, sellTrendlinePrice, SellTrendlineColor);

            var lastClose = Bars.ClosePrices.Last(1);
            var lastCandle = Bars.LastBar;

            // Check for closing of the candle above the buy trendline
            if (lastCandle.Close > buyTrendline.Y1 && lastCandle.Close == lastClose)
            {
                ExecuteBuyTrades();
            }

            // Check for closing of the candle below the sell trendline
            if (lastCandle.Close < sellTrendline.Y1 && lastCandle.Close == lastClose)
            {
                ExecuteSellTrades();
            }
        }

        protected override void OnTick()
        {
            // Periodically check conditions for managing existing positions and pending orders
            ManageOpenPositions();
        }

        private void ManageOpenPositions()
        {
            // Get all positions for this symbol with the specific magic number
            var positions = Positions.FindAll(SymbolName, MagicNumber.ToString());
            
            double bufferPointsBE1 = BufferPointsBE1 * Symbol.PipSize; 
            double bufferPointsBE2 = BufferPointsBE2 * Symbol.PipSize; 
            double bufferPointsBE3 = BufferPointsBE3 * Symbol.PipSize; 

            double firstLayerPrice = 0;
            double secondLayerPrice = 0;

            foreach (var position in positions)
            {
                // Check if the position is for buying
                if (position.TradeType == TradeType.Buy)
                {
                    // Save first layer price on first successful trade
                    if (firstLayerPrice == 0)
                    {
                        firstLayerPrice = position.EntryPrice;
                    }

                    if (secondLayerPrice == 0)
                    {
                        secondLayerPrice = position.EntryPrice;
                    }

                    // Check for take profit levels
                    if (position.Pips >= TP1)
                    {
                        ClosePosition(position); // Close if TP1 is hit
                        if (DeleteAllPendingOrdersAt == DeleteAllPendingOrders.TP)
                        {
                            DeletePendingOrders();
                        }
                        continue; // Proceed to the next position
                    }

                    if (position.Pips >= TP2)
                    {
                        ClosePosition(position); // Close if TP2 is hit
                        continue; // Proceed to the next position
                    }

                    if (position.Pips >= TP3)
                    {
                        ClosePosition(position); // Close if TP3 is hit
                        continue; // Proceed to the next position
                    }

                    if (position.Pips >= TP4)
                    {
                        ClosePosition(position); // Close if TP4 is hit
                        continue; // Proceed to the next position
                    }

                    // Manage Breakeven for Layer 1
                    if (SetBreakevenLayer1 == SetBreakeven1.TP1 && position.Pips >= bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer1 == SetBreakeven1.TP2 && position.Pips >= bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer1 == SetBreakeven1.TP3 && position.Pips >= bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer1 == SetBreakeven1.TP4 && position.Pips >= bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }

                    // Manage Breakeven for Layer 2
                    if (SetBreakevenLayer2 == SetBreakeven2.FirstLayer && position.Pips >= bufferPointsBE2)
                    {
                        ModifyPosition(position, firstLayerPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP1 && position.Pips >= bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP2 && position.Pips >= bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP3 && position.Pips >= bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP4 && position.Pips >= bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }

                    // Manage Breakeven for Layer 3
                    if (SetBreakevenLayer3 == SetBreakeven3.FirstLayer && position.Pips >= bufferPointsBE3)
                    {
                        ModifyPosition(position, firstLayerPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.SecondLayer && position.Pips >= bufferPointsBE3)
                    {
                        ModifyPosition(position, secondLayerPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP1 && position.Pips >= bufferPointsBE3)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP2 && position.Pips >= bufferPointsBE3)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP3 && position.Pips >= bufferPointsBE3)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP4 && position.Pips >= bufferPointsBE3)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                }
                else if (position.TradeType == TradeType.Sell)
                {
                    // Save first layer price on first successful trade
                    if (firstLayerPrice == 0)
                    {
                        firstLayerPrice = position.EntryPrice;
                    }

                    if (secondLayerPrice == 0)
                    {
                        secondLayerPrice = position.EntryPrice;
                    }

                    // Check for sell positions take profit levels
                    if (position.Pips <= -TP1)
                    {
                        ClosePosition(position); // Close if TP1 is hit
                        if (DeleteAllPendingOrdersAt == DeleteAllPendingOrders.TP)
                        {
                            DeletePendingOrders();
                        }
                        continue; // Proceed to the next position
                    }

                    if (position.Pips <= -TP2)
                    {
                        ClosePosition(position); // Close if TP2 is hit
                        continue; // Proceed to the next position
                    }

                    if (position.Pips <= -TP3)
                    {
                        ClosePosition(position); // Close if TP3 is hit
                        continue; // Proceed to the next position
                    }

                    if (position.Pips <= -TP4)
                    {
                        ClosePosition(position); // Close if TP4 is hit
                        continue; // Proceed to the next position
                    }

                    // Manage Breakeven for Layer 1
                    if (SetBreakevenLayer1 == SetBreakeven1.TP1 && position.Pips >= -bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer1 == SetBreakeven1.TP2 && position.Pips >= -bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer1 == SetBreakeven1.TP3 && position.Pips >= -bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer1 == SetBreakeven1.TP4 && position.Pips >= -bufferPointsBE1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }

                    // Manage Breakeven for Layer 2
                    if (SetBreakevenLayer2 == SetBreakeven2.FirstLayer && position.Pips >= -bufferPointsBE2)
                    {
                        ModifyPosition(position, firstLayerPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP1 && position.Pips >= -bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP2 && position.Pips >= -bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP3 && position.Pips >= -bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer2 == SetBreakeven2.TP4 && position.Pips >= -bufferPointsBE2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }

                    // Manage Breakeven for Layer 3
                    if (SetBreakevenLayer3 == SetBreakeven3.FirstLayer && position.Pips <= -bufferPointsBE3)
                    {
                        ModifyPosition(position, firstLayerPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.SecondLayer && position.Pips <= -bufferPointsBE3)
                    {
                        ModifyPosition(position, secondLayerPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP1 && position.Pips <= -TP1)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP2 && position.Pips <= -TP2)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP3 && position.Pips <= -TP3)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                    else if (SetBreakevenLayer3 == SetBreakeven3.TP4 && position.Pips <= -TP4)
                    {
                        ModifyPosition(position, position.EntryPrice, position.TakeProfit, ProtectionType.None);
                    }
                }
            }
        }

        private void DeletePendingOrders()
        {
            // Get all pending orders for this symbol with the specific magic number
            foreach (var order in PendingOrders)
            {
                if (order.SymbolName == SymbolName && order.Label == MagicNumber.ToString())
                {
                    CancelPendingOrder(order);
                }
            }
        }

        protected override void OnStop()
        {
            // Clean up actions (e.g., close open positions or remove trendlines)
            foreach (var position in Positions.FindAll(SymbolName, MagicNumber.ToString()))
            {
                ClosePosition(position);
            }

            // Optionally delete trendlines from the chart
            Chart.RemoveObject("BuyTrendline");
            Chart.RemoveObject("SellTrendline");
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
