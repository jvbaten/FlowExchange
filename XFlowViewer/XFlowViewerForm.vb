'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class XFlowViewerForm

    Dim fList As New List(Of FlowExchange.Flow)
    Dim titleList As New List(Of String)
    Dim fv As XFlowFileViewer

    Sub New()
        InitializeComponent()
        For Each s In My.Application.CommandLineArgs
            If s.ToLower.EndsWith(".xflow") Then
                Dim f As New Flow
                Try
                    Dim warn As New List(Of String)
                    f = XFlowFile.Read(s, warn)
                    fList.Add(f)
                    titleList.Add(System.IO.Path.GetFileNameWithoutExtension(s))
                Catch ex As Exception
                    MsgBox("Failed to load """ + s + """: " + ex.Message, MsgBoxStyle.Critical, "Error:")
                End Try
            End If
        Next
        CopyGridToClipboardToolStripMenuItem.Enabled = False
    End Sub

    Private Sub XFlowViewerForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.xflow
        If fList.Count > 0 Then
            fv = New XFlowFileViewer(fList.ToArray, titleList.ToArray)
            Panel.Controls.Add(fv)
            fv.Dock = DockStyle.Fill
            CopyGridToClipboardToolStripMenuItem.Enabled = True
        End If
    End Sub

    Private Sub Open(addToCurrent As Boolean)
        Dim dlg As New OpenFileDialog
        dlg.DefaultExt = ".xflow"
        dlg.Filter = "FileExchange files (*.xflow)|*.xflow"
        dlg.Multiselect = True
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim changed As Boolean = False
            Dim newFlist As New List(Of Flow)
            Dim newTitleList As New List(Of String)
            If addToCurrent Then
                For Each f As Flow In fList
                    newFlist.Add(f)
                Next
                For Each s As String In titleList
                    newTitleList.Add(s)
                Next
            End If
            For Each s As String In dlg.FileNames
                Dim f As New Flow
                Try
                    Dim warn As New List(Of String)
                    f = XFlowFile.Read(s, warn)
                    newFlist.Add(f)
                    newTitleList.Add(System.IO.Path.GetFileNameWithoutExtension(s))
                    changed = True
                Catch ex As Exception
                    MsgBox("Failed to load """ + s + """: " + ex.Message, MsgBoxStyle.Critical, "Error:")
                End Try
            Next
            If changed Then
                Panel.Controls.Clear()
                fList = newFlist
                titleList = newTitleList
                fv = New XFlowFileViewer(fList.ToArray, titleList.ToArray)
                Panel.Controls.Add(fv)
                fv.Dock = DockStyle.Fill
                CopyGridToClipboardToolStripMenuItem.Enabled = True
            End If
        End If
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        Open(False)
    End Sub

    Private Sub OpenAndAddToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles OpenAndAddToolStripMenuItem.Click
        Open(True)
    End Sub

    Private Sub CopyGridToClipboardToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles CopyGridToClipboardToolStripMenuItem.Click
        If fv IsNot Nothing Then fv.OnCopy()
    End Sub

End Class
