Public Enum NamingType
    TGIOnly
    TGIName
    NameOnly
    Native
    S4S
End Enum

Public Class frmMain
    Private Structure structGamePackInfo
        Dim ShortName As String
        Dim LongName As String
        Dim Value As Integer
    End Structure

    Private SelectedGroup As UInt32
    Private boolExtractInProgress As Boolean = False
    Private GamePackInfo() As structGamePackInfo
    Private stblCombined As clsCombinedStringTable = Nothing
    Private XmlNameTable As clsCombinedXmlNameTable = Nothing
    Private InstanceXRefTable As clsInstanceXRefTable = Nothing
    Public Shared AlwaysOverwrite As Boolean = False
    Public Shared AlwaysRename As Boolean = False
    Public Shared streamFileIndex As TextWriter = Nothing
    Private numLogicalCPUs As Integer = Environment.ProcessorCount
    Public Shared NamingConvention As NamingType
    Private Shared DialogLock As New Object
    Private Shared CloseRequested As Boolean = False

    Private Sub frmMain_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If boolExtractInProgress Then
            If MsgBox("An extract is in progress, quitting will result in an incomplete extract." & Chr(13) & Chr(13) & "Are you sure you want to exit?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                e.Cancel = True
            Else
                CloseRequested = True
            End If
        End If
    End Sub

    Private Sub frmMain_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        GetGamePackData()
        If My.Settings.SettingsNeedUpgrade Then
            My.Settings.Upgrade()
            My.Settings.SettingsNeedUpgrade = False
        End If
        txtInputFile.Text = ""
        If Not String.IsNullOrEmpty(My.Settings.InputFilePath) Then
            If FileIO.FileSystem.FileExists(My.Settings.InputFilePath) Then
                txtInputFile.Text = My.Settings.InputFilePath
                If txtInputFile.Text.EndsWith(".package") Then
                    cbSingleIncludeJazz.Enabled = True
                Else
                    cbSingleIncludeJazz.Enabled = False
                End If
            End If
        End If
        txtOutputFolder.Text = ""
        If Not String.IsNullOrEmpty(My.Settings.OutputFolderPath) Then
            If FileIO.FileSystem.DirectoryExists(My.Settings.OutputFolderPath) Then
                txtOutputFolder.Text = My.Settings.OutputFolderPath
            End If
        End If
        If My.Computer.FileSystem.DirectoryExists(My.Settings.GameFolderPath) Then
            txtGameFolder.Text = My.Settings.GameFolderPath
        Else
            txtGameFolder.Text = LocateGameFolderPath()
        End If
        If My.Settings.FullModeSelected Then
            rbFull.Checked = True
            gbFull.Enabled = True
            rbSingle.Checked = False
            gbSingle.Enabled = False
            If My.Computer.FileSystem.DirectoryExists(txtGameFolder.Text) Then
                btnExtract.Enabled = True
            Else
                btnExtract.Enabled = False
            End If
        Else
            rbFull.Checked = False
            gbFull.Enabled = False
            rbSingle.Checked = True
            gbSingle.Enabled = True
            If My.Computer.FileSystem.FileExists(txtInputFile.Text) Then
                btnExtract.Enabled = True
            Else
                btnExtract.Enabled = False
            End If
        End If
        If My.Settings.IncludeStrings Then
            cbIncludeStrings.Checked = True
        End If
        If My.Settings.IncludeCrossrefs Then
            cbIncludeXmlNames.Checked = True
        End If
        If My.Settings.CreateReferences Then
            cbCreateRefFiles.Checked = True
        End If
        If My.Settings.CreateFileIndex Then
            cbCreateFileIndex.Checked = True
        End If
        If My.Settings.IgnoreNoTuning Then
            cbIgnoreNoTuning.Checked = True
        End If
        If My.Settings.NamingTGIOnly Then
            rbTGIOnly.Checked = True
            NamingConvention = NamingType.TGIOnly
        ElseIf My.Settings.NamingTGIName Then
            rbTGIName.Checked = True
            NamingConvention = NamingType.TGIName
        ElseIf My.Settings.NamingNameOnly Then
            rbNameOnly.Checked = True
            NamingConvention = NamingType.NameOnly
        ElseIf My.Settings.NamingNative Then
            rbNative.Checked = True
            NamingConvention = NamingType.Native
        Else
            rbS4S.Checked = True
            NamingConvention = NamingType.S4S
        End If
        If My.Settings.AlwaysOverwrite Then
            rbExistingOverwrite.Checked = True
            AlwaysOverwrite = True
        ElseIf My.Settings.AlwaysRename Then
            rbExistingRename.Checked = True
            AlwaysRename = True
        Else
            rbExistingAsk.Checked = True
        End If
        If My.Settings.NormalizeAttributeOrder Then
            cbNormalizeAttributes.Checked = True
        End If
        If Not My.Settings.AttrNT Then
            rbAttr_TN.Checked = True
        Else
            rbAttr_NT.Checked = True
        End If
        ScanForLanguages(txtGameFolder.Text)
        For I = 1 To numLogicalCPUs
            cbThreads.Items.Add(I)
        Next
        If My.Settings.NumberOfThreads > 0 Then
            cbThreads.SelectedIndex = Math.Min(My.Settings.NumberOfThreads - 1, numLogicalCPUs - 1)
        Else
            cbThreads.SelectedIndex = (numLogicalCPUs / 2) - 1
        End If
        If My.Settings.SingleIncludeJazz Then
            cbSingleIncludeJazz.Checked = True
        End If
        If My.Settings.IncludeJazz Then
            cbIncludeJazz.Checked = True
        End If

        Me.Text = "XML Extractor for The Sims 4 - v" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
        UpdateStatusbar("XML Extractor for The Sims 4 -- by Scumbumbo @ Mod The Sims")
    End Sub

    Private Sub ScanForLanguages(GameFolder As String)
        Dim Curdir As DirectoryInfo
        Dim Name As String
        Dim NoLanguages As Boolean = True

        If My.Computer.FileSystem.DirectoryExists(GameFolder) Then
            cbLanguage.Items.Clear()
            Curdir = New DirectoryInfo(GameFolder)
            For Each FInfo As FileInfo In Curdir.GetFiles("Strings_*.package", SearchOption.AllDirectories)
                Name = Path.GetFileNameWithoutExtension(FInfo.FullName)
                If Not cbLanguage.Items.Contains(Name) Then
                    cbLanguage.Items.Add(Name)
                    NoLanguages = False
                End If
            Next
        End If
        If NoLanguages Then
            cbLanguage.Items.Add("No string package found")
            cbLanguage.Enabled = False
            cbIncludeStrings.Enabled = False
            cbIncludeStrings.Checked = False
            My.Settings.IncludeStrings = False
        Else
            cbIncludeStrings.Enabled = True
            cbLanguage.Enabled = True
        End If
        If cbLanguage.Items.Contains(My.Settings.Language) Then
            cbLanguage.SelectedItem = My.Settings.Language
        Else
            cbLanguage.SelectedIndex = 0
        End If
    End Sub

    Private Sub GetGamePackData()
        Dim xmlGPData As New XmlDocument
        Dim GPs As XmlNode
        Dim idx As Integer = 0

        xmlGPData.LoadXml(My.Resources.xmlGPData)
        GPs = xmlGPData.FirstChild
        For Each GP As XmlNode In GPs.ChildNodes
            ReDim Preserve GamePackInfo(idx)
            GamePackInfo(idx).ShortName = GP.Item("SHORTNAME").InnerText
            GamePackInfo(idx).LongName = GP.Item("LONGNAME").InnerText
            GamePackInfo(idx).Value = GP.Item("VALUE").InnerText
            cbPack.Items.Add(GamePackInfo(idx).ShortName & " (" & GamePackInfo(idx).LongName & ")")
            idx += 1
        Next
        cbPack.SelectedIndex = 0
    End Sub

    Private Function ExtractBinaryTuningFromPackage(inputFilePath As String, outputFolderPath As String) As UInt32
        Dim resourceBinaryTuning As clsTS4Resource
        Dim packageFile As clsTS4Package
        Dim data() As Byte
        Dim outputFilename As String
        Dim fileOutput As BinaryWriter

        packageFile = New clsTS4Package(inputFilePath)
        If packageFile.IsValid Then
            resourceBinaryTuning = packageFile.GetResourceItem(Type:=&H62E94D38UI)
            If resourceBinaryTuning IsNot Nothing Then
                data = packageFile.GetResourceData(resourceBinaryTuning)
                outputFilename = "S4_" & resourceBinaryTuning.Type.ToString("X8") & "_" & resourceBinaryTuning.Group.ToString("X8") & "_" & resourceBinaryTuning.Instance.ToString("X16") & ".bnry"
                CheckForOverwrite(Path.Combine(outputFolderPath, outputFilename))
                fileOutput = New BinaryWriter(File.Open(Path.Combine(outputFolderPath, outputFilename), FileMode.Create))
                fileOutput.Write(data)
                fileOutput.Close()
                Return resourceBinaryTuning.Group
            Else
                packageFile.Close()
                If Not cbIgnoreNoTuning.Checked Then
                    NoTuningWarning(inputFilePath)
                End If
            End If
        End If

        Return 64
    End Function

    Private Function ExtractBinaryTuningFromPackage(inputFilePath As String, memStream As MemoryStream) As UInt32
        Dim resourceBinaryTuning As clsTS4Resource
        Dim packageFile As clsTS4Package
        Dim data() As Byte
        Dim fileOutput As BinaryWriter

        packageFile = New clsTS4Package(inputFilePath)
        If packageFile.IsValid Then
            resourceBinaryTuning = packageFile.GetResourceItem(Type:=&H62E94D38UI)
            If resourceBinaryTuning IsNot Nothing Then
                data = packageFile.GetResourceData(resourceBinaryTuning)
                fileOutput = New BinaryWriter(memStream)
                fileOutput.Write(data)
                Return resourceBinaryTuning.Group
            Else
                packageFile.Close()
                If Not cbIgnoreNoTuning.Checked Then
                    NoTuningWarning(inputFilePath)
                End If
            End If
        End If

        Return 64
    End Function

    Private Sub ExtractJazzFromPackage(inputFilePath As String, outputFolderPath As String)
        Dim jazzBinaryTunings() As clsTS4Resource
        Dim packageFile As clsTS4Package
        Dim data() As Byte
        Dim outputFilename As String
        Dim jazzOutputFolderPath As String = Path.Combine(outputFolderPath, "AsmJazz")
        Dim fileOutput As BinaryWriter

        If Not My.Computer.FileSystem.DirectoryExists(jazzOutputFolderPath) Then
            My.Computer.FileSystem.CreateDirectory(jazzOutputFolderPath)
        End If
        packageFile = New clsTS4Package(inputFilePath)
        If packageFile.IsValid Then
            packageFile.CloseOnGet = False
            jazzBinaryTunings = packageFile.SearchResources(Type:=&H2D5DF13UI)
            For Each jazzBinaryTuning As clsTS4Resource In jazzBinaryTunings
                data = packageFile.GetResourceData(jazzBinaryTuning)
                outputFilename = "S4_" & jazzBinaryTuning.Type.ToString("X8") & "_" & jazzBinaryTuning.Group.ToString("X8") & "_" & jazzBinaryTuning.Instance.ToString("X16") & ".jazz"
                CheckForOverwrite(Path.Combine(jazzOutputFolderPath, outputFilename))
                fileOutput = New BinaryWriter(File.Open(Path.Combine(jazzOutputFolderPath, outputFilename), FileMode.Create))
                fileOutput.Write(data)
                fileOutput.Close()
            Next
            packageFile.Close()
        End If
    End Sub

    Private Function ExtractJazzFromPackage(inputFilePath As String, Group As UInt32) As Dictionary(Of String, MemoryStream)
        Dim jazzBinaryTunings() As clsTS4Resource
        Dim packageFile As clsTS4Package
        Dim data() As Byte
        Dim strTGI As String
        Dim memStream As MemoryStream
        Dim fileOutput As BinaryWriter
        Dim jazzResources As New Dictionary(Of String, MemoryStream)

        packageFile = New clsTS4Package(inputFilePath)
        If packageFile.IsValid Then
            packageFile.CloseOnGet = False
            jazzBinaryTunings = packageFile.SearchResources(Type:=&H2D5DF13UI)
            For Each jazzBinaryTuning As clsTS4Resource In jazzBinaryTunings
                data = packageFile.GetResourceData(jazzBinaryTuning)
                strTGI = jazzBinaryTuning.Type.ToString("X8") & "_" & Group.ToString("X8") & "_" & jazzBinaryTuning.Instance.ToString("X16")
                data = packageFile.GetResourceData(jazzBinaryTuning)
                memStream = New MemoryStream
                fileOutput = New BinaryWriter(memStream)
                fileOutput.Write(data)
                jazzResources.Add(strTGI, memStream)
            Next
            packageFile.Close()
        End If

        Return jazzResources
    End Function

    Private Sub NoTuningWarning(strFilePath As String)
        Dim strBuildType As String
        Dim strGamePackage As String
        Dim intPackageNumber As Integer

        If strFilePath.Contains("FullBuild") Then
            strBuildType = "Full"
        Else
            strBuildType = "Delta"
        End If
        If strFilePath.Contains("\Data\") Then
            strGamePackage = "the base game"
        ElseIf strFilePath.Contains("\EP") Then
            intPackageNumber = strFilePath.Substring(strFilePath.IndexOf("EP") + 2, 2)
            strGamePackage = "EP" & intPackageNumber.ToString("00")
        ElseIf strFilePath.Contains("\SP") Then
            intPackageNumber = strFilePath.Substring(strFilePath.IndexOf("SP") + 2, 2)
            strGamePackage = "SP" & intPackageNumber.ToString("00")
        ElseIf strFilePath.Contains("\GP") Then
            intPackageNumber = strFilePath.Substring(strFilePath.IndexOf("GP") + 2, 2)
            strGamePackage = "GP" & intPackageNumber.ToString("00")
        Else
            strGamePackage = "FP01"
        End If
        MsgBox("Warning: The " & strBuildType & "Build package for " & strGamePackage & Chr(13) & "contains no binary tuning.", MsgBoxStyle.Information, "No Tuning in Package")
    End Sub

    Public Function FormatTime(tmStart As Date, tmEnd As Date) As String
        Dim Seconds As Integer = (tmEnd - tmStart).TotalSeconds
        Dim Minutes As Integer = Int(Seconds / 60)
        Dim str As String = String.Empty

        Seconds = Seconds Mod 60

        If Minutes > 1 Then
            str = Minutes & " minutes and "
        ElseIf Minutes > 0 Then
            str = Minutes & " minute and "
        End If

        If Seconds = 1 Then
            str = str & Seconds & " second"
        Else
            str = str & Seconds & " seconds"
        End If

        Return str
    End Function

    Private Sub PerformPackageExtract(inputFilePath As String, outputFolderPath As String)
        Dim tmStart As Date = Now
        Dim tmEnd As Date

        UpdateStatusbar("Extracting binary tuning from package file.")

        ExtractBinaryTuningFromPackage(inputFilePath, outputFolderPath)
        If cbSingleIncludeJazz.Checked Then
            ExtractJazzFromPackage(inputFilePath, outputFolderPath)
        End If

        tmEnd = Now
        UpdateStatusbar("Package extract to binary file completed in " & FormatTime(tmStart, tmEnd) & ".")
    End Sub

    Private Sub PerformBinaryExtract(inputFilePath As String, outputFolderPath As String)
        Dim tmStart As Date = Now
        Dim tmEnd As Date
        Dim binaryTuning As clsBinaryTuningConvertor

        UpdateStatusbar("Converting binary tuning file to combined XML.  This may take some time....")

        binaryTuning = New clsBinaryTuningConvertor(Me, txtInputFile.Text, txtOutputFolder.Text, False, False)

        tmEnd = Now
        UpdateStatusbar("Binary tuning file conversion completed in " & FormatTime(tmStart, tmEnd) & ".")
    End Sub

    Private Sub LoadStringTable(inputFilePath As String, outputFolderPath As String)
        If stblCombined Is Nothing Then
            stblCombined = New clsCombinedStringTable(Me)
        End If

        stblCombined.AddPackage(inputFilePath)
    End Sub

    Private Sub MergeJazzDictionaries(Dest As Dictionary(Of String, MemoryStream), Src As Dictionary(Of String, MemoryStream))
        For Each kvp As KeyValuePair(Of String, MemoryStream) In Src
            If Dest.ContainsKey(kvp.Key) Then
                Dim tmp_memStream As MemoryStream = Dest(kvp.Key)
                tmp_memStream.Close()
                Dest.Remove(kvp.Key)
            End If
            Dest.Add(kvp.Key, kvp.Value)
        Next
    End Sub

    Private Sub PerformFullAutoExtract(gameFolderPath As String, outputFolderPath As String)
        Dim tmStart As Date = Now
        Dim tmEnd As Date
        Dim CurDir As DirectoryInfo
        Dim FInfo() As FileInfo
        Dim packageName As String
        Dim memStreams(127) As MemoryStream
        Dim GamePackNumber As Integer
        Dim MaxThreads As Integer = cbThreads.SelectedIndex + 1
        Dim TaskIdx As Integer
        Dim Preload As Integer
        Dim NumLeft As Integer
        Dim NextBinTask As clsBinaryTuningConvertorThread
        Dim NextXmlTask As clsXmlWriter
        Dim BinThreadQueue As New Queue(Of clsBinaryTuningConvertorThread)
        Dim XmlThreadQueue As New Queue(Of clsXmlWriter)
        Dim RunningTasks As New List(Of Task)
        Dim Group As UInt32
        Dim JazzResources As New Dictionary(Of String, MemoryStream)

        UpdateStatusbar("Getting binary tuning from packages")

        CurDir = New DirectoryInfo(gameFolderPath)
        FInfo = CurDir.GetFiles("SimulationFullBuild0.package", SearchOption.AllDirectories)
        For I = 0 To FInfo.Count - 1
            Dim tmp_memStream As New MemoryStream
            Group = ExtractBinaryTuningFromPackage(FInfo(I).FullName, tmp_memStream)
            If Group < 64 Then
                memStreams(Group) = tmp_memStream
            End If
            If cbIncludeJazz.Checked Then
                MergeJazzDictionaries(JazzResources, ExtractJazzFromPackage(FInfo(I).FullName, Group))
            End If
            My.Application.DoEvents()
        Next

        FInfo = CurDir.GetFiles("SimulationDeltaBuild0.package", SearchOption.AllDirectories)
        For I = 0 To FInfo.Count - 1
            Dim tmp_memStream As New MemoryStream
            Group = ExtractBinaryTuningFromPackage(FInfo(I).FullName, tmp_memStream)
            If Group < 64 Then
                ' Overwrite full build with a delta if we have one
                If memStreams(Group) IsNot Nothing Then
                    memStreams(Group).Close()
                    memStreams(Group).Dispose()
                End If
                memStreams(Group) = tmp_memStream
            End If
            If cbIncludeJazz.Checked Then
                MergeJazzDictionaries(JazzResources, ExtractJazzFromPackage(FInfo(I).FullName, Group))
            End If
            My.Application.DoEvents()
        Next

        If cbIncludeStrings.Checked Then
            UpdateStatusbar("Reading and indexing string table packages")
            FInfo = CurDir.GetFiles(cbLanguage.SelectedItem & ".package", SearchOption.AllDirectories)
            For I = 0 To FInfo.Count - 1
                LoadStringTable(FInfo(I).FullName, outputFolderPath)
                My.Application.DoEvents()
            Next
            If cbCreateRefFiles.Checked Then
                stblCombined.DumpStringTables(outputFolderPath)
            End If
        End If

        If cbIncludeXmlNames.Checked Then
            XmlNameTable = New clsCombinedXmlNameTable(Me)
            InstanceXRefTable = New clsInstanceXRefTable(Me)
        Else
            XmlNameTable = Nothing
            InstanceXRefTable = Nothing
        End If

        If cbCreateFileIndex.Checked Then
            CheckForOverwrite(Path.Combine(outputFolderPath, "XML File Index.txt"))
            streamFileIndex = TextWriter.Synchronized(New StreamWriter(Path.Combine(outputFolderPath, "XML File Index.txt")))
        End If

        If cbIncludeJazz.Checked Then
            Dim JazzThreadQueue As New Queue(Of clsJazzConvertorThread)
            Dim NextJazzTask As clsJazzConvertorThread

            NumLeft = 0
            For Each kvp As KeyValuePair(Of String, MemoryStream) In JazzResources
                Dim convertor As clsJazzConvertorThread = New clsJazzConvertorThread(Me, kvp.Key, kvp.Value, XmlNameTable)
                GamePackNumber = GetGamePackNumber(convertor.Group)
                convertor.OutputBasePath = outputFolderPath
                convertor.OutputFolderPath = Path.Combine(GamePackInfo(GamePackNumber).ShortName, "AsmJazz")
                JazzThreadQueue.Enqueue(convertor)
                NumLeft += 1
            Next
            JazzResources.Clear()

            Preload = Math.Min(MaxThreads, JazzThreadQueue.Count)
            For I = 1 To Preload
                My.Application.DoEvents()
                NextJazzTask = JazzThreadQueue.Dequeue
                Dim taskRun As Task = Task.Run(AddressOf NextJazzTask.PerformExtract)
                RunningTasks.Add(taskRun)
                UpdateStatusbar("Converting ASM/JAZZ resources:  " & NumLeft & " resources remaining, " & RunningTasks.Count & " threads running.")
            Next
            While JazzThreadQueue.Count > 0
                My.Application.DoEvents()
                If CloseRequested Then
                    Exit Sub
                End If
                TaskIdx = Task.WaitAny(RunningTasks.ToArray())
                RunningTasks.RemoveAt(TaskIdx)
                NextJazzTask = JazzThreadQueue.Dequeue
                Dim taskRun As Task = Task.Run(AddressOf NextJazzTask.PerformExtract)
                RunningTasks.Add(taskRun)
                NumLeft -= 1
                UpdateStatusbar("Converting ASM/JAZZ resources:  " & NumLeft & " resources remaining, " & RunningTasks.Count & " threads running.")
            End While

            While RunningTasks.Count > 0
                My.Application.DoEvents()
                If CloseRequested Then
                    Exit Sub
                End If
                TaskIdx = Task.WaitAny(RunningTasks.ToArray())
                RunningTasks.RemoveAt(TaskIdx)
                NumLeft -= 1
                UpdateStatusbar("Converting ASM/JAZZ resources:  " & NumLeft & " resources remaining, " & RunningTasks.Count & " threads running.")
            End While
        End If

        NumLeft = 0
        For I = 0 To 63
            My.Application.DoEvents()
            If memStreams(I) IsNot Nothing Then
                packageName = GamePackInfo(GetGamePackNumber(I)).ShortName
                memStreams(64 + I) = New MemoryStream
                BinThreadQueue.Enqueue(New clsBinaryTuningConvertorThread(Me, memStreams(I), memStreams(64 + I), cbIncludeStrings.Checked, cbIncludeXmlNames.Checked, stblCombined, XmlNameTable))
                NumLeft += 1
                My.Application.DoEvents()
            End If
        Next

        Preload = Math.Min(MaxThreads, BinThreadQueue.Count)
        For I = 1 To Preload
            My.Application.DoEvents()
            NextBinTask = BinThreadQueue.Dequeue
            Dim taskRun As Task = Task.Run(AddressOf NextBinTask.ThreadMain)
            RunningTasks.Add(taskRun)
            UpdateStatusbar("Converting binary tuning to combined XML format:  " & NumLeft & " packs remaining, " & RunningTasks.Count & " threads running.")
        Next
        While BinThreadQueue.Count > 0
            My.Application.DoEvents()
            If CloseRequested Then
                Exit Sub
            End If
            TaskIdx = Task.WaitAny(RunningTasks.ToArray())
            RunningTasks.RemoveAt(TaskIdx)
            NextBinTask = BinThreadQueue.Dequeue
            Dim taskrun As Task = Task.Run(AddressOf NextBinTask.ThreadMain)
            RunningTasks.Add(taskrun)
            NumLeft -= 1
            UpdateStatusbar("Converting binary tuning to combined XML format:  " & NumLeft & " packs remaining, " & RunningTasks.Count & " threads running.")
        End While

        While RunningTasks.Count > 0
            My.Application.DoEvents()
            If CloseRequested Then
                Exit Sub
            End If
            TaskIdx = Task.WaitAny(RunningTasks.ToArray())
            RunningTasks.RemoveAt(TaskIdx)
            NumLeft -= 1
            UpdateStatusbar("Converting binary tuning to combined XML format: " & NumLeft & " packs remaining, " & RunningTasks.Count & " threads running.")
        End While

        If cbIncludeXmlNames.Checked And cbCreateRefFiles.Checked Then
            XmlNameTable.DumpXmlNamesTable(outputFolderPath)
        End If

        NumLeft = 0
        For I = 64 To 127
            My.Application.DoEvents()
            If memStreams(I) IsNot Nothing Then
                GamePackNumber = GetGamePackNumber(I - 64)
                packageName = GamePackInfo(GamePackNumber).ShortName
                Group = GamePackInfo(GamePackNumber).Value

                XmlThreadQueue.Enqueue(New clsXmlWriter(Me, memStreams(I), outputFolderPath, packageName, Group, XmlNameTable, InstanceXRefTable))
                NumLeft += 1
            End If
        Next

        Preload = Math.Min(MaxThreads, XmlThreadQueue.Count)
        For I = 1 To Preload
            My.Application.DoEvents()
            NextXmlTask = XmlThreadQueue.Dequeue
            Dim taskRun As Task = Task.Run(AddressOf NextXmlTask.PerformMemBasedXMLExtract)
            RunningTasks.Add(taskRun)
            UpdateStatusbar("Unpacking combined XML to folders:  " & NumLeft & " packs remaining, " & RunningTasks.Count & " threads running.")
        Next
        While XmlThreadQueue.Count > 0
            My.Application.DoEvents()
            If CloseRequested Then
                Exit Sub
            End If
            TaskIdx = Task.WaitAny(RunningTasks.ToArray())
            RunningTasks.RemoveAt(TaskIdx)
            NextXmlTask = XmlThreadQueue.Dequeue
            Dim taskrun As Task = Task.Run(AddressOf NextXmlTask.PerformMemBasedXMLExtract)
            RunningTasks.Add(taskrun)
            NumLeft -= 1
            UpdateStatusbar("Unpacking combined XML to folders:  " & NumLeft & " packs remaining, " & RunningTasks.Count & " threads running.")
        End While

        While RunningTasks.Count > 0
            My.Application.DoEvents()
            If CloseRequested Then
                Exit Sub
            End If
            TaskIdx = Task.WaitAny(RunningTasks.ToArray())
            RunningTasks.RemoveAt(TaskIdx)
            NumLeft -= 1
            UpdateStatusbar("Unpacking combined XML to folders: " & NumLeft & " packs remaining, " & RunningTasks.Count & " threads running.")
        End While

        For I = 0 To 127
            If memStreams(I) IsNot Nothing Then
                memStreams(I).Close()
                memStreams(I).Dispose()
            End If
        Next
        If streamFileIndex IsNot Nothing Then
            streamFileIndex.Close()
            streamFileIndex.Dispose()
            streamFileIndex = Nothing
        End If

        If cbCreateFileIndex.Checked Then
            InstanceXRefTable.DumpInstanceXRefTable(outputFolderPath)
        End If

        tmEnd = Now
        UpdateStatusbar("All XML extracted from the game folders in " & FormatTime(tmStart, tmEnd) & ".")
    End Sub

    Private Sub btnExtract_Click(sender As System.Object, e As System.EventArgs) Handles btnExtract.Click
        rbFull.Enabled = False
        rbSingle.Enabled = False
        gbFull.Enabled = False
        gbSingle.Enabled = False
        gbOutput.Enabled = False
        btnExtract.Enabled = False
        boolExtractInProgress = True
        Me.Cursor = Cursors.WaitCursor

        If rbFull.Checked Then
            PerformFullAutoExtract(txtGameFolder.Text, txtOutputFolder.Text)
        Else
            If txtInputFile.Text.EndsWith(".package") Then
                PerformPackageExtract(txtInputFile.Text, txtOutputFolder.Text)
            ElseIf txtInputFile.Text.EndsWith(".bnry") Then
                PerformBinaryExtract(txtInputFile.Text, txtOutputFolder.Text)
            ElseIf txtInputFile.Text.EndsWith(".jazz") Then
                PerformBinaryExtract(txtInputFile.Text, txtOutputFolder.Text)
            ElseIf txtInputFile.Text.EndsWith(".xml") Then
                Dim XmlWriterThread As New clsXmlWriter(Me, txtInputFile.Text, txtOutputFolder.Text, SelectedGroup)
                XmlWriterThread.PerformFileBasedXMLExtract()
            Else
                MsgBox("Unable to identify input file from extension, combined XML input assumed.")
                Dim XmlWriterThread As New clsXmlWriter(Me, txtInputFile.Text, txtOutputFolder.Text, SelectedGroup)
                XmlWriterThread.PerformFileBasedXMLExtract()
            End If
        End If

        If AlwaysOverwrite Then
            rbExistingOverwrite.Checked = True
        ElseIf AlwaysRename Then
            rbExistingRename.Checked = True
        End If

        If rbFull.Checked Then
            gbFull.Enabled = True
        Else
            gbSingle.Enabled = True
        End If
        rbFull.Enabled = True
        rbSingle.Enabled = True
        gbOutput.Enabled = True
        btnExtract.Enabled = True
        boolExtractInProgress = False
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub btnBrowseInputFile_Click(sender As System.Object, e As System.EventArgs) Handles btnBrowseInputFile.Click
        Dim dlgResult As Windows.Forms.DialogResult

        dlgFileBrowser.Filter = "Combined Binary File (*.bnry)|*.bnry|Combined XML File (*.xml)|*.xml|Game Package File (*.package)|*.package|JAZZ File (*.jazz)|*.jazz|All Files (*.*)|*.*"
        dlgFileBrowser.Multiselect = False
        dlgFileBrowser.FileName = txtInputFile.Text

        dlgResult = dlgFileBrowser.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        txtInputFile.Text = dlgFileBrowser.FileName
        My.Settings.InputFilePath = txtInputFile.Text
        If txtInputFile.Text.EndsWith(".package") Then
            cbSingleIncludeJazz.Enabled = True
        Else
            cbSingleIncludeJazz.Enabled = False
        End If
    End Sub

    Private Sub btnBrowseOutputFolder_Click(sender As System.Object, e As System.EventArgs) Handles btnBrowseOutputFolder.Click
        Dim dialog As New clsFolderBrowser
        Dim dlgResult As Windows.Forms.DialogResult

        dialog.DirectoryPath = txtOutputFolder.Text
        dlgResult = dialog.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        txtOutputFolder.Text = dialog.DirectoryPath
        My.Settings.OutputFolderPath = txtOutputFolder.Text
    End Sub

    Private Sub cbPack_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbPack.SelectedIndexChanged
        SelectedGroup = GamePackInfo(cbPack.SelectedIndex).Value
        lblGPValue.Text = "0x" & SelectedGroup.ToString("X8")
    End Sub

    Private Function GetGamePackNumber(Filename As String) As Integer
        Dim strGroup As String = Path.GetFileName(Filename)
        Dim GroupNumber As Integer
        Dim idx As Integer

        If strGroup.Length < 20 Then
            Return 0
        End If
        If strGroup.StartsWith("0x") Then
            strGroup = strGroup.Substring(2, 8)
        Else
            If strGroup.EndsWith(".bnry") Or strGroup.EndsWith(".xml") Or strGroup.EndsWith(".jazz") Then
                strGroup = strGroup.Substring(12, 8)
            End If
        End If
        If Not Int32.TryParse(strGroup, Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.CurrentCulture, GroupNumber) Then
            Return 0
        End If
        For idx = 0 To GamePackInfo.Count - 1
            If GroupNumber = GamePackInfo(idx).Value Then
                Return idx
            End If
        Next
        Return 0
    End Function

    Private Function GetGamePackNumber(GroupNumber As Integer) As Integer
        For idx = 0 To GamePackInfo.Count - 1
            If GroupNumber = GamePackInfo(idx).Value Then
                Return idx
            End If
        Next
        Return 0
    End Function

    Private Function GetGamePackNumber(GroupNumber As UInteger) As Integer
        For idx = 0 To GamePackInfo.Count - 1
            If GroupNumber = GamePackInfo(idx).Value Then
                Return idx
            End If
        Next
        Return 0
    End Function

    Private Sub txtInputFile_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtInputFile.TextChanged
        cbPack.SelectedIndex = GetGamePackNumber(txtInputFile.Text)
        If My.Computer.FileSystem.FileExists(txtInputFile.Text) Then
            btnExtract.Enabled = True
        Else
            btnExtract.Enabled = False
        End If
    End Sub

    Private Sub rbFull_CheckedChanged(sender As Object, e As EventArgs) Handles rbFull.CheckedChanged
        If rbFull.Checked Then
            gbFull.Enabled = True
            gbSingle.Enabled = False
            My.Settings.FullModeSelected = True
            If My.Computer.FileSystem.DirectoryExists(txtGameFolder.Text) Then
                btnExtract.Enabled = True
            Else
                btnExtract.Enabled = False
            End If
        Else
            gbFull.Enabled = False
            gbSingle.Enabled = True
            My.Settings.FullModeSelected = False
            If My.Computer.FileSystem.FileExists(txtInputFile.Text) Then
                btnExtract.Enabled = True
            Else
                btnExtract.Enabled = False
            End If
        End If
    End Sub

    Private Function LocateGameFolderPath() As String
        Dim RegKey As RegistryKey

        RegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Maxis\The Sims 4")
        If Not RegKey Is Nothing Then
            Return RegKey.GetValue("Install Dir")
        End If

        Return String.Empty
    End Function

    Private Sub btnBrowseGameFolder_Click(sender As Object, e As EventArgs) Handles btnBrowseGameFolder.Click
        Dim dialog As New clsFolderBrowser
        Dim dlgResult As Windows.Forms.DialogResult

        dialog.DirectoryPath = txtGameFolder.Text
        dlgResult = dialog.ShowDialog()
        If dlgResult = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        txtGameFolder.Text = dialog.DirectoryPath
        My.Settings.GameFolderPath = txtGameFolder.Text
    End Sub

    Private Sub cbIncludeStrings_CheckedChanged(sender As Object, e As EventArgs) Handles cbIncludeStrings.CheckedChanged
        My.Settings.IncludeStrings = cbIncludeStrings.Checked
    End Sub

    Private Sub cbIncludeXmlNames_CheckedChanged(sender As Object, e As EventArgs) Handles cbIncludeXmlNames.CheckedChanged
        My.Settings.IncludeCrossrefs = cbIncludeXmlNames.Checked
    End Sub

    Private Sub cbCreateRefFiles_CheckedChanged(sender As Object, e As EventArgs) Handles cbCreateRefFiles.CheckedChanged
        My.Settings.CreateReferences = cbCreateRefFiles.Checked
    End Sub

    Private Sub cbCreateFileIndex_CheckedChanged(sender As Object, e As EventArgs) Handles cbCreateFileIndex.CheckedChanged
        My.Settings.CreateFileIndex = cbCreateFileIndex.Checked
    End Sub

    Private Sub cbIgnoreNoTuning_CheckedChanged(sender As Object, e As EventArgs) Handles cbIgnoreNoTuning.CheckedChanged
        My.Settings.IgnoreNoTuning = cbIgnoreNoTuning.Checked
    End Sub

    Private Sub rbTGIOnly_CheckedChanged(sender As Object, e As EventArgs) Handles rbTGIOnly.CheckedChanged
        If rbTGIOnly.Checked Then
            My.Settings.NamingTGIOnly = True
            My.Settings.NamingTGIName = False
            My.Settings.NamingNameOnly = False
            My.Settings.NamingNative = False
            My.Settings.NamingS4S = False
            NamingConvention = NamingType.TGIOnly
        End If
    End Sub

    Private Sub rbTGIName_CheckedChanged(sender As Object, e As EventArgs) Handles rbTGIName.CheckedChanged
        If rbTGIName.Checked Then
            My.Settings.NamingTGIOnly = False
            My.Settings.NamingTGIName = True
            My.Settings.NamingNameOnly = False
            My.Settings.NamingNative = False
            My.Settings.NamingS4S = False
            NamingConvention = NamingType.TGIName
        End If
    End Sub

    Private Sub rbNameOnly_CheckedChanged(sender As Object, e As EventArgs) Handles rbNameOnly.CheckedChanged
        If rbNameOnly.Checked Then
            My.Settings.NamingTGIOnly = False
            My.Settings.NamingTGIName = False
            My.Settings.NamingNameOnly = True
            My.Settings.NamingNative = False
            My.Settings.NamingS4S = False
            NamingConvention = NamingType.NameOnly
        End If
    End Sub

    Private Sub rbNative_CheckedChanged(sender As Object, e As EventArgs) Handles rbNative.CheckedChanged
        If rbNative.Checked Then
            My.Settings.NamingTGIOnly = False
            My.Settings.NamingTGIName = False
            My.Settings.NamingNameOnly = False
            My.Settings.NamingNative = True
            My.Settings.NamingS4S = False
            NamingConvention = NamingType.Native
        End If
    End Sub

    Private Sub rbS4S_CheckedChanged(sender As Object, e As EventArgs) Handles rbS4S.CheckedChanged
        If rbS4S.Checked Then
            My.Settings.NamingTGIOnly = False
            My.Settings.NamingTGIName = False
            My.Settings.NamingNameOnly = False
            My.Settings.NamingNative = False
            My.Settings.NamingS4S = True
            NamingConvention = NamingType.S4S
        End If
    End Sub

    Public Sub CheckForOverwrite(filename As String)
        If My.Computer.FileSystem.FileExists(filename) Then
            If Not AlwaysOverwrite Then
                SyncLock DialogLock
                    ' A dialog we were waiting on may have set AlwaysOverwrite, so retest
                    If Not AlwaysOverwrite Then
                        ' It's actually stupidly slow to create a new dialog object to accomplish the rename
                        ' if rename always is on, but no one ever uses that option so...
                        Dim dlg As dlgOverwrite = New dlgOverwrite(filename)
                        Dim dlgResult As DialogResult

                        dlg = New dlgOverwrite(filename)
                        If AlwaysRename Then
                            ' Not sure why exactly I did that this way to be honest...
                            dlg.RenameFile()
                        Else
                            dlgResult = dlg.ShowDialog()
                            If dlgResult = Windows.Forms.DialogResult.OK Then
                                If dlg.AlwaysChecked Then
                                    AlwaysOverwrite = True
                                    AlwaysRename = False
                                End If
                            Else
                                If dlg.AlwaysChecked Then
                                    AlwaysOverwrite = False
                                    AlwaysRename = True
                                End If
                            End If
                        End If
                        ' Oh well, it's over, not too worried about it...
                        dlg.Dispose()
                    End If
                End SyncLock
            End If
        End If
    End Sub

    Public Sub UpdateStatusbar(Message As String)
        If Not Me.InvokeRequired Then
            Me.statusBar.Text = Message
            Me.statusStrip.Refresh()
        End If
    End Sub

    Private Sub rbExistingAsk_CheckedChanged(sender As Object, e As EventArgs) Handles rbExistingAsk.CheckedChanged
        If rbExistingAsk.Checked Then
            My.Settings.AlwaysOverwrite = False
            AlwaysOverwrite = False
            My.Settings.AlwaysRename = False
            AlwaysRename = False
        End If
    End Sub

    Private Sub rbExistingOverwrite_CheckedChanged(sender As Object, e As EventArgs) Handles rbExistingOverwrite.CheckedChanged
        If rbExistingOverwrite.Checked Then
            My.Settings.AlwaysOverwrite = True
            AlwaysOverwrite = True
            My.Settings.AlwaysRename = False
            AlwaysRename = False
        End If
    End Sub

    Private Sub rbExistingRename_CheckedChanged(sender As Object, e As EventArgs) Handles rbExistingRename.CheckedChanged
        If rbExistingRename.Checked Then
            My.Settings.AlwaysOverwrite = False
            AlwaysOverwrite = False
            My.Settings.AlwaysRename = True
            AlwaysRename = True
        End If
    End Sub

    Private Sub rbExistingRename_Click(sender As Object, e As EventArgs) Handles rbExistingRename.Click
        MsgBox("Warning: Using the rename files option will be significantly slower if performing a full extract.  There are over 30,000 files that will need to be renamed." & vbCrLf & vbCrLf & "Only choose this option if you absolutely need it.", MsgBoxStyle.Information)
    End Sub

    Private Sub txtGameFolder_TextChanged(sender As Object, e As EventArgs) Handles txtGameFolder.TextChanged
        If My.Computer.FileSystem.DirectoryExists(txtGameFolder.Text) Then
            btnExtract.Enabled = True
            ScanForLanguages(txtGameFolder.Text)
        Else
            btnExtract.Enabled = False
        End If
    End Sub

    Private Sub cbLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbLanguage.SelectedIndexChanged
        If Not cbLanguage.SelectedItem = "No string package found" Then
            My.Settings.Language = cbLanguage.SelectedItem
        End If
    End Sub

    Private Sub cbThreads_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbThreads.SelectedIndexChanged
        My.Settings.NumberOfThreads = cbThreads.SelectedIndex + 1
    End Sub

    Private Sub cbSingleIncludeJazz_CheckedChanged(sender As Object, e As EventArgs) Handles cbSingleIncludeJazz.CheckedChanged
        My.Settings.SingleIncludeJazz = cbSingleIncludeJazz.Checked
    End Sub

    Private Sub cbIncludeJazz_CheckedChanged(sender As Object, e As EventArgs) Handles cbIncludeJazz.CheckedChanged
        My.Settings.IncludeJazz = cbIncludeJazz.Checked
    End Sub

    Private Sub cbNormalizeAttributes_CheckedChanged(sender As Object, e As EventArgs) Handles cbNormalizeAttributes.CheckedChanged
        My.Settings.NormalizeAttributeOrder = cbNormalizeAttributes.Checked
        If cbNormalizeAttributes.Checked Then
            rbAttr_NT.Enabled = True
            rbAttr_TN.Enabled = True
        Else
            rbAttr_NT.Enabled = False
            rbAttr_TN.Enabled = False
        End If
    End Sub

    Private Sub rbAttr_NT_CheckedChanged(sender As Object, e As EventArgs) Handles rbAttr_NT.CheckedChanged
        My.Settings.AttrNT = rbAttr_NT.Checked
    End Sub
End Class
