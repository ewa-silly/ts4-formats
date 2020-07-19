Public Class clsBinaryTuningConvertorThread
    Private _memStreamIn As MemoryStream
    Private _memStreamOut As MemoryStream
    Private _enable_strings As Boolean
    Private _enable_crossrefs As Boolean
    Private _string_index As clsCombinedStringTable
    Private _crossref_index As clsCombinedXmlNameTable
    Private obj_frmMain As frmMain

    Public Sub New(Caller As frmMain, memStreamIn As MemoryStream, memStreamOut As MemoryStream, enable_strings As Boolean, enable_crossrefs As Boolean, Optional string_index As clsCombinedStringTable = Nothing, Optional crossref_index As clsCombinedXmlNameTable = Nothing)
        obj_frmMain = Caller
        _memStreamIn = memStreamIn
        _memStreamOut = memStreamOut
        _enable_strings = enable_strings
        _enable_crossrefs = enable_crossrefs
        _string_index = string_index
        _crossref_index = crossref_index
    End Sub

    Public Sub ThreadMain()
        Dim binaryTuning As clsBinaryTuningConvertor = New clsBinaryTuningConvertor(obj_frmMain, _memStreamIn, _memStreamOut, _enable_strings, _enable_crossrefs, _string_index, _crossref_index)
    End Sub
End Class
