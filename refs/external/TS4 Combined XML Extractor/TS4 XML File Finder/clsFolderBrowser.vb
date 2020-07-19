Imports System.Runtime.InteropServices

Public Class clsFolderBrowser
    ' Adapted for VB from C# class FolderBrowser2 by Simon Mourier
    ' http://stackoverflow.com/questions/15368771/show-detailed-folder-browser-from-a-propertygrid

    <DllImport("shell32.dll")>
    Private Shared Function SHILCreateFromPath(
        <MarshalAs(UnmanagedType.LPWStr)> pszPath As String,
        <Out()> ByRef ppIdl As IntPtr,
        ByRef rgflnOut As UInteger) As Integer
    End Function

    <DllImport("shell32.dll")>
    Private Shared Function SHCreateShellItem(
        pidlParent As IntPtr,
        psfParent As IntPtr,
        pidl As IntPtr,
        <Out()> ByRef ppsi As IShellItem) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetActiveWindow() As IntPtr
    End Function

    Private Const ERROR_CANCELLED As UInteger = &H800704C7UI

    <ComImport> <Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")> _
    Private Class FileOpenDialog
    End Class

    <ComImport> <Guid("42f85136-db7e-439c-85f1-e4075d135fc8")> _
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Private Interface IFileOpenDialog
        <PreserveSig> Function Show(<[In]> parent As IntPtr) As UInteger    ' IModalWindow
        Sub SetFilesTypes() ' Not fully defined
        Sub SetFileTypeIndex(<[In]> iFileType As UInteger)
        Sub GetFileTypeIndex(<Out()> ByRef piFileType As UInteger)
        Sub Advise()    ' Not fully defined
        Sub Unadvise()
        Sub SetOptions(<[In]> fos As FOS)
        Sub GetOptions(<Out()> ByRef pfos As FOS)
        Sub SetDefaultFolder(psi As IShellItem)
        Sub SetFolder(psi As IShellItem)
        Sub GetFolder(<Out()> ByRef ppsi As IShellItem)
        Sub GetCurrentSelection(<Out()> ByRef ppsi As IShellItem)
        Sub SetFileName(<[In], MarshalAs(UnmanagedType.LPWStr)> pszName As String)
        Sub GetFileName(<MarshalAs(UnmanagedType.LPWStr)> <Out()> ByRef pszName As String)
        Sub SetTitle(<[In], MarshalAs(UnmanagedType.LPWStr)> pszTitle As String)
        Sub SetOkButtonLabel(<[In], MarshalAs(UnmanagedType.LPWStr)> pszText As String)
        Sub SetFileNameLabel(<[In], MarshalAs(UnmanagedType.LPWStr)> pszLabel As String)
        Sub GetResult(<Out()> ByRef ppsi As IShellItem)
        Sub AddPlace(psi As IShellItem, alignment As Integer)
        Sub SetDefaultExtension(<[In], MarshalAs(UnmanagedType.LPWStr)> pszDefaultExtension As String)
        Sub Close(hr As Integer)
        Sub SetClientGuid() ' Not fully defined
        Sub ClearClientData()
        Sub SetFilter(<MarshalAs(UnmanagedType.Interface)> pFilter As IntPtr)
        Sub GetResults(<MarshalAs(UnmanagedType.Interface)> <Out()> ByRef ppenum As IntPtr)  ' Not fully defined
        Sub GetSelectedItems(<MarshalAs(UnmanagedType.Interface)> <Out()> ByRef ppsai As IntPtr) ' Not fully defined
    End Interface

    <ComImport> <Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")> _
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Private Interface IShellItem
        Sub BindToHandler() ' Not fully defined
        Sub GetParent() ' Not fully defined
        Sub GetDisplayName(<[In]> signdnName As SIGDN, <MarshalAs(UnmanagedType.LPWStr)> <Out()> ByRef ppszName As String)
        Sub GetAttributes() ' Not fully defined
        Sub Compare()   ' Not fully defined
    End Interface

    Private Enum SIGDN As UInteger
        SIGDN_DESKTOPABSOLUTEEDITING = &H8004C000UI
        SIGDN_DESKTOPABSOLUTEPARSING = &H80028000UI
        SIGDN_FILESYSPATH = &H80058000UI
        SIGDN_NORMALDISPLAY = &H0UI
        SIGDN_PARENTRELATIVE = &H80080001UI
        SIGDN_PARENTRELATIVEEDITING = &H80031001UI
        SIGDN_PARENTRELATIVEFORADDRESSBAR = &H8007C001UI
        SIGDN_PARENTRELATIVEPARSING = &H80018001UI
        SIGDN_URL = &H80068000UI
    End Enum

    <Flags>
    Private Enum FOS
        FOS_ALLNONSTORAGEITEMS = &H80
        FOS_ALLOWMULTISELECT = &H200
        FOS_CREATEPROMPT = &H2000
        FOS_DEFAULTNOMINIMODE = &H20000000
        FOS_DONTADDTORECENT = &H2000000
        FOS_FILEMUSTEXIST = &H1000
        FOS_FORCEFILESYSTEM = &H40
        FOS_FORCESHOWHIDDEN = &H10000000
        FOS_HIDEMRUPLACES = &H20000
        FOS_HIDEPINNEDPLACES = &H40000
        FOS_NOCHANGEDIR = 8
        FOS_NODEREFERENCELINKS = &H100000
        FOS_NOREADONLYRETURN = &H8000
        FOS_NOTESTFILECREATE = &H10000
        FOS_NOVALIDATE = &H100
        FOS_OVERWRITEPROMPT = 2
        FOS_PATHMUSTEXIST = &H800
        FOS_PICKFOLDERS = &H20
        FOS_SHAREAWARE = &H4000
        FOS_STRICTFILETYPES = 4
    End Enum

    Private _DirectoryPath As String

    Public Property DirectoryPath As String
        Get
            Return _DirectoryPath
        End Get
        Set(strPath As String)
            _DirectoryPath = strPath
        End Set
    End Property

    Public Function ShowDialog(Optional owner As IWin32Window = Nothing) As DialogResult
        Dim hwndOwner As IntPtr
        Dim dialog As IFileOpenDialog

        If owner IsNot Nothing Then
            hwndOwner = owner.Handle
        Else
            hwndOwner = GetActiveWindow()
        End If
        dialog = New FileOpenDialog

        Try
            Dim item As IShellItem = Nothing
            Dim hr As UInteger
            Dim path As String = Nothing

            If Not String.IsNullOrEmpty(_DirectoryPath) Then
                Dim idl As IntPtr
                Dim atts As UInteger = 0
                If SHILCreateFromPath(_DirectoryPath, idl, atts) = 0 Then
                    If SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, idl, item) = 0 Then
                        dialog.SetFolder(item)
                    End If
                End If
            End If
            dialog.SetOptions(FOS.FOS_PICKFOLDERS Or FOS.FOS_FORCEFILESYSTEM)
            hr = dialog.Show(hwndOwner)
            If hr = ERROR_CANCELLED Then
                Return DialogResult.Cancel
            End If
            If hr <> 0 Then
                Return DialogResult.Abort
            End If
            dialog.GetResult(item)
            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, path)
            DirectoryPath = path
            Return DialogResult.OK
        Finally
            Marshal.ReleaseComObject(dialog)
        End Try
    End Function
End Class