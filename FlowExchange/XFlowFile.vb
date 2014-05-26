'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Globalization
Imports System.Threading
Imports System.Xml

Public Class XFlowFile

    Friend Shared englishFormatProvider As Globalization.NumberFormatInfo

    Shared Sub New()
        Dim groupSizes As Integer() = {0}
        englishFormatProvider = New System.Globalization.NumberFormatInfo()
        englishFormatProvider.NumberDecimalSeparator = "."
        englishFormatProvider.NumberGroupSeparator = String.Empty
        englishFormatProvider.NumberDecimalDigits = 20
        englishFormatProvider.NumberNegativePattern = 1
        englishFormatProvider.NumberGroupSizes = groupSizes
    End Sub

    Shared Sub Write(pathName As String, flow As Flow, Optional options As XFlowFileOptions = Nothing)
        Dim docXFlow As New XmlDocument
        If options Is Nothing Then options = New XFlowFileOptions 'use defaults
        docXFlow.AppendChild(docXFlow.CreateXmlDeclaration("1.0", "UTF-8", Nothing))
        Dim rootNode As XmlNode = docXFlow.CreateElement("XFlow")
        Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
        If options.writeCreatorComment Then
            Try
                Dim comment As New System.Text.StringBuilder
                comment.Append("Written by ")
                comment.Append(System.IO.Path.GetFileName(Environment.GetCommandLineArgs(0)))
                comment.Append(" at ")
                Dim now As Date = Date.Now
                comment.Append(now.ToShortDateString)
                comment.Append(" "c)
                comment.Append(now.ToShortTimeString)
                comment.Append(", executed by ")
                comment.Append(Environment.UserName)
                comment.Append(" at ")
                comment.Append(Environment.MachineName)
                docXFlow.AppendChild(docXFlow.CreateComment(comment.ToString))
            Catch
            End Try
        End If
        Thread.CurrentThread.CurrentCulture = originalCulture
        docXFlow.AppendChild(rootNode)
        Dim node As XmlNode
        If options.writeComments Then rootNode.AppendChild(docXFlow.CreateComment("Overall properties"))
        flow.StorePropertiesToXML(docXFlow, rootNode)
        If (flow.compounds.Count > 0) Then
            If options.writeComments Then rootNode.AppendChild(docXFlow.CreateComment("Compounds"))
            For Each c As Compound In flow.compounds
                Dim cNode As XmlNode = docXFlow.CreateElement("Compound")
                If c.compID IsNot Nothing Then
                    node = docXFlow.CreateElement("ID")
                    node.InnerText = c.compID
                    cNode.AppendChild(node)
                End If
                If c.name IsNot Nothing Then
                    node = docXFlow.CreateElement("name")
                    node.InnerText = c.name
                    cNode.AppendChild(node)
                End If
                If c.CAS IsNot Nothing Then
                    node = docXFlow.CreateElement("CAS")
                    node.InnerText = c.CAS
                    cNode.AppendChild(node)
                End If
                If c.formula IsNot Nothing Then
                    node = docXFlow.CreateElement("Formula")
                    node.InnerText = c.formula
                    cNode.AppendChild(node)
                End If
                c.StorePropertiesToXML(docXFlow, cNode)
                rootNode.AppendChild(cNode)
            Next
        End If
        If (flow.phases.Count > 0) Then
            If options.writeComments Then rootNode.AppendChild(docXFlow.CreateComment("Phases"))
            For Each p As Phase In flow.phases
                Dim pNode As XmlNode = docXFlow.CreateElement("Phase")
                node = docXFlow.CreateElement("Name")
                node.InnerText = p.name
                pNode.AppendChild(node)
                node = docXFlow.CreateElement("StateOfAggregation")
                node.InnerText = p.stateOfAggregation
                pNode.AppendChild(node)
                If p.keyCompound IsNot Nothing Then
                    node = docXFlow.CreateElement("KeyCompound")
                    node.InnerText = p.keyCompound
                    pNode.AppendChild(node)
                End If
                p.StorePropertiesToXML(docXFlow, pNode)
                rootNode.AppendChild(pNode)
            Next
        End If
        If options.writeEnthalpy Then
            If Not Double.IsNaN(flow.molarHcorrected) Or Not Double.IsNaN(flow.massHcorrected) Then
                If options.writeComments Then rootNode.AppendChild(docXFlow.CreateComment("Enthalpy is relative to a known single-phase state at overall composition"))
                If Not Double.IsNaN(flow.molarHcorrected) Then
                    node = docXFlow.CreateElement("MolarEnthalpy")
                    node.InnerText = flow.molarHcorrected.ToString(englishFormatProvider)
                    If options.writeComments Then node.AppendChild(docXFlow.CreateComment("[J/mol]"))
                    rootNode.AppendChild(node)
                End If
                If Not Double.IsNaN(flow.massHcorrected) Then
                    node = docXFlow.CreateElement("SpecificEnthalpy")
                    node.InnerText = flow.massHcorrected.ToString(englishFormatProvider)
                    If options.writeComments Then node.AppendChild(docXFlow.CreateComment("[J/kg]"))
                    rootNode.AppendChild(node)
                End If
                If Not Double.IsNaN(flow.referenceP) Then
                    node = docXFlow.CreateElement("EnthalpyReferencePressure")
                    node.InnerText = flow.referenceP.ToString(englishFormatProvider)
                    If options.writeComments Then node.AppendChild(docXFlow.CreateComment("[Pa]"))
                    rootNode.AppendChild(node)
                End If
                If Not Double.IsNaN(flow.referenceT) Then
                    node = docXFlow.CreateElement("EnthalpyReferenceTemperature")
                    node.InnerText = flow.referenceT.ToString(englishFormatProvider)
                    If options.writeComments Then node.AppendChild(docXFlow.CreateComment("[K]"))
                    rootNode.AppendChild(node)
                End If
                If flow.referencePhase IsNot Nothing Then
                    node = docXFlow.CreateElement("EnthalpyReferencePhase")
                    node.InnerText = flow.referencePhase
                    rootNode.AppendChild(node)
                End If
                If flow.referencePhaseAggState IsNot Nothing Then
                    node = docXFlow.CreateElement("EnthalpyReferencePhaseAggregationState")
                    node.InnerText = flow.referencePhaseAggState
                    rootNode.AppendChild(node)
                End If
            End If
        End If
        If options.writeAutoFlash Then
            If options.writeComments Then rootNode.AppendChild(docXFlow.CreateComment("Suggested flash type, if nothing else is specified"))
            If flow.autoFlash IsNot Nothing Then
                node = docXFlow.CreateElement("AutoFlash")
                node.InnerText = flow.autoFlash
                rootNode.AppendChild(node)
            End If
        End If
        docXFlow.Save(pathName)
    End Sub

    Shared Function Read(pathName As String, ByRef warnings As List(Of String)) As Flow
        Dim docXFlow As New XmlDocument
        docXFlow.Load(pathName)
        Dim RootNode As XmlNode = docXFlow.SelectSingleNode("/XFlow")
        Dim f As New Flow
        For Each node As XmlNode In RootNode.ChildNodes
            Select Case node.Name
                Case "#comment"
                    'ignore
                Case "Property"
                    Try
                        f.ReadPropertyFromXML(node)
                    Catch ex As Exception
                        warnings.Add(ex.Message)
                    End Try
                Case "Compound"
                    Dim c As New Compound
                    c.compID = "<unknown>"
                    For Each cNode As XmlNode In node.ChildNodes
                        Select Case cNode.Name
                            Case "#comment"
                                'ignore
                            Case "ID"
                                c.compID = cNode.InnerText.Trim
                            Case "name"
                                c.name = cNode.InnerText.Trim
                            Case "CAS"
                                c.CAS = cNode.InnerText.Trim
                            Case "Formula"
                                c.formula = cNode.InnerText.Trim
                            Case "Property"
                                Try
                                    c.ReadPropertyFromXML(cNode)
                                Catch ex As Exception
                                    warnings.Add(ex.Message)
                                End Try
                        End Select
                    Next
                    If c.name IsNot Nothing Then c.name = c.compID
                    f.compounds.Add(c)
                Case "Phase"
                    Dim p As New Phase
                    p.name = "<unknown>"
                    For Each pNode As XmlNode In node.ChildNodes
                        Select Case pNode.Name
                            Case "#comment"
                                'ignore
                            Case "Name"
                                p.name = pNode.InnerText.Trim
                            Case "KeyCompound"
                                p.keyCompound = pNode.InnerText.Trim
                            Case "StateOfAggregation"
                                p.stateOfAggregation = pNode.InnerText.Trim
                            Case "Property"
                                Try
                                    p.ReadPropertyFromXML(pNode)
                                Catch ex As Exception
                                    warnings.Add(ex.Message)
                                End Try
                        End Select
                    Next
                    f.phases.Add(p)
                Case "MolarEnthalpy"
                    Try
                        f.molarHcorrected = Double.Parse(node.InnerText, englishFormatProvider)
                    Catch ex As Exception
                        warnings.Add("Failed to read MolarEnthalpy: " + ex.Message)
                    End Try
                Case "SpecificEnthalpy"
                    Try
                        f.massHcorrected = Double.Parse(node.InnerText, englishFormatProvider)
                    Catch ex As Exception
                        warnings.Add("Failed to read SpecificEnthalpy: " + ex.Message)
                    End Try
                Case "EnthalpyReferencePressure"
                    Try
                        f.referenceP = Double.Parse(node.InnerText, englishFormatProvider)
                    Catch ex As Exception
                        warnings.Add("Failed to read EnthalpyReferencePressure: " + ex.Message)
                    End Try
                Case "EnthalpyReferenceTemperature"
                    Try
                        f.referenceT = Double.Parse(node.InnerText, englishFormatProvider)
                    Catch ex As Exception
                        warnings.Add("Failed to read EnthalpyReferenceTemperature: " + ex.Message)
                    End Try
                Case "EnthalpyReferencePhase"
                    Try
                        f.referencePhase = node.InnerText.Trim
                    Catch ex As Exception
                        warnings.Add("Failed to read EnthalpyReferencePhase: " + ex.Message)
                    End Try
                Case "EnthalpyReferencePhaseAggregationState"
                    Try
                        f.referencePhaseAggState = node.InnerText.Trim
                    Catch ex As Exception
                        warnings.Add("Failed to read EnthalpyReferencePhaseAggregationState: " + ex.Message)
                    End Try
                Case "AutoFlash"
                    Try
                        f.autoFlash = node.InnerText.Trim
                    Catch ex As Exception
                        warnings.Add("Failed to read AutoFlash: " + ex.Message)
                    End Try
                Case Else
                    warnings.Add("Unknown/unsupported overall node """ + node.Name + """")
            End Select
        Next
        Return f
    End Function

End Class
