'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class Compound
    Inherits PropertyContainer
    Implements DisplayOrderSorter.ISortOrderItem
    'these are the details we store for a compound
    Friend compID As String 'ID as exposed by simulation environment
    Friend name As String 'display name
    Friend CAS As String 'CAS registry number, optional
    Friend formula As String 'chemical formula, optional
    Public Overrides Function ToString() As String
        If String.IsNullOrEmpty(name) Then
            If String.IsNullOrEmpty(compID) Then
                Return "<unnamed compound>"
            Else
                Return "Compound """ + compID + """"
            End If
        Else
            Return "Compound """ + name + """"
        End If
    End Function
    Friend Sub Save(s As StorageData, prefix As String)
        SaveProperties(s, prefix)
        If name IsNot Nothing Then s(prefix + "name") = New StorageDataItem(name)
        If CAS IsNot Nothing Then s(prefix + "CAS") = New StorageDataItem(CAS)
        If formula IsNot Nothing Then s(prefix + "formula") = New StorageDataItem(formula)
    End Sub
    Friend Sub Load(s As StorageData, prefix As String)
        LoadProperties(s, prefix)
        Dim si As StorageDataItem = Nothing
        If s.TryGetValue(prefix + "name", si) Then If (si.type = StorageDataType.StringType) Then name = CStr(si.data)
        If s.TryGetValue(prefix + "CAS", si) Then If (si.type = StorageDataType.StringType) Then CAS = CStr(si.data)
        If s.TryGetValue(prefix + "formula", si) Then If (si.type = StorageDataType.StringType) Then formula = CStr(si.data)
        If name Is Nothing Then
            Debug.Assert(False)
            name = compID
        End If
    End Sub
    Public Function GetID() As String Implements DisplayOrderSorter.ISortOrderItem.GetID
        Return name.ToLower 'unique row in a XFlowFileViewer, e.g. combine Methane with methane
    End Function
    Function Clone() As Compound
        Dim res As New Compound
        res.CopyPropertiesFrom(Me)
        If compID IsNot Nothing Then res.compID = compID.Clone
        If name IsNot Nothing Then res.name = name.Clone
        If CAS IsNot Nothing Then res.CAS = CAS.Clone
        If formula IsNot Nothing Then res.formula = formula.Clone
        Return res
    End Function
End Class
