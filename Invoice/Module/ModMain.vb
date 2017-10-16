Imports System.IO
Imports System.Data.Odbc

Module ModMain
    Private Declare Unicode Function GetPrivateProfileString Lib "kernel32" _
       Alias "GetPrivateProfileStringW" (ByVal lpApplicationName As String, _
       ByVal lpKeyName As String, ByVal lpDefault As String, _
       ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Int32, _
       ByVal lpFileName As String) As Int32

    Private Declare Unicode Function WritePrivateProfileString Lib "kernel32" _
    Alias "WritePrivateProfileStringW" (ByVal lpApplicationName As String, _
    ByVal lpKeyName As String, ByVal lpString As String, _
    ByVal lpFileName As String) As Int32

    Private Const INVALID_HANDLE_VALUE = -1

    Public g_Conn As String = "DATABASE"
    Public Connlocal As OdbcConnection
    Public g_strLogonLocationID As String
    Public g_OutletCode As String
    Public g_msgString As String
    Public g_strDbConn As String
    Public g_strINIFile As String = "System.dll"
    Public g_strINIRPT As String = "StockTakeValue.ini"
    Public ConnString As String
    Public LogPath As String
    Public g_PrinterPort As String
    Public g_IsIBT As Boolean

    Public g_strLogonUserID As String
    Public g_strInvoiceNo As String
    Public g_strInvoiceID As String
    Public g_strDeliveryNo As String = ""
    Public g_strSONo As String = ""
    Public g_strOrderBy As String = ""
    Public g_strPriority As String = ""

    Public g_CutOffDateINVUnitPrice As Date = New Date(2017, 9, 1)

    Public Sub Init()
        ConnString = "Driver={MySQL ODBC 3.51 Driver}" & _
            ";SERVER=" & GetINI("DATABASE", "SERVER") & _
            ";DATABASE=" & GetINI("DATABASE", "DATABASE") & _
            ";PORT=" & CHEXStr(GetINI("DATABASE", "PORT")) & _
            ";UID=" & CHEXStr(GetINI("DATABASE", "UID")) & _
            ";PWD=" & CHEXStr(GetINI("DATABASE", "PWD")) & _
            ";OPTION=" & 1 + 2 + 8 + 32 + 2048 + 16384

        'MsgBox(g_strLogonLocationID)
    End Sub

    Public Function GetINI(ByVal strSection As String, ByVal strKey As String, Optional ByVal TmpINI As String = "") As String
        Dim lngRet As Long
        Dim strValue As New System.Text.StringBuilder(256)

        If TmpINI = "" Then
            lngRet = GetPrivateProfileString(strSection, strKey, "", strValue, 255, Path.Combine(New DirectoryInfo(Application.StartupPath).Parent.FullName, g_strINIFile))
            'lngRet = GetPrivateProfileString(strSection, strKey, "", strValue, 255, AppDomain.CurrentDomain.BaseDirectory() & g_strINIFile)
        Else
            lngRet = GetPrivateProfileString(strSection, strKey, "", strValue, 255, TmpINI)
        End If

        If lngRet <> 0 Then
            GetINI = Left(strValue.ToString, lngRet)
        Else
            GetINI = ""
        End If
    End Function

    Public Function GetINIRPT(ByVal strSection As String, ByVal strKey As String) As String
        Dim lngRet As Long
        Dim strValue As New System.Text.StringBuilder(256)

        lngRet = GetPrivateProfileString(strSection, strKey, "", strValue, 255, Path.Combine(New DirectoryInfo(Application.StartupPath).Parent.FullName, g_strINIRPT))

        If lngRet <> 0 Then
            GetINIRPT = Left(strValue.ToString, lngRet)
        Else
            Return Nothing
        End If
    End Function

    Public Sub WriteINI(ByVal strSection As String, ByVal strKey As String, ByVal strValue As String)
        WritePrivateProfileString(strSection, strKey, CStrINI(strValue), Path.Combine(Application.StartupPath, g_strINIFile))
    End Sub

    Private Function CStrINI(ByVal Value As String) As String
        Dim strTemp As String
        strTemp = Value
        strTemp = Replace(strTemp, "\", "\\")
        strTemp = Replace(strTemp, vbCrLf, "\n")
        CStrINI = strTemp
    End Function

    Private Function CHEXNUMBER(ByVal Value As String) As Long
        On Error Resume Next
        CHEXNUMBER = CLng("&H" & Value)
    End Function

    Public Function CHEXStr(ByVal Value As String) As String
        Dim pos As Long
        Dim temp As String
        Dim result As String

        result = ""
        For pos = 1 To Len(Value) \ 2
            temp = Mid(Value, (pos * 2) - 1, 2)
            result = result & Chr(CHEXNUMBER(temp) - pos)
        Next
        CHEXStr = result
    End Function

    Public Function CSQLDate(ByVal dtDate As Date) As String
        Return Format(dtDate, "yyyy-MM-dd")
    End Function

    Public Function CSQLDateMonth(ByVal dtDate As Date) As String
        Return Format(dtDate, "dd-MMM-yyyy")
    End Function

    Public Function ConnectDB(ByRef Conn As OdbcConnection, ByVal code As String) As Boolean
        Try
            Select Case code
                Case g_Conn
                    If Connlocal Is Nothing Then Connlocal = New OdbcConnection
                    If Connlocal.State <> ConnectionState.Open Then
                        Connlocal.ConnectionString = g_strDbConn
                        Connlocal.Open()
                    End If
                    Conn = Connlocal
                    ConnectDB = True
            End Select
            Exit Function
        Catch ex As Exception
            MsgBox(g_strDbConn)
            g_msgString = "Error while trying to connect to " & code & " connection"
            MsgBox(g_msgString, MsgBoxStyle.Exclamation, "Warning!")
            MsgBox(ex.ToString)
        End Try
    End Function
    Public Function CNullStr(ByVal Value)
        'Convert NULL to EMPTY STRING
        Return IIf(IsEmptyString(Value), "", Value)
    End Function
    Public Function IsEmptyString(ByVal Value) As Boolean
        'NULL OR EMPTY are considered EmptyString
        If IsDBNull(Value) Then
            Return True
        ElseIf Value Is Nothing Then
            Return True
        ElseIf Value.ToString = "" Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Class Win32API
        Public Const GENERIC_WRITE = &H40000000
        Public Const CREATE_ALWAYS = 2
        Public Const OPEN_EXISTING = 3

        Public Declare Function CreateFile Lib "kernel32" Alias "CreateFileA" (ByVal lpFileName _
        As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, _
        ByVal lpSecurityAttributes As Integer, ByVal dwCreationDisposition As Integer, _
        ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As Integer) As Integer

        Public Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (ByVal hObject As Long) As Long
    End Class

    Public Function PrintBarcode(ByRef sPort As String, ByRef sText As String) As Boolean
        Dim FileHandle As Long
        Dim FileHandleIntPtr As IntPtr
        Dim FStream As FileStream
        Dim SWriter As StreamWriter

        FileHandle = Win32API.CreateFile(sPort, Win32API.GENERIC_WRITE, 0, 0, Win32API.CREATE_ALWAYS, 0, 0)

        If FileHandle = INVALID_HANDLE_VALUE Then
            Return False
        End If

        FileHandleIntPtr = New IntPtr(FileHandle)
        'FStream = New FileStream(FileHandleIntPtr, FileAccess.Write) (FileStream(FileHandleIntPtr, FileAccess.Write))
        FStream = New FileStream(FileHandleIntPtr, FileAccess.Write, False)

        SWriter = New StreamWriter(FStream)
        SWriter.AutoFlush = True
        SWriter.WriteLine(sText)
        FStream.Flush()
        SWriter.Close()
        FStream.Close()

        Win32API.CloseHandle(FileHandle)

        Return True

    End Function

    Public Sub WriteLog(ByVal strModule As String, ByVal strErrMsg As String)
        Dim attempt As Integer = 0
        While attempt < 3
            Try
                Dim sw As New StreamWriter(Path.Combine(Application.StartupPath, Format(Now, "yyyyMMdd-")), True)
                sw.Write(vbCrLf & "*** " & Now() & " ***" & vbCrLf)
                sw.Write(vbCrLf & "Module :" & strModule)
                sw.Write(vbCrLf & "Error :" & strErrMsg & vbCrLf)
                sw.Close()
            Catch ex As Exception
                attempt += 1
                Continue While
            End Try
            Exit While
        End While
    End Sub
End Module
