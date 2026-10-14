using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;
using UI.Elements;

namespace UI.Dialogs
{
    /// <summary>
    /// Content-sized exit dialog. All children use fixed offsets from the dialog,
    /// whether it has just opened, been dragged, or been re-centered.
    /// </summary>
    public class ExitDialog : UIDialog
    {
        private const float SidePadding = 25f;
        private const float ButtonGap = 16f;
        private const float MessageTop = 55f;
        private const float ButtonTop = 118f;
        private const float BottomPadding = 28f;

        private readonly UIButton m_ReloginButton, m_ExitButton, m_CancelButton;
        private readonly UILabel m_TitleText, m_MessageText;
        private float m_ButtonGap = ButtonGap;
        private float m_ButtonTop = ButtonTop;

        public ExitDialog(UIScreen screen, Vector2 position, TreeWalker walker, string scriptPath)
            : base(screen, position, true, true, true)
        {
            //Dialogs must render above the CAS/SAS browsing controls.
            ZIndex = (int)DrawOrderEnum.MessageBoxes;

            ParseResult result = new ParseResult();
            walker.Initialize(scriptPath, ref result);

            m_ReloginButton = (UIButton)result.Elements["\"ReLoginButton\""];
            m_ExitButton = (UIButton)result.Elements["\"ExitButton\""];
            m_CancelButton = (UIButton)result.Elements["\"CancelButton\""];
            m_TitleText = (UILabel)result.Elements["\"TitleText\""];
            m_MessageText = (UILabel)result.Elements["\"MessageText\""];

            // Avoid AddParent here: it translates positions once AND subscribes
            // to OnDragged. That would conflict with our single layout method.
            foreach (var child in new UIElement[]
                     { m_ReloginButton, m_ExitButton, m_CancelButton, m_TitleText, m_MessageText })
            {
                child.ZIndex = ZIndex + 1;
                Children.Add(child);
            }

            m_Font = screen.Font9px;
            CalculateSize();
            LayoutChildren();
        }

        private void CalculateSize()
        {
            float TitleWidth = m_Screen.Font10px.MeasureString(m_TitleText.Caption ?? "").X;
            float MessageWidth = m_Screen.Font9px.MeasureString(m_MessageText.Caption ?? "").X;
            float TotalButtons = m_ReloginButton.DisplayWidth + m_ExitButton.DisplayWidth + m_CancelButton.DisplayWidth;

            //ScreenArea is in virtual UI coordinates, matching SpriteBatch's
            //Resolution.TransformationMatrix. Leave a 20-pixel margin per side.
            float AvailableWidth = Math.Max(240f, Resolution.ScreenArea.Width - 40f);
            float NaturalWidth = Math.Max(m_DefaultSize.X, Math.Max(Math.Max(TitleWidth + 70f, MessageWidth + 2f * SidePadding), TotalButtons + 2f * ButtonGap + 2f * SidePadding));

            m_ButtonGap = ButtonGap;
            if (NaturalWidth > AvailableWidth)
            {
                m_ButtonGap = 8f;
                float AllowedButtons = AvailableWidth - 2f * SidePadding - 2f * m_ButtonGap;
                if (TotalButtons > AllowedButtons)
                {
                    //Reduce each button proportionally; UIButton keeps its
                    //caption within the actual visual and clickable bounds.
                    float Factor = Math.Max(0.1f, AllowedButtons / TotalButtons);
                    m_ReloginButton.LimitDisplayWidth(m_ReloginButton.DisplayWidth * Factor);
                    m_ExitButton.LimitDisplayWidth(m_ExitButton.DisplayWidth * Factor);
                    m_CancelButton.LimitDisplayWidth(m_CancelButton.DisplayWidth * Factor);
                }
            }

            TotalButtons = m_ReloginButton.DisplayWidth + m_ExitButton.DisplayWidth + m_CancelButton.DisplayWidth;
            float Width = Math.Min(AvailableWidth, Math.Max(m_DefaultSize.X, 
                Math.Max(Math.Max(TitleWidth + 70f, MessageWidth + 2f * SidePadding), TotalButtons + 2f * m_ButtonGap + 2f * SidePadding)));
            m_ButtonTop = Math.Max(ButtonTop, MessageTop + m_Screen.Font9px.MeasureString(m_MessageText.Caption ?? "").Y + 20f);
            float ButtonHeight = Math.Max(m_ReloginButton.DisplayHeight, Math.Max(m_ExitButton.DisplayHeight, m_CancelButton.DisplayHeight));
            float Height = Math.Max(190f, m_ButtonTop + ButtonHeight + BottomPadding);

            //Actual 9-slice background grows to contain the controls.
            SetSize(Width, Height);
        }

        private void LayoutChildren()
        {
            float totalButtons = m_ReloginButton.DisplayWidth + m_ExitButton.DisplayWidth +
                                 m_CancelButton.DisplayWidth + 2f * m_ButtonGap;
            float x = (Size.X - totalButtons) / 2f;
            m_ReloginButton.Position = Position + new Vector2(x, m_ButtonTop);
            x += m_ReloginButton.DisplayWidth + m_ButtonGap;
            m_ExitButton.Position = Position + new Vector2(x, m_ButtonTop);
            x += m_ExitButton.DisplayWidth + m_ButtonGap;
            m_CancelButton.Position = Position + new Vector2(x, m_ButtonTop);

            float titleWidth = m_Screen.Font10px.MeasureString(m_TitleText.Caption ?? "").X;
            float messageWidth = m_Screen.Font9px.MeasureString(m_MessageText.Caption ?? "").X;
            m_TitleText.Position = Position + new Vector2(Math.Max(30f, (Size.X - titleWidth) / 2f), 10f);
            m_MessageText.Position = Position + new Vector2(Math.Max(SidePadding, (Size.X - messageWidth) / 2f), MessageTop);
        }

        public override void Update(InputHelper helper, GameTime gameTime)
        {
            if (!Visible)
                return;

            // Place the controls before hit-testing and after any drag/center.
            LayoutChildren();
            base.Update(helper, gameTime);
            LayoutChildren();
        }
    }
}
