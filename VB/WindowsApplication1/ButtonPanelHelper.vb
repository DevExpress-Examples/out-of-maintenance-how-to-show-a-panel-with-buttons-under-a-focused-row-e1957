Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors.Drawing
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Utils
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.Utils.Drawing
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo

Namespace WindowsApplication1
	Public Class ButtonPanelHelper
		Private _PanelHeight As Integer
		Public Property PanelHeight() As Integer
			Get
				Return _PanelHeight
			End Get
			Set(ByVal value As Integer)
				_PanelHeight = value
			End Set
		End Property
		Private _ButtonWidth As Integer
		Public Property ButtonWidth() As Integer
			Get
				Return _ButtonWidth
			End Get
			Set(ByVal value As Integer)
				_ButtonWidth = value
			End Set
		End Property

		Private _Buttons As EditorButtonCollection
		Public Property Buttons() As EditorButtonCollection
			Get
				Return _Buttons
			End Get
			Set(ByVal value As EditorButtonCollection)
				_Buttons = value
			End Set
		End Property

		Private _PanelAppearance As AppearanceObject
		Public Property PanelAppearance() As AppearanceObject
			Get
				Return _PanelAppearance
			End Get
			Set(ByVal value As AppearanceObject)
				_PanelAppearance = value
			End Set
		End Property


		#Region "Fields"
		Private linkedView As GridView
		Private offset As Integer = 5
		Private previewBounds As Rectangle
		Private hotTrackedIndex As Integer = -1
		#End Region

		#Region "Methods"
		Public Sub New(ByVal view As GridView)
			Me.linkedView = view
			Buttons = New EditorButtonCollection()
			InitProperties()
			SubscribeEvents()
		End Sub

		Private Sub InitProperties()
			PanelHeight = 40
			ButtonWidth = 40
			PanelAppearance = New AppearanceObject()
			PanelAppearance.BackColor = Color.White
			PanelAppearance.BackColor2 = Color.Gray
			PanelAppearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
			Dim button As New EditorButton(ButtonPredefines.Glyph)
			button.Caption = "Test1"
			Buttons.Add(button)
			button = New EditorButton(ButtonPredefines.Glyph)
			button.Caption = "Test2"
			Buttons.Add(button)
			button = New EditorButton(ButtonPredefines.Glyph)
			button.Caption = "Test3"
			Buttons.Add(button)
			linkedView.OptionsView.ShowPreview = True

		End Sub

		Private Sub SubscribeEvents()
			AddHandler linkedView.MeasurePreviewHeight, AddressOf linkedView_MeasurePreviewHeight
			AddHandler linkedView.CustomDrawRowPreview, AddressOf linkedView_CustomDrawRowPreview
			AddHandler linkedView.FocusedRowChanged, AddressOf linkedView_FocusedRowChanged
			AddHandler linkedView.MouseMove, AddressOf linkedView_MouseMove
			AddHandler linkedView.MouseDown, AddressOf linkedView_MouseDown
		End Sub

		Private Sub linkedView_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
			If hotTrackedIndex > -1 Then
				MessageBox.Show(Buttons(hotTrackedIndex).Caption)
			End If
		End Sub

		Private Sub linkedView_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If hotTrackedIndex <> GetHotButton() Then
				InvalidateButtons()
			End If
		End Sub

		Private Sub InvalidateButtons()
			For i As Integer = 0 To Buttons.Count - 1
				linkedView.InvalidateRect(GetButtonRect(previewBounds, i))
			Next i
		End Sub

		Private Function GetHotButton() As Integer
			Dim p As Point = GetMousePosition()
			Dim result As Rectangle = Rectangle.Empty
			For i As Integer = 0 To Buttons.Count - 1
				result = GetButtonRect(previewBounds, i)
				If result.Contains(p) Then
					hotTrackedIndex = i
					Return hotTrackedIndex
				End If
			Next i
			hotTrackedIndex = -1
			Return hotTrackedIndex

		End Function


		Private Sub linkedView_FocusedRowChanged(ByVal sender As Object, ByVal e As FocusedRowChangedEventArgs)
			linkedView.LayoutChanged()
		End Sub

		Private Sub linkedView_CustomDrawRowPreview(ByVal sender As Object, ByVal e As RowObjectCustomDrawEventArgs)
			previewBounds = e.Bounds
			PanelAppearance.FillRectangle(e.Cache, e.Bounds)
			DrawButtons(e)
			e.Handled = True
		End Sub

		Private Sub DrawButtons(ByVal e As RowObjectCustomDrawEventArgs)
			For Each button As EditorButton In Buttons
				DrawButton(button, e)
			Next button
		End Sub

		Private Function GetMousePosition() As Point
		Return linkedView.GridControl.PointToClient(Control.MousePosition)
		End Function

		Private Function GetHitInfo() As GridHitInfo
			Return linkedView.CalcHitInfo(GetMousePosition())
		End Function

		Private Sub DrawButton(ByVal button As EditorButton, ByVal e As RowObjectCustomDrawEventArgs)
			Dim buttonRect As Rectangle = GetButtonRect(e.Bounds, button.Index)
			Dim painter As New SkinEditorButtonPainter(linkedView.GridControl.LookAndFeel.ActiveLookAndFeel)
			Dim args As New EditorButtonObjectInfoArgs(e.Cache, button, e.Appearance)
			args.Bounds = buttonRect
			If button.Index = hotTrackedIndex Then
				args.State = ObjectState.Hot
			End If
			painter.DrawObject(args)
		End Sub

		Private Function GetPanelHeight(ByVal rowHandle As Integer) As Integer
			If rowHandle = linkedView.FocusedRowHandle Then
				Return PanelHeight
			Else
				Return 0
			End If
		End Function

		Private Function GetButtonSize() As Size
			Return New Size(ButtonWidth, PanelHeight - 2 * offset)
		End Function

		Private Function GetButtonRect(ByVal clientRect As Rectangle, ByVal buttonIndex As Integer) As Rectangle
			Dim buttonSize As Size = GetButtonSize()
			Dim rect As New Rectangle(New Point(0, 0), buttonSize)
			Dim left As Integer = clientRect.Width - (buttonSize.Width + offset) * (buttonIndex + 1) + offset
			Dim top As Integer = clientRect.Top + offset
			rect.Offset(left, top)
			Return rect
		End Function


		#End Region


		#Region "Events"
		Private Sub linkedView_MeasurePreviewHeight(ByVal sender As Object, ByVal e As RowHeightEventArgs)
			e.RowHeight = GetPanelHeight(e.RowHandle)
		End Sub
		#End Region
	End Class
End Namespace
