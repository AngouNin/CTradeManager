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
        public double LotsSize { get; set; }

        [Parameter("Magic Number", Group = "Default trade parameters", DefaultValue = 1234)]
        public double MagicNumber { get; set; }

        [Parameter("Slippage Point", Group = "Default trade parameters", DefaultValue = 5)]
        public double SlippagePoint { get; set; }

        [Parameter("Buy Sell buttons for Execution", Group = "Default trade parameters", DefaultValue = true)]
        public bool BuySellButtonsForExecution { get; set; }

        [Parameter("Trendlines for Execution", Group = "Default trade parameters", DefaultValue = false)]
        public bool TrendlinesForExecution { get; set; }

        [Parameter("Take Profit 1", Group = "Default trade parameters", DefaultValue = 200)]
        public double TakeProfitPips1 { get; set; }

        [Parameter("Take Profit 2", Group = "Default trade parameters", DefaultValue = 400)]
        public double TakeProfitPips2 { get; set; }

        [Parameter("Take Profit 3", Group = "Default trade parameters", DefaultValue = 600)]
        public double TakeProfitPips3 { get; set; }

        [Parameter("Take Profit 4", Group = "Default trade parameters", DefaultValue = 1000)]
        public double TakeProfitPips4 { get; set; }

        [Parameter("Stop Loss", Group = "Default trade parameters", DefaultValue = 500)]
        public double StopLossPips { get; set; }


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
            var tradingPanel = new TradingPanel(this, Symbol, LotsSize, StopLossPips, TakeProfitPips1);
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

        public class TradingPanel : CustomControl
        {
            private readonly IDictionary<string, TextBox> _inputMap = new Dictionary<string, TextBox>();
            private readonly Robot _robot;
            private readonly Symbol _symbol;
            private readonly double _lotsSize;
            private readonly double _stopLossPips;
            private readonly double _takeProfitPips1;

            public TradingPanel(Robot robot, Symbol symbol, double lotsSize, double stopLossPips, double takeProfitPips1)
            {
                _robot = robot;
                _symbol = symbol;
                _lotsSize = lotsSize;
                _stopLossPips = stopLossPips;
                _takeProfitPips1 = takeProfitPips1;
                AddChild(CreateTradingPanel());
            }

            private ControlBase CreateTradingPanel()
            {
                var mainPanel = new StackPanel();

                var header = CreateHeader();
                mainPanel.AddChild(header);

                var contentPanel = CreateContentPanel();
                mainPanel.AddChild(contentPanel);

                return mainPanel;
            }

            private static ControlBase CreateHeader()
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

            private StackPanel CreateContentPanel()
            {
                var contentPanel = new StackPanel 
                {
                    Margin = 10
                };
                var grid = new Grid(4, 3);
                grid.Columns[1].SetWidthInPixels(5);

                var buyButton = CreateTradeButton("BUY", Styles.CreateBuyButtonStyle(), TradeType.Buy);
                grid.AddChild(buyButton, 0, 0);

                var sellButton = CreateTradeButton("SELL", Styles.CreateSellButtonStyle(), TradeType.Sell);
                grid.AddChild(sellButton, 0, 2);

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

            private void ExecuteMarketOrderAsync(TradeType tradeType)
            {
                var lots = _lotsSize;
                if (lots <= 0)
                {
                    _robot.Print(string.Format("{0} failed, invalid Lots", tradeType));
                    return;
                }

                var stopLossPips = _stopLossPips;
                var takeProfitPips1 = _takeProfitPips1;

                _robot.Print(string.Format("Open position with: LotsParameter: {0}, StopLossPipsParameter: {1}, TakeProfitPipsParameter: {2}", lots, stopLossPips, takeProfitPips1));

                var volume = _symbol.QuantityToVolumeInUnits(lots);
                _robot.ExecuteMarketOrderAsync(tradeType, _symbol.Name, volume, "CTrade Manager Panel", stopLossPips, takeProfitPips1);
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