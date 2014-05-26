'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Xml

Public MustInherit Class PropertyContainer
    Inherits Dictionary(Of String, PropertyData)
    Implements DisplayOrderSorter.ISortOrderItemProvider
    Dim proplist As New List(Of DisplayInfo) 'order in which properties are added; properties are never removed
    Shared separator As Char() = {";"c}
    Function TryGetProperty(prop As String) As PropertyData
        Dim p As PropertyData = Nothing
        TryGetValue(prop, p)
        Return p
    End Function
    Sub SetPropertyValue(prop As String, value As Double, Optional unitOfMeasure As String = Nothing)
        Me(prop) = New PropertyData(prop, value, unitOfMeasure)
        proplist.Add(New DisplayInfo(prop, unitOfMeasure))
    End Sub
    Sub SetPropertyValue(prop As String, values() As Double, Optional unitOfMeasure As String = Nothing)
        Me(prop) = New PropertyData(prop, values, unitOfMeasure)
        proplist.Add(New DisplayInfo(prop, unitOfMeasure))
    End Sub
    Function TryGetPropertyValue(prop As String, Optional unitOfMeasure As String = Nothing) As Double()
        Dim res() As Double = Nothing
        Dim p As PropertyData = TryGetProperty(prop)
        If p IsNot Nothing Then If p.unit = unitOfMeasure Then Return p.value
        Return Nothing
    End Function
    Protected Sub CopyPropertiesFrom(other As PropertyContainer)
        For Each d As DisplayInfo In other.proplist
            proplist.Add(d.Clone)
        Next
        For Each e As KeyValuePair(Of String, PropertyData) In other
            Me(e.Key) = e.Value.Clone
        Next
    End Sub
    Sub ReadPropertyFromXML(node As XmlNode)
        Dim name As String = Nothing
        Try
            Dim unitOfMeasure As String = Nothing
            For Each at As Xml.XmlAttribute In node.Attributes
                Select Case at.Name
                    Case "Name"
                        name = at.Value
                    Case "UnitOfMeasure"
                        unitOfMeasure = at.Value
                End Select
            Next
            Dim values() As Double = Nothing
            If name Is Nothing Then
                Throw New Exception("""" + Me.ToString + """: property missing Name attribute")
            Else
                Dim vals() As String = node.InnerText.Split(separator)
                ReDim values(vals.Length - 1)
                For i = 0 To values.Length - 1
                    values(i) = Double.Parse(vals(i))
                Next
            End If
            If values Is Nothing Then Throw New Exception("no values")
            SetPropertyValue(name, values, unitOfMeasure)
        Catch ex As Exception
            If name Is Nothing Then
                Throw New Exception("Failed to read """ + Me.ToString + """ unnamed property: " + ex.Message)
            Else
                Throw New Exception("Failed to read """ + Me.ToString + """ property """ + name + """: " + ex.Message)
            End If
        End Try
    End Sub
    Sub StorePropertiesToXML(xmlDoc As XmlDocument, parentNode As XmlNode)
        Dim node As XmlNode
        Dim att As XmlAttribute
        Dim sb As New System.Text.StringBuilder
        For Each propEntry As KeyValuePair(Of String, PropertyData) In Me
            node = xmlDoc.CreateElement("Property")
            att = xmlDoc.CreateAttribute("Name")
            att.Value = propEntry.Key
            node.Attributes.Append(att)
            If propEntry.Value.unit IsNot Nothing Then
                att = xmlDoc.CreateAttribute("UnitOfMeasure")
                att.Value = propEntry.Value.unit
                node.Attributes.Append(att)
            End If
            sb.Length = 0
            For i = 0 To propEntry.Value.value.Length - 1
                If (i <> 0) Then sb.Append(";"c)
                sb.Append(propEntry.Value.value(i).ToString(XFlowFile.englishFormatProvider))
            Next
            node.InnerText = sb.ToString
            parentNode.AppendChild(node)
        Next
    End Sub

    Friend Sub SaveProperties(s As StorageData, baseName As String)
        Dim prefix As String = baseName + "props."
        Dim nameList As New List(Of String)
        'maintain order in which the properties where added
        s(prefix + "count") = New StorageDataItem(proplist.Count)
        Dim i As Integer
        For i = 1 To proplist.Count
            'we presume this does not fail:
            Dim d() As Double = TryGetProperty(proplist(i - 1).prop).value
            Debug.Assert(d IsNot Nothing)
            Dim pre As String = prefix + i.ToString + "."c
            s(pre + "name") = New StorageDataItem(proplist(i - 1).prop)
            If (d.Length = 1) Then
                s(pre + "data") = New StorageDataItem(d(0)) 'store as double
            Else
                s(pre + "data") = New StorageDataItem(d)
            End If
            If proplist(i - 1).unitOfMeasure IsNot Nothing Then s(pre + "unit") = New StorageDataItem(proplist(i - 1).unitOfMeasure)
        Next
    End Sub

    Friend Sub LoadProperties(s As StorageData, baseName As String)
        Dim prefix As String = baseName + "props."
        Try
            Dim i, count As Integer
            count = CInt(s(prefix + "count").data)
            For i = 1 To count
                Dim pre As String = prefix + i.ToString + "."c
                Dim name As String = CStr(s(pre + "name").data)
                Dim data() As Double
                Dim si As StorageDataItem = Nothing
                s.TryGetValue(pre + "data", si)
                If (si.type = StorageDataType.DoubleType) Then
                    ReDim data(0)
                    data(0) = CDbl(si.data)
                ElseIf (si.type = StorageDataType.DoubleArrayType) Then
                    data = CType(si.data, Double())
                Else
                    Debug.Assert(False)
                    Continue For
                End If
                s.TryGetValue(pre + "unit", si)
                Dim unit As String = Nothing
                If si IsNot Nothing Then unit = CStr(si.data)
                SetPropertyValue(name, data, unit)
            Next
        Catch ex As Exception
            Debug.Assert(False)
        End Try
    End Sub

    Class DisplayInfo
        Implements DisplayOrderSorter.ISortOrderItem
        Sub New(prop As String, unitOfMeasure As String)
            Me.prop = prop
            Me.unitOfMeasure = unitOfMeasure
        End Sub
        Friend prop As String, unitOfMeasure As String
        Dim ID As String
        Public Function GetID() As String Implements DisplayOrderSorter.ISortOrderItem.GetID
            If ID Is Nothing Then
                ID = prop
                If unitOfMeasure IsNot Nothing Then ID += "/" + unitOfMeasure
            End If
            Return ID
        End Function
        Friend Function Clone() As DisplayInfo
            Return New DisplayInfo(Me.prop, Me.unitOfMeasure)
        End Function
    End Class
    Public Function GetSortOrderItems() As DisplayOrderSorter.ISortOrderItem() Implements DisplayOrderSorter.ISortOrderItemProvider.GetSortOrderItems
        Return proplist.ToArray
    End Function
End Class
