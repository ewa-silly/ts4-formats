Imports System.ComponentModel

Public Class frmMain
    Private strFileEntry() As String
    Private strGroups() As String
    Private boolCtrlKey As Boolean = False
    Private boolShiftKey As Boolean = False
    Friend strBaseFolder As String
    Friend InstanceXRefTable As clsInstanceXRefTable = Nothing

    Declare Function AddClipboardFormatListener Lib "user32.dll" (hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    Declare Function RemoveClipboardFormatListener Lib "user32.dll" (hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    Private Const WM_CLIPBOARDUPDATE As Int32 = &H31D

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.SettingsNeedUpgrade Then
            My.Settings.Upgrade()
            My.Settings.SettingsNeedUpgrade = False
        End If
        If My.Computer.FileSystem.FileExists(My.Settings.InputFilePath) Then
            txtIndexFilename.Text = My.Settings.InputFilePath
            strBaseFolder = Path.GetDirectoryName(txtIndexFilename.Text) & "\"
            If My.Computer.FileSystem.FileExists(strBaseFolder & "XML Cross Reference.dat") Then
                InstanceXRefTable = New clsInstanceXRefTable(strBaseFolder)
            End If
            LoadIndexFile()
        Else
            txtIndexFilename.Text = ""
        End If
        If My.Settings.GroupItems Then
            cbGroupItems.Checked = True
        End If
        Me.Text = "XML File Finder for The Sims 4 - v" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
        If toolstripStatusbar.Text = "" Then
            toolstripStatusbar.Text = "XML File Finder for The Sims 4 -- by Scumbumbo @ Mod The Sims"
        End If
        txtSearchString.Focus()
    End Sub

    Private Sub CloseAllReferenceForms()
        Dim listForms As New List(Of Form)

        For Each frm In My.Application.OpenForms
            If TypeOf frm Is frmBrowseReferences Then
                listForms.Add(frm)
            End If
        Next
        For Each frm In listForms
            frm.Close()
        Next
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim dlgResult As Windows.Forms.DialogResult

        dlgFileBrowser.Filter = "Text Index File (*.txt)|*.txt|All Files (*.*)|*.*"
        dlgFileBrowser.Multiselect = False
        If Not String.IsNullOrEmpty(txtIndexFilename.Text) Then
            dlgFileBrowser.InitialDirectory = Path.GetDirectoryName(txtIndexFilename.Text)
        End If
        dlgFileBrowser.FileName = txtIndexFilename.Text

        dlgResult = dlgFileBrowser.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        txtIndexFilename.Text = dlgFileBrowser.FileName
        My.Settings.InputFilePath = txtIndexFilename.Text

        strBaseFolder = Path.GetDirectoryName(txtIndexFilename.Text) & "\"
        InstanceXRefTable = Nothing
        If My.Computer.FileSystem.FileExists(strBaseFolder & "XML Cross Reference.dat") Then
            InstanceXRefTable = New clsInstanceXRefTable(strBaseFolder)
        End If

        CloseAllReferenceForms()

        LoadIndexFile()
    End Sub

    Private Sub AddGroup(strGroup As String)
        If Not strGroups.Contains(strGroup) Then
            Array.Resize(strGroups, strGroups.Length + 1)
            strGroups(strGroups.Length - 1) = strGroup
        End If
    End Sub

    Private Sub LoadIndexFile()
        Dim streamFileIndex As New StreamReader(txtIndexFilename.Text)
        Dim strLine As String
        Dim strParse() As String
        Dim idx As Integer = 0
        Dim firstEntry As Boolean = True
        Dim strStrip As String = String.Empty

        ReDim strFileEntry(-1)
        ReDim strGroups(-1)
        lvSearchResults.Items.Clear()
        txtSearchString.Text = ""
        strLine = streamFileIndex.ReadLine()
        Do
            If idx = strFileEntry.Count Then
                Array.Resize(strFileEntry, idx + 1000)
            End If
            strParse = strLine.Split(vbTab)
            If firstEntry Then
                If Not strParse(4).StartsWith("\BG\") Then
                    Dim basePos As Integer = strParse(4).IndexOf("\BG\")
                    strStrip = strParse(4).Substring(0, basePos + 1)
                End If
                firstEntry = False
            End If
            If Not String.IsNullOrEmpty(strStrip) Then
                strLine = strLine.Replace(strStrip, "")
            End If
            strFileEntry(idx) = strLine
            idx += 1
            AddGroup(strParse(3))
            If InstanceXRefTable IsNot Nothing AndAlso InstanceXRefTable.IsValid Then
                Dim Instance As UInt64
                If UInt64.TryParse(strParse(0), Globalization.NumberStyles.Integer, Globalization.CultureInfo.CurrentCulture, Instance) Then
                    Dim InstanceXRef As clsInstanceXRef = InstanceXRefTable.GetInstanceXRef(Instance)
                    If InstanceXRef IsNot Nothing Then
                        InstanceXRef.Name = strParse(1)
                        InstanceXRef.Filename = strParse(4)
                    End If
                End If
            End If
            strLine = streamFileIndex.ReadLine()
        Loop Until strLine Is Nothing
        Array.Resize(strFileEntry, idx)

        streamFileIndex.Dispose()

        Array.Sort(strGroups)

        txtSearchString.Enabled = True
        toolstripStatusbar.Text = "XML file index loaded"
        txtSearchString.Focus()
    End Sub

    Private Sub txtSearchString_TextChanged(sender As Object, e As EventArgs) Handles txtSearchString.TextChanged
        timerTriggerSearch.Stop()
        timerTriggerSearch.Interval = 500
        timerTriggerSearch.Start()
    End Sub

    Friend Sub PopulateSearchResults(strSearchStrings() As String)
        Dim strResults() As String
        Dim strSortResults(-1) As String
        Dim strMatch As String
        Dim strParse() As String
        Dim I As Integer

        Me.Cursor = Cursors.WaitCursor

        strResults = Array.FindAll(strFileEntry, Function(s) s.ToLower().Contains(strSearchStrings(0)))
        If strSearchStrings.Length > 1 Then
            For I = 1 To strSearchStrings.Length - 1
                strResults = Array.FindAll(strResults, Function(s) s.ToLower().Contains(strSearchStrings(I)))
            Next
        End If
        lvSearchResults.Items.Clear()

        If strResults.Count > 1000 Then
            toolstripStatusbar.Text = strResults.Length & " matches, showing 1000"
            ReDim Preserve strResults(999)
        Else
            toolstripStatusbar.Text = strResults.Length & " matches"
        End If

        For Each strMatch In strResults
            strParse = strMatch.Split(vbTab)
            Array.Resize(strSortResults, strSortResults.Length + 1)
            strSortResults(strSortResults.Length - 1) = strParse(1) & vbTab & strParse(3) & vbTab & strParse(0) & vbTab & strParse(4)
        Next
        Array.Sort(strSortResults)

        lvSearchResults.Groups.Clear()
        If cbGroupItems.Checked Then
            For Each strGroup In strGroups
                lvSearchResults.Groups.Add(New ListViewGroup(strGroup, strGroup))
            Next
        End If
        For Each strMatch In strSortResults
            strParse = strMatch.Split(vbTab)
            Dim lvItem As New ListViewItem(strParse(0))
            lvItem.SubItems.Add(strParse(2))
            lvItem.SubItems.Add(strParse(3))
            If cbGroupItems.Checked Then
                lvItem.Group = lvSearchResults.Groups.Item(strParse(1))
            End If
            If Not My.Computer.FileSystem.FileExists(Path.Combine(strBaseFolder, strParse(3))) Then
                lvItem.Tag = "NOFILE"
                lvItem.Font = New Font(lvItem.Font, lvItem.Font.Style & FontStyle.Strikeout)
            End If
            lvSearchResults.Items.Add(lvItem)
        Next

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub timerTriggerSearch_Tick(sender As Object, e As EventArgs) Handles timerTriggerSearch.Tick
        Dim strSearchStrings() As String

        timerTriggerSearch.Stop()
        If txtSearchString.Text.Length = 0 Then
            Exit Sub
        End If
        strSearchStrings = txtSearchString.Text.ToLower.Split(" ")
        PopulateSearchResults(strSearchStrings)
    End Sub

    Private Sub lvSearchResults_MouseDown(sender As Object, e As MouseEventArgs) Handles lvSearchResults.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If lvSearchResults.GetItemAt(e.X, e.Y) IsNot Nothing Then
                If Not (My.Computer.Keyboard.CtrlKeyDown Or My.Computer.Keyboard.ShiftKeyDown) And lvSearchResults.GetItemAt(e.X, e.Y).Selected = False Then
                    lvSearchResults.SelectedItems.Clear()
                End If
                If lvSearchResults.GetItemAt(e.X, e.Y).Tag <> "NOFILE" Then
                    lvSearchResults.GetItemAt(e.X, e.Y).Selected = True
                    mnuBrowseReferences.Enabled = False
                    If InstanceXRefTable IsNot Nothing AndAlso InstanceXRefTable.IsValid Then
                        If lvSearchResults.SelectedItems.Count = 1 Then
                            Dim Instance As UInt64
                            If UInt64.TryParse(lvSearchResults.SelectedItems.Item(0).SubItems.Item(1).Text, Globalization.NumberStyles.Integer, Globalization.CultureInfo.CurrentCulture, Instance) Then
                                If InstanceXRefTable.GetInstanceXRef(Instance) IsNot Nothing Then
                                    mnuBrowseReferences.Enabled = True
                                End If
                            End If
                        End If
                    End If
                    mnuContextMenu.Show(lvSearchResults, New Point(e.X, e.Y))
                End If
            End If
        End If
    End Sub

    Private Sub lvSearchResults_Resize(sender As Object, e As EventArgs) Handles lvSearchResults.Resize
        lvSearchResults.Columns(1).Width = -2
    End Sub

    Private Sub mnuLaunchExplorer_Click(sender As Object, e As EventArgs) Handles mnuLaunchExplorer.Click
        For Each item As ListViewItem In lvSearchResults.SelectedItems
            Process.Start("explorer.exe", "/select,""" & strBaseFolder & item.SubItems(2).Text & """")
        Next
    End Sub

    Private Sub mnuLaunchEditor_Click(sender As Object, e As EventArgs) Handles mnuLaunchEditor.Click
        For Each item As ListViewItem In lvSearchResults.SelectedItems
            Process.Start(strBaseFolder & item.SubItems(2).Text)
        Next
    End Sub

    Private Sub cbGroupItems_CheckedChanged(sender As Object, e As EventArgs) Handles cbGroupItems.CheckedChanged
        My.Settings.GroupItems = cbGroupItems.Checked
    End Sub

    Private Sub lvSearchResults_DoubleClick(sender As Object, e As EventArgs) Handles lvSearchResults.DoubleClick
        For Each item As ListViewItem In lvSearchResults.SelectedItems
            Process.Start(strBaseFolder & item.SubItems(2).Text)
        Next
    End Sub

    Private Sub lvSearchResults_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvSearchResults.SelectedIndexChanged
        For Each item As ListViewItem In lvSearchResults.SelectedItems
            If item.Tag = "NOFILE" Then
                item.Selected = False
            End If
        Next
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

        For Each item As ListViewItem In lvSearchResults.SelectedItems
            Dim SourceFile As String = strBaseFolder & item.SubItems(2).Text
            Dim DestinationFile As String = Path.Combine(DestinationFolder, Path.GetFileName(SourceFile))
            FileCopy(SourceFile, DestinationFile)
        Next
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)

        If m.Msg = WM_CLIPBOARDUPDATE Then
            If My.Computer.Clipboard.ContainsText Then
                txtSearchString.Text = My.Computer.Clipboard.GetText
            End If
        End If
    End Sub

    Private Sub cbClipboardMonitor_CheckedChanged(sender As Object, e As EventArgs) Handles cbClipboardMonitor.CheckedChanged
        If cbClipboardMonitor.Checked Then
            AddClipboardFormatListener(Me.Handle)
        Else
            RemoveClipboardFormatListener(Me.Handle)
        End If
    End Sub

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If cbClipboardMonitor.Checked Then
            RemoveClipboardFormatListener(Me.Handle)
        End If
    End Sub

    Private Sub mnuBrowseReferences_Click(sender As Object, e As EventArgs) Handles mnuBrowseReferences.Click
        Dim Instance As UInt64
        Dim InstanceXRef As clsInstanceXRef
        Dim newForm As frmBrowseReferences

        If Not UInt64.TryParse(lvSearchResults.SelectedItems.Item(0).SubItems.Item(1).Text, Globalization.NumberStyles.Integer, Globalization.CultureInfo.CurrentCulture, Instance) Then
            ' Should never happen, we checked this when user right-clicked
            Exit Sub
        End If

        InstanceXRef = InstanceXRefTable.GetInstanceXRef(Instance)

        newForm = New frmBrowseReferences(Me)
        newForm.Show()

        newForm.BrowseTo(InstanceXRef)
    End Sub

    Private _WindowStateMinimized As Boolean = False
    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            For Each frm As Form In My.Application.OpenForms
                If frm.WindowState <> FormWindowState.Minimized Then
                    frm.WindowState = FormWindowState.Minimized
                End If
            Next
            _WindowStateMinimized = True
        ElseIf Me.WindowState = FormWindowState.Normal AndAlso _WindowStateMinimized Then
            For Each frm As Form In My.Application.OpenForms
                If frm.WindowState <> FormWindowState.Normal Then
                    frm.WindowState = FormWindowState.Normal
                End If
            Next
            _WindowStateMinimized = False
        End If
    End Sub
End Class
