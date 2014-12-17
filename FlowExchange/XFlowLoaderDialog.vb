'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports System.Windows.Forms
Imports System.Diagnostics.FileVersionInfo
Imports System.Drawing
Imports System.Reflection

Public Class XFlowLoaderDialog

    Dim unit As XFlowLoader
    Dim changed As Boolean = False
    Dim changesApplied As Boolean = False
    Dim compEditIndex As Integer
    Friend compoundMapping As New Dictionary(Of String, String) 'ID to ID, special cases of target ID: Purge (means get rid of it); Unused (error if non-zero)

    Friend Shared stringFormat As New StringFormat

    Shared Sub New()
        stringFormat.Alignment = StringAlignment.Near
        stringFormat.LineAlignment = StringAlignment.Center
        stringFormat.Trimming = StringTrimming.EllipsisCharacter
    End Sub

    Sub New(unit As XFlowLoader)
        InitializeComponent()
        Me.unit = unit
        For Each e As KeyValuePair(Of String, String) In unit.compoundMapping
            compoundMapping(e.Key) = e.Value
        Next
        CopyGridToClipboardToolStripMenuItem.Enabled = False
    End Sub

    Private Sub FillDialog()
        If (unit.xFlowFileName.value IsNot Nothing) Then XPath.Text = unit.xFlowFileName.value
        AutoLoad.Checked = unit.autoLoad.value
        FillFlowList()
        For i As Integer = 0 To ProductFlashOptions.FlashTypeDescription.Length - 1
            flashSpec.Items.Add(ProductFlashOptions.FlashTypeDescription(i))
        Next
        flashSpec.SelectedIndex = unit.flashType
        FillMappingList()
    End Sub

    Private Sub FillMappingList()
        mappingList.SuspendLayout()
        mappingList.Items.Clear()
        Dim idMap As New Dictionary(Of String, String)
        For Each c As Compound In unit.comps
            idMap(c.compID.ToLower) = c.name
        Next
        If unit.loadedFlow IsNot Nothing Then
            For Each c As Compound In unit.loadedFlow.compounds
                Dim it As ListViewItem = mappingList.Items.Add(c.name)
                it.Tag = c
                Dim mapsTo As String = Nothing
                If compoundMapping.TryGetValue(c.compID, mapsTo) Then
                    If mapsTo <> "Unused" And mapsTo <> "Purge" Then
                        'convert to name
                        Dim cname As String = Nothing
                        If idMap.TryGetValue(mapsTo.ToLower, cname) Then mapsTo = cname
                    End If
                    it.SubItems.Add(mapsTo)
                End If
            Next
        End If
        mappingList.ResumeLayout()
    End Sub

    Private Sub FillFlowList()
        Dim flowList As New List(Of Flow)
        Dim flowTitleList As New List(Of String)
        If (unit.loadedFlow IsNot Nothing) Then
            flowList.Add(unit.loadedFlow)
            flowTitleList.Add("XFlow File")
        End If
        If (unit.productFlow IsNot Nothing) Then
            flowList.Add(unit.productFlow)
            flowTitleList.Add("Product Stream at last Calculation")
        End If
        If flowList.Count = 0 Then
            Try
                Tabs.TabPages.Remove(flowTab)
            Catch
            End Try
        Else
            If Not Tabs.TabPages.Contains(flowTab) Then Tabs.TabPages.Insert(1, flowTab)
            flowTab.Controls.Clear()
            Dim flv As New XFlowFileViewer(flowList.ToArray, flowTitleList.ToArray)
            flowTab.Controls.Add(flv)
            flv.Dock = DockStyle.Fill
        End If

    End Sub

    Private Sub XFlowLoaderDialog_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.flowexchange
        Me.Text = "Unit operation " + unit.ComponentName + ":"
        aboutText.Text += vbCrLf + GetVersionInfo(GetType(XFlowSaver).Assembly.Location).LegalCopyright.ToString
        aboutText.Text += vbCrLf + "Version: " + GetVersionInfo(GetType(XFlowLoader).Assembly.Location).FileVersion.ToString
        aboutText.Text += vbCrLf + "http://www.amsterchem.com/"
        FillDialog()
    End Sub

    Private Sub closeBtn_Click(sender As System.Object, e As System.EventArgs) Handles cancelBtn.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub browseXPath_Click(sender As System.Object, e As System.EventArgs) Handles browseXPath.Click
        Dim path As String = XPath.Text
        If (String.IsNullOrEmpty(path)) Then
            path = Nothing
        Else
            Try
                Dim message As String = Nothing
                If Not unit.xFlowFileName.ValidateString(path, message) Then path = Nothing
            Catch
                path = Nothing
            End Try
        End If
        Dim dlg As New OpenFileDialog
        dlg.DefaultExt = ".xflow"
        If path IsNot Nothing Then
            dlg.FileName = path
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(path)
        End If
        dlg.Filter = "FileExchange files (*.xflow)|*.xflow"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If String.Compare(XPath.Text, dlg.FileName) <> 0 Then
                XPath.Text = dlg.FileName
                changed = True
            End If
        End If
    End Sub

    Private Sub ResetBtn_Click(sender As System.Object, e As System.EventArgs) Handles ResetBtn.Click
        If Not String.IsNullOrEmpty(XPath.Text) Then
            XPath.Text = Nothing
            changed = True
        End If
    End Sub

    Private Sub ApplyChanges()
        'deal with all modified inputs
        ' try not to set variables to values that are the same as this will invalidate the unit
        Dim path As String = XPath.Text
        If String.IsNullOrEmpty(path) Then
            If (unit.xFlowFileName.value IsNot Nothing) Then unit.xFlowFileName.value = Nothing
        Else
            If Not CAPEOPENBase.SameString(unit.xFlowFileName.value, path) Then unit.xFlowFileName.value = path
        End If
        If AutoLoad.Checked <> unit.autoLoad.value Then unit.autoLoad.value = AutoLoad.Checked
        unit.flashType = flashSpec.SelectedIndex
        unit.compoundMapping.Clear()
        For i = 0 To mappingList.Items.Count - 1
            Dim value As String = mappingList.Items(i).SubItems(1).Text
            If (value IsNot Nothing) Then
                If value <> "Unused" And value <> "Purge" Then
                    'comp ID?
                    For Each c As Compound In unit.comps
                        If c.name = value Then
                            value = c.compID
                            Exit For
                        End If
                    Next
                End If
                unit.compoundMapping(unit.loadedFlow.compounds(i).compID) = value
            End If
        Next
        compoundMapping.Clear()
        For Each e As KeyValuePair(Of String, String) In unit.compoundMapping
            compoundMapping(e.Key) = e.Value
        Next
        changed = False
    End Sub

    Private Sub okBtn_Click(sender As System.Object, e As System.EventArgs) Handles okBtn.Click
        Dim oldChanged As Boolean = changed
        ApplyChanges()
        changed = oldChanged 'save so that ConfigurationChanged returns the proper answer
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub loadNowBtn_Click(sender As System.Object, e As System.EventArgs) Handles loadNowBtn.Click
        Dim path As String = XPath.Text
        If (String.IsNullOrEmpty(path)) Then
            path = Nothing
        Else
            Try
                Dim message As String = Nothing
                If Not unit.xFlowFileName.ValidateString(path, message) Then path = Nothing
            Catch
                path = Nothing
            End Try
        End If
        Dim dlg As New OpenFileDialog
        dlg.DefaultExt = ".xflow"
        If path IsNot Nothing Then
            dlg.FileName = path
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(path)
        End If
        dlg.Filter = "FileExchange files (*.xflow)|*.xflow"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try
                Dim loadWarnings As New List(Of String)
                unit.loadedFlow = XFlowFile.Read(dlg.FileName, loadWarnings)
                For Each s In loadWarnings
                    unit.LogMessage(s, UnitOperationBase.MessageType.WarningMessage)
                Next
            Catch ex As Exception
                MsgBox("Failed to store XFlow file: " + ex.Message, MsgBoxStyle.Critical, "Save XFlow File Now:")
            End Try
            'map unmapped compounds
            For Each c As Compound In unit.loadedFlow.compounds
                Dim value As String = Nothing
                If Not compoundMapping.TryGetValue(c.compID, value) Then
                    'attempt to auto map
                    value = unit.AutoMap(c)
                    If (value IsNot Nothing) Then compoundMapping(c.compID) = value
                End If
            Next
            FillFlowList()
            FillMappingList()
        End If
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LoadToolStripMenuItem.Click
        Try
            Dim dlg As New OpenFileDialog
            dlg.DefaultExt = ".xflowloader"
            dlg.Filter = "XFlowLoader configuration files (*.xflowloader)|*.xflowloader"
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                If changed Then If MsgBox("Loading the configuration will change the configuration even if pressing Cancel. Continue?", MsgBoxStyle.OkCancel, "Load configuration:") <> MsgBoxResult.Ok Then Exit Sub
                Dim sd As New StorageData
                sd.LoadXML("XFlowLoader", dlg.FileName)
                unit.Load(sd)
                FillDialog()
                changesApplied = True
            End If
        Catch ex As Exception
            MsgBox("Failed to load configuration: " + ex.Message, MsgBoxStyle.Critical, "Error:")
        End Try
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        Try
            Dim dlg As New SaveFileDialog
            dlg.DefaultExt = ".xflowloader"
            dlg.Filter = "XFlowLoader configuration files (*.xflowloader)|*.xflowloader"
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                If changed Then
                    If MsgBox("Saving the configuration will apply all changes made in this window. Continue?", MsgBoxStyle.OkCancel, "Save configuration:") <> MsgBoxResult.Ok Then Exit Sub
                    If changed Then changesApplied = True
                    ApplyChanges()
                End If
                Dim sd As StorageData = unit.Save(True)
                sd.SaveXML("XFlowLoader", dlg.FileName)
            End If
        Catch ex As Exception
            MsgBox("Failed to save configuration: " + ex.Message, MsgBoxStyle.Critical, "Error:")
        End Try
    End Sub

    Private Sub mappingList_MouseClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles mappingList.MouseClick
        Dim hi As ListViewHitTestInfo = mappingList.HitTest(e.Location)
        If hi.Item IsNot Nothing Then
            If hi.SubItem Is hi.Item.SubItems(1) Then
                compEditIndex = -1
                selectCompList.Items.Clear()
                selectCompList.Items.Add("Purge")
                selectCompList.Items.Add("Unused")
                For Each c As Compound In unit.comps
                    selectCompList.Items.Add(c.name)
                Next
                If Not String.IsNullOrEmpty(hi.SubItem.Text) Then
                    For i = 0 To selectCompList.Items.Count - 1
                        If selectCompList.Items(i).ToString = hi.SubItem.Text Then
                            selectCompList.SelectedIndex = i
                            Exit For
                        End If
                    Next
                    If (selectCompList.SelectedIndex < 0) Then
                        selectCompList.SelectedIndex = selectCompList.Items.Add(hi.SubItem.Text)
                    End If
                End If
                Dim p As New Point(hi.SubItem.Bounds.Left, hi.SubItem.Bounds.Top)
                p = selectCompList.Parent.PointToClient(mappingList.PointToScreen(p))
                selectCompList.Left = p.X
                selectCompList.Top = p.Y
                selectCompList.Width = hi.SubItem.Bounds.Width
                selectCompList.Height = hi.SubItem.Bounds.Height
                selectCompList.Visible = True
                selectCompList.DroppedDown = True
                compEditIndex = hi.Item.Index
            End If
        End If
    End Sub

    Private Sub selectCompList_DropDownClosed(sender As System.Object, e As System.EventArgs) Handles selectCompList.DropDownClosed
        selectCompList.Visible = False
    End Sub

    Private Sub mappingList_DrawSubItem(sender As System.Object, e As System.Windows.Forms.DrawListViewSubItemEventArgs) Handles mappingList.DrawSubItem
        e.DrawBackground()
        If (e.ColumnIndex = 1) Then
            Dim w As Integer = e.Bounds.Height + 1
            ComboBoxRenderer.DrawDropDownButton(e.Graphics, New Rectangle(e.Bounds.Right - w, e.Bounds.Top, w, w), VisualStyles.ComboBoxState.Normal)
            Dim r As New Rectangle(e.Bounds.Left + 5, e.Bounds.Top, e.Bounds.Width - w - 5, e.Bounds.Height)
            e.Graphics.DrawString(e.SubItem.Text, mappingList.Font, Brushes.Black, r, stringFormat)
        Else
            e.DrawText()
        End If
    End Sub

    Private Sub mappingList_DrawColumnHeader(sender As System.Object, e As System.Windows.Forms.DrawListViewColumnHeaderEventArgs) Handles mappingList.DrawColumnHeader
        e.DrawBackground()
        e.DrawText()
    End Sub

    Private Sub selectCompList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles selectCompList.SelectedIndexChanged
        If compEditIndex >= 0 Then If (selectCompList.SelectedItem IsNot Nothing) Then mappingList.Items(compEditIndex).SubItems(1).Text = selectCompList.SelectedItem
    End Sub

    Private Sub CopyGridToClipboardToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CopyGridToClipboardToolStripMenuItem.Click
        Dim flv As XFlowFileViewer = flowTab.Controls(0)
        flv.OnCopy()
    End Sub

    Private Sub Tabs_Selected(sender As System.Object, e As System.Windows.Forms.TabControlEventArgs) Handles Tabs.Selected
        CopyGridToClipboardToolStripMenuItem.Enabled = (Tabs.SelectedTab Is flowTab)
    End Sub

    Private Sub helpBtn_Click(sender As System.Object, e As System.EventArgs) Handles helpBtn.Click
        Help.ShowHelp(Me, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetAssembly(Me.GetType).Location), "FlowExchange.chm"), HelpNavigator.Topic, "XFlowLoader.htm")
    End Sub

    Friend ReadOnly Property ConfigurationChanged As Boolean
        Get
            'note that we also return the state as modified in case we should auto-load a named .xflow file
            Return changed Or changesApplied Or (unit.autoLoad.value And Not String.IsNullOrEmpty(unit.xFlowFileName.value))
        End Get
    End Property

End Class