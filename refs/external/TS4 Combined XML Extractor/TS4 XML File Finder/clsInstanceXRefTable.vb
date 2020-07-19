Public Class clsInstanceXRef
    Private _Instance As UInt64
    Private _Name As String
    Private _Filename As String = String.Empty
    Friend _ReferenceBy(-1) As UInt64
    Friend _ReferenceTo(-1) As UInt64

    Public Sub New(Instance As UInt64)
        _Instance = Instance
    End Sub

    Friend ReadOnly Property Instance
        Get
            Return _Instance
        End Get
    End Property

    Friend Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Friend Property Filename As String
        Get
            Return _Filename
        End Get
        Set(value As String)
            _Filename = value
        End Set
    End Property

    Friend Sub AddReferenceBy(Instance As UInt64)
        If Not _ReferenceBy.Contains(Instance) Then
            ReDim Preserve _ReferenceBy(_ReferenceBy.Length)
            _ReferenceBy(_ReferenceBy.Length - 1) = Instance
        End If
    End Sub

    Friend Sub AddReferenceTo(Instance As UInt64)
        If Not _ReferenceTo.Contains(Instance) Then
            ReDim Preserve _ReferenceTo(_ReferenceTo.Length)
            _ReferenceTo(_ReferenceTo.Length - 1) = Instance
        End If
    End Sub
End Class

Public Class clsInstanceXRefTable
    Private _IndexedXRefs As SortedList(Of UInt64, clsInstanceXRef)
    Private _Valid As Boolean = False

    Public Sub New(FolderPath As String)
        _IndexedXRefs = New SortedList(Of UInt64, clsInstanceXRef)
        ReadInstanceXRefTable(FolderPath)
    End Sub

    Friend ReadOnly Property IsValid As Boolean
        Get
            Return _Valid
        End Get
    End Property

    Friend Function AddInstance(Instance As UInt64, Name As String) As clsInstanceXRef
        Dim newInstanceXRef As New clsInstanceXRef(Instance)

        If Not _IndexedXRefs.ContainsKey(Instance) Then
            _IndexedXRefs.Add(Instance, newInstanceXRef)
        Else
            newInstanceXRef = _IndexedXRefs.Item(Instance)
        End If

        Return newInstanceXRef
    End Function

    Friend Function GetInstanceXRef(Instance As UInt64) As clsInstanceXRef
        If _IndexedXRefs.ContainsKey(Instance) Then
            Return _IndexedXRefs.Item(Instance)
        End If
        Return Nothing
    End Function

    Private Sub ReadInstanceXRefTable(FolderPath As String)
        Dim inFile As BinaryReader
        Dim fileVersion As UInt64
        Dim Instance As UInt64
        Dim magicNum As UInt64 = &H4DDC4353UL

        _Valid = False
        inFile = New BinaryReader(File.Open(FolderPath & "XML Cross Reference.dat", FileMode.Open, FileAccess.Read))

        magicNum = inFile.ReadUInt64()
        If (magicNum And &HFFFFFFFFUL) <> &H4DDC4353UL Then
            MsgBox("XML Cross Reference.dat - Invalid magic number") '
            Exit Sub
        End If

        fileVersion = magicNum >> 56
        If fileVersion <> 1 Then
            MsgBox("XML Cross Reference.dat - Unknown version error")
            Exit Sub
        End If

        While inFile.BaseStream.Position <> inFile.BaseStream.Length
            Dim idx As Integer
            Dim newInstanceXRef As clsInstanceXRef
            Dim lenArray As Integer

            Instance = inFile.ReadUInt64
            newInstanceXRef = New clsInstanceXRef(Instance)

            lenArray = inFile.ReadUInt32
            ReDim newInstanceXRef._ReferenceBy(lenArray - 1)
            For idx = 0 To lenArray - 1
                newInstanceXRef._ReferenceBy(idx) = inFile.ReadUInt64
            Next

            lenArray = inFile.ReadUInt32
            ReDim newInstanceXRef._ReferenceTo(lenArray - 1)
            For idx = 0 To lenArray - 1
                newInstanceXRef._ReferenceTo(idx) = inFile.ReadUInt64
            Next
            _IndexedXRefs.Add(Instance, newInstanceXRef)
        End While

        inFile.Close()
        inFile.Dispose()
        _Valid = True
    End Sub
End Class
