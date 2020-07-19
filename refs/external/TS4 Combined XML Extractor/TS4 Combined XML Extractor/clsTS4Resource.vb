Public Class clsTS4Resource
    Private _Type As UInt32
    Private _Group As UInt32
    Private _InstanceEx As UInt32
    Private _Instance As UInt32
    Private _Position As UInt32
    Private _Size As UInt32
    Private _ExtendedCompressionType As Boolean = False
    Private _SizeDecompressed As UInt32
    Private _CompressionType As UInt16 = 0
    Private _Committed As UInt16 = 0

    Sub New(inputPackage As clsTS4Package, constTypeFlag As Boolean, constType As UInteger, constGroupFlag As Boolean, constGroup As UInteger, constInstanceExFlag As Boolean, constInstanceEx As UInteger)
        If constTypeFlag Then
            _Type = constType
        Else
            _Type = inputPackage.GetUInt32
        End If
        If constGroupFlag Then
            _Group = constGroup
        Else
            _Group = inputPackage.GetUInt32
        End If
        If constInstanceExFlag Then
            _InstanceEx = constInstanceEx
        Else
            _InstanceEx = inputPackage.GetUInt32
        End If
        _Instance = inputPackage.GetUInt32
        _Position = inputPackage.GetUInt32
        _Size = inputPackage.GetUInt32
        If _Size And &H80000000 Then
            _ExtendedCompressionType = True
            _Size = _Size Xor &H80000000UI
        End If
        _SizeDecompressed = inputPackage.GetUInt32
        If _ExtendedCompressionType Then
            _CompressionType = inputPackage.GetUInt16
            _Committed = inputPackage.GetUInt16
        End If
    End Sub

    Friend ReadOnly Property Type As UInt32
        Get
            Return _Type
        End Get
    End Property

    Friend ReadOnly Property Group As UInt32
        Get
            Return _Group
        End Get
    End Property

    Friend ReadOnly Property Instance As UInt64
        Get
            Dim Tmp As UInt64 = _InstanceEx
            Return (Tmp << 32) Or _Instance
        End Get
    End Property

    Friend ReadOnly Property IsDeleted() As Boolean
        Get
            If _ExtendedCompressionType And _CompressionType = &HFFE0US Then
                Return True
            End If

            Return False
        End Get
    End Property

    Friend ReadOnly Property Data(inputFile As FileStream, Optional decompressFlag As Boolean = True) As Byte()
        Get
            If Me.IsDeleted Then
                Throw New Exception("Cannot read deleted resource")
                Return Nothing
            End If

            Dim bufCompressed(_Size) As Byte

            inputFile.Seek(_Position, SeekOrigin.Begin)
            If inputFile.Read(bufCompressed, 0, _Size) <> _Size Then
                Throw New Exception("Unexpected end of file reading data")
                Return Nothing
            End If

            If decompressFlag And _ExtendedCompressionType Then
                Return DecompressData(bufCompressed)
            Else
                Return bufCompressed
            End If
        End Get
    End Property

    Private Function DecompressData(data As Byte()) As Byte()
        If _CompressionType = &H5A42US Then
            ' ZLIB Compression
            Dim garbage(1) As Byte
            Dim oStream As New MemoryStream()
            Dim dStream As DeflateStream
            Dim iStream As New MemoryStream(data)

            ' Skip the first two bytes of the stream
            iStream.Read(garbage, 0, 2)

            dStream = New DeflateStream(iStream, CompressionMode.Decompress)

            dStream.CopyTo(oStream)
            Return oStream.ToArray
        ElseIf _CompressionType = &HFFFFUS Then
            ' Internal compression
            Return InternalDecompression(data)
        Else
            ' Unknown compression
            Throw New Exception("Unable to decompress compression type" & _CompressionType.ToString("X4"))
            Return Nothing
        End If
    End Function

    Private Function InternalDecompression(data() As Byte) As Byte()
        Dim udata(_SizeDecompressed - 1) As Byte
        Dim udata_idx As Integer = 0
        Dim data_idx As Integer = 5 ' Skip 2 bytes of flags + 3 indicating length (use the package header for uncompressed size)
        Dim CompressionFormat As Byte = data(0)
        Dim ControlCode As Byte
        Dim Size As Integer
        Dim CopySize As Integer
        Dim CopyOffset As Integer

        ' Skip an extra byte if compression format flag has high-bit set (indicating 4 byte length)
        If CompressionFormat And &H80 Then
            data_idx += 1
        End If

        Do
            ControlCode = data(data_idx)
            data_idx += 1
            If ControlCode <= &H7F Then
                Size = ControlCode And &H3
                CopySize = ((ControlCode And &H1C) / 4) + 3
                CopyOffset = ((ControlCode And &H60) * 8) + data(data_idx)
                data_idx += 1
                Array.Copy(data, data_idx, udata, udata_idx, Size)
                data_idx += Size
                udata_idx += Size
                For I = 0 To CopySize - 1
                    udata(udata_idx + I) = udata((udata_idx + I) - CopyOffset - 1)
                Next
                udata_idx += CopySize
            ElseIf ControlCode <= &HBF Then
                Size = (data(data_idx) And &HC0) / 64
                CopySize = (ControlCode And &H3F) + 4
                CopyOffset = ((data(data_idx) And &H3F) * 256) + data(data_idx + 1)
                data_idx += 2
                Array.Copy(data, data_idx, udata, udata_idx, Size)
                data_idx += Size
                udata_idx += Size
                For I = 0 To CopySize - 1
                    udata(udata_idx + I) = udata((udata_idx + I) - CopyOffset - 1)
                Next
                udata_idx += CopySize
            ElseIf ControlCode <= &HDF Then
                Size = ControlCode And &H3
                CopySize = ((ControlCode And &HC) * 64) + data(data_idx + 2) + 5
                CopyOffset = ((ControlCode And &H10) * 4096) + (data(data_idx) * 256) + data(data_idx + 1)
                data_idx += 3
                Array.Copy(data, data_idx, udata, udata_idx, Size)
                data_idx += Size
                udata_idx += Size
                For I = 0 To CopySize - 1
                    udata(udata_idx + I) = udata((udata_idx + I) - CopyOffset - 1)
                Next
                udata_idx += CopySize
            ElseIf ControlCode <= &HFB Then
                Size = ((ControlCode And &H1F) * 4) + 4
                Array.Copy(data, data_idx, udata, udata_idx, Size)
                data_idx += Size
                udata_idx += Size
            Else
                Size = ControlCode And &H3
                If Size > 0 Then
                    Array.Copy(data, data_idx, udata, udata_idx, Size)
                    data_idx += Size
                    udata_idx += Size
                End If
            End If
        Loop Until ControlCode >= &HFC

        Return udata
    End Function
End Class