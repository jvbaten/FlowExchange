'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN110
Imports System.Runtime.InteropServices

Public Class CAPEOPENBase
    Implements ICapeIdentification
    Implements ECapeUnknown
    Implements ECapeUser
    Implements ECapeRoot

    Dim _canRename As Boolean
    Dim _name, _description As String 'identification members
    Dim _errDesc, _errScope, _errIface As String 'error members

    Sub New(Optional name As String = Nothing, Optional description As String = Nothing, Optional canRename As Boolean = False)
        If name Is Nothing Or description Is Nothing Then
            'pick them from the attribute
            Dim n As NameDescriptionAttribute = CType(Attribute.GetCustomAttribute(Me.GetType(), GetType(NameDescriptionAttribute)), NameDescriptionAttribute)
            Debug.Assert(n IsNot Nothing)
            If name Is Nothing Then name = n.GetName
            If description Is Nothing Then description = n.GetDescription
        End If
        _name = name
        _description = description
        _canRename = canRename
    End Sub

    Sub RaiseError(desc As String, scope As String, iface As String)
        _errDesc = desc
        _errScope = scope
        _errIface = iface
        Throw New COMException(desc, eCapeErrorInterfaceHR_tag.ECapeUnknownHR)
    End Sub

    'unit operation for saving an XFlow file from a stream

    Public Property ComponentDescription As String Implements ICapeIdentification.ComponentDescription
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
        End Set
    End Property

    Public Property ComponentName As String Implements ICapeIdentification.ComponentName
        Get
            Return _name
        End Get
        Set(value As String)
            If _canRename Then
                _name = value
            Else
                RaiseError("The name of this object cannot be changed", "put_ComponentName", "ICapeIdentification")
            End If
        End Set
    End Property

    Public ReadOnly Property code As Integer Implements ECapeUser.code
        Get
            'not supported
            Return 0
        End Get
    End Property

    Public ReadOnly Property description As String Implements ECapeUser.description
        Get
            Return _errDesc
        End Get
    End Property

    Public ReadOnly Property interfaceName As String Implements ECapeUser.interfaceName
        Get
            Return _errIface
        End Get
    End Property

    Public ReadOnly Property moreInfo As String Implements ECapeUser.moreInfo
        Get
            Return "N/A"
        End Get
    End Property

    Public ReadOnly Property operation As String Implements ECapeUser.operation
        Get
            Return "N/A"
        End Get
    End Property

    Public ReadOnly Property scope As String Implements ECapeUser.scope
        Get
            Return _errScope
        End Get
    End Property

    Public ReadOnly Property name As String Implements ECapeRoot.name
        Get
            Return _errDesc
        End Get
    End Property

    Public Shared Function SameString(ByVal a As String, ByVal b As String) As Boolean
        'CAPE-OPEN strings are case insensitive
        Return (String.Compare(a, b, StringComparison.InvariantCultureIgnoreCase) = 0)
    End Function

    Public Shared Sub ReleaseIfCOM(ByVal o As Object)
        'external objects require explicit releases
        If o.GetType().IsCOMObject Then Marshal.ReleaseComObject(o)
    End Sub

    Public Shared Function ErrorFromCOObject(ByVal o As Object, ByVal e As Exception) As String
        'get an error description from a CAPE-OPEN object
        Dim err As ECapeUser
        Dim res As String
        Dim isCOError As Boolean = False
        If TypeOf (e) Is COMException Then
            If [Enum].IsDefined(GetType(eCapeErrorInterfaceHR_tag), CType(e, COMException).ErrorCode) Then
                isCOError = True
            End If
        End If
        If Not isCOError Then
            'not a CAPE-OPEN error
            Return e.Message
        End If
        Try
            err = o
            Try
                res = err.description
                'if no description, use e
                If (res Is Nothing) Then
                    If (e IsNot Nothing) Then res = e.Message
                ElseIf (res = String.Empty) Then
                    If (e IsNot Nothing) Then res = e.Message
                End If
            Catch ex As Exception
                res = "Failed to get error from CAPE-OPEN object: " + ex.Message
            End Try
        Catch ex As Exception
            If e Is Nothing Then
                res = "Unknown error (ECapeUser not exposed by object)"
            Else
                res = e.Message
            End If
        End Try
        Return res
    End Function

    Public Overrides Function ToString() As String
        Return ComponentName 'note that this is required for items that live in a collection. See Item() of the Generic Collection class
    End Function

End Class
