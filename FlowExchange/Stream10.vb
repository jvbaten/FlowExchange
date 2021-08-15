'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN

'version 1.0 thermo

Friend Class Stream10
    Inherits Stream
    Sub New(unit As UnitOperationBase, mat As ICapeThermoMaterialObject)
        MyBase.New(unit)
        Me.mat = mat
    End Sub
    Private mat As ICapeThermoMaterialObject
    Private propList As Dictionary(Of String, Boolean) = Nothing
    Public Overrides Function GetComps(compoundProps As List(Of PropertyInstance)) As System.Collections.Generic.List(Of Compound)
        Dim comps As New List(Of Compound)
        Try
            Dim compIDS() As String = mat.ComponentIds
            Dim oneComp(0) As String
            Dim oneProp(0) As String
            Dim d As Double
            For Each compID As String In compIDS
                oneComp(0) = compID
                Dim comp As New Compound
                comp.compID = compID
                comp.name = compID
                compIDS(0) = compID
                Try
                    oneProp(0) = "CASRegistryNumber"
                    comp.CAS = mat.GetComponentConstant(oneProp, oneComp)(0)
                Catch
                End Try
                Try
                    oneProp(0) = "ChemicalFormula"
                    comp.formula = mat.GetComponentConstant(oneProp, oneComp)(0)
                Catch
                End Try
                For Each p As PropertyInstance In compoundProps
                    Try
                        oneProp(0) = p.name10
                        d = mat.GetComponentConstant(oneProp, oneComp)(0)
                        comp.SetPropertyValue(p.exposedName, d, p.unit)
                    Catch
                    End Try
                Next
                comps.Add(comp)
            Next
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
        Return comps
    End Function

    Public Overrides Sub CalcSinglePhaseProp(props() As String, phase As String)
        Try
            Dim phases(0) As String
            phases(0) = phase
            mat.CalcProp(props, phases, "mixture")
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Function Duplicate() As Stream
        Try
            Return New Stream10(unit, mat.Duplicate)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Public Overrides Function GetKeyCompound(phase As String) As String
        Return Nothing
    End Function

    Public Overrides Function GetOverallProp(propName As String, basis As String, Optional isMixture As Boolean = True) As Double()
        Try
            Dim calcType As String = vbNullString
            If isMixture Then calcType = "mixture"
            Return mat.GetProp(propName, "overall", Nothing, calcType, basis)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Public Overrides Function GetSinglePhaseProp(propName As String, phase As String, basis As String, Optional isMixture As Boolean = True) As Double()
        Try
            Dim calcType As String = vbNullString
            If isMixture Then calcType = "mixture"
            Return mat.GetProp(propName, phase, Nothing, calcType, basis)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Public Overrides Function GetStateOfAggregation(phase As String) As String
        Dim phaseLower As String = phase.ToLower
        If (phaseLower.StartsWith("v")) Then Return "Vapor" 'assume all vapor phases start with v
        If (phaseLower.StartsWith("l")) Then Return "Liquid" 'assume all vapor liquid start with l
        If (phaseLower.StartsWith("s")) Then Return "Solid" 'assume all vapor solid start with s
        If (phaseLower.StartsWith("aq") Or phaseLower.StartsWith("water")) Then Return "Liquid" 'wrong, but let's support in any case
        Return Nothing
    End Function

    Public Overrides Function GetVaporPhaseName() As String
        Return "Vapor"
    End Function

    Public Overrides Sub PHFlash(Optional haveGuess As Boolean = False)
        Try
            mat.CalcEquilibrium("PH", Nothing)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Function PresentPhases() As String()
        Try
            Return mat.PhaseIds
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Function

    Public Overrides Function PropNameFromPropertyInstance(pi As PropertyInstance) As String
        Return pi.name10
    End Function

    Public Overrides Sub PVFFlash(Optional haveGuess As Boolean = False)
        Try
            mat.CalcEquilibrium("PVF", Nothing)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Sub Release()
        'Note: should be called on, and only on, Duplicate materials, not on ones obtained from ports
        CAPEOPENBase.ReleaseIfCOM(mat)
    End Sub

    Public Overrides Sub SetOverallProp(propName As String, basis As String, data() As Double, Optional isMixture As Boolean = True)
        Try
            Dim calcType As String = vbNullString
            If isMixture Then calcType = "mixture"
            mat.SetProp(propName, "overall", Nothing, calcType, basis, data)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Sub SetSinglePhaseProp(propName As String, phase As String, basis As String, data() As Double, Optional isMixture As Boolean = True)
        Try
            Dim calcType As String = vbNullString
            If isMixture Then calcType = "mixture"
            mat.SetProp(propName, phase, Nothing, calcType, basis, data)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Sub TPFlash(Optional haveGuess As Boolean = False)
        Try
            mat.CalcEquilibrium("TP", Nothing)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Function CheckSupported(pi As PropertyInfo) As Boolean
        If propList Is Nothing Then
            propList = New Dictionary(Of String, Boolean)
            Try
                Dim props() As String = mat.GetPropList
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
                    propList(p.name10.ToLower()) = True
                Next
            End Try
        End If
        Dim supported As Boolean = False
        propList.TryGetValue(pi.name10.ToLower, supported)
        Return supported
    End Function

    Public Overrides Sub SetPhasePressure(phase As String, P As Double)
        Dim d(0) As Double
        d(0) = P
        Try
            mat.SetProp("Pressure", "Overall", Nothing, vbNullString, vbNullString, d)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Sub SetPhaseTemperature(phase As String, T As Double)
        Dim d(0) As Double
        d(0) = T
        Try
            mat.SetProp("Temperature", "Overall", Nothing, vbNullString, vbNullString, d)
        Catch ex As Exception
            Throw New Exception(CAPEOPENBase.ErrorFromCOObject(mat, ex))
        End Try
    End Sub

    Public Overrides Function PhaseSelectionOptions() As String()
        'which phases can we select? check which ones live on the current material, add 'Vapor', 'Liquid', 'Solid'
        Dim res() As String
        Try
            res = mat.PhaseIds
        Catch ex As Exception
            res = Nothing
        End Try
        Dim phaseList As New List(Of String)
        If res IsNot Nothing Then
            For Each s As String In res
                If CAPEOPENBase.SameString(s, "Overall") Then Continue For
                Dim testPhase As String
                Dim twoPhase As Boolean = False
                For i As Integer = 0 To phaseList.Count - 1
                    For j As Integer = 0 To i - 1
                        testPhase = phaseList(i) + phaseList(j)
                        If CAPEOPENBase.SameString(s, testPhase) Then
                            twoPhase = True
                            Exit For
                        End If
                        testPhase = phaseList(j) + phaseList(i)
                        If CAPEOPENBase.SameString(s, testPhase) Then
                            twoPhase = True
                            Exit For
                        End If
                    Next
                    If twoPhase Then Exit For
                Next
                If twoPhase Then Continue For
                phaseList.Add(s)
            Next
        End If
        Dim standardPhaseList() As String = {"Vapor", "Liquid", "Solid"}
        For Each s As String In standardPhaseList
            If Not phaseList.Contains(s) Then phaseList.Add(s)
        Next
        Return phaseList.ToArray
    End Function

    Public Overrides Function TryGetReferencePhase(referencePhase As String, referencePhaseAggState As String) As String
        'little we can do in 1.0 but return the reference state itself
        Return referencePhaseAggState
    End Function

End Class
