'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN110
Imports FlowExchange.UnitOperationBase

Public MustInherit Class Parameter(Of ValueType)
    Inherits ParameterBase
    Implements ICapeParameter
    Implements ICapeParameterSpec
    Dim _mode As CapeParamMode
    Dim _unit As UnitOperationBase

    Protected _value As ValueType
    Protected _defaultValue As ValueType

    Friend Overrides Sub ResetValue()
        Reset()
    End Sub

    Sub New(unit As UnitOperationBase, name As String, description As String, mode As CapeParamMode, value As ValueType)
        MyBase.New(name, description)
        _mode = mode
        _value = value
        _defaultValue = value
        _unit = unit
    End Sub

    Friend Overridable Function CheckValueValid(value As ValueType, ByRef message As String) As Boolean
        'default implementation, override to validate values
        Return True
    End Function

    Public Property value As Object Implements ICapeParameter.value
        Get
            Return _value
        End Get
        Set(newValue As Object)
            Dim message As String = Nothing
            If Not CheckValueValid(newValue, message) Then RaiseError(message, "put_Value", "ICapeParameter")
            _value = newValue
            _unit.Invalidate()
        End Set
    End Property

    Public Sub Reset() Implements ICapeParameter.Reset
        If (_mode <> CapeParamMode.CAPE_INPUT) Then
            RaiseError("Cannot reset an output parameter", "Reset", "ICapeParameter")
        End If
        _value = _defaultValue
        _unit.Invalidate()
    End Sub

    Public Property Mode As CapeParamMode Implements ICapeParameter.Mode
        Get
            Return _mode
        End Get
        Set(value As CapeParamMode)
            RaiseError("Cannot set mode", "put_Mode", "ICapeParameter")
        End Set
    End Property

    Public ReadOnly Property Specification As Object Implements ICapeParameter.Specification
        Get
            Return Me
        End Get
    End Property

    Public Function Validate(ByRef message As String) As Boolean Implements ICapeParameter.Validate
        'we don't bother with parameter validation
        Return True
    End Function

    Public ReadOnly Property ValStatus As CapeValidationStatus Implements ICapeParameter.ValStatus
        Get
            'we don't bother with parameter validation
            Return CapeValidationStatus.CAPE_VALID
        End Get
    End Property

    Public ReadOnly Property Dimensionality As Object Implements ICapeParameterSpec.Dimensionality
        Get
            'override for something that has a dimensionality
            Dim dimen As Double() = {0, 0, 0}
            Return dimen
        End Get
    End Property

    Public MustOverride ReadOnly Property Type As CapeParamType Implements ICapeParameterSpec.Type

End Class



