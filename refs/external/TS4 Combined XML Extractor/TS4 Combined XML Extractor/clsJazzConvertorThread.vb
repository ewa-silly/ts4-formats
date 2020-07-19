Public Class clsJazzConvertorThread
    Private _Type As UInt32
    Private _Group As UInt32
    Private _Instance As UInt64
    Private _memStream As MemoryStream
    Private obj_frmMain As frmMain
    Private _outputBasePath As String = String.Empty
    Private _outputFolderPath As String = String.Empty
    Private _crossref_index As clsCombinedXmlNameTable

    Public Sub New(Caller As frmMain, strTGI As String, memStream As MemoryStream, crossref_index As clsCombinedXmlNameTable)
        _Type = UInt32.Parse(strTGI.Substring(0, 8), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.CurrentCulture)
        _Group = UInt32.Parse(strTGI.Substring(9, 8), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.CurrentCulture)
        _Instance = UInt64.Parse(strTGI.Substring(18), Globalization.NumberStyles.HexNumber, Globalization.CultureInfo.CurrentCulture)
        _memStream = memStream
        _crossref_index = crossref_index
        obj_frmMain = Caller
    End Sub

    Public ReadOnly Property Group As UInt32
        Get
            Return _Group
        End Get
    End Property

    Public Property OutputBasePath As String
        Get
            Return _outputBasePath
        End Get
        Set(strPath As String)
            _outputBasePath = strPath & Path.DirectorySeparatorChar
        End Set
    End Property

    Public Property OutputFolderPath As String
        Get
            Return _outputFolderPath
        End Get
        Set(strPath As String)
            Dim fullFolderPath As String = Path.Combine(_outputBasePath, strPath)
            _outputFolderPath = strPath
            If Not My.Computer.FileSystem.DirectoryExists(fullFolderPath) Then
                My.Computer.FileSystem.CreateDirectory(fullFolderPath)
            End If
        End Set
    End Property

    Public Sub PerformExtract()
        Dim JazzConvertor As clsBinaryTuningConvertor
        Dim memStreamXml As New MemoryStream
        Dim filename As String
        Dim inputDoc As XmlDocument = New XmlDocument
        Dim rootNode As XmlElement
        Dim strName As String
        Dim settings As New XmlWriterSettings
        Dim writer As XmlWriter
        Dim fullFolderPath As String = Path.Combine(_outputBasePath, _outputFolderPath)

        JazzConvertor = New clsBinaryTuningConvertor(obj_frmMain, _memStream, memStreamXml, False, False, jazz_mode:=True)
        memStreamXml.Position = 0
        inputDoc.Load(memStreamXml)
        memStreamXml.Close()
        rootNode = inputDoc.DocumentElement
        strName = rootNode.GetAttribute("name")
        If Not _crossref_index Is Nothing Then
            _crossref_index.AddJazzXmlName(strName, _Instance)
        End If

        If frmMain.NamingConvention = NamingType.TGIOnly Then
            filename = Path.Combine(fullFolderPath, "S4_" & _Type.ToString("X8") & "_00000000_" & _Instance.ToString("X16") & ".xml")
        ElseIf frmMain.NamingConvention = NamingType.TGIName Then
            filename = Path.Combine(fullFolderPath, "S4_" & _Type.ToString("X8") & "_00000000_" & _Instance.ToString("X16") & "_" & strName & ".xml")
        ElseIf frmMain.NamingConvention = NamingType.NameOnly Then
            filename = Path.Combine(fullFolderPath, strName & ".xml")
        ElseIf frmMain.NamingConvention = NamingType.Native Then
            filename = Path.Combine(fullFolderPath, "0x00000000!0x" & _Instance.ToString("x16") & ".0x" & _Type.ToString("x8"))
        Else
            filename = Path.Combine(fullFolderPath, _Type.ToString("x8") & "!00000000!" & _Instance.ToString("x16") & "." & strName & ".AsmJazz.xml")
        End If
        settings.Indent = True
        obj_frmMain.CheckForOverwrite(filename)
        writer = XmlWriter.Create(filename, settings)
        inputDoc.WriteTo(writer)
        writer.Close()
        writer.Dispose()

        If frmMain.streamFileIndex IsNot Nothing Then
            filename = filename.Replace(_outputBasePath, "")
            frmMain.streamFileIndex.WriteLine(_Instance & ControlChars.Tab & strName & ControlChars.Tab & "JAZZ" & ControlChars.Tab & "ASM" & ControlChars.Tab & filename)
        End If
    End Sub
End Class
