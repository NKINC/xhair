Imports System.Windows.Forms

Public Class DialogSettings
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        saveSettingsToForm()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
    Public Sub refreshPicturebox()
        Try
            Dim bm As Bitmap = ownerForm.getOverlay(True, getSetting())
            If bm.Width > PictureBox3.ClientSize.Width OrElse bm.Height > PictureBox3.ClientSize.Height Then
                PictureBox3.SizeMode = PictureBoxSizeMode.Zoom 'if image is larger than picturebox
            Else
                PictureBox3.SizeMode = PictureBoxSizeMode.CenterImage 'if image is smaller than picturebox
            End If
            If Not bm Is Nothing Then
                'bm.MakeTransparent(Color.Black)
                PictureBox3.Parent = Me
                SetStyle(ControlStyles.SupportsTransparentBackColor, True)
                If CheckBox2.Checked Then
                    PictureBox3.BackColor = Color.Black
                Else
                    PictureBox3.BackColor = Color.Transparent
                End If
                PictureBox3.Image = bm.Clone
            Else
                PictureBox3.Image = Nothing
            End If

        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public Sub refreshPicturebox(s As Setting)
        Try
            Dim bm As Bitmap = ownerForm.getOverlay(True, s)
            If bm.Width > PictureBox3.ClientSize.Width OrElse bm.Height > PictureBox3.ClientSize.Height Then
                PictureBox3.SizeMode = PictureBoxSizeMode.Zoom 'if image is larger than picturebox
            Else
                PictureBox3.SizeMode = PictureBoxSizeMode.CenterImage 'if image is smaller than picturebox
            End If
            If Not bm Is Nothing Then
                'bm.MakeTransparent(Color.Black)
                PictureBox3.Parent = Me
                SetStyle(ControlStyles.SupportsTransparentBackColor, True)
                If CheckBox2.Checked Then
                    PictureBox3.BackColor = Color.Black
                Else
                    PictureBox3.BackColor = Color.Transparent
                End If
                PictureBox3.Image = bm.Clone
            Else
                PictureBox3.Image = Nothing
            End If

        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Private Sub DialogSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public presetIndex As Integer = 0
    Public ownerForm As Form1 = Nothing
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
    Public imgfilePath As String = ""
    Public imageSavePath As String = Application.StartupPath.ToString.TrimEnd("\"c) & "\screencapture-"
    Public settings As New List(Of Setting)
    <Serializable()>
    Public Class Setting
        Public presetIndex As Integer '= 0
        'Public ownerForm As Form1 '= Nothing
        ' HORIZONTAL OFFSET
        Public offsetX As Single '= 0 '-10
        ' VERTICAL OFFSET
        Public offsetY As Single '= 0 '-10

        Public drawXHair As Boolean '= False 'False
        Public drawXHairRotateAngle As Single '= 0 '45
        ' SIZE OF CIRCLE
        Public eclipseDiameter As Single '= 6.0F '3.0F
        Public lineWidth As Single '= 1.0F '0.25F
        Public lineWidthArc As Single '= 1.0F '1
        Public refreshRate As Single '= 59 '120
        Public timerInterval As Integer '= 1 '1
        Public eclipseBrushOpacity As Integer '= 255 '255
        Public colorIndexFill As Integer '= 0
        Public colorIndexPen As Integer '= 0
        Public imgfilePath As String '= ""


        Public imageSavePath As String = Application.StartupPath.ToString.TrimEnd("\"c) & "\screencapture-"
        Public settingName As String = ""

        Public Sub New()
            presetIndex = 0
            'ownerForm = Nothing
            offsetX = 0
            offsetY = 0
            drawXHair = False
            drawXHairRotateAngle = 0
            eclipseDiameter = 6.0F
            lineWidth = 1.0F
            lineWidthArc = 1.0F
            refreshRate = 59
            timerInterval = 1
            eclipseBrushOpacity = 255
            colorIndexFill = 0
            colorIndexPen = 0
            imgfilePath = ""
            imageSavePath = Application.StartupPath.ToString.TrimEnd("\"c) & "\screencapture-"
            settingName = ""
        End Sub

        Public Sub New(settingName1 As String, presetIndex1 As Integer, offsetX1 As Integer, offsetY1 As Integer, drawXHairRotateAngle1 As Single, eclipseDiameter1 As Single, lineWidth1 As Single, lineWidthArc1 As Single, refreshRate1 As Single, timerInterval1 As Single, eclipseBrushOpacity1 As Integer, colorIndexFill1 As Integer, colorIndexPen1 As Integer, imageSavePath1 As String, colorBaseColorInnerFill1 As Color, colorBaseColorOuterRing1 As Color, drawXHair1 As Boolean, imgfilePath1 As String)
            'ownerForm = Nothing
            settingName = settingName
            presetIndex = presetIndex1
            offsetX = offsetX1
            offsetY = offsetY1
            drawXHair = drawXHair1
            drawXHairRotateAngle = drawXHairRotateAngle1
            eclipseDiameter = eclipseDiameter1
            lineWidth = lineWidth1
            lineWidthArc = lineWidthArc1
            refreshRate = refreshRate1
            timerInterval = timerInterval1
            eclipseBrushOpacity = eclipseBrushOpacity1
            colorIndexFill = colorIndexFill1
            colorIndexPen = colorIndexPen1
            imgfilePath = imgfilePath1
            imageSavePath = imageSavePath1
        End Sub
    End Class
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try
            ComboBox1.Items.Clear()
            For i As Integer = 0 To 9 Step 1
                ComboBox1.Items.Add(CStr(i.ToString()))
            Next

        Catch ex As Exception
            Err.Clear()
        End Try
        If Not Me.Owner Is Nothing Then
            'ownerForm = Me.Owner
            If Not ownerForm Is Nothing Then
                loadSettingsFromForm(True)
            Else
                loadSettings()
            End If
        Else
            loadSettings()
        End If
    End Sub
    Public Sub New(ByRef ownerForm1 As Form1)

        ' This call is required by the designer.
        InitializeComponent()
        Try
            ComboBox1.Items.Clear()
            For i As Integer = 0 To 9 Step 1
                ComboBox1.Items.Add(CStr(i.ToString()))
            Next
        Catch ex As Exception
            Err.Clear()
        End Try
        ' Add any initialization after the InitializeComponent() call.
        If Not ownerForm1 Is Nothing Then
            ownerForm = ownerForm1
            loadSettingsFromForm(True)
        Else
            loadSettings()
        End If
    End Sub
    Public Sub loadSettings()
        If Not ownerForm Is Nothing Then
            TextBoxPresetIndex.Text = ownerForm.indexPreset
            ComboBox1.SelectedIndex = ownerForm.indexPreset
            TrackBar2.Maximum = ownerForm.baseColorInnerFill.Count - 1
            TrackBar3.Maximum = ownerForm.baseColorOuterRing.Count - 1
        Else
            TextBoxPresetIndex.Text = presetIndex
            ComboBox1.SelectedIndex = presetIndex
        End If
        TextBox1.Text = offsetX
        TextBox2.Text = offsetY
        TextBox3.Text = drawXHairRotateAngle
        TextBox4.Text = eclipseDiameter
        TextBox5.Text = lineWidth
        TextBox6.Text = lineWidthArc
        TextBox7.Text = refreshRate
        TextBox8.Text = timerInterval
        TextBox9.Text = eclipseBrushOpacity
        TextBox10.Text = colorIndexFill
        TextBox11.Text = colorIndexPen
        TextBox12.Text = imageSavePath
        TextBox13.Text = imgfilePath
        TrackBar1.Value = CInt(TextBox9.Text)
        TrackBar2.Value = CInt(TextBox10.Text)
        TrackBar3.Value = CInt(TextBox11.Text)
        CheckBox1.Checked = drawXHair

        Try
            If Not ownerForm Is Nothing Then
                PictureBox1.BackColor = ownerForm.baseColorInnerFill(CInt(TrackBar2.Value))
                PictureBox2.BackColor = ownerForm.baseColorOuterRing(CInt(TrackBar3.Value))
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public Sub loadSetting(s As Setting)
        If Not ownerForm Is Nothing Then
            TrackBar2.Maximum = ownerForm.baseColorInnerFill.Count - 1
            TrackBar3.Maximum = ownerForm.baseColorOuterRing.Count - 1
        End If

        Try
            TextBox1.Text = s.offsetX
            TextBox2.Text = s.offsetY
            TextBox3.Text = s.drawXHairRotateAngle
            TextBox4.Text = s.eclipseDiameter
            TextBox5.Text = s.lineWidth
            TextBox6.Text = s.lineWidthArc
            TextBox7.Text = s.refreshRate
            TextBox8.Text = s.timerInterval
            TextBox9.Text = s.eclipseBrushOpacity
            TextBox10.Text = s.colorIndexFill
            TextBox11.Text = s.colorIndexPen
            TextBox12.Text = s.imageSavePath
            TextBox13.Text = s.imgfilePath
            TrackBar1.Value = CInt(TextBox9.Text)
            TrackBar2.Value = CInt(TextBox10.Text)
            TrackBar3.Value = CInt(TextBox11.Text)
            CheckBox1.Checked = s.drawXHair
            If Not ownerForm Is Nothing Then
                PictureBox1.BackColor = ownerForm.baseColorInnerFill(CInt(TrackBar2.Value))
                PictureBox2.BackColor = ownerForm.baseColorOuterRing(CInt(TrackBar3.Value))
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public Function getSetting() As Setting
        Dim s As New Setting
        s.presetIndex = TextBoxPresetIndex.Text
        s.offsetX = TextBox1.Text
        s.offsetY = TextBox2.Text
        s.drawXHairRotateAngle = TextBox3.Text
        s.eclipseDiameter = TextBox4.Text
        s.lineWidth = TextBox5.Text
        s.lineWidthArc = TextBox6.Text
        s.refreshRate = TextBox7.Text
        s.timerInterval = TextBox8.Text
        s.eclipseBrushOpacity = TextBox9.Text
        s.colorIndexFill = TextBox10.Text
        s.colorIndexPen = TextBox11.Text
        s.imageSavePath = TextBox12.Text
        s.imgfilePath = TextBox13.Text
        s.drawXHair = CheckBox1.Checked
        Return s
    End Function
    Public Sub loadSettings(index As Integer, countBaseColorInnerFill As Integer, countBaseColorOuterRing As Integer, offsetX1 As Integer, offsetY1 As Integer, drawXHairRotateAngle1 As Single, eclipseDiameter1 As Single, lineWidth1 As Single, lineWidthArc1 As Single, refreshRate1 As Single, timerInterval1 As Single, eclipseBrushOpacity1 As Integer, colorIndexFill1 As Integer, colorIndexPen1 As Integer, imageSavePath1 As String, colorBaseColorInnerFill1 As Color, colorBaseColorOuterRing1 As Color, drawXHair1 As Boolean, imgfilePath1 As String)
        TextBoxPresetIndex.Text = index
        If index >= 0 Then
            ComboBox1.SelectedIndex = index
        End If
        Try
            TrackBar2.Maximum = countBaseColorInnerFill
            TrackBar3.Maximum = countBaseColorOuterRing
            TextBox1.Text = offsetX1
            TextBox2.Text = offsetY1
            TextBox3.Text = drawXHairRotateAngle1
            TextBox4.Text = eclipseDiameter1
            TextBox5.Text = lineWidth1
            TextBox6.Text = lineWidthArc1
            TextBox7.Text = refreshRate1
            TextBox8.Text = timerInterval1
            TextBox9.Text = eclipseBrushOpacity1
            TextBox10.Text = colorIndexFill1
            TextBox11.Text = colorIndexPen1
            TextBox12.Text = imageSavePath1
            TextBox13.Text = imgfilePath1
            TrackBar1.Value = CInt(TextBox9.Text)
            TrackBar2.Value = CInt(TextBox10.Text)
            TrackBar3.Value = CInt(TextBox11.Text)
            CheckBox1.Checked = drawXHair1

            PictureBox1.BackColor = colorBaseColorInnerFill1 'ownerForm.baseColorInnerFill(CInt(TrackBar2.Value))
            PictureBox2.BackColor = colorBaseColorOuterRing1 'ownerForm.baseColorOuterRing(CInt(TrackBar3.Value))
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
    Public Sub loadSettingsFromForm(Optional UpdateComboBox1 As Boolean = False)
        If Not ownerForm Is Nothing Then
            TextBoxPresetIndex.Text = ownerForm.indexPreset
            If UpdateComboBox1 Then
                ComboBox1.SelectedIndex = ownerForm.indexPreset
            End If
            TextBox1.Text = ownerForm.offsetX
            TextBox2.Text = ownerForm.offsetY
            TextBox3.Text = ownerForm.drawXHairRotateAngle
            TextBox4.Text = ownerForm.eclipseDiameter
            TextBox5.Text = ownerForm.lineWidth
            TextBox6.Text = ownerForm.lineWidthArc
            TextBox7.Text = ownerForm.refreshRate
            TextBox8.Text = ownerForm.timerInterval
            TextBox9.Text = ownerForm.eclipseBrushOpacity
            TextBox10.Text = ownerForm.colorIndexFill
            TextBox11.Text = ownerForm.colorIndexPen
            TextBox12.Text = ownerForm.imageSavePath
            TextBox13.Text = ownerForm.imgfilePath
            TrackBar2.Maximum = ownerForm.baseColorInnerFill.Count - 1
            TrackBar3.Maximum = ownerForm.baseColorOuterRing.Count - 1
            TrackBar1.Value = CInt(TextBox9.Text)
            TrackBar2.Value = CInt(TextBox10.Text)
            TrackBar3.Value = CInt(TextBox11.Text)
            CheckBox1.Checked = ownerForm.drawXHair
            Try
                If Not ownerForm Is Nothing Then
                    PictureBox1.BackColor = ownerForm.baseColorInnerFill(CInt(TrackBar2.Value))
                    PictureBox2.BackColor = ownerForm.baseColorOuterRing(CInt(TrackBar3.Value))
                End If
            Catch ex As Exception
                Err.Clear()
            End Try
            Try
                If Not ownerForm Is Nothing Then
                    TextBoxCountdown.Text = ownerForm.countdownMax.ToString()
                    TextBoxCountdownText.Text = ownerForm.countdownText & ""
                Else
                    TextBoxCountdown.Text = 10
                    TextBoxCountdownText.Text = "GO!"
                End If
            Catch ex As Exception
                Err.Clear()
            End Try
        End If
    End Sub
    Public Sub saveSettings()
        presetIndex = TextBoxPresetIndex.Text
        offsetX = TextBox1.Text
        offsetY = TextBox2.Text
        drawXHairRotateAngle = TextBox3.Text
        eclipseDiameter = TextBox4.Text
        lineWidth = TextBox5.Text
        lineWidthArc = TextBox6.Text
        refreshRate = TextBox7.Text
        timerInterval = TextBox8.Text
        eclipseBrushOpacity = TextBox9.Text
        colorIndexFill = TextBox10.Text
        colorIndexPen = TextBox11.Text
        imageSavePath = TextBox12.Text
        imgfilePath = TextBox13.Text
        drawXHair = CheckBox1.Checked
    End Sub
    Public Sub saveSettingsToForm()
        Try
            If Not ownerForm Is Nothing Then
                ownerForm.indexPreset = CInt(TextBoxPresetIndex.Text)
                ownerForm.offsetX = CSng(TextBox1.Text)
                ownerForm.offsetY = CSng(TextBox2.Text)
                ownerForm.drawXHairRotateAngle = CSng(TextBox3.Text)
                ownerForm.eclipseDiameter = CSng(TextBox4.Text)
                ownerForm.lineWidth = CSng(TextBox5.Text)
                ownerForm.lineWidthArc = CSng(TextBox6.Text)
                ownerForm.refreshRate = CSng(TextBox7.Text)
                ownerForm.timerInterval = CInt(TextBox8.Text)
                ownerForm.eclipseBrushOpacity = CInt(TextBox9.Text)
                ownerForm.colorIndexFill = CInt(TextBox10.Text)
                ownerForm.colorIndexPen = CInt(TextBox11.Text)
                ownerForm.imageSavePath = CStr(TextBox12.Text)
                ownerForm.imgfilePath = CStr(TextBox13.Text)
                ownerForm.drawXHair = CheckBox1.Checked
                If ownerForm.indexPreset > 0 Then
                    ownerForm.settingSave(CStr("_" + presetIndex.ToString()))
                    If KeyboardHook.IsKeyToggled(Keys.NumLock) = True Then
                        ownerForm.pauseDrawing = True
                        ownerForm.bitmapXHair = Nothing
                        ownerForm.clearCanvas()
                        ownerForm.settingLoad(CStr("_" + presetIndex.ToString()))
                        'Win32Helper.NotifyFileAssociationChanged()
                        ownerForm.pauseDrawing = False
                        ownerForm.Timer1.Enabled = True
                        ownerForm.drawOverlay(ownerForm.drawOverlayFullScreen)
                    End If
                End If
                ownerForm.countdownText = CStr(TextBoxCountdownText.Text & "")
                ownerForm.countdownMax = CInt(TextBoxCountdown.Text)
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If Not String.IsNullOrEmpty(TextBox12.Text) Then
                If System.IO.Directory.Exists(TextBox12.Text) Then
                    FolderBrowserDialog1.SelectedPath = CStr(TextBox12.Text)
                End If
            End If
            Select Case FolderBrowserDialog1.ShowDialog(Me)
                Case DialogResult.Yes, DialogResult.OK
                    TextBox12.Text = FolderBrowserDialog1.SelectedPath
            End Select
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            Me.TextBoxPresetIndex.Text = ComboBox1.SelectedIndex.ToString()
            presetIndex = CInt(Me.TextBoxPresetIndex.Text)
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Try
            TextBox9.Text = TrackBar1.Value.ToString()
            refreshPicturebox()
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub TrackBar2_Scroll(sender As Object, e As EventArgs) Handles TrackBar2.Scroll
        Try
            TextBox10.Text = TrackBar2.Value.ToString()
            If Not ownerForm Is Nothing Then
                PictureBox1.BackColor = ownerForm.baseColorInnerFill(CInt(TrackBar2.Value))
                refreshPicturebox()
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub TrackBar3_Scroll(sender As Object, e As EventArgs) Handles TrackBar3.Scroll
        Try
            TextBox11.Text = TrackBar3.Value.ToString()
            If Not ownerForm Is Nothing Then
                PictureBox2.BackColor = ownerForm.baseColorInnerFill(CInt(TrackBar3.Value))
                refreshPicturebox()
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Me.TextBoxPresetIndex.Text = ComboBox1.SelectedIndex.ToString()
            presetIndex = CInt(Me.TextBoxPresetIndex.Text)
            If presetIndex >= 0 Then
                ownerForm.settingLoad(CStr("_" + presetIndex.ToString()))
                loadSettingsFromForm(False)
            Else
                loadSettings()
            End If
        Catch ex As Exception
            Err.Clear()
        Finally
            refreshPicturebox()
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        saveSettingsToForm()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox7_TextChanged(sender As Object, e As EventArgs) Handles TextBox7.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub TextBox8_TextChanged(sender As Object, e As EventArgs) Handles TextBox8.TextChanged

        refreshPicturebox()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        refreshPicturebox()
    End Sub

    Private Sub CheckBox1_Click(sender As Object, e As EventArgs) Handles CheckBox1.Click
        refreshPicturebox()
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click

    End Sub

    Private Sub PictureBox3_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox3.DoubleClick

    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        refreshPicturebox()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            Dim fn As String = Application.StartupPath.ToString.TrimEnd("\"c) & "\crosshair-" & ComboBox1.SelectedIndex.ToString & ".png"
            PictureBox3.Image.Save(fn, System.Drawing.Imaging.ImageFormat.Png)
            Process.Start(fn)
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            TextBox13.Text = ""
            imgfilePath = ""
        Catch ex As Exception
            Err.Clear()
        Finally
            refreshPicturebox()
        End Try
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            Dim img As Bitmap = Nothing
            Dim openDialog1 As New OpenFileDialog
            Try
                openDialog1.InitialDirectory = Application.StartupPath
                openDialog1.Filter = "All Files|*.*"
                openDialog1.FilterIndex = 0
                openDialog1.FileName = ownerForm.imgfilePath

                Select Case openDialog1.ShowDialog(Me)
                    Case DialogResult.Yes, DialogResult.OK
                        ownerForm.imgfilePath = openDialog1.FileName
                        TextBox13.Text = openDialog1.FileName
                        Try
                            img = Bitmap.FromFile(ownerForm.imgfilePath)
                            If Not img Is Nothing Then
                                ownerForm.imgXHair = img.Clone
                            End If
                        Catch ex As Exception
                            Err.Clear()
                        End Try
                    Case Else

                End Select
            Catch ex As Exception
                Err.Clear()
            Finally
                refreshPicturebox()
            End Try
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Try
            'export
            If Not ownerForm Is Nothing Then
                Dim fp As String = ""
                Dim fs As New SaveFileDialog
                fs.Filter = "xhair|*.xhair|all|*.*"
                fs.DefaultExt = "xhair"
                fs.FileName = "setting-" & ComboBox1.SelectedIndex.ToString() & ".xhair"
                fs.InitialDirectory = System.IO.Path.GetDirectoryName(Application.StartupPath.ToString.TrimEnd("\"c) & "\")
                Select Case fs.ShowDialog(Me)
                    Case DialogResult.Yes, DialogResult.OK
                        fp = fs.FileName
                        Dim s As Setting = getSetting()
                        Dim xml_serializer As New System.Xml.Serialization.XmlSerializer(GetType(Setting))
                        Dim streamMem As New System.IO.MemoryStream
                        xml_serializer.Serialize(streamMem, s)
                        fp = fs.FileName
                        System.IO.File.WriteAllBytes(fp, streamMem.ToArray())
                End Select
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Err.Clear()
        End Try
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Try
            'import
            If Not ownerForm Is Nothing Then
                Dim fp As String = "setting-" & ComboBox1.SelectedIndex.ToString() & ".xhair"
                Dim fs As New OpenFileDialog
                fs.Filter = "xhair|*.xhair|all|*.*"
                fs.DefaultExt = "xhair"
                fs.FileName = ""
                fs.InitialDirectory = System.IO.Path.GetDirectoryName(Application.StartupPath.ToString.TrimEnd("\"c) & "\")
                Select Case fs.ShowDialog(Me)
                    Case DialogResult.Yes, DialogResult.OK
                        fp = fs.FileName
                        'Dim s As Setting = ownerForm.getSettingObject(CInt(ComboBox1.SelectedIndex))
                        Dim xml_serializer As New System.Xml.Serialization.XmlSerializer(GetType(Setting))
                        Dim string_reader As New System.IO.MemoryStream(System.IO.File.ReadAllBytes(fp))
                        Dim s As Setting = DirectCast(xml_serializer.Deserialize(string_reader), Setting)
                        s.presetIndex = CInt(ComboBox1.SelectedIndex)
                        loadSetting(s)
                        refreshPicturebox(s)
                End Select
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        End
    End Sub

    Private Sub TextBoxCountdown_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCountdown.TextChanged
        Try
            If IsNumeric(TextBoxCountdown.Text) Then
                If Not ownerForm Is Nothing Then
                    ownerForm.countdownMax = CInt(TextBoxCountdown.Text)
                End If
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub

    Private Sub TextBoxCountdownText_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCountdownText.TextChanged
        Try
            If Not String.IsNullOrEmpty(TextBoxCountdownText.Text) Then
                If Not ownerForm Is Nothing Then
                    ownerForm.countdownText = CStr(TextBoxCountdownText.Text & "")
                End If
            End If
        Catch ex As Exception
            Err.Clear()
        End Try
    End Sub
End Class
