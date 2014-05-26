'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN110

Public Class OptionParameter
    Inherits Parameter(Of String)
    Implements ICapeOptionParameterSpec

    Sub New(unit As UnitOperationBase, name As String, description As String, mode As CapeParamMode, value As String, options() As String, exclusive As Boolean)
        MyBase.New(unit, name, description, mode, value)
        _exclusive = exclusive
        _options = options
    End Sub

    Public Overrides ReadOnly Property Type As CapeParamType
        Get
            Return CapeParamType.CAPE_OPTION
        End Get
    End Property

    Dim _options() As String
    Dim _exclusive As Boolean

    Public ReadOnly Property DefaultValue As String Implements ICapeOptionParameterSpec.DefaultValue
        Get
            Return _defaultValue
        End Get
    End Property

    Public ReadOnly Property OptionList As Object Implements ICapeOptionParameterSpec.OptionList
        Get
            Return _options
        End Get
    End Property

    Public ReadOnly Property RestrictedToList As Boolean Implements ICapeOptionParameterSpec.RestrictedToList
        Get
            Return _exclusive
        End Get
    End Property

    Public Overridable Function ValidateString(value As String, ByRef message As String) As Boolean Implements ICapeOptionParameterSpec.Validate
        If (_exclusive) Then
            For Each s As String In _options
                If String.Compare(s, value) = 0 Then Return True
            Next
            Return False
        End If
        Return True
    End Function

    Friend Overrides Sub Load(d As StorageData)
        Dim s As StorageDataItem = Nothing
        _value = Nothing
        If (d.TryGetValue(ComponentName, s)) Then
            If s.type = StorageDataType.StringType Then
                _value = CStr(s.data)
            End If
        End If
    End Sub

    Friend Overrides Sub Save(d As StorageData)
        If (_value IsNot Nothing) Then d(ComponentName) = New StorageDataItem(_value)
    End Sub

End Class
