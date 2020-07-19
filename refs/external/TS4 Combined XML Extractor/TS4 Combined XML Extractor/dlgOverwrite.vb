Public Class dlgOverwrite
    Private _filename As String
    Private _extension As String
    Private _folder As String

    Public Sub New(filename As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _filename = Path.GetFileNameWithoutExtension(filename)
        _extension = Path.GetExtension(filename)
        _folder = Path.GetDirectoryName(filename)
        If String.IsNullOrEmpty(_folder) Then
            _folder = My.Computer.FileSystem.CurrentDirectory
        End If
        lblFolder.Text = "Folder: " & _folder
        lblFilename.Text = "Filename: " & _filename & _extension
        pbIcon.Image = SystemIcons.Question.ToBitmap
    End Sub

    Public ReadOnly Property AlwaysChecked As Boolean
        Get
            Return cbAlways.Checked
        End Get
    End Property

    Public Sub RenameFile()
        Dim num As Integer = 1

        While num > 0
            Try
                My.Computer.FileSystem.RenameFile(Path.Combine(_folder, _filename & _extension), _filename & " (Copy " & num & ")" & _extension)
                num = -1
            Catch ex As Exception
                num = num + 1
            End Try
        End While
    End Sub

    Private Sub btnRename_Click(sender As Object, e As EventArgs) Handles btnRename.Click
        RenameFile()
    End Sub
End Class