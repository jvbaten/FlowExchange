'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Xml
Imports System.Globalization
Imports System.Threading

Public Class StorageData
    Inherits Dictionary(Of String, StorageDataItem)

    Friend Sub SaveXML(typeName As String, pathName As String)
        Dim docXML As New XmlDocument
        docXML.AppendChild(docXML.CreateXmlDeclaration("1.0", "UTF-8", Nothing))
        Dim rootNode As XmlNode = docXML.CreateElement("Storage")
        Dim att As XmlAttribute = docXML.CreateAttribute("type")
        att.Value = typeName
        rootNode.Attributes.Append(att)
        Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
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
            docXML.AppendChild(docXML.CreateComment(comment.ToString))
            docXML.AppendChild(docXML.CreateComment("This XML document is written by the FlowExchange suite of Unit Operations"))
            docXML.AppendChild(docXML.CreateComment("See http://www.amsterchem.com/ for more information"))
        Catch
        End Try
        Thread.CurrentThread.CurrentCulture = originalCulture
        docXML.AppendChild(rootNode)
        Dim node, subnode As XmlNode
        For Each e As KeyValuePair(Of String, StorageDataItem) In Me
            node = docXML.CreateElement("Item")
            att = docXML.CreateAttribute("name")
            att.Value = e.Key
            node.Attributes.Append(att)
            att = docXML.CreateAttribute("type")
            Select Case e.Value.type
                Case StorageDataType.DoubleType
                    att.Value = "Real"
                    node.InnerText = CDbl(e.Value.data).ToString(XFlowFile.englishFormatProvider)
                Case StorageDataType.StringType
                    att.Value = "String"
                    node.InnerText = CStr(e.Value.data)
                Case StorageDataType.BooleanType
                    att.Value = "Boolean"
                    If CBool(e.Value.data) Then
                        node.InnerText = "True"
                    Else
                        node.InnerText = "False"
                    End If
                Case StorageDataType.IntegerType
                    att.Value = "Integer"
                    node.InnerText = CInt(e.Value.data).ToString()
                Case StorageDataType.DoubleArrayType
                    att.Value = "RealArray"
                    For Each d As Double In CType(e.Value.data, Double())
                        subnode = docXML.CreateElement("Element")
                        subnode.InnerText = d.ToString(XFlowFile.englishFormatProvider)
                        node.AppendChild(subnode)
                    Next
                Case StorageDataType.StringArrayType
                    att.Value = "StringArray"
                    For Each s As String In CType(e.Value.data, String())
                        subnode = docXML.CreateElement("Element")
                        subnode.InnerText = s
                        node.AppendChild(subnode)
                    Next
                Case Else
                    Throw New Exception("Internal error: unknown data type in StorageData::SaveXML")
            End Select
            node.Attributes.Append(att)
            rootNode.AppendChild(node)
        Next
        docXML.Save(pathName)
    End Sub

    Friend Sub LoadXML(typeName As String, path As String)
        Dim docXML As New XmlDocument
        docXML.Load(path)
        Dim RootNode As XmlNode = docXML.SelectSingleNode("/Storage")
        Dim att As XmlAttribute
        Try
            att = RootNode.Attributes("type")
        Catch
            Throw New Exception("Attribute 'type' missing from Storage")
        End Try
        If att.Value.CompareTo(typeName) <> 0 Then
            Throw New Exception("Storage document is of type """ + att.Value + """; expected type """ + typeName + """")
        End If
        Dim f As New Flow
        For Each node As XmlNode In RootNode.ChildNodes
            If node.Name = "Item" Then
                Dim name As String = Nothing
                Dim type As String = Nothing
                For Each att In node.Attributes
                    Select Case att.Name
                        Case "name"
                            name = att.Value
                        Case "type"
                            type = att.Value
                    End Select
                Next
                If name Is Nothing Then Throw New Exception("Item missing name attribute")
                If type Is Nothing Then Throw New Exception("Item missing type attribute")
                Dim data As StorageDataItem
                Select Case type
                    Case "Real"
                        Try
                            data = New StorageDataItem(Double.Parse(node.InnerText, XFlowFile.englishFormatProvider))
                        Catch ex As Exception
                            Throw New Exception("Parse error for """ + name + """: " + ex.Message)
                        End Try
                    Case "String"
                        data = New StorageDataItem(node.InnerText)
                    Case "Boolean"
                        Try
                            data = New StorageDataItem(Boolean.Parse(node.InnerText))
                        Catch ex As Exception
                            Throw New Exception("Parse error for """ + name + """: " + ex.Message)
                        End Try
                    Case "Integer"
                        Try
                            data = New StorageDataItem(Integer.Parse(node.InnerText))
                        Catch ex As Exception
                            Throw New Exception("Parse error for """ + name + """: " + ex.Message)
                        End Try
                    Case "RealArray"
                        Dim dList As New List(Of Double)
                        For Each elnode As XmlNode In node.ChildNodes
                            If elnode.Name = "Element" Then
                                Try
                                    dList.Add(Double.Parse(node.InnerText, XFlowFile.englishFormatProvider))
                                Catch ex As Exception
                                    Throw New Exception("Parse error for """ + name + """: " + ex.Message)
                                End Try

                            End If
                        Next
                        data = New StorageDataItem(dList.ToArray)
                    Case "StringArray"
                        Dim sList As New List(Of String)
                        For Each elnode As XmlNode In node.ChildNodes
                            If elnode.Name = "Element" Then
                                sList.Add(node.InnerText)
                            End If
                        Next
                        data = New StorageDataItem(sList.ToArray)
                    Case Else
                        Throw New Exception("Item with unknown value """ + type + """ for type attribute")
                End Select
                Me(name) = data
            End If
        Next
    End Sub

End Class