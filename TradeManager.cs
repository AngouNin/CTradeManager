using System;
using cAlgo.API;
using cAlgo.API.Internals;
using System.Collections.Generic;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TradeManagerBot : Robot
    {
        [Parameter("Vertical Position", Group = "Panel alignment", DefaultValue = VerticalAlignment.Top)]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Horizontal Position", Group = "Panel alignment", DefaultValue = HorizontalAlignment.Left)]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        // General Parameters
        [Parameter("Lot Size", DefaultValue = 0.01)]
        public double LotSize { get; set; }

        [Parameter("Magic Number", DefaultValue = 1234)]
        public int MagicNumber { get; set; }

        [Parameter("Slippage Points", DefaultValue = 5)]
        public int Slippage { get; set; }

        // Take Profit and Stop Loss Levels
        [Parameter("Take Profit 1", DefaultValue = 200)]
        public double TP1 { get; set; }

        [Parameter("Take Profit 2", DefaultValue = 400)]
        public double TP2 { get; set; }

        [Parameter("Take Profit 3", DefaultValue = 600)]
        public double TP3 { get; set; }

        [Parameter("Take Profit 4", DefaultValue = 1000)]
        public double TP4 { get; set; }

        [Parameter("Stop Loss", DefaultValue = 500)]
        public double SL { get; set; }

        // Trade Execution Settings
        [Parameter("Use Buy/Sell Buttons", DefaultValue = true)]
        public bool UseButtons { get; set; }

        [Parameter("Use Trendline Execution", DefaultValue = true)]
        public bool UseTrendlines { get; set; }

        // Layered Trading Parameters
        [Parameter("Enable Layer 2", DefaultValue = true)]
        public bool EnableLayer2 { get; set; }

        [Parameter("Layer 2 Distance", DefaultValue = 150)]
        public double Layer2Distance { get; set; }

        [Parameter("Enable Layer 3", DefaultValue = true)]
        public bool EnableLayer3 { get; set; }

        [Parameter("Layer 3 Distance", DefaultValue = 150)]
        public double Layer3Distance { get; set; }

        // Trendline Filtering
        [Parameter("Min Trendline Distance", DefaultValue = 20)]
        public double MinTrendlineDistance { get; set; }

        [Parameter("Max Candle Body Size", DefaultValue = 600)]
        public double MaxCandleBodySize { get; set; }

        [Parameter("Delete Trendline after Trade", DefaultValue = true)]
        public bool AutoDeleteTrendline { get; set; }

        protected override void OnStart()
        {
            if (UseButtons)
            {
                DrawTradePanel();
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

            button.Click += args =>
            {
                if (closeAll)
                {
                    CloseAllTrades();
                }
                else if (tradeType.HasValue)
                {
                    ExecuteTrade(tradeType.Value);
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

        private void ExecuteTrade(TradeType tradeType)
        {
            double volume = Symbol.NormalizeVolumeInUnits(LotSize, RoundingMode.Up);
            ExecuteMarketOrder(tradeType, SymbolName, volume, "Layer1", SL, TP1);
            PlacePendingOrders(tradeType);
        }

        private void PlacePendingOrders(TradeType tradeType)
        {
            if (EnableLayer2)
            {
                double layer2Price = tradeType == TradeType.Buy 
                ? Symbol.Ask + Layer2Distance * Symbol.PipSize 
                : Symbol.Bid - Layer2Distance * Symbol.PipSize;
                PlaceLimitOrder(tradeType, SymbolName, LotSize, layer2Price, "Layer2");
            }

            if (EnableLayer3)
            {
                double layer3Price = tradeType == TradeType.Buy 
                ? Symbol.Ask + Layer3Distance * Symbol.PipSize 
                : Symbol.Bid - Layer3Distance * Symbol.PipSize;
                PlaceLimitOrder(tradeType, SymbolName, LotSize, layer3Price, "Layer3");
            }
        }

        protected override void OnBar()
        {
            if (!UseTrendlines) return;

            var lastClose = Bars.ClosePrices.Last(1);
            var lastOpen = Bars.OpenPrices.Last(1);
            double bodySize = Math.Abs(lastClose - lastOpen) / Symbol.PipSize;

            foreach (var obj in Chart.Objects)
            {
                if (obj is ChartTrendLine trendline)
                {
                    bool isBuy = trendline.Color == Color.Green;
                    bool isSell = trendline.Color == Color.Red;
                    double trendlinePrice = trendline.Y1;

                    // Validate candle body size before executing trade
                    if (bodySize < MinTrendlineDistance || bodySize > MaxCandleBodySize)
                        continue;

                    if (isBuy && lastClose > trendlinePrice)
                        ExecuteTrade(TradeType.Buy);

                    if (isSell && lastClose < trendlinePrice)
                        ExecuteTrade(TradeType.Sell);

                    // Auto delete trendline to prevent multiple executions
                    if (AutoDeleteTrendline)
                        Chart.RemoveObject(trendline.Name);
                }
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
