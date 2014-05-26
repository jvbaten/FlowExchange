'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class StorageDataItem
    Sub New(val As Integer)
        type = StorageDataType.IntegerType
        data = val
    End Sub
    Sub New(val As Boolean)
        type = StorageDataType.BooleanType
        data = val
    End Sub
    Sub New(val As Double)
        type = StorageDataType.DoubleType
        data = val
    End Sub
    Sub New(val As String)
        type = StorageDataType.StringType
        data = val
    End Sub
    Sub New(val As Double())
        type = StorageDataType.DoubleArrayType
        data = val
    End Sub
    Sub New(val As String())
        type = StorageDataType.StringArrayType
        data = val
    End Sub
    Public type As StorageDataType
    Public data As Object
End Class