'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN110
Imports System.Text

'version 1.1 thermo

Friend Class Stream11
    Inherits Stream
    Sub New(unit As UnitOperationBase, mat As ICapeThermoMaterial)
        MyBase.New(unit)
        Me.mat = mat
    End Sub
    Private mat As ICapeThermoMaterial
    Private iRoutine As ICapeThermoPropertyRoutine
    Private iPhases As ICapeThermoPhases
    Private iFlash As ICapeThermoEquilibriumRoutine
    Private supportedPhases() As String = Nothing
    Private vaporPhaseName As String = Nothing
    Private propList As Dictionary(Of String, Boolean) = Nothing

    Public Overrides Function GetComps(compoundProps As List(Of PropertyInstance)) As System.Collections.Generic.List(Of Compound)
        Dim comps As New List(Of Compound)
        Try
            Dim i As Integer
            Dim iComp As ICapeThermoCompounds = mat
            Dim compIds As Object = Nothing
            Dim formulae As Object = Nothing
            Dim names As Object = Nothing
            Dim NBP As Object = Nothing
            Dim MW As Object = Nothing
            Dim casnos As Object = Nothing
            Dim values() As Double = Nothing
            iComp.GetCompoundList(compIds, formulae, names, NBP, MW, casnos)
            For Each c As String In compIds
                Dim comp As New Compound
                comp.compID = c
                comps.Add(comp)
            Next
            Try
                For i = 0 To comps.Count - 1
                    Try
                        comps(i).name = names(i)
                    Catch
                    End Try
                Next
            Catch
            End Try
            Try
                For i = 0 To comps.Count - 1
                    Try
                        comps(i).CAS = casnos(i)
                    Catch
                    End Try
                Next
            Catch
            End Try
            Try
                For i = 0 To comps.Count - 1
                    Try
                        comps(i).formula = formulae(i)
                    Catch
                    End Try
                Next
            Catch
            End Try
            For Each p As PropertyInstance In compoundProps
                Try
                    Select Case p.name11
                        Case "NormalBoilingPoint"
                            values = NBP
fromArray:
                            For i = 0 To comps.Count - 1
                                Try
                                    If Not Double.IsNaN(values(i)) Then
                                        comps(i).SetPropertyValue(p.exposedName, values(i), p.unit)
                                    End If
                                Catch
                                End Try
                            Next
                        Case "MolecularWeight"
                            values = MW
                            GoTo fromArray
                        Case Else
                            'use GetCompoundConstant
                            Dim compounds(0) As String
                            Dim props(0) As String
                            props(0) = p.name11
                            For i = 0 To comps.Count - 1
                                compounds(0) = comps(i).compID
                                Try
                                    values = iComp.GetCompoundConstant(props, compounds)
                                    If Not Double.IsNaN(values(0)) Then
                                        comps(i).SetPropertyValue(p.exposedName, values(0), p.unit)
                                    End If
                                Catch
                                End Try
                            Next
                    End Select
                Catch
                End Try
            Next
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
        Return comps
    End Function

    Public Overrides Sub CalcSinglePhaseProp(props() As String, phase As String)
        If iRoutine Is Nothing Then
            Try
                iRoutine = mat
            Catch ex As Exception
                Throw New Exception("Failed to get ICapeThermoPropertyRoutine from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            End Try
        End If
        Try
            iRoutine.CalcSinglePhaseProp(props, phase)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(iRoutine, ex))
        End Try
    End Sub

    Public Overrides Function Duplicate() As Stream
        Dim mat As ICapeThermoMaterial
        Try
            mat = Me.mat.CreateMaterial
        Catch ex As Exception
            Throw New Exception("Failed to create duplicate material object: " + CAPEOPENBase.ErrorFromCOObject(Me.mat, ex))
        End Try
        Try
            mat.CopyFromMaterial(Me.mat)
        Catch ex As Exception
            Throw New Exception("Failed to copy duplicate material object from original: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            CAPEOPENBase.ReleaseIfCOM(mat)
        End Try
        Return New Stream11(unit, mat)
    End Function

    Public Overrides Function GetKeyCompound(phase As String) As String
        If iPhases Is Nothing Then
            Try
                iPhases = mat
            Catch ex As Exception
                Throw New Exception("Failed to get ICapeThermoPhases interface from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            End Try
        End If
        Dim res As String
        Try
            res = iPhases.GetPhaseInfo(phase, "KeyCompoundId")
        Catch
            res = Nothing
        End Try
        Return res
    End Function

    Public Overrides Function GetOverallProp(propName As String, basis As String, Optional isMixture As Boolean = True) As Double()
        Try
            Dim data
            data = Nothing
            mat.GetOverallProp(propName, basis, data)
            Return data
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Public Overrides Function GetSinglePhaseProp(propName As String, phase As String, basis As String, Optional isMixture As Boolean = True) As Double()
        Try
            Dim data
            data = Nothing
            mat.GetSinglePhaseProp(propName, phase, basis, data)
            Return data
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Private Function GetSupportedPhases() As String()
        If supportedPhases Is Nothing Then
            If iPhases Is Nothing Then
                Try
                    iPhases = mat
                Catch ex As Exception
                    Throw New Exception("Failed to get ICapeThermoPhases interface from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
                End Try
            End If
            Try
                Dim aggState = Nothing
                Dim keyComps = Nothing
                iPhases.GetPhaseList(supportedPhases, aggState, keyComps)
            Catch ex As Exception
                Throw New Exception("Failed to get list of supported phase from material object: " + CAPEOPENBase.ErrorFromCOObject(iPhases, ex))
            End Try
        End If
        Return supportedPhases
    End Function

    Public Overrides Function GetStateOfAggregation(phase As String) As String
        If iPhases Is Nothing Then
            Try
                iPhases = mat
            Catch ex As Exception
                Throw New Exception("Failed to get ICapeThermoPhases interface from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            End Try
        End If
        Dim res As String
        Try
            res = iPhases.GetPhaseInfo(phase, "StateOfAggregation")
        Catch
            res = Nothing
        End Try
        Return res
    End Function

    Public Overrides Function GetVaporPhaseName() As String
        If vaporPhaseName Is Nothing Then
            For Each phase As String In GetSupportedPhases()
                If (CAPEOPENBase.SameString(GetStateOfAggregation(phase), "Vapor")) Then
                    vaporPhaseName = phase
                    Exit For
                End If
            Next
        End If
        Return vaporPhaseName
    End Function

    Private Sub PFlash(flashConstraint2() As String, haveGuess As Boolean)
        Dim flashConstraint1(2) As String
        flashConstraint1(0) = "Pressure"
        flashConstraint1(2) = "Overall"
        'set all phases
        Dim supportedPhases() As String = GetSupportedPhases()
        Dim phaseStatus(supportedPhases.Length - 1) As Integer
        Dim i As Integer
        If haveGuess Then
            For i = 0 To supportedPhases.Length - 1
                phaseStatus(i) = CAPEOPEN110.eCapePhaseStatus.CAPE_ESTIMATES
            Next
        Else
            For i = 0 To supportedPhases.Length - 1
                phaseStatus(i) = CAPEOPEN110.eCapePhaseStatus.CAPE_UNKNOWNPHASESTATUS
            Next
        End If
        Try
            mat.SetPresentPhases(supportedPhases, phaseStatus)
        Catch ex As Exception
            Throw New Exception("Failed to set present phases before flash: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
        If iFlash Is Nothing Then
            Try
                iFlash = mat
            Catch ex As Exception
                Throw New Exception("Failed to get ICapeThermoEquilibriumRoutine interface from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            End Try
        End If
        Try
            iFlash.CalcEquilibrium(flashConstraint1, flashConstraint2, "Unspecified")
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(iFlash, ex))
        End Try
    End Sub

    Public Overrides Sub PHFlash(Optional haveGuess As Boolean = False)
        Dim flashConstraint2(2) As String
        flashConstraint2(0) = "Enthalpy"
        flashConstraint2(2) = "Overall"
        PFlash(flashConstraint2, haveGuess)
    End Sub

    Public Overrides Sub TPFlash(Optional haveGuess As Boolean = False)
        Dim flashConstraint2(2) As String
        flashConstraint2(0) = "Temperature"
        flashConstraint2(2) = "Overall"
        PFlash(flashConstraint2, haveGuess)
    End Sub

    Public Overrides Sub PVFFlash(Optional haveGuess As Boolean = False)
        Dim flashConstraint2(2) As String
        flashConstraint2(0) = "PhaseFraction"
        flashConstraint2(1) = "mole"
        flashConstraint2(2) = GetVaporPhaseName()
        PFlash(flashConstraint2, haveGuess)
    End Sub

    Public Overrides Function PresentPhases() As String()
        Try
            Dim phases, status
            phases = Nothing
            status = Nothing
            mat.GetPresentPhases(phases, status)
            Return phases
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Public Overrides Function PropNameFromPropertyInstance(pi As PropertyInstance) As String
        Return pi.name11
    End Function

    Public Overrides Sub Release()
        'Note: should be called on, and only on, Duplicate materials, not on ones obtained from ports
        CAPEOPENBase.ReleaseIfCOM(mat)
    End Sub

    Public Overrides Sub SetOverallProp(propName As String, basis As String, data() As Double, Optional isMixture As Boolean = True)
        Try
            mat.SetOverallProp(propName, basis, data)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Sub SetSinglePhaseProp(propName As String, phase As String, basis As String, data() As Double, Optional isMixture As Boolean = True)
        Try
            mat.SetSinglePhaseProp(propName, phase, basis, data)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Function CheckSupported(pi As PropertyInfo) As Boolean
        If propList Is Nothing Then
            propList = New Dictionary(Of String, Boolean)
            Try
                If iRoutine Is Nothing Then iRoutine = mat
                Dim props() As String = iRoutine.GetSinglePhasePropList
                For Each prop As String In props
                    propList(prop.ToLower()) = True
                Next
                'always support these
                propList("temperature") = True
                propList("pressure") = True
                propList("vaporfraction") = True
                propList("phasefraction") = True
                propList("flow") = True
                propList("totalflow") = True
                propList("fraction") = True
            Catch ex As Exception
                'assume all properties are supported
                For Each p As PropertyInfo In PropertiesManager.GetProperties
                    propList(p.name11.ToLower()) = True
                Next
            End Try
        End If
        Dim supported As Boolean = False
        propList.TryGetValue(pi.name11.ToLower, supported)
        Return supported
    End Function

    Public Overrides Sub SetPhasePressure(phase As String, P As Double)
        Dim d(0) As Double
        d(0) = P
        Try
            mat.SetSinglePhaseProp("Pressure", phase, vbNullString, d)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try

    End Sub

    Public Overrides Sub SetPhaseTemperature(phase As String, T As Double)
        Dim d(0) As Double
        d(0) = T
        Try
            mat.SetSinglePhaseProp("Temperature", phase, vbNullString, d)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Function PhaseSelectionOptions() As String()
        If iPhases Is Nothing Then
            Try
                iPhases = mat
            Catch ex As Exception
                Throw New Exception("Failed to get ICapeThermoPhases interface from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            End Try
        End If
        Try
            Dim aggState = Nothing
            Dim keyComps = Nothing
            iPhases.GetPhaseList(supportedPhases, aggState, keyComps)
            Return supportedPhases
        Catch ex As Exception
            Throw New Exception("Failed to get list of supported phase from material object: " + CAPEOPENBase.ErrorFromCOObject(iPhases, ex))
        End Try
    End Function

    Public Overrides Function TryGetReferencePhase(referencePhase As String, referencePhaseAggState As String) As String
        If iPhases Is Nothing Then
            Try
                iPhases = mat
            Catch ex As Exception
                Throw New Exception("Failed to get ICapeThermoPhases interface from material object: " + CAPEOPENBase.ErrorFromCOObject(mat, ex))
            End Try
        End If
        Try
            Dim aggState = Nothing
            Dim keyComps = Nothing
            iPhases.GetPhaseList(supportedPhases, aggState, keyComps)
            Dim l As New List(Of String)
            Dim i As Integer
            For i = 0 To supportedPhases.Length - 1
                If CAPEOPENBase.SameString(aggState(i), referencePhaseAggState) Then l.Add(supportedPhases(i))
            Next
            If l.Count = 0 Then Return Nothing
            If l.Count = 1 Then Return l(0)
            'check for exact match
            For Each s As String In l
                If CAPEOPENBase.SameString(s, referencePhase) Then Return s
            Next
            'we cannot pick between multiple phases
            Dim sb As New StringBuilder
            sb.Append("Ambiguous reference phase """)
            sb.Append(referencePhase)
            sb.Append(""" (state of aggregation: ")
            sb.Append(referencePhaseAggState)
            sb.Append("). Not sure which to select of {")
            For i = 0 To l.Count - 1
                If i > 0 Then sb.Append(";"c)
                sb.Append(l(i))
            Next
            sb.Append("}"c)
            Throw New Exception(sb.ToString)
        Catch ex As Exception
            Throw New Exception("Failed to get list of supported phase from material object: " + CAPEOPENBase.ErrorFromCOObject(iPhases, ex))
        End Try
    End Function

End Class

