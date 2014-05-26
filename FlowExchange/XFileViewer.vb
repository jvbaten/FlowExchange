'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Text

Public Class XFlowFileViewer
    Inherits ListView
    Dim root As New LVSection(Nothing)
    Const levelIndent As Integer = 10
    Const spacing As Integer = 5

    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer

    Friend Shared stringFormat As New StringFormat

    Shared Sub New()
        Dim groupSizes As Integer() = {0}
        stringFormat.Alignment = StringAlignment.Near
        stringFormat.LineAlignment = StringAlignment.Center
        stringFormat.Trimming = StringTrimming.EllipsisCharacter
    End Sub

    MustInherit Class LVLine
        Inherits ListViewItem
        MustOverride Sub AddTo(v As ListView)
    End Class

    Class LVSection
        Inherits LVLine
        Sub New(text As String, Optional shown As Boolean = True)
            Me.Text = text
            Me.shown = shown
        End Sub
        Friend level As Integer = -1
        Friend shown As Boolean
        Friend content As New List(Of LVLine)
        Function AddDataLine(text As String) As LVData
            Dim line As New LVData(text)
            content.Add(line)
            line.level = level
            Return line
        End Function
        Function AddSection(Title As String, Optional shown As Boolean = True) As LVSection
            Dim line As New LVSection(Title, shown)
            content.Add(line)
            line.level = level + 1
            Return line
        End Function
        Public Overrides Sub AddTo(v As System.Windows.Forms.ListView)
            If (level >= 0) Then v.Items.Add(Me)
            If shown Then
                For Each line As LVLine In content
                    line.AddTo(v)
                Next
            End If
        End Sub
    End Class

    Class LVData
        Inherits LVLine
        Sub New(s As String)
            Me.Text = s
            content.Add(s)
        End Sub
        Public Overrides Sub AddTo(v As System.Windows.Forms.ListView)
            v.Items.Add(Me)
        End Sub
        Friend level As Integer
        Friend Sub AddData(s As String)
            SubItems.Add(s)
            content.Add(s)
        End Sub
        Friend Sub AddData(d As Double)
            SubItems.Add(d.ToString("g5", XFlowFile.englishFormatProvider))
            content.Add(d)
        End Sub
        Friend Sub AddData()
            SubItems.Add(vbNullString)
            content.Add(Nothing)
        End Sub
        Friend content As New List(Of Object)
    End Class

    Sub New(flows() As Flow, flowTitles() As String)
        Dim i, j As Integer, n As Integer = flows.Length - 1
        Dim txt As String
        Debug.Assert(flows.Length > 0)
        Debug.Assert(flows.Length = flowTitles.Length)
        Me.HeaderStyle = Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.View = Windows.Forms.View.Details
        Me.FullRowSelect = True
        Me.OwnerDraw = True
        Me.DoubleBuffered = True
        Me.BackColor = Color.LightGray
        AddHandler Me.DrawItem, AddressOf DrawListItem
        AddHandler Me.DrawSubItem, AddressOf DrawListSubItem
        AddHandler Me.MouseClick, AddressOf MouseClickList
        AddHandler Me.DrawColumnHeader, AddressOf DrawListColumnHeader
        'create the data table
        Columns.Add("")
        Columns.Add("Unit")
        For Each s As String In flowTitles
            Columns.Add(s)
        Next
        'sort the compounds in display order
        Dim sorter As New DisplayOrderSorter
        For i = n To 0 Step -1
            sorter.AddList(flows(i).GetCompoundSortOrderProvider)
        Next
        Dim sortedCompounds() = sorter.GetSortedItems
        'create compound index lookup map
        Dim compIndices(n, sortedCompounds.Length - 1) As Integer
        Dim compoundIndexMap As New Dictionary(Of String, Integer)
        For j = 0 To sortedCompounds.Length - 1
            compoundIndexMap(sortedCompounds(j).GetID) = j
        Next
        For i = 0 To n
            For j = 0 To sortedCompounds.Length - 1
                compIndices(i, j) = -1
            Next
            For j = 0 To flows(i).compounds.Count - 1
                compIndices(i, compoundIndexMap(flows(i).compounds(j).GetID)) = j
            Next
        Next
        'get sorted overall properties
        sorter = New DisplayOrderSorter
        For i = n To 0 Step -1
            sorter.AddList(flows(i))
        Next
        Dim sortedProps() = sorter.GetSortedItems
        Dim d()() As Double
        ReDim d(n)
        Dim overall As LVSection = root.AddSection("Overall properties")
        For Each di As PropertyContainer.DisplayInfo In sortedProps
            'get data to see about dimensionality
            Dim perCompound As Boolean = False
            For i = 0 To n
                d(i) = flows(i).TryGetPropertyValue(di.prop, di.unitOfMeasure)
                If d(i) IsNot Nothing Then
                    If d(i).Length > 1 Then
                        perCompound = True
                    End If
                End If
            Next
            If perCompound Then
                'add section, and data per compound
                Dim sec As LVSection = overall.AddSection(di.prop)
                For j = 0 To sortedCompounds.Length - 1
                    Dim line As LVData = sec.AddDataLine(CType(sortedCompounds(j), Compound).name)
                    line.AddData(di.unitOfMeasure)
                    For i = 0 To n
                        If d(i) Is Nothing Then
                            line.AddData()
                        ElseIf compIndices(i, j) < 0 Then
                            line.AddData()
                        Else
                            line.AddData(d(i)(compIndices(i, j)))
                        End If
                    Next
                Next
            Else
                'add one data line
                Dim line As LVData = overall.AddDataLine(di.prop)
                line.AddData(di.unitOfMeasure)
                For i = 0 To n
                    If d(i) Is Nothing Then
                        line.AddData()
                    Else
                        line.AddData(d(i)(0))
                    End If
                Next
            End If
        Next
        'compounds
        Dim compounds As LVSection = root.AddSection("Compounds", False)
        For j = 0 To sortedCompounds.Length - 1
            Dim compName As String = CType(sortedCompounds(j), Compound).name
            Dim sec As LVSection = compounds.AddSection(compName)
            Dim line As LVData
            'CAS
            Dim check As Boolean = False
            For i = 0 To n
                If compIndices(i, j) >= 0 Then
                    If flows(i).compounds(compIndices(i, j)).CAS IsNot Nothing Then
                        check = True
                    End If
                End If
            Next
            If check Then
                line = sec.AddDataLine("CAS registry number")
                line.AddData()
                For i = 0 To n
                    txt = Nothing
                    If compIndices(i, j) >= 0 Then
                        If flows(i).compounds(compIndices(i, j)).CAS IsNot Nothing Then
                            txt = flows(i).compounds(compIndices(i, j)).CAS
                        End If
                    End If
                    line.AddData(txt)
                Next
            End If
            'formula
            check = False
            For i = 0 To n
                If compIndices(i, j) >= 0 Then
                    If flows(i).compounds(compIndices(i, j)).formula IsNot Nothing Then
                        check = True
                    End If
                End If
            Next
            If check Then
                line = sec.AddDataLine("Chemical formula")
                line.AddData()
                For i = 0 To n
                    txt = Nothing
                    If compIndices(i, j) >= 0 Then
                        If flows(i).compounds(compIndices(i, j)).formula IsNot Nothing Then
                            txt = flows(i).compounds(compIndices(i, j)).formula
                        End If
                    End If
                    line.AddData(txt)
                Next
            End If
            'properties
            sorter = New DisplayOrderSorter
            For i = n To 0 Step -1
                If compIndices(i, j) >= 0 Then
                    sorter.AddList(flows(i).compounds(compIndices(i, j)))
                End If
            Next
            sortedProps = sorter.GetSortedItems
            For Each di As PropertyContainer.DisplayInfo In sortedProps
                For i = 0 To n
                    If compIndices(i, j) >= 0 Then
                        d(i) = flows(i).compounds(compIndices(i, j)).TryGetPropertyValue(di.prop, di.unitOfMeasure)
                    Else
                        d(i) = Nothing
                    End If
                Next
                line = sec.AddDataLine(di.prop)
                line.AddData(di.unitOfMeasure)
                For i = 0 To n
                    If d(i) Is Nothing Then
                        line.AddData()
                    Else
                        line.AddData(d(i)(0))
                    End If
                Next
            Next
        Next
        'phases
        sorter = New DisplayOrderSorter
        For i = n To 0 Step -1
            sorter.AddList(flows(i).GetPhaseSortOrderProvider)
        Next
        Dim sortedPhases() = sorter.GetSortedItems
        'create phase index lookup map
        Dim phaseIndices(n, sortedPhases.Length - 1) As Integer
        Dim phaseIndexMap As New Dictionary(Of String, Integer)
        For j = 0 To sortedPhases.Length - 1
            phaseIndexMap(sortedPhases(j).GetID) = j
        Next
        For i = 0 To n
            For j = 0 To sortedPhases.Length - 1
                phaseIndices(i, j) = -1
            Next
            For j = 0 To flows(i).phases.Count - 1
                phaseIndices(i, phaseIndexMap(flows(i).phases(j).GetID)) = j
            Next
        Next
        Dim phases As LVSection = root.AddSection("Phases", False)
        For j = 0 To sortedPhases.Length - 1
            Dim phaseName As String = CType(sortedPhases(j), Phase).name
            Dim sec As LVSection = phases.AddSection(phaseName)
            Dim line As LVData
            'stateOfAggregation
            Dim check As Boolean = False
            For i = 0 To n
                If phaseIndices(i, j) >= 0 Then
                    If flows(i).phases(phaseIndices(i, j)).stateOfAggregation IsNot Nothing Then
                        check = True
                    End If
                End If
            Next
            If check Then
                line = sec.AddDataLine("State of Aggregation")
                line.AddData()
                For i = 0 To n
                    txt = Nothing
                    If phaseIndices(i, j) >= 0 Then
                        If flows(i).phases(phaseIndices(i, j)).stateOfAggregation IsNot Nothing Then
                            txt = flows(i).phases(phaseIndices(i, j)).stateOfAggregation
                        End If
                    End If
                    line.AddData(txt)
                Next
            End If
            'keyCompound
            check = False
            For i = 0 To n
                If phaseIndices(i, j) >= 0 Then
                    If flows(i).phases(phaseIndices(i, j)).keyCompound IsNot Nothing Then
                        check = True
                    End If
                End If
            Next
            If check Then
                line = sec.AddDataLine("Key compound")
                line.AddData()
                For i = 0 To n
                    txt = Nothing
                    If phaseIndices(i, j) >= 0 Then
                        If flows(i).phases(phaseIndices(i, j)).keyCompound IsNot Nothing Then
                            txt = flows(i).phases(phaseIndices(i, j)).keyCompound
                        End If
                    End If
                    line.AddData(txt)
                Next
            End If
            'properties
            sorter = New DisplayOrderSorter
            For i = n To 0 Step -1
                If phaseIndices(i, j) >= 0 Then
                    sorter.AddList(flows(i).phases(phaseIndices(i, j)))
                End If
            Next
            sortedProps = sorter.GetSortedItems
            For Each di As PropertyContainer.DisplayInfo In sortedProps
                Dim perCompound As Boolean = False
                For i = 0 To n
                    If phaseIndices(i, j) >= 0 Then
                        d(i) = flows(i).phases(phaseIndices(i, j)).TryGetPropertyValue(di.prop, di.unitOfMeasure)
                    Else
                        d(i) = Nothing
                    End If
                    If d(i) IsNot Nothing Then
                        If d(i).Length > 1 Then
                            perCompound = True
                        End If
                    End If
                Next
                If perCompound Then
                    'add section, and data per compound
                    Dim subSec As LVSection = sec.AddSection(di.prop)
                    For k As Integer = 0 To sortedCompounds.Length - 1
                        line = subSec.AddDataLine(CType(sortedCompounds(k), Compound).name)
                        line.AddData(di.unitOfMeasure)
                        For i = 0 To n
                            If d(i) Is Nothing Then
                                line.AddData()
                            ElseIf compIndices(i, k) < 0 Then
                                line.AddData()
                            Else
                                line.AddData(d(i)(compIndices(i, k)))
                            End If
                        Next
                    Next
                Else
                    'add one data line
                    line = sec.AddDataLine(di.prop)
                    line.AddData(di.unitOfMeasure)
                    For i = 0 To n
                        If d(i) Is Nothing Then
                            line.AddData()
                        Else
                            line.AddData(d(i)(0))
                        End If
                    Next
                End If
            Next
        Next
        Rebuild()
    End Sub

    Sub Rebuild()
        Me.SuspendLayout()
        Const WM_SETREDRAW As Long = &HB
        SendMessage(Handle, WM_SETREDRAW, False, 0)
        Items.Clear()
        root.AddTo(Me)
        'measure items
        Dim g As Graphics = Graphics.FromHwnd(Handle)
        Dim n As Integer = Me.Columns.Count - 1
        Dim width(n) As Integer
        Dim w As Integer
        Dim minWidth As Integer = 0
        Dim i As Integer
        For i = 0 To n
            width(i) = g.MeasureString(Me.Columns(i).Text, Me.Font).Width + 20
        Next
        For Each it As ListViewItem In Items
            If TypeOf it Is LVData Then
                Dim dat As LVData = it
                w = g.MeasureString(dat.Text, Me.Font).Width + dat.level * levelIndent
                If (width(0) < w) Then width(0) = w
                For i = 1 To n
                    w = g.MeasureString(dat.SubItems(i).Text, Me.Font).Width
                    If (width(i) < w) Then width(i) = w
                Next
            ElseIf TypeOf it Is LVSection Then
                Dim sec As LVSection = it
                w = g.MeasureString(sec.Text, Me.Font).Width + sec.level * levelIndent
                If (minWidth < w) Then minWidth = w
            End If
        Next
        minWidth += 3 * spacing + 16
        w = 0
        For i = 0 To n
            width(i) += 2 * spacing
            w += width(i)
        Next
        If (w < minWidth) Then
            w = minWidth \ (n + 1) + 1
            For i = 0 To n
                width(i) += w
            Next
        End If
        For i = 0 To n
            Columns(i).Width = width(i) + 2
        Next
        g.Dispose()
        SendMessage(Handle, WM_SETREDRAW, True, 0)
        Invalidate()
        Me.ResumeLayout()
    End Sub

    Sub DrawListItem(sender As Object, e As System.Windows.Forms.DrawListViewItemEventArgs)
        If TypeOf e.Item Is LVSection Then
            Dim sec As LVSection = e.Item
            Dim brush As New LinearGradientBrush(e.Bounds, Color.LightBlue, Color.CornflowerBlue, LinearGradientMode.Vertical)
            Try
                e.Graphics.FillRectangle(brush, e.Bounds)
            Finally
                brush.Dispose()
            End Try
            Dim left As Integer = spacing + levelIndent * sec.level
            Dim r As New Rectangle(e.Bounds.Left + spacing + left + 16, e.Bounds.Top, e.Bounds.Width - spacing - 16 - left, e.Bounds.Height)
            e.Graphics.DrawString(e.Item.Text, Me.Font, Brushes.Black, r, stringFormat)
            Dim ic As Icon
            If (sec.shown) Then ic = My.Resources.open Else ic = My.Resources.closed
            e.Graphics.DrawIcon(ic, e.Bounds.Left + left, e.Bounds.Top + (e.Bounds.Height - 16) \ 2)
        End If
    End Sub

    Sub DrawListSubItem(sender As Object, e As System.Windows.Forms.DrawListViewSubItemEventArgs)
        If TypeOf e.Item Is LVData Then
            Dim line As LVData = e.Item
            Dim brush As New LinearGradientBrush(e.Bounds, Color.White, Color.LightGray, LinearGradientMode.Vertical)
            Try
                e.Graphics.FillRectangle(brush, e.Bounds)
            Finally
                brush.Dispose()
            End Try
            Dim left As Integer = spacing
            If e.ColumnIndex = 0 Then left += levelIndent * line.level
            Dim r As New Rectangle(e.Bounds.Left + left, e.Bounds.Top, e.Bounds.Width - spacing - left, e.Bounds.Height)
            e.Graphics.DrawString(e.SubItem.Text, Me.Font, Brushes.Black, r, stringFormat)
        End If
    End Sub

    Sub DrawListColumnHeader(sender As Object, e As System.Windows.Forms.DrawListViewColumnHeaderEventArgs)
        e.DrawBackground()
        e.DrawText()
    End Sub

    Sub MouseClickList(sender As Object, e As MouseEventArgs)
        Dim it As ListViewItem = TopItem
        Dim index As Integer = TopItem.Index + (e.Location.Y - TopItem.Bounds.Top) \ TopItem.Bounds.Height
        If index < Items.Count Then
            If TypeOf Items(index) Is LVSection Then
                CType(Items(index), LVSection).shown = Not CType(Items(index), LVSection).shown
                Dim top_item As ListViewItem = Me.TopItem
                Dim hPos As Integer = 0
                Rebuild()
                TopItem = top_item
            End If
        End If
    End Sub

    Private Sub AddToString(sb As StringBuilder, line As LVLine)
        If TypeOf line Is LVData Then
            Dim dLine As LVData = line
            Dim first As Boolean = True
            For Each it As Object In dLine.content
                If first Then first = False Else sb.Append(vbTab)
                If it IsNot Nothing Then
                    If TypeOf it Is String Then
                        sb.Append(""""c)
                        sb.Append(CStr(it))
                        sb.Append(""""c)
                    Else
                        Debug.Assert(TypeOf it Is Double)
                        sb.Append(CDbl(it).ToString)
                    End If
                End If
            Next
            sb.Append(vbCrLf)
        Else
            Debug.Assert(TypeOf line Is LVSection)
            Dim sec As LVSection = line
            sb.Append(""""c)
            sb.Append(sec.Text)
            sb.Append(""""c)
            sb.Append(vbCrLf)
            For Each line In sec.content
                AddToString(sb, line)
            Next
        End If
    End Sub

    Public Sub OnCopy()
        Dim sb As New StringBuilder
        For Each line As LVLine In root.content
            AddToString(sb, line)
        Next
        Clipboard.SetData(DataFormats.Text, sb.ToString)
    End Sub

End Class
