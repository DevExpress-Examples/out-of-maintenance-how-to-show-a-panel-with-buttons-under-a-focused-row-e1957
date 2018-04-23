using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace WindowsApplication1
{
    public class ButtonPanelHelper
    {
        private int _PanelHeight;
        public int PanelHeight
        {
            get { return _PanelHeight; }
            set { _PanelHeight = value; }
        }
        private int _ButtonWidth;
        public int ButtonWidth
        {
            get { return _ButtonWidth; }
            set { _ButtonWidth = value; }
        }

        private EditorButtonCollection _Buttons;
        public EditorButtonCollection Buttons
        {
            get { return _Buttons; }
            set { _Buttons = value; }
        }

        private AppearanceObject _PanelAppearance;
        public AppearanceObject PanelAppearance
        {
            get { return _PanelAppearance; }
            set { _PanelAppearance = value; }
        }
 

        #region Fields
        private GridView linkedView;
        private int offset = 5;
        private Rectangle previewBounds;
        private int hotTrackedIndex = -1;
        #endregion

        #region Methods
        public ButtonPanelHelper(GridView view)
        {
            this.linkedView = view;
            Buttons = new EditorButtonCollection();
            InitProperties();
            SubscribeEvents();
        }

        void InitProperties()
        {
            PanelHeight = 40;
            ButtonWidth = 40;
            PanelAppearance = new AppearanceObject();
            PanelAppearance.BackColor = Color.White;
            PanelAppearance.BackColor2 = Color.Gray;
            PanelAppearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            EditorButton button = new EditorButton(ButtonPredefines.Glyph);
            button.Caption = "Test1";
            Buttons.Add(button);
            button = new EditorButton(ButtonPredefines.Glyph);
            button.Caption = "Test2";
            Buttons.Add(button);
            button = new EditorButton(ButtonPredefines.Glyph);
            button.Caption = "Test3";
            Buttons.Add(button);
            linkedView.OptionsView.ShowPreview = true;
         
        }

        void SubscribeEvents()
        {
            linkedView.MeasurePreviewHeight += linkedView_MeasurePreviewHeight;
            linkedView.CustomDrawRowPreview += linkedView_CustomDrawRowPreview;
            linkedView.FocusedRowChanged += linkedView_FocusedRowChanged;
            linkedView.MouseMove += linkedView_MouseMove;
            linkedView.MouseDown += linkedView_MouseDown;
        }

        void linkedView_MouseDown(object sender, MouseEventArgs e)
        {
            if (hotTrackedIndex > -1) MessageBox.Show(Buttons[hotTrackedIndex].Caption);
        }

        void linkedView_MouseMove(object sender, MouseEventArgs e)
        {
            if (hotTrackedIndex != GetHotButton()) InvalidateButtons();
        }

        void InvalidateButtons()
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                linkedView.InvalidateRect(GetButtonRect(previewBounds, i));
            }
        }

        int GetHotButton()
        {
            Point p = GetMousePosition();
            Rectangle result = Rectangle.Empty;
            for (int i = 0; i < Buttons.Count; i++)
            {
                result = GetButtonRect(previewBounds, i);
                if (result.Contains(p)) { hotTrackedIndex = i; return hotTrackedIndex; };
            }
            hotTrackedIndex = -1;
            return hotTrackedIndex; ;
        }


        void linkedView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            linkedView.LayoutChanged();
        }

        void linkedView_CustomDrawRowPreview(object sender, RowObjectCustomDrawEventArgs e)
        {
            previewBounds = e.Bounds;
            PanelAppearance.FillRectangle(e.Cache, e.Bounds);
            DrawButtons(e);
            e.Handled = true;
        }

        void DrawButtons(RowObjectCustomDrawEventArgs e)
        {
            foreach (EditorButton button in Buttons)
            {
                DrawButton(button, e);
            }
        }

        Point GetMousePosition()
        { 
        return linkedView.GridControl.PointToClient(Control.MousePosition);
        }

        GridHitInfo GetHitInfo()
        {
            return linkedView.CalcHitInfo(GetMousePosition());
        }

        void DrawButton(EditorButton button, RowObjectCustomDrawEventArgs e)
        {
            Rectangle buttonRect = GetButtonRect(e.Bounds, button.Index);
            SkinEditorButtonPainter painter = new SkinEditorButtonPainter(linkedView.GridControl.LookAndFeel.ActiveLookAndFeel);
            EditorButtonObjectInfoArgs args = new EditorButtonObjectInfoArgs(e.Cache, button, e.Appearance);
            args.Bounds = buttonRect;
            if (button.Index == hotTrackedIndex) args.State = ObjectState.Hot;
            painter.DrawObject(args);
        }

        int GetPanelHeight(int rowHandle)
        {
            return rowHandle == linkedView.FocusedRowHandle ? PanelHeight : 0;
        }

        Size GetButtonSize()
        {
            return new Size(ButtonWidth, PanelHeight - 2 * offset);
        }

        Rectangle GetButtonRect(Rectangle clientRect, int buttonIndex)
        {
            Size buttonSize = GetButtonSize();
            Rectangle rect = new Rectangle(new Point(0, 0), buttonSize);
            int left = clientRect.Width - (buttonSize.Width + offset) * (buttonIndex + 1) + offset;
            int top = clientRect.Top + offset;
            rect.Offset(left, top);
            return rect;
        }


        #endregion


        #region Events
        void linkedView_MeasurePreviewHeight(object sender, RowHeightEventArgs e)
        {
            e.RowHeight = GetPanelHeight(e.RowHandle);
        }
        #endregion
    }
}
