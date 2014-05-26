'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Class PropertyInstance
    Sub New(exposedName As String, name10 As String, name11 As String, Optional unit As String = Nothing, Optional basis As String = Nothing)
        Me.exposedName = exposedName
        Me.name10 = name10
        Me.name11 = name11
        Me.unit = unit
        Me.basis = basis
    End Sub
    Friend exposedName As String
    Friend name10 As String
    Friend name11 As String 'this is what appears in the xflow file
    Friend unit As String = Nothing
    Friend basis As String = Nothing 'valid are: Nothing, "mole", "mass", "flow"
    Public Overrides Function ToString() As String
        If String.IsNullOrEmpty(unit) Then Return exposedName
        Dim sb As New System.Text.StringBuilder
        sb.Append(exposedName)
        sb.Append(" / [")
        sb.Append(unit)
        sb.Append("]"c)
        Return sb.ToString
    End Function
    Friend Sub Save(s As StorageData, prefix As String)
        If exposedName IsNot Nothing Then s(prefix + "name") = New StorageDataItem(exposedName)
        If name10 IsNot Nothing Then s(prefix + "name10") = New StorageDataItem(name10)
        If name11 IsNot Nothing Then s(prefix + "name11") = New StorageDataItem(name11)
        If unit IsNot Nothing Then s(prefix + "unit") = New StorageDataItem(unit)
        If basis IsNot Nothing Then s(prefix + "basis") = New StorageDataItem(basis)
    End Sub
    Friend Sub New(s As StorageData, prefix As String)
        Dim si As StorageDataItem = Nothing
        If s.TryGetValue(prefix + "name", si) Then If (si.type = StorageDataType.StringType) Then exposedName = CStr(si.data)
        If s.TryGetValue(prefix + "name10", si) Then If (si.type = StorageDataType.StringType) Then name10 = CStr(si.data)
        If s.TryGetValue(prefix + "name11", si) Then If (si.type = StorageDataType.StringType) Then name11 = CStr(si.data)
        If s.TryGetValue(prefix + "unit", si) Then If (si.type = StorageDataType.StringType) Then unit = CStr(si.data)
        If s.TryGetValue(prefix + "basis", si) Then If (si.type = StorageDataType.StringType) Then basis = CStr(si.data)
    End Sub
    'this is just a helper for the dialog
    Friend selected As Boolean
End Class
