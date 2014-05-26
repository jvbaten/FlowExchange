'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Reflection
Imports System.Xml

Public Class PropertiesManager

    Shared Sub New()
        'parse properties.conf
        Try
            Dim path As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetAssembly(GetType(PropertiesManager)).Location), "Properties.conf")
            Dim docProps As New XmlDocument
            docProps.Load(path)
            Dim RootNode As XmlNode = docProps.SelectSingleNode("/Properties")
            Dim f As New Flow
            For Each pNode As XmlNode In RootNode.ChildNodes
                If pNode.Name = "Property" Then
                    Dim p As New PropertyInfo
                    For Each att As XmlAttribute In pNode.Attributes
                        Select Case att.Name
                            Case "name"
                                p.exposedName = att.Value.Trim
                            Case "name10"
                                p.name10 = att.Value.Trim
                            Case "name11"
                                p.name11 = att.Value.Trim
                            Case "unit"
                                p.unit = att.Value.Trim
                            Case "basis"
                                p.basis = att.Value.Trim
                            Case "overall"
                                If String.Compare(att.Value.Trim, "true", True) = 0 Then p.allowForOverall = True
                            Case "phase"
                                If String.Compare(att.Value.Trim, "true", True) = 0 Then p.allowForPhase = True
                            Case "compound"
                                If String.Compare(att.Value.Trim, "true", True) = 0 Then p.allowForCompound = True
                            Case Else
                                Debug.Assert(False)
                        End Select
                    Next
                    'sanity check
                    If (String.IsNullOrEmpty(p.exposedName)) Then
                        Debug.Assert(False)
                        Continue For
                    End If
                    If (String.IsNullOrEmpty(p.name10) Or String.IsNullOrEmpty(p.name11)) Then
                        Debug.Assert(False)
                        Continue For
                    End If
                    If Not p.allowForPhase And Not p.allowForOverall And Not p.allowForCompound Then
                        'never exposed
                        Debug.Assert(False)
                        Continue For
                    End If
                    'all ok 
                    properties.Add(p)
                    propertyDict(p.exposedName) = p
                End If
            Next
        Catch ex As Exception
            'no properties...
            Debug.Assert(False)
        End Try
    End Sub

    Shared properties As New List(Of PropertyInfo)
    Shared propertyDict As New Dictionary(Of String, PropertyInfo)

    Friend Shared Function TryGetPropertyInfo(exposedName As String) As PropertyInfo
        Dim pi As PropertyInfo = Nothing
        propertyDict.TryGetValue(exposedName, pi)
        Return pi
    End Function

    Friend Shared Function GetProperties() As List(Of PropertyInfo)
        Return properties
    End Function

End Class
