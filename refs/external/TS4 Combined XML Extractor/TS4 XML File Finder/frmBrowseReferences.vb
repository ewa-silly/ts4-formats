Public Class frmBrowseReferences
    Private _frmMain As frmMain
    Private _Target As clsInstanceXRef

    Public Sub New(Caller As Form)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _frmMain = Caller
    End Sub

    Friend Sub BrowseTo(InstanceXRef As clsInstanceXRef)
        lblXmlName.Text = InstanceXRef.Name
        lvReferBy.Items.Clear()
        lvReferBy.SelectedItems.Clear()
        lvReferBy.Sorting = SortOrder.None
        lvReferTo.Items.Clear()
        lvReferTo.SelectedItems.Clear()
        lvReferTo.Sorting = SortOrder.None

        For Each Instance As UInt64 In InstanceXRef._ReferenceBy
            Dim refByXRef As clsInstanceXRef = _frmMain.InstanceXRefTable.GetInstanceXRef(Instance)
            Dim lvItem As New ListViewItem(refByXRef.Name)
            lvItem.Tag = refByXRef
            lvReferBy.Items.Add(lvItem)
        Next
        For Each Instance As UInt64 In InstanceXRef._ReferenceTo
            Dim refToXRef As clsInstanceXRef = _frmMain.InstanceXRefTable.GetInstanceXRef(Instance)
            Dim lvItem As New ListViewItem(refToXRef.Name)
            lvItem.Tag = refToXRef
            lvReferTo.Items.Add(lvItem)
        Next
        lvReferTo.Columns.Item(0).Width = -1
        lvReferTo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
        lvReferTo.Sorting = SortOrder.Ascending
        lvReferBy.Columns.Item(0).Width = -1
        lvReferBy.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
        lvReferBy.Sorting = SortOrder.Ascending
    End Sub

    Private Sub frmBrowseReferences_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim mid As Integer = Me.Width / 2
        lvReferBy.Width = mid - lvReferBy.Left - 12
        lvReferTo.Width = lvReferBy.Width
        lvReferTo.Left = lvReferTo.Width + 25
    End Sub

    Private Sub EnableMenuItems()
        If My.Computer.FileSystem.FileExists(Path.Combine(_frmMain.strBaseFolder, _Target.Filename)) Then
            mnuCopyToFolder.Enabled = True
            mnuLaunchExplorer.Enabled = True
            mnuLaunchEditor.Enabled = True
        Else
            mnuCopyToFolder.Enabled = False
            mnuLaunchExplorer.Enabled = False
            mnuLaunchEditor.Enabled = False
        End If
    End Sub

    Private Sub lvReferBy_MouseUp(sender As Object, e As MouseEventArgs) Handles lvReferBy.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If lvReferBy.GetItemAt(e.X, e.Y) IsNot Nothing Then
                lvReferBy.GetItemAt(e.X, e.Y).Selected = True
                _Target = lvReferBy.SelectedItems.Item(0).Tag
                EnableMenuItems()
                mnuContextMenu.Show(lvReferBy, New Point(e.X, e.Y))
            End If
        Else
            If lvReferBy.GetItemAt(e.X, e.Y) IsNot Nothing Then
                lvReferBy.GetItemAt(e.X, e.Y).Selected = True
                BrowseTo(lvReferBy.SelectedItems.Item(0).Tag)
            End If
        End If
    End Sub

    Private Sub lvReferTo_MouseUp(sender As Object, e As MouseEventArgs) Handles lvReferTo.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If lvReferTo.GetItemAt(e.X, e.Y) IsNot Nothing Then
                lvReferTo.GetItemAt(e.X, e.Y).Selected = True
                _Target = lvReferTo.SelectedItems.Item(0).Tag
                EnableMenuItems()
                mnuContextMenu.Show(lvReferTo, New Point(e.X, e.Y))
            End If
        Else
            If lvReferTo.GetItemAt(e.X, e.Y) IsNot Nothing Then
                lvReferTo.GetItemAt(e.X, e.Y).Selected = True
                BrowseTo(lvReferTo.SelectedItems.Item(0).Tag)
            End If
        End If
    End Sub

    Private Sub mnuShowInSearchWindow_Click(sender As Object, e As EventArgs) Handles mnuShowInSearchWindow.Click
        _frmMain.txtSearchString.Text = _Target.Instance.ToString & " " & _Target.Name
        _frmMain.timerTriggerSearch.Stop()
        _frmMain.timerTriggerSearch.Interval = 10
        _frmMain.timerTriggerSearch.Start()
    End Sub

    Private Sub mnuCopyToFolder_Click(sender As Object, e As EventArgs) Handles mnuCopyToFolder.Click
        Dim DestinationFolder As String = My.Settings.LastCopyToFolder
        Dim dialog As New clsFolderBrowser
        Dim dlgResult As Windows.Forms.DialogResult

        If Not My.Computer.FileSystem.DirectoryExists(DestinationFolder) Then
            DestinationFolder = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End If
        dialog.DirectoryPath = DestinationFolder
        dlgResult = dialog.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        DestinationFolder = dialog.DirectoryPath
        My.Settings.LastCopyToFolder = DestinationFolder
        Dim DestinationFile As String = Path.Combine(DestinationFolder, Path.GetFileName(_Target.Filename))
        FileCopy(Path.Combine(_frmMain.strBaseFolder, _Target.Filename), DestinationFile)
    End Sub

    Private Sub mnuLaunchExplorer_Click(sender As Object, e As EventArgs) Handles mnuLaunchExplorer.Click
        Process.Start("explorer.exe", "/select,""" & Path.Combine(_frmMain.strBaseFolder, _Target.Filename) & """")
    End Sub

    Private Sub mnuLaunchEditor_Click(sender As Object, e As EventArgs) Handles mnuLaunchEditor.Click
        Process.Start(Path.Combine(_frmMain.strBaseFolder, _Target.Filename))
    End Sub

    Private Sub mnuBrowseReferences_Click(sender As Object, e As EventArgs) Handles mnuBrowseReferences.Click
        Dim newForm As frmBrowseReferences
        newForm = New frmBrowseReferences(_frmMain)
        newForm.Show()
        newForm.BrowseTo(_Target)
    End Sub
End Class