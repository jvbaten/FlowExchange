'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class Flow
    Inherits PropertyContainer
    Friend compounds As New List(Of Compound)
    Friend phases As New List(Of Phase)
    'enthalpy is stored separately
    Friend molarHcorrected As Double 'molar enthalpy, J/mol, corrected for reference state
    Friend massHcorrected As Double 'molar enthalpy, J/kg, corrected for reference state
    Friend molarHraw As Double 'molar enthalpy, J/mol, not corrected for reference state
    Friend massHraw As Double 'molar enthalpy, J/kg, not corrected for reference state
    Friend referencePhase As String
    Friend referencePhaseAggState As String
    Friend referenceT As Double
    Friend referenceP As Double
    Friend autoFlash As String 'advice on how to flash the target stream
    Public Overrides Function ToString() As String
        Return "XFlow"
    End Function
    Friend Function Clone() As Flow
        Dim res As New Flow
        res.CopyPropertiesFrom(Me)
        For Each c As Compound In compounds
            res.compounds.Add(c.Clone)
        Next
        For Each p As Phase In phases
            res.phases.Add(p.Clone)
        Next
        res.molarHcorrected = molarHcorrected
        res.massHcorrected = massHcorrected
        res.molarHraw = molarHraw
        res.massHraw = massHraw
        res.referencePhase = referencePhase.Clone
        res.referencePhaseAggState = referencePhaseAggState.Clone
        res.referenceT = referenceT
        res.referenceP = referenceP
        Return res
    End Function
    Friend Sub Save(s As StorageData, baseName As String)
        Dim prefix As String = baseName + "."c
        SaveProperties(s, prefix)
        If Not Double.IsNaN(molarHcorrected) Then s(prefix + "molarH") = New StorageDataItem(molarHcorrected)
        If Not Double.IsNaN(massHcorrected) Then s(prefix + "massH") = New StorageDataItem(massHcorrected)
        If Not referencePhase Is Nothing Then s(prefix + "referencePhase") = New StorageDataItem(referencePhase)
        If Not referencePhaseAggState Is Nothing Then s(prefix + "referencePhaseAggState") = New StorageDataItem(referencePhaseAggState)
        If Not Double.IsNaN(referenceT) Then s(prefix + "referenceT") = New StorageDataItem(referenceT)
        If Not Double.IsNaN(referenceP) Then s(prefix + "referenceP") = New StorageDataItem(referenceP)
        If autoFlash IsNot Nothing Then s(prefix + "autoFlash") = New StorageDataItem(autoFlash)
        Dim cPrefix As String = prefix + "comps."
        Dim names As New List(Of String)
        For Each c As Compound In compounds
            names.Add(c.compID)
            c.Save(s, cPrefix + c.compID + ".")
        Next
        s(cPrefix + "IDs") = New StorageDataItem(names.ToArray)
        Dim pPrefix As String = prefix + "phases."
        names.Clear()
        For Each p As Phase In phases
            names.Add(p.name)
            p.Save(s, pPrefix + p.name + ".")
        Next
        s(pPrefix + "names") = New StorageDataItem(names.ToArray)
    End Sub
    Friend Sub Load(s As StorageData, baseName As String)
        Dim prefix As String = baseName + "."c
        LoadProperties(s, prefix)
        Dim si As StorageDataItem = Nothing
        If s.TryGetValue(prefix + "molarH", si) Then If (si.type = StorageDataType.DoubleType) Then molarHcorrected = CDbl(si.data)
        If s.TryGetValue(prefix + "massH", si) Then If (si.type = StorageDataType.DoubleType) Then massHcorrected = CDbl(si.data)
        If s.TryGetValue(prefix + "referencePhase", si) Then If (si.type = StorageDataType.StringType) Then referencePhase = CStr(si.data)
        If s.TryGetValue(prefix + "referencePhaseAggState", si) Then If (si.type = StorageDataType.StringType) Then referencePhaseAggState = CStr(si.data)
        If s.TryGetValue(prefix + "referenceT", si) Then If (si.type = StorageDataType.DoubleType) Then referenceT = CDbl(si.data)
        If s.TryGetValue(prefix + "referenceP", si) Then If (si.type = StorageDataType.DoubleType) Then referenceP = CDbl(si.data)
        If s.TryGetValue(prefix + "autoFlash", si) Then If (si.type = StorageDataType.StringType) Then autoFlash = CStr(si.data)
        Dim names() As String
        Try
            Dim cPrefix As String = prefix + "comps."
            names = s(cPrefix + "IDs").data
            For Each id In names
                Dim c As New Compound
                c.compID = id
                c.Load(s, cPrefix + id + ".")
                compounds.Add(c)
            Next
        Catch ex As Exception
            Debug.Assert(False)
        End Try
        Try
            Dim pPrefix As String = prefix + "phases."
            names = s(pPrefix + "names").data
            For Each name In names
                Dim p As New Phase
                p.name = name
                p.Load(s, pPrefix + name + ".")
                phases.Add(p)
            Next
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub
    Class CompoundSorterData
        Implements DisplayOrderSorter.ISortOrderItemProvider
        Sub New(c() As Compound)
            Me.c = c
        End Sub
        Dim c() As Compound
        Public Function GetSortOrderItems() As DisplayOrderSorter.ISortOrderItem() Implements DisplayOrderSorter.ISortOrderItemProvider.GetSortOrderItems
            Return c
        End Function
    End Class
    Friend Function GetCompoundSortOrderProvider()
        Return New CompoundSorterData(compounds.ToArray)
    End Function
    Class PhaseSorterData
        Implements DisplayOrderSorter.ISortOrderItemProvider
        Sub New(p() As Phase)
            Me.p = p
        End Sub
        Dim p() As Phase
        Public Function GetSortOrderItems() As DisplayOrderSorter.ISortOrderItem() Implements DisplayOrderSorter.ISortOrderItemProvider.GetSortOrderItems
            Return p
        End Function
    End Class
    Friend Function GetPhaseSortOrderProvider()
        Return New PhaseSorterData(phases.ToArray)
    End Function
End Class
