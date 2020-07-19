<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgOverwrite
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblFolder = New System.Windows.Forms.Label()
        Me.lblFilename = New System.Windows.Forms.Label()
        Me.btnOverwrite = New System.Windows.Forms.Button()
        Me.btnRename = New System.Windows.Forms.Button()
        Me.cbAlways = New System.Windows.Forms.CheckBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.pbIcon = New System.Windows.Forms.PictureBox()
        Me.Panel1.SuspendLayout()
        CType(Me.pbIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(66, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(250, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "File already exists, do you wish to overwrite this file?"
        '
        'lblFolder
        '
        Me.lblFolder.AutoSize = True
        Me.lblFolder.Location = New System.Drawing.Point(66, 35)
        Me.lblFolder.Name = "lblFolder"
        Me.lblFolder.Size = New System.Drawing.Size(42, 13)
        Me.lblFolder.TabIndex = 1
        Me.lblFolder.Text = "Folder: "
        '
        'lblFilename
        '
        Me.lblFilename.AutoSize = True
        Me.lblFilename.Location = New System.Drawing.Point(66, 48)
        Me.lblFilename.Name = "lblFilename"
        Me.lblFilename.Size = New System.Drawing.Size(55, 13)
        Me.lblFilename.TabIndex = 2
        Me.lblFilename.Text = "Filename: "
        '
        'btnOverwrite
        '
        Me.btnOverwrite.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOverwrite.Location = New System.Drawing.Point(0, 4)
        Me.btnOverwrite.Name = "btnOverwrite"
        Me.btnOverwrite.Size = New System.Drawing.Size(102, 23)
        Me.btnOverwrite.TabIndex = 3
        Me.btnOverwrite.Text = "Overwrite File"
        Me.btnOverwrite.UseVisualStyleBackColor = True
        '
        'btnRename
        '
        Me.btnRename.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnRename.Location = New System.Drawing.Point(108, 4)
        Me.btnRename.Name = "btnRename"
        Me.btnRename.Size = New System.Drawing.Size(102, 23)
        Me.btnRename.TabIndex = 4
        Me.btnRename.Text = "Rename Original"
        Me.btnRename.UseVisualStyleBackColor = True
        '
        'cbAlways
        '
        Me.cbAlways.AutoSize = True
        Me.cbAlways.Location = New System.Drawing.Point(69, 71)
        Me.cbAlways.Name = "cbAlways"
        Me.cbAlways.Size = New System.Drawing.Size(101, 17)
        Me.cbAlways.TabIndex = 5
        Me.cbAlways.Text = "Stop Asking Me"
        Me.cbAlways.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Controls.Add(Me.btnRename)
        Me.Panel1.Controls.Add(Me.btnOverwrite)
        Me.Panel1.Location = New System.Drawing.Point(127, 94)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(217, 30)
        Me.Panel1.TabIndex = 6
        '
        'pbIcon
        '
        Me.pbIcon.Location = New System.Drawing.Point(12, 12)
        Me.pbIcon.Name = "pbIcon"
        Me.pbIcon.Size = New System.Drawing.Size(48, 48)
        Me.pbIcon.TabIndex = 7
        Me.pbIcon.TabStop = False
        '
        'dlgOverwrite
        '
        Me.AcceptButton = Me.btnOverwrite
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.btnRename
        Me.ClientSize = New System.Drawing.Size(356, 136)
        Me.Controls.Add(Me.pbIcon)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cbAlways)
        Me.Controls.Add(Me.lblFilename)
        Me.Controls.Add(Me.lblFolder)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgOverwrite"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Overwrite Existing File?"
        Me.Panel1.ResumeLayout(False)
        CType(Me.pbIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblFolder As System.Windows.Forms.Label
    Friend WithEvents lblFilename As System.Windows.Forms.Label
    Friend WithEvents btnOverwrite As System.Windows.Forms.Button
    Friend WithEvents btnRename As System.Windows.Forms.Button
    Friend WithEvents cbAlways As System.Windows.Forms.CheckBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents pbIcon As System.Windows.Forms.PictureBox
End Class
