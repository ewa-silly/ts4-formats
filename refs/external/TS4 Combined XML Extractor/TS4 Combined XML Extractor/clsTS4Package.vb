Public Class clsTS4Package
    Private _filename As String
    Private _inputFile As FileStream
    Private _recordCount As UInt32
    Private _recordSize As UInt32
    Private _indexPosition As UInt64
    Private _constTypeFlag As Boolean = False
    Private _constGroupFlag As Boolean = False
    Private _constInstanceExFlag As Boolean = False
    Private _constType As UInt32
    Private _constGroup As UInt32
    Private _constInstanceEx As UInt32
    Private _indexEntry() As clsTS4Resource
    Private _isValid As Boolean = False
    Private _closeOnGet As Boolean = True

    Sub New(filename As String)
        Try
            _inputFile = File.Open(filename, FileMode.Open, FileAccess.Read)
            _isValid = GetIndex()
        Catch ex As System.IO.IOException
            If ex.HResult = &H80070020 Then
                MsgBox("Package is open in editor or game, unable to read:" & ControlChars.CrLf & filename, MsgBoxStyle.Exclamation)
                _isValid = False
            End If
        Catch ex As Exception
            _isValid = False
            _inputFile.Close()
            _inputFile.Dispose()
            _inputFile = Nothing
        End Try
        _filename = filename
    End Sub

    Friend ReadOnly Property PackageName As String
        Get
            Return FileIO.FileSystem.GetName(_filename)
        End Get
    End Property

    Friend ReadOnly Property PackageFolder As String
        Get
            Return FileIO.FileSystem.GetParentPath(_filename)
        End Get
    End Property

    Friend ReadOnly Property IsValid As Boolean
        Get
            Return _isValid
        End Get
    End Property

    Friend ReadOnly Property Count As Integer
        Get
            Return _recordCount
        End Get
    End Property

    Friend ReadOnly Property Resource(idx As Integer) As clsTS4Resource
        Get
            Return _indexEntry(idx)
        End Get
    End Property

    Friend Property CloseOnGet As Boolean
        Get
            Return _closeOnGet
        End Get
        Set(value As Boolean)
            _closeOnGet = value
        End Set
    End Property

    Friend Function SearchResources(Optional Type As UInt32 = &HFFFFFFFFUI, Optional Group As UInt32 = &HFFFFFFFFUI, Optional Instance As UInt64 = &HFFFFFFFFFFFFFFFFUL) As clsTS4Resource()
        Dim Matches As New List(Of clsTS4Resource)

        For I = 0 To _recordCount - 1
            If (Type And _indexEntry(I).Type) = _indexEntry(I).Type AndAlso ((Group = &HFFFFFFFFUI And (_indexEntry(I).Group = 0)) Or (Group And _indexEntry(I).Group) = _indexEntry(I).Group) AndAlso (Instance And _indexEntry(I).Instance) = _indexEntry(I).Instance AndAlso Not _indexEntry(I).IsDeleted Then
                Matches.Add(_indexEntry(I))
            End If
        Next

        Return Matches.ToArray
    End Function

    Friend Function GetResourceItem(Optional Type As UInt32 = &HFFFFFFFFUI, Optional Group As UInt32 = &HFFFFFFFFUI, Optional Instance As UInt64 = &HFFFFFFFFFFFFFFFFUL) As clsTS4Resource
        For I = 0 To _recordCount - 1
            If (Type And _indexEntry(I).Type) = _indexEntry(I).Type AndAlso ((Group = &HFFFFFFFFUI And (_indexEntry(I).Group = 0)) Or (Group And _indexEntry(I).Group) = _indexEntry(I).Group) AndAlso (Instance And _indexEntry(I).Instance) = _indexEntry(I).Instance Then
                Return _indexEntry(I)
            End If
        Next

        Return Nothing
    End Function

    Friend Function GetResourceData(Optional Type As UInt32 = &HFFFFFFFFUI, Optional Group As UInt32 = &HFFFFFFFFUI, Optional Instance As UInt64 = &HFFFFFFFFFFFFFFFFUL) As Byte()
        Dim data() As Byte

        For I = 0 To _recordCount - 1
            If (Type And _indexEntry(I).Type) = _indexEntry(I).Type AndAlso ((Group = &HFFFFFFFFUI And (_indexEntry(I).Group = 0)) Or (Group And _indexEntry(I).Group) = _indexEntry(I).Group) AndAlso (Instance And _indexEntry(I).Instance) = _indexEntry(I).Instance Then
                If _inputFile Is Nothing Then
                    _inputFile = File.Open(_filename, FileMode.Open, FileAccess.Read)
                End If
                data = _indexEntry(I).Data(_inputFile)
                If _closeOnGet Then
                    _inputFile.Close()
                End If
                Return data
            End If
        Next

        Throw New Exception("Unable to load resource, not found in package")

        Return Nothing
    End Function

    Friend Function GetResourceData(Resource As clsTS4Resource) As Byte()
        Dim data() As Byte

        If _inputFile Is Nothing Then
            _inputFile = File.Open(_filename, FileMode.Open, FileAccess.Read)
        End If
        data = Resource.Data(_inputFile)
        If _closeOnGet Then
            _inputFile.Close()
        End If

        Return data
    End Function

    Private Function GetIndex() As Boolean
        Dim FileIdentifier As UInt32 = GetUInt32()
        Dim MajorVersion As UInt32 = GetUInt32()
        Dim MinorVersion As UInt32 = GetUInt32()
        Dim indexPositionLow As UInt32
        Dim Flags As UInt32

        If FileIdentifier <> &H46504244 Or MajorVersion <> 2 Or MinorVersion <> 1 Then
            Return False
        End If
        _inputFile.Seek(24, SeekOrigin.Current)
        _recordCount = GetUInt32()
        indexPositionLow = GetUInt32()
        _recordSize = GetUInt32()
        _inputFile.Seek(16, SeekOrigin.Current)
        _indexPosition = GetUInt64()
        If _indexPosition = 0 Then
            _indexPosition = indexPositionLow
        End If
        _inputFile.Seek(_indexPosition, SeekOrigin.Begin)
        Flags = GetUInt32()
        If Flags And &H1 Then
            _constTypeFlag = True
            _constType = GetUInt32()
        End If
        If Flags And &H2 Then
            _constGroupFlag = True
            _constGroup = GetUInt32()
        End If
        If Flags And &H4 Then
            _constInstanceExFlag = True
            _constInstanceEx = GetUInt32()
        End If
        ReDim _indexEntry(_recordCount)
        For I = 0 To _recordCount - 1
            _indexEntry(I) = New clsTS4Resource(Me, _constTypeFlag, _constType, _constGroupFlag, _constGroup, _constInstanceExFlag, _constInstanceEx)
        Next
        Return True
    End Function

    Protected Function GetBytes(len As Integer) As Byte()
        Dim buffer(len - 1) As Byte

        If _inputFile.Read(buffer, 0, len) <> len Then
            Throw New Exception("Unexpected end of file reading data")
            Return Nothing
        End If

        Return buffer
    End Function

    Protected Friend Function GetUInt16() As UInt16
        Dim buffer() As Byte = GetBytes(2)
        Dim value As UInt16 = BitConverter.ToUInt16(buffer, 0)

        Return value
    End Function

    Protected Friend Function GetUInt32() As UInt32
        Dim buffer() As Byte = GetBytes(4)
        Dim value As UInt32 = BitConverter.ToUInt32(buffer, 0)

        Return value
    End Function

    Private Function GetUInt64() As UInt64
        Dim buffer() As Byte = GetBytes(8)
        Dim value As UInt64 = BitConverter.ToUInt64(buffer, 0)

        Return value
    End Function

    Public Sub Close()
        If Not _inputFile Is Nothing Then
            _inputFile.Close()
            _inputFile.Dispose()
            _inputFile = Nothing
        End If
    End Sub

    Public Sub Dispose()
        Me.Finalize()
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Close()
        For i = 0 To _recordCount - 1
            _indexEntry(i) = Nothing
        Next
        _indexEntry = Nothing
    End Sub
End Class
