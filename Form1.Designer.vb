<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.tbPrint = New System.Windows.Forms.TabControl
        Me.tpInvoice = New System.Windows.Forms.TabPage
        Me.CrystalReportViewer1 = New CrystalDecisions.Windows.Forms.CrystalReportViewer
        Me.tpDO = New System.Windows.Forms.TabPage
        Me.CrystalReportViewer2 = New CrystalDecisions.Windows.Forms.CrystalReportViewer
        Me.tpTPL = New System.Windows.Forms.TabPage
        Me.CrystalReportViewer3 = New CrystalDecisions.Windows.Forms.CrystalReportViewer
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnPrintSticker = New System.Windows.Forms.Button
        Me.btnPrint = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.cbInvoice = New System.Windows.Forms.ComboBox
        Me.btnClose = New System.Windows.Forms.Button
        Me.txtBarcode = New System.Windows.Forms.TextBox
        Me.PrintDialog1 = New System.Windows.Forms.PrintDialog
        Me.txtBarcodeID = New System.Windows.Forms.TextBox
        Me.tbPrint.SuspendLayout()
        Me.tpInvoice.SuspendLayout()
        Me.tpDO.SuspendLayout()
        Me.tpTPL.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tbPrint
        '
        Me.tbPrint.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbPrint.Controls.Add(Me.tpInvoice)
        Me.tbPrint.Controls.Add(Me.tpDO)
        Me.tbPrint.Controls.Add(Me.tpTPL)
        Me.tbPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbPrint.Location = New System.Drawing.Point(11, 78)
        Me.tbPrint.Name = "tbPrint"
        Me.tbPrint.SelectedIndex = 0
        Me.tbPrint.Size = New System.Drawing.Size(732, 479)
        Me.tbPrint.TabIndex = 3
        '
        'tpInvoice
        '
        Me.tpInvoice.Controls.Add(Me.CrystalReportViewer1)
        Me.tpInvoice.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tpInvoice.Location = New System.Drawing.Point(4, 25)
        Me.tpInvoice.Name = "tpInvoice"
        Me.tpInvoice.Padding = New System.Windows.Forms.Padding(3)
        Me.tpInvoice.Size = New System.Drawing.Size(724, 450)
        Me.tpInvoice.TabIndex = 0
        Me.tpInvoice.Text = "Invoice"
        Me.tpInvoice.UseVisualStyleBackColor = True
        '
        'CrystalReportViewer1
        '
        Me.CrystalReportViewer1.ActiveViewIndex = -1
        Me.CrystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer1.DisplayGroupTree = False
        Me.CrystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CrystalReportViewer1.Location = New System.Drawing.Point(3, 3)
        Me.CrystalReportViewer1.Name = "CrystalReportViewer1"
        Me.CrystalReportViewer1.SelectionFormula = ""
        Me.CrystalReportViewer1.ShowPrintButton = False
        Me.CrystalReportViewer1.Size = New System.Drawing.Size(718, 444)
        Me.CrystalReportViewer1.TabIndex = 0
        Me.CrystalReportViewer1.ViewTimeSelectionFormula = ""
        '
        'tpDO
        '
        Me.tpDO.Controls.Add(Me.CrystalReportViewer2)
        Me.tpDO.Location = New System.Drawing.Point(4, 25)
        Me.tpDO.Name = "tpDO"
        Me.tpDO.Padding = New System.Windows.Forms.Padding(3)
        Me.tpDO.Size = New System.Drawing.Size(724, 450)
        Me.tpDO.TabIndex = 1
        Me.tpDO.Text = "Delivery Order"
        Me.tpDO.UseVisualStyleBackColor = True
        '
        'CrystalReportViewer2
        '
        Me.CrystalReportViewer2.ActiveViewIndex = -1
        Me.CrystalReportViewer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer2.DisplayGroupTree = False
        Me.CrystalReportViewer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CrystalReportViewer2.Location = New System.Drawing.Point(3, 3)
        Me.CrystalReportViewer2.Name = "CrystalReportViewer2"
        Me.CrystalReportViewer2.SelectionFormula = ""
        Me.CrystalReportViewer2.ShowPrintButton = False
        Me.CrystalReportViewer2.Size = New System.Drawing.Size(718, 444)
        Me.CrystalReportViewer2.TabIndex = 0
        Me.CrystalReportViewer2.ViewTimeSelectionFormula = ""
        '
        'tpTPL
        '
        Me.tpTPL.Controls.Add(Me.CrystalReportViewer3)
        Me.tpTPL.Location = New System.Drawing.Point(4, 25)
        Me.tpTPL.Name = "tpTPL"
        Me.tpTPL.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTPL.Size = New System.Drawing.Size(724, 450)
        Me.tpTPL.TabIndex = 2
        Me.tpTPL.Text = "TPL Labelling"
        Me.tpTPL.UseVisualStyleBackColor = True
        '
        'CrystalReportViewer3
        '
        Me.CrystalReportViewer3.ActiveViewIndex = -1
        Me.CrystalReportViewer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer3.DisplayGroupTree = False
        Me.CrystalReportViewer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CrystalReportViewer3.Location = New System.Drawing.Point(3, 3)
        Me.CrystalReportViewer3.Name = "CrystalReportViewer3"
        Me.CrystalReportViewer3.SelectionFormula = ""
        Me.CrystalReportViewer3.ShowPrintButton = False
        Me.CrystalReportViewer3.Size = New System.Drawing.Size(718, 444)
        Me.CrystalReportViewer3.TabIndex = 1
        Me.CrystalReportViewer3.ViewTimeSelectionFormula = ""
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnPrintSticker)
        Me.GroupBox1.Controls.Add(Me.btnPrint)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.cbInvoice)
        Me.GroupBox1.Controls.Add(Me.btnClose)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 9)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(599, 61)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        '
        'btnPrintSticker
        '
        Me.btnPrintSticker.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrintSticker.Location = New System.Drawing.Point(408, 12)
        Me.btnPrintSticker.Name = "btnPrintSticker"
        Me.btnPrintSticker.Size = New System.Drawing.Size(85, 45)
        Me.btnPrintSticker.TabIndex = 12
        Me.btnPrintSticker.Text = "Print TPL Sticker"
        Me.btnPrintSticker.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(315, 12)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(85, 45)
        Me.btnPrint.TabIndex = 11
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(13, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 16)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Selection       :"
        '
        'cbInvoice
        '
        Me.cbInvoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbInvoice.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbInvoice.FormattingEnabled = True
        Me.cbInvoice.Items.AddRange(New Object() {"Invoice", "Delivery Order", "TPL Labelling"})
        Me.cbInvoice.Location = New System.Drawing.Point(107, 19)
        Me.cbInvoice.Name = "cbInvoice"
        Me.cbInvoice.Size = New System.Drawing.Size(178, 24)
        Me.cbInvoice.TabIndex = 8
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(500, 12)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(85, 45)
        Me.btnClose.TabIndex = 7
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'txtBarcode
        '
        Me.txtBarcode.Location = New System.Drawing.Point(612, 17)
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.Size = New System.Drawing.Size(132, 20)
        Me.txtBarcode.TabIndex = 10
        Me.txtBarcode.Visible = False
        '
        'PrintDialog1
        '
        Me.PrintDialog1.AllowSelection = True
        Me.PrintDialog1.AllowSomePages = True
        Me.PrintDialog1.UseEXDialog = True
        '
        'txtBarcodeID
        '
        Me.txtBarcodeID.Location = New System.Drawing.Point(613, 43)
        Me.txtBarcodeID.Name = "txtBarcodeID"
        Me.txtBarcodeID.Size = New System.Drawing.Size(132, 20)
        Me.txtBarcodeID.TabIndex = 11
        Me.txtBarcodeID.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(752, 566)
        Me.Controls.Add(Me.txtBarcodeID)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.tbPrint)
        Me.Name = "Form1"
        Me.Text = "Invoice"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.tbPrint.ResumeLayout(False)
        Me.tpInvoice.ResumeLayout(False)
        Me.tpDO.ResumeLayout(False)
        Me.tpTPL.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tbPrint As System.Windows.Forms.TabControl
    Friend WithEvents tpInvoice As System.Windows.Forms.TabPage
    Friend WithEvents CrystalReportViewer1 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents tpDO As System.Windows.Forms.TabPage
    Friend WithEvents CrystalReportViewer2 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents txtBarcode As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cbInvoice As System.Windows.Forms.ComboBox
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents PrintDialog1 As System.Windows.Forms.PrintDialog
    Friend WithEvents txtBarcodeID As System.Windows.Forms.TextBox
    Friend WithEvents tpTPL As System.Windows.Forms.TabPage
    Friend WithEvents CrystalReportViewer3 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents btnPrintSticker As System.Windows.Forms.Button

End Class
