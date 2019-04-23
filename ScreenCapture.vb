Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Drawing.Graphics
'var image = ScreenCapture.CaptureActiveWindow();
'image.Save(@"C:\temp\snippetsource.jpg", ImageFormat.Jpeg);
'var image = ScreenCapture.CaptureDesktop();
'image.Save(@"C:\temp\snippetsource.jpg", ImageFormat.Jpeg);
Public Class ScreenCapture
    'DLL import stuff
    <DllImport("user32.dll")>
    Private Shared Function InvalidateRect(hWnd As IntPtr, rect As IntPtr, clear As Boolean) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function UpdateWindow(hWnd As IntPtr) As Boolean
    End Function
    'Core refresh function, which calls dll-imported-functions
    Public Shared Sub RefreshDesktop()
        InvalidateRect(IntPtr.Zero, IntPtr.Zero, True)
        UpdateWindow(IntPtr.Zero)
    End Sub
    <DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)>
    Public Shared Function GetDesktopWindow() As IntPtr
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Rect
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef rect As Rect) As IntPtr
    End Function
    Public Shared Function CaptureDesktop() As Image
        Return CaptureWindow(GetDesktopWindow())
    End Function
    Public Shared Function CaptureActiveWindow() As Bitmap
        Return CaptureWindow(GetForegroundWindow())
    End Function
    Public Shared Function CaptureWindow(ByVal handle As IntPtr) As Bitmap
        Dim rect = New Rect()
        GetWindowRect(handle, rect)

        Dim bounds = New System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top)
        Dim result = New System.Drawing.Bitmap(bounds.Width, bounds.Height)

        Using graphics = System.Drawing.Graphics.FromImage(result)
            graphics.CopyFromScreen(New System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size)
        End Using
        Return result
    End Function
End Class
