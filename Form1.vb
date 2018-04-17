Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public Class Form1
    'Adjust the offsets for different screen sizes - current settings are for 1920x1080 resolution
    Public offsetX As Integer = -7, offsetY As Integer = -8
    Public lineWidth As Single = 1
    Public lineWidthArc As Single = 1
    Public timerInterval As Integer = 1
    Public eclipseDiameter As Single = 3.0F
    Public eclipseBrushTransparency As Integer = 255
    Public eclipseBrushColorOffset As Integer = 64
    Public eclipseDiameterQuantifier As Integer = 7
    Public baseColor1 As Color = Color.White
    Public eclipseBrush As SolidBrush = Nothing
    Public eclipsePen As System.Drawing.Pen = New Pen(baseColor1)
    Public pauseDrawing As Boolean = True

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Bounds = Screen.PrimaryScreen.Bounds
            Me.FormBorderStyle = FormBorderStyle.None
            Me.CenterToScreen()
            Me.WindowState = FormWindowState.Maximized
            Me.DoubleBuffered = True
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.AutoSizeMode = AutoSizeMode.GrowOnly
            Return
        Catch ex As Exception
            Err.Clear()
        Finally
            Me.Invalidate()
        End Try
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Try
            If pauseDrawing = False Or Control.IsKeyLocked(Keys.NumLock) = True Then
                baseColor1 = Color.FromArgb(eclipseBrushTransparency, baseColor1.R, baseColor1.G, baseColor1.B)
                Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
                    Dim pts As New List(Of PointF)
                    Try
                        g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                        g.CompositingMode = CompositingMode.SourceOver

                        ' GET CENTER PIXEL COLOR ON SCREEN
                        Dim Bmp As New Bitmap(1, 1)
                        Dim gTemp As Graphics = Graphics.FromImage(Bmp)
                        gTemp.CopyFromScreen(New Point(CInt((Me.ClientRectangle.Width / 2) + offsetX - eclipseDiameter / 2), CInt((Me.ClientRectangle.Height / 2) + offsetY - eclipseDiameter / 2)), New Point(0, 0), Bmp.Size)
                        Dim colorOfScreenPixel As Color = Color.FromArgb(255, Bmp.GetPixel(0, 0))

                        Dim eclipseBrushColor As Color = baseColor1
                        If CInt(colorOfScreenPixel.R) >= 196 And CInt(colorOfScreenPixel.G) >= 196 And CInt(colorOfScreenPixel.B >= 196) Then 'white
                            eclipseBrushColor = Color.Red
                        ElseIf CInt(colorOfScreenPixel.R) >= 196 And CInt(colorOfScreenPixel.G) >= 196 And CInt(colorOfScreenPixel.B) <= 128 Then 'yellow
                            eclipseBrushColor = Color.Black
                        End If

                        ' DRAW SOLID CIRCLE
                        eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushTransparency, eclipseBrushColor.R, eclipseBrushColor.G, eclipseBrushColor.B))
                        g.FillEllipse(eclipseBrush, CSng((Me.ClientRectangle.Width / 2) + offsetX - eclipseDiameter / 2), CSng((Me.ClientRectangle.Height / 2) + offsetY - eclipseDiameter / 2), eclipseDiameter, eclipseDiameter)
                        g.Dispose()

                        ''VERTICAL CROSSHAIR LINE
                        'Dim verticalLength As Integer = 6 '24
                        'pts.Add(New PointF((Me.ClientRectangle.Width / 2) + offsetX, (Me.ClientRectangle.Height / 2) - verticalLength + offsetY))
                        'pts.Add(New PointF((Me.ClientRectangle.Width / 2) + offsetX, (Me.ClientRectangle.Height / 2) + verticalLength + offsetY)) 'Me.LineShape1.EndPoint = 

                        ''HORIZONTAL CROSSHAIR LINE
                        'pts.Add(New PointF((Me.ClientRectangle.Width / 2) - verticalLength + offsetX, (Me.ClientRectangle.Height / 2) + offsetY))
                        'pts.Add(New PointF((Me.ClientRectangle.Width / 2) + verticalLength + offsetX, (Me.ClientRectangle.Height / 2) + offsetY))
                        'eclipsePen = New Pen(eclipseBrush)
                        'eclipsePen.Width = lineWidth
                        'g.DrawLine(eclipsePen, pts(0), pts(1))
                        'g.DrawLine(eclipsePen, pts(2), pts(3))

                    Catch ex As Exception
                        Err.Clear()
                    End Try
                End Using
            End If
            Return
        Catch ex As Exception
            Err.Clear()
        Finally
            Me.TopMost = True
            Me.BringToFront()
            Me.Invalidate()
        End Try
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        pauseDrawing = True
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        pauseDrawing = True
    End Sub
End Class
