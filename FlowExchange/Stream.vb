'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN
Imports System.Text

'wrapper class for a material object representing a stream

Friend MustInherit Class Stream
    Sub New(unit As UnitOperationBase)
        Me.unit = unit
    End Sub
    MustOverride Function GetComps(compoundProps As List(Of PropertyInstance)) As List(Of Compound)
    MustOverride Sub SetOverallProp(propName As String, basis As String, data() As Double, Optional isMixture As Boolean = True)
    MustOverride Sub SetSinglePhaseProp(propName As String, phase As String, basis As String, data() As Double, Optional isMixture As Boolean = True)
    MustOverride Function GetOverallProp(propName As String, basis As String, Optional isMixture As Boolean = True) As Double()
    MustOverride Function GetSinglePhaseProp(propName As String, phase As String, basis As String, Optional isMixture As Boolean = True) As Double()
    MustOverride Function PropNameFromPropertyInstance(pi As PropertyInstance) As String
    MustOverride Function PresentPhases() As String()
    MustOverride Function GetStateOfAggregation(phase As String) As String
    MustOverride Function GetKeyCompound(phase As String) As String
    MustOverride Function Duplicate() As Stream
    MustOverride Function GetVaporPhaseName() As String
    MustOverride Sub TPFlash(Optional haveGuess As Boolean = False)
    MustOverride Sub PHFlash(Optional haveGuess As Boolean = False)
    MustOverride Sub PVFFlash(Optional haveGuess As Boolean = False)
    MustOverride Sub CalcSinglePhaseProp(props() As String, phase As String)
    MustOverride Sub Release() 'Note: should be called on, and only on, Duplicate materials, not on ones obtained from ports
    MustOverride Sub SetPhaseTemperature(phase As String, T As Double)
    MustOverride Sub SetPhasePressure(phase As String, P As Double)
    MustOverride Function CheckSupported(pi As PropertyInfo) As Boolean
    MustOverride Function PhaseSelectionOptions() As String()
    MustOverride Function TryGetReferencePhase(referencePhase As String, referencePhaseAggState As String) As String
    Protected unit As UnitOperationBase

    Friend Sub ProductFromFeed(f As Flow, runReport As StringBuilder)
        Dim value() As Double
        Dim i As Integer
        Dim ok As Boolean = True
        Dim nCompoundPresent As Integer = 0
        Dim basis As String = "mole"
        Dim errorIfNoCompositionAndFlow As String = Nothing
        value = f.TryGetPropertyValue("Molar Compound Flow", "mol/mol")
        If value Is Nothing Then
            basis = "mass"
            value = f.TryGetPropertyValue("Mass Compound Flow", "kg/kg")
        End If
        If value IsNot Nothing Then
            Try
                SetOverallProp("flow", basis, value, False)
                For i = 0 To value.Length - 1
                    If (value(i) > 0) Then
                        nCompoundPresent += 1
                    End If
                Next
            Catch
                'separately set composition and flow rate
                Dim x() As Double
                Try
                    ReDim x(value.Length - 1)
                    Dim sum As Double
                    For i = 0 To value.Length - 1
                        sum += value(i)
                    Next
                    If (sum <= 0) Then
                        errorIfNoCompositionAndFlow = "Unable to set flow and composition based on compound flow rate: total flow is zero or negative (cannot determine composition)"
                        GoTo setFlowComposition
                    End If
                    Dim norm As Double = 1.0 / sum
                    For i = 0 To value.Length - 1
                        If (value(i) > 0.0000000001) Then
                            nCompoundPresent += 1
                            x(i) = norm * value(i)
                        Else
                            x(i) = 0
                        End If
                    Next
                    Try
                        SetOverallProp("fraction", basis, x, False)
                    Catch ex As Exception
                        Throw New Exception("Failed to set product composition: " + ex.Message())
                    End Try
                    ReDim value(0)
                    value(0) = sum
                    Try
                        SetOverallProp("totalFlow", basis, value, False)
                    Catch ex As Exception
                        Throw New Exception("Failed to set product flow rate: " + ex.Message)
                    End Try
                Catch ex As Exception
                    errorIfNoCompositionAndFlow = "Invalid values for compound flow rate: " + ex.Message
                    GoTo setFlowComposition
                End Try
            End Try
        Else
setFlowComposition:
            basis = "mole"
            value = f.TryGetPropertyValue("Mole Fraction", "mol/mol")
            If value Is Nothing Then
                basis = "mass"
                value = f.TryGetPropertyValue("Mass Fraction", "kg/kg")
            End If
            If value Is Nothing Then
                If errorIfNoCompositionAndFlow IsNot Nothing Then Throw New Exception(errorIfNoCompositionAndFlow)
                Throw New Exception("Unknown composition")
            End If
            Try
                SetOverallProp("fraction", basis, value, False)
            Catch ex As Exception
                Throw New Exception("Failed to set product composition: " + ex.Message())
            End Try
            For i = 0 To value.Length - 1
                If (value(i) > 0.0000000001) Then
                    nCompoundPresent += 1
                End If
            Next i
            basis = "mole"
            value = f.TryGetPropertyValue("Molar Flow", "mol/s")
            If (value Is Nothing) Then
                basis = "mass"
                value = f.TryGetPropertyValue("Mass Flow", "kg/s")
            End If
            If value Is Nothing Then
                If errorIfNoCompositionAndFlow IsNot Nothing Then Throw New Exception(errorIfNoCompositionAndFlow)
                Throw New Exception("Unknown flow rate")
            End If
            Try
                SetOverallProp("totalFlow", basis, value, False)
            Catch ex As Exception
                Throw New Exception("Failed to set product flow rate: " + ex.Message)
            End Try
        End If
        value = f.TryGetPropertyValue("Pressure", "Pa")
        If value Is Nothing Then Throw New Exception("Unknown pressure")
        Try
            SetOverallProp("pressure", vbNullString, value, False)
        Catch ex As Exception
            Throw New Exception("Failed to set product pressure: " + ex.Message)
        End Try
        Select Case f.autoFlash
            Case "PVF"
                basis = "mole"
                value = f.TryGetPropertyValue("Molar Vapor Fraction", "mol/mol")
                If (value Is Nothing) Then
                    basis = "mass"
                    value = f.TryGetPropertyValue("Mass Vapor Fraction", "kg/kg")
                End If
                If value Is Nothing Then
                    For Each p As Phase In f.phases
                        If CAPEOPENBase.SameString(p.stateOfAggregation, "Vapor") Then
                            basis = "mole"
                            value = f.TryGetPropertyValue("Molar Phase Fraction", "mol/mol")
                            If (value Is Nothing) Then
                                basis = "mass"
                                value = f.TryGetPropertyValue("Mass Phase Fraction", "kg/kg")
                            End If
                            Exit For
                        End If
                    Next
                End If
                If value Is Nothing Then Throw New Exception("Unknown vapor phase fraction")
                If value(0) = 0 Or value(0) = 1 Then basis = "mole"
                If basis = "mass" Then
                    'for now we do this, we can however support mass based vapor fraction flashes in 1.1
                    Throw New Exception("Mass based vapor fraction flashes are not supported")
                End If
                Try
                    SetSinglePhaseProp("phaseFraction", GetVaporPhaseName, basis, value, False)
                Catch ex As Exception
                    Throw New Exception("Failed to set product vapor fraction: " + ex.Message)
                End Try
                Try
                    PVFFlash(True)
                    runReport.Append("Using P-VF flash for product stream")
                    runReport.Append(vbCrLf)
                Catch ex As Exception
                    Throw New Exception("Failed to perform P-VF flash: " + ex.Message)
                End Try
            Case "PH"
                'flash PH
                basis = "mole"
                ReDim value(0)
                If Double.IsNaN(f.molarHraw) Then
                    If Double.IsNaN(f.massHraw) Then
                        Throw New Exception("Unknown enthalpy")
                    Else
                        basis = "mass"
                        value(0) = f.massHraw
                    End If
                Else
                    basis = "mole"
                    value(0) = f.molarHraw
                End If
                Try
                    SetOverallProp("Enthalpy", basis, value)
                Catch ex As Exception
                    Throw New Exception("Failed to set product enthalpy: " + ex.Message)
                End Try
                Try
                    PHFlash(True)
                    runReport.Append("Using PH flash for product stream")
                    runReport.Append(vbCrLf)
                Catch ex As Exception
                    Throw New Exception("Failed to perform PH flash: " + ex.Message)
                End Try
            Case Else
                'flash TP
                value = f.TryGetPropertyValue("Temperature", "K")
                If value Is Nothing Then Throw New Exception("Unknown temperature")
                Try
                    SetOverallProp("temperature", vbNullString, value, False)
                Catch ex As Exception
                    Throw New Exception("Failed to set product temperature: " + ex.Message)
                End Try
                Try
                    TPFlash(True)
                    runReport.Append("Using TP flash for product stream")
                    runReport.Append(vbCrLf)
                Catch ex As Exception
                    Throw New Exception("Failed to perform TP flash: " + ex.Message)
                End Try
        End Select
    End Sub

    Friend Function GetFlow(comps As List(Of Compound), overallProps As List(Of PropertyInstance), phaseProps As List(Of PropertyInstance), options As XFlowFileOptions) As Flow
        Dim f As New Flow
        Dim nPhase As Integer
        Dim vaporFractionOne As Boolean = False
        f.compounds = comps
        Dim i As Integer
        Dim pi As PropertyInstance
        Dim taskList As New Dictionary(Of String, PhaseTask)
        Dim data As Object
        Dim phaseImmediatePropList As New List(Of PropertyInstance)
        Dim setMolarVaporFraction As Boolean = False
        Dim setMassVaporFraction As Boolean = False
        Dim molarFlowRate As Double = -1
        Dim massFlowRate As Double = -1
        'compile task list (and get overall immediate properties on the fly)
        Dim task As New PhaseTask
        If options.writeEnthalpy Then
            task.massSum = 0
            task.molarSum = 0
            taskList("enthalpy") = task
        End If
        For Each pi In overallProps
            Select Case pi.exposedName
                Case "Molar Flow", "Mass Flow"
                    'readily available
                    Try
                        data = GetOverallProp(PropNameFromPropertyInstance(pi), pi.basis, False)
                        f.SetPropertyValue(pi.exposedName, data, pi.unit)
                        'store the flow rate for phase summations
                        If (pi.basis = "mass") Then massFlowRate = data(0) Else molarFlowRate = data(0)
                    Catch ex As Exception
                        unit.LogMessage("Failed to get " + pi.exposedName + ": " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                        If (pi.basis = "mass") Then massFlowRate = Double.NaN Else molarFlowRate = Double.NaN
                    End Try
                Case "Temperature", "Pressure", "Mole Fraction", "Mass Fraction", "Molar Compound Flow", "Mass Compound Flow"
                    'readily available
                    Try
                        data = GetOverallProp(PropNameFromPropertyInstance(pi), pi.basis, False)
                        f.SetPropertyValue(pi.exposedName, data, pi.unit)
                    Catch ex As Exception
                        unit.LogMessage("Failed to get " + pi.exposedName + ": " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                    End Try
                Case "Molar Vapor Fraction", "Mass Vapor Fraction"
                    If (pi.basis = "mass") Then setMassVaporFraction = True Else setMolarVaporFraction = True
                Case Else
                    task = Nothing
                    If Not taskList.TryGetValue(PropNameFromPropertyInstance(pi), task) Then
                        task = New PhaseTask
                        taskList(PropNameFromPropertyInstance(pi)) = task
                    End If
                    task.overall.Add(pi)
                    task.massSum = 0
                    task.molarSum = 0
            End Select
        Next
        For Each pi In phaseProps
            Select Case pi.exposedName
                Case "Temperature", "Pressure", "Molar Vapor Fraction", "Mass Vapor Fraction"
                    Debug.Assert(False)
                Case "Molar Flow", "Mass Flow", "Mole Fraction", "Mass Fraction", "Molar Compound Flow", "Mass Compound Flow", "Molar Phase Fraction", "Mass Phase Fraction"
                    phaseImmediatePropList.Add(pi)
                Case Else
                    task = Nothing
                    If Not taskList.TryGetValue(PropNameFromPropertyInstance(pi), task) Then
                        task = New PhaseTask
                        taskList(PropNameFromPropertyInstance(pi)) = task
                    End If
                    task.phase.Add(pi)
            End Select
        Next
        Dim presentPhaseIDs() As String = Nothing
        Try
            presentPhaseIDs = PresentPhases()
        Catch ex As Exception
            unit.LogMessage("Failed to present phases: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
        End Try
        If (presentPhaseIDs Is Nothing) Then
            nPhase = 0
        Else
            Try
                nPhase = presentPhaseIDs.Length
            Catch
                unit.LogMessage("Failed to present phases: returned value is not an array", UnitOperationBase.MessageType.WarningMessage)
                nPhase = 0
            End Try
            'we have phases, if we do not find vapor the vapor fraction is zero
            For Each phase As String In presentPhaseIDs
                Dim aggState As String
                Try
                    aggState = GetStateOfAggregation(phase)
                Catch ex As Exception
                    unit.LogMessage("Failed to get state of aggregation for """ + phase + """: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                    aggState = Nothing
                End Try
                If setMolarVaporFraction Or setMassVaporFraction Then
                    If CAPEOPENBase.SameString(aggState, "vapor") Then
                        'we have a vapor phase, get vapor fraction
                        If setMolarVaporFraction Then
                            Try
                                data = GetSinglePhaseProp("phaseFraction", phase, "mole", False)
                                f.SetPropertyValue("Molar Vapor Fraction", data(0), "mol/mol")
                                If (data(0) = 0) Then f.autoFlash = "PVF" Else If (data(0) = 1) Then vaporFractionOne = True
                            Catch ex As Exception
                                unit.LogMessage("Failed to get molar vapor fraction: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                            End Try
                        End If
                        If setMassVaporFraction Then
                            Try
                                data = GetSinglePhaseProp("phaseFraction", phase, "mass", False)
                                f.SetPropertyValue("Mass Vapor Fraction", data(0), "kg/kg")
                                If (data(0) = 0) Then f.autoFlash = "PVF" Else If (data(0) = 1) Then vaporFractionOne = True
                            Catch ex As Exception
                                unit.LogMessage("Failed to get mass vapor fraction: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                            End Try
                        End If
                    End If
                End If
                'create a phase object
                If phaseProps.Count > 0 Or options.writePhaseInfo Then
                    Dim p As New Phase
                    p.name = phase
                    p.stateOfAggregation = aggState
                    Try
                        p.keyCompound = GetKeyCompound(phase)
                        If String.IsNullOrEmpty(p.keyCompound) Then p.keyCompound = Nothing
                    Catch
                    End Try
                    For Each pi In phaseImmediatePropList
                        Try
                            data = GetSinglePhaseProp(PropNameFromPropertyInstance(pi), phase, pi.basis)
                            p.SetPropertyValue(pi.exposedName, data, pi.unit)
                            If pi.exposedName = "Molar Phase Fraction" Or pi.exposedName = "Mass Phase Fraction" Then
                                If CAPEOPENBase.SameString(p.stateOfAggregation, "Vapor") Then
                                    If (data(0) = 0) Then
                                        If (f.autoFlash Is Nothing) Then f.autoFlash = "PVF"
                                    ElseIf (data(0) = 1) Then
                                        vaporFractionOne = True
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            unit.LogMessage("Failed to get """ + pi.ToString + """ for phase """ + phase + """: " + ex.Message(), UnitOperationBase.MessageType.WarningMessage)
                        End Try
                    Next
                    f.phases.Add(p)
                End If
            Next
        End If
        'loop over a duplicate material's phases to execute the calculation tasks
        If (taskList.Count > 0) Or (options.writeEnthalpy) Then
            Dim dup As Stream = Nothing
            If f.phases.Count Then
                Try
                    dup = Duplicate()
                Catch ex As Exception
                    unit.LogMessage("Failed to duplicate/copy feed material: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                End Try
            End If
            If dup IsNot Nothing Then
                If (taskList.Count > 0) Then
                    'compile the total list of properties to calculate for each phase
                    Dim totalPropListList As New List(Of String)
                    For Each d As KeyValuePair(Of String, PhaseTask) In taskList
                        totalPropListList.Add(d.Key)
                    Next
                    Dim totalPropList() As String = totalPropListList.ToArray
                    Dim propList(0) As String 'in case we need it
                    For Each p As Phase In f.phases
                        Dim phase As String = p.name
                        For Each d As KeyValuePair(Of String, PhaseTask) In taskList
                            task = d.Value
                            task.propOK = True
                            For Each pi In task.overall
                                Select Case pi.basis
                                    Case "mass"
                                        task.massSum = 0
                                    Case "mole"
                                        task.molarSum = 0
                                    Case "flow"
                                        'flow basis, try to get both
                                        task.massSum = 0
                                        task.molarSum = 0
                                    Case Else
                                        Debug.Assert(False)
                                End Select
                            Next
                        Next
                        'try calculating all properties together
                        Try
                            dup.CalcSinglePhaseProp(totalPropList, phase)
                        Catch ex As Exception
                            Dim sb As New StringBuilder
                            sb.Append("Failed calculation of {")
                            For i = 0 To totalPropList.Length - 1
                                If i > 0 Then sb.Append(";"c)
                                sb.Append(totalPropList(i))
                            Next
                            sb.Append("} for phase """)
                            sb.Append(phase)
                            sb.Append(""": ")
                            sb.Append(ex.Message)
                            unit.LogMessage(sb.ToString, UnitOperationBase.MessageType.WarningMessage)
                            If (totalPropList.Length = 1) Then
                                'no point in individual calculations
                                task = taskList(totalPropList(0))
                                If task.phase.Count = 0 Then
                                    taskList.Remove(totalPropList(0))
                                    Exit For 'no need to continue looping phases
                                End If
                                task.massSum = Double.NaN
                                task.molarSum = Double.NaN
                                task.overall.Clear()
                            End If
                            'try individual property calculations
                            i = 0
                            While i < totalPropListList.Count
                                propList(0) = totalPropListList(i)
                                Try
                                    dup.CalcSinglePhaseProp(propList, phase)
                                    i += 1
                                Catch ex1 As Exception
                                    unit.LogMessage("Failed to calculate " + totalPropListList(i) + " for phase """ + phase + """: " + ex1.Message, UnitOperationBase.MessageType.WarningMessage)
                                    'failed, adjust the task
                                    If task.phase.Count = 0 Then
                                        'no need to keep this task
                                        totalPropListList.RemoveAt(i)
                                        totalPropList = totalPropListList.ToArray
                                        taskList.Remove(propList(0))
                                        Continue While
                                    End If
                                    task = taskList(propList(0))
                                    task.massSum = Double.NaN
                                    task.molarSum = Double.NaN
                                    task.overall.Clear()
                                    task.propOK = False
                                    i += 1
                                End Try
                            End While
                        End Try
                        'we have some properties for this phase, get phase fractions as we need them
                        Dim molarPhaseFraction As Double = -1
                        Dim massPhaseFraction As Double = -1
                        i = 0
                        While i < totalPropListList.Count
                            Dim prop As String = totalPropListList(i)
                            task = taskList(prop)
                            If Not task.propOK Then Continue For 'property not calculated, filtering already done
                            Dim test As Boolean = False
                            If Not Double.IsNaN(task.massSum) Then
                                If Not Double.IsNaN(massPhaseFraction) Then
                                    If massPhaseFraction < 0 Then
                                        Try
                                            data = dup.GetSinglePhaseProp("phaseFraction", phase, "mass", False)
                                            massPhaseFraction = data(0)
                                            test = True
                                        Catch ex As Exception
                                            unit.LogMessage("Failed to obtain mass phase fraction for phase """ + phase + """: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                        End Try
                                    Else
                                        test = True
                                    End If
                                End If
                            End If
                            If Not test Then
                                For Each pi In task.phase
                                    If pi.basis = "mass" Then
                                        test = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If test Then
                                'obtain mass property
                                Try
                                    data = dup.GetSinglePhaseProp(prop, phase, "mass")
                                    If Not Double.IsNaN(task.massSum) Then
                                        If Not Double.IsNaN(massPhaseFraction) Then
                                            task.massSum += massPhaseFraction * data(0)
                                        End If
                                    End If
                                    For Each pi In task.phase
                                        If pi.basis = "mass" Then
                                            p.SetPropertyValue(pi.exposedName, data(0), pi.unit)
                                            Exit For
                                        End If
                                    Next
                                Catch ex As Exception
                                    unit.LogMessage("Failed to obtain mass " + prop + " for phase """ + phase + """: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                    task.massSum = Double.NaN
                                    If Double.IsNaN(task.molarSum) And task.phase.Count = 0 Then
                                        'remove and continue
                                        totalPropListList.RemoveAt(i)
                                        totalPropList = totalPropListList.ToArray
                                        taskList.Remove(propList(0))
                                        Continue While
                                    End If
                                End Try
                            End If
                            test = False
                            If Not Double.IsNaN(task.molarSum) Then
                                If Not Double.IsNaN(molarPhaseFraction) Then
                                    If molarPhaseFraction < 0 Then
                                        Try
                                            data = dup.GetSinglePhaseProp("phaseFraction", phase, "mole", False)
                                            molarPhaseFraction = data(0)
                                            test = True
                                        Catch ex As Exception
                                            unit.LogMessage("Failed to obtain molar phase fraction for phase """ + phase + """: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                        End Try
                                    Else
                                        test = True
                                    End If
                                End If
                            End If
                            If Not test Then
                                For Each pi In task.phase
                                    If pi.basis = "mole" Then
                                        test = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If test Then
                                'obtain mass property
                                Try
                                    data = dup.GetSinglePhaseProp(prop, phase, "mole")
                                    If Not Double.IsNaN(task.molarSum) Then
                                        If Not Double.IsNaN(molarPhaseFraction) Then
                                            task.molarSum += molarPhaseFraction * data(0)
                                        End If
                                    End If
                                    For Each pi In task.phase
                                        If pi.basis = "mole" Then
                                            p.SetPropertyValue(pi.exposedName, data(0), pi.unit)
                                            Exit For
                                        End If
                                    Next
                                Catch ex As Exception
                                    unit.LogMessage("Failed to obtain molar " + prop + " for phase """ + phase + """: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                    task.molarSum = Double.NaN
                                    If Double.IsNaN(task.massSum) And task.phase.Count = 0 Then
                                        'remove and continue
                                        totalPropListList.RemoveAt(i)
                                        totalPropList = totalPropListList.ToArray
                                        taskList.Remove(propList(0))
                                        Continue While
                                    End If
                                End Try
                            End If
                            test = False
                            For Each pi In task.phase
                                If String.IsNullOrEmpty(pi.basis) Then
                                    test = True
                                    Exit For
                                End If
                            Next
                            If test Then
                                'obtain property
                                Try
                                    data = dup.GetSinglePhaseProp(prop, phase, vbNullString)
                                    For Each pi In task.phase
                                        If String.IsNullOrEmpty(pi.basis) Then
                                            p.SetPropertyValue(pi.exposedName, data(0), pi.unit)
                                            Exit For
                                        End If
                                    Next
                                Catch ex As Exception
                                    unit.LogMessage("Failed to obtain " + prop + " for phase """ + phase + """: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                End Try
                            End If
                            i += 1
                        End While
                    Next
                    'loop over tasks to set overall properties
                    For i = 0 To totalPropList.Length - 1
                        Dim prop As String = totalPropList(i)
                        task = taskList(prop)
                        For Each pi In task.overall
                            Select Case pi.basis
                                Case "mass"
                                    If Not Double.IsNaN(task.massSum) Then f.SetPropertyValue(pi.exposedName, task.massSum, pi.unit)
                                Case "mole"
                                    If Not Double.IsNaN(task.molarSum) Then f.SetPropertyValue(pi.exposedName, task.molarSum, pi.unit)
                                Case "flow"
                                    If Not Double.IsNaN(molarFlowRate) And molarFlowRate >= 0 And Not Double.IsNaN(task.molarSum) Then
                                        'we have all molar data
                                        f.SetPropertyValue(pi.exposedName, task.molarSum * molarFlowRate, pi.unit)
                                    ElseIf Not Double.IsNaN(massFlowRate) And massFlowRate >= 0 And Not Double.IsNaN(task.massSum) Then
                                        'we have all mass data
                                        f.SetPropertyValue(pi.exposedName, task.massSum * massFlowRate, pi.unit)
                                    Else
                                        Dim done As Boolean = False
                                        If Not Double.IsNaN(task.molarSum) And Not Double.IsNaN(molarFlowRate) Then
                                            'try to get molar flow rate
                                            Try
                                                data = GetOverallProp("totalFlow", "mole", False)
                                                f.SetPropertyValue(pi.exposedName, task.molarSum * molarFlowRate, pi.unit)
                                                done = True
                                            Catch ex As Exception
                                                unit.LogMessage("Failed to get Molar Flow Rate: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                                molarFlowRate = Double.NaN
                                            End Try
                                        End If
                                        If Not done Then
                                            If Not Double.IsNaN(task.massSum) And Not Double.IsNaN(massFlowRate) Then
                                                Try
                                                    data = GetOverallProp("totalFlow", "mass", False)
                                                    f.SetPropertyValue(pi.exposedName, task.massSum * massFlowRate, pi.unit)
                                                Catch ex As Exception
                                                    unit.LogMessage("Failed to get Mass Flow Rate: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                                                    massFlowRate = Double.NaN
                                                End Try
                                            End If
                                        End If
                                    End If
                                Case Else
                                    Debug.Assert(False)
                            End Select
                        Next
                    Next i
                End If
                'store reference for enthalpy
                If options.writeEnthalpy Then
                    If (taskList.TryGetValue("enthalpy", task)) Then
                        If Not Double.IsNaN(task.massSum) Then f.massHraw = task.massSum
                        If Not Double.IsNaN(task.molarSum) Then f.molarHraw = task.molarSum
                    End If
                    If Not Double.IsNaN(f.massHraw) Or Not Double.IsNaN(f.molarHraw) Then
                        Try
                            Dim dblData(0) As Double
                            If (options.enthalpyReferencePhase Is Nothing) Then
                                'determine the state best suitable as reference state. First see if we can set it to vapor as it is the best choice of state
                                ' however we do not want to extrapolate a vapor phase, so we only use it in case it is present
                                'first attempt a vapor phase, at 1 bar, but we need a temperature at which it actually exists
                                f.referencePhase = GetVaporPhaseName()
                                If f.referencePhase Is Nothing Then Throw New Exception
                                f.referencePhaseAggState = "Vapor"
                                f.referenceP = 101325
                                dblData(0) = f.referenceP
                                dup.SetOverallProp("pressure", vbNullString, dblData, False)
                                'determine the bubble point temperature
                                dblData(0) = 1
                                dup.SetSinglePhaseProp("phaseFraction", f.referencePhase, "mole", dblData, False)
                                'calculate PVF flash
                                dup.PVFFlash()
                                'get the temperature
                                data = dup.GetOverallProp("Temperature", vbNullString, False)
                                f.referenceT = data(0)
                                If Double.IsNaN(f.referenceT) Then Throw New Exception
                            Else
                                'use the provided reference state
                                f.referencePhase = options.enthalpyReferencePhase
                                f.referencePhaseAggState = GetStateOfAggregation(f.referencePhase)
                                f.referenceP = options.enthalpyReferenceP
                                f.referenceT = options.enthalpyReferenceT
                                dblData(0) = f.referenceP
                                dup.SetPhasePressure(f.referencePhase, f.referenceP)
                                dblData(0) = f.referenceT
                                dup.SetPhaseTemperature(f.referencePhase, f.referenceT)
                            End If
                            'get the reference enthalpy
                            Dim props(0) As String
                            props(0) = "Enthalpy"
                            dup.CalcSinglePhaseProp(props, f.referencePhase)
                            Dim haveReference As Boolean = False
                            If Not Double.IsNaN(f.massHraw) Then
                                'store corrected mass enthalpy
                                Try
                                    data = dup.GetSinglePhaseProp("enthalpy", f.referencePhase, "mass")
                                    f.massHcorrected = f.massHraw - data(0)
                                    haveReference = True
                                Catch
                                    f.massHcorrected = Double.NaN
                                End Try
                            End If
                            If Not Double.IsNaN(f.molarHraw) Then
                                'store corrected molar enthalpy
                                Try
                                    data = dup.GetSinglePhaseProp("enthalpy", f.referencePhase, "mole")
                                    f.molarHcorrected = f.molarHraw - data(0)
                                    haveReference = True
                                Catch
                                    f.molarHcorrected = Double.NaN
                                End Try
                            End If
                            If Not haveReference Then Throw New Exception
                        Catch ex As Exception
                            f.referenceP = Double.NaN
                            f.referenceT = Double.NaN
                            f.referencePhase = Nothing
                            f.referencePhaseAggState = Nothing
                            unit.LogMessage("Unable to determine reference state corrected enthalpy: " + ex.Message, UnitOperationBase.MessageType.WarningMessage)
                        End Try
                        'no? well - this is the only thing we try at the moment, until we have a better scheme
                    End If
                End If
                dup.Release()
            End If
        End If
        If f.autoFlash Is Nothing Then
            If vaporFractionOne And nPhase > 1 Then
                f.autoFlash = "PVF"
            ElseIf nPhase = 0 Or (nPhase > comps.Count) Then
                f.autoFlash = "PH"
            Else
                f.autoFlash = "TP"
            End If
        End If
        Return f
    End Function

End Class


