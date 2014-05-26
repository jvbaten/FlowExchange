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
Imports System.Reflection

Public Class XFlowSaverDialog

    Dim unit As XFlowSaver
    Dim changed As Boolean = False
    Dim changesApplied As Boolean = False

    Sub New(unit As XFlowSaver)
        InitializeComponent()
        Me.unit = unit
        CopyGridToClipboardToolStripMenuItem.Enabled = False
    End Sub

    Private Sub FillDialog()
        Dim pi As PropertyInstance
        If (unit.xFlowFileName.value IsNot Nothing) Then XPath.Text = unit.xFlowFileName.value
        saveNowBtn.Enabled = (unit.currentFlow IsNot Nothing)
        AutoSave.Checked = unit.autoSave.value
        Dim presentDict As New Dictionary(Of String, Boolean)
        Dim phasePresentDict As New Dictionary(Of String, Boolean)
        overallPropList.Items.Clear()
        overallAvailList.Items.Clear()
        phasePropList.Items.Clear()
        phaseAvailList.Items.Clear()
        For Each pi In unit.overallProperties
            overallPropList.Items.Add(pi)
            presentDict.Add(pi.ToString, True)
        Next
        For Each pi In unit.phaseProperties
            phasePropList.Items.Add(pi)
            phasePresentDict.Add(pi.ToString, True)
        Next
        Dim str As Stream = unit.feedPort.GetStream
        For Each p As PropertyInfo In PropertiesManager.GetProperties
            If str IsNot Nothing Then If Not str.CheckSupported(p) Then Continue For
            If p.allowForOverall Then
                pi = p.GetPropertyInstance
                If Not presentDict.ContainsKey(pi.ToString) Then
                    overallAvailList.Items.Add(pi)
                End If
            End If
            If p.allowForPhase Then
                pi = p.GetPropertyInstance
                If Not phasePresentDict.ContainsKey(pi.ToString) Then
                    phaseAvailList.Items.Add(pi)
                End If
            End If
        Next
        Dim flowList As New List(Of Flow)
        Dim flowTitleList As New List(Of String)
        If (unit.currentFlow IsNot Nothing) Then
            flowList.Add(unit.currentFlow)
            flowTitleList.Add("Current")
        End If
        If (unit.lastSavedFlow IsNot Nothing) Then
            flowList.Add(unit.lastSavedFlow)
            flowTitleList.Add("Last saved")
        End If
        If flowList.Count = 0 Then
            Try
                Tabs.TabPages.Remove(flowTab)
            Catch
            End Try
        Else
            Dim flv As New XFlowFileViewer(flowList.ToArray, flowTitleList.ToArray)
            flowTab.Controls.Add(flv)
            flv.Dock = DockStyle.Fill
        End If
        Dim ch As String = unit.xOptions.enthalpyReferencePhase
        If ch Is Nothing Then ch = "Automatic"
        referenceChoice.Items.Add(ch)
        referenceChoice.SelectedIndex = 0
        writeCreatorComment.Checked = unit.xOptions.writeCreatorComment
        writeComment.Checked = unit.xOptions.writeComments
        writeAutoFlash.Checked = unit.xOptions.writeAutoFlash
        writeEnthalpy.Checked = unit.xOptions.writeEnthalpy
        referenceP.Text = unit.xOptions.enthalpyReferenceP.ToString(XFlowFile.englishFormatProvider)
        referenceT.Text = unit.xOptions.enthalpyReferenceT.ToString(XFlowFile.englishFormatProvider)
        If advancedTab Is Tabs.SelectedTab Then advancedTab_Enter(Nothing, Nothing)
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub XFlowSaverDialog_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.flowexchange
        Me.Text = "Unit operation " + unit.ComponentName + ":"
        aboutText.Text += vbCrLf + GetVersionInfo(GetType(XFlowSaver).Assembly.Location).LegalCopyright.ToString
        aboutText.Text += vbCrLf + "Version: " + GetVersionInfo(GetType(XFlowSaver).Assembly.Location).FileVersion.ToString
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
        Dim dlg As New SaveFileDialog
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
        Dim refT, refP As Double
        refT = 298.15
        refP = 101325
        If referenceChoice.SelectedItem <> "Automatic" Then
            Try
                refT = Double.Parse(referenceT.Text)
            Catch ex As Exception
                advancedTab.Select()
                Throw New Exception("Invalid reference temperature """ + referenceT.Text + """: " + ex.Message)
            End Try
            Try
                advancedTab.Select()
                refP = Double.Parse(referenceP.Text)
            Catch ex As Exception
                Throw New Exception("Invalid reference pressure """ + referenceP.Text + """: " + ex.Message)
            End Try
        End If
        Dim path As String = XPath.Text
        If String.IsNullOrEmpty(path) Then
            If (unit.xFlowFileName.value IsNot Nothing) Then unit.xFlowFileName.value = Nothing
        Else
            If Not CAPEOPENBase.SameString(unit.xFlowFileName.value, path) Then unit.xFlowFileName.value = path
        End If
        If AutoSave.Checked <> unit.autoSave.value Then unit.autoSave.value = AutoSave.Checked
        unit.overallProperties.Clear()
        For Each pi As PropertyInstance In overallPropList.Items
            unit.overallProperties.Add(pi)
        Next
        unit.phaseProperties.Clear()
        For Each pi As PropertyInstance In phasePropList.Items
            unit.phaseProperties.Add(pi)
        Next
        If (unit.phaseProperties.Count > 0) Then
            unit.xOptions.writePhaseInfo = True
        Else
            unit.xOptions.writePhaseInfo = writePhaseInfo.Checked
        End If
        unit.xOptions.writeAutoFlash = writeAutoFlash.Checked
        unit.xOptions.writeComments = writeComment.Checked
        unit.xOptions.writeCreatorComment = writeCreatorComment.Checked
        unit.xOptions.writeEnthalpy = writeEnthalpy.Checked
        unit.xOptions.enthalpyReferencePhase = referenceChoice.SelectedItem
        If (unit.xOptions.enthalpyReferencePhase = "Automatic") Then
            unit.xOptions.enthalpyReferencePhase = Nothing
        Else
            Try
                unit.xOptions.enthalpyReferenceT = refT
            Catch ex As Exception
                Throw New Exception("Invalid reference temperature """ + referenceT.Text + """: " + ex.Message)
            End Try
            Try
                unit.xOptions.enthalpyReferenceP = refP
            Catch ex As Exception
                Throw New Exception("Invalid reference pressure """ + referenceP.Text + """: " + ex.Message)
            End Try
        End If
        changed = False
    End Sub

    Private Sub okBtn_Click(sender As System.Object, e As System.EventArgs) Handles okBtn.Click
        Dim oldChanged As Boolean = changed
        Try
            ApplyChanges()
        Catch ex As Exception
            changed = oldChanged 'save so that ConfigurationChanged returns the proper answer
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error:")
            Exit Sub
        End Try
        changed = oldChanged 'save so that ConfigurationChanged returns the proper answer
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub overallAddBtn_Click(sender As System.Object, e As System.EventArgs) Handles overallAddBtn.Click
        Dim pl As New List(Of PropertyInstance)
        overallPropList.ClearSelected()
        overallPropList.SuspendLayout()
        For Each it As PropertyInstance In overallAvailList.SelectedItems
            pl.Add(it)
            overallPropList.Items.Add(it)
            overallPropList.SelectedItems.Add(it)
            changed = True
        Next
        overallPropList.ResumeLayout()
        overallAvailList.SuspendLayout()
        For Each it As PropertyInstance In pl
            overallAvailList.Items.Remove(it)
        Next
        overallAvailList.ResumeLayout()
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub overallRemoveBtn_Click(sender As System.Object, e As System.EventArgs) Handles overallRemoveBtn.Click
        Dim pl As New List(Of PropertyInstance)
        overallAvailList.ClearSelected()
        overallAvailList.SuspendLayout()
        For Each it As PropertyInstance In overallPropList.SelectedItems
            pl.Add(it)
            overallAvailList.Items.Add(it)
            overallAvailList.SelectedItems.Add(it)
            changed = True
        Next
        overallAvailList.ResumeLayout()
        overallPropList.SuspendLayout()
        For Each it As PropertyInstance In pl
            overallPropList.Items.Remove(it)
        Next
        overallPropList.ResumeLayout()
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub overallUpBtn_Click(sender As System.Object, e As System.EventArgs) Handles overallUpBtn.Click
        If overallPropList.SelectedItems.Count > 0 Then
            Dim pi As PropertyInstance
            'mark selection
            For Each pi In overallPropList.Items
                pi.selected = False
            Next
            For Each pi In overallPropList.SelectedItems
                pi.selected = True
            Next
            overallPropList.SuspendLayout()
            'loop down until first free slot
            Dim i As Integer = 0
            Do While i < overallPropList.Items.Count
                pi = overallPropList.Items(i)
                If Not pi.selected Then Exit Do
                i += 1
            Loop
            Do While i < overallPropList.Items.Count
                pi = overallPropList.Items(i)
                If pi.selected Then
                    'exchange with previous one
                    overallPropList.Items.Remove(pi)
                    overallPropList.Items.Insert(i - 1, pi)
                    overallPropList.SelectedItems.Add(pi)
                    changed = True
                End If
                i += 1
            Loop
            overallPropList.ResumeLayout()
            ButtonUpdateTimer.Enabled = True
        End If
    End Sub

    Private Sub overallDownBtn_Click(sender As System.Object, e As System.EventArgs) Handles overallDownBtn.Click
        If overallPropList.SelectedItems.Count > 0 Then
            Dim pi As PropertyInstance
            'mark selection
            For Each pi In overallPropList.Items
                pi.selected = False
            Next
            For Each pi In overallPropList.SelectedItems
                pi.selected = True
            Next
            overallPropList.SuspendLayout()
            'loop up until first free slot
            Dim i As Integer = overallPropList.Items.Count - 1
            Do While i >= 0
                pi = overallPropList.Items(i)
                If Not pi.selected Then Exit Do
                i -= 1
            Loop
            Do While i >= 0
                pi = overallPropList.Items(i)
                If pi.selected Then
                    'exchange with previous one
                    overallPropList.Items.Remove(pi)
                    overallPropList.Items.Insert(i + 1, pi)
                    overallPropList.SelectedItems.Add(pi)
                    changed = True
                End If
                i -= 1
            Loop
            overallPropList.ResumeLayout()
            ButtonUpdateTimer.Enabled = True
        End If
    End Sub

    Private Sub ButtonUpdateTimer_Tick(sender As System.Object, e As System.EventArgs) Handles ButtonUpdateTimer.Tick
        ButtonUpdateTimer.Enabled = False
        'update the buttons
        Dim i As Integer
        overallAddBtn.Enabled = (overallAvailList.SelectedIndices.Count > 0)
        overallRemoveBtn.Enabled = (overallPropList.SelectedIndices.Count > 0)
        If (overallPropList.SelectedIndices.Count > 0) Then
            Dim sel(overallPropList.SelectedIndices.Count - 1) As Integer
            overallPropList.SelectedIndices.CopyTo(sel, 0)
            Array.Sort(sel)
            Dim test As Boolean
            test = False
            For i = sel.Length - 1 To 0 Step -1
                If (sel(i) < overallPropList.Items.Count - sel.Length + i) Then
                    test = True
                    Exit For
                End If
            Next
            overallDownBtn.Enabled = test
            test = False
            For i = 0 To sel.Length - 1
                If (sel(i) > i) Then
                    test = True
                    Exit For
                End If
            Next
            overallUpBtn.Enabled = test
        Else
            overallUpBtn.Enabled = False
            overallDownBtn.Enabled = False
        End If
        phaseAddBtn.Enabled = (phaseAvailList.SelectedIndices.Count > 0)
        phaseRemoveBtn.Enabled = (phasePropList.SelectedIndices.Count > 0)
        If (phasePropList.SelectedIndices.Count > 0) Then
            Dim sel(phasePropList.SelectedIndices.Count - 1) As Integer
            phasePropList.SelectedIndices.CopyTo(sel, 0)
            Array.Sort(sel)
            Dim test As Boolean
            test = False
            For i = sel.Length - 1 To 0 Step -1
                If (sel(i) < phasePropList.Items.Count - sel.Length + i) Then
                    test = True
                    Exit For
                End If
            Next
            phaseDownBtn.Enabled = test
            test = False
            For i = 0 To sel.Length - 1
                If (sel(i) > i) Then
                    test = True
                    Exit For
                End If
            Next
            phaseUpBtn.Enabled = test
        Else
            phaseUpBtn.Enabled = False
            phaseDownBtn.Enabled = False
        End If
    End Sub

    Private Sub overallPropList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles overallPropList.SelectedIndexChanged
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub overallAvailList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles overallAvailList.SelectedIndexChanged
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub saveNowBtn_Click(sender As System.Object, e As System.EventArgs) Handles saveNowBtn.Click
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
        Dim dlg As New SaveFileDialog
        dlg.DefaultExt = ".xflow"
        If path IsNot Nothing Then
            dlg.FileName = path
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(path)
        End If
        dlg.Filter = "FileExchange files (*.xflow)|*.xflow"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try
                Dim options As New XFlowFileOptions
                options.writePhaseInfo = writePhaseInfo.Checked
                options.writeAutoFlash = writeAutoFlash.Checked
                options.writeComments = writeComment.Checked
                options.writeCreatorComment = writeCreatorComment.Checked
                options.writeEnthalpy = writeEnthalpy.Checked
                options.enthalpyReferencePhase = referenceChoice.SelectedText
                If (options.enthalpyReferencePhase = "Automatic") Then
                    options.enthalpyReferencePhase = Nothing
                Else
                    Try
                        options.enthalpyReferenceT = Double.Parse(referenceT.Text)
                    Catch ex As Exception
                        Throw New Exception("Invalid reference temperature """ + referenceT.Text + """: " + ex.Message)
                    End Try
                    Try
                        options.enthalpyReferenceP = Double.Parse(referenceP.Text)
                    Catch ex As Exception
                        Throw New Exception("Invalid reference pressure """ + referenceP.Text + """: " + ex.Message)
                    End Try
                End If
                XFlowFile.Write(dlg.FileName, unit.currentFlow, options)
                unit.lastSavedFlow = unit.currentFlow.Clone
            Catch ex As Exception
                MsgBox("Failed to store XFlow file: " + ex.Message, MsgBoxStyle.Critical, "Save XFlow File Now:")
            End Try
        End If
    End Sub

    Private Sub phasePropList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles phasePropList.SelectedIndexChanged
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub phaseAvailList_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles phaseAvailList.SelectedIndexChanged
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub phaseAddBtn_Click(sender As System.Object, e As System.EventArgs) Handles phaseAddBtn.Click
        Dim pl As New List(Of PropertyInstance)
        phasePropList.ClearSelected()
        phasePropList.SuspendLayout()
        For Each it As PropertyInstance In phaseAvailList.SelectedItems
            pl.Add(it)
            phasePropList.Items.Add(it)
            phasePropList.SelectedItems.Add(it)
            changed = True
        Next
        phasePropList.ResumeLayout()
        phaseAvailList.SuspendLayout()
        For Each it As PropertyInstance In pl
            phaseAvailList.Items.Remove(it)
        Next
        phaseAvailList.ResumeLayout()
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub phaseRemoveBtn_Click(sender As System.Object, e As System.EventArgs) Handles phaseRemoveBtn.Click
        Dim pl As New List(Of PropertyInstance)
        phaseAvailList.ClearSelected()
        phaseAvailList.SuspendLayout()
        For Each it As PropertyInstance In phasePropList.SelectedItems
            pl.Add(it)
            phaseAvailList.Items.Add(it)
            phaseAvailList.SelectedItems.Add(it)
            changed = True
        Next
        phaseAvailList.ResumeLayout()
        phasePropList.SuspendLayout()
        For Each it As PropertyInstance In pl
            phasePropList.Items.Remove(it)
        Next
        phasePropList.ResumeLayout()
        ButtonUpdateTimer.Enabled = True
    End Sub

    Private Sub phaseUpBtn_Click(sender As System.Object, e As System.EventArgs) Handles phaseUpBtn.Click
        If phasePropList.SelectedItems.Count > 0 Then
            Dim pi As PropertyInstance
            'mark selection
            For Each pi In phasePropList.Items
                pi.selected = False
            Next
            For Each pi In phasePropList.SelectedItems
                pi.selected = True
            Next
            phasePropList.SuspendLayout()
            'loop down until first free slot
            Dim i As Integer = 0
            Do While i < phasePropList.Items.Count
                pi = phasePropList.Items(i)
                If Not pi.selected Then Exit Do
                i += 1
            Loop
            Do While i < phasePropList.Items.Count
                pi = phasePropList.Items(i)
                If pi.selected Then
                    'exchange with previous one
                    phasePropList.Items.Remove(pi)
                    phasePropList.Items.Insert(i - 1, pi)
                    phasePropList.SelectedItems.Add(pi)
                    changed = True
                End If
                i += 1
            Loop
            phasePropList.ResumeLayout()
            ButtonUpdateTimer.Enabled = True
        End If
    End Sub

    Private Sub phaseDownBtn_Click(sender As System.Object, e As System.EventArgs) Handles phaseDownBtn.Click
        If phasePropList.SelectedItems.Count > 0 Then
            Dim pi As PropertyInstance
            'mark selection
            For Each pi In phasePropList.Items
                pi.selected = False
            Next
            For Each pi In phasePropList.SelectedItems
                pi.selected = True
            Next
            phasePropList.SuspendLayout()
            'loop up until first free slot
            Dim i As Integer = phasePropList.Items.Count - 1
            Do While i >= 0
                pi = phasePropList.Items(i)
                If Not pi.selected Then Exit Do
                i -= 1
            Loop
            Do While i >= 0
                pi = phasePropList.Items(i)
                If pi.selected Then
                    'exchange with previous one
                    phasePropList.Items.Remove(pi)
                    phasePropList.Items.Insert(i + 1, pi)
                    phasePropList.SelectedItems.Add(pi)
                    changed = True
                End If
                i -= 1
            Loop
            phasePropList.ResumeLayout()
            ButtonUpdateTimer.Enabled = True
        End If
    End Sub

    Private Sub overallAvailList_DoubleClick(sender As System.Object, e As System.EventArgs) Handles overallAvailList.DoubleClick
        overallAddBtn_Click(Nothing, Nothing)
    End Sub

    Private Sub phaseAvailList_DoubleClick(sender As System.Object, e As System.EventArgs) Handles phaseAvailList.DoubleClick
        phaseAddBtn_Click(Nothing, Nothing)
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LoadToolStripMenuItem.Click
        Try
            Dim dlg As New OpenFileDialog
            dlg.DefaultExt = ".xflowsaver"
            dlg.Filter = "XFlowSaver configuration files (*.xflowsaver)|*.xflowsaver"
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                If changed Then If MsgBox("Loading the configuration will change the configuration even if pressing Cancel. Continue?", MsgBoxStyle.OkCancel, "Load configuration:") <> MsgBoxResult.Ok Then Exit Sub
                Dim sd As New StorageData
                sd.LoadXML("XFlowSaver", dlg.FileName)
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
            dlg.DefaultExt = ".xflowsaver"
            dlg.Filter = "XFlowSaver configuration files (*.xflowsaver)|*.xflowsaver"
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                If changed Then
                    If MsgBox("Saving the configuration will apply all changes made in this window. Continue?", MsgBoxStyle.OkCancel, "Save configuration:") <> MsgBoxResult.Ok Then Exit Sub
                    If changed Then changesApplied = True
                    ApplyChanges()
                End If
                Dim sd As StorageData = unit.Save(True)
                sd.SaveXML("XFlowSaver", dlg.FileName)
            End If
        Catch ex As Exception
            MsgBox("Failed to save configuration: " + ex.Message, MsgBoxStyle.Critical, "Error:")
        End Try
    End Sub

    Private Sub referenceChoice_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles referenceChoice.SelectedIndexChanged
        FixEnthalpyOptions()
    End Sub

    Private Sub writeEnthalpy_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles writeEnthalpy.CheckedChanged
        FixEnthalpyOptions()
    End Sub

    Private Sub advancedTab_Enter(sender As System.Object, e As System.EventArgs) Handles advancedTab.Enter
        If (unit.phaseProperties.Count) Then
            writePhaseInfo.Checked = True
            writePhaseInfo.Enabled = False
        Else
            writePhaseInfo.Checked = unit.xOptions.writePhaseInfo
            writePhaseInfo.Enabled = True
        End If
        Dim currentChoice As String = referenceChoice.SelectedItem
        Dim str As Stream = unit.feedPort.GetStream
        referenceChoice.Items.Clear()
        referenceChoice.Items.Add("Automatic")
        If str IsNot Nothing Then
            Try
                For Each s As String In str.PhaseSelectionOptions
                    referenceChoice.Items.Add(s)
                Next
            Catch
            End Try
        End If
        If Not referenceChoice.Items.Contains(currentChoice) Then referenceChoice.Items.Add(currentChoice)
        referenceChoice.SelectedItem = currentChoice
        FixEnthalpyOptions()
    End Sub

    Sub FixEnthalpyOptions()
        phaseLabel.Enabled = writeEnthalpy.Checked
        referenceChoice.Enabled = writeEnthalpy.Checked
        Dim able As Boolean = writeEnthalpy.Checked And referenceChoice.SelectedIndex > 0
        tLabel.Enabled = able
        pLabel.Enabled = able
        referenceT.Enabled = able
        referenceP.Enabled = able
    End Sub

    Private Sub CopyGridToClipboardToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CopyGridToClipboardToolStripMenuItem.Click
        Dim flv As XFlowFileViewer = flowTab.Controls(0)
        flv.OnCopy()
    End Sub

    Private Sub Tabs_Selected(sender As System.Object, e As System.Windows.Forms.TabControlEventArgs) Handles Tabs.Selected
        CopyGridToClipboardToolStripMenuItem.Enabled = (Tabs.SelectedTab Is flowTab)
    End Sub

    Private Sub helpBtn_Click(sender As System.Object, e As System.EventArgs) Handles helpBtn.Click
        Help.ShowHelp(Me, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetAssembly(Me.GetType).Location), "FlowExchange.chm"), HelpNavigator.Topic, "XFlowSaver.htm")
    End Sub

    Friend ReadOnly Property ConfigurationChanged As Boolean
        Get
            Return changed Or changesApplied
        End Get
    End Property

End Class