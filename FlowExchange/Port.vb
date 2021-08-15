'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN

Public Class Port
    Inherits CAPEOPENBase
    Implements ICapeUnitPort
    Dim _direction As CapePortDirection
    Dim _unit As UnitOperationBase
    'version 1.0 material object
    Friend material10 As ICapeThermoMaterialObject
    'version 1.1 material object
    Friend material11 As ICapeThermoMaterial

    Sub New(name As String, description As String, direction As CapePortDirection, unit As UnitOperationBase)
        MyBase.New(name, description)
        _direction = direction
        _unit = unit
    End Sub

    Friend Sub ReleaseObjects()
        If material10 IsNot Nothing Then
            ReleaseIfCOM(material10)
            material10 = Nothing
            Debug.Assert(material11 Is Nothing)
        ElseIf (material11 IsNot Nothing) Then
            ReleaseIfCOM(material11)
            material11 = Nothing
        End If
    End Sub

    Friend Function GetStream() As Stream
        If (material10 IsNot Nothing) Then Return New Stream10(_unit, material10)
        If (material11 IsNot Nothing) Then Return New Stream11(_unit, material11)
        Return Nothing
    End Function

    Public Sub Connect(objectToConnect As Object) Implements ICapeUnitPort.Connect
        'release old
        _unit.Invalidate()
        ReleaseObjects()
        If objectToConnect IsNot Nothing Then
            'check for material 1.1 
            Try
                material11 = objectToConnect
            Catch
                'check for material 1.0
                Try
                    material10 = objectToConnect
                Catch
                    RaiseError("Object does not expose CAPE-OPEN version 1.0 or 1.1 material object interfaces", "Connect", "ICapeUnitPort")
                End Try
            End Try
        End If
    End Sub

    Public ReadOnly Property connectedObject As Object Implements ICapeUnitPort.connectedObject
        Get
            If material10 IsNot Nothing Then Return material10
            If material11 IsNot Nothing Then Return material11
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property direction As CapePortDirection Implements ICapeUnitPort.direction
        Get
            Return _direction
        End Get
    End Property

    Public Sub Disconnect() Implements ICapeUnitPort.Disconnect
        _unit.Invalidate()
        ReleaseObjects()
    End Sub

    Public ReadOnly Property portType As CapePortType Implements ICapeUnitPort.portType
        Get
            Return CapePortType.CAPE_MATERIAL
        End Get
    End Property

End Class
