using System.Collections.Generic;
using System.Text;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;

namespace Gonzo.Dialogs
{
    /// <summary>
    /// The buttons which might be on a MessageBox.
    /// </summary>
    public enum MsgBoxButtonEnum
    {
        Ok = 1,
        OkCancel = 2,
        YesNo = 3
    }

    public class MessageBox : UIDialog
    {
        private Dictionary<string, UIElement> m_Elements = new Dictionary<string, UIElement>();
        private Dictionary<string, UIControl> m_Controls = new Dictionary<string, UIControl>();

        protected List<CaretSeparatedText> m_StringTables = new List<CaretSeparatedText>();
        protected Dictionary<string, string> m_Strings = new Dictionary<string, string>();

        private MsgBoxButtonEnum m_Buttons;
        private UIButton m_OKButton, m_CancelButton, m_YesButton, m_NoButton;

        /// <summary>
        /// Occurs when the OK button is clicked. Can be overridden with specific behavior.
        /// The default behavior is to close the messagebox.
        /// </summary>
        public event ButtonClickDelegate OnOKButtonClicked;

        /// <summary>
        /// Occurs when the Cancel button is clicked. Can be overridden with specific behavior.
        /// The default behavior is to close the messagebox.
        /// </summary>
        public event ButtonClickDelegate OnCancelButtonClicked;

        /// <summary>
        /// Occurs when the Yes button is clicked. Can be overridden with specific behavior.
        /// The default behavior is to close the messagebox.
        /// </summary>
        public event ButtonClickDelegate OnYesButtonClicked;

        /// <summary>
        /// Occurs when the No button is clicked. Can be overridden with specific behavior.
        /// The default behavior is to close the messagebox.
        /// </summary>
        public event ButtonClickDelegate OnNoButtonClicked;

        private UILabel m_TitleText, m_MessageText;

        private static int DEFAULTMESSAGEBOXWIDTH = 350;
        private static int DEFAULTMESSAGEBOXHEIGHT = 200;

        /// <summary>
        /// Gets or sets the message for this MessageBox instance.
        /// </summary>
        public string Message
        {
            get { return m_MessageText.Caption; }
            set { m_MessageText.Caption = WrapText(value); }
        }

        /// <summary>
        /// Constructs a new MessageBox instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Position">The position of this MessageBox.</param>
        /// <param name="Message">The caption of this MessageBox.</param>
        /// <param name="Title">The title of this MessageBox.</param>
        public MessageBox(UIScreen Screen, Vector2 Position, string Message = "", string Title = "", 
            MsgBoxButtonEnum Buttons = MsgBoxButtonEnum.Ok) :
            base(Screen, Position, false, true, true)
        {
            m_Buttons = Buttons;

            Vector2 RelativePosition;

            if (Buttons == MsgBoxButtonEnum.Ok)
            {
                switch (Buttons)
                {
                    case MsgBoxButtonEnum.Ok:
                        RelativePosition = new Vector2(150, 120);
                        break;
                    case MsgBoxButtonEnum.OkCancel:
                        RelativePosition = new Vector2(55, 120);
                        break;
                    default:
                        RelativePosition = new Vector2(150, 120);
                        break;
                }

                m_OKButton = new UIButton("OKButton", Position + RelativePosition, Screen, null, "OK", 9, true, this);
                m_OKButton.Position = Position;
                m_OKButton.Position += RelativePosition;
                m_OKButton.OnButtonClicked += M_OKButton_OnButtonClicked;
                OnOKButtonClicked += MessageBox_OnOKButtonClicked;
                m_OKButton.AddParent(this);
            }

            if (Buttons == MsgBoxButtonEnum.OkCancel)
            {
                RelativePosition = new Vector2(250, 120);
                m_CancelButton = new UIButton("CancelButton", Position + RelativePosition, Screen, null, "Cancel", 9,
                    true, this);
                m_CancelButton.Position = Position;
                m_CancelButton.Position += RelativePosition;
                m_CancelButton.OnButtonClicked += M_CancelButton_OnButtonClicked;
                OnCancelButtonClicked += MessageBox_OnCancelButtonClicked;
                m_CancelButton.AddParent(this);
            }

            if(Buttons == MsgBoxButtonEnum.YesNo)
            {
                RelativePosition = new Vector2(55, 120);
                m_YesButton = new UIButton("YesButton", Position + RelativePosition, Screen, null, "Yes", 9, 
                    true, this);
                m_YesButton.Position = Position;
                m_YesButton.Position += RelativePosition;
                m_YesButton.OnButtonClicked += M_YesButton_OnButtonClicked;
                OnYesButtonClicked += MessageBox_OnYesButtonClicked;
                m_YesButton.AddParent(this);

                RelativePosition = new Vector2(250, 120);
                m_NoButton = new UIButton("NoButton", Position + RelativePosition, Screen, null, "No", 9,
                    true, this);
                m_NoButton.Position = Position;
                m_NoButton.Position += RelativePosition;
                m_NoButton.OnButtonClicked += M_NoButton_OnButtonClicked;
                OnNoButtonClicked += MessageBox_OnNoButtonClicked;
                m_NoButton.AddParent(this);
            }

            m_IsDraggable = true;

            m_Font = m_Screen.Manager.Font9px; //Needs to be set for debug purposes.

            RelativePosition = new Vector2(150, 6);
            m_TitleText = new UILabel(Title, 1, Position + RelativePosition, m_Font.MeasureString(Title), 
                Color.Wheat, 9, Screen, this);
            m_TitleText.Position += RelativePosition;
            m_TitleText.AddParent(this);

            RelativePosition = new Vector2(((Size.X / 2) - (m_Font.MeasureString(Message).X / 2)), Size.Y / 2);

            string WrappedMessage = WrapText(Message);
            m_MessageText = new UILabel(Message, 1, Position + RelativePosition, m_Font.MeasureString(WrappedMessage),
                Color.Wheat, 9, Screen, this);
            m_MessageText.Position += Position + RelativePosition;
            m_MessageText.AddParent(this);

            float Scale = Resolution.ScreenArea.Width / Resolution.ScreenArea.Height;
            SetSize(DEFAULTMESSAGEBOXWIDTH * Scale, DEFAULTMESSAGEBOXHEIGHT * Scale);
        }

        #region Handlers

        /// <summary>
        /// The yes button was clicked.
        /// </summary>
        private void M_YesButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            OnYesButtonClicked(this, new ButtonClickEventArgs(MouseButtons.LeftButton));
        }

        /// <summary>
        /// Default behavior for when the yes button is clicked.
        /// </summary>
        private void MessageBox_OnYesButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            this.Visible = false;
        }

        /// <summary>
        /// The no button was clicked.
        /// </summary>
        private void M_NoButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            OnNoButtonClicked(this, new ButtonClickEventArgs(MouseButtons.LeftButton));
        }

        /// <summary>
        /// Default behavior for when the no button is clicked.
        /// </summary>
        private void MessageBox_OnNoButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            this.Visible = false;
        }

        /// <summary>
        /// The ok button was clicked.
        /// </summary>
        private void M_OKButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            OnOKButtonClicked(this, new ButtonClickEventArgs(MouseButtons.LeftButton));
        }

        /// <summary>
        /// Default behavior for when the OK button is clicked.
        /// </summary>
        private void MessageBox_OnOKButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            this.Visible = false;
        }

        /// <summary>
        /// The cancel button was clicked.
        /// </summary>
        private void M_CancelButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            OnCancelButtonClicked(this, new ButtonClickEventArgs(MouseButtons.LeftButton));
        }

        /// <summary>
        /// Default behavior for when the Cancel button is clicked.
        /// </summary>
        private void MessageBox_OnCancelButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            this.Visible = false;
        }

        #endregion

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if (Visible)
            {
                if(m_OKButton != null)
                    m_OKButton.Update(Helper, GTime);

                if(m_CancelButton != null)
                    m_CancelButton.Update(Helper, GTime);

                if (m_YesButton != null)
                    m_YesButton.Update(Helper, GTime);

                if (m_NoButton != null)
                    m_NoButton.Update(Helper, GTime);

                m_TitleText.Update(Helper, GTime);
                m_MessageText.Update(Helper, GTime);

                Vector2 OffsetFromMouse;

                if (m_DoDrag)
                {
                    switch (m_Buttons)
                    {
                        case MsgBoxButtonEnum.Ok:
                            OffsetFromMouse = new Vector2(150, 120);
                            break;
                        case MsgBoxButtonEnum.OkCancel:
                            OffsetFromMouse = new Vector2(55, 120);
                            break;
                        default:
                            OffsetFromMouse = new Vector2(150, 120);
                            break;
                    }

                    m_OKButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                    if (m_CancelButton != null)
                    {
                        OffsetFromMouse = new Vector2(250, 120);
                        m_CancelButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    }

                    if (m_Buttons == MsgBoxButtonEnum.YesNo)
                    {
                        OffsetFromMouse = new Vector2(55, 120);
                        m_YesButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                        OffsetFromMouse = new Vector2(250, 120);
                        m_NoButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    }

                    OffsetFromMouse = new Vector2(150, 6);
                    m_TitleText.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                    OffsetFromMouse = new Vector2(60, 48);
                    m_MessageText.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                }
            }
        }

        /// <summary>
        /// Wraps the text in this MessageBox instance.
        /// </summary>
        /// <param name="text">The text to wrap.</param>
        /// <returns>The wrapped text.</returns>
        private string WrapText(string text)
        {
            if (m_Font.MeasureString(text).X < m_Size.Y)
                return text;

            string[] words = text.Split(' ');
            StringBuilder wrappedText = new StringBuilder();
            float linewidth = 0f;
            float spaceWidth = m_Font.MeasureString(" ").X;
            for (int i = 0; i < words.Length; ++i)
            {
                Vector2 size = m_Font.MeasureString(words[i]);
                if (linewidth + size.X < m_Size.Y)
                    linewidth += size.X + spaceWidth;
                else
                {
                    wrappedText.Append("\n");
                    linewidth = size.X + spaceWidth;
                }
                wrappedText.Append(words[i]);
                wrappedText.Append(" ");
            }

            return wrappedText.ToString();
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            if (Visible)
            {
                float Depth;
                if (LayerDepth != null)
                    Depth = (float)LayerDepth;
                else
                    Depth = 0.10f;

                if (Visible)
                {
                    if(m_OKButton != null)
                        m_OKButton.Draw(SBatch, (float)(Depth + 0.1));

                    if(m_CancelButton != null)
                        m_CancelButton.Draw(SBatch, (float)(Depth + 0.1));

                    m_TitleText.Draw(SBatch, (float)(Depth + 0.1));
                    m_MessageText.Draw(SBatch, (float)(Depth + 0.1));
                }

                base.Draw(SBatch, LayerDepth);
            }
        }
    }
}
