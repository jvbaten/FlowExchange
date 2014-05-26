<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XFlowLoaderDialog
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(XFlowLoaderDialog))
        Me.cancelBtn = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.Tabs = New System.Windows.Forms.TabControl()
        Me.settingsTab = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.flashSpec = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.selectCompList = New System.Windows.Forms.ComboBox()
        Me.mappingList = New System.Windows.Forms.ListView()
        Me.fromCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.toCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.loadNowBtn = New System.Windows.Forms.Button()
        Me.ResetBtn = New System.Windows.Forms.Button()
        Me.AutoLoad = New System.Windows.Forms.CheckBox()
        Me.browseXPath = New System.Windows.Forms.Button()
        Me.XPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.flowTab = New System.Windows.Forms.TabPage()
        Me.About = New System.Windows.Forms.TabPage()
        Me.aboutText = New System.Windows.Forms.TextBox()
        Me.okBtn = New System.Windows.Forms.Button()
        Me.mainMenu = New System.Windows.Forms.MenuStrip()
        Me.ConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyGridToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.helpBtn = New System.Windows.Forms.Button()
        Me.Tabs.SuspendLayout()
        Me.settingsTab.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.About.SuspendLayout()
        Me.mainMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'cancelBtn
        '
        Me.cancelBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cancelBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cancelBtn.ImageIndex = 2
        Me.cancelBtn.ImageList = Me.ImageList1
        Me.cancelBtn.Location = New System.Drawing.Point(603, 539)
        Me.cancelBtn.Name = "cancelBtn"
        Me.cancelBtn.Size = New System.Drawing.Size(101, 23)
        Me.cancelBtn.TabIndex = 3
        Me.cancelBtn.Text = "&Cancel"
        Me.cancelBtn.UseVisualStyleBackColor = True
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Fuchsia
        Me.ImageList1.Images.SetKeyName(0, "OpenFolder.bmp")
        Me.ImageList1.Images.SetKeyName(1, "Delete.bmp")
        Me.ImageList1.Images.SetKeyName(2, "Close.bmp")
        Me.ImageList1.Images.SetKeyName(3, "ok.png")
        Me.ImageList1.Images.SetKeyName(4, "down.bmp")
        Me.ImageList1.Images.SetKeyName(5, "up.bmp")
        Me.ImageList1.Images.SetKeyName(6, "add.bmp")
        Me.ImageList1.Images.SetKeyName(7, "Help.bmp")
        '
        'Tabs
        '
        Me.Tabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tabs.Controls.Add(Me.settingsTab)
        Me.Tabs.Controls.Add(Me.flowTab)
        Me.Tabs.Controls.Add(Me.About)
        Me.Tabs.Location = New System.Drawing.Point(13, 27)
        Me.Tabs.Name = "Tabs"
        Me.Tabs.SelectedIndex = 0
        Me.Tabs.Size = New System.Drawing.Size(691, 506)
        Me.Tabs.TabIndex = 1
        '
        'settingsTab
        '
        Me.settingsTab.Controls.Add(Me.GroupBox3)
        Me.settingsTab.Controls.Add(Me.GroupBox2)
        Me.settingsTab.Controls.Add(Me.GroupBox1)
        Me.settingsTab.Location = New System.Drawing.Point(4, 22)
        Me.settingsTab.Name = "settingsTab"
        Me.settingsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.settingsTab.Size = New System.Drawing.Size(683, 480)
        Me.settingsTab.TabIndex = 0
        Me.settingsTab.Text = "Settings"
        Me.settingsTab.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.Controls.Add(Me.flashSpec)
        Me.GroupBox3.Controls.Add(Me.Label2)
        Me.GroupBox3.Location = New System.Drawing.Point(7, 99)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(670, 50)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Product specification:"
        '
        'flashSpec
        '
        Me.flashSpec.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.flashSpec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.flashSpec.FormattingEnabled = True
        Me.flashSpec.Location = New System.Drawing.Point(45, 18)
        Me.flashSpec.Name = "flashSpec"
        Me.flashSpec.Size = New System.Drawing.Size(619, 21)
        Me.flashSpec.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Use:"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.selectCompList)
        Me.GroupBox2.Controls.Add(Me.mappingList)
        Me.GroupBox2.Location = New System.Drawing.Point(7, 155)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(670, 317)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Compound mapping:"
        '
        'selectCompList
        '
        Me.selectCompList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.selectCompList.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.selectCompList.FormattingEnabled = True
        Me.selectCompList.Location = New System.Drawing.Point(272, 102)
        Me.selectCompList.Name = "selectCompList"
        Me.selectCompList.Size = New System.Drawing.Size(121, 21)
        Me.selectCompList.TabIndex = 1
        Me.selectCompList.Visible = False
        '
        'mappingList
        '
        Me.mappingList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.mappingList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.fromCol, Me.toCol})
        Me.mappingList.FullRowSelect = True
        Me.mappingList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.mappingList.Location = New System.Drawing.Point(9, 20)
        Me.mappingList.Name = "mappingList"
        Me.mappingList.OwnerDraw = True
        Me.mappingList.Size = New System.Drawing.Size(655, 291)
        Me.mappingList.TabIndex = 0
        Me.mappingList.UseCompatibleStateImageBehavior = False
        Me.mappingList.View = System.Windows.Forms.View.Details
        '
        'fromCol
        '
        Me.fromCol.Text = "Compound"
        Me.fromCol.Width = 258
        '
        'toCol
        '
        Me.toCol.Text = "Maps to"
        Me.toCol.Width = 261
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.loadNowBtn)
        Me.GroupBox1.Controls.Add(Me.ResetBtn)
        Me.GroupBox1.Controls.Add(Me.AutoLoad)
        Me.GroupBox1.Controls.Add(Me.browseXPath)
        Me.GroupBox1.Controls.Add(Me.XPath)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 7)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(670, 85)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Location of .xflow file:"
        '
        'loadNowBtn
        '
        Me.loadNowBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.loadNowBtn.ImageIndex = 0
        Me.loadNowBtn.ImageList = Me.ImageList1
        Me.loadNowBtn.Location = New System.Drawing.Point(579, 49)
        Me.loadNowBtn.Name = "loadNowBtn"
        Me.loadNowBtn.Size = New System.Drawing.Size(85, 23)
        Me.loadNowBtn.TabIndex = 5
        Me.loadNowBtn.Text = "Load Now"
        Me.loadNowBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.loadNowBtn.UseVisualStyleBackColor = True
        '
        'ResetBtn
        '
        Me.ResetBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ResetBtn.ImageIndex = 1
        Me.ResetBtn.ImageList = Me.ImageList1
        Me.ResetBtn.Location = New System.Drawing.Point(493, 25)
        Me.ResetBtn.Name = "ResetBtn"
        Me.ResetBtn.Size = New System.Drawing.Size(85, 23)
        Me.ResetBtn.TabIndex = 2
        Me.ResetBtn.Text = "Reset"
        Me.ResetBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ResetBtn.UseVisualStyleBackColor = True
        '
        'AutoLoad
        '
        Me.AutoLoad.AutoSize = True
        Me.AutoLoad.Location = New System.Drawing.Point(45, 54)
        Me.AutoLoad.Name = "AutoLoad"
        Me.AutoLoad.Size = New System.Drawing.Size(250, 17)
        Me.AutoLoad.TabIndex = 4
        Me.AutoLoad.Text = "Automatically load .xflow file at each calculation"
        Me.AutoLoad.UseVisualStyleBackColor = True
        '
        'browseXPath
        '
        Me.browseXPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.browseXPath.ImageIndex = 0
        Me.browseXPath.ImageList = Me.ImageList1
        Me.browseXPath.Location = New System.Drawing.Point(579, 25)
        Me.browseXPath.Name = "browseXPath"
        Me.browseXPath.Size = New System.Drawing.Size(85, 23)
        Me.browseXPath.TabIndex = 3
        Me.browseXPath.Text = "Browse"
        Me.browseXPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.browseXPath.UseVisualStyleBackColor = True
        '
        'XPath
        '
        Me.XPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.XPath.Location = New System.Drawing.Point(45, 27)
        Me.XPath.Name = "XPath"
        Me.XPath.ReadOnly = True
        Me.XPath.Size = New System.Drawing.Size(442, 20)
        Me.XPath.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 27)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Path:"
        '
        'flowTab
        '
        Me.flowTab.Location = New System.Drawing.Point(4, 22)
        Me.flowTab.Name = "flowTab"
        Me.flowTab.Padding = New System.Windows.Forms.Padding(3)
        Me.flowTab.Size = New System.Drawing.Size(683, 480)
        Me.flowTab.TabIndex = 1
        Me.flowTab.Text = "Flow data"
        Me.flowTab.UseVisualStyleBackColor = True
        '
        'About
        '
        Me.About.BackColor = System.Drawing.Color.White
        Me.About.BackgroundImage = Global.FlowExchange.My.Resources.Resources.logo
        Me.About.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.About.Controls.Add(Me.aboutText)
        Me.About.Location = New System.Drawing.Point(4, 22)
        Me.About.Name = "About"
        Me.About.Size = New System.Drawing.Size(683, 480)
        Me.About.TabIndex = 2
        Me.About.Text = "About"
        '
        'aboutText
        '
        Me.aboutText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.aboutText.BackColor = System.Drawing.Color.White
        Me.aboutText.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.aboutText.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.aboutText.Location = New System.Drawing.Point(13, 139)
        Me.aboutText.Multiline = True
        Me.aboutText.Name = "aboutText"
        Me.aboutText.ReadOnly = True
        Me.aboutText.Size = New System.Drawing.Size(647, 264)
        Me.aboutText.TabIndex = 0
        Me.aboutText.Text = "XFlowLoader CAPE-OPEN Unit Operation"
        '
        'okBtn
        '
        Me.okBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.okBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.okBtn.ImageIndex = 3
        Me.okBtn.ImageList = Me.ImageList1
        Me.okBtn.Location = New System.Drawing.Point(501, 539)
        Me.okBtn.Name = "okBtn"
        Me.okBtn.Size = New System.Drawing.Size(101, 23)
        Me.okBtn.TabIndex = 2
        Me.okBtn.Text = "&Ok"
        Me.okBtn.UseVisualStyleBackColor = True
        '
        'mainMenu
        '
        Me.mainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConfigurationToolStripMenuItem, Me.EditToolStripMenuItem})
        Me.mainMenu.Location = New System.Drawing.Point(0, 0)
        Me.mainMenu.Name = "mainMenu"
        Me.mainMenu.Size = New System.Drawing.Size(716, 24)
        Me.mainMenu.TabIndex = 0
        Me.mainMenu.Text = "MenuStrip1"
        '
        'ConfigurationToolStripMenuItem
        '
        Me.ConfigurationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadToolStripMenuItem, Me.SaveToolStripMenuItem})
        Me.ConfigurationToolStripMenuItem.Name = "ConfigurationToolStripMenuItem"
        Me.ConfigurationToolStripMenuItem.Size = New System.Drawing.Size(93, 20)
        Me.ConfigurationToolStripMenuItem.Text = "&Configuration"
        '
        'LoadToolStripMenuItem
        '
        Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
        Me.LoadToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.LoadToolStripMenuItem.Text = "Load..."
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.SaveToolStripMenuItem.Text = "Save..."
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
        'helpBtn
        '
        Me.helpBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.helpBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.helpBtn.ImageIndex = 7
        Me.helpBtn.ImageList = Me.ImageList1
        Me.helpBtn.Location = New System.Drawing.Point(13, 539)
        Me.helpBtn.Name = "helpBtn"
        Me.helpBtn.Size = New System.Drawing.Size(101, 23)
        Me.helpBtn.TabIndex = 2
        Me.helpBtn.Text = "&Help"
        Me.helpBtn.UseVisualStyleBackColor = True
        '
        'XFlowLoaderDialog
        '
        Me.AcceptButton = Me.okBtn
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cancelBtn
        Me.ClientSize = New System.Drawing.Size(716, 574)
        Me.Controls.Add(Me.helpBtn)
        Me.Controls.Add(Me.okBtn)
        Me.Controls.Add(Me.Tabs)
        Me.Controls.Add(Me.cancelBtn)
        Me.Controls.Add(Me.mainMenu)
        Me.MainMenuStrip = Me.mainMenu
        Me.MinimizeBox = False
        Me.Name = "XFlowLoaderDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "XFlowLoaderDialog"
        Me.Tabs.ResumeLayout(False)
        Me.settingsTab.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.About.ResumeLayout(False)
        Me.About.PerformLayout()
        Me.mainMenu.ResumeLayout(False)
        Me.mainMenu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cancelBtn As System.Windows.Forms.Button
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents Tabs As System.Windows.Forms.TabControl
    Friend WithEvents settingsTab As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents AutoLoad As System.Windows.Forms.CheckBox
    Friend WithEvents browseXPath As System.Windows.Forms.Button
    Friend WithEvents XPath As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents flowTab As System.Windows.Forms.TabPage
    Friend WithEvents ResetBtn As System.Windows.Forms.Button
    Friend WithEvents okBtn As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents loadNowBtn As System.Windows.Forms.Button
    Friend WithEvents About As System.Windows.Forms.TabPage
    Friend WithEvents aboutText As System.Windows.Forms.TextBox
    Friend WithEvents mainMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents ConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents flashSpec As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents mappingList As System.Windows.Forms.ListView
    Friend WithEvents fromCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents toCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents selectCompList As System.Windows.Forms.ComboBox
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyGridToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents helpBtn As System.Windows.Forms.Button
End Class
