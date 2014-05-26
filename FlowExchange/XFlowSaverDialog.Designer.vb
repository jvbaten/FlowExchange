<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XFlowSaverDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(XFlowSaverDialog))
        Me.cancelBtn = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.Tabs = New System.Windows.Forms.TabControl()
        Me.settingsTab = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.overallAvailList = New System.Windows.Forms.ListBox()
        Me.overallRemoveBtn = New System.Windows.Forms.Button()
        Me.overallAddBtn = New System.Windows.Forms.Button()
        Me.overallDownBtn = New System.Windows.Forms.Button()
        Me.overallUpBtn = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.overallPropList = New System.Windows.Forms.ListBox()
        Me.phaseAvailList = New System.Windows.Forms.ListBox()
        Me.phaseRemoveBtn = New System.Windows.Forms.Button()
        Me.phaseAddBtn = New System.Windows.Forms.Button()
        Me.phaseDownBtn = New System.Windows.Forms.Button()
        Me.phaseUpBtn = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.phasePropList = New System.Windows.Forms.ListBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.saveNowBtn = New System.Windows.Forms.Button()
        Me.ResetBtn = New System.Windows.Forms.Button()
        Me.AutoSave = New System.Windows.Forms.CheckBox()
        Me.browseXPath = New System.Windows.Forms.Button()
        Me.XPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.advancedTab = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.writePhaseInfo = New System.Windows.Forms.CheckBox()
        Me.writeEnthalpy = New System.Windows.Forms.CheckBox()
        Me.writeAutoFlash = New System.Windows.Forms.CheckBox()
        Me.writeComment = New System.Windows.Forms.CheckBox()
        Me.writeCreatorComment = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.referenceP = New System.Windows.Forms.TextBox()
        Me.referenceT = New System.Windows.Forms.TextBox()
        Me.referenceChoice = New System.Windows.Forms.ComboBox()
        Me.pLabel = New System.Windows.Forms.Label()
        Me.tLabel = New System.Windows.Forms.Label()
        Me.phaseLabel = New System.Windows.Forms.Label()
        Me.flowTab = New System.Windows.Forms.TabPage()
        Me.About = New System.Windows.Forms.TabPage()
        Me.aboutText = New System.Windows.Forms.TextBox()
        Me.okBtn = New System.Windows.Forms.Button()
        Me.ButtonUpdateTimer = New System.Windows.Forms.Timer(Me.components)
        Me.mainMenu = New System.Windows.Forms.MenuStrip()
        Me.ConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyGridToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.helpBtn = New System.Windows.Forms.Button()
        Me.Tabs.SuspendLayout()
        Me.settingsTab.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.advancedTab.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
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
        Me.cancelBtn.Location = New System.Drawing.Point(594, 521)
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
        Me.ImageList1.Images.SetKeyName(7, "Save.bmp")
        Me.ImageList1.Images.SetKeyName(8, "Help.bmp")
        '
        'Tabs
        '
        Me.Tabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tabs.Controls.Add(Me.settingsTab)
        Me.Tabs.Controls.Add(Me.advancedTab)
        Me.Tabs.Controls.Add(Me.flowTab)
        Me.Tabs.Controls.Add(Me.About)
        Me.Tabs.Location = New System.Drawing.Point(13, 27)
        Me.Tabs.Name = "Tabs"
        Me.Tabs.SelectedIndex = 0
        Me.Tabs.Size = New System.Drawing.Size(682, 488)
        Me.Tabs.TabIndex = 1
        '
        'settingsTab
        '
        Me.settingsTab.Controls.Add(Me.GroupBox2)
        Me.settingsTab.Controls.Add(Me.GroupBox1)
        Me.settingsTab.Location = New System.Drawing.Point(4, 22)
        Me.settingsTab.Name = "settingsTab"
        Me.settingsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.settingsTab.Size = New System.Drawing.Size(674, 462)
        Me.settingsTab.TabIndex = 0
        Me.settingsTab.Text = "Settings"
        Me.settingsTab.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.SplitContainer1)
        Me.GroupBox2.Location = New System.Drawing.Point(7, 99)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(661, 355)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Additional properties in .xflow file:"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 16)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.overallAvailList)
        Me.SplitContainer1.Panel1.Controls.Add(Me.overallRemoveBtn)
        Me.SplitContainer1.Panel1.Controls.Add(Me.overallAddBtn)
        Me.SplitContainer1.Panel1.Controls.Add(Me.overallDownBtn)
        Me.SplitContainer1.Panel1.Controls.Add(Me.overallUpBtn)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.overallPropList)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.phaseAvailList)
        Me.SplitContainer1.Panel2.Controls.Add(Me.phaseRemoveBtn)
        Me.SplitContainer1.Panel2.Controls.Add(Me.phaseAddBtn)
        Me.SplitContainer1.Panel2.Controls.Add(Me.phaseDownBtn)
        Me.SplitContainer1.Panel2.Controls.Add(Me.phaseUpBtn)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel2.Controls.Add(Me.phasePropList)
        Me.SplitContainer1.Size = New System.Drawing.Size(655, 336)
        Me.SplitContainer1.SplitterDistance = 315
        Me.SplitContainer1.TabIndex = 0
        '
        'overallAvailList
        '
        Me.overallAvailList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.overallAvailList.FormattingEnabled = True
        Me.overallAvailList.Location = New System.Drawing.Point(6, 241)
        Me.overallAvailList.Name = "overallAvailList"
        Me.overallAvailList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.overallAvailList.Size = New System.Drawing.Size(306, 95)
        Me.overallAvailList.Sorted = True
        Me.overallAvailList.TabIndex = 6
        '
        'overallRemoveBtn
        '
        Me.overallRemoveBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.overallRemoveBtn.Enabled = False
        Me.overallRemoveBtn.ImageIndex = 1
        Me.overallRemoveBtn.ImageList = Me.ImageList1
        Me.overallRemoveBtn.Location = New System.Drawing.Point(7, 206)
        Me.overallRemoveBtn.Name = "overallRemoveBtn"
        Me.overallRemoveBtn.Size = New System.Drawing.Size(75, 23)
        Me.overallRemoveBtn.TabIndex = 3
        Me.overallRemoveBtn.Text = "Remove"
        Me.overallRemoveBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.overallRemoveBtn.UseVisualStyleBackColor = True
        '
        'overallAddBtn
        '
        Me.overallAddBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.overallAddBtn.Enabled = False
        Me.overallAddBtn.ImageIndex = 6
        Me.overallAddBtn.ImageList = Me.ImageList1
        Me.overallAddBtn.Location = New System.Drawing.Point(6, 179)
        Me.overallAddBtn.Name = "overallAddBtn"
        Me.overallAddBtn.Size = New System.Drawing.Size(76, 23)
        Me.overallAddBtn.TabIndex = 2
        Me.overallAddBtn.Text = "Add"
        Me.overallAddBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.overallAddBtn.UseVisualStyleBackColor = True
        '
        'overallDownBtn
        '
        Me.overallDownBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.overallDownBtn.Enabled = False
        Me.overallDownBtn.ImageIndex = 4
        Me.overallDownBtn.ImageList = Me.ImageList1
        Me.overallDownBtn.Location = New System.Drawing.Point(245, 206)
        Me.overallDownBtn.Name = "overallDownBtn"
        Me.overallDownBtn.Size = New System.Drawing.Size(67, 23)
        Me.overallDownBtn.TabIndex = 5
        Me.overallDownBtn.Text = "Down"
        Me.overallDownBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.overallDownBtn.UseVisualStyleBackColor = True
        '
        'overallUpBtn
        '
        Me.overallUpBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.overallUpBtn.Enabled = False
        Me.overallUpBtn.ImageIndex = 5
        Me.overallUpBtn.ImageList = Me.ImageList1
        Me.overallUpBtn.Location = New System.Drawing.Point(245, 179)
        Me.overallUpBtn.Name = "overallUpBtn"
        Me.overallUpBtn.Size = New System.Drawing.Size(67, 23)
        Me.overallUpBtn.TabIndex = 4
        Me.overallUpBtn.Text = "Up"
        Me.overallUpBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.overallUpBtn.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 4)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(93, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Overall Properties:"
        '
        'overallPropList
        '
        Me.overallPropList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.overallPropList.FormattingEnabled = True
        Me.overallPropList.Location = New System.Drawing.Point(6, 23)
        Me.overallPropList.Name = "overallPropList"
        Me.overallPropList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.overallPropList.Size = New System.Drawing.Size(306, 147)
        Me.overallPropList.TabIndex = 1
        '
        'phaseAvailList
        '
        Me.phaseAvailList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.phaseAvailList.FormattingEnabled = True
        Me.phaseAvailList.Location = New System.Drawing.Point(3, 241)
        Me.phaseAvailList.Name = "phaseAvailList"
        Me.phaseAvailList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.phaseAvailList.Size = New System.Drawing.Size(330, 95)
        Me.phaseAvailList.Sorted = True
        Me.phaseAvailList.TabIndex = 6
        '
        'phaseRemoveBtn
        '
        Me.phaseRemoveBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.phaseRemoveBtn.Enabled = False
        Me.phaseRemoveBtn.ImageIndex = 1
        Me.phaseRemoveBtn.ImageList = Me.ImageList1
        Me.phaseRemoveBtn.Location = New System.Drawing.Point(4, 206)
        Me.phaseRemoveBtn.Name = "phaseRemoveBtn"
        Me.phaseRemoveBtn.Size = New System.Drawing.Size(75, 23)
        Me.phaseRemoveBtn.TabIndex = 3
        Me.phaseRemoveBtn.Text = "Remove"
        Me.phaseRemoveBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.phaseRemoveBtn.UseVisualStyleBackColor = True
        '
        'phaseAddBtn
        '
        Me.phaseAddBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.phaseAddBtn.Enabled = False
        Me.phaseAddBtn.ImageIndex = 6
        Me.phaseAddBtn.ImageList = Me.ImageList1
        Me.phaseAddBtn.Location = New System.Drawing.Point(3, 179)
        Me.phaseAddBtn.Name = "phaseAddBtn"
        Me.phaseAddBtn.Size = New System.Drawing.Size(76, 23)
        Me.phaseAddBtn.TabIndex = 2
        Me.phaseAddBtn.Text = "Add"
        Me.phaseAddBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.phaseAddBtn.UseVisualStyleBackColor = True
        '
        'phaseDownBtn
        '
        Me.phaseDownBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.phaseDownBtn.Enabled = False
        Me.phaseDownBtn.ImageIndex = 4
        Me.phaseDownBtn.ImageList = Me.ImageList1
        Me.phaseDownBtn.Location = New System.Drawing.Point(266, 206)
        Me.phaseDownBtn.Name = "phaseDownBtn"
        Me.phaseDownBtn.Size = New System.Drawing.Size(67, 23)
        Me.phaseDownBtn.TabIndex = 5
        Me.phaseDownBtn.Text = "Down"
        Me.phaseDownBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.phaseDownBtn.UseVisualStyleBackColor = True
        '
        'phaseUpBtn
        '
        Me.phaseUpBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.phaseUpBtn.Enabled = False
        Me.phaseUpBtn.ImageIndex = 5
        Me.phaseUpBtn.ImageList = Me.ImageList1
        Me.phaseUpBtn.Location = New System.Drawing.Point(266, 179)
        Me.phaseUpBtn.Name = "phaseUpBtn"
        Me.phaseUpBtn.Size = New System.Drawing.Size(67, 23)
        Me.phaseUpBtn.TabIndex = 4
        Me.phaseUpBtn.Text = "Up"
        Me.phaseUpBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.phaseUpBtn.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 4)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(109, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Per Phase Properties:"
        '
        'phasePropList
        '
        Me.phasePropList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.phasePropList.FormattingEnabled = True
        Me.phasePropList.Location = New System.Drawing.Point(3, 23)
        Me.phasePropList.Name = "phasePropList"
        Me.phasePropList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.phasePropList.Size = New System.Drawing.Size(330, 147)
        Me.phasePropList.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.saveNowBtn)
        Me.GroupBox1.Controls.Add(Me.ResetBtn)
        Me.GroupBox1.Controls.Add(Me.AutoSave)
        Me.GroupBox1.Controls.Add(Me.browseXPath)
        Me.GroupBox1.Controls.Add(Me.XPath)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 7)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(661, 85)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Location of .xflow file:"
        '
        'saveNowBtn
        '
        Me.saveNowBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.saveNowBtn.ImageIndex = 7
        Me.saveNowBtn.ImageList = Me.ImageList1
        Me.saveNowBtn.Location = New System.Drawing.Point(570, 49)
        Me.saveNowBtn.Name = "saveNowBtn"
        Me.saveNowBtn.Size = New System.Drawing.Size(85, 23)
        Me.saveNowBtn.TabIndex = 5
        Me.saveNowBtn.Text = "Save Now"
        Me.saveNowBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.saveNowBtn.UseVisualStyleBackColor = True
        '
        'ResetBtn
        '
        Me.ResetBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ResetBtn.ImageIndex = 1
        Me.ResetBtn.ImageList = Me.ImageList1
        Me.ResetBtn.Location = New System.Drawing.Point(484, 25)
        Me.ResetBtn.Name = "ResetBtn"
        Me.ResetBtn.Size = New System.Drawing.Size(85, 23)
        Me.ResetBtn.TabIndex = 2
        Me.ResetBtn.Text = "Reset"
        Me.ResetBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ResetBtn.UseVisualStyleBackColor = True
        '
        'AutoSave
        '
        Me.AutoSave.AutoSize = True
        Me.AutoSave.Location = New System.Drawing.Point(45, 54)
        Me.AutoSave.Name = "AutoSave"
        Me.AutoSave.Size = New System.Drawing.Size(253, 17)
        Me.AutoSave.TabIndex = 4
        Me.AutoSave.Text = "Automatically save .xflow file at each calculation"
        Me.AutoSave.UseVisualStyleBackColor = True
        '
        'browseXPath
        '
        Me.browseXPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.browseXPath.ImageIndex = 0
        Me.browseXPath.ImageList = Me.ImageList1
        Me.browseXPath.Location = New System.Drawing.Point(570, 25)
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
        Me.XPath.Size = New System.Drawing.Size(433, 20)
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
        'advancedTab
        '
        Me.advancedTab.Controls.Add(Me.GroupBox4)
        Me.advancedTab.Controls.Add(Me.GroupBox3)
        Me.advancedTab.Location = New System.Drawing.Point(4, 22)
        Me.advancedTab.Name = "advancedTab"
        Me.advancedTab.Size = New System.Drawing.Size(674, 462)
        Me.advancedTab.TabIndex = 3
        Me.advancedTab.Text = "Advanced"
        Me.advancedTab.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.Controls.Add(Me.writePhaseInfo)
        Me.GroupBox4.Controls.Add(Me.writeEnthalpy)
        Me.GroupBox4.Controls.Add(Me.writeAutoFlash)
        Me.GroupBox4.Controls.Add(Me.writeComment)
        Me.GroupBox4.Controls.Add(Me.writeCreatorComment)
        Me.GroupBox4.Location = New System.Drawing.Point(4, 110)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(667, 138)
        Me.GroupBox4.TabIndex = 6
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "XFlow file options:"
        '
        'writePhaseInfo
        '
        Me.writePhaseInfo.AutoSize = True
        Me.writePhaseInfo.Location = New System.Drawing.Point(10, 112)
        Me.writePhaseInfo.Name = "writePhaseInfo"
        Me.writePhaseInfo.Size = New System.Drawing.Size(137, 17)
        Me.writePhaseInfo.TabIndex = 4
        Me.writePhaseInfo.Text = "Write phase information"
        Me.writePhaseInfo.UseVisualStyleBackColor = True
        '
        'writeEnthalpy
        '
        Me.writeEnthalpy.AutoSize = True
        Me.writeEnthalpy.Location = New System.Drawing.Point(10, 89)
        Me.writeEnthalpy.Name = "writeEnthalpy"
        Me.writeEnthalpy.Size = New System.Drawing.Size(95, 17)
        Me.writeEnthalpy.TabIndex = 3
        Me.writeEnthalpy.Text = "Write Enthalpy"
        Me.writeEnthalpy.UseVisualStyleBackColor = True
        '
        'writeAutoFlash
        '
        Me.writeAutoFlash.AutoSize = True
        Me.writeAutoFlash.Location = New System.Drawing.Point(10, 66)
        Me.writeAutoFlash.Name = "writeAutoFlash"
        Me.writeAutoFlash.Size = New System.Drawing.Size(170, 17)
        Me.writeAutoFlash.TabIndex = 2
        Me.writeAutoFlash.Text = "Write recommended auto flash"
        Me.writeAutoFlash.UseVisualStyleBackColor = True
        '
        'writeComment
        '
        Me.writeComment.AutoSize = True
        Me.writeComment.Location = New System.Drawing.Point(10, 43)
        Me.writeComment.Name = "writeComment"
        Me.writeComment.Size = New System.Drawing.Size(102, 17)
        Me.writeComment.TabIndex = 1
        Me.writeComment.Text = "Write comments"
        Me.writeComment.UseVisualStyleBackColor = True
        '
        'writeCreatorComment
        '
        Me.writeCreatorComment.AutoSize = True
        Me.writeCreatorComment.Location = New System.Drawing.Point(10, 20)
        Me.writeCreatorComment.Name = "writeCreatorComment"
        Me.writeCreatorComment.Size = New System.Drawing.Size(133, 17)
        Me.writeCreatorComment.TabIndex = 0
        Me.writeCreatorComment.Text = "Write creator comment"
        Me.writeCreatorComment.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.Controls.Add(Me.referenceP)
        Me.GroupBox3.Controls.Add(Me.referenceT)
        Me.GroupBox3.Controls.Add(Me.referenceChoice)
        Me.GroupBox3.Controls.Add(Me.pLabel)
        Me.GroupBox3.Controls.Add(Me.tLabel)
        Me.GroupBox3.Controls.Add(Me.phaseLabel)
        Me.GroupBox3.Location = New System.Drawing.Point(4, 4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(667, 100)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Enthalpy reference state:"
        '
        'referenceP
        '
        Me.referenceP.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.referenceP.Location = New System.Drawing.Point(190, 67)
        Me.referenceP.Name = "referenceP"
        Me.referenceP.Size = New System.Drawing.Size(471, 20)
        Me.referenceP.TabIndex = 5
        '
        'referenceT
        '
        Me.referenceT.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.referenceT.Location = New System.Drawing.Point(190, 42)
        Me.referenceT.Name = "referenceT"
        Me.referenceT.Size = New System.Drawing.Size(471, 20)
        Me.referenceT.TabIndex = 4
        '
        'referenceChoice
        '
        Me.referenceChoice.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.referenceChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.referenceChoice.FormattingEnabled = True
        Me.referenceChoice.Location = New System.Drawing.Point(190, 17)
        Me.referenceChoice.Name = "referenceChoice"
        Me.referenceChoice.Size = New System.Drawing.Size(471, 21)
        Me.referenceChoice.TabIndex = 3
        '
        'pLabel
        '
        Me.pLabel.AutoSize = True
        Me.pLabel.Location = New System.Drawing.Point(7, 68)
        Me.pLabel.Name = "pLabel"
        Me.pLabel.Size = New System.Drawing.Size(127, 13)
        Me.pLabel.TabIndex = 2
        Me.pLabel.Text = "Reference pressure / Pa:"
        '
        'tLabel
        '
        Me.tLabel.AutoSize = True
        Me.tLabel.Location = New System.Drawing.Point(7, 44)
        Me.tLabel.Name = "tLabel"
        Me.tLabel.Size = New System.Drawing.Size(137, 13)
        Me.tLabel.TabIndex = 1
        Me.tLabel.Text = "Reference temperature / K:"
        '
        'phaseLabel
        '
        Me.phaseLabel.AutoSize = True
        Me.phaseLabel.Location = New System.Drawing.Point(7, 20)
        Me.phaseLabel.Name = "phaseLabel"
        Me.phaseLabel.Size = New System.Drawing.Size(86, 13)
        Me.phaseLabel.TabIndex = 0
        Me.phaseLabel.Text = "Reference state:"
        '
        'flowTab
        '
        Me.flowTab.Location = New System.Drawing.Point(4, 22)
        Me.flowTab.Name = "flowTab"
        Me.flowTab.Padding = New System.Windows.Forms.Padding(3)
        Me.flowTab.Size = New System.Drawing.Size(674, 462)
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
        Me.About.Size = New System.Drawing.Size(674, 462)
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
        Me.aboutText.Text = "XFlowSaver CAPE-OPEN Unit Operation"
        '
        'okBtn
        '
        Me.okBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.okBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.okBtn.ImageIndex = 3
        Me.okBtn.ImageList = Me.ImageList1
        Me.okBtn.Location = New System.Drawing.Point(492, 521)
        Me.okBtn.Name = "okBtn"
        Me.okBtn.Size = New System.Drawing.Size(101, 23)
        Me.okBtn.TabIndex = 2
        Me.okBtn.Text = "&Ok"
        Me.okBtn.UseVisualStyleBackColor = True
        '
        'ButtonUpdateTimer
        '
        '
        'mainMenu
        '
        Me.mainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConfigurationToolStripMenuItem, Me.EditToolStripMenuItem})
        Me.mainMenu.Location = New System.Drawing.Point(0, 0)
        Me.mainMenu.Name = "mainMenu"
        Me.mainMenu.Size = New System.Drawing.Size(707, 24)
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
        Me.helpBtn.ImageIndex = 8
        Me.helpBtn.ImageList = Me.ImageList1
        Me.helpBtn.Location = New System.Drawing.Point(13, 521)
        Me.helpBtn.Name = "helpBtn"
        Me.helpBtn.Size = New System.Drawing.Size(101, 23)
        Me.helpBtn.TabIndex = 2
        Me.helpBtn.Text = "&Help"
        Me.helpBtn.UseVisualStyleBackColor = True
        '
        'XFlowSaverDialog
        '
        Me.AcceptButton = Me.okBtn
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cancelBtn
        Me.ClientSize = New System.Drawing.Size(707, 556)
        Me.Controls.Add(Me.helpBtn)
        Me.Controls.Add(Me.okBtn)
        Me.Controls.Add(Me.Tabs)
        Me.Controls.Add(Me.cancelBtn)
        Me.Controls.Add(Me.mainMenu)
        Me.MainMenuStrip = Me.mainMenu
        Me.MinimizeBox = False
        Me.Name = "XFlowSaverDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "XFlowSaverDialog"
        Me.Tabs.ResumeLayout(False)
        Me.settingsTab.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        Me.SplitContainer1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.advancedTab.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
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
    Friend WithEvents AutoSave As System.Windows.Forms.CheckBox
    Friend WithEvents browseXPath As System.Windows.Forms.Button
    Friend WithEvents XPath As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents flowTab As System.Windows.Forms.TabPage
    Friend WithEvents ResetBtn As System.Windows.Forms.Button
    Friend WithEvents okBtn As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents overallAvailList As System.Windows.Forms.ListBox
    Friend WithEvents overallRemoveBtn As System.Windows.Forms.Button
    Friend WithEvents overallAddBtn As System.Windows.Forms.Button
    Friend WithEvents overallDownBtn As System.Windows.Forms.Button
    Friend WithEvents overallUpBtn As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents overallPropList As System.Windows.Forms.ListBox
    Friend WithEvents ButtonUpdateTimer As System.Windows.Forms.Timer
    Friend WithEvents saveNowBtn As System.Windows.Forms.Button
    Friend WithEvents phaseAvailList As System.Windows.Forms.ListBox
    Friend WithEvents phaseRemoveBtn As System.Windows.Forms.Button
    Friend WithEvents phaseAddBtn As System.Windows.Forms.Button
    Friend WithEvents phaseDownBtn As System.Windows.Forms.Button
    Friend WithEvents phaseUpBtn As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents phasePropList As System.Windows.Forms.ListBox
    Friend WithEvents About As System.Windows.Forms.TabPage
    Friend WithEvents aboutText As System.Windows.Forms.TextBox
    Friend WithEvents mainMenu As System.Windows.Forms.MenuStrip
    Friend WithEvents ConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents advancedTab As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents pLabel As System.Windows.Forms.Label
    Friend WithEvents tLabel As System.Windows.Forms.Label
    Friend WithEvents phaseLabel As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents writePhaseInfo As System.Windows.Forms.CheckBox
    Friend WithEvents writeEnthalpy As System.Windows.Forms.CheckBox
    Friend WithEvents writeAutoFlash As System.Windows.Forms.CheckBox
    Friend WithEvents writeComment As System.Windows.Forms.CheckBox
    Friend WithEvents writeCreatorComment As System.Windows.Forms.CheckBox
    Friend WithEvents referenceP As System.Windows.Forms.TextBox
    Friend WithEvents referenceT As System.Windows.Forms.TextBox
    Friend WithEvents referenceChoice As System.Windows.Forms.ComboBox
    Friend WithEvents EditToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyGridToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents helpBtn As System.Windows.Forms.Button
End Class
