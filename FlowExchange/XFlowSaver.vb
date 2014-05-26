'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN110
Imports System.Runtime.InteropServices
Imports System.Text

<Guid("F00AFC4F-477F-4F65-A837-312EE3492187")> _
<ProgId("FlowExchange.XFlowSaver")> _
<ComVisible(True)> _
<NameDescriptionAttribute("XFlowSaver", "Unit operation to export an XFlow stream description file")> _
Public Class XFlowSaver
    Inherits UnitOperationBase
    Friend feedPort As New Port("Feed", "Feed port containing data to be saved", CapePortDirection.CAPE_INLET, Me)
    Dim productPort As New Port("Product", "Product port (optional); copied from feed", CapePortDirection.CAPE_OUTLET, Me)
    Friend xFlowFileName As New XFlowFileNameParameter(Me, False)
    Friend autoSave As New BooleanParameter(Me, "Auto-save", "Save .xflow file upon each calculation", CapeParamMode.CAPE_INPUT, False)
    Friend overallProperties As New List(Of PropertyInstance)
    Friend phaseProperties As New List(Of PropertyInstance)
    Friend compoundProperties As New List(Of PropertyInstance)
    Dim comps As New List(Of Compound)
    Friend currentFlow As Flow
    Friend lastSavedFlow As Flow
    Friend xOptions As New XFlowFileOptions

    Protected Overrides Function TypeName() As String
        Dim n As NameDescriptionAttribute = CType(Attribute.GetCustomAttribute(Me.GetType, GetType(NameDescriptionAttribute)), NameDescriptionAttribute)
        Return n.GetName
    End Function

    Sub New()
        MyBase.New()
        Dim propName As String
        Dim pInfo As PropertyInfo = Nothing
        AddPort(feedPort)
        AddPort(productPort)
        AddParameter(xFlowFileName)
        AddParameter(autoSave)
        'default properties:
        Dim defaultOverallProps() As String = {"Temperature", "Pressure", "Molar Flow", "Mass Flow", "Mole Fraction", "Mass Fraction", "Molar Vapor Fraction", "Mass Vapor Fraction", "Specific Volume", "Molar Volume", "Volume Flow"}
        Dim defaultPhaseProps() As String = {"Molar Phase Fraction", "Mass Phase Fraction", "Mole Fraction", "Mass Fraction"}
        Dim defaultCompoundProps() As String = {"Normal Boiling Point", "Molecular Weight"}
        For Each propName In defaultOverallProps
            pInfo = PropertiesManager.TryGetPropertyInfo(propName)
            If pInfo IsNot Nothing Then
                Debug.Assert(pInfo.allowForOverall)
                overallProperties.Add(pInfo.GetPropertyInstance)
            Else
                Debug.Assert(False)
            End If
        Next
        For Each propName In defaultPhaseProps
            pInfo = PropertiesManager.TryGetPropertyInfo(propName)
            If pInfo IsNot Nothing Then
                Debug.Assert(pInfo.allowForPhase)
                phaseProperties.Add(pInfo.GetPropertyInstance)
            Else
                Debug.Assert(False)
            End If
        Next
        For Each propName In defaultCompoundProps
            pInfo = PropertiesManager.TryGetPropertyInfo(propName)
            If pInfo IsNot Nothing Then
                Debug.Assert(pInfo.allowForCompound)
                compoundProperties.Add(pInfo.GetPropertyInstance)
            Else
                Debug.Assert(False)
            End If
        Next
    End Sub

    Overrides Sub Calculate()
        lastRunReportBuilder = New StringBuilder
        lastRunReportBuilder.Append("Calculation starts at ")
        Dim now As Date = Date.Now
        lastRunReportBuilder.Append(now.ToShortDateString)
        lastRunReportBuilder.Append(", ")
        lastRunReportBuilder.Append(now.ToShortTimeString)
        lastRunReportBuilder.Append(vbCrLf)
        Try
            If (validationStatus <> CapeValidationStatus.CAPE_VALID) Then
                Dim message As String = Nothing
                If Not ValidateUnit(message) Then Throw New Exception(message)
            End If
            Dim feedStream As Stream = feedPort.GetStream
            Debug.Assert(feedStream IsNot Nothing)
            currentFlow = feedStream.GetFlow(comps, overallProperties, phaseProperties, xOptions)
            If (comps.Count = 0) Then Throw New Exception("Material connected to feed streams does not contain compounds")
            If autoSave.value Then
                Try
                    XFlowFile.Write(xFlowFileName.value, currentFlow, xOptions)
                    LogMessage("Stored stream info to " + xFlowFileName.value)
                    lastSavedFlow = currentFlow.Clone
                Catch ex As Exception
                    Throw New Exception("Failed to save XFlow file: " + ex.Message)
                End Try
            End If
            Dim prodStream As Stream = productPort.GetStream
            If prodStream IsNot Nothing Then prodStream.ProductFromFeed(currentFlow, lastRunReportBuilder)
        Catch ex As Exception
            lastRunReportBuilder.Append("error: ")
            lastRunReportBuilder.Append(ex.Message)
            RaiseError(ex.Message, "Calculate", "ICapeUnit")
        End Try
        lastRunReport = lastRunReportBuilder.ToString
        lastRunReportBuilder = Nothing
    End Sub

    Overrides Function ValidateUnit(ByRef message As String) As Boolean
        'check feed port connected
        Try
            Dim feedStream As Stream = feedPort.GetStream
            If feedStream Is Nothing Then
                message = "Feed port is not connected"
                Return False
            End If
            comps = feedStream.GetComps(compoundProperties)
            If (comps.Count = 0) Then Throw New Exception("Material connected to feed streams does not contain compounds")
        Catch ex As Exception
            message = ex.Message
            Return False
        End Try
        If productPort.connectedObject IsNot Nothing Then
            'we need at a minimum flow, composition, pressure and temperature
            Dim haveFlow As Boolean = False
            Dim haveX As Boolean = False
            Dim haveP As Boolean = False
            Dim haveT As Boolean = False
            For Each p As PropertyInstance In overallProperties
                Select Case p.exposedName
                    Case "Mass Compound Flow", "Molar Compound Flow"
                        haveFlow = True
                        haveX = True
                    Case "Mass Flow", "Molar Flow"
                        haveFlow = True
                    Case "Mole Fraction", "Mass Fraction"
                        haveX = True
                    Case "Pressure"
                        haveP = True
                    Case "Temperature"
                        haveT = True
                End Select
            Next
            If Not haveT Then
                message = "Product port is connected; Temperature must be included in saved values"
                Return False
            End If
            If Not haveP Then
                message = "Product port is connected; Pressure must be included in saved values"
                Return False
            End If
            If Not haveFlow Then
                message = "Product port is connected; overall Flow Rate must be included in saved values"
                Return False
            End If
            If Not haveX Then
                message = "Product port is connected; overall Composition must be included in saved values"
                Return False
            End If
        End If
        If autoSave.value Then
            'check valid path
            If String.IsNullOrEmpty(xFlowFileName.value) Then
                message = "Auto-save is enabled, but no xflow file name is specified"
                Return False
            End If
            If Not xFlowFileName.ValidateString(xFlowFileName.value, message) Then
                message = "Auto-save is enabled, but xflow file is not valid: " + message
                Return False
            End If
        End If
        Return True
    End Function

    Overrides Function EditUnit() As Boolean
        Dim dlg As New XFlowSaverDialog(Me)
        dlg.ShowDialog()
        If dlg.ConfigurationChanged Then Invalidate()
        Return dlg.ConfigurationChanged
    End Function

    Friend Overrides Function Save(Optional configOnly As Boolean = False) As StorageData
        Dim s As StorageData
        Dim i As Integer
        s = MyBase.Save
        If Not configOnly Then
            If currentFlow IsNot Nothing Then
                s("hasFlow") = New StorageDataItem(True)
                currentFlow.Save(s, "flow")
            End If
            If lastSavedFlow IsNot Nothing Then
                s("hasSavedFlow") = New StorageDataItem(True)
                lastSavedFlow.Save(s, "savedFlow")
            End If
        End If
        If overallProperties.Count > 0 Then
            Dim prefix As String = "props."
            s(prefix + "count") = New StorageDataItem(overallProperties.Count)
            For i = 1 To overallProperties.Count
                Dim pi As PropertyInstance = overallProperties(i - 1)
                pi.Save(s, prefix + i.ToString + "."c)
            Next
        End If
        If phaseProperties.Count > 0 Then
            Dim prefix As String = "phaseProps."
            s(prefix + "count") = New StorageDataItem(phaseProperties.Count)
            For i = 1 To phaseProperties.Count
                Dim pi As PropertyInstance = phaseProperties(i - 1)
                pi.Save(s, prefix + i.ToString + "."c)
            Next
        Else
            s("xFlowOptions.writePhaseInfo") = New StorageDataItem(xOptions.writePhaseInfo)
        End If
        s("xFlowOptions.writeEnthalpy") = New StorageDataItem(xOptions.writeEnthalpy)
        s("xFlowOptions.writeAutoFlash") = New StorageDataItem(xOptions.writeAutoFlash)
        s("xFlowOptions.writeCreatorComment") = New StorageDataItem(xOptions.writeCreatorComment)
        s("xFlowOptions.writeComments") = New StorageDataItem(xOptions.writeComments)
        If xOptions.writeEnthalpy Then
            If (xOptions.enthalpyReferencePhase IsNot Nothing) Then
                s("xFlowOptions.referencePhase") = New StorageDataItem(xOptions.enthalpyReferencePhase)
                s("xFlowOptions.referenceT") = New StorageDataItem(xOptions.enthalpyReferenceT)
                s("xFlowOptions.referenceP") = New StorageDataItem(xOptions.enthalpyReferenceP)
            End If
        End If
        Return s
    End Function

    Friend Overrides Sub Load(s As StorageData, Optional configOnly As Boolean = False)
        _parameterCollection.ResetValues()
        comps.Clear()
        currentFlow = Nothing
        lastSavedFlow = Nothing
        MyBase.Load(s)
        Dim i, count As Integer
        Dim si As StorageDataItem = Nothing
        If Not configOnly Then
            If s.TryGetValue("hasFlow", si) Then
                If (si.type = StorageDataType.BooleanType) Then
                    If (CBool(si.data)) Then
                        currentFlow = New Flow
                        currentFlow.Load(s, "flow")
                    End If
                End If
            End If
            If s.TryGetValue("hasSavedFlow", si) Then
                If (si.type = StorageDataType.BooleanType) Then
                    If (CBool(si.data)) Then
                        lastSavedFlow = New Flow
                        lastSavedFlow.Load(s, "flow")
                    End If
                End If
            End If
        End If
        overallProperties.Clear()
        Dim prefix As String = "props."
        If s.TryGetValue(prefix + "count", si) Then
            If (si.type = StorageDataType.IntegerType) Then
                count = CInt(si.data)
                For i = 1 To count
                    Try
                        overallProperties.Add(New PropertyInstance(s, prefix + i.ToString + "."c))
                    Catch ex As Exception
                        Debug.Assert(False)
                    End Try
                Next
            End If
        End If
        phaseProperties.Clear()
        prefix = "phaseProps."
        If s.TryGetValue(prefix + "count", si) Then
            If (si.type = StorageDataType.IntegerType) Then
                count = CInt(si.data)
                For i = 1 To count
                    Try
                        phaseProperties.Add(New PropertyInstance(s, prefix + i.ToString + "."c))
                    Catch ex As Exception
                        Debug.Assert(False)
                    End Try
                Next
            End If
        End If
        If s.TryGetValue("xFlowOptions.writePhaseInfo", si) Then
            If si.type = StorageDataType.BooleanType Then
                xOptions.writePhaseInfo = CBool(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.writeEnthalpy", si) Then
            If si.type = StorageDataType.BooleanType Then
                xOptions.writeEnthalpy = CBool(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.writeAutoFlash", si) Then
            If si.type = StorageDataType.BooleanType Then
                xOptions.writeAutoFlash = CBool(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.writeCreatorComment", si) Then
            If si.type = StorageDataType.BooleanType Then
                xOptions.writeCreatorComment = CBool(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.writeComments", si) Then
            If si.type = StorageDataType.BooleanType Then
                xOptions.writeComments = CBool(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.referencePhase", si) Then
            If si.type = StorageDataType.StringType Then
                xOptions.enthalpyReferencePhase = CStr(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.referenceT", si) Then
            If si.type = StorageDataType.DoubleType Then
                xOptions.enthalpyReferenceT = CDbl(si.data)
            End If
        End If
        If s.TryGetValue("xFlowOptions.referenceP", si) Then
            If si.type = StorageDataType.DoubleType Then
                xOptions.enthalpyReferenceP = CDbl(si.data)
            End If
        End If
    End Sub

End Class
