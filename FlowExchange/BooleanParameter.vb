'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN

Public Class BooleanParameter
    Inherits Parameter(Of Boolean)
    Implements ICapeBooleanParameterSpec

    Sub New(unit As UnitOperationBase, name As String, description As String, mode As CapeParamMode, value As Boolean)
        MyBase.New(unit, name, description, mode, value)
    End Sub

    Public Overrides ReadOnly Property Type As CapeParamType
        Get
            Return CapeParamType.CAPE_BOOLEAN
        End Get
    End Property

    Public ReadOnly Property DefaultValue As Boolean Implements ICapeBooleanParameterSpec.DefaultValue
        Get
            Return _defaultValue
        End Get
    End Property

    Public Function ValidateBoolean(value As Boolean, ByRef message As String) As Boolean Implements ICapeBooleanParameterSpec.Validate
        Return True
    End Function

    Friend Overrides Sub Load(d As StorageData)
        Dim s As StorageDataItem = Nothing
        If (d.TryGetValue(ComponentName, s)) Then
            If s.type = StorageDataType.BooleanType Then
                _value = CBool(s.data)
            End If
        End If
    End Sub

    Friend Overrides Sub Save(d As StorageData)
        d(ComponentName) = New StorageDataItem(_value)
    End Sub

End Class
