'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class Phase
    Inherits PropertyContainer
    Implements DisplayOrderSorter.ISortOrderItem
    Friend name As String
    Friend stateOfAggregation As String
    Friend keyCompound As String
    Public Overrides Function ToString() As String
        If String.IsNullOrEmpty(name) Then Return "<unnamed phase>"
        Return "Phase """ + name + """"
    End Function
    Friend Sub Save(s As StorageData, prefix As String)
        SaveProperties(s, prefix)
        If stateOfAggregation IsNot Nothing Then s(prefix + "stateOfAggregation") = New StorageDataItem(stateOfAggregation)
        If keyCompound IsNot Nothing Then s(prefix + "keyCompound") = New StorageDataItem(keyCompound)
    End Sub
    Friend Sub Load(s As StorageData, prefix As String)
        LoadProperties(s, prefix)
        Dim si As StorageDataItem = Nothing
        If s.TryGetValue(prefix + "stateOfAggregation", si) Then If (si.type = StorageDataType.StringType) Then stateOfAggregation = CStr(si.data)
        If s.TryGetValue(prefix + "keyCompound", si) Then If (si.type = StorageDataType.StringType) Then keyCompound = CStr(si.data)
    End Sub
    Public Function GetID() As String Implements DisplayOrderSorter.ISortOrderItem.GetID
        Return name
    End Function
    Friend Function Clone() As Phase
        Dim res As New Phase
        res.CopyPropertiesFrom(Me)
        If name IsNot Nothing Then res.name = name.Clone
        If stateOfAggregation IsNot Nothing Then res.stateOfAggregation = stateOfAggregation.Clone
        If keyCompound IsNot Nothing Then res.keyCompound = keyCompound.Clone
        Return res
    End Function
End Class
