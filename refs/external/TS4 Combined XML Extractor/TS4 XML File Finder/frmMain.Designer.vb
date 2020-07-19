<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.txtIndexFilename = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.dlgFileBrowser = New System.Windows.Forms.OpenFileDialog()
        Me.txtSearchString = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.timerTriggerSearch = New System.Windows.Forms.Timer(Me.components)
        Me.lvSearchResults = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.stripStatusbar = New System.Windows.Forms.StatusStrip()
        Me.toolstripStatusbar = New System.Windows.Forms.ToolStripStatusLabel()
        Me.mnuContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuBrowseReferences = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCopyToFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuLaunchExplorer = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuLaunchEditor = New System.Windows.Forms.ToolStripMenuItem()
        Me.cbGroupItems = New System.Windows.Forms.CheckBox()
        Me.cbClipboardMonitor = New System.Windows.Forms.CheckBox()
        Me.GroupBox1.SuspendLayout()
        Me.stripStatusbar.SuspendLayout()
        Me.mnuContextMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtIndexFilename
        '
        Me.txtIndexFilename.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIndexFilename.Enabled = False
        Me.txtIndexFilename.Location = New System.Drawing.Point(6, 19)
        Me.txtIndexFilename.Name = "txtIndexFilename"
        Me.txtIndexFilename.Size = New System.Drawing.Size(574, 20)
        Me.txtIndexFilename.TabIndex = 5
        '
        'btnBrowse
        '
        Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowse.Location = New System.Drawing.Point(586, 17)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 4
        Me.btnBrowse.Text = "Browse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.txtIndexFilename)
        Me.GroupBox1.Controls.Add(Me.btnBrowse)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 13)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(667, 48)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Load XML Extractor Index File"
        '
        'dlgFileBrowser
        '
        Me.dlgFileBrowser.FileName = "OpenFileDialog1"
        '
        'txtSearchString
        '
        Me.txtSearchString.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSearchString.Enabled = False
        Me.txtSearchString.Location = New System.Drawing.Point(78, 76)
        Me.txtSearchString.Name = "txtSearchString"
        Me.txtSearchString.Size = New System.Drawing.Size(601, 20)
        Me.txtSearchString.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 79)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Search For:"
        '
        'timerTriggerSearch
        '
        Me.timerTriggerSearch.Interval = 250
        '
        'lvSearchResults
        '
        Me.lvSearchResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvSearchResults.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.lvSearchResults.FullRowSelect = True
        Me.lvSearchResults.GridLines = True
        Me.lvSearchResults.HideSelection = False
        Me.lvSearchResults.Location = New System.Drawing.Point(12, 125)
        Me.lvSearchResults.Name = "lvSearchResults"
        Me.lvSearchResults.Size = New System.Drawing.Size(667, 251)
        Me.lvSearchResults.TabIndex = 2
        Me.lvSearchResults.UseCompatibleStateImageBehavior = False
        Me.lvSearchResults.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Name"
        Me.ColumnHeader1.Width = 176
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Instance"
        Me.ColumnHeader2.Width = 96
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Filename"
        Me.ColumnHeader3.Width = 389
        '
        'stripStatusbar
        '
        Me.stripStatusbar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolstripStatusbar})
        Me.stripStatusbar.Location = New System.Drawing.Point(0, 382)
        Me.stripStatusbar.Name = "stripStatusbar"
        Me.stripStatusbar.Size = New System.Drawing.Size(691, 22)
        Me.stripStatusbar.TabIndex = 8
        Me.stripStatusbar.Text = "StatusStrip1"
        '
        'toolstripStatusbar
        '
        Me.toolstripStatusbar.Name = "toolstripStatusbar"
        Me.toolstripStatusbar.Size = New System.Drawing.Size(0, 17)
        '
        'mnuContextMenu
        '
        Me.mnuContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBrowseReferences, Me.mnuCopyToFolder, Me.mnuLaunchExplorer, Me.mnuLaunchEditor})
        Me.mnuContextMenu.Name = "mnuContextMenu"
        Me.mnuContextMenu.ShowImageMargin = False
        Me.mnuContextMenu.Size = New System.Drawing.Size(176, 114)
        '
        'mnuBrowseReferences
        '
        Me.mnuBrowseReferences.Name = "mnuBrowseReferences"
        Me.mnuBrowseReferences.Size = New System.Drawing.Size(175, 22)
        Me.mnuBrowseReferences.Text = "Browse References"
        '
        'mnuCopyToFolder
        '
        Me.mnuCopyToFolder.Name = "mnuCopyToFolder"
        Me.mnuCopyToFolder.Size = New System.Drawing.Size(175, 22)
        Me.mnuCopyToFolder.Text = "Copy to Folder"
        '
        'mnuLaunchExplorer
        '
        Me.mnuLaunchExplorer.Name = "mnuLaunchExplorer"
        Me.mnuLaunchExplorer.Size = New System.Drawing.Size(175, 22)
        Me.mnuLaunchExplorer.Text = "Show in Windows Explorer"
        '
        'mnuLaunchEditor
        '
        Me.mnuLaunchEditor.Name = "mnuLaunchEditor"
        Me.mnuLaunchEditor.Size = New System.Drawing.Size(175, 22)
        Me.mnuLaunchEditor.Text = "Open in XML Editor"
        '
        'cbGroupItems
        '
        Me.cbGroupItems.AutoSize = True
        Me.cbGroupItems.Location = New System.Drawing.Point(78, 102)
        Me.cbGroupItems.Name = "cbGroupItems"
        Me.cbGroupItems.Size = New System.Drawing.Size(347, 17)
        Me.cbGroupItems.TabIndex = 3
        Me.cbGroupItems.Text = "Group Items by Resource Type (Setting takes effect for next search)"
        Me.cbGroupItems.UseVisualStyleBackColor = True
        '
        'cbClipboardMonitor
        '
        Me.cbClipboardMonitor.AutoSize = True
        Me.cbClipboardMonitor.Location = New System.Drawing.Point(484, 102)
        Me.cbClipboardMonitor.Name = "cbClipboardMonitor"
        Me.cbClipboardMonitor.Size = New System.Drawing.Size(108, 17)
        Me.cbClipboardMonitor.TabIndex = 9
        Me.cbClipboardMonitor.Text = "Clipboard Monitor"
        Me.cbClipboardMonitor.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(691, 404)
        Me.Controls.Add(Me.cbClipboardMonitor)
        Me.Controls.Add(Me.cbGroupItems)
        Me.Controls.Add(Me.stripStatusbar)
        Me.Controls.Add(Me.lvSearchResults)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtSearchString)
        Me.Controls.Add(Me.GroupBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "XML File Finder for The Sims 4"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.stripStatusbar.ResumeLayout(False)
        Me.stripStatusbar.PerformLayout()
        Me.mnuContextMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtIndexFilename As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents dlgFileBrowser As System.Windows.Forms.OpenFileDialog
    Friend WithEvents txtSearchString As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents timerTriggerSearch As System.Windows.Forms.Timer
    Friend WithEvents lvSearchResults As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents stripStatusbar As System.Windows.Forms.StatusStrip
    Friend WithEvents toolstripStatusbar As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents mnuContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuLaunchExplorer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuLaunchEditor As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cbGroupItems As System.Windows.Forms.CheckBox
    Friend WithEvents mnuCopyToFolder As ToolStripMenuItem
    Friend WithEvents cbClipboardMonitor As CheckBox
    Friend WithEvents mnuBrowseReferences As ToolStripMenuItem
    Friend WithEvents ColumnHeader3 As ColumnHeader
End Class
