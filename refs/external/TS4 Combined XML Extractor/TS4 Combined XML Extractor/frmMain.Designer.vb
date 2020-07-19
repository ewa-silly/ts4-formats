<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtInputFile = New System.Windows.Forms.TextBox()
        Me.btnBrowseInputFile = New System.Windows.Forms.Button()
        Me.btnExtract = New System.Windows.Forms.Button()
        Me.dlgFileBrowser = New System.Windows.Forms.OpenFileDialog()
        Me.btnBrowseOutputFolder = New System.Windows.Forms.Button()
        Me.txtOutputFolder = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.statusStrip = New System.Windows.Forms.StatusStrip()
        Me.statusBar = New System.Windows.Forms.ToolStripStatusLabel()
        Me.cbPack = New System.Windows.Forms.ComboBox()
        Me.lblGPValue = New System.Windows.Forms.Label()
        Me.gbFull = New System.Windows.Forms.GroupBox()
        Me.cbIncludeJazz = New System.Windows.Forms.CheckBox()
        Me.cbThreads = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cbIgnoreNoTuning = New System.Windows.Forms.CheckBox()
        Me.cbCreateFileIndex = New System.Windows.Forms.CheckBox()
        Me.cbLanguage = New System.Windows.Forms.ComboBox()
        Me.cbCreateRefFiles = New System.Windows.Forms.CheckBox()
        Me.cbIncludeXmlNames = New System.Windows.Forms.CheckBox()
        Me.cbIncludeStrings = New System.Windows.Forms.CheckBox()
        Me.btnBrowseGameFolder = New System.Windows.Forms.Button()
        Me.txtGameFolder = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.rbAttr_TN = New System.Windows.Forms.RadioButton()
        Me.rbAttr_NT = New System.Windows.Forms.RadioButton()
        Me.cbNormalizeAttributes = New System.Windows.Forms.CheckBox()
        Me.gbSingle = New System.Windows.Forms.GroupBox()
        Me.cbSingleIncludeJazz = New System.Windows.Forms.CheckBox()
        Me.rbFull = New System.Windows.Forms.RadioButton()
        Me.rbSingle = New System.Windows.Forms.RadioButton()
        Me.gbOutput = New System.Windows.Forms.GroupBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.rbNative = New System.Windows.Forms.RadioButton()
        Me.rbTGIOnly = New System.Windows.Forms.RadioButton()
        Me.rbTGIName = New System.Windows.Forms.RadioButton()
        Me.rbNameOnly = New System.Windows.Forms.RadioButton()
        Me.rbExistingRename = New System.Windows.Forms.RadioButton()
        Me.rbExistingOverwrite = New System.Windows.Forms.RadioButton()
        Me.rbExistingAsk = New System.Windows.Forms.RadioButton()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.rbS4S = New System.Windows.Forms.RadioButton()
        Me.statusStrip.SuspendLayout()
        Me.gbFull.SuspendLayout()
        Me.gbSingle.SuspendLayout()
        Me.gbOutput.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Input File"
        '
        'txtInputFile
        '
        Me.txtInputFile.Location = New System.Drawing.Point(62, 19)
        Me.txtInputFile.Name = "txtInputFile"
        Me.txtInputFile.Size = New System.Drawing.Size(484, 20)
        Me.txtInputFile.TabIndex = 1
        '
        'btnBrowseInputFile
        '
        Me.btnBrowseInputFile.Location = New System.Drawing.Point(552, 16)
        Me.btnBrowseInputFile.Name = "btnBrowseInputFile"
        Me.btnBrowseInputFile.Size = New System.Drawing.Size(61, 23)
        Me.btnBrowseInputFile.TabIndex = 2
        Me.btnBrowseInputFile.Text = "Browse"
        Me.btnBrowseInputFile.UseVisualStyleBackColor = True
        '
        'btnExtract
        '
        Me.btnExtract.Location = New System.Drawing.Point(280, 369)
        Me.btnExtract.Name = "btnExtract"
        Me.btnExtract.Size = New System.Drawing.Size(99, 23)
        Me.btnExtract.TabIndex = 3
        Me.btnExtract.Text = "Begin Extracting"
        Me.btnExtract.UseVisualStyleBackColor = True
        '
        'dlgFileBrowser
        '
        Me.dlgFileBrowser.DefaultExt = "exe"
        Me.dlgFileBrowser.FileName = "OpenFileDialog1"
        '
        'btnBrowseOutputFolder
        '
        Me.btnBrowseOutputFolder.Location = New System.Drawing.Point(572, 15)
        Me.btnBrowseOutputFolder.Name = "btnBrowseOutputFolder"
        Me.btnBrowseOutputFolder.Size = New System.Drawing.Size(61, 23)
        Me.btnBrowseOutputFolder.TabIndex = 6
        Me.btnBrowseOutputFolder.Text = "Browse"
        Me.btnBrowseOutputFolder.UseVisualStyleBackColor = True
        '
        'txtOutputFolder
        '
        Me.txtOutputFolder.Location = New System.Drawing.Point(104, 17)
        Me.txtOutputFolder.Name = "txtOutputFolder"
        Me.txtOutputFolder.Size = New System.Drawing.Size(462, 20)
        Me.txtOutputFolder.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Destination Folder"
        '
        'statusStrip
        '
        Me.statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.statusBar})
        Me.statusStrip.Location = New System.Drawing.Point(0, 400)
        Me.statusStrip.Name = "statusStrip"
        Me.statusStrip.Size = New System.Drawing.Size(663, 22)
        Me.statusStrip.TabIndex = 11
        Me.statusStrip.Text = "StatusStrip1"
        '
        'statusBar
        '
        Me.statusBar.Name = "statusBar"
        Me.statusBar.Size = New System.Drawing.Size(0, 17)
        '
        'cbPack
        '
        Me.cbPack.FormattingEnabled = True
        Me.cbPack.Location = New System.Drawing.Point(62, 45)
        Me.cbPack.Name = "cbPack"
        Me.cbPack.Size = New System.Drawing.Size(168, 21)
        Me.cbPack.TabIndex = 12
        '
        'lblGPValue
        '
        Me.lblGPValue.AutoSize = True
        Me.lblGPValue.Location = New System.Drawing.Point(236, 48)
        Me.lblGPValue.Name = "lblGPValue"
        Me.lblGPValue.Size = New System.Drawing.Size(0, 13)
        Me.lblGPValue.TabIndex = 13
        '
        'gbFull
        '
        Me.gbFull.Controls.Add(Me.cbIncludeJazz)
        Me.gbFull.Controls.Add(Me.cbThreads)
        Me.gbFull.Controls.Add(Me.Label6)
        Me.gbFull.Controls.Add(Me.cbIgnoreNoTuning)
        Me.gbFull.Controls.Add(Me.cbCreateFileIndex)
        Me.gbFull.Controls.Add(Me.cbLanguage)
        Me.gbFull.Controls.Add(Me.cbCreateRefFiles)
        Me.gbFull.Controls.Add(Me.cbIncludeXmlNames)
        Me.gbFull.Controls.Add(Me.cbIncludeStrings)
        Me.gbFull.Controls.Add(Me.btnBrowseGameFolder)
        Me.gbFull.Controls.Add(Me.txtGameFolder)
        Me.gbFull.Controls.Add(Me.Label3)
        Me.gbFull.Location = New System.Drawing.Point(31, 11)
        Me.gbFull.Name = "gbFull"
        Me.gbFull.Size = New System.Drawing.Size(621, 136)
        Me.gbFull.TabIndex = 14
        Me.gbFull.TabStop = False
        Me.gbFull.Text = "Full Automatic Extract (From Game Folder Packages)"
        '
        'cbIncludeJazz
        '
        Me.cbIncludeJazz.AutoSize = True
        Me.cbIncludeJazz.Location = New System.Drawing.Point(332, 44)
        Me.cbIncludeJazz.Name = "cbIncludeJazz"
        Me.cbIncludeJazz.Size = New System.Drawing.Size(144, 17)
        Me.cbIncludeJazz.TabIndex = 13
        Me.cbIncludeJazz.Text = "Include JAZZ Resources"
        Me.cbIncludeJazz.UseVisualStyleBackColor = True
        '
        'cbThreads
        '
        Me.cbThreads.FormattingEnabled = True
        Me.cbThreads.Location = New System.Drawing.Point(433, 86)
        Me.cbThreads.Name = "cbThreads"
        Me.cbThreads.Size = New System.Drawing.Size(43, 21)
        Me.cbThreads.TabIndex = 12
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(329, 91)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(98, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Number of Threads"
        '
        'cbIgnoreNoTuning
        '
        Me.cbIgnoreNoTuning.AutoSize = True
        Me.cbIgnoreNoTuning.Location = New System.Drawing.Point(332, 67)
        Me.cbIgnoreNoTuning.Name = "cbIgnoreNoTuning"
        Me.cbIgnoreNoTuning.Size = New System.Drawing.Size(216, 17)
        Me.cbIgnoreNoTuning.TabIndex = 10
        Me.cbIgnoreNoTuning.Text = "Disable ""No tuning in package"" warning"
        Me.cbIgnoreNoTuning.UseVisualStyleBackColor = True
        '
        'cbCreateFileIndex
        '
        Me.cbCreateFileIndex.AutoSize = True
        Me.cbCreateFileIndex.Location = New System.Drawing.Point(12, 113)
        Me.cbCreateFileIndex.Name = "cbCreateFileIndex"
        Me.cbCreateFileIndex.Size = New System.Drawing.Size(130, 17)
        Me.cbCreateFileIndex.TabIndex = 9
        Me.cbCreateFileIndex.Text = "Create XML File Index"
        Me.cbCreateFileIndex.UseVisualStyleBackColor = True
        '
        'cbLanguage
        '
        Me.cbLanguage.FormattingEnabled = True
        Me.cbLanguage.Location = New System.Drawing.Point(137, 42)
        Me.cbLanguage.Name = "cbLanguage"
        Me.cbLanguage.Size = New System.Drawing.Size(140, 21)
        Me.cbLanguage.TabIndex = 8
        '
        'cbCreateRefFiles
        '
        Me.cbCreateRefFiles.AutoSize = True
        Me.cbCreateRefFiles.Location = New System.Drawing.Point(12, 90)
        Me.cbCreateRefFiles.Name = "cbCreateRefFiles"
        Me.cbCreateRefFiles.Size = New System.Drawing.Size(227, 17)
        Me.cbCreateRefFiles.TabIndex = 7
        Me.cbCreateRefFiles.Text = "Create Reference Files (STBL and Names)"
        Me.cbCreateRefFiles.UseVisualStyleBackColor = True
        '
        'cbIncludeXmlNames
        '
        Me.cbIncludeXmlNames.AutoSize = True
        Me.cbIncludeXmlNames.Location = New System.Drawing.Point(12, 67)
        Me.cbIncludeXmlNames.Name = "cbIncludeXmlNames"
        Me.cbIncludeXmlNames.Size = New System.Drawing.Size(144, 17)
        Me.cbIncludeXmlNames.TabIndex = 5
        Me.cbIncludeXmlNames.Text = "Include XML References"
        Me.cbIncludeXmlNames.UseVisualStyleBackColor = True
        '
        'cbIncludeStrings
        '
        Me.cbIncludeStrings.AutoSize = True
        Me.cbIncludeStrings.Location = New System.Drawing.Point(12, 44)
        Me.cbIncludeStrings.Name = "cbIncludeStrings"
        Me.cbIncludeStrings.Size = New System.Drawing.Size(119, 17)
        Me.cbIncludeStrings.TabIndex = 4
        Me.cbIncludeStrings.Text = "Include Strings from"
        Me.cbIncludeStrings.UseVisualStyleBackColor = True
        '
        'btnBrowseGameFolder
        '
        Me.btnBrowseGameFolder.Location = New System.Drawing.Point(552, 15)
        Me.btnBrowseGameFolder.Name = "btnBrowseGameFolder"
        Me.btnBrowseGameFolder.Size = New System.Drawing.Size(61, 23)
        Me.btnBrowseGameFolder.TabIndex = 3
        Me.btnBrowseGameFolder.Text = "Browse"
        Me.btnBrowseGameFolder.UseVisualStyleBackColor = True
        '
        'txtGameFolder
        '
        Me.txtGameFolder.Location = New System.Drawing.Point(82, 17)
        Me.txtGameFolder.Name = "txtGameFolder"
        Me.txtGameFolder.Size = New System.Drawing.Size(464, 20)
        Me.txtGameFolder.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 20)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(67, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Game Folder"
        '
        'rbAttr_TN
        '
        Me.rbAttr_TN.AutoSize = True
        Me.rbAttr_TN.Enabled = False
        Me.rbAttr_TN.Location = New System.Drawing.Point(111, 3)
        Me.rbAttr_TN.Name = "rbAttr_TN"
        Me.rbAttr_TN.Size = New System.Drawing.Size(92, 17)
        Me.rbAttr_TN.TabIndex = 16
        Me.rbAttr_TN.Text = "t Attribute First"
        Me.rbAttr_TN.UseVisualStyleBackColor = True
        '
        'rbAttr_NT
        '
        Me.rbAttr_NT.AutoSize = True
        Me.rbAttr_NT.Enabled = False
        Me.rbAttr_NT.Location = New System.Drawing.Point(10, 3)
        Me.rbAttr_NT.Name = "rbAttr_NT"
        Me.rbAttr_NT.Size = New System.Drawing.Size(95, 17)
        Me.rbAttr_NT.TabIndex = 15
        Me.rbAttr_NT.Text = "n Attribute First"
        Me.rbAttr_NT.UseVisualStyleBackColor = True
        '
        'cbNormalizeAttributes
        '
        Me.cbNormalizeAttributes.AutoSize = True
        Me.cbNormalizeAttributes.Location = New System.Drawing.Point(9, 88)
        Me.cbNormalizeAttributes.Name = "cbNormalizeAttributes"
        Me.cbNormalizeAttributes.Size = New System.Drawing.Size(223, 17)
        Me.cbNormalizeAttributes.TabIndex = 14
        Me.cbNormalizeAttributes.Text = "Normalize n="".."" and t="".."" Attribute Order"
        Me.cbNormalizeAttributes.UseVisualStyleBackColor = True
        '
        'gbSingle
        '
        Me.gbSingle.Controls.Add(Me.cbSingleIncludeJazz)
        Me.gbSingle.Controls.Add(Me.Label1)
        Me.gbSingle.Controls.Add(Me.txtInputFile)
        Me.gbSingle.Controls.Add(Me.btnBrowseInputFile)
        Me.gbSingle.Controls.Add(Me.cbPack)
        Me.gbSingle.Controls.Add(Me.lblGPValue)
        Me.gbSingle.Location = New System.Drawing.Point(31, 152)
        Me.gbSingle.Name = "gbSingle"
        Me.gbSingle.Size = New System.Drawing.Size(621, 72)
        Me.gbSingle.TabIndex = 15
        Me.gbSingle.TabStop = False
        Me.gbSingle.Text = "Single Combined Tuning File (From Package, Binary or Combined XML File)"
        '
        'cbSingleIncludeJazz
        '
        Me.cbSingleIncludeJazz.AutoSize = True
        Me.cbSingleIncludeJazz.Enabled = False
        Me.cbSingleIncludeJazz.Location = New System.Drawing.Point(332, 47)
        Me.cbSingleIncludeJazz.Name = "cbSingleIncludeJazz"
        Me.cbSingleIncludeJazz.Size = New System.Drawing.Size(213, 17)
        Me.cbSingleIncludeJazz.TabIndex = 14
        Me.cbSingleIncludeJazz.Text = "Include JAZZ Resources from Package"
        Me.cbSingleIncludeJazz.UseVisualStyleBackColor = True
        '
        'rbFull
        '
        Me.rbFull.AutoSize = True
        Me.rbFull.Location = New System.Drawing.Point(11, 11)
        Me.rbFull.Name = "rbFull"
        Me.rbFull.Size = New System.Drawing.Size(14, 13)
        Me.rbFull.TabIndex = 16
        Me.rbFull.UseVisualStyleBackColor = True
        '
        'rbSingle
        '
        Me.rbSingle.AutoSize = True
        Me.rbSingle.Location = New System.Drawing.Point(11, 151)
        Me.rbSingle.Name = "rbSingle"
        Me.rbSingle.Size = New System.Drawing.Size(14, 13)
        Me.rbSingle.TabIndex = 18
        Me.rbSingle.UseVisualStyleBackColor = True
        '
        'gbOutput
        '
        Me.gbOutput.Controls.Add(Me.Panel2)
        Me.gbOutput.Controls.Add(Me.Panel1)
        Me.gbOutput.Controls.Add(Me.rbExistingRename)
        Me.gbOutput.Controls.Add(Me.cbNormalizeAttributes)
        Me.gbOutput.Controls.Add(Me.rbExistingOverwrite)
        Me.gbOutput.Controls.Add(Me.rbExistingAsk)
        Me.gbOutput.Controls.Add(Me.Label5)
        Me.gbOutput.Controls.Add(Me.Label4)
        Me.gbOutput.Controls.Add(Me.Label2)
        Me.gbOutput.Controls.Add(Me.txtOutputFolder)
        Me.gbOutput.Controls.Add(Me.btnBrowseOutputFolder)
        Me.gbOutput.Location = New System.Drawing.Point(11, 230)
        Me.gbOutput.Name = "gbOutput"
        Me.gbOutput.Size = New System.Drawing.Size(640, 133)
        Me.gbOutput.TabIndex = 19
        Me.gbOutput.TabStop = False
        Me.gbOutput.Text = "Output Options"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.rbAttr_NT)
        Me.Panel2.Controls.Add(Me.rbAttr_TN)
        Me.Panel2.Location = New System.Drawing.Point(17, 104)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(233, 23)
        Me.Panel2.TabIndex = 17
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rbS4S)
        Me.Panel1.Controls.Add(Me.rbNative)
        Me.Panel1.Controls.Add(Me.rbTGIOnly)
        Me.Panel1.Controls.Add(Me.rbTGIName)
        Me.Panel1.Controls.Add(Me.rbNameOnly)
        Me.Panel1.Location = New System.Drawing.Point(102, 42)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(413, 23)
        Me.Panel1.TabIndex = 15
        '
        'rbNative
        '
        Me.rbNative.AutoSize = True
        Me.rbNative.Location = New System.Drawing.Point(250, 3)
        Me.rbNative.Name = "rbNative"
        Me.rbNative.Size = New System.Drawing.Size(56, 17)
        Me.rbNative.TabIndex = 11
        Me.rbNative.TabStop = True
        Me.rbNative.Text = "Native"
        Me.rbNative.UseVisualStyleBackColor = True
        '
        'rbTGIOnly
        '
        Me.rbTGIOnly.AutoSize = True
        Me.rbTGIOnly.Location = New System.Drawing.Point(3, 3)
        Me.rbTGIOnly.Name = "rbTGIOnly"
        Me.rbTGIOnly.Size = New System.Drawing.Size(67, 17)
        Me.rbTGIOnly.TabIndex = 8
        Me.rbTGIOnly.TabStop = True
        Me.rbTGIOnly.Text = "TGI Only"
        Me.rbTGIOnly.UseVisualStyleBackColor = True
        '
        'rbTGIName
        '
        Me.rbTGIName.AutoSize = True
        Me.rbTGIName.Location = New System.Drawing.Point(77, 3)
        Me.rbTGIName.Name = "rbTGIName"
        Me.rbTGIName.Size = New System.Drawing.Size(83, 17)
        Me.rbTGIName.TabIndex = 9
        Me.rbTGIName.TabStop = True
        Me.rbTGIName.Text = "TGI + Name"
        Me.rbTGIName.UseVisualStyleBackColor = True
        '
        'rbNameOnly
        '
        Me.rbNameOnly.AutoSize = True
        Me.rbNameOnly.Location = New System.Drawing.Point(167, 3)
        Me.rbNameOnly.Name = "rbNameOnly"
        Me.rbNameOnly.Size = New System.Drawing.Size(77, 17)
        Me.rbNameOnly.TabIndex = 10
        Me.rbNameOnly.TabStop = True
        Me.rbNameOnly.Text = "Name Only"
        Me.rbNameOnly.UseVisualStyleBackColor = True
        '
        'rbExistingRename
        '
        Me.rbExistingRename.AutoSize = True
        Me.rbExistingRename.Location = New System.Drawing.Point(228, 65)
        Me.rbExistingRename.Name = "rbExistingRename"
        Me.rbExistingRename.Size = New System.Drawing.Size(133, 17)
        Me.rbExistingRename.TabIndex = 14
        Me.rbExistingRename.Text = "Rename Original (slow)"
        Me.rbExistingRename.UseVisualStyleBackColor = True
        '
        'rbExistingOverwrite
        '
        Me.rbExistingOverwrite.AutoSize = True
        Me.rbExistingOverwrite.Location = New System.Drawing.Point(152, 65)
        Me.rbExistingOverwrite.Name = "rbExistingOverwrite"
        Me.rbExistingOverwrite.Size = New System.Drawing.Size(70, 17)
        Me.rbExistingOverwrite.TabIndex = 13
        Me.rbExistingOverwrite.Text = "Overwrite"
        Me.rbExistingOverwrite.UseVisualStyleBackColor = True
        '
        'rbExistingAsk
        '
        Me.rbExistingAsk.AutoSize = True
        Me.rbExistingAsk.Location = New System.Drawing.Point(104, 65)
        Me.rbExistingAsk.Name = "rbExistingAsk"
        Me.rbExistingAsk.Size = New System.Drawing.Size(43, 17)
        Me.rbExistingAsk.TabIndex = 12
        Me.rbExistingAsk.Text = "Ask"
        Me.rbExistingAsk.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 67)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(70, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Existing Files:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 46)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(90, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "XML File Naming:"
        '
        'rbS4S
        '
        Me.rbS4S.AutoSize = True
        Me.rbS4S.Location = New System.Drawing.Point(312, 3)
        Me.rbS4S.Name = "rbS4S"
        Me.rbS4S.Size = New System.Drawing.Size(71, 17)
        Me.rbS4S.TabIndex = 12
        Me.rbS4S.TabStop = True
        Me.rbS4S.Text = "S4S Style"
        Me.rbS4S.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(663, 422)
        Me.Controls.Add(Me.gbOutput)
        Me.Controls.Add(Me.rbSingle)
        Me.Controls.Add(Me.rbFull)
        Me.Controls.Add(Me.gbSingle)
        Me.Controls.Add(Me.gbFull)
        Me.Controls.Add(Me.statusStrip)
        Me.Controls.Add(Me.btnExtract)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "TS4 Binary Tuning and Combined XML Extractor"
        Me.statusStrip.ResumeLayout(False)
        Me.statusStrip.PerformLayout()
        Me.gbFull.ResumeLayout(False)
        Me.gbFull.PerformLayout()
        Me.gbSingle.ResumeLayout(False)
        Me.gbSingle.PerformLayout()
        Me.gbOutput.ResumeLayout(False)
        Me.gbOutput.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtInputFile As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowseInputFile As System.Windows.Forms.Button
    Friend WithEvents btnExtract As System.Windows.Forms.Button
    Friend WithEvents dlgFileBrowser As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnBrowseOutputFolder As System.Windows.Forms.Button
    Friend WithEvents txtOutputFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents statusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents statusBar As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents cbPack As System.Windows.Forms.ComboBox
    Friend WithEvents lblGPValue As System.Windows.Forms.Label
    Friend WithEvents gbFull As System.Windows.Forms.GroupBox
    Friend WithEvents gbSingle As System.Windows.Forms.GroupBox
    Friend WithEvents rbFull As System.Windows.Forms.RadioButton
    Friend WithEvents rbSingle As System.Windows.Forms.RadioButton
    Friend WithEvents btnBrowseGameFolder As System.Windows.Forms.Button
    Friend WithEvents txtGameFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cbIncludeStrings As System.Windows.Forms.CheckBox
    Friend WithEvents cbIncludeXmlNames As System.Windows.Forms.CheckBox
    Friend WithEvents gbOutput As System.Windows.Forms.GroupBox
    Friend WithEvents rbNameOnly As System.Windows.Forms.RadioButton
    Friend WithEvents rbTGIName As System.Windows.Forms.RadioButton
    Friend WithEvents rbTGIOnly As System.Windows.Forms.RadioButton
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cbCreateRefFiles As System.Windows.Forms.CheckBox
    Friend WithEvents rbExistingRename As System.Windows.Forms.RadioButton
    Friend WithEvents rbExistingOverwrite As System.Windows.Forms.RadioButton
    Friend WithEvents rbExistingAsk As System.Windows.Forms.RadioButton
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents rbNative As System.Windows.Forms.RadioButton
    Friend WithEvents cbLanguage As System.Windows.Forms.ComboBox
    Friend WithEvents cbCreateFileIndex As System.Windows.Forms.CheckBox
    Friend WithEvents cbIgnoreNoTuning As System.Windows.Forms.CheckBox
    Friend WithEvents cbThreads As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cbSingleIncludeJazz As System.Windows.Forms.CheckBox
    Friend WithEvents cbIncludeJazz As System.Windows.Forms.CheckBox
    Friend WithEvents rbAttr_TN As RadioButton
    Friend WithEvents rbAttr_NT As RadioButton
    Friend WithEvents cbNormalizeAttributes As CheckBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents rbS4S As RadioButton
End Class
