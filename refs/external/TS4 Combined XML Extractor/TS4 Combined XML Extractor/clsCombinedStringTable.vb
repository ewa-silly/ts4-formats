Public Class clsCombinedStringTable
    Private IndexedStrings As New SortedList(Of UInt32, String)
    Private obj_frmMain As New Object

    Public Sub New(Caller As Object)
        obj_frmMain = Caller
    End Sub

    Public Function GetString(hash As UInt32) As String
        Dim strValue As String = String.Empty

        If IndexedStrings.TryGetValue(hash, strValue) Then
            Return strValue
        End If

        Return Nothing
    End Function

    Friend Sub AddPackage(inputFilePath As String)
        Dim resourceString As clsTS4Resource
        Dim packageFile As clsTS4Package
        Dim data() As Byte
        Dim mnFileIdentifier As UInt32
        Dim mnVersion As UInt16
        Dim mbCompressed As Byte
        Dim mnNumEntries As UInt64
        Dim mnStringLength As UInt32
        Dim I As UInt64
        Dim idx As Integer
        Dim mnKeyHash As UInt32
        Dim mnFlags As Byte
        Dim mnLength As UInt16
        Dim mString As String

        packageFile = New clsTS4Package(inputFilePath)
        If Not packageFile.IsValid Then
            Exit Sub
        End If
        resourceString = packageFile.GetResourceItem(Type:=&H220557DAUI)
        data = packageFile.GetResourceData(resourceString)

        mnFileIdentifier = BitConverter.ToUInt32(data, 0)
        mnVersion = BitConverter.ToUInt16(data, 4)
        mbCompressed = data(6)  ' Not currently used by TS4
        mnNumEntries = BitConverter.ToUInt64(data, 7)
        mnStringLength = BitConverter.ToUInt32(data, 17)

        If mnFileIdentifier <> &H4C425453UI Then
            Throw New Exception("Invalid string table, ignoring file: " & inputFilePath)
            Return
        ElseIf mnVersion <> 5 Then
            Throw New Exception("Unsupported string table version (" & mnVersion & "), ignoring file: " & inputFilePath)
            Return
        End If

        idx = 21
        For I = 1 To mnNumEntries
            mnKeyHash = BitConverter.ToUInt32(data, idx)
            mnFlags = data(idx + 4)
            mnLength = BitConverter.ToUInt16(data, idx + 5)
            If mnLength > 0 Then
                mString = System.Text.Encoding.UTF8.GetString(data, idx + 7, mnLength).Replace(ControlChars.CrLf, "\n")
            Else
                mString = String.Empty
            End If
            If Not IndexedStrings.ContainsKey(mnKeyHash) Then
                IndexedStrings.Add(mnKeyHash, mString)
            End If
            idx += 7 + mnLength
        Next
    End Sub

    Friend Sub DumpStringTables(outputFolderPath As String)
        Dim outFile As StreamWriter

        obj_frmMain.CheckForOverwrite(Path.Combine(outputFolderPath, "Reference - All Strings.txt"))
        outFile = New StreamWriter(Path.Combine(outputFolderPath, "Reference - All Strings.txt"))
        For Each kvp As KeyValuePair(Of UInt32, String) In IndexedStrings
            outFile.WriteLine("0x" & kvp.Key.ToString("X8") & ControlChars.Tab & kvp.Value)
        Next
        outFile.Close()
        outFile.Dispose()
    End Sub
End Class
