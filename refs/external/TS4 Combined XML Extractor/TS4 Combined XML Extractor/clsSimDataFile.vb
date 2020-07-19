Public Class clsSimDataFile
    Protected fileInput As MemoryStream

    Structure TableInfo
        Dim mnNameOffset As Int32
        Dim mnNameHash As UInt32
        Dim mnSchemaOffset As Int32
        Dim mnDataType As UInt32
        Dim mnRowSize As UInt32
        Dim mnRowOffset As Int32
        Dim mnRowCount As UInt32
        Dim mnName As String

        Dim startofSchemaOffset As Long
        Dim startofRows As Long
    End Structure

    Structure SchemaColumn
        Dim mnNameOffset As Int32
        Dim mnNameHash As UInt32
        Dim mnDataType As UInt16
        Dim mnFlags As UInt16
        Dim mnOffset As UInt32
        Dim mnSchemaOffset As Int32
        Dim mnName As String
    End Structure

    Structure Schema
        Dim mnNameOffset As Int32
        Dim mnNameHash As UInt32
        Dim mnSchemaHash As UInt32
        Dim mnSchemaSize As UInt32
        Dim mnColumnOffset As Int32
        Dim mnNumColumns As UInt32
        Dim mColumn() As SchemaColumn
        Dim mnName As String

        Dim startofSchemaPos As Long
    End Structure

    Structure Vector2
        Dim x As Single
        Dim y As Single
    End Structure

    Structure Vector3
        Dim x As Single
        Dim y As Single
        Dim z As Single
    End Structure

    Structure Vector4
        Dim x As Single
        Dim y As Single
        Dim z As Single
        Dim w As Single
    End Structure

    Structure StringRef
        Dim mDataOffset As UInt32
        Dim startofDataOffset As Long
    End Structure

    Structure HashedStringRef
        Dim mDataOffset As UInt32
        Dim mHash As UInt32
        Dim startofDataOffset As Long
    End Structure

    Structure LocKey
        Dim mKey As UInt32
    End Structure

    Structure ResourceKey
        Dim mInstance As UInt64
        Dim mType As UInt32
        Dim mGroup As UInt32
    End Structure

    Structure ObjectRef
        Dim mDataOffset As UInt32
        Dim startofDataOffset As Long
    End Structure

    Structure TableSetRef
        Dim mValue As UInt64
    End Structure

    Structure Vector
        Dim mDataOffset As UInt32
        Dim mCount As UInt32
        Dim startofDataOffset As Long
    End Structure

    Structure Row
        Dim mValue() As Object
    End Structure

    Structure TableData
        Dim mRow() As Row
    End Structure

    Friend Enum DataType As Integer
        TYPE_BOOL
        TYPE_CHAR8
        TYPE_INT8
        TYPE_UINT8
        TYPE_INT16
        TYPE_UINT16
        TYPE_INT32
        TYPE_UINT32
        TYPE_INT64
        TYPE_UINT64
        TYPE_FLOAT
        TYPE_STRING8
        TYPE_HASHEDSTRING8
        TYPE_OBJECT
        TYPE_VECTOR
        TYPE_FLOAT2
        TYPE_FLOAT3
        TYPE_FLOAT4
        TYPE_TABLESETREFERENCE
        TYPE_RESOURCEKEY
        TYPE_LOCKEY
        TYPE_UNDEFINED
    End Enum

    Protected mnFileIdentifier As String
    Protected mnVersion As UInt32
    Protected mnTableHeaderOffset As UInt32
    Protected mnNumTables As Int32
    Protected mnSchemaOffset As Int32
    Protected mnNumSchemas As Int32
    Protected mTable() As TableInfo
    Protected mSchema() As Schema
    Protected mStringTable() As String
    Protected mTableData() As TableData

    Protected nTableHeaderPos As Long
    Protected nSchemaPos As Long
    Protected lastColumnEndPos As Long

    ' Zero can be a valid offset, so use 0x8000000 to represent
    ' a NULL relative offset.
    Friend Const RELOFFSET_NULL As Int32 = &H80000000

    Protected _valid As Boolean = False
    Protected _isJazzXml As Boolean = False

    ' Old-style file-based version of SimData reader
    Public Sub New(filename As String, loadData As Boolean)
        Dim I As Integer
        Dim rowDataPos As Long
        Dim fsStream As FileStream

        fsStream = File.Open(filename, FileMode.Open, FileAccess.Read)
        fileInput = New MemoryStream
        fsStream.CopyTo(fileInput)
        fileInput.Position = 0

        ' Adapted from the SimdataTemplate provided by EA

        ' Header
        ' There are a few unused or deprecated fields in the header. Most of these
        ' should be set to 0 unless marked otherwise.

        mnFileIdentifier = GetString(4)
        If String.Compare(mnFileIdentifier, "DATA") <> 0 Then
            If String.Compare(mnFileIdentifier, "<com") = 0 Then
                MsgBox("WARNING: A binary data file appears to already be a valid combined XML file.  This may be normal if your game is not fully patched, or if extracting from a SimulationFullBuild for another reason." & ControlChars.CrLf & ControlChars.CrLf & "Renaming as XML: " & filename, MsgBoxStyle.Information)
                fileInput.Close()
                Try
                    My.Computer.FileSystem.RenameFile(filename, Path.GetFileNameWithoutExtension(filename) & ".xml")
                Catch ex As Exception
                    Try
                        My.Computer.FileSystem.RenameFile(filename, Path.GetFileName(filename) & ".xml")
                    Catch ex2 As Exception
                        MsgBox("Unable to rename file, skipping.", MsgBoxStyle.Exclamation)
                    End Try
                End Try
                Exit Sub
            Else
                MsgBox("Input file is not a valid binary tuning file, skipping: " & filename, MsgBoxStyle.Exclamation)
                fileInput.Close()
                Exit Sub
            End If
        End If

        ' Base game version is 0x101 (257)
        mnVersion = GetUInt32()
        If mnVersion < 256 Or mnVersion > 257 Then
            MsgBox("Binary tuning file is an unknown version, skipping: " & filename, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        ' Offset of table header data
        nTableHeaderPos = fileInput.Position
        mnTableHeaderOffset = GetUInt32()

        ' Number of table headers
        mnNumTables = GetInt32()

        ' Offset of schema data
        nSchemaPos = fileInput.Position
        mnSchemaOffset = GetInt32()

        ' Number of schemas
        mnNumSchemas = GetInt32()

        If mnVersion >= 257 Then
            Dim mnUnused = GetUInt32()
        End If

        ' Skip to the beginning of the table header block
        fileInput.Position = nTableHeaderPos + mnTableHeaderOffset

        ' Load the tables
        ReDim mTable(mnNumTables - 1)
        For I = 0 To mnNumTables - 1
            mTable(I) = GetTable()
        Next

        ' We need to read schemas before we can load data, so
        ' just remember the position at which row data starts.
        rowDataPos = fileInput.Position

        ' Read schemas.
        fileInput.Position = nSchemaPos + mnSchemaOffset
        ReDim mSchema(mnNumSchemas - 1)
        For I = 0 To mnNumSchemas - 1
            mSchema(I) = GetSchema()
        Next

        ' Now jump past the columns that were read in the last schema
        fileInput.Position = lastColumnEndPos

        ' Read the string table, which consists of NULL separated
        ' UTF-8 encoded strings, and comprises the rest of the file.
        ReDim mStringTable(-1)
        While fileInput.Position < fileInput.Length
            Array.Resize(mStringTable, mStringTable.Length + 1)
            mStringTable(mStringTable.Length - 1) = GetNullTerminatedString()
        End While

        If loadData Then
            ' Now, we can jump back and read data.
            ReDim mTableData(mnNumTables - 1)
            fileInput.Position = rowDataPos
            For I = 0 To mnNumTables - 1
                mTableData(I) = GetTableData(I)
            Next

            fileInput.Close()
        End If

        _valid = True
    End Sub

    ' Memory-stream version of SimData reader
    Public Sub New(memStream As MemoryStream, loadData As Boolean, Optional jazz_mode As Boolean = False)
        Dim I As Integer
        Dim rowDataPos As Long

        'fileInput = File.Open(filename, FileMode.Open, FileAccess.Read)
        fileInput = memStream
        fileInput.Position = 0

        ' Adapted from the SimdataTemplate provided by EA

        ' Header
        ' There are a few unused or deprecated fields in the header. Most of these
        ' should be set to 0 unless marked otherwise.

        mnFileIdentifier = GetString(4)
        If String.Compare(mnFileIdentifier, "DATA") <> 0 Then
            If Not jazz_mode Then
                ' We can't rename a memory stream to make it readable as an XML file like we do
                ' in the file-based version above.  Not worried enough about it to implement this,
                ' so just skip it.  This shouldn't happen unless someone has a REALLY old unpatched game.
                MsgBox("Input file is not a valid binary tuning file, skipping.", MsgBoxStyle.Exclamation)
                fileInput.Close()
                Exit Sub
            Else
                _isJazzXml = True
                Exit Sub
            End If
        End If
        'End If

        ' Base game version is 0x101 (257)
        mnVersion = GetUInt32()
        If mnVersion < 256 Or mnVersion > 257 Then
            MsgBox("Binary tuning file is an unknown version, skipping.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        ' Offset of table header data
        nTableHeaderPos = fileInput.Position
        mnTableHeaderOffset = GetUInt32()

        ' Number of table headers
        mnNumTables = GetInt32()

        ' Offset of schema data
        nSchemaPos = fileInput.Position
        mnSchemaOffset = GetInt32()

        ' Number of schemas
        mnNumSchemas = GetInt32()

        If mnVersion >= 257 Then
            Dim mnUnused = GetUInt32()
        End If

        ' Skip to the beginning of the table header block
        fileInput.Position = nTableHeaderPos + mnTableHeaderOffset

        ' Load the tables
        ReDim mTable(mnNumTables - 1)
        For I = 0 To mnNumTables - 1
            mTable(I) = GetTable()
        Next

        ' We need to read schemas before we can load data, so
        ' just remember the position at which row data starts.
        rowDataPos = fileInput.Position

        ' Read schemas.
        fileInput.Position = nSchemaPos + mnSchemaOffset
        ReDim mSchema(mnNumSchemas - 1)
        For I = 0 To mnNumSchemas - 1
            mSchema(I) = GetSchema()
        Next

        ' Now jump past the columns that were read in the last schema
        fileInput.Position = lastColumnEndPos

        ' Read the string table, which consists of NULL separated
        ' UTF-8 encoded strings, and comprises the rest of the file.
        ReDim mStringTable(-1)
        While fileInput.Position < fileInput.Length
            Array.Resize(mStringTable, mStringTable.Length + 1)
            mStringTable(mStringTable.Length - 1) = GetNullTerminatedString()
        End While

        If loadData Then
            ' Now, we can jump back and read data.
            ReDim mTableData(mnNumTables - 1)
            fileInput.Position = rowDataPos
            For I = 0 To mnNumTables - 1
                mTableData(I) = GetTableData(I)
            Next
            ' Don't close the memory stream, we have no idea what the caller wants to do with it
            ' (loadData feature not used in extractor anyway)
        End If

        _valid = True
    End Sub

    Friend Function GetData(table As Integer, row As Integer, col As Integer) As Object
        Return mTableData(table).mRow(row).mValue(col)
    End Function

    Protected Function GetBytes(offset As Integer, len As Integer) As Byte()
        Dim buffer(len - 1) As Byte

        fileInput.Position = offset
        If fileInput.Read(buffer, 0, len) <> len Then
            Throw New Exception("Unexpected end of file reading data")
            Return Nothing
        End If

        Return buffer
    End Function

    Protected Function GetBytes(len As Integer) As Byte()
        Dim buffer(len - 1) As Byte

        If fileInput.Read(buffer, 0, len) <> len Then
            Throw New Exception("Unexpected end of file reading data")
            Return Nothing
        End If

        Return buffer
    End Function

    Protected Function GetString(offset As Long, len As Integer) As String
        Dim buffer() As Byte = GetBytes(offset, len)
        Dim strData As String = System.Text.Encoding.UTF8.GetString(buffer)

        Return strData
    End Function

    Protected Function GetString(len As Integer) As String
        Dim buffer() As Byte = GetBytes(len)
        Dim strData As String = System.Text.Encoding.UTF8.GetString(buffer)

        Return strData
    End Function

    Protected Function GetNullTerminatedString() As String
        Dim str As String = String.Empty
        Dim buffer(0) As Byte

        Do
            buffer(0) = fileInput.ReadByte
            str = str & System.Text.Encoding.UTF8.GetString(buffer)(0)
        Loop While buffer(0) <> 0

        Return str
    End Function

    ' This really doesn't work too well if there's unicode characters in the string
    Protected Function GetNullTerminatedString(offset As Long) As String
        Dim str As String = String.Empty
        Dim buffer(0) As Byte

        fileInput.Position = offset
        Do
            buffer(0) = fileInput.ReadByte
            str = str & System.Text.Encoding.UTF8.GetString(buffer)(0)
        Loop While buffer(0) <> 0

        Return str
    End Function

    Protected Function GetUInt32(offset As Long) As UInt32
        Dim buffer() As Byte = GetBytes(offset, 4)
        Dim value As UInt32 = BitConverter.ToUInt32(buffer, 0)

        Return value
    End Function

    Protected Function GetUInt32() As UInt32
        Dim buffer() As Byte = GetBytes(4)
        Dim value As UInt32 = BitConverter.ToUInt32(buffer, 0)

        Return value
    End Function

    Protected Function GetInt32(offset As Long) As Int32
        Dim buffer() As Byte = GetBytes(offset, 4)
        Dim value As Int32 = BitConverter.ToInt32(buffer, 0)

        Return value
    End Function

    Protected Function GetInt32() As Int32
        Dim buffer() As Byte = GetBytes(4)
        Dim value As Int32 = BitConverter.ToInt32(buffer, 0)

        Return value
    End Function

    Protected Function GetUInt16() As UInt16
        Dim buffer() As Byte = GetBytes(2)
        Dim value As UInt16 = BitConverter.ToUInt16(buffer, 0)

        Return value
    End Function

    Protected Function GetInt16() As Int16
        Dim buffer() As Byte = GetBytes(2)
        Dim value As Int16 = BitConverter.ToInt16(buffer, 0)

        Return value
    End Function

    Protected Function GetUByte() As Byte
        Return fileInput.ReadByte()
    End Function

    Protected Function GetSByte() As SByte
        Dim byt As Byte = GetUByte()
        Dim value As SByte = byt - ((byt And &H80) << 1)

        Return value
    End Function

    Protected Function GetChar() As Char
        Dim buffer As Byte
        Dim value As Char

        buffer = fileInput.ReadByte()
        value = Convert.ToChar(buffer)

        Return value
    End Function

    Protected Function GetInt64() As Int64
        Dim buffer() As Byte = GetBytes(8)
        Dim value As Int64 = BitConverter.ToInt64(buffer, 0)

        Return value
    End Function

    Private Function GetUInt64() As UInt64
        Dim buffer() As Byte = GetBytes(8)
        Dim value As UInt64 = BitConverter.ToUInt64(buffer, 0)

        Return value
    End Function

    Protected Function GetFloat() As Single
        Dim buffer() As Byte = GetBytes(4)
        Dim value As Single = BitConverter.ToSingle(buffer, 0)

        Return value
    End Function

    Protected Function GetString8() As StringRef
        Dim value As StringRef
        value.startofDataOffset = fileInput.Position

        Dim buffer() As Byte = GetBytes(4)
        value.mDataOffset = BitConverter.ToUInt32(buffer, 0)

        Return value
    End Function

    Protected Function GetHashedString8() As HashedStringRef
        Dim value As HashedStringRef
        value.startofDataOffset = fileInput.Position

        Dim buffer() As Byte = GetBytes(8)
        value.mDataOffset = BitConverter.ToUInt32(buffer, 0)
        value.mHash = BitConverter.ToUInt32(buffer, 4)

        Return value
    End Function

    Protected Function GetObject() As ObjectRef
        Dim value As ObjectRef
        value.startofDataOffset = fileInput.Position

        Dim buffer() As Byte = GetBytes(4)
        value.mDataOffset = BitConverter.ToUInt32(buffer, 0)

        Return value
    End Function

    Protected Function GetVector() As Vector
        Dim value As Vector
        value.startofDataOffset = fileInput.Position

        Dim buffer() As Byte = GetBytes(8)
        value.mDataOffset = BitConverter.ToUInt32(buffer, 0)
        value.mCount = BitConverter.ToUInt32(buffer, 4)

        Return value
    End Function

    Protected Function GetVector2() As Vector2
        Dim buffer() As Byte = GetBytes(8)
        Dim value As Vector2

        value.x = BitConverter.ToSingle(buffer, 0)
        value.y = BitConverter.ToSingle(buffer, 4)

        Return value
    End Function

    Protected Function GetVector3() As Vector3
        Dim buffer() As Byte = GetBytes(12)
        Dim value As Vector3

        value.x = BitConverter.ToSingle(buffer, 0)
        value.y = BitConverter.ToSingle(buffer, 4)
        value.z = BitConverter.ToSingle(buffer, 8)

        Return value
    End Function

    Protected Function GetVector4() As Vector4
        Dim buffer() As Byte = GetBytes(16)
        Dim value As Vector4

        value.x = BitConverter.ToSingle(buffer, 0)
        value.y = BitConverter.ToSingle(buffer, 4)
        value.z = BitConverter.ToSingle(buffer, 8)
        value.w = BitConverter.ToSingle(buffer, 12)

        Return value
    End Function

    Protected Function GetTableSetRef() As TableSetRef
        Dim buffer() As Byte = GetBytes(8)
        Dim value As TableSetRef

        value.mValue = BitConverter.ToUInt64(buffer, 0)

        Return value
    End Function

    Protected Function GetResourceKey() As ResourceKey
        Dim buffer() As Byte = GetBytes(16)
        Dim value As ResourceKey

        value.mInstance = BitConverter.ToUInt64(buffer, 0)
        value.mType = BitConverter.ToUInt32(buffer, 8)
        value.mGroup = BitConverter.ToUInt32(buffer, 12)

        Return value
    End Function

    Protected Function GetLocKey() As LocKey
        Dim buffer() As Byte = GetBytes(4)
        Dim value As LocKey

        value.mKey = BitConverter.ToUInt32(buffer, 0)

        Return value
    End Function

    Protected Sub SeekToAlignment(alignmentMask As UInt32)
        Dim nCurPos As Int32 = fileInput.Position
        Dim nPadAmount As Int32 = -nCurPos And alignmentMask
        fileInput.Position = nCurPos + nPadAmount
    End Sub

    Private Function GetTable() As TableInfo
        Dim tTable As New TableInfo
        Dim startofNameOffset As Long = fileInput.Position
        Dim tableEndPos As Long

        tTable.mnNameOffset = GetInt32()
        tTable.mnNameHash = GetUInt32()
        tTable.startofSchemaOffset = fileInput.Position
        tTable.mnSchemaOffset = GetInt32()
        tTable.mnDataType = GetUInt32()
        tTable.mnRowSize = GetUInt32()
        tTable.startofRows = fileInput.Position
        tTable.mnRowOffset = GetInt32()
        tTable.startofRows += tTable.mnRowOffset
        tTable.mnRowCount = GetUInt32()

        ' Get the name
        If tTable.mnNameOffset <> RELOFFSET_NULL Then
            tableEndPos = fileInput.Position
            fileInput.Position = startofNameOffset + tTable.mnNameOffset
            tTable.mnName = GetNullTerminatedString()
            fileInput.Position = tableEndPos
        Else
            tTable.mnName = "Unnamed"
        End If

        Return tTable
    End Function

    Private Function GetSchema() As Schema
        Dim tSchema As New Schema
        Dim startofNameOffset As Long = fileInput.Position
        Dim startofColumnOffset As Long
        Dim schemaEndPos As Long
        Dim I As Integer

        ' Dim tColumn As SchemaColumn
        tSchema.startofSchemaPos = fileInput.Position
        tSchema.mnNameOffset = GetInt32()
        tSchema.mnNameHash = GetUInt32()
        tSchema.mnSchemaHash = GetUInt32()
        tSchema.mnSchemaSize = GetUInt32()
        startofColumnOffset = fileInput.Position
        tSchema.mnColumnOffset = GetInt32()
        tSchema.mnNumColumns = GetUInt32()
        schemaEndPos = fileInput.Position

        ' Load the schema columns inside the schema
        ReDim tSchema.mColumn(tSchema.mnNumColumns - 1)
        fileInput.Position = startofColumnOffset + tSchema.mnColumnOffset
        For I = 0 To tSchema.mnNumColumns - 1
            tSchema.mColumn(I) = GetSchemaColumn()
        Next

        ' Remember the end of the final column
        lastColumnEndPos = fileInput.Position

        ' Get the name
        If tSchema.mnNameOffset <> RELOFFSET_NULL Then
            fileInput.Position = startofNameOffset + tSchema.mnNameOffset
            tSchema.mnName = GetNullTerminatedString()
        Else
            tSchema.mnName = "Unnamed"
        End If

        ' Rewind back to the end of the schema to prepare for next
        fileInput.Position = schemaEndPos

        Return tSchema
    End Function

    Private Function GetSchemaColumn() As SchemaColumn
        Dim tSchemaColumn As New SchemaColumn
        Dim startofNameOffset As Long = fileInput.Position
        Dim schemacolumnEndPos As Long

        tSchemaColumn.mnNameOffset = GetInt32()
        tSchemaColumn.mnNameHash = GetUInt32()
        tSchemaColumn.mnDataType = GetUInt16()
        tSchemaColumn.mnFlags = GetUInt16()
        tSchemaColumn.mnOffset = GetUInt32()
        tSchemaColumn.mnSchemaOffset = GetInt32()

        ' Get the name
        If tSchemaColumn.mnNameOffset <> RELOFFSET_NULL Then
            schemacolumnEndPos = fileInput.Position
            fileInput.Position = startofNameOffset + tSchemaColumn.mnNameOffset
            tSchemaColumn.mnName = GetNullTerminatedString()
            fileInput.Position = schemacolumnEndPos
        Else
            tSchemaColumn.mnName = "Unnamed"
        End If

        Return tSchemaColumn
    End Function

    Protected Function GetAlignmentForType(typeCode As Int32) As UInt32
        Select Case typeCode
            Case DataType.TYPE_BOOL
                Return 1
            Case DataType.TYPE_CHAR8
                Return 1
            Case DataType.TYPE_INT8
                Return 1
            Case DataType.TYPE_UINT8
                Return 1
            Case DataType.TYPE_INT16
                Return 2
            Case DataType.TYPE_UINT16
                Return 2
            Case DataType.TYPE_INT32
                Return 4
            Case DataType.TYPE_UINT32
                Return 4
            Case DataType.TYPE_INT64
                Return 8
            Case DataType.TYPE_UINT64
                Return 8
            Case DataType.TYPE_FLOAT
                Return 4
            Case DataType.TYPE_STRING8
                Return 4
            Case DataType.TYPE_HASHEDSTRING8
                Return 4
            Case DataType.TYPE_OBJECT
                Return 4
            Case DataType.TYPE_VECTOR
                Return 4
            Case DataType.TYPE_FLOAT2
                Return 4
            Case DataType.TYPE_FLOAT3
                Return 4
            Case DataType.TYPE_FLOAT4
                Return 4
            Case DataType.TYPE_TABLESETREFERENCE
                Return 8
            Case DataType.TYPE_RESOURCEKEY
                Return 8
            Case DataType.TYPE_LOCKEY
                Return 4
            Case DataType.TYPE_UNDEFINED
                Return 1
            Case Else
                Return 1
        End Select
    End Function

    Protected Function ReadData(typeCode As Int32) As Object
        Select Case typeCode
            Case DataType.TYPE_BOOL
                Return GetUByte()
            Case DataType.TYPE_UINT8
                Return GetUByte()
            Case DataType.TYPE_CHAR8
                Return GetChar()
            Case DataType.TYPE_INT8
                Return GetSByte()
            Case DataType.TYPE_INT16
                Return GetInt16()
            Case DataType.TYPE_UINT16
                Return GetUInt16()
            Case DataType.TYPE_INT32
                Return GetInt32()
            Case DataType.TYPE_UINT32
                Return GetUInt32()
            Case DataType.TYPE_INT64
                Return GetInt64()
            Case DataType.TYPE_UINT64
                Return GetUInt64()
            Case DataType.TYPE_FLOAT
                Return GetFloat()
            Case DataType.TYPE_STRING8
                Return GetString8()
            Case DataType.TYPE_HASHEDSTRING8
                Return GetHashedString8()
            Case DataType.TYPE_OBJECT
                Return GetObject()
            Case DataType.TYPE_VECTOR
                Return GetVector()
            Case DataType.TYPE_FLOAT2
                Return GetVector2()
            Case DataType.TYPE_FLOAT3
                Return GetVector3()
            Case DataType.TYPE_FLOAT4
                Return GetVector4()
            Case DataType.TYPE_TABLESETREFERENCE
                Return GetTableSetRef()
            Case DataType.TYPE_RESOURCEKEY
                Return GetResourceKey()
            Case DataType.TYPE_LOCKEY
                Return GetLocKey()
            Case DataType.TYPE_UNDEFINED
                MsgBox("Unknown type code " & typeCode)
            Case Else
                Throw New Exception("Invalid data type in binary table: " & typeCode)
        End Select

        Return Nothing
    End Function

    Protected Function GetSchemaIndex(offset As Long) As Integer
        Dim I As Integer = 0

        For Each tSchema In mSchema
            If tSchema.startofSchemaPos = offset Then
                Return I
            End If
            I += 1
        Next
        Throw New Exception("Unable to locate schema index in mSchema")
    End Function

    Protected Function GetTableData(I As Integer) As TableData
        Dim tTableData As New TableData
        Dim alignment
        Dim J, K As Integer
        Dim schemaIndex As Integer
        Dim rowStart As Long
        Dim columnAlignment As Integer

        ' Note that the start of the row data must be aligned (mask == 15).
        SeekToAlignment(15)
        alignment = 1
        ReDim tTableData.mRow(mTable(I).mnRowCount - 1)
        For J = 0 To mTable(I).mnRowCount - 1
            ' Some tables have no schema; these support only one
            ' data type.
            If mTable(I).mnSchemaOffset = RELOFFSET_NULL Then
                ReDim tTableData.mRow(J).mValue(0)
                tTableData.mRow(J).mValue(0) = ReadData(mTable(I).mnDataType)
                alignment = GetAlignmentForType(mTable(I).mnDataType)
            Else
                schemaIndex = GetSchemaIndex(mTable(I).startofSchemaOffset + mTable(I).mnSchemaOffset)
                ReDim tTableData.mRow(J).mValue(mSchema(schemaIndex).mnNumColumns - 1)
                rowStart = fileInput.Position
                ' Read each column.  The order of column data does not match the column
                ' order (columns are sorted by name hash).
                For K = 0 To mSchema(schemaIndex).mnNumColumns - 1
                    'schemaColumnName = mSchema(schemaIndex).mColumn(K).mnName
                    fileInput.Position = rowStart + mSchema(schemaIndex).mColumn(K).mnOffset
                    tTableData.mRow(J).mValue(K) = ReadData(mSchema(schemaIndex).mColumn(K).mnDataType)
                    columnAlignment = GetAlignmentForType(mTable(I).mnDataType)
                    If columnAlignment > alignment Then
                        alignment = columnAlignment
                    End If
                Next
                ' Reading the rows will modify the file position.  We need to save the current row start
                ' and then manually increment the position after loading the column data.
                fileInput.Position = rowStart + mSchema(schemaIndex).mnSchemaSize
            End If
            SeekToAlignment(alignment - 1)
        Next

        SeekToAlignment(15)

        Return tTableData
    End Function
End Class
