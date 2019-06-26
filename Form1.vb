Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Drawing.Graphics
Imports sm = System.Management
''' <summary>
''' XHAIR
''' Gaming Crosshairs
''' </summary>
Public Class Form1
    ' HORIZONTAL OFFSET
    Public offsetX As Single = 0 '-10
    ' VERTICAL OFFSET
    Public offsetY As Single = 0 '-10

    Public drawXHair As Boolean = False 'False
    Public drawXHairRotateAngle As Single = 0 '45
    ' SIZE OF CIRCLE
    Public eclipseDiameter As Single = 6.0F '3.0F
    Public lineWidth As Single = 1.0F '0.25F
    Public lineWidthArc As Single = 1.0F '1
    Public refreshRate As Single = 59 '120
    Public timerInterval As Integer = 1 '1
    Public eclipseBrushOpacity As Integer = 255 '255
    Public colorIndexFill As Integer = 0
    Public colorIndexPen As Integer = 0
    Public imageSavePath As String = Application.StartupPath.ToString.TrimEnd("\"c) & "\screencapture-"
    Public aTimer As New System.Timers.Timer()
    Public baseColorInnerFill As New List(Of Color)
    Public baseColorOuterRing As New List(Of Color)

    Public eclipseBrush As SolidBrush = Nothing 'Nothing
    Public eclipsePen As System.Drawing.Pen = Nothing
    Public pauseDrawing As Boolean = True
    Public numLockPrevious As Boolean = False
    Public query As New sm.SelectQuery("Win32_VideoController")
    Public Function fillColorList(ByRef colorList As List(Of Color)) As List(Of Color)
        colorList = New List(Of Color)
        colorList.Add(Color.FromArgb(255, 255, 154, 2))
        colorList.Add(Color.FromArgb(255, 255, 255, 255))
        colorList.Add(Color.FromArgb(255, 255, 255, 0))
        colorList.Add(Color.FromArgb(255, 255, 0, 0))
        colorList.Add(Color.FromArgb(255, 0, 255, 0))
        colorList.Add(Color.FromArgb(255, 0, 255, 255))
        colorList.Add(Color.FromArgb(255, 0, 0, 255))
        colorList.Add(Color.FromArgb(255, 64, 64, 64))
        colorList.Add(Color.FromArgb(255, 32, 32, 32))
        colorList.Add(Color.FromArgb(255, 128, 128, 128))
        colorList.Add(Color.FromArgb(255, 192, 192, 192))
        colorList.Add(Color.FromArgb(255, 0, 0, 0))
        colorList.Add(Color.FromArgb(0, Color.Black))
        Return colorList
    End Function
    Public Sub clearCanvas()
        Try
            If Not drawOverlayFullScreen Then
                Using g As Graphics = Graphics.FromHwnd(Me.Handle)
                    g.Clear(Color.Transparent)
                End Using
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public Sub settingLoad(Optional settingName As String = "")
        On Error Resume Next
        Dim pauseTemp = pauseDrawing
        '= GetSetting(Application.ProductName, "SETTINGS", "", )
        offsetX = CSng(GetSetting(Application.ProductName, "SETTINGS" & settingName, "offsetX", offsetX.ToString()))
        offsetY = CSng(GetSetting(Application.ProductName, "SETTINGS" & settingName, "offsetY", offsetY.ToString()))
        drawXHair = CBool(GetSetting(Application.ProductName, "SETTINGS" & settingName, "drawXHair", drawXHair.ToString()))
        drawXHairRotateAngle = CSng(GetSetting(Application.ProductName, "SETTINGS" & settingName, "drawXHairRotateAngle", drawXHairRotateAngle.ToString()))
        eclipseDiameter = CSng(GetSetting(Application.ProductName, "SETTINGS" & settingName, "eclipseDiameter", eclipseDiameter.ToString()))
        lineWidth = CSng(GetSetting(Application.ProductName, "SETTINGS" & settingName, "lineWidth", lineWidth.ToString()))
        lineWidthArc = CSng(GetSetting(Application.ProductName, "SETTINGS" & settingName, "lineWidthArc", lineWidthArc.ToString()))
        imgfilePath = ""
        imgXHair = Nothing
        imgfilePath = CStr(GetSetting(Application.ProductName, "SETTINGS" & settingName, "imgfilePath", ""))
        If Not String.IsNullOrEmpty(imgfilePath) Then
            imgXHair = Bitmap.FromFile(imgfilePath)
        End If
        clearCanvas()
        'refreshRate = CInt(GetSetting(Application.ProductName, "SETTINGS" & settingName, "refreshRate", refreshRate.ToString()))
        'Dim query As New sm.SelectQuery("Win32_VideoController")
        'Dim query As New sm.SelectQuery("Win32_VideoController")
        'For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
        '        Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
        '        If CurrentRefreshRate IsNot Nothing Then
        '            'MessageBox.Show(String.Concat("Refresh = ", CurrentRefreshRate.ToString))
        '            If IsNumeric(CurrentRefreshRate) Then
        '            refreshRate = CSng(CurrentRefreshRate)
        '            Exit For
        '        End If
        '    End If
        'Next
        Timer1.Interval = 100 'CInt(1000 / refreshRate) 'ms '(1000ms / 60hz)
        timerInterval = Timer1.Interval
        eclipseBrushOpacity = CInt(GetSetting(Application.ProductName, "SETTINGS" & settingName, "eclipseBrushOpacity", eclipseBrushOpacity.ToString()))
        imageSavePath = CStr(GetSetting(Application.ProductName, "SETTINGS" & settingName, "imageSavePath", imageSavePath.ToString()))
        colorIndexFill = CInt(GetSetting(Application.ProductName, "SETTINGS" & settingName, "colorIndexFill", colorIndexFill.ToString()))
        colorIndexPen = CInt(GetSetting(Application.ProductName, "SETTINGS" & settingName, "colorIndexPen", colorIndexPen.ToString()))
        eclipsePen = New Pen(Color.FromArgb(eclipseBrushOpacity, baseColorOuterRing(colorIndexPen)))
        eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
        bitmapXHair = Nothing
        pauseDrawing = pauseTemp
        drawOverlay(drawOverlayFullScreen)
    End Sub
    Public Sub settingSave(Optional settingName As String = "")
        'On Error Resume Next
        ' SaveSetting(Application.ProductName, "SETTINGS", "", )
        Dim pauseTemp = pauseDrawing
        pauseDrawing = True
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "offsetX", offsetX.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "offsetY", offsetY.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "drawXHair", drawXHair.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "drawXHairRotateAngle", drawXHairRotateAngle.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "eclipseDiameter", eclipseDiameter.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "lineWidth", lineWidth.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "lineWidthArc", lineWidthArc.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "imgfilePath", imgfilePath.ToString())
        'Dim query As New sm.SelectQuery("Win32_VideoController")
        'For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
        '    Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
        '    If CurrentRefreshRate IsNot Nothing Then
        '        'MessageBox.Show(String.Concat("Refresh = ", CurrentRefreshRate.ToString))
        '        If IsNumeric(CurrentRefreshRate) Then
        '            refreshRate = CSng(CurrentRefreshRate)
        '            Exit For
        '        End If
        '    End If
        'Next
        Timer1.Interval = 100 'CInt(1000 / refreshRate) 'ms '(1000ms / 60hz)
        timerInterval = Timer1.Interval
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "refreshRate", refreshRate.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "timerInterval", timerInterval.ToString())
        eclipseBrushOpacity = eclipseBrush.Color.A
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "eclipseBrushOpacity", eclipseBrushOpacity.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "imageSavePath", imageSavePath.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "colorIndexFill", colorIndexFill.ToString())
        SaveSetting(Application.ProductName, "SETTINGS" & settingName, "colorIndexPen", colorIndexPen.ToString())
        clearCanvas()
        bitmapXHair = Nothing
        pauseDrawing = pauseTemp
        drawOverlay(drawOverlayFullScreen)
    End Sub
    Public Function fibonacci(x1 As Integer, x2 As Integer, count As Integer) As List(Of Integer)
        Dim lst As New List(Of Integer)
        If Not lst.Contains(x1) Then lst.Add(x1)
        If Not lst.Contains(x2) Then lst.Add(x2)
        Dim x3 As Integer = CInt(x1 + x2)
        If Not lst.Contains(x3) Then lst.Add(x3)
        If count > (lst.Count) Then
            For x As Integer = (lst.Count) To count Step 1
                'lst = fibonacci(lst, lst(lst.Count - 1), lst(lst.Count - 2), count)
                x1 = lst(lst.Count - 1)
                x2 = lst(lst.Count - 2)
                If Not lst.Contains(x1) Then lst.Add(x1)
                If Not lst.Contains(x2) Then lst.Add(x2)
                x3 = CInt(x1 + x2)
                If Not lst.Contains(x3) Then lst.Add(x3)
            Next
        End If
        Return lst
    End Function

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            aTimer.Enabled = False
            Dim strOut As String = ""
            For Each x As Integer In fibonacci(CInt(0), CInt(1), CInt(4)).ToArray
                strOut = strOut & "" & x.ToString()
            Next
            System.IO.File.WriteAllText(Application.StartupPath.ToString().TrimEnd("\"c) & "\fibbonacci-series.txt", strOut)
        Catch ex As Exception
            Err.Clear()
        End Try
        Try
            bitmapXHair = Nothing
            clearCanvas()
            'Me.Hide()
            imageSavePath = Application.StartupPath.ToString().TrimEnd("\"c) & "\"c '"C:\Program Files (x86)\Steam\userdata\55379670\760\remote\518790\screenshots\xhair-screenshot-"
            baseColorInnerFill = fillColorList(baseColorInnerFill)
            baseColorOuterRing = fillColorList(baseColorOuterRing)
        Catch ex As Exception
            Err.Clear()
        End Try
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
            Me.BackColor = Color.Transparent
            'Dim query As New sm.SelectQuery("Win32_VideoController")
            'For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
            '    Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
            '    If CurrentRefreshRate IsNot Nothing Then
            '        'MessageBox.Show(String.Concat("Refresh = ", CurrentRefreshRate.ToString))
            '        If IsNumeric(CurrentRefreshRate) Then
            '            refreshRate = CSng(CurrentRefreshRate)
            '            Exit For
            '        End If
            '    End If
            'Next
            Timer1.Interval = 100 ' CInt(1000 / refreshRate) 'ms '(1000ms / 60hz)
            timerInterval = Timer1.Interval
            If drawXHair = True Then
                If eclipseDiameter < 6.0F Then
                    eclipseDiameter = 6.0F
                End If
            End If
            If baseColorOuterRing(colorIndexPen).A > 0 Then
                eclipsePen = New Pen(Color.FromArgb(eclipseBrushOpacity, baseColorOuterRing(colorIndexPen)))
            Else
                eclipsePen = New Pen(baseColorOuterRing(colorIndexPen))
            End If
            If baseColorInnerFill(colorIndexFill).A > 0 Then
                eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
            Else
                eclipseBrush = New SolidBrush(baseColorInnerFill(colorIndexFill))
            End If
            settingSave("_0")
            Return
        Catch ex As Exception
            Err.Clear()
        Finally
            Dim query As New sm.SelectQuery("Win32_VideoController")
            For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
                Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
                If CurrentRefreshRate IsNot Nothing Then
                    'MessageBox.Show(String.Concat("Refresh = ", CurrentRefreshRate.ToString))
                    If IsNumeric(CurrentRefreshRate) Then
                        refreshRate = CSng(CurrentRefreshRate)
                        Exit For
                    End If
                End If
            Next
            pauseDrawing = False
            clearCanvas()
            Timer1.Interval = 1
            Timer1.Enabled = True
            imgfilePath = ""
            imgXHair = Nothing
            bitmapXHair = Nothing
            clearCanvas()
            settingLoad("_0")
        End Try
    End Sub
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Try
            Timer1_Tick(Me, New EventArgs())
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        pauseDrawing = True
        Me.Hide()
        Me.Invalidate(False)
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        pauseDrawing = True
        Me.Visible = False
        ScreenCapture.RefreshDesktop()
    End Sub
    Private WithEvents kbHook As New KeyboardHook
    Private Sub kbHook_KeyDown(ByVal Key As System.Windows.Forms.Keys) Handles kbHook.KeyDown
        'Debug.WriteLine(Key.ToString)
    End Sub
    Public previousKey As Keys
    Public Function DoEvents_Wait(ByVal WaitTimeMilliseconds As Integer) As Boolean
        Try
            Dim dt As DateTime = DateTime.Now
            Dim dtSave As DateTime = DateTime.Now
            Do While dt < dtSave.AddMilliseconds(CInt(WaitTimeMilliseconds + 0))
                dt = DateTime.Now
            Loop
        Catch ex As Exception
            Err.Clear()
        End Try
        Return True
    End Function
    Public Function DoEvents_Wait(ByVal WaitTimeMilliseconds As Integer, dtSave As DateTime) As Boolean
        Try
            Dim dt As DateTime = DateTime.Now
            Do While dt < dtSave.AddMilliseconds(CInt(WaitTimeMilliseconds + 0))
                dt = DateTime.Now
            Loop
        Catch ex As Exception
            Err.Clear()
        End Try
        Return True
    End Function
    Public strInstructions As New System.Text.StringBuilder
    Public Function getInstructions()
        strInstructions = New System.Text.StringBuilder
        strInstructions.AppendLine("CROSSHAIR INSTRUCTIONS:ALT+?")
        strInstructions.AppendLine("ALT+PRINTSCR=Screenshot")
        strInstructions.AppendLine("CNTRL+PRINTSCR=Screenshot")
        strInstructions.AppendLine("PRINTSCR=Screenshot")
        strInstructions.AppendLine("F12=Screenshot")
        strInstructions.AppendLine("ALT+PlusSign=Increase Size 2px")
        strInstructions.AppendLine("ALT+MinusSign=Descrease Size 2px")
        strInstructions.AppendLine("CNTRL+PlusSign=Increase Size 10px")
        strInstructions.AppendLine("CNTRL+MinusSign=Descrease Size 10px")
        strInstructions.AppendLine("ALT+X=Toggle Crosshair paint")
        strInstructions.AppendLine("ALT+Arrows=Move Crosshair 1px")
        strInstructions.AppendLine("CNTRL+Arrows=Move Crosshair 10px")
        strInstructions.AppendLine("ALT+OpenBrackets=Reduce Opacity")
        strInstructions.AppendLine("ALT+CloseBrackets=Increase Opacity")
        strInstructions.AppendLine("ALT+O=cycle outer color")
        strInstructions.AppendLine("ALT+F=cycle fill color")
        strInstructions.AppendLine("ALT+NUMPAD=store preset")
        strInstructions.AppendLine("NUMPAD=recall preset")
        strInstructions.AppendLine("ALT+C=Reset presets")
        strInstructions.AppendLine("ALT+I=Custom Crosshair Image")
        strInstructions.AppendLine("ALT+L=lineWidth+1")
        strInstructions.AppendLine("CNTRL+L=lineWidth-1")
        strInstructions.AppendLine("CNTRL+O=Open screenshot path")
        strInstructions.AppendLine("ALT+F11=toggle fullscreen mode")
        'strInstructions.AppendLine("CNTRL+M=toogle Magnification 2x")
        strInstructions.AppendLine("")
        strInstructions.AppendLine("")
        Return strInstructions.ToString
    End Function
    Private Sub kbHook_KeyUp(ByVal Key As System.Windows.Forms.Keys) Handles kbHook.KeyUp
        'Debug.WriteLine(Key)
        Dim pd As Boolean = pauseDrawing
        Try
            bitmapXHair = Nothing
            If Key = System.Windows.Forms.Keys.PrintScreen Then
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    pauseDrawing = True
                    drawOverlay(drawOverlayFullScreen)
                    If DoEvents_Wait(CInt(1000 / refreshRate)) Then

                        Dim img As System.Drawing.Image = ScreenCapture.CaptureActiveWindow()
                        'img.Save(imageSavePath & DateTime.Now.ToFileTime.ToString().Replace(":"c, "") & ".png", System.Drawing.Imaging.ImageFormat.Png)
                        Dim fn As String = imageSavePath & DateTime.Now.ToFileTime.ToString().Replace(":"c, "") & ".png"
                        img.Save(fn, System.Drawing.Imaging.ImageFormat.Png)
                        Clipboard.Clear()
                        Clipboard.SetImage(img.Clone())

                        bitmapXHair = Nothing
                        clearCanvas()
                    End If
                    pauseDrawing = False
                    drawOverlay(drawOverlayFullScreen)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Then
                    pauseDrawing = True
                    drawOverlay(drawOverlayFullScreen)
                    If DoEvents_Wait(CInt(1000 / refreshRate)) Then
                        Dim img As System.Drawing.Image = ScreenCapture.CaptureActiveWindow()
                        Dim fn As String = imageSavePath & DateTime.Now.ToFileTime.ToString().Replace(":"c, "") & ".png"
                        img.Save(fn, System.Drawing.Imaging.ImageFormat.Png)
                        Clipboard.Clear()
                        Clipboard.SetImage(img.Clone())
                        bitmapXHair = Nothing
                        clearCanvas()

                    End If
                    pauseDrawing = False
                    drawOverlay(drawOverlayFullScreen)
                Else
                    pauseDrawing = True
                    drawOverlay(drawOverlayFullScreen)
                    If DoEvents_Wait(CInt(1000 / refreshRate)) Then
                        Dim img As System.Drawing.Image = ScreenCapture.CaptureDesktop()
                        Dim fn As String = imageSavePath & DateTime.Now.ToFileTime.ToString().Replace(":"c, "") & ".png"
                        img.Save(fn, System.Drawing.Imaging.ImageFormat.Png)
                        Clipboard.Clear()
                        Clipboard.SetImage(img.Clone())
                        bitmapXHair = Nothing
                        clearCanvas()
                    End If
                    pauseDrawing = False
                    drawOverlay(drawOverlayFullScreen)
                End If
                pauseDrawing = False
                Timer1.Enabled = True
                drawOverlay(drawOverlayFullScreen)
            ElseIf Key = System.Windows.Forms.Keys.F12 Then
                pauseDrawing = True
                drawOverlay(drawOverlayFullScreen)
                If DoEvents_Wait(CInt(1000 / refreshRate)) Then
                    'Me.Invalidate(True)
                    Dim img As System.Drawing.Image = ScreenCapture.CaptureActiveWindow()
                    Dim fn As String = imageSavePath & DateTime.Now.ToFileTime.ToString().Replace(":"c, "") & ".png"
                    img.Save(fn, System.Drawing.Imaging.ImageFormat.Png)
                    Clipboard.Clear()
                    Clipboard.SetImage(img.Clone())
                    bitmapXHair = Nothing
                    clearCanvas()
                End If
                pauseDrawing = False
                Timer1.Enabled = True
                drawOverlay(drawOverlayFullScreen)
            ElseIf Key = System.Windows.Forms.Keys.Oemplus Then ' CONTROL + PLUS SIGN INCREASES XHAIR SIZE
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    If eclipseDiameter < 400 Then
                        eclipseDiameter = eclipseDiameter + 2
                        bitmapXHair = Nothing
                        clearCanvas()
                        drawOverlay(drawOverlayFullScreen)
                    End If
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Then
                    If eclipseDiameter < 400 Then
                        eclipseDiameter = eclipseDiameter + 10
                        bitmapXHair = Nothing
                        clearCanvas()
                        drawOverlay(drawOverlayFullScreen)
                    End If
                End If
            ElseIf Key = System.Windows.Forms.Keys.OemMinus Then ' CONTROL + MINUS SIGN DECREASES XHAIR SIZE
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    If eclipseDiameter > 2 Then
                        eclipseDiameter = eclipseDiameter - 2
                        bitmapXHair = Nothing
                        clearCanvas()
                        drawOverlay(drawOverlayFullScreen)
                    End If
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Then
                    If eclipseDiameter > 10 Then
                        eclipseDiameter = eclipseDiameter - 10
                        bitmapXHair = Nothing
                        clearCanvas()
                        drawOverlay(drawOverlayFullScreen)
                    End If
                End If

            ElseIf Key = System.Windows.Forms.Keys.X Then ' X + ALT KEY TOGGLES CENTER CROSS HAIR PAINT
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    If drawXHair = True Then
                        drawXHair = False
                    Else
                        drawXHair = True
                    End If
                    bitmapXHair = Nothing
                    clearCanvas()
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.I Then ' I + ALT KEY TOGGLES CROSS HAIR ICON or IMAGE
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    Dim img As Bitmap = Nothing
                    Dim openDialog1 As New OpenFileDialog
                    Try
                        openDialog1.InitialDirectory = Application.StartupPath
                        openDialog1.Filter = "All Files|*.*"
                        openDialog1.FilterIndex = 0
                        openDialog1.FileName = imgfilePath
                        Select Case openDialog1.ShowDialog(Me)
                            Case DialogResult.Yes, DialogResult.OK
                                imgfilePath = openDialog1.FileName
                                Try
                                    img = Bitmap.FromFile(imgfilePath)
                                    If Not img Is Nothing Then
                                        imgXHair = img.Clone
                                    End If
                                Catch ex As Exception
                                    Err.Clear()
                                End Try
                        End Select
                    Catch ex As Exception
                        Err.Clear()
                    Finally
                        bitmapXHair = Nothing
                        clearCanvas()
                        Timer1.Enabled = True
                        drawOverlay(drawOverlayFullScreen)
                    End Try

                End If
            ElseIf Key = System.Windows.Forms.Keys.F11 Then
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    Dim hwnd As IntPtr = Me.Handle          'IntPtr.Zero
                    Using g As Graphics = Graphics.FromHwnd(hwnd)
                        g.Clear(Color.Transparent)
                    End Using
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    drawOverlayFullScreen = Not drawOverlayFullScreen
                    bitmapXHair = Nothing
                    clearCanvas()
                    pauseDrawing = False
                    Timer1.Enabled = True
                End If
            ElseIf Key = System.Windows.Forms.Keys.Up Then ' CNTRL + UP ARROW  MOVES CENTER UP
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    offsetY = offsetY - 1.0F
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then    ' ALT + UP MOVES ARROW MOVES CENTER UP - MORE
                    offsetY = offsetY - 10
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.Down Then ' CNTRL + DOWN ARROW  MOVES CENTER DOWN
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    offsetY = offsetY + 1.0F
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + DOWN MOVES ARROW MOVES CENTER DOWN - MORE
                    offsetY = offsetY + 10
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.O Then
                If Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Then
                    Process.Start(imageSavePath)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    baseColorInnerFill = fillColorList(baseColorInnerFill)
                    baseColorOuterRing = fillColorList(baseColorOuterRing)
                    If Not baseColorOuterRing Is Nothing Then
                        If baseColorOuterRing.Count > 0 Then
                            If colorIndexPen >= baseColorOuterRing.Count - 1 Then
                                colorIndexPen = 0
                            Else
                                colorIndexPen = colorIndexPen + 1
                            End If
                            If baseColorOuterRing(colorIndexPen).A > 0 Then
                                eclipsePen = New Pen(baseColorOuterRing(colorIndexPen)) 'Color.FromArgb(eclipseBrushOpacity, baseColorOuterRing(colorIndexPen)))
                            Else
                                eclipsePen = New Pen(baseColorOuterRing(colorIndexPen))
                            End If
                            bitmapXHair = Nothing
                            clearCanvas()
                            drawOverlay(drawOverlayFullScreen)
                        End If
                    End If
                End If
            ElseIf Key = System.Windows.Forms.Keys.Left Then ' CNTRL + LEFT ARROW MOVES CENTER LEFT
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    offsetX = offsetX - 1.0F
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    offsetX = offsetX - 10
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.M Then ' CNTRL + LEFT ARROW MOVES CENTER LEFT
                'If Control.ModifierKeys  = System.Windows.Forms.Keys.Alt Then
                '    offsetX = offsetX - 1.0F
                '    bitmapXHair = Nothing
                'ElseIf Control.ModifierKeys  = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys  = System.Windows.Forms.Keys.Control Or Control.ModifierKeys  = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys  = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                '    offsetX = offsetX - 10
                '    bitmapXHair = Nothing
                'End If
            ElseIf Key = System.Windows.Forms.Keys.Right Then ' CNTRL + RIGHT ARROW MOVES CENTER RIGHT
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    offsetX = offsetX + 1.0F
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + RIGHT MOVES ARROW MOVES CENTER RIGHT - MORE
                    offsetX = offsetX + 10
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If

            ElseIf Key = System.Windows.Forms.Keys.C Then
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    Select Case MsgBox("Are you sure you want to reset presets?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.ApplicationModal, "Confirm reset")
                        Case MsgBoxResult.Yes, MsgBoxResult.Ok
                            settingLoad("_0")
                            bitmapXHair = Nothing
                            For i As Integer = 1 To 9
                                settingSave("_" & i.ToString())
                            Next
                            bitmapXHair = Nothing
                            clearCanvas()
                            drawOverlay(drawOverlayFullScreen)
                    End Select
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    settingLoad("_0")
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.OemOpenBrackets Then ' DECREASES OPACITY OF CENTER BRUSH
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    baseColorInnerFill = fillColorList(baseColorInnerFill)
                    baseColorOuterRing = fillColorList(baseColorOuterRing)
                    eclipseBrushOpacity = eclipseBrushOpacity - 16
                    If eclipseBrushOpacity <= 0 Then
                        eclipseBrushOpacity = 0
                    End If
                    If baseColorInnerFill(colorIndexFill).A > 0 Then
                        eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                    Else
                        eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                    End If
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.OemCloseBrackets Then ' INCREASES OPACITY OF CENTER BRUSH
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    baseColorInnerFill = fillColorList(baseColorInnerFill)
                    baseColorOuterRing = fillColorList(baseColorOuterRing)
                    eclipseBrushOpacity = eclipseBrushOpacity + 16
                    If eclipseBrushOpacity > 255 Then
                        eclipseBrushOpacity = 255
                    End If
                    If baseColorInnerFill(colorIndexFill).A > 0 Then
                        eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                    Else
                        'eclipseBrush = New SolidBrush(baseColorInnerFill(colorIndexFill))
                        eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                    End If
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.OemQuestion Then
                Dim strFile As New System.Text.StringBuilder
                strFile.AppendLine("CREATED:" & DateTime.Now.ToString())
                strFile.AppendLine(getInstructions())
                strFile.AppendLine(CStr("offsetX:" & offsetX.ToString() & ",offsetY:" & offsetY.ToString() & ",drawXHair:" & drawXHair.ToString() & ",eclipseDiameter:" & eclipseDiameter.ToString() & ",lineWidth:" & lineWidth.ToString() & ",lineWidthArc:" & lineWidthArc.ToString() & ",eclipseBrushOpacity:" & eclipseBrushOpacity.ToString() & ",refreshRate:" & refreshRate & ",colorIndexPen:" & colorIndexPen & ",colorIndexFill:" & colorIndexFill))
                If Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    System.IO.File.WriteAllText(Application.StartupPath.ToString.TrimEnd("\"c) & "\dimensions.txt", strFile.ToString())
                    Process.Start(Application.StartupPath.ToString.TrimEnd("\"c) & "\dimensions.txt")
                End If
                bitmapXHair = Nothing
                clearCanvas()
                drawOverlay(drawOverlayFullScreen)
            ElseIf Key = System.Windows.Forms.Keys.F Then
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    baseColorInnerFill = fillColorList(baseColorInnerFill)
                    baseColorOuterRing = fillColorList(baseColorOuterRing)
                    If Not baseColorInnerFill Is Nothing Then
                        If baseColorInnerFill.Count > 0 Then
                            If colorIndexFill >= baseColorOuterRing.Count - 1 Then
                                colorIndexFill = 0
                            Else
                                colorIndexFill = colorIndexFill + 1
                            End If
                            If baseColorInnerFill(colorIndexFill).A > 0 Then
                                eclipseBrush = New SolidBrush(baseColorInnerFill(colorIndexFill)) 'Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                            Else
                                eclipseBrush = New SolidBrush(baseColorInnerFill(colorIndexFill))
                            End If
                            bitmapXHair = Nothing
                            clearCanvas()
                            drawOverlay(drawOverlayFullScreen)
                        End If
                    End If
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad0 Then ' LOAD DEFAULT SETTINGS
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    'settingSave("_0")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_0")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad1 Then ' LOAD DEFAULT SETTINGS
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_1")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                Else
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_1")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad2 Then ' LOAD/SAVE SETTINGS SLOT 2
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_2")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_2")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad3 Then ' LOAD/SAVE SETTINGS SLOT 3
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_3")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_3")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad4 Then  ' LOAD/SAVE SETTINGS SLOT 4
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_4")
                    bitmapXHair = Nothing
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    clearCanvas()
                    settingLoad("_4")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad5 Then ' LOAD/SAVE SETTINGS SLOT 5
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_5")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_5")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad6 Then ' LOAD/SAVE SETTINGS SLOT 6
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_6")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_6")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad7 Then ' LOAD/SAVE SETTINGS SLOT 7
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_7")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_7")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad8 Then ' LOAD/SAVE SETTINGS SLOT 8
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_8")
                    bitmapXHair = Nothing
                    clearCanvas()
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_8")
                    'Win32Helper.NotifyFileAssociationChanged()
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumPad9 Then ' LOAD/SAVE SETTINGS SLOT 9
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    settingSave("_9")
                    bitmapXHair = Nothing
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    bitmapXHair = Nothing
                    clearCanvas()
                    imgfilePath = ""
                    imgXHair = Nothing
                    drawOverlay(drawOverlayFullScreen)
                ElseIf KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    pauseDrawing = True
                    bitmapXHair = Nothing
                    clearCanvas()
                    settingLoad("_9")
                    pauseDrawing = False
                    Timer1.Enabled = True
                    drawOverlay(drawOverlayFullScreen)
                End If
            ElseIf Key = System.Windows.Forms.Keys.NumLock Then
                bitmapXHair = Nothing
                clearCanvas()
                Timer1.Enabled = KeyboardHook.IsKeyToggled(Keys.NumLock)
                drawOverlay(drawOverlayFullScreen)
            ElseIf Key = System.Windows.Forms.Keys.L Then ' LOAD/SAVE SETTINGS SLOT 9
                If Control.ModifierKeys = System.Windows.Forms.Keys.Alt Then
                    lineWidth = lineWidth + 1
                    lineWidthArc = lineWidthArc + 1
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                ElseIf Control.ModifierKeys = System.Windows.Forms.Keys.ControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.Control Or Control.ModifierKeys = System.Windows.Forms.Keys.LControlKey Or Control.ModifierKeys = System.Windows.Forms.Keys.RControlKey Then  ' ALT + LEFT MOVES ARROW MOVES CENTER LEFT - MORE
                    If lineWidth > 1 Then
                        lineWidth = lineWidth - 1
                        lineWidthArc = lineWidthArc - 1
                    Else
                        lineWidth = 1
                        lineWidthArc = 1
                    End If
                    bitmapXHair = Nothing
                    clearCanvas()
                    drawOverlay(drawOverlayFullScreen)
                End If
            Else
                'bitmapXHair = Nothing
            End If
        Catch ex As Exception
            Err.Clear()
        Finally
            pauseDrawing = pd
            previousKey = Key
            Timer1.Enabled = True
            'bitmapXHair = Nothing
        End Try
    End Sub
    Dim dblBuffered As Integer = 0
    Public drawOverlayFullScreen As Boolean = False
    Public Sub saveTempFileOpen(strOut As String, Optional openFile As Boolean = True)
        Try
            Dim fn As String = Application.StartupPath.ToString.TrimEnd("\"c) & "\tempData.txt"
            System.IO.File.WriteAllText(fn, strOut)
            If openFile Then
                Process.Start(fn)
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public imgXHair As Bitmap = Nothing
    Public imgfilePath As String = ""
    Public Sub drawOverlay(fullScreen As Boolean)
        Dim dtSave As DateTime = DateTime.Now
        If fullScreen Then
            Try
                ' NUM-KEY LOCK TOGGGLES CROSS HAIR DOT
                If pauseDrawing = False And KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then 'And Me.Visible
                    Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
                        Try
                            Dim bm As Bitmap = New Bitmap(eclipseDiameter + (lineWidth * 2), eclipseDiameter + (lineWidth * 2), Imaging.PixelFormat.Format32bppArgb)
                            Try
                                If Not imgXHair Is Nothing Then
                                    bitmapXHair = Nothing
                                    bitmapXHair = imgXHair
                                ElseIf Not String.IsNullOrEmpty(imgfilePath & "") Then
                                    bitmapXHair = Nothing
                                    bitmapXHair = Bitmap.FromFile(imgfilePath)
                                End If
                                If bitmapXHair Is Nothing Then
                                    Using gClone As Graphics = Graphics.FromImage(bm)
                                        gClone.Clear(Color.Transparent)
                                        If Not drawXHairRotateAngle = 0 Then
                                            Dim mat As New Drawing2D.Matrix
                                            mat.RotateAt(drawXHairRotateAngle, New PointF(CSng(eclipseDiameter / 2), CSng(eclipseDiameter / 2)))
                                            gClone.Transform = mat
                                        End If

                                        ' SET IMAGE SETTINGS
                                        gClone.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                        gClone.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                        gClone.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                                        gClone.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                        gClone.CompositingMode = Drawing2D.CompositingMode.SourceOver

                                        eclipsePen = New Pen(baseColorOuterRing(colorIndexPen))
                                        If baseColorInnerFill(colorIndexFill).A > 0 Then
                                            eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                                        Else
                                            eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                                        End If

                                        ' DRAW SOLID CIRCLE IN CENTER OF SCREEN
                                        If eclipseBrush.Color.A > 0 Then
                                            gClone.FillEllipse(eclipseBrush, lineWidth, lineWidth, CSng(eclipseDiameter - lineWidth), CSng(eclipseDiameter - lineWidth))
                                        End If
                                        ' DRAW BLACK CIRCLE AROUND CENTER DOT
                                        eclipsePen.Width = lineWidth
                                        gClone.DrawEllipse(eclipsePen, lineWidth, lineWidth, CSng(eclipseDiameter - lineWidth), CSng(eclipseDiameter - lineWidth))

                                        If eclipseDiameter >= 20 Then
                                            gClone.DrawEllipse(New Pen(Color.FromArgb(192, 255, 255, 255)), (lineWidth) + (eclipseDiameter / 4), (lineWidth) + (eclipseDiameter / 4), CSng(eclipseDiameter - (eclipseDiameter / 2) - (lineWidth)), CSng(eclipseDiameter - (eclipseDiameter / 2) - (lineWidth)))
                                        End If
                                        'gClone.Dispose()

                                        If drawXHair Then
                                            Dim pts As New List(Of PointF)
                                            Dim penCrossHairs As Pen = New Pen(Color.Black, 1)
                                            ' HORIZONTAL LINE
                                            pts.Add(New PointF(CSng(lineWidth), CSng((eclipseDiameter))))
                                            pts.Add(New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter))))
                                            ' VERTICAL LINE
                                            pts.Add(New PointF(CSng((eclipseDiameter)), CSng(lineWidth)))
                                            pts.Add(New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter))))
                                            gClone.DrawLine(eclipsePen, New PointF(CSng((eclipseDiameter) / 2), lineWidth), New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter - lineWidth))))
                                            gClone.DrawLine(eclipsePen, New PointF(lineWidth, CSng((eclipseDiameter) / 2)), New PointF(CSng((eclipseDiameter - lineWidth)), CSng((eclipseDiameter) / 2)))
                                            Dim ptCenter As New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter) / 2))
                                            If penCrossHairs.Color = Color.Transparent Then
                                                bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                                            Else
                                                If CInt(penCrossHairs.Color.R + penCrossHairs.Color.G + penCrossHairs.Color.B) / 3 >= 128 Then
                                                    bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                                                Else
                                                    bm.SetPixel(ptCenter.X, ptCenter.Y, Color.White)
                                                End If
                                            End If
                                        End If
                                        gClone.Dispose()
                                    End Using
                                    bitmapXHair = bm.Clone()
                                End If
                                ' SET IMAGE SETTINGS
                                g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                                g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                g.CompositingMode = Drawing2D.CompositingMode.SourceOver
                                g.DrawImage(bitmapXHair.Clone(), CSng(g.VisibleClipBounds.Width / 2 + offsetX - bitmapXHair.Width / 2), CSng(g.VisibleClipBounds.Height / 2 + offsetY - bitmapXHair.Height / 2))
                            Catch ex As Exception
                                Err.Clear()
                            Finally
                                bm.Dispose()
                                bm = Nothing
                            End Try
                        Catch ex As Exception
                            Err.Clear()
                        End Try
                    End Using
                End If
                Return
            Catch ex As Exception
                Err.Clear()
            Finally
                If pauseDrawing = False And KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    Try
                        Me.Show() 'If Not Me.Visible Then 
                        Me.TopMost = True 'If Not Me.TopMost Then 
                        Me.BringToFront()
                        If numLockPrevious = False Then
                            numLockPrevious = KeyboardHook.IsKeyToggled(Keys.NumLock)
                            Dim query As New sm.SelectQuery("Win32_VideoController")
                            For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
                                Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
                                If CurrentRefreshRate IsNot Nothing Then
                                    If IsNumeric(CurrentRefreshRate) Then
                                        refreshRate = CSng(CurrentRefreshRate)
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        Err.Clear()
                    Finally
                        Timer1.Interval = 1
                        timerInterval = Timer1.Interval
                        Timer1.Enabled = True
                    End Try
                Else
                    numLockPrevious = KeyboardHook.IsKeyToggled(Keys.NumLock)
                    Me.TopMost = False
                    Me.SendToBack()
                End If
            End Try
        Else
            Try
                ' NUM-KEY LOCK TOGGGLES CROSS HAIR DOT
                If pauseDrawing = False And KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then 'And Me.Visible 
                    Dim hwnd As IntPtr = Me.Handle          'IntPtr.Zero
                    Using g As Graphics = Graphics.FromHwnd(hwnd)
                        Try
                            Dim bm As Bitmap = New Bitmap(eclipseDiameter + (lineWidth * 2), eclipseDiameter + (lineWidth * 2), Imaging.PixelFormat.Format32bppArgb)
                            Try
                                If Not imgXHair Is Nothing Then
                                    bitmapXHair = imgXHair
                                ElseIf Not String.IsNullOrEmpty(imgfilePath & "") Then
                                    bitmapXHair = Bitmap.FromFile(imgfilePath)
                                End If
                                If bitmapXHair Is Nothing Then
                                    Using gClone As Graphics = Graphics.FromImage(bm)
                                        gClone.Clear(Color.Transparent)
                                        If Not drawXHairRotateAngle = 0 Then
                                            Dim mat As New Drawing2D.Matrix
                                            mat.RotateAt(drawXHairRotateAngle, New PointF(CSng(eclipseDiameter / 2), CSng(eclipseDiameter / 2)))
                                            gClone.Transform = mat
                                        End If

                                        ' SET IMAGE SETTINGS
                                        gClone.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                        gClone.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                        gClone.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                                        gClone.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                        gClone.CompositingMode = Drawing2D.CompositingMode.SourceOver

                                        eclipsePen = New Pen(baseColorOuterRing(colorIndexPen))
                                        ' DRAW SOLID CIRCLE IN CENTER OF SCREEN
                                        If eclipseBrush.Color.A > 0 Then
                                            gClone.FillEllipse(eclipseBrush, lineWidth, lineWidth, CSng(eclipseDiameter - lineWidth), CSng(eclipseDiameter - lineWidth))
                                        End If
                                        ' DRAW BLACK CIRCLE AROUND CENTER DOT
                                        eclipsePen.Width = lineWidth
                                        gClone.DrawEllipse(eclipsePen, lineWidth, lineWidth, CSng(eclipseDiameter - lineWidth), CSng(eclipseDiameter - lineWidth))

                                        If eclipseDiameter >= 20 Then
                                            gClone.DrawEllipse(New Pen(Color.FromArgb(192, 255, 255, 255)), (lineWidth) + (eclipseDiameter / 4), (lineWidth) + (eclipseDiameter / 4), CSng(eclipseDiameter - (eclipseDiameter / 2) - (lineWidth)), CSng(eclipseDiameter - (eclipseDiameter / 2) - (lineWidth)))
                                        End If

                                        If drawXHair Then
                                            Dim pts As New List(Of PointF)
                                            Dim penCrossHairs As Pen = New Pen(Color.FromArgb(255, Color.Black), 1.0F)
                                            ' HORIZONTAL LINE
                                            pts.Add(New PointF(CSng(lineWidth), CSng((eclipseDiameter))))
                                            pts.Add(New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter))))
                                            '' VERTICAL LINE
                                            pts.Add(New PointF(CSng((eclipseDiameter)), CSng(lineWidth)))
                                            pts.Add(New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter))))
                                            gClone.DrawLine(eclipsePen, New PointF(CSng((eclipseDiameter) / 2), lineWidth), New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter - lineWidth))))
                                            gClone.DrawLine(eclipsePen, New PointF(lineWidth, CSng((eclipseDiameter) / 2)), New PointF(CSng((eclipseDiameter - lineWidth)), CSng((eclipseDiameter) / 2)))
                                            Dim ptCenter As New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter) / 2))
                                            If penCrossHairs.Color = Color.Transparent Then
                                                bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                                            Else
                                                If CInt(penCrossHairs.Color.R + penCrossHairs.Color.G + penCrossHairs.Color.B) / 3 >= 128 Then
                                                    bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                                                Else
                                                    bm.SetPixel(ptCenter.X, ptCenter.Y, Color.White)
                                                End If
                                            End If
                                        End If
                                        gClone.Dispose()
                                    End Using
                                    bitmapXHair = bm.Clone()
                                    g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                    g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                                    g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                    g.CompositingMode = Drawing2D.CompositingMode.SourceOver
                                End If
                                g.DrawImage(bitmapXHair.Clone(), CSng(g.VisibleClipBounds.Width / 2 + offsetX - bitmapXHair.Width / 2), CSng(g.VisibleClipBounds.Height / 2 + offsetY - bitmapXHair.Height / 2))
                            Catch ex As Exception
                                Err.Clear()
                            Finally
                                bm.Dispose()
                                bm = Nothing
                            End Try
                        Catch ex As Exception
                            Err.Clear()
                        End Try
                    End Using
                End If
                Return
            Catch ex As Exception
                Err.Clear()
            Finally
                If pauseDrawing = False And KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                    If numLockPrevious = False Then
                        numLockPrevious = KeyboardHook.IsKeyToggled(Keys.NumLock)
                        Win32Helper.NotifyFileAssociationChanged()
                    End If
                    Try
                        'Dim query As New sm.SelectQuery("Win32_VideoController")
                        'For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
                        '    Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
                        '    If CurrentRefreshRate IsNot Nothing Then
                        '        'MessageBox.Show(String.Concat("Refresh = ", CurrentRefreshRate.ToString))
                        '        If IsNumeric(CurrentRefreshRate) Then
                        '            refreshRate = CSng(CurrentRefreshRate)
                        '            Exit For
                        '        End If
                        '    End If
                        'Next
                    Catch ex As Exception
                        refreshRate = 143
                        Err.Clear()
                    Finally
                        Timer1.Interval = 100
                        If KeyboardHook.IsKeyToggled(Keys.NumLock) Then
                            If Not Me.Visible Then Me.Show()
                            If Not Me.TopMost = True Then Me.TopMost = True
                            Me.BringToFront()
                        End If
                    End Try
                Else
                    numLockPrevious = KeyboardHook.IsKeyToggled(Keys.NumLock)
                    Me.TopMost = False
                    Me.SendToBack()
                End If
                Timer1.Enabled = True 'KeyboardHook.IsKeyToggled(Keys.NumLock)
            End Try
        End If
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            drawOverlay(drawOverlayFullScreen)
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public Sub OnTimedEvent(source As Object, e As System.Timers.ElapsedEventArgs)
        Dim dtSave As DateTime = DateTime.Now
        Try
            ' NUM-KEY LOCK TOGGGLES CROSS HAIR DOT
            If pauseDrawing = False And KeyboardHook.IsKeyToggled(Keys.NumLock) = True And Me.Visible Then
                Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
                    Try
                        Dim bm As Bitmap = New Bitmap(eclipseDiameter + (lineWidth * 5), eclipseDiameter + (lineWidth * 5), Imaging.PixelFormat.Format32bppArgb)
                        Try
                            Using gClone As Graphics = Graphics.FromImage(bm)
                                gClone.Clear(Color.Transparent)
                                If Not drawXHairRotateAngle = 0 Then
                                    Dim mat As New Drawing2D.Matrix
                                    mat.RotateAt(drawXHairRotateAngle, New PointF(CSng(eclipseDiameter / 2), CSng(eclipseDiameter / 2)))
                                    gClone.Transform = mat
                                End If

                                ' SET IMAGE SETTINGS
                                gClone.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                                gClone.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                                gClone.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                                gClone.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                                gClone.CompositingMode = Drawing2D.CompositingMode.SourceOver


                                eclipsePen = New Pen(baseColorOuterRing(colorIndexPen))
                                If baseColorInnerFill(colorIndexFill).A > 0 Then
                                    eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                                Else
                                    eclipseBrush = New SolidBrush(baseColorInnerFill(colorIndexFill))
                                End If

                                gClone.FillEllipse(eclipseBrush, lineWidth / 2, lineWidth / 2, CSng(eclipseDiameter - lineWidth / 2), CSng(eclipseDiameter - lineWidth / 2))
                                ' DRAW BLACK CIRCLE AROUND CENTER DOT
                                eclipsePen.Width = lineWidth
                                gClone.DrawEllipse(eclipsePen, lineWidth / 2, lineWidth / 2, CSng(eclipseDiameter - lineWidth / 2), CSng(eclipseDiameter - lineWidth / 2))

                                If eclipseDiameter >= 20 Then
                                    gClone.DrawEllipse(New Pen(Color.FromArgb(192, 255, 255, 255)), (lineWidth / 2) + (eclipseDiameter / 4), (lineWidth / 2) + (eclipseDiameter / 4), CSng(eclipseDiameter - (eclipseDiameter / 2) - (lineWidth / 2)), CSng(eclipseDiameter - (eclipseDiameter / 2) - (lineWidth / 2)))
                                End If

                                If drawXHair Then
                                    Dim pts As New List(Of PointF)
                                    Dim penCrossHairs As Pen = New Pen(Color.Transparent, 1)
                                    If baseColorInnerFill(colorIndexFill).A <= 0 Then
                                        penCrossHairs = New Pen(eclipsePen.Color, 1)
                                    End If
                                    ' HORIZONTAL LINE
                                    pts.Add(New PointF(CSng(lineWidth / 2), CSng((eclipseDiameter))))
                                    pts.Add(New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter))))
                                    ' VERTICAL LINE
                                    pts.Add(New PointF(CSng((eclipseDiameter)), CSng(lineWidth / 2)))
                                    pts.Add(New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter))))
                                    gClone.DrawLine(penCrossHairs, New PointF(CSng((eclipseDiameter) / 2), 0), New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter))))
                                    gClone.DrawLine(penCrossHairs, New PointF(0, CSng((eclipseDiameter) / 2)), New PointF(CSng((eclipseDiameter)), CSng((eclipseDiameter) / 2)))
                                    Dim ptCenter As New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter) / 2))
                                    If penCrossHairs.Color = Color.Transparent Then
                                        bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                                    Else
                                        If CInt(penCrossHairs.Color.R + penCrossHairs.Color.G + penCrossHairs.Color.B) / 3 >= 128 Then
                                            bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                                        Else
                                            bm.SetPixel(ptCenter.X, ptCenter.Y, Color.White)
                                        End If
                                    End If
                                End If
                            End Using

                            bitmapXHair = bm.Clone()
                            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                            g.CompositingMode = Drawing2D.CompositingMode.SourceOver
                            g.DrawImage(bitmapXHair.Clone(), CSng(Me.ClientRectangle.Width / 2 + offsetX + lineWidth / 2 - eclipseDiameter / 2), CSng(Me.ClientRectangle.Height / 2 + offsetY + lineWidth / 2 - eclipseDiameter / 2))
                            If KeyboardHook.IsKeyToggled(Keys.NumLock) Then
                                Me.BringToFront()
                                Me.Show()
                                Me.TopMost = True
                            End If
                        Catch ex As Exception
                            Err.Clear()
                        Finally
                            g.Dispose()
                            bm.Dispose()
                            bm = Nothing
                        End Try
                    Catch ex As Exception
                        Err.Clear()
                    End Try
                End Using
            End If
            Return
        Catch ex As Exception
            Err.Clear()
        Finally
            If pauseDrawing = False And KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                Me.BringToFront()
                If Not Me.Visible Then Me.Show()
                If Not Me.TopMost = True Then Me.TopMost = True
                If numLockPrevious = False Then
                    numLockPrevious = KeyboardHook.IsKeyToggled(Keys.NumLock)
                End If
                Try
                    Dim query As New sm.SelectQuery("Win32_VideoController")
                    For Each mo As sm.ManagementObject In New sm.ManagementObjectSearcher(query).Get
                        Dim CurrentRefreshRate As Object = mo("CurrentRefreshRate")
                        If CurrentRefreshRate IsNot Nothing Then
                            If IsNumeric(CurrentRefreshRate) Then
                                refreshRate = CSng(CurrentRefreshRate)
                                Exit For
                            End If
                        End If
                    Next
                Catch ex As Exception
                    refreshRate = 59
                    Err.Clear()
                Finally
                    aTimer.Interval = CInt(CInt(1000 / refreshRate))  'ms '(1000ms / 60hz)
                    aTimer.AutoReset = True
                    If DoEvents_Wait(aTimer.Interval, dtSave) Then
                        timerInterval = aTimer.Interval
                        aTimer.Enabled = True
                        OnTimedEvent(Me, Nothing)
                    End If
                End Try
            Else
                numLockPrevious = KeyboardHook.IsKeyToggled(Keys.NumLock)
                Me.TopMost = False
                Me.SendToBack()
            End If
            aTimer.Enabled = KeyboardHook.IsKeyToggled(Keys.NumLock)
        End Try

    End Sub
    Public bitmapXHair As Bitmap = Nothing
    'bitmapXHair, setBitmapImageXHair
    Public Sub setBitmapImageXHair() 'As System.Drawing.Bitmap
        Dim bm As Bitmap = New Bitmap(eclipseDiameter + (lineWidth * 5), eclipseDiameter + (lineWidth * 5), Imaging.PixelFormat.Format32bppArgb)
        Try
            Using gClone As Graphics = Graphics.FromImage(bm)
                gClone.Clear(Color.Transparent)
                If Not drawXHairRotateAngle = 0 Then
                    Dim mat As New Drawing2D.Matrix
                    mat.RotateAt(drawXHairRotateAngle, New PointF(CSng(eclipseDiameter / 2), CSng(eclipseDiameter / 2)))
                    gClone.Transform = mat
                End If

                ' SET IMAGE SETTINGS
                gClone.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                gClone.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                gClone.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                gClone.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                gClone.CompositingMode = Drawing2D.CompositingMode.SourceOver

                eclipsePen = New Pen(baseColorOuterRing(colorIndexPen))
                If baseColorInnerFill(colorIndexFill).A > 0 Then
                    eclipseBrush = New SolidBrush(Color.FromArgb(eclipseBrushOpacity, baseColorInnerFill(colorIndexFill)))
                Else
                    eclipseBrush = New SolidBrush(baseColorInnerFill(colorIndexFill))
                End If

                ' DRAW SOLID CIRCLE IN CENTER OF SCREEN
                gClone.FillEllipse(eclipseBrush, lineWidth * 3, lineWidth * 3, CSng(eclipseDiameter - lineWidth * 3), CSng(eclipseDiameter - lineWidth * 3))
                ' DRAW BLACK CIRCLE AROUND CENTER DOT
                eclipsePen.Width = lineWidth
                gClone.DrawEllipse(eclipsePen, lineWidth * 3, lineWidth * 3, CSng(eclipseDiameter - lineWidth * 3), CSng(eclipseDiameter - lineWidth * 3))

                If eclipseDiameter >= 20 Then
                    gClone.DrawEllipse(New Pen(Color.FromArgb(192, 255, 255, 255)), lineWidth * 3 + (eclipseDiameter / 4), lineWidth * 3 + (eclipseDiameter / 4), CSng(eclipseDiameter - (eclipseDiameter / 2) - lineWidth * 3), CSng(eclipseDiameter - (eclipseDiameter / 2) - lineWidth * 3))
                End If
                If drawXHair Then
                    Dim pts As New List(Of PointF)
                    Dim penCrossHairs As Pen = New Pen(Color.Transparent, 1)
                    If baseColorInnerFill(colorIndexFill).A <= 0 Then
                        penCrossHairs = New Pen(eclipsePen.Color, 1)
                    End If
                    ' HORIZONTAL LINE
                    pts.Add(New PointF(CSng(lineWidth / 2), CSng((eclipseDiameter) - lineWidth / 2)))
                    pts.Add(New PointF(CSng((eclipseDiameter) - lineWidth / 2), CSng((eclipseDiameter) - lineWidth / 2)))
                    ' VERTICAL LINE
                    pts.Add(New PointF(CSng((eclipseDiameter) - lineWidth / 2), CSng(lineWidth / 2)))
                    pts.Add(New PointF(CSng((eclipseDiameter) - lineWidth / 2), CSng((eclipseDiameter) - lineWidth / 2)))
                    gClone.DrawLine(penCrossHairs, New PointF(pts(0).X, CSng((eclipseDiameter) / 2)), New PointF(pts(1).X, CSng((eclipseDiameter) / 2)))
                    gClone.DrawLine(penCrossHairs, New PointF(CSng((eclipseDiameter) / 2), pts(2).Y), New PointF(CSng((eclipseDiameter) / 2), pts(3).Y))
                    Dim ptCenter As New PointF(CSng((eclipseDiameter) / 2), CSng((eclipseDiameter) / 2))
                    If penCrossHairs.Color = Color.Transparent Then
                        bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                    Else
                        If CInt(penCrossHairs.Color.R + penCrossHairs.Color.G + penCrossHairs.Color.B) / 3 >= 128 Then
                            bm.SetPixel(ptCenter.X, ptCenter.Y, Color.Black)
                        Else
                            bm.SetPixel(ptCenter.X, ptCenter.Y, Color.White)
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            Err.Clear()
        Finally
            bitmapXHair = bm.Clone
        End Try
    End Sub

    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        Try
            Me.TopMost = True
            Return
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        Try
            Me.TopMost = True
            Return
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public NotInheritable Class Win32Helper

        Const SHCNE_ASSOCCHANGED As Integer = &H8000000
        Const SHCNF_IDLIST As Integer = 0

        Private Sub New()
        End Sub

        Private Class NativeMethods
            <DllImport("shell32")>
            Public Shared Sub SHChangeNotify(ByVal wEventId As Integer, ByVal flags As Integer, ByVal item1 As IntPtr, ByVal item2 As IntPtr)
            End Sub
        End Class

        Public Shared Sub NotifyFileAssociationChanged()
            ' SHChangeNotify notifies the system of events. 
            ' You can notify that various events occured, one is SHCNE_ASSOCCHANGED: 
            ' "A file type association has changed.  
            ' SHCNF_IDLIST must be specified in the uFlags parameter.  
            ' dwItem1 and dwItem2 are not used and must be NULL."  
            NativeMethods.SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, Nothing, Nothing)
        End Sub

    End Class
End Class
