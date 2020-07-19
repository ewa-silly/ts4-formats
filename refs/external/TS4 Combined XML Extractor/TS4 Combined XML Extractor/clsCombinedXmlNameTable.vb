Public Class clsCombinedXmlNameTable
    Public Structure XmlName
        Dim nValue As String
        Dim cValue As String
    End Structure

    Private obj_frmMain As frmMain
    Public Shared IndexedXmlNames As New SortedList(Of UInt64, XmlName)
    Private Shared InsertLock As New Object

    Public Sub New(Caller As frmMain)
        obj_frmMain = Caller
    End Sub

    Public Function GetXmlName(instance As UInt64) As XmlName
        Dim XmlNameValue As New XmlName

        If IndexedXmlNames.TryGetValue(instance, XmlNameValue) Then
            Return XmlNameValue
        End If

        XmlNameValue.nValue = Nothing
        Return XmlNameValue
    End Function

    Friend Sub AddXmlName(n() As Byte, s() As Byte, c() As Byte)
        Dim instance As UInt64
        Dim newXmlName As New XmlName

        If Not UInt64.TryParse(System.Text.Encoding.UTF8.GetString(s), Globalization.NumberStyles.Integer, Globalization.CultureInfo.CurrentCulture, instance) Then
            Throw New Exception("Invalid s value found in binary tuning, unable to index")
            Return
        End If
        newXmlName.nValue = System.Text.Encoding.UTF8.GetString(n).Replace("&amp;", "&").Replace("&apos;", "'")
        If c IsNot Nothing Then
            newXmlName.cValue = System.Text.Encoding.UTF8.GetString(c)
        Else
            newXmlName.cValue = String.Empty
        End If

        SyncLock InsertLock
            Try
                IndexedXmlNames.Add(instance, newXmlName)
            Catch
                ' Ignore, duplicate key
            End Try
        End SyncLock
    End Sub

    Friend Sub AddJazzXmlName(name As String, instance As UInt64)
        Dim newXmlName As New XmlName

        newXmlName.nValue = name
        newXmlName.cValue = "JAZZ"
        SyncLock InsertLock
            Try
                IndexedXmlNames.Add(instance, newXmlName)
            Catch ex As Exception
                ' Ignore, duplicate key
            End Try
        End SyncLock
    End Sub

    Friend Sub DumpXmlNamesTable(outputFolderPath As String)
        Dim outFile As StreamWriter

        obj_frmMain.CheckForOverwrite(Path.Combine(outputFolderPath, "Reference - All XML Names.txt"))
        outFile = New StreamWriter(Path.Combine(outputFolderPath, "Reference - All XML Names.txt"))
        For Each kvp As KeyValuePair(Of UInt64, XmlName) In IndexedXmlNames
            outFile.WriteLine("0x" & kvp.Key.ToString("X16") & ControlChars.Tab & kvp.Value.cValue & ControlChars.Tab & kvp.Value.nValue)
        Next
        outFile.Close()
        outFile.Dispose()
    End Sub
End Class
