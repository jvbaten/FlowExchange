'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN
Imports System.Runtime.InteropServices
Imports System.Text

<Guid("A64B7DE8-6398-42F6-9F6C-49E832D2F625")>
<ProgId("FlowExchange.XFlowLoader")>
<ComVisible(True)>
<NameDescriptionAttribute("XFlowLoader", "Unit operation to import an XFlow stream description file")>
Public Class XFlowLoader
    Inherits UnitOperationBase
    Dim productPort As New Port("Product", "Product port; specified by external .xflow file", CapePortDirection.CAPE_OUTLET, Me)
    Friend xFlowFileName As New XFlowFileNameParameter(Me, False)
    Friend autoLoad As New BooleanParameter(Me, "Auto-load", "Load .xflow file upon each calculation", CapeParamMode.CAPE_INPUT, False)
    Friend comps As New List(Of Compound)
    Friend loadedFlow As Flow
    Friend productFlow As Flow
    Friend flashType As ProductFlashOptions.FlashType = ProductFlashOptions.FlashType.Automatic
    Friend compoundMapping As New Dictionary(Of String, String) 'ID to ID, special cases of target ID: Purge (means get rid of it); Unused (error if non-zero)
    Dim overallPropList As New List(Of PropertyInstance)
    Dim phasePropList As New List(Of PropertyInstance)
    Dim compoundPropList As New List(Of PropertyInstance)
    Dim phaseDict As New Dictionary(Of String, Boolean)
    Dim compoundDict As New Dictionary(Of String, Boolean)
    'set in validation, used at calculation
    Dim runFlashType As ProductFlashOptions.FlashType
    Dim flowRateMass As Double, compositionMass() As Double, flowRateMole As Double, compositionMole() As Double
    Dim p As Double, T As Double, hMass As Double, hMole As Double, vaporFractionMass As Double, vaporFractionMole As Double
    Protected Overrides Function TypeName() As String
        Dim n As NameDescriptionAttribute = CType(Attribute.GetCustomAttribute(Me.GetType, GetType(NameDescriptionAttribute)), NameDescriptionAttribute)
        Return n.GetName
    End Function

    Sub New()
        MyBase.New()
        AddPort(productPort)
        AddParameter(xFlowFileName)
        AddParameter(autoLoad)
    End Sub

    Overrides Sub Calculate()
        'validation includes preparing for all options
        lastRunReportBuilder = New StringBuilder
        lastRunReportBuilder.Append("Calculation starts at ")
        Dim now As Date = Date.Now
        lastRunReportBuilder.Append(now.ToShortDateString)
        lastRunReportBuilder.Append(", ")
        lastRunReportBuilder.Append(now.ToShortTimeString)
        lastRunReportBuilder.Append(vbCrLf)
        Try
            Dim message As String = Nothing
            If Not ValidateUnit(message) Then Throw New Exception(message)
            Dim prodStream As Stream = productPort.GetStream
            Debug.Assert(prodStream IsNot Nothing)
            Dim scalar(0) As Double
            Dim i As Integer
            'set properties and flash
            If compositionMole IsNot Nothing Then
                Try
                    prodStream.SetOverallProp("fraction", "mole", compositionMole, False)
                    lastRunReportBuilder.Append("Setting composition to {")
                    For i = 0 To compositionMole.Length - 1
                        If i > 0 Then lastRunReportBuilder.Append(";"c)
                        lastRunReportBuilder.Append(compositionMole(i).ToString("g5"))
                    Next
                    lastRunReportBuilder.Append("} mol/mol")
                    lastRunReportBuilder.Append(vbCrLf)
                Catch ex As Exception
                    If compositionMass IsNot Nothing Then
                        GoTo setMassComposition
                    Else
                        Throw New Exception("Unable to set product composition: " + ex.Message)
                    End If
                End Try
            Else
                Debug.Assert(compositionMass IsNot Nothing)
setMassComposition:
                Try
                    prodStream.SetOverallProp("fraction", "mass", compositionMass, False)
                    lastRunReportBuilder.Append("Setting composition to {")
                    For i = 0 To compositionMole.Length - 1
                        If i > 0 Then lastRunReportBuilder.Append(";"c)
                        lastRunReportBuilder.Append(compositionMole(i).ToString("g5"))
                    Next
                    lastRunReportBuilder.Append("} kg/kg")
                    lastRunReportBuilder.Append(vbCrLf)
                Catch ex As Exception
                    Throw New Exception("Unable to set product composition: " + ex.Message)
                End Try
            End If
            If Not Double.IsNaN(flowRateMole) Then
                Try
                    scalar(0) = flowRateMole
                    prodStream.SetOverallProp("totalflow", "mole", scalar, False)
                    lastRunReportBuilder.Append("Setting flow rate to ")
                    lastRunReportBuilder.Append(flowRateMole.ToString("g5"))
                    lastRunReportBuilder.Append(" mol/s")
                    lastRunReportBuilder.Append(vbCrLf)
                Catch ex As Exception
                    If Not Double.IsNaN(flowRateMass) Then
                        GoTo setMassFlowRate
                    Else
                        Throw New Exception("Unable to set product flow rate: " + ex.Message)
                    End If
                End Try
            Else
                Debug.Assert(Not Double.IsNaN(flowRateMass))
setMassFlowRate:
                Try
                    scalar(0) = flowRateMass
                    prodStream.SetOverallProp("totalflow", "mass", scalar, False)
                    lastRunReportBuilder.Append("Setting flow rate to ")
                    lastRunReportBuilder.Append(flowRateMass.ToString("g5"))
                    lastRunReportBuilder.Append(" kg/s")
                    lastRunReportBuilder.Append(vbCrLf)
                Catch ex As Exception
                    Throw New Exception("Unable to set product flow rate: " + ex.Message)
                End Try
            End If
            Try
                scalar(0) = p
                prodStream.SetOverallProp("pressure", vbNullString, scalar, False)
                lastRunReportBuilder.Append("Setting pressure to ")
                lastRunReportBuilder.Append(p.ToString("g5"))
                lastRunReportBuilder.Append(" Pa")
                lastRunReportBuilder.Append(vbCrLf)
            Catch ex As Exception
                Throw New Exception("Unable to set product pressure: " + ex.Message)
            End Try
            Select Case runFlashType
                Case ProductFlashOptions.FlashType.PH
                    If Not Double.IsNaN(hMole) Then
                        Try
                            scalar(0) = hMole
                            prodStream.SetOverallProp("enthalpy", "mole", scalar)
                            lastRunReportBuilder.Append("Setting enthalpy to ")
                            lastRunReportBuilder.Append(hMole.ToString("g5"))
                            lastRunReportBuilder.Append(" J/mol")
                            lastRunReportBuilder.Append(vbCrLf)
                        Catch ex As Exception
                            If Not Double.IsNaN(hMass) Then
                                GoTo setMassEnthalpy
                            Else
                                Throw New Exception("Unable to set product enthalpy: " + ex.Message)
                            End If
                        End Try
                    Else
                        Debug.Assert(Not Double.IsNaN(hMass))
setMassEnthalpy:
                        Try
                            scalar(0) = hMass
                            prodStream.SetOverallProp("enthalpy", "mass", scalar)
                            lastRunReportBuilder.Append("Setting enthalpy to ")
                            lastRunReportBuilder.Append(hMass.ToString("g5"))
                            lastRunReportBuilder.Append(" J/kg")
                            lastRunReportBuilder.Append(vbCrLf)
                        Catch ex As Exception
                            Throw New Exception("Unable to set product enthalpy: " + ex.Message)
                        End Try
                    End If
                    Try
                        prodStream.PHFlash()
                        lastRunReportBuilder.Append("Performing PH flash")
                        lastRunReportBuilder.Append(vbCrLf)
                    Catch ex As Exception
                        Throw New Exception("Product PH flash failure: " + ex.Message)
                    End Try
                Case ProductFlashOptions.FlashType.PVF
                    Try
                        If Double.IsNaN(vaporFractionMole) Then Throw New Exception("Internal error: unknown molar vapor fraction") 'should have been dealt with at Validate
                        scalar(0) = vaporFractionMole
                        Dim vaporPhaseName As String
                        Try
                            vaporPhaseName = prodStream.GetVaporPhaseName
                        Catch ex As Exception
                            Throw New Exception("Unable to determine vapor phase name: " + ex.Message)
                        End Try
                        If vaporPhaseName Is Nothing Then Throw New Exception("Unable to determine vapor phase name")
                        prodStream.SetSinglePhaseProp("phaseFraction", vaporPhaseName, "mole", scalar, False)
                        lastRunReportBuilder.Append("Setting vapor fraction to ")
                        lastRunReportBuilder.Append(vaporFractionMole.ToString("g5"))
                        lastRunReportBuilder.Append(" mol/mol")
                        lastRunReportBuilder.Append(vbCrLf)
                    Catch ex As Exception
                        Throw New Exception("Unable to set vapor fraction: " + ex.Message)
                    End Try
                    Try
                        prodStream.PVFFlash()
                        lastRunReportBuilder.Append("Performing P-VF flash")
                        lastRunReportBuilder.Append(vbCrLf)
                    Catch ex As Exception
                        Throw New Exception("Product P-VF flash failure: " + ex.Message)
                    End Try
                Case ProductFlashOptions.FlashType.TP
                    Try
                        scalar(0) = T
                        prodStream.SetOverallProp("temperature", vbNullString, scalar, False)
                        lastRunReportBuilder.Append("Setting temperature to ")
                        lastRunReportBuilder.Append(T.ToString("g5"))
                        lastRunReportBuilder.Append(" K")
                        lastRunReportBuilder.Append(vbCrLf)
                    Catch ex As Exception
                        Throw New Exception("Unable to set product temperature: " + ex.Message)
                    End Try
                    Try
                        prodStream.TPFlash()
                        lastRunReportBuilder.Append("Performing TP flash")
                        lastRunReportBuilder.Append(vbCrLf)
                    Catch ex As Exception
                        Throw New Exception("Product TP flash failure: " + ex.Message)
                    End Try
                Case Else
                    Throw New Exception("Internal error, undetermined flash type")
            End Select
            Try
                'make a list of property instances that appear in the input
                Dim present As Boolean = False
                overallPropList.Clear()
                phasePropList.Clear()
                compoundPropList.Clear()
                phaseDict.Clear()
                compoundDict.Clear()
                Dim checkPhasePropDict As New Dictionary(Of String, Boolean)
                For Each e As KeyValuePair(Of String, PropertyData) In loadedFlow
                    Dim pInfo As PropertyInfo = Nothing
                    pInfo = PropertiesManager.TryGetPropertyInfo(e.Key)
                    If pInfo IsNot Nothing Then
                        If pInfo.allowForOverall Then
                            overallPropList.Add(pInfo.GetPropertyInstance)
                        End If
                    End If
                Next
                For Each p As Phase In loadedFlow.phases
                    For Each e As KeyValuePair(Of String, PropertyData) In p
                        Dim pInfo As PropertyInfo = Nothing
                        pInfo = PropertiesManager.TryGetPropertyInfo(e.Key)
                        If pInfo IsNot Nothing Then
                            If pInfo.allowForPhase Then
                                If Not phaseDict.TryGetValue(pInfo.exposedName, present) Then
                                    phaseDict(pInfo.exposedName) = True
                                    phasePropList.Add(pInfo.GetPropertyInstance)
                                End If
                            End If
                        End If
                    Next
                Next
                For Each c As Compound In loadedFlow.compounds
                    For Each e As KeyValuePair(Of String, PropertyData) In c
                        Dim pInfo As PropertyInfo = Nothing
                        pInfo = PropertiesManager.TryGetPropertyInfo(e.Key)
                        If pInfo IsNot Nothing Then
                            If pInfo.allowForCompound Then
                                If Not compoundDict.TryGetValue(pInfo.exposedName, present) Then
                                    compoundDict(pInfo.exposedName) = True
                                    compoundPropList.Add(pInfo.GetPropertyInstance)
                                End If
                            End If
                        End If
                    Next
                Next
                Dim xoptions As New XFlowFileOptions
                xoptions.writeEnthalpy = False
                xoptions.writeAutoFlash = False
                If (phasePropList.Count = 0) Then xoptions.writePhaseInfo = False
                productFlow = prodStream.GetFlow(comps, overallPropList, phasePropList, xoptions)
            Catch ex As Exception
                LogMessage("Failed to store product flow data for comparison: " + ex.Message, MessageType.WarningMessage)
                productFlow = Nothing
            End Try
        Catch ex As Exception
            lastRunReportBuilder.Append("error: ")
            lastRunReportBuilder.Append(ex.Message)
            RaiseError(ex.Message, "Calculate", "ICapeUnit")
        End Try
        lastRunReport = lastRunReportBuilder.ToString
        lastRunReportBuilder = Nothing
    End Sub

    Overrides Function ValidateUnit(ByRef message As String) As Boolean
        'check product connected
        Dim prodStream As Stream
        Try
            prodStream = productPort.GetStream
            If prodStream Is Nothing Then
                message = "Product port is not connected"
                Return False
            End If
            comps = prodStream.GetComps(compoundPropList)
            If (comps.Count = 0) Then Throw New Exception("Material connected to product streams does not contain compounds")
            If autoLoad.value Then
                'check valid path
                If String.IsNullOrEmpty(xFlowFileName.value) Then
                    message = "Auto-load is enabled, but no xflow file name is specified"
                    Return False
                End If
                If Not xFlowFileName.ValidateString(xFlowFileName.value, message) Then
                    message = "Auto-load is enabled, but xflow file is not valid: " + message
                    Return False
                End If
                'auto load X file
                Try
                    Dim loadWarnings As New List(Of String)
                    loadedFlow = XFlowFile.Read(xFlowFileName.value, loadWarnings)
                    If lastRunReportBuilder IsNot Nothing Then
                        lastRunReportBuilder.Append("Reading XFlow file from: " + xFlowFileName.value)
                        lastRunReportBuilder.Append(vbCrLf)
                    End If
                    For Each s As String In loadWarnings
                        LogMessage(s, MessageType.WarningMessage)
                    Next
                Catch ex As Exception
                    message = "Failed to load """ + xFlowFileName.value + """: " + message
                    Return False
                End Try
            End If
            If loadedFlow Is Nothing Then
                message = "No .xflow file is currently loaded"
                Return False
            End If
            If loadedFlow.compounds.Count = 0 Then
                message = "Loaded .xflow file does not contain compounds"
                Return False
            End If
            'we need at a minimum flow, composition, pressure and temperature
            Dim d() As Double = Nothing
            compositionMass = Nothing
            compositionMole = Nothing
            flowRateMass = Double.NaN
            flowRateMole = Double.NaN
            d = loadedFlow.TryGetPropertyValue("Molar Compound Flow", "mol/s")
            If d IsNot Nothing Then
                If d.Length <> loadedFlow.compounds.Count Then
                    message = "Invalid length of Compound Flow Rate in xflow file; mismatch with compound count"
                    Return False
                End If
                flowRateMole = 0
                For i = 0 To d.Length - 1
                    flowRateMole += d(i)
                Next
                If flowRateMole > 0 Then
                    ReDim compositionMole(d.Length - 1)
                    Dim norm As Double = 1.0 / flowRateMole
                    For i = 0 To d.Length - 1
                        compositionMole(i) = d(i) * norm
                    Next
                End If
            End If
            d = loadedFlow.TryGetPropertyValue("Molar Compound Flow", "kg/s")
            If d IsNot Nothing Then
                If d.Length <> loadedFlow.compounds.Count Then
                    message = "Invalid length of Compound Flow Rate in xflow file; mismatch with compound count"
                    Return False
                End If
                flowRateMass = 0
                For i = 0 To d.Length - 1
                    flowRateMass += d(i)
                Next
                If flowRateMass > 0 Then
                    ReDim compositionMass(d.Length - 1)
                    Dim norm As Double = 1.0 / flowRateMass
                    For i = 0 To d.Length - 1
                        compositionMass(i) = d(i) * norm
                    Next
                End If
            End If
            If compositionMole Is Nothing Then
                d = loadedFlow.TryGetPropertyValue("Molar Flow", "mol/s")
                If d IsNot Nothing Then
                    If d.Length <> 1 Then
                        message = "Invalid length of Flow Rate in xflow file: expected scalar"
                        Return False
                    End If
                    compositionMole = loadedFlow.TryGetPropertyValue("Mole Fraction", "mol/mol")
                    If compositionMole IsNot Nothing Then
                        If compositionMole.Length <> loadedFlow.compounds.Count Then
                            message = "Invalid length of Composition in xflow file; mismatch with compound count"
                            Return False
                        End If
                        flowRateMole = d(0)
                    End If
                End If
            End If
            If compositionMass Is Nothing Then
                d = loadedFlow.TryGetPropertyValue("Mass Flow", "kg/s")
                If d IsNot Nothing Then
                    If d.Length <> 1 Then
                        message = "Invalid length of Flow Rate in xflow file: expected scalar"
                        Return False
                    End If
                    compositionMass = loadedFlow.TryGetPropertyValue("Mass Fraction", "kg/kg")
                    If compositionMass IsNot Nothing Then
                        If compositionMass.Length <> loadedFlow.compounds.Count Then
                            message = "Invalid length of Composition in xflow file; mismatch with compound count"
                            Return False
                        End If
                        flowRateMass = d(0)
                    End If
                End If
            End If
            If compositionMole Is Nothing And compositionMass Is Nothing Then
                message = "Unable to determine overall flow rate and composition in consistent bases from loaded xflow file"
                Return False
            End If
            d = loadedFlow.TryGetPropertyValue("Pressure", "Pa")
            If d Is Nothing Then
                message = "Loaded XFlow file does not contain overall pressure."
                Return False
            End If
            If d.Length <> 1 Then
                message = "Invalid length of Pressure in xflow file: expected scalar"
                Return False
            End If
            p = d(0)
            'determine compound mapping
            ' - If any compound gets purged, disallow any other flash type than TP, and disallow zero flow (we need to revise flow and composition)
            Dim compoundMap(loadedFlow.compounds.Count - 1) As Integer
            Dim targetIndexMap As New Dictionary(Of String, Integer)
            For i = 0 To comps.Count - 1
                targetIndexMap(comps(i).compID.ToLower) = i
            Next
            Dim purged As Boolean = False
            For i = 0 To loadedFlow.compounds.Count - 1
                Dim comp As Compound = loadedFlow.compounds(i)
                'check if mapped
                Dim mappedTo As String = Nothing
                If Not compoundMapping.TryGetValue(comp.compID, mappedTo) Then
                    'not mapped, see if we can auto map
                    mappedTo = AutoMap(comp)
                    If mappedTo Is Nothing Then
                        message = "Compound """ + comp.name + """ is not mapped. Please manually map this compound from the XFlowLoader user interface"
                        Return False
                    End If
                    compoundMapping(comp.compID) = mappedTo
                End If
                'check special cases
                If mappedTo = "Purge" Then
                    Dim isZero As Boolean
                    If compositionMass IsNot Nothing Then
                        isZero = (compositionMass(i) = 0)
                    Else
                        Debug.Assert(compositionMole IsNot Nothing)
                        isZero = (compositionMole(i) = 0)
                    End If
                    If Not isZero Then purged = True ' we have at least one purged compound
                    compoundMap(i) = -1
                    If lastRunReportBuilder IsNot Nothing Then
                        lastRunReportBuilder.Append("Purging compound ")
                        lastRunReportBuilder.Append(comp.name)
                        lastRunReportBuilder.Append(vbCrLf)
                    End If
                ElseIf mappedTo = "Unused" Then
                    Dim isZero As Boolean
                    If compositionMass IsNot Nothing Then
                        isZero = (compositionMass(i) = 0)
                    Else
                        Debug.Assert(compositionMole IsNot Nothing)
                        isZero = (compositionMole(i) = 0)
                    End If
                    If Not isZero Then
                        message = "Compound """ + comp.name + """ is mapped to Unused; the composition of this compound must be zero and is non-zero in the loaded XFlow file."
                        Return False
                    End If
                    If lastRunReportBuilder IsNot Nothing Then
                        lastRunReportBuilder.Append("Ignoring compound ")
                        lastRunReportBuilder.Append(comp.name)
                        lastRunReportBuilder.Append(vbCrLf)
                    End If
                    compoundMap(i) = -1
                Else
                    'get target compound index
                    If Not targetIndexMap.TryGetValue(mappedTo.ToLower, compoundMap(i)) Then
                        message = "Compound """ + comp.name + """ is mapped to a compound with ID """ + mappedTo + """ which is not present in the product stream"
                        Return False
                    End If
                    If lastRunReportBuilder IsNot Nothing Then
                        lastRunReportBuilder.Append("Compound ")
                        lastRunReportBuilder.Append(comp.name)
                        lastRunReportBuilder.Append(" maps to compound ")
                        lastRunReportBuilder.Append(comps(compoundMap(i)).name)
                        lastRunReportBuilder.Append(vbCrLf)
                    End If
                End If
            Next
            If purged Then
                'multiply existing compositions with flow rate (as we need to redetermine flow rate after purge
                If compositionMole IsNot Nothing Then
                    For i = 0 To compositionMole.Length - 1
                        compositionMole(i) *= flowRateMole
                    Next
                End If
                If compositionMass IsNot Nothing Then
                    For i = 0 To compositionMass.Length - 1
                        compositionMass(i) *= flowRateMass
                    Next
                End If
            End If
            'map compositions (or compound flows, in case we purged something)
            Dim compositionNew(comps.Count - 1) As Double
            If compositionMole IsNot Nothing Then
                For i = 0 To comps.Count - 1
                    compositionNew(i) = 0
                Next
                For i = 0 To compositionMole.Length - 1
                    If compoundMap(i) >= 0 Then compositionNew(compoundMap(i)) += compositionMole(i)
                Next
                compositionMole = compositionNew.Clone
            End If
            If compositionMass IsNot Nothing Then
                For i = 0 To comps.Count - 1
                    compositionNew(i) = 0
                Next
                For i = 0 To compositionMass.Length - 1
                    If compoundMap(i) >= 0 Then compositionNew(compoundMap(i)) += compositionMass(i)
                Next
                compositionMass = compositionNew
            End If
            If purged Then
                'reconstruct flow and composition
                If compositionMole IsNot Nothing Then
                    flowRateMole = 0
                    For i = 0 To compositionMole.Length - 1
                        flowRateMole += compositionMole(i)
                    Next
                    If (flowRateMole <= 0) Then
                        message = "One or more compounds are purged; resulting flow rate is zero. Cannot re-normalize composition after compound mapping"
                        Return False
                    End If
                    Dim norm As Double = 1.0 / flowRateMole
                    For i = 0 To compositionMole.Length - 1
                        compositionMole(i) *= norm
                    Next
                End If
                If compositionMass IsNot Nothing Then
                    flowRateMass = 0
                    For i = 0 To compositionMass.Length - 1
                        flowRateMass += compositionMass(i)
                    Next
                    If (flowRateMass <= 0) Then
                        message = "One or more compounds are purged; resulting flow rate is zero. Cannot re-normalize composition after compound mapping"
                        Return False
                    End If
                    Dim norm As Double = 1.0 / flowRateMass
                    For i = 0 To compositionMass.Length - 1
                        compositionMass(i) *= norm
                    Next
                End If
            End If
            'determine flash type and required flash property
            runFlashType = flashType
            If runFlashType = ProductFlashOptions.FlashType.Automatic Then
                If loadedFlow.autoFlash IsNot Nothing Then runFlashType = ProductFlashOptions.GetFlashType(loadedFlow.autoFlash)
            End If
            If runFlashType = ProductFlashOptions.FlashType.Automatic Then
                'check PVF flash from phase collection
                If loadedFlow.phases.Count > 1 Then
                    For Each p As Phase In loadedFlow.phases
                        If SameString(p.stateOfAggregation, "Vapor") Then
                            d = p.TryGetPropertyValue("Molar Phase Fraction", "mol/mol")
                            If d Is Nothing Then d = p.TryGetPropertyValue("Mass Phase Fraction", "kg/kg")
                            If d IsNot Nothing Then
                                If d.Length <> 1 Then
                                    message = "Invalid length of " + p.name + " Phase Fraction in xflow file: expected scalar"
                                    Return False
                                End If
                                If (d(0) = 0) Or (d(0) = 1) Then
                                    'vapor phase boundary
                                    runFlashType = ProductFlashOptions.FlashType.PVF
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If
            If runFlashType = ProductFlashOptions.FlashType.Automatic Then
                'check PH or TP flash
                Dim nComp As Integer = 0
                If compositionMole IsNot Nothing Then
                    For i = 0 To compositionMole.Length - 1
                        If compositionMole(i) > 0.0000000001 Then nComp += 1
                    Next
                Else
                    Debug.Assert(compositionMass IsNot Nothing)
                    For i = 0 To compositionMass.Length - 1
                        If compositionMass(i) > 0.0000000001 Then nComp += 1
                    Next
                End If
                If (nComp < loadedFlow.phases.Count) Then
                    runFlashType = ProductFlashOptions.FlashType.PH
                Else
                    runFlashType = ProductFlashOptions.FlashType.TP
                End If
            End If
            Select Case runFlashType
                Case ProductFlashOptions.FlashType.PH
                    If purged Then
                        message = "Flash type is determined as PH. One or more compound are purged; PH flash type is not allowed. Please explicitly select TP flash type"
                        Return False
                    End If
                    'calculate the reference enthalpy
                    Dim dup As Stream = Nothing
                    Try
                        hMole = Double.NaN
                        hMass = Double.NaN
                        dup = prodStream.Duplicate
                        If loadedFlow.referencePhase Is Nothing Then
                            message = "No enthalpy reference phase in loaded xflow file. Please select a different flash type"
                            Return False
                        End If
                        If loadedFlow.referencePhaseAggState Is Nothing Then
                            message = "No enthalpy reference phase aggregation state in loaded xflow file. Please select a different flash type"
                            Return False
                        End If
                        If Double.IsNaN(loadedFlow.referenceP) Then
                            message = "No enthalpy reference pressure in loaded xflow file. Please select a different flash type"
                            Return False
                        End If
                        If Double.IsNaN(loadedFlow.referenceT) Then
                            message = "No enthalpy reference temperature in loaded xflow file. Please select a different flash type"
                            Return False
                        End If
                        If Double.IsNaN(loadedFlow.molarHcorrected) And Double.IsNaN(loadedFlow.massHcorrected) Then
                            message = "No enthalpy state in loaded xflow file. Please select a different flash type"
                            Return False
                        End If
                        Dim phaseName As String = dup.TryGetReferencePhase(loadedFlow.referencePhase, loadedFlow.referencePhaseAggState)
                        If phaseName Is Nothing Then
                            message = "Unable to determine enthalpy reference state; saved state was " + loadedFlow.referencePhase + " (aggregation state " + loadedFlow.referencePhaseAggState + "). Please select a different flash type"
                            Return False
                        End If
                        Try
                            dup.SetPhaseTemperature(phaseName, loadedFlow.referenceT)
                        Catch ex As Exception
                            Throw New Exception("Unable to set temperature on reference phase (" + ex.Message + ")")
                        End Try
                        Try
                            dup.SetPhasePressure(phaseName, loadedFlow.referenceP)
                        Catch ex As Exception
                            Throw New Exception("Unable to set pressure on reference phase (" + ex.Message + ")")
                        End Try
                        Try
                            If compositionMole IsNot Nothing Then
                                Try
                                    dup.SetSinglePhaseProp("fraction", phaseName, "mole", compositionMole, False)
                                Catch ex As Exception
                                    If compositionMass Is Nothing Then Throw ex
                                    dup.SetSinglePhaseProp("fraction", phaseName, "mass", compositionMass, False)
                                End Try
                            Else
                                If compositionMass IsNot Nothing Then
                                    dup.SetSinglePhaseProp("fraction", phaseName, "mass", compositionMass, False)
                                End If
                            End If
                        Catch ex As Exception
                            Throw New Exception("Unable to set composition on reference phase (" + ex.Message + ")")
                        End Try
                        Dim props(0) As String
                        props(0) = "Enthalpy"
                        Try
                            dup.CalcSinglePhaseProp(props, phaseName)
                        Catch ex As Exception
                            Throw New Exception("Unable to calculate enthalpy on reference phase (" + ex.Message + ")")
                        End Try
                        If Double.IsNaN(loadedFlow.molarHcorrected) Then
                            Try
                                d = dup.GetSinglePhaseProp("Enthalpy", phaseName, "mole")
                                hMole = loadedFlow.molarHcorrected + d(0)
                            Catch
                            End Try
                        End If
                        If Double.IsNaN(loadedFlow.massHcorrected) Then
                            Try
                                d = dup.GetSinglePhaseProp("Enthalpy", phaseName, "mass")
                                hMole = loadedFlow.massHcorrected + d(0)
                            Catch
                            End Try
                        End If
                        If Double.IsNaN(hMole) And Double.IsNaN(hMass) Then
                            Debug.Assert(False) 'why should this happen? Investigate...
                            Throw New Exception("Unable to obtain enthalpy in proper basis from reference phase")
                        End If
                    Catch ex As Exception
                        If dup IsNot Nothing Then dup.Release()
                        Throw New Exception(ex.Message + ". Please select a different flash type")
                    End Try
                Case ProductFlashOptions.FlashType.PVF
                    If purged Then
                        message = "Flash type is determined as PH. One or more compound are purged; PH flash type is not allowed. Please explicitly select TP flash type"
                        Return False
                    End If
                    Dim pVap As Phase = Nothing
                    vaporFractionMole = Double.NaN
                    vaporFractionMass = Double.NaN
                    d = loadedFlow.TryGetPropertyValue("Molar Vapor Fraction", "mol/mol")
                    If d IsNot Nothing Then
                        If d.Length <> 1 Then
                            message = "Unexpected length for Vapor Fraction in loaded xflow file: expected scalar"
                            Return False
                        End If
                        vaporFractionMole = d(0)
                    End If
                    d = loadedFlow.TryGetPropertyValue("Mass Vapor Fraction", "kg/kg")
                    If d IsNot Nothing Then
                        If d.Length <> 1 Then
                            message = "Unexpected length for Vapor Fraction in loaded xflow file: expected scalar"
                            Return False
                        End If
                        vaporFractionMass = d(0)
                    End If
                    If (Double.IsNaN(vaporFractionMole) Or Double.IsNaN(vaporFractionMass)) Then
                        For Each p As Phase In loadedFlow.phases
                            If SameString(p.stateOfAggregation, "Vapor") Then
                                d = p.TryGetPropertyValue("Phase Fraction", "mol/mol")
                                If d IsNot Nothing Then
                                    If d.Length <> 1 Then
                                        message = "Unexpected length for " + p.name + " Phase Fraction in loaded xflow file: expected scalar"
                                        Return False
                                    End If
                                    vaporFractionMole = d(0)
                                End If
                                d = loadedFlow.TryGetPropertyValue("Phase Fraction", "kg/kg")
                                If d IsNot Nothing Then
                                    If d.Length <> 1 Then
                                        message = "Unexpected length for " + p.name + " Phase Fraction in loaded xflow file: expected scalar"
                                        Return False
                                    End If
                                    vaporFractionMass = d(0)
                                End If
                                Exit For
                            End If
                        Next
                    End If
                    If Double.IsNaN(vaporFractionMole) Then
                        If (vaporFractionMass = 0 Or vaporFractionMass = 1) Then vaporFractionMole = vaporFractionMole
                    End If
                    If Double.IsNaN(vaporFractionMole) Then
                        If Double.IsNaN(vaporFractionMass) Then
                            message = "Unable to determine vapor phase fraction from loaded xflow file. Please specify different flash type"
                            Return False
                        Else
                            message = "Unable to determine molar vapor phase fraction from loaded xflow file (mass vapor fraction flashes are currently not supported). Please specify different flash type"
                            Return False
                        End If
                    End If
                Case ProductFlashOptions.FlashType.TP
                    d = loadedFlow.TryGetPropertyValue("Temperature", "K")
                    If d Is Nothing Then
                        message = "Loaded XFlow file does not contain overall temperature."
                        Return False
                    End If
                    If d.Length <> 1 Then
                        message = "Invalid length of Temperature in xflow file: expected scalar"
                        Return False
                    End If
                    T = d(0)
                Case Else
                    Debug.Assert(False)
                    message = "Unable to determine how to flash the product stream"
                    Return False
            End Select
        Catch ex As Exception
            message = ex.Message
            Return False
        End Try
        Return True
    End Function

    Overrides Function EditUnit() As Boolean
        'take care of auto compound mapping
        Dim message As String = Nothing
        ValidateUnit(message)
        'edit
        Dim dlg As New XFlowLoaderDialog(Me)
        dlg.ShowDialog()
        If dlg.ConfigurationChanged Then Invalidate()
        Return dlg.ConfigurationChanged
    End Function

    Friend Overrides Function Save(Optional configOnly As Boolean = False) As StorageData
        Dim s As StorageData
        s = MyBase.Save
        If Not configOnly Then
            If loadedFlow IsNot Nothing Then
                s("hasFlow") = New StorageDataItem(True)
                loadedFlow.Save(s, "flow")
            End If
            If productFlow IsNot Nothing Then
                s("hasProductFlow") = New StorageDataItem(True)
                productFlow.Save(s, "productFlow")
            End If
        End If
        s("flashType") = New StorageDataItem(ProductFlashOptions.FlashTypeTag(flashType))
        s("mapping.count") = New StorageDataItem(compoundMapping.Count)
        Dim index As Integer = 1
        For Each e As KeyValuePair(Of String, String) In compoundMapping
            s("mapping." + index.ToString + ".from") = New StorageDataItem(e.Key)
            s("mapping." + index.ToString + ".to") = New StorageDataItem(e.Value)
            index += 1
        Next
        Return s
    End Function

    Friend Overrides Sub Load(s As StorageData, Optional configOnly As Boolean = False)
        _parameterCollection.ResetValues()
        comps.Clear()
        loadedFlow = Nothing
        MyBase.Load(s)
        Dim si As StorageDataItem = Nothing
        If Not configOnly Then
            If s.TryGetValue("hasFlow", si) Then
                If (si.type = StorageDataType.BooleanType) Then
                    If (CBool(si.data)) Then
                        loadedFlow = New Flow
                        loadedFlow.Load(s, "flow")
                    End If
                End If
            End If
            If s.TryGetValue("hasProductFlow", si) Then
                If (si.type = StorageDataType.BooleanType) Then
                    If (CBool(si.data)) Then
                        productFlow = New Flow
                        productFlow.Load(s, "productFlow")
                    End If
                End If
            End If
        End If
        flashType = ProductFlashOptions.FlashType.Automatic
        If s.TryGetValue("flashType", si) Then
            If si.type = StorageDataType.StringType Then
                flashType = ProductFlashOptions.GetFlashType(CStr(si.data))
            End If
        End If
        compoundMapping.Clear()
        If s.TryGetValue("mapping.count", si) Then
            If si.type = StorageDataType.IntegerType Then
                Dim i As Integer, count As Integer
                count = CInt(si.data)
                For i = 1 To count
                    Dim _from As String
                    If Not s.TryGetValue("mapping." + i.ToString + ".from", si) Then Continue For
                    If si.type <> StorageDataType.StringType Then Continue For
                    _from = CStr(si.data)
                    If Not s.TryGetValue("mapping." + i.ToString + ".to", si) Then Continue For
                    If si.type <> StorageDataType.StringType Then Continue For
                    compoundMapping(_from) = CStr(si.data)
                Next
            End If
        End If
    End Sub

    Friend Function AutoMap(comp As Compound) As String
        ' step 1, match by ID
        Dim mappedTo As String = Nothing
        For Each c As Compound In comps
            If SameString(comp.compID, c.compID) Then
                mappedTo = c.compID
                Exit For
            End If
        Next
        If mappedTo Is Nothing Then
            ' step 2, match by CAS number
            For Each c As Compound In comps
                If SameString(comp.CAS, c.CAS) Then
                    mappedTo = c.compID
                    Exit For
                End If
            Next
        End If
        If mappedTo Is Nothing Then
            ' step 3, match by name
            If comp.name IsNot Nothing Then
                For Each c As Compound In comps
                    If c.name Is Nothing Then Continue For
                    If SameString(comp.CAS, c.CAS) Then
                        mappedTo = c.compID
                        Exit For
                    End If
                Next
            End If
        End If
        Return mappedTo
    End Function
End Class
