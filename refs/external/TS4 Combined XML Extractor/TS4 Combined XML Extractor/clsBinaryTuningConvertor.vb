Public Class clsBinaryTuningConvertor
    Inherits clsSimDataFile
    Implements IDisposable

    Private element_count As UInt32
    Private first_element As ObjectRef
    Private first_element_rowNum As Integer
    Private top_element As ObjectRef
    Private string_table As Vector
    Private table_data_offsets() As Long
    Private fileOutput As BinaryWriter = Nothing
    Private include_strings As Boolean = False
    Private include_crossrefs As Boolean = False
    Private stblCombined As clsCombinedStringTable = Nothing
    Private crossrefCombined As clsCombinedXmlNameTable = Nothing
    Private obj_frmMain As frmMain

    ' Table 0 stores the basic info for the packed xml document
    ' Table 1 stores a list of nodes
    '    - nodes prior to the the first_element are text
    '    - the remaining nodes are xml tags
    '    - mValue(0) = offset into child nodes (table 3)
    '    - mValue(1) = row number to get text (via table 5)
    '    - mValue(2) = offset into attribute nodes (table 4)
    ' Table 2 stores a list of row numbers storing attribute value,name pairs (via table 5)
    ' Table 3 stores a list of child nodes
    '    - these are offsets back into table 1
    '    - a RELOFFSET_NULL offset indicates the end of this node's children
    ' Table 4 stores a list of attributes
    '    - these are offsets into table 2
    '    - a RELOFFSET_NULL offset indicates the end of this node's attributes
    ' Table 5 stores a list of offsets pointing to the actual string data (table 6)
    ' Table 6 stores a list of null terminated strings
    '
    ' Offsets are not calculated from a fixed position, but are offsets
    ' from the current file position.

    ' Old style file-based converter
    Public Sub New(Caller As frmMain, filename As String, dest_folder As String, enable_strings As Boolean, enable_crossrefs As Boolean, Optional string_index As clsCombinedStringTable = Nothing, Optional crossref_index As clsCombinedXmlNameTable = Nothing)
        MyBase.New(filename, False)
        If Not _valid Then
            ' Can't convert, but should already have seen an error message, skip
            Exit Sub
        End If

        Dim I As Integer
        Dim out_filename As String = Path.Combine(dest_folder, Path.GetFileNameWithoutExtension(filename) & ".xml")

        obj_frmMain = Caller

        include_strings = enable_strings
        stblCombined = string_index
        include_crossrefs = enable_crossrefs
        crossrefCombined = crossref_index

        ' Precalculate row(0) offset for all tables
        ReDim table_data_offsets(mnNumTables - 1)
        For I = 0 To mnNumTables - 1
            fileInput.Position = mTable(I).startofRows
            SeekToAlignment(15)
            table_data_offsets(I) = fileInput.Position
        Next

        ' Get table(0,0) for Packed XML Document info
        Dim r As Row = GetRow(0, 0)
        element_count = r.mValue(0)
        first_element = r.mValue(1)
        first_element_rowNum = ((r.mValue(1).mDataOffset + r.mValue(1).startofDataOffset) - table_data_offsets(1)) / mTable(1).mnRowSize
        top_element = r.mValue(2)
        string_table = r.mValue(3)

        obj_frmMain.CheckForOverwrite(out_filename)
        fileOutput = New BinaryWriter(File.Open(out_filename, FileMode.Create))

        fileOutput.Write(System.Text.Encoding.UTF8.GetBytes("<?xml version='1.0' encoding='utf-8'?>"))
        WriteNodeAndChildren(r.mValue(2).mDataOffset + r.mValue(2).startofDataOffset)

        fileInput.Close()
        fileOutput.Close()
        fileInput.Dispose()
        fileOutput.Dispose()
        fileInput = Nothing
        fileOutput = Nothing
    End Sub

    ' Memory-stream version of converter
    Public Sub New(Caller As frmMain, memStreamIn As MemoryStream, memStreamOut As MemoryStream, enable_strings As Boolean, enable_crossrefs As Boolean, Optional string_index As clsCombinedStringTable = Nothing, Optional crossref_index As clsCombinedXmlNameTable = Nothing, Optional jazz_mode As Boolean = False)
        MyBase.New(memStreamIn, False, jazz_mode)
        If Not _valid Then
            If jazz_mode Then
                memStreamIn.Position = 0
                memStreamIn.CopyTo(memStreamOut)
                fileInput.Close()
                fileInput.Dispose()
                fileInput = Nothing
            End If
            ' Can't convert, but should already have seen an error message, skip
            Exit Sub
        End If

        Dim I As Integer

        obj_frmMain = Caller

        include_strings = enable_strings
        stblCombined = string_index
        include_crossrefs = enable_crossrefs
        crossrefCombined = crossref_index

        ' Precalculate row(0) offset for all tables
        ReDim table_data_offsets(mnNumTables - 1)
        For I = 0 To mnNumTables - 1
            fileInput.Position = mTable(I).startofRows
            SeekToAlignment(15)
            table_data_offsets(I) = fileInput.Position
        Next

        ' Get table(0,0) for Packed XML Document info
        Dim r As Row = GetRow(0, 0)
        element_count = r.mValue(0)
        first_element = r.mValue(1)
        first_element_rowNum = ((r.mValue(1).mDataOffset + r.mValue(1).startofDataOffset) - table_data_offsets(1)) / mTable(1).mnRowSize
        top_element = r.mValue(2)
        string_table = r.mValue(3)

        fileOutput = New BinaryWriter(memStreamOut)

        fileOutput.Write(System.Text.Encoding.UTF8.GetBytes("<?xml version='1.0' encoding='utf-8'?>"))
        WriteNodeAndChildren(r.mValue(2).mDataOffset + r.mValue(2).startofDataOffset)

        fileInput.Close()
        fileInput.Dispose()
        fileInput = Nothing
        ' Don't close the output, since it's a memory stream created by the caller who still needs it
        fileOutput = Nothing
    End Sub

    ' Get a table row from a table number and row number.
    Private Function GetRow(tblNum As Integer, rowNum As Integer) As Row
        If tblNum >= mnNumTables OrElse rowNum >= mTable(tblNum).mnRowCount Then
            Throw New Exception("Invalid table (" & tblNum & ") or row (" & rowNum & ")")
        End If

        Dim r As Row
        Dim K As Integer
        Dim schemaIndex As Integer
        Dim rowStart As Long

        fileInput.Position = table_data_offsets(tblNum) + (rowNum * mTable(tblNum).mnRowSize)

        If mTable(tblNum).mnSchemaOffset = RELOFFSET_NULL Then
            ReDim r.mValue(0)
            r.mValue(0) = ReadData(mTable(tblNum).mnDataType)
        Else
            schemaIndex = GetSchemaIndex(mTable(tblNum).startofSchemaOffset + mTable(tblNum).mnSchemaOffset)
            ReDim r.mValue(mSchema(schemaIndex).mnNumColumns - 1)
            rowStart = fileInput.Position
            For K = 0 To mSchema(schemaIndex).mnNumColumns - 1
                fileInput.Position = rowStart + mSchema(schemaIndex).mColumn(K).mnOffset
                r.mValue(K) = ReadData(mSchema(schemaIndex).mColumn(K).mnDataType)
            Next
            fileInput.Position = rowStart + mSchema(schemaIndex).mnSchemaSize
        End If

        Return r
    End Function

    ' Get a row from an offset into the file.  We need to get the table number
    ' so we can process the schema.
    Private Function GetRow(rowPos As Long) As Row
        Dim tblNum As Integer = GetTableFromOffset(rowPos)
        Dim r As Row
        Dim K As Integer
        Dim schemaIndex As Integer
        Dim rowStart As Long

        fileInput.Position = rowPos

        If mTable(tblNum).mnSchemaOffset = RELOFFSET_NULL Then
            ReDim r.mValue(0)
            r.mValue(0) = ReadData(mTable(tblNum).mnDataType)
        Else
            schemaIndex = GetSchemaIndex(mTable(tblNum).startofSchemaOffset + mTable(tblNum).mnSchemaOffset)
            ReDim r.mValue(mSchema(schemaIndex).mnNumColumns - 1)
            rowStart = fileInput.Position
            For K = 0 To mSchema(schemaIndex).mnNumColumns - 1
                fileInput.Position = rowStart + mSchema(schemaIndex).mColumn(K).mnOffset
                r.mValue(K) = ReadData(mSchema(schemaIndex).mColumn(K).mnDataType)
            Next
            fileInput.Position = rowStart + mSchema(schemaIndex).mnSchemaSize
        End If

        Return r
    End Function

    ' Figure out what table number our offset points into
    Private Function GetTableFromOffset(offset As Long) As Integer
        Dim I As Integer

        For I = 0 To mnNumTables - 1
            If table_data_offsets(I) <= offset Then
                If offset < table_data_offsets(I) + (mTable(I).mnRowSize * mTable(I).mnRowCount) Then
                    Return I
                End If
            End If
        Next

        Throw New Exception("Unable to identify table from offset=" & offset)
    End Function

    ' Write a tag and attributes from a table 1 row and process any child nodes
    Private Sub WriteNodeAndChildren(offset As Long)
        Dim r As Row = GetRow(offset)
        Dim child_pos As Int32 = SignedOffset(r.mValue(0).mDataOffset)
        Dim text_row As Integer
        Dim attrs_pos As Int32 = SignedOffset(r.mValue(2).mDataOffset)
        Dim Tag As Byte()
        Dim Comment As String = String.Empty

        If child_pos <> RELOFFSET_NULL Then
            child_pos += r.mValue(0).startofDataOffset
        End If
        text_row = r.mValue(1)
        If attrs_pos <> RELOFFSET_NULL Then
            attrs_pos += r.mValue(2).startofDataOffset
        End If

        Tag = GetText(text_row)

        If child_pos = RELOFFSET_NULL And attrs_pos = RELOFFSET_NULL Then
            If (offset - table_data_offsets(1)) / mTable(1).mnRowSize < first_element_rowNum Then
                fileOutput.Write(Tag)
                If include_strings And Tag(0) >= 48 And Tag(0) <= 57 Then
                    ' First byte is digit
                    Dim Parsed As Boolean = False
                    Dim Val As UInt32
                    If Tag.Length > 2 AndAlso (Tag(1) = 88 Or Tag(1) = 120) AndAlso Tag(0) = 48 Then
                        ' 0x - This appears to be a hex value
                        Parsed = UInt32.TryParse(System.Text.Encoding.UTF8.GetString(Tag, 2, Tag.Length - 2), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.CurrentCulture, Val)
                    Else
                        ' Might be an integer value, which is valid though unusual
                        Parsed = UInt32.TryParse(System.Text.Encoding.UTF8.GetString(Tag), Globalization.NumberStyles.Integer, Globalization.CultureInfo.CurrentCulture, Val)
                    End If
                    If Parsed Then
                        Dim strTmp As String = stblCombined.GetString(Val)
                        If strTmp IsNot Nothing Then
                            While strTmp.Contains("--")
                                ' XML Comments may not contain --
                                strTmp = strTmp.Replace("--", "-")
                            End While
                            Comment = "<!--String: """ & strTmp & """-->"
                            fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(Comment))
                        End If
                    End If
                End If
            Else
                fileOutput.Write(System.Text.Encoding.UTF8.GetBytes("<"))
                fileOutput.Write(Tag)
                fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(" />"))
            End If
            Exit Sub
        End If

        fileOutput.Write(System.Text.Encoding.UTF8.GetBytes("<"))
        fileOutput.Write(Tag)
        If attrs_pos <> RELOFFSET_NULL Then
            WriteAttrs(attrs_pos)
        End If
        If child_pos <> RELOFFSET_NULL Then
            fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(">"))
            WriteChildren(child_pos)
            fileOutput.Write(System.Text.Encoding.UTF8.GetBytes("</"))
            fileOutput.Write(Tag)
            fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(">"))
        Else
            fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(" />"))
        End If
    End Sub

    ' Get all the child nodes for this node from table 3, RELOFFSET_NULL is used to indicate
    ' that no more child nodes exist for the node.
    Private Function WriteChildren(offset As Long) As String
        Dim XmlStub As String = String.Empty
        Dim r As Row = GetRow(offset)
        Dim next_offset As Long
        Dim signed_offset = SignedOffset(r.mValue(0).mDataOffset)

        While signed_offset <> RELOFFSET_NULL
            next_offset = fileInput.Position
            WriteNodeAndChildren(SignedOffset(r.mValue(0).mDataOffset) + r.mValue(0).startofDataOffset)
            r = GetRow(next_offset)
            signed_offset = SignedOffset(r.mValue(0).mDataOffset)
        End While

        Return XmlStub
    End Function

    ' Write all the attributes for this node from table 4, RELOFFSET_NULL is used to indicate
    ' that no more attributes exist for the node.
    Private Sub WriteAttrs(offset As Long)
        Dim r As Row = GetRow(offset)
        Dim next_offset As Long
        Dim signed_offset = SignedOffset(r.mValue(0).mDataOffset)
        Dim name() As Byte
        Dim value() As Byte
        Dim n() As Byte = Nothing
        Dim s() As Byte = Nothing
        Dim c() As Byte = Nothing

        While signed_offset <> RELOFFSET_NULL
            next_offset = fileInput.Position
            Dim r1 As Row = GetRow(SignedOffset(r.mValue(0).mDataOffset) + r.mValue(0).startofDataOffset)
            name = GetText(r1.mValue(1))
            value = GetText(r1.mValue(0))
            If include_crossrefs AndAlso name.Length = 1 Then
                If name(0) = &H6E Then
                    n = value
                ElseIf name(0) = &H73 Then
                    s = value
                ElseIf name(0) = &H63 Then
                    c = value
                End If
            End If
            WriteAttrPair(name, value)
            r = GetRow(next_offset)
            signed_offset = SignedOffset(r.mValue(0).mDataOffset)
        End While
        If n IsNot Nothing AndAlso s IsNot Nothing Then
            ' We have n/s attributes to index (c is optional)
            crossrefCombined.AddXmlName(n, s, c)
            End If
    End Sub

    ' Retrieve and write the attribute name/value pair from a table 2 row
    Private Sub WriteAttrPair(name() As Byte, value() As Byte)
        fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(" "))
        fileOutput.Write(name)
        fileOutput.Write(System.Text.Encoding.UTF8.GetBytes("="""))
        fileOutput.Write(value)
        fileOutput.Write(System.Text.Encoding.UTF8.GetBytes(""""))
    End Sub

    ' Get a text value from a row number index into table 5
    Private Function GetText(rowNum As Integer) As Byte()
        Dim r As Row = GetRow(5, rowNum)
        Dim text_offset As Long
        Dim t As Byte()

        text_offset = r.mValue(0).mDataOffset + r.mValue(0).startofDataOffset
        t = GetNullTerminatedByteString(text_offset)
        Return t
    End Function

    ' Some of the offsets are stored in an unsigned int, but are actually signed
    ' Convert them to signed ints
    Private Function SignedOffset(offset As Long) As Int32
        Return BitConverter.ToInt32(BitConverter.GetBytes(offset), 0)
    End Function

    Private staticBuffer(1024) As Byte

    ' We have to read the strings as actual bytes as there may be international characters
    Protected Function GetNullTerminatedByteString(offset As Long) As Byte()
        Dim len As Integer = -1
        Dim chr As Byte
        Dim buffer(-1) As Byte

        fileInput.Position = offset
        chr = fileInput.ReadByte
        Do While chr <> 0
            len += 1
            'Array.Resize(buffer, len + 1)
            If chr = Asc("&"c) Then
                'Array.Resize(buffer, len + 5)
                Array.Copy(System.Text.Encoding.UTF8.GetBytes("&amp;"), 0, staticBuffer, len, 5)
                len += 4
            ElseIf chr = Asc("'"c) Then
                'Array.Resize(buffer, len + 6)
                Array.Copy(System.Text.Encoding.UTF8.GetBytes("&apos;"), 0, staticBuffer, len, 6)
                len += 5
            Else
                staticBuffer(len) = chr
            End If
            chr = fileInput.ReadByte
        Loop

        If len >= 0 Then
            ReDim buffer(len)
            Array.Copy(staticBuffer, 0, buffer, 0, len + 1)
        End If
        Return buffer
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If fileInput IsNot Nothing Then
                    fileInput.Close()
                    fileInput.Dispose()
                End If
                If fileOutput IsNot Nothing Then
                    fileOutput.Close()
                    fileOutput.Dispose()
                End If
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
