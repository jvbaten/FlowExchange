'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

<NameDescriptionAttribute("Parameter Collection", "FlowExchange parameter collection")> _
Public Class ParameterCollection
    Inherits Collection(Of ParameterBase)

    Friend Sub Save(d As StorageData)
        For Each p As ParameterBase In itemList
            p.Save(d)
        Next
    End Sub

    Friend Sub Load(d As StorageData)
        For Each p As ParameterBase In itemList
            p.Load(d)
        Next
    End Sub

    Friend Sub ResetValues()
        For Each p As ParameterBase In itemList
            p.ResetValue()
        Next
    End Sub

End Class
