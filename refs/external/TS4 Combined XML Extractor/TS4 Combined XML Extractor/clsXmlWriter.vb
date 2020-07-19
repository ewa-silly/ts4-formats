Public Class clsXmlWriter
    Private nIndexed As Collection
    Private _inputFilePath As String
    Private _memStream As MemoryStream
    Private _outputBasePath As String
    Private _outputFolderPath As String
    Private _fullFolderPath As String
    Private _OutputGroup As UInt32
    Private _XmlNameTable As clsCombinedXmlNameTable
    Private _InstanceXRefTable As clsInstanceXRefTable
    Private _rbFull As Boolean = True
    Private obj_frmMain As frmMain
    Private _NormalizeAttrNT As Boolean = False
    Private _NormalizeAttrTN As Boolean = False

    ' NoXrefNames - XML elements which will not generate reference XML comments
    ' GroupNoXrefNames - XML elements which will not generate reference XML comments for self or child nodes
    Private NoXrefNames() As String = {"_default_convergence_value", "_household_funds", "actual_object", "add_weight", "amount", "attractor point definition", "base_value", "bassinet_to_use", "cas_part", "CASPart", "CHEF_STATION_CUTTING_BOARD_OBJECT", "CHEF_STATION_PAN_OBJECT", "CHEF_STATION_POT_OBJECT", "CHEF_STATION_SERVING_PLATTER_OBJECT", "collectable_item", "cost", "decoration_resource", "default_object_to_create", "default_value", "DefaultPart", "definition", "destination_lot", "fallback_definition", "House Description", "jig", "jig_to_use", "LARGE_PORTRAIT_OBJ_DEF", "lower_bound", "magazine_collection", "MannequinPart", "MatchingPart", "max_duration", "max_value_tuning", "maximum_auto_satisfy_time", "MEDIUM_PORTRAIT_OBJ_DEF", "model", "multiplier", "object_definition", "object_reference", "object_to_be_mailed", "pet_crate_object_definition", "plant", "Region Description", "RegionDescription", "restaurant_menu_icon_definition", "sign_definition", "SMALL_LANDSCAPE_OBJ_DEF", "Spawn Point ID", "Street Description ID", "Template House Description", "TEMPLE_LOT_DESCRIPTION", "time_overhead", "treasure", "upper_bound", "wall_mounted_object", "weight", "WorldDescription"}
    Private GroupNoXrefNames() As String = {"BABY_BASSINET_DEFINITION_MAP", "ingredient_list", "ingredients", "ObjectDefinitions", "possible_fish", "resource_key", "ResourceIds"}

    Public Sub New(Caller As frmMain, inputFilePath As String, outputFolderPath As String, OutputGroup As UInt32)
        obj_frmMain = Caller
        _inputFilePath = inputFilePath
        _outputFolderPath = outputFolderPath
        _fullFolderPath = _outputFolderPath
        _OutputGroup = OutputGroup
        _rbFull = False
        If obj_frmMain.cbNormalizeAttributes.Checked Then
            If obj_frmMain.rbAttr_NT.Checked Then
                _NormalizeAttrNT = True
            Else
                _NormalizeAttrTN = True
            End If
        End If
    End Sub

    Public Sub New(Caller As frmMain, memStream As MemoryStream, outputBasePath As String, outputFolderPath As String, OutputGroup As UInt32, XmlNameTable As clsCombinedXmlNameTable, InstanceXRefTable As clsInstanceXRefTable)
        obj_frmMain = Caller
        _memStream = memStream
        _outputBasePath = outputBasePath & Path.DirectorySeparatorChar
        _outputFolderPath = outputFolderPath
        _fullFolderPath = Path.Combine(_outputBasePath, _outputFolderPath)
        _OutputGroup = OutputGroup
        _XmlNameTable = XmlNameTable
        _InstanceXRefTable = InstanceXRefTable
        If obj_frmMain.cbNormalizeAttributes.Checked Then
            If obj_frmMain.rbAttr_NT.Checked Then
                _NormalizeAttrNT = True
            Else
                _NormalizeAttrTN = True
            End If
        End If
    End Sub

    Public Sub PerformFileBasedXMLExtract()
        Dim tmStart As Date = Now
        Dim tmEnd As Date
        Dim inputDoc As XmlDocument = New XmlDocument
        Dim eCombined As XmlElement

        If Not My.Computer.FileSystem.DirectoryExists(_fullFolderPath) Then
            My.Computer.FileSystem.CreateDirectory(_fullFolderPath)
        End If

        obj_frmMain.UpdateStatusbar("Reading Combined XML")

        nIndexed = New Collection

        Try
            inputDoc.Load(_inputFilePath)
            eCombined = inputDoc.DocumentElement

            For Each nCurrent As XmlNode In inputDoc.DocumentElement.ChildNodes
                If nCurrent.Name = "g" Then
                    GetAndIndexChildren(nCurrent)
                ElseIf nCurrent.Name = "R" Then
                    ProcessFolder(nCurrent, _fullFolderPath, _OutputGroup)
                End If
            Next

            tmEnd = Now
            obj_frmMain.UpdateStatusbar("Combined XML tuning extract completed in " & obj_frmMain.FormatTime(tmStart, tmEnd) & ".")
        Catch ex As XmlException
            Dim errorFolderPath As String = Path.Combine(_fullFolderPath, "XmlErrors")

            If Not My.Computer.FileSystem.DirectoryExists(errorFolderPath) Then
                My.Computer.FileSystem.CreateDirectory(errorFolderPath)
            End If
            My.Computer.FileSystem.CopyFile(_inputFilePath, Path.Combine(errorFolderPath, My.Computer.FileSystem.GetName(_inputFilePath)), True)
            obj_frmMain.UpdateStatusbar("Error reading XML tuning document " & _inputFilePath & ".")
        End Try

        nIndexed.Clear()
    End Sub

    Public Sub PerformMemBasedXMLExtract()
        Dim tmStart As Date = Now
        Dim tmEnd As Date
        Dim inputDoc As XmlDocument = New XmlDocument
        Dim eCombined As XmlElement

        If Not My.Computer.FileSystem.DirectoryExists(_fullFolderPath) Then
            My.Computer.FileSystem.CreateDirectory(_fullFolderPath)
        End If

        nIndexed = New Collection

        _memStream.Position = 0
        Try
            inputDoc.Load(_memStream)
            eCombined = inputDoc.DocumentElement

            For Each nCurrent As XmlNode In inputDoc.DocumentElement.ChildNodes
                If nCurrent.Name = "g" Then
                    GetAndIndexChildren(nCurrent)
                ElseIf nCurrent.Name = "R" Then
                    ProcessFolder(nCurrent, _fullFolderPath, _OutputGroup)
                End If
            Next

            tmEnd = Now
        Catch ex As XmlException
            Dim errorFolderPath As String = Path.Combine(_fullFolderPath, "XmlErrors")
            Dim dumpStream As StreamWriter

            If Not My.Computer.FileSystem.DirectoryExists(errorFolderPath) Then
                My.Computer.FileSystem.CreateDirectory(errorFolderPath)
            End If
            dumpStream = New StreamWriter(Path.Combine(errorFolderPath, "XmlError Group 0x", _OutputGroup.ToString("X8")))
            _memStream.Position = 0
            _memStream.CopyTo(dumpStream.BaseStream)
            dumpStream.Close()
        End Try

        nIndexed.Clear()
    End Sub

    Private Sub GetAndIndexChildren(node As XmlNode)
        For Each nCurrent As XmlNode In node.ChildNodes
            If nCurrent.Name <> "r" Then
                If Not nCurrent.Attributes Is Nothing Then
                    Dim ax As XmlAttribute = nCurrent.Attributes.ItemOf("x")
                    If Not ax Is Nothing Then
                        nIndexed.Add(nCurrent, ax.Value)
                    End If
                End If
            End If
            If nCurrent.HasChildNodes Then
                GetAndIndexChildren(nCurrent)
            End If
        Next
    End Sub

    Private Function ResolveNode(node As XmlNode) As XmlNode
        Dim newNode As XmlNode

        If node.Name = "r" Then
            Dim iNode As XmlNode = nIndexed.Item(node.Attributes.ItemOf("x").Value)
            newNode = iNode.Clone
            For Each aCurrent As XmlAttribute In node.Attributes
                newNode.Attributes.Append(aCurrent.Clone)
            Next
            newNode.Attributes.RemoveNamedItem("x")
        Else
            newNode = node.Clone
        End If

        If newNode.HasChildNodes Then
            For i = 0 To newNode.ChildNodes.Count - 1
                newNode.ReplaceChild(ResolveNode(newNode.ChildNodes(i)), newNode.ChildNodes(i))
            Next
        End If

        If (_NormalizeAttrNT Or _NormalizeAttrTN) AndAlso Not newNode.Attributes Is Nothing Then
            Dim nAttr As XmlAttribute = newNode.Attributes.ItemOf("n")
            Dim tAttr As XmlAttribute = newNode.Attributes.ItemOf("t")
            If Not nAttr Is Nothing And Not tAttr Is Nothing Then
                If _NormalizeAttrNT Then
                    newNode.Attributes.Remove(tAttr)
                    newNode.Attributes.Append(tAttr)
                Else
                    newNode.Attributes.Remove(nAttr)
                    newNode.Attributes.Append(nAttr)
                End If
            End If
        End If

        Return newNode
    End Function

    Private Sub WriteDoc(Type As UInt32, Group As UInt32, Instance As UInt64, strName As String, strClass As String, strInstanceType As String, doc As XmlDocument, outputFolderPath As String)
        Dim settings As New XmlWriterSettings
        Dim filename As String
        Dim writer As XmlWriter

        If frmMain.NamingConvention = NamingType.TGIOnly Then
            filename = Path.Combine(outputFolderPath, "S4_" & Type.ToString("X8") & "_" & Group.ToString("X8") & "_" & Instance.ToString("X16") & ".xml")
        ElseIf frmMain.NamingConvention = NamingType.TGIName Then
            filename = Path.Combine(outputFolderPath, "S4_" & Type.ToString("X8") & "_" & Group.ToString("X8") & "_" & Instance.ToString("X16") & "_" & strName & ".xml")
        ElseIf frmMain.NamingConvention = NamingType.NameOnly Then
            filename = Path.Combine(outputFolderPath, strName & ".xml")
        ElseIf frmMain.NamingConvention = NamingType.Native Then
            filename = Path.Combine(outputFolderPath, "0x" & Group.ToString("x8") & "!0x" & Instance.ToString("x16") & ".0x" & Type.ToString("x8"))
        Else
            filename = Path.Combine(outputFolderPath, Type.ToString("x8") & "!" & Group.ToString("x8") & "!" & Instance.ToString("x16") & "." & strName & "." & strInstanceType & ".xml")
        End If

        settings.Indent = True
        obj_frmMain.CheckForOverwrite(filename)
        writer = XmlWriter.Create(filename, settings)
        doc.WriteTo(writer)
        writer.Close()
        writer.Dispose()

        If frmMain.streamFileIndex IsNot Nothing Then
            filename = filename.Replace(_outputBasePath, "")
            frmMain.streamFileIndex.WriteLine(Instance & ControlChars.Tab & strName & ControlChars.Tab & strClass & ControlChars.Tab & strInstanceType & ControlChars.Tab & filename)
        End If
    End Sub

    Private Sub InsertXmlNameComments(doc As XmlDocument, nodes As XmlNodeList, InstanceXRef As clsInstanceXRef)
        Dim value As UInt64
        Dim XmlName As clsCombinedXmlNameTable.XmlName

        For Each node As XmlNode In nodes
            If Not node.Attributes Is Nothing Then
                If Not node.Attributes.GetNamedItem("n") Is Nothing Then
                    If GroupNoXrefNames.Contains(node.Attributes.GetNamedItem("n").Value) Then
                        Continue For
                    End If
                End If
            End If

            If node.ChildNodes.Count > 0 Then
                InsertXmlNameComments(doc, node.ChildNodes, InstanceXRef)
            Else
                If Not node.ParentNode.Attributes Is Nothing Then
                    If Not node.ParentNode.Attributes.GetNamedItem("n") Is Nothing Then
                        If NoXrefNames.Contains(node.ParentNode.Attributes.GetNamedItem("n").Value) Then
                            Continue For
                        End If
                    End If
                End If
                If UInt64.TryParse(node.InnerText, value) Then
                    XmlName = _XmlNameTable.GetXmlName(value)
                    If XmlName.nValue IsNot Nothing Then
                        Dim newComment As XmlComment
                        Dim newInstanceXRef As clsInstanceXRef

                        newInstanceXRef = _InstanceXRefTable.AddInstance(value)
                        newInstanceXRef.AddReferenceBy(InstanceXRef.Instance)
                        InstanceXRef.AddReferenceTo(value)

                        If XmlName.cValue IsNot Nothing Then
                            newComment = doc.CreateComment(XmlName.cValue & ": " & XmlName.nValue)
                        Else
                            newComment = doc.CreateComment(XmlName.nValue)
                        End If
                        node.ParentNode.AppendChild(newComment)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ProcessFile(node As XmlNode, Type As UInt32, outputFolderPath As String, OutputGroup As UInt32)
        Dim newDoc As New XmlDocument
        Dim newNode As XmlNode = node.Clone
        Dim i As Integer
        Dim Instance As UInt64 = UInt64.Parse(newNode.Attributes.GetNamedItem("s").Value)
        Dim strName As String
        Dim strClass As String = ""
        Dim strInstanceType As String = ""
        Dim InstanceXRef As clsInstanceXRef

        For i = 0 To node.ChildNodes.Count - 1
            newNode.ReplaceChild(ResolveNode(newNode.ChildNodes(i)), newNode.ChildNodes(i))
        Next
        newDoc.AppendChild(newDoc.ImportNode(newNode, True))
        If _rbFull AndAlso _XmlNameTable IsNot Nothing Then
            InstanceXRef = _InstanceXRefTable.AddInstance(Instance)
            InsertXmlNameComments(newDoc, newDoc.ChildNodes, InstanceXRef)
        End If
        strName = newNode.Attributes.GetNamedItem("n").Value
        If newNode.Attributes.ItemOf("c") IsNot Nothing Then
            strClass = newNode.Attributes.GetNamedItem("c").Value
            strInstanceType = newNode.Attributes.GetNamedItem("i").Value
        End If
        obj_frmMain.UpdateStatusbar("Writing " & strName)
        My.Application.DoEvents()
        WriteDoc(Type, OutputGroup, Instance, strName, strClass, strInstanceType, newDoc, outputFolderPath)
    End Sub

    Private Sub ProcessFolder(node As XmlNode, outputFolderPath As String, OutputGroup As UInt32)
        Dim FolderName As String
        Dim Type As UInt32 = GetTypeNumber(node.Attributes.GetNamedItem("n").Value)

        If node.Attributes.GetNamedItem("n").Value = "relbit" Then
            Type = GetTypeNumber("relationshipbit")
        End If

        FolderName = Path.Combine(outputFolderPath, node.Attributes.ItemOf("n").Value)
        If Not My.Computer.FileSystem.DirectoryExists(FolderName) Then
            My.Computer.FileSystem.CreateDirectory(FolderName)
        End If
        For Each nCurrent As XmlNode In node.ChildNodes
            If nCurrent.Name = "I" Then
                ProcessFile(nCurrent, Type, FolderName, OutputGroup)
            ElseIf nCurrent.Name = "M" Then
                ProcessFile(nCurrent, &H3B33DDFUI, FolderName, OutputGroup)
            End If
        Next
    End Sub

    Private Function GetTypeNumber(strTypeName As String) As UInt32
        Dim Hash As Int64 = &H811C9DC5UI

        For Each c As Char In strTypeName
            Hash = (Hash * &H1000193UI) And &HFFFFFFFFUI
            Hash = Hash Xor Asc(c)
        Next

        Return Hash And &HFFFFFFFFUI
    End Function
End Class
