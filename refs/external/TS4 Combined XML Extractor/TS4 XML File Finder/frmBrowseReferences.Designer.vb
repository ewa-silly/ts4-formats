<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBrowseReferences
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBrowseReferences))
        Me.lblXmlName = New System.Windows.Forms.Label()
        Me.lvReferBy = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lvReferTo = New System.Windows.Forms.ListView()
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.mnuContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuBrowseReferences = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuShowInSearchWindow = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCopyToFolder = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuLaunchExplorer = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuLaunchEditor = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuContextMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblXmlName
        '
        Me.lblXmlName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblXmlName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblXmlName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblXmlName.Location = New System.Drawing.Point(12, 9)
        Me.lblXmlName.Name = "lblXmlName"
        Me.lblXmlName.Size = New System.Drawing.Size(473, 24)
        Me.lblXmlName.TabIndex = 1
        Me.lblXmlName.Text = "lblXmlName"
        Me.lblXmlName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lvReferBy
        '
        Me.lvReferBy.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lvReferBy.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.lvReferBy.Location = New System.Drawing.Point(12, 37)
        Me.lvReferBy.MultiSelect = False
        Me.lvReferBy.Name = "lvReferBy"
        Me.lvReferBy.Size = New System.Drawing.Size(230, 282)
        Me.lvReferBy.TabIndex = 2
        Me.lvReferBy.UseCompatibleStateImageBehavior = False
        Me.lvReferBy.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "References From"
        Me.ColumnHeader1.Width = 186
        '
        'lvReferTo
        '
        Me.lvReferTo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lvReferTo.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader2})
        Me.lvReferTo.Location = New System.Drawing.Point(255, 37)
        Me.lvReferTo.MultiSelect = False
        Me.lvReferTo.Name = "lvReferTo"
        Me.lvReferTo.Size = New System.Drawing.Size(230, 282)
        Me.lvReferTo.TabIndex = 3
        Me.lvReferTo.UseCompatibleStateImageBehavior = False
        Me.lvReferTo.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "References To"
        Me.ColumnHeader2.Width = 186
        '
        'mnuContextMenu
        '
        Me.mnuContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuBrowseReferences, Me.mnuShowInSearchWindow, Me.mnuCopyToFolder, Me.mnuLaunchExplorer, Me.mnuLaunchEditor})
        Me.mnuContextMenu.Name = "mnuContextMenu"
        Me.mnuContextMenu.Size = New System.Drawing.Size(201, 114)
        '
        'mnuBrowseReferences
        '
        Me.mnuBrowseReferences.Name = "mnuBrowseReferences"
        Me.mnuBrowseReferences.Size = New System.Drawing.Size(200, 22)
        Me.mnuBrowseReferences.Text = "Browse References"
        '
        'mnuShowInSearchWindow
        '
        Me.mnuShowInSearchWindow.Name = "mnuShowInSearchWindow"
        Me.mnuShowInSearchWindow.Size = New System.Drawing.Size(200, 22)
        Me.mnuShowInSearchWindow.Text = "Show in Search Window"
        '
        'mnuCopyToFolder
        '
        Me.mnuCopyToFolder.Name = "mnuCopyToFolder"
        Me.mnuCopyToFolder.Size = New System.Drawing.Size(200, 22)
        Me.mnuCopyToFolder.Text = "Copy to Folder"
        '
        'mnuLaunchExplorer
        '
        Me.mnuLaunchExplorer.Name = "mnuLaunchExplorer"
        Me.mnuLaunchExplorer.Size = New System.Drawing.Size(200, 22)
        Me.mnuLaunchExplorer.Text = "Show in Windows Explorer"
        '
        'mnuLaunchEditor
        '
        Me.mnuLaunchEditor.Name = "mnuLaunchEditor"
        Me.mnuLaunchEditor.Size = New System.Drawing.Size(200, 22)
        Me.mnuLaunchEditor.Text = "Open in XML Editor"
        '
        'frmBrowseReferences
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(499, 328)
        Me.Controls.Add(Me.lvReferTo)
        Me.Controls.Add(Me.lvReferBy)
        Me.Controls.Add(Me.lblXmlName)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmBrowseReferences"
        Me.Text = "XML Cross-Reference Browser"
        Me.mnuContextMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblXmlName As Label
    Friend WithEvents lvReferBy As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents lvReferTo As ListView
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents mnuContextMenu As ContextMenuStrip
    Friend WithEvents mnuShowInSearchWindow As ToolStripMenuItem
    Friend WithEvents mnuCopyToFolder As ToolStripMenuItem
    Friend WithEvents mnuLaunchExplorer As ToolStripMenuItem
    Friend WithEvents mnuLaunchEditor As ToolStripMenuItem
    Friend WithEvents mnuBrowseReferences As ToolStripMenuItem
End Class
