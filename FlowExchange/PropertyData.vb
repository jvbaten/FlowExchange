'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class PropertyData
    Sub New(name As String, value As Double, Optional unitOfMeasure As String = Nothing)
        Me.name = name
        ReDim Me.value(0)
        Me.value(0) = value
        unit = unitOfMeasure
    End Sub
    Sub New(name As String, value() As Double, Optional unitOfMeasure As String = Nothing)
        Me.name = name
        Me.value = value.Clone
        unit = unitOfMeasure
    End Sub
    Friend name As String
    Friend unit As String
    Friend value() As Double
    Friend Function Clone() As PropertyData
        Return New PropertyData(name, value, unit)
    End Function
End Class
