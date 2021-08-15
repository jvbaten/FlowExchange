'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO
Imports CAPEOPEN
Imports System.Windows.Forms

'base class for the XFileLoader and XFileSaver unit operations

<ComDefaultInterface(GetType(ICapeUnit))> _
Public MustInherit Class UnitOperationBase
    Inherits CAPEOPENBase
    Implements ICapeUnit
    Implements ICapeUtilities
    Implements IPersistStream
    Implements ICapeUnitReport

    Friend validationStatus As CapeValidationStatus = CapeValidationStatus.CAPE_NOT_VALIDATED
    Protected _portCollection As New PortCollection
    Protected _parameterCollection As New ParameterCollection
    Dim _diags As ICapeDiagnostic
    Dim _dirty As Boolean = True
    Dim currentReport As String = Nothing
    Shared LastRunReportName As String = "Last Run"
    Protected lastRunReport As String = Nothing
    Protected lastRunReportBuilder As StringBuilder = Nothing

    Public MustOverride Sub Calculate() Implements ICapeUnit.Calculate
    Public MustOverride Function ValidateUnit(ByRef message As String) As Boolean
    Public MustOverride Function EditUnit() As Boolean 'should return true if something was changed
    Protected MustOverride Function TypeName() As String

    Sub New()
        MyBase.New(, , True)
    End Sub

    Private Shared Sub Register(ByVal t As Type, ByVal rootKey As Microsoft.Win32.RegistryKey)
        'this is the mixed set of CAPE-OPEN defined keys and COM defined keys. To get the 
        ' COM defined keys for this object, run regasm with the /regfile /codebase options.
        'root key should be HKEY_CLASSES_ROOT or HKEY_LOCAL_MACHINE\Software\Classes or HKEY_CURRENT_USER\Software\Classes
        Dim g As GuidAttribute = CType(Attribute.GetCustomAttribute(t, GetType(GuidAttribute)), GuidAttribute)
        Dim p As ProgIdAttribute = CType(Attribute.GetCustomAttribute(t, GetType(ProgIdAttribute)), ProgIdAttribute)
        Dim n As NameDescriptionAttribute = CType(Attribute.GetCustomAttribute(t, GetType(NameDescriptionAttribute)), NameDescriptionAttribute)
        Dim key, subKey, subSubKey As Microsoft.Win32.RegistryKey
        Dim dllPath As String = t.Assembly.Location
        Dim dllUrl As String = "file://" + dllPath.Replace("\", "/")
        Dim dllVersion As String = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath).FileVersion.ToString
        Dim clrVersion As String = t.Assembly.ImageRuntimeVersion
        key = rootKey.CreateSubKey("CLSID\{" + g.Value + "}")
        key.SetValue(Nothing, p.Value)
        subKey = key.CreateSubKey("Implemented Categories")
        subKey.CreateSubKey("{678C09A5-7D66-11D2-A67D-00105A42887F}").Close()
        subKey.CreateSubKey("{678C09A1-7D66-11D2-A67D-00105A42887F}").Close()
        subKey.CreateSubKey("{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}").Close()
        subKey.CreateSubKey("{4150C28A-EE06-403f-A871-87AFEC38A249}").Close()
        subKey.CreateSubKey("{0D562DC8-EA8E-4210-AB39-B66513C0CD09}").Close()
        subKey.CreateSubKey("{4667023A-5A8E-4CCA-AB6D-9D78C5112FED}").Close()
        subKey.Close()
        subKey = key.CreateSubKey("CapeDescription")
        subKey.SetValue("Name", n.GetName)
        subKey.SetValue("Description", n.GetDescription)
        subKey.SetValue("CapeVersion", "1.0")
        subKey.SetValue("ComponentVersion", dllVersion)
        subKey.SetValue("VendorURL", "http://www.amsterchem.com/")
        subKey.SetValue("About", "Utility unit operation for XFlow CAPE-OPEN based simulation stream exchange files")
        subKey.Close()
        subKey = key.CreateSubKey("InprocServer32")
        subKey.SetValue(Nothing, "mscoree.dll")
        subKey.SetValue("ThreadingModel", "Both")
        subKey.SetValue("Class", t.FullName)
        subKey.SetValue("Assembly", t.Assembly.FullName)
        subKey.SetValue("RuntimeVersion", clrVersion)
        subKey.SetValue("CodeBase", dllUrl)
        subSubKey = subKey.CreateSubKey(dllVersion)
        subSubKey.SetValue("Class", t.FullName)
        subSubKey.SetValue("Assembly", t.Assembly.FullName)
        subSubKey.SetValue("RuntimeVersion", clrVersion)
        subSubKey.SetValue("CodeBase", dllUrl)
        subSubKey.Close()
        subKey.Close()
        subKey = key.CreateSubKey("ProgId")
        subKey.SetValue(Nothing, p.Value)
        subKey.Close()
        key.Close()
        key = rootKey.CreateSubKey(p.Value)
        key.SetValue(Nothing, p.Value)
        subKey = key.CreateSubKey("CLSID")
        subKey.SetValue(Nothing, "{" + g.Value + "}")
        subKey.Close()
        subKey = key.CreateSubKey("CurVer")
        subKey.SetValue(Nothing, p.Value + ".1")
        subKey.Close()
        key.Close()
        key = rootKey.CreateSubKey(p.Value + ".1")
        key.SetValue(Nothing, p.Value)
        subKey = key.CreateSubKey("CLSID")
        subKey.SetValue(Nothing, "{" + g.Value + "}")
        subKey.Close()
        key.Close()
    End Sub

    Private Shared Sub Unregister(ByVal t As Type, ByVal rootKey As Microsoft.Win32.RegistryKey)
        'root key should be HKEY_CLASSES_ROOT or HKEY_LOCAL_MACHINE\Software\Classes or HKEY_CURRENT_USER\Software\Classes
        Dim g As GuidAttribute = CType(Attribute.GetCustomAttribute(t, GetType(GuidAttribute)), GuidAttribute)
        Dim p As ProgIdAttribute = CType(Attribute.GetCustomAttribute(t, GetType(ProgIdAttribute)), ProgIdAttribute)
        Try
            rootKey.DeleteSubKeyTree("CLSID\{" + g.Value + "}")
        Catch ex As ArgumentException
        End Try
        Try
            rootKey.DeleteSubKeyTree(p.Value)
        Catch ex As ArgumentException
        End Try
    End Sub

    Public Shared Sub RegisterAllUsers(ByVal t As Type)
        Register(t, Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\Classes"))
    End Sub

    Public Shared Sub UnregisterAllUsers(ByVal t As Type)
        Unregister(t, Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\Classes"))
    End Sub

    Public Shared Sub RegisterCurrentUser(ByVal t As Type)
        Register(t, Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Classes"))
    End Sub

    Public Shared Sub UnregisterCurrentUser(ByVal t As Type)
        Unregister(t, Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Classes"))
    End Sub

    <ComRegisterFunctionAttribute()> Public Shared Sub RegisterFunction(ByVal t As Type)
        'additional registry entries, default is in HKEY_CLASSES_ROOT
        'Register(Microsoft.Win32.Registry.ClassesRoot)
        Register(t, Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Classes"))
    End Sub

    <ComUnregisterFunctionAttribute()> Public Shared Sub UnregisterFunction(ByVal t As Type)
        'this should work ok for all users as well as current user registration
        Unregister(t, Microsoft.Win32.Registry.ClassesRoot)
    End Sub

    Public ReadOnly Property ports As Object Implements ICapeUnit.ports
        Get
            Return _portCollection
        End Get
    End Property

    Friend Sub Invalidate()
        validationStatus = CapeValidationStatus.CAPE_NOT_VALIDATED
        _dirty = True
    End Sub

    Public Function Validate(ByRef message As String) As Boolean Implements ICapeUnit.Validate
        Dim res As Boolean = ValidateUnit(message)
        If res Then validationStatus = CapeValidationStatus.CAPE_VALID Else validationStatus = CapeValidationStatus.CAPE_INVALID
        Return res
    End Function

    Public ReadOnly Property ValStatus As CapeValidationStatus Implements ICapeUnit.ValStatus
        Get
            Return validationStatus
        End Get
    End Property

    Public Function Edit() As Integer Implements ICapeUtilities.Edit
        Const S_OK As Integer = 0
        Const S_FALSE As Integer = 1
        If EditUnit() Then
            'changed
            Return S_OK
        Else
            'unchanged
            Return S_FALSE
        End If
    End Function

    Public Sub Initialize() Implements ICapeUtilities.Initialize
    End Sub

    Public ReadOnly Property parameters As Object Implements ICapeUtilities.parameters
        Get
            Return _parameterCollection
        End Get
    End Property

    Public WriteOnly Property simulationContext As Object Implements ICapeUtilities.simulationContext
        Set(value As Object)
            If _diags IsNot Nothing Then
                ReleaseIfCOM(_diags)
                _diags = Nothing
            End If
            If value IsNot Nothing Then
                Try
                    _diags = value
                Catch
                End Try
            End If
        End Set
    End Property

    Public Sub Terminate() Implements ICapeUtilities.Terminate
        _portCollection.Clear()
        _parameterCollection.Clear()
        If _diags IsNot Nothing Then
            ReleaseIfCOM(_diags)
            _diags = Nothing
        End If
    End Sub

    Friend Enum MessageType
        InfoMessage
        WarningMessage
        ErrorMessage
    End Enum

    Friend Sub LogMessage(message As String, Optional type As MessageType = MessageType.InfoMessage)
        If (_diags IsNot Nothing) Then
            Dim s As New StringBuilder
            s.Append(ComponentName)
            Select Case type
                Case MessageType.WarningMessage
                    s.Append(" warning")
                Case MessageType.ErrorMessage
                    s.Append(" error")
            End Select
            s.Append(": ")
            s.Append(message)
            _diags.LogMessage(s.ToString)
        End If
        If lastRunReportBuilder IsNot Nothing Then
            Select Case type
                Case MessageType.WarningMessage
                    lastRunReportBuilder.Append("warning: ")
                Case MessageType.ErrorMessage
                    lastRunReportBuilder.Append("error: ")
            End Select
            lastRunReportBuilder.Append(message)
            lastRunReportBuilder.Append(vbCrLf)
        End If
    End Sub

    Protected Sub AddPort(p As Port)
        _portCollection.Add(p)
    End Sub

    Protected Sub AddParameter(p As ParameterBase)
        _parameterCollection.Add(p)
    End Sub

    Public Sub GetClassID(ByRef pClassId As System.Guid) Implements IPersist.GetClassID
        Dim g As GuidAttribute = CType(Attribute.GetCustomAttribute(Me.GetType(), GetType(GuidAttribute)), GuidAttribute)
        pClassId = New Guid(g.Value)
    End Sub

    Public Sub GetClassID1(ByRef pClassId As System.Guid) Implements IPersistStream.GetClassID
        GetClassID(pClassId)
    End Sub

    Public Function IsDirty() As Integer Implements IPersistStream.IsDirty
        Return _dirty
    End Function

    Friend Overridable Function Save(Optional configOnly As Boolean = False) As StorageData
        Dim s As New StorageData
        _parameterCollection.Save(s)
        If lastRunReport IsNot Nothing Then s("LastRunReport") = New StorageDataItem(lastRunReport)
        Return s
    End Function

    Friend Overridable Sub Load(s As StorageData, Optional configOnly As Boolean = False)
        _parameterCollection.Load(s)
        Dim si As StorageDataItem = Nothing
        If s.TryGetValue("LastRunReport", si) Then
            If si.type = StorageDataType.StringType Then lastRunReport = CStr(si.data)
        End If
    End Sub

    Private Function SaveToMemory() As Byte()
        Dim s As StorageData = Save()
        '#If DEBUG Then
        '        'save an XML copy on the desktop
        '        s.SaveXML(TypeName(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), TypeName() + ".xml"))
        '#End If
        Dim ms As New MemoryStream
        Dim gz As New Compression.GZipStream(ms, Compression.CompressionMode.Compress)
        Dim writer As New BinaryWriter(gz)
        writer.Write(s.Count)
        For Each pair As KeyValuePair(Of String, StorageDataItem) In s
            writer.Write(pair.Key)
            writer.Write(CByte(pair.Value.type))
            Select Case pair.Value.type
                Case StorageDataType.BooleanType
                    writer.Write(CBool(pair.Value.data))
                Case StorageDataType.DoubleType
                    writer.Write(CDbl(pair.Value.data))
                Case StorageDataType.IntegerType
                    writer.Write(CInt(pair.Value.data))
                Case StorageDataType.StringType
                    writer.Write(CStr(pair.Value.data))
                Case StorageDataType.DoubleArrayType
                    Dim data() As Double = CType(pair.Value.data, Double())
                    writer.Write(data.Length)
                    For Each dbl As Double In data
                        writer.Write(dbl)
                    Next
                Case StorageDataType.StringArrayType
                    Dim data() As String = CType(pair.Value.data, String())
                    writer.Write(data.Length)
                    For Each str As String In data
                        writer.Write(str)
                    Next
                Case Else
                    Debug.Assert(False)
                    Throw New Exception("Internal error: unknown storage data type")
            End Select
        Next
        gz.Close()
        Return ms.ToArray
    End Function

    Public Sub GetMaxSize(ByRef pCbSize As Long) Implements IPersistStream.GetMaxSize
        Dim data() As Byte = SaveToMemory()
        pCbSize = data.Length + 4 'data + length
    End Sub

    Public Sub Load(pStm As System.Runtime.InteropServices.ComTypes.IStream) Implements IPersistStream.Load
        Dim read As System.IntPtr = 0
        Dim intBytes(3) As Byte
        pStm.Read(intBytes, 4, read)
        Dim len As Integer = BitConverter.ToInt32(intBytes, 0)
        Dim data(len - 1) As Byte
        pStm.Read(data, len, read)
        'unpack into dictionary 
        Dim s As New StorageData
        Dim ms As New MemoryStream(data)
        Dim gz As New Compression.GZipStream(ms, Compression.CompressionMode.Decompress)
        Dim reader As New BinaryReader(gz)
        Dim itemCount As Integer
        itemCount = reader.ReadInt32
        Dim i As Integer
        For i = 1 To itemCount
            Dim key As String = reader.ReadString
            Dim b As StorageDataType = reader.ReadByte
            Select Case b
                Case StorageDataType.BooleanType
                    s(key) = New StorageDataItem(reader.ReadBoolean())
                Case StorageDataType.DoubleType
                    s(key) = New StorageDataItem(reader.ReadDouble())
                Case StorageDataType.IntegerType
                    s(key) = New StorageDataItem(reader.ReadInt32)
                Case StorageDataType.StringType
                    s(key) = New StorageDataItem(reader.ReadString)
                Case StorageDataType.DoubleArrayType
                    Dim length As Integer = reader.ReadInt32
                    Dim dblData(length - 1) As Double
                    For j As Integer = 0 To length - 1
                        dblData(j) = reader.ReadDouble()
                    Next
                    s(key) = New StorageDataItem(dblData)
                Case StorageDataType.StringArrayType
                    Dim length As Integer = reader.ReadInt32
                    Dim strData(length - 1) As String
                    For j As Integer = 0 To length - 1
                        strData(j) = reader.ReadString()
                    Next
                    s(key) = New StorageDataItem(strData)
                Case Else
                    Debug.Assert(False)
                    Throw New Exception("Internal error: unknown storage data type")
            End Select
        Next
        'load from dictionary 
        Load(s)
        ReleaseIfCOM(pStm)
    End Sub

    Public Sub Save(pStm As System.Runtime.InteropServices.ComTypes.IStream, fClearDirty As Boolean) Implements IPersistStream.Save
        Dim data() As Byte = SaveToMemory()
        Dim written As System.IntPtr = 0
        Dim len As Integer = data.Length
        Dim intBytes() As Byte = BitConverter.GetBytes(len)
        pStm.Write(intBytes, 4, written)
        pStm.Write(data, len, written)
        If fClearDirty Then _dirty = False
        ReleaseIfCOM(pStm)
    End Sub

    Public Sub ProduceReport(ByRef message As String) Implements ICapeUnitReport.ProduceReport
        If currentReport = LastRunReportName Then
            If lastRunReport Is Nothing Then
                message = "Report not available"
            Else
                message = lastRunReport
            End If
            Exit Sub
        End If
        RaiseError("Current report not set", "ProduceReport", "ICapeUnitReport")
    End Sub

    Public ReadOnly Property reports As Object Implements ICapeUnitReport.reports
        Get
            Dim s(0) As String
            s(0) = LastRunReportName
            Return s
        End Get
    End Property

    Public Property selectedReport As String Implements ICapeUnitReport.selectedReport
        Get
            If currentReport Is Nothing Then Return vbNullString
            Return currentReport
        End Get
        Set(value As String)
            If SameString(value, LastRunReportName) Then
                currentReport = LastRunReportName
            Else
                RaiseError("No such report", "put_selectedReport", "ICapeUnitReport")
            End If
        End Set
    End Property

End Class
