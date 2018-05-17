Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public Class Form1


    'Note:  Adjust the offsets for different screen sizes and games 
    '       current settings are For 1920x1080 resolution On theHunter: CotW

    ' HORIZONTAL OFFSET
    Public offsetX As Integer = -7
    ' VERTICAL OFFSET
    Public offsetY As Integer = -8

    Public lineWidth As Single = 1
    Public lineWidthArc As Single = 1
    Public timerInterval As Integer = 1
    ' SIZE OF CIRCLE
    Public eclipseDiameter As Single = 6.0F
    Public eclipseBrushTransparency As Integer = 255 '255
    Public baseColor1 As Color = Color.FromArgb(eclipseBrushTransparency, 255, 154, 2)
    Public baseColor2 As Color = Color.FromArgb(eclipseBrushTransparency, 0, 0, 0)
    Public eclipseBrush As SolidBrush = Nothing
    Public eclipsePen As System.Drawing.Pen = New Pen(baseColor2)
    Public pauseDrawing As Boolean = True
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Bounds = Screen.PrimaryScreen.Bounds
            Me.FormBorderStyle = FormBorderStyle.None
            Me.CenterToScreen()
            Me.WindowState = FormWindowState.Maximized
            Me.DoubleBuffered = True
            Me.MaximumSize = Screen.FromControl(Me).WorkingArea.Size
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.AutoSizeMode = AutoSizeMode.GrowOnly
            Return
        Catch ex As Exception
            Err.Clear()
        Finally
            Me.Invalidate(False)
        End Try
    End Sub
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Try
            ' NUM-KEY LOCK TOGGGLES CROSS HAIR DOT
            If pauseDrawing = False Or Control.IsKeyLocked(Keys.NumLock) = True Then
                baseColor1 = Color.FromArgb(eclipseBrushTransparency, baseColor1)
                Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
                    Try
                        ' SET IMAGE SETTINGS
                        g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                        g.CompositingMode = CompositingMode.SourceOver

                        ' DRAW SOLID CIRCLE IN CENTER OF SCREEN
                        eclipseBrush = New SolidBrush(baseColor1)
                        g.FillEllipse(eclipseBrush, CSng((Me.ClientRectangle.Width / 2) + offsetX - eclipseDiameter / 2), CSng((Me.ClientRectangle.Height / 2) + offsetY - eclipseDiameter / 2), eclipseDiameter, eclipseDiameter)
                        ' DRAW BLACK CIRCLE AROUND CENTER DOT
                        eclipsePen.Width = 0.5F
                        g.DrawEllipse(eclipsePen, CSng((Me.ClientRectangle.Width / 2) + offsetX - eclipseDiameter / 2), CSng((Me.ClientRectangle.Height / 2) + offsetY - eclipseDiameter / 2), eclipseDiameter, eclipseDiameter)
                    Catch ex As Exception
                        Err.Clear()
                    Finally
                        g.Dispose()
                    End Try
                End Using
            End If
            Return
        Catch ex As Exception
            Err.Clear()
        Finally
            Me.TopMost = True
            Me.BringToFront()
            'Application.DoEvents()
            Me.Invalidate(False)
        End Try
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        pauseDrawing = True
        Me.Invalidate(False)
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        pauseDrawing = True
        Me.Invalidate(False)
    End Sub
End Class
