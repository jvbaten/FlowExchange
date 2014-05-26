'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class DisplayOrderSorter
    'a class to determine the display order of multiple lists, each of which are in a certain order
    ' the order is determined by attempting to sort the entries such that each individual sort order
    ' is respected. Add the most significant sort order last.
    Public Interface ISortOrderItem
        Function GetID() As String
    End Interface
    Public Interface ISortOrderItemProvider
        Function GetSortOrderItems() As ISortOrderItem()
    End Interface
    Friend Sub AddList(newList As ISortOrderItemProvider)
        'add the items
        Dim its() As ISortOrderItem = newList.GetSortOrderItems
        Dim i, j, n As Integer
        n = its.Length - 1
        For i = 0 To n
            itemDict(its(i).GetID) = its(i)
            For j = 0 To i - 1
                sb.Length = 0
                sb.Append(its(i).GetID)
                sb.Append("^"c)
                sb.Append(its(j).GetID)
                orderDict(sb.ToString) = 1 'i after j
                sb.Length = 0
                sb.Append(its(j).GetID)
                sb.Append("^"c)
                sb.Append(its(i).GetID)
                orderDict(sb.ToString) = -1 'j before i
            Next
        Next
    End Sub
    Friend Function GetSortedItems() As ISortOrderItem()
        Dim items As New List(Of ISortOrderItem) 'sorted items
        'add all items 
        For Each e As KeyValuePair(Of String, ISortOrderItem) In itemDict
            items.Add(e.Value)
        Next
        'sort; we need a sorter that can deal with greater than, smaller than or unknown. 
        'Array.Sort will not do, this is a quick sort. 
        'We will use a bubble sort as we anticipate small sets
        Dim i As Integer, j As Integer
        For i = 0 To items.Count - 1
            For j = i + 1 To items.Count - 1
                sb.Length = 0
                sb.Append(items(i).GetID)
                sb.Append("^"c)
                sb.Append(items(j).GetID)
                Dim res As Integer = 0
                orderDict.TryGetValue(sb.ToString, res)
                If res > 0 Then
                    'swap
                    Dim swap As ISortOrderItem = items(i)
                    items(i) = items(j)
                    items(j) = swap
                End If
            Next
        Next
        'return result
        Return items.ToArray
    End Function
    Dim itemDict As New Dictionary(Of String, ISortOrderItem) 'unsorted items
    Dim orderDict As New Dictionary(Of String, Integer) 'sort indicators
    Dim sb As New System.Text.StringBuilder
End Class
