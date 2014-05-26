<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XFlowViewerForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.XFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenAndAddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel = New System.Windows.Forms.Panel()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyGridToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XFileToolStripMenuItem, Me.EditToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(452, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "mainMenuStrip"
        '
        'XFileToolStripMenuItem
        '
        Me.XFileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.OpenAndAddToolStripMenuItem})
        Me.XFileToolStripMenuItem.Name = "XFileToolStripMenuItem"
        Me.XFileToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.XFileToolStripMenuItem.Text = "XFile"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.OpenToolStripMenuItem.Text = "&Open..."
        '
        'OpenAndAddToolStripMenuItem
        '
        Me.OpenAndAddToolStripMenuItem.Name = "OpenAndAddToolStripMenuItem"
        Me.OpenAndAddToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.OpenAndAddToolStripMenuItem.Text = "&Open and add..."
        '
        'Panel
        '
        Me.Panel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel.Location = New System.Drawing.Point(0, 27)
        Me.Panel.Name = "Panel"
        Me.Panel.Size = New System.Drawing.Size(452, 430)
        Me.Panel.TabIndex = 1
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyGridToClipboardToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "&Edit"
        '
        'CopyGridToClipboardToolStripMenuItem
        '
        Me.CopyGridToClipboardToolStripMenuItem.Name = "CopyGridToClipboardToolStripMenuItem"
        Me.CopyGridToClipboardToolStripMenuItem.Size = New System.Drawing.Size(193, 22)
        Me.CopyGridToClipboardToolStripMenuItem.Text = "Copy grid to clipboard"
        '
        'XFlowViewerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(452, 457)
        Me.Controls.Add(Me.Panel)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "XFlowViewerForm"
        Me.Text = "XFlow File Viewer:"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents XFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenAndAddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel As System.Windows.Forms.Panel
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyGridToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
