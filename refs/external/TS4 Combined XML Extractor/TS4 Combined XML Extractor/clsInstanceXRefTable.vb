Public Class clsInstanceXRef
    Private _Instance As UInt64
    Friend _ReferenceBy(-1) As UInt64
    Friend _ReferenceTo(-1) As UInt64
    Private ModifyLock As New Object

    Public Sub New(Instance As UInt64)
        _Instance = Instance
    End Sub

    Friend ReadOnly Property Instance
        Get
            Return _Instance
        End Get
    End Property

    Friend Sub AddReferenceBy(Instance As UInt64)
        SyncLock ModifyLock
            If Not _ReferenceBy.Contains(Instance) Then
                ReDim Preserve _ReferenceBy(_ReferenceBy.Length)
                _ReferenceBy(_ReferenceBy.Length - 1) = Instance
            End If
        End SyncLock
    End Sub

    Friend Sub AddReferenceTo(Instance As UInt64)
        SyncLock ModifyLock
            If Not _ReferenceTo.Contains(Instance) Then
                ReDim Preserve _ReferenceTo(_ReferenceTo.Length)
                _ReferenceTo(_ReferenceTo.Length - 1) = Instance
            End If
        End SyncLock
    End Sub
End Class

Public Class clsInstanceXRefTable
    Private obj_frmMain As frmMain
    Public IndexedXRefs As New SortedList(Of UInt64, clsInstanceXRef)
    Private InsertLock As New Object

    Public Sub New(Caller As frmMain)
        obj_frmMain = Caller
    End Sub

    Friend Function AddInstance(Instance As UInt64) As clsInstanceXRef
        Dim newInstanceXRef As New clsInstanceXRef(Instance)

        SyncLock InsertLock
            If Not IndexedXRefs.ContainsKey(Instance) Then
                IndexedXRefs.Add(Instance, newInstanceXRef)
            Else
                newInstanceXRef = IndexedXRefs.Item(Instance)
            End If
        End SyncLock

        Return newInstanceXRef
    End Function

    Friend Function GetInstanceXRef(Instance As UInt64) As clsInstanceXRef
        If IndexedXRefs.ContainsKey(Instance) Then
            Return IndexedXRefs.Item(Instance)
        End If
        Return Nothing
    End Function

    Friend Sub DumpInstanceXRefTable(outputFolderPath As String)
        Dim outFile As BinaryWriter
        Dim fileVersion As UInt64 = &H1UL
        Dim versionNum As UInt64 = My.Application.Info.Version.Major Or (My.Application.Info.Version.Minor << 8) Or (My.Application.Info.Version.Build << 16) Or (fileVersion << 24)
        Dim magicNum As UInt64 = &H4DDC4353UL Or (versionNum << 32)

        obj_frmMain.CheckForOverwrite(Path.Combine(outputFolderPath, "XML Cross Reference.dat"))
        outFile = New BinaryWriter(File.Open(Path.Combine(outputFolderPath, "XML Cross Reference.dat"), FileMode.Create, FileAccess.Write))
        outFile.Write(magicNum)
        For Each kvp As KeyValuePair(Of UInt64, clsInstanceXRef) In IndexedXRefs
            outFile.Write(kvp.Key)
            outFile.Write(kvp.Value._ReferenceBy.Length)
            For Each Instance As UInt64 In kvp.Value._ReferenceBy
                outFile.Write(Instance)
            Next
            outFile.Write(kvp.Value._ReferenceTo.Length)
            For Each Instance As UInt64 In kvp.Value._ReferenceTo
                outFile.Write(Instance)
            Next
        Next
        outFile.Close()
        outFile.Dispose()
    End Sub
End Class
