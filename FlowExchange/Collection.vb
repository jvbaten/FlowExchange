'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN

Public Class Collection(Of CollectionItem)
    Inherits CAPEOPENBase
    Implements ICapeCollection

    Public Function Count() As Integer Implements ICapeCollection.Count
        Return itemList.Count
    End Function

    Public Function Item(id As Object) As Object Implements ICapeCollection.Item
        If IsNumeric(id) Then
            '1 based integer
            Dim index As Integer
            Try
                index = CInt(id)
            Catch
                RaiseError("ID is numeric, but cannot be converted to integer", "Item", "ICapeCollection")
                Return Nothing
            End Try
            If (index < 1) Or (index > itemList.Count) Then
                RaiseError("index out of range", "Item", "ICapeCollection")
                Return Nothing
            End If
            Return itemList(index - 1)
        Else
            'string
            Dim strID As String
            Try
                strID = id.ToString
            Catch
                RaiseError("ID is not numeric and cannot be converted to string", "Item", "ICapeCollection")
                Return Nothing
            End Try
            For Each p As CollectionItem In itemList
                If SameString(p.ToString, strID) Then Return p 'items in the collection must implement toString to return the ComponentName; the CAPEOPENBase class does this
            Next
            RaiseError("""" + strID + """ does not exit", "Item", "ICapeCollection")
            Return Nothing
        End If
    End Function

    Friend Overridable Sub Clear()
        itemList.Clear()
    End Sub

    Friend Sub Add(p As CollectionItem)
        itemList.Add(p)
    End Sub

    Protected itemList As New List(Of CollectionItem)

End Class
