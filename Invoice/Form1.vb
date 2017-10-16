Imports System.Data.Odbc
Imports System.Text
Imports Invoice.GenCode128
Imports System.Drawing.Printing
Imports System.IO
Imports System.Threading

Public Class Form1
    Private DBReport As New DBzReport
    Private Conn As New OdbcConnection
    Private TmpINI As String
    Private sbSQL As StringBuilder
    Dim cmd As OdbcCommand
    Dim da As New OdbcDataAdapter
    Dim dt As New DataTable
    Dim ValRes As String
    Dim ValRes1 As String
    Dim g_socreatedby As String
    Dim RptINV
    Dim RptDO
    Dim RptTPL
    Dim dtTPL As New DataTable

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not (Conn Is Nothing) Then
            If Conn.State <> ConnectionState.Closed Then
                Conn.Close()
            End If
            Conn = Nothing
        End If

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim strCustom As String
        Dim strInvNo As String = g_strInvoiceNo
        Dim arr()
        Dim strCustName As String = "", strAdd1 As String = "", strAdd2 As String = ""
        Dim strPostCode As String = "", strCity As String = "", strState As String = "", strCountry As String = ""
        Dim strRef As String = "", strRemark As String = "", strCreatedBy As String = ""
        Dim strComName As String = ""

        TmpINI = Microsoft.VisualBasic.Command()
        If TmpINI = "" Then Environment.Exit(0)

        btnPrintSticker.Enabled = False
        tbPrint.TabPages.Remove(tpTPL)

        Init()

        g_strLogonUserID = GetINI("REPORT", "USER", TmpINI)
        g_strInvoiceNo = GetINI("REPORT", "PARAM1", TmpINI)
        g_strLogonLocationID = GetINI("REPORT", "LOCATION", TmpINI)
        g_PrinterPort = GetINI("BARCODE", "PORT")
        strCustom = GetINI("REPORT", "CUSTOM", TmpINI)

        arr = Split(strCustom, ";")
        If arr.Length > 0 Then
            g_IsIBT = CBool(Val(arr(0)))
            If arr.Length > 1 Then
                g_OutletCode = arr(1)
            End If
            If arr.Length > 2 Then
                g_strInvoiceID = arr(2)
            End If
        End If

        ''********** TESTING **********' 
        'g_strLogonUserID = "ADMIN"
        'g_strLogonLocationID = "PENAMPANG2"
        'g_OutletCode = "005"

        ''IBT
        'g_strInvoiceNo = "00000008"
        'g_strInvoiceID = "005z8Kl1y"
        'g_IsIBT = True

        ''INV-IsTransfer=0
        'g_strInvoiceNo = "00002659"
        'g_strInvoiceID = "005z8Kl1y"
        'g_IsIBT = False

        ''INV-IsTransfer=1
        'g_strInvoiceNo = "00002660"
        'g_strInvoiceID = "0051nkM4d"
        'g_IsIBT = False
        ''********** TESTING **********'

        Conn = New OdbcConnection
        Conn.ConnectionString = ConnString
        Conn.Open()
        'checkno14(g_strLogonLocationID & "IV" & g_strInvoiceNo)
        txtBarcode.Text = checkno14(g_OutletCode & "IV" & g_strInvoiceNo)
        MakeBarcodeImage("BarcodeImage", txtBarcode.Text, 1, True)

        If Trim(g_strInvoiceID) <> "" Then
            txtBarcodeID.Text = checkno14(g_strInvoiceID)
            MakeBarcodeImage("BarcodeRefID", txtBarcodeID.Text, 1, True)
        End If

        'MsgBox(ConnString)
        'Conn.Open()
        Me.Text = "Invoice (INV) - Version " & System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString & ""
        'Dim sw As New Stopwatch 
        Dim dtcom As New DataTable
        Dim ConDO As Integer
        Dim dtDO As New DataTable
        Try
            dtcom = DBReport.GetCompany(Conn, g_strLogonLocationID, g_strInvoiceNo)
            If dtcom.Rows.Count > 0 Then
                strComName = CNullStr(dtcom.Rows(0).Item("comName"))
            End If

            dt = New DataTable

            RptINV = New rptInvoiceNoID
            RptDO = New rptDO
            RptTPL = New rptLabel

            If g_strInvoiceNo <> "" Then
                'sw.Start()
                Cursor.Current = Cursors.WaitCursor

                sbSQL = New StringBuilder
                If g_IsIBT Then
                    With sbSQL
                        .Append("SELECT (a.datecreated)AS entrydate, a.barcodeimage AS 'BarcodeImage', a.BarcodeRefID, (d.sellingprice*(100+e.gstvalue)/100)AS SELLINGPRICE, a.istransfer, a.CreatedBy AS 'CreatedBy', a.HasDO AS 'HasDO', a.HasSO AS 'HasSO', a.G_DiscAmt AS 'DiscAmt1', ")
                        .Append("a.G_DiscRate AS 'DiscRate1', a.grandtotal AS 'GrandTotal', a.othercharges AS 'OtherCharges', a.freightcharges AS 'FreightCharges', ")
                        .Append("a.locationcode AS 'LocationCode', a.IBTno AS 'InvoiceNo', a.CustName AS 'CustName', a.Add1 AS 'Add1', a.Add2 AS 'Add2', ")
                        .Append("a.Postcode AS 'Postcode', a.city AS 'City', a.State AS 'State', a.country AS 'Country', ")
                        .Append("a.IBTDate AS InvoiceDate, b.GSTRate, b.GSTAmt, a.custcode, a.GSTCharges, a.GSTNo, a.customcharges1, ")
                        .Append("a.Reference AS 'Reference', a.Remark AS 'Remark', CASE WHEN a.isposted='1' THEN 'Y' ELSE 'N' END AS 'IsPosted', b.rowno AS 'RowNo', ")
                        .Append("if((isnull(b.productcode) or b.productcode=''),c.productcode,b.productcode) AS 'Barcode', IFNULL(c.RefProductCode,'') AS RefProductCode, b.itemcode AS 'ItemCode', b.batchcode AS 'BatchCode', ")
                        .Append("IF(d.GSTCode='03',CONCAT('*',CONCAT(b.description,' ', b.remark)), ")
                        .Append("IF(d.gstcode='02',CONCAT('**',CONCAT(b.description,' ', b.remark)), ")
                        .Append("CONCAT(b.description,' ',b.remark))) AS 'Description', ")
                        .Append("b.artno AS 'ArtNo', ")
                        .Append("b.remark AS 'Remark', ROUND(b.quantity,2) AS 'Quantity', b.uomid AS 'UOMID', b.uomcode AS 'UOMCode', b.factor AS 'Factor', ")
                        .Append("b.quantity AS 'TotalQuantity', b.unitcost AS 'UnitCost', b.totalcost AS 'TotalCost', b.unitprice AS 'UnitPrice', b.discrate AS 'DiscRate', b.discamt AS 'DiscAMT', b.totalprice AS 'TotalPrice'")
                        .Append(", i.TotalZR, i.TotalSR, a.regno ")
                        .Append(" FROM t_ibt a, t_ibtdetail b")
                        .Append(" INNER JOIN (SELECT LocationCode, IBTNo, SUM(IF(GSTRate=0, TotalPrice, 0)) AS TotalZR, SUM(IF(GSTRate<>0, TotalPrice, 0)) AS TotalSR")
                        .Append(" FROM t_ibtdetail WHERE LocationCode='" & g_strLogonLocationID & "' AND IBTNo='" & g_strInvoiceNo & "'")
                        .Append(" GROUP BY LocationCode, IBTNo) i ON a.LocationCode=i.LocationCode AND a.IBTNo=i.IBTNo")
                        .Append(" LEFT JOIN mas_itempacking c ON b.barcode=c.barcode ")
                        .Append("LEFT JOIN mas_itemsellingprice d ON b.locationcode=d.locationcode AND b.itemcode=d.itemcode AND b.barcode=d.barcode AND a.priceid=d.priceid ")
                        .Append("LEFT JOIN MAS_GST e ON d.GSTID=e.GSTID AND d.GSTCODE=e.GSTCODE ")
                        .Append("WHERE(a.IBTno = b.IBTno And a.locationcode = b.locationcode) AND a.locationcode='" & g_strLogonLocationID & "' AND a.IBTNo='" & g_strInvoiceNo & "' AND ROUND(b.quantity,2) <> '0' ORDER BY rowno ")
                    End With
                Else
                    With sbSQL
                        .Append("SELECT (a.datecreated)AS entrydate, a.barcodeimage AS 'BarcodeImage', a.BarcodeRefID, (d.sellingprice*(100+e.gstvalue)/100)AS SELLINGPRICE, a.istransfer, a.CreatedBy AS 'CreatedBy', a.HasDO AS 'HasDO', a.HasSO AS 'HasSO', a.G_DiscAmt AS 'DiscAmt1', ")
                        .Append("a.G_DiscRate AS 'DiscRate1', a.grandtotal AS 'GrandTotal', a.othercharges AS 'OtherCharges', a.freightcharges AS 'FreightCharges', ")
                        .Append("a.locationcode AS 'LocationCode', a.invoiceno AS 'InvoiceNo', a.CustName AS 'CustName', a.Add1 AS 'Add1', a.Add2 AS 'Add2', ")
                        .Append("a.Postcode AS 'Postcode', a.city AS 'City', a.State AS 'State', a.country AS 'Country', ")
                        .Append("a.InvoiceDate, b.GSTRate, b.GSTAmt, a.custcode, a.GSTCharges, a.GSTNo, a.customcharges1, ")
                        .Append("a.Reference AS 'Reference', a.Remark AS 'Remark', CASE WHEN a.isposted='1' THEN 'Y' ELSE 'N' END AS 'IsPosted', b.rowno AS 'RowNo', ")
                        .Append("if((isnull(b.productcode) or b.productcode=''),c.productcode,b.productcode) AS 'Barcode', IFNULL(c.RefProductCode,'') AS RefProductCode, b.itemcode AS 'ItemCode', b.batchcode AS 'BatchCode', ")
                        .Append("IF(d.GSTCode='03',CONCAT('*',CONCAT(b.description,' ', b.remark)), ")
                        .Append("IF(d.gstcode='02',CONCAT('**',CONCAT(b.description,' ', b.remark)), ")
                        .Append("CONCAT(b.description,' ',b.remark))) AS 'Description', ")
                        .Append("b.artno AS 'ArtNo', ")
                        .Append("b.remark AS 'Remark', ROUND(b.quantity,2) AS 'Quantity', b.uomid AS 'UOMID', b.uomcode AS 'UOMCode', b.factor AS 'Factor', ")
                        .Append("b.quantity AS 'TotalQuantity', b.unitcost AS 'UnitCost', b.totalcost AS 'TotalCost', b.unitprice AS 'UnitPrice', b.discrate AS 'DiscRate', b.discamt AS 'DiscAMT', b.totalprice AS 'TotalPrice' ")
                        .Append(", i.TotalZR, i.TotalSR, a.regno ")
                        .Append("FROM t_invoice a, t_invoicedetail b")
                        .Append(" INNER JOIN (SELECT LocationCode, invoiceno, SUM(IF(GSTRate=0, TotalPrice, 0)) AS TotalZR, SUM(IF(GSTRate<>0, TotalPrice, 0)) AS TotalSR")
                        .Append(" FROM t_invoicedetail WHERE LocationCode='" & g_strLogonLocationID & "' AND invoiceno='" & g_strInvoiceNo & "'")
                        .Append(" GROUP BY LocationCode, invoiceno) i ON a.LocationCode=i.LocationCode AND a.invoiceno=i.invoiceno")
                        .Append(" LEFT JOIN mas_itempacking c ON b.barcode=c.barcode ")
                        .Append("LEFT JOIN mas_itemsellingprice d ON b.locationcode=d.locationcode AND b.itemcode=d.itemcode AND b.barcode=d.barcode AND a.priceid=d.priceid ")
                        .Append("LEFT JOIN MAS_GST e ON d.GSTID=e.GSTID AND d.GSTCODE=e.GSTCODE ")
                        .Append("WHERE(a.invoiceno = b.invoiceno And a.locationcode = b.locationcode) AND a.locationcode='" & g_strLogonLocationID & "' AND a.InvoiceNo='" & g_strInvoiceNo & "' AND ROUND(b.quantity,2) <> '0' ORDER BY rowno ")
                    End With
                End If
                cmd = New OdbcCommand(sbSQL.ToString, Conn)
                da = New OdbcDataAdapter(cmd)
                dt = New DataTable
                da.Fill(dt)

                If dt.Rows.Count > 0 Then
                    strInvNo = CNullStr(dt.Rows(0).Item("InvoiceNo"))
                    'Has DO
                    ConDO = dt.Rows(0).Item("HasDO")
                    If ConDO = 1 Then
                        '
                        sbSQL = New StringBuilder
                        If g_IsIBT Then
                            With sbSQL
                                .Append("SELECT IBTno AS 'InvoiceNo', t_ibtref.refdocno AS 'RefDocNo', HasSO, IFNULL(t_deliveryorderref.refdocno,'') AS 'SONo',t_salesorder.createdby as createdby ")
                                .Append(", t_salesorder.IsInventoryEntry, t_salesorder.Priority FROM t_ibtref ")
                                .Append("INNER JOIN t_deliveryorder ON t_ibtref.LocationCode=t_deliveryorder.LocationCode AND t_ibtref.RefDocNo=t_deliveryorder.DoNo ")
                                .Append("LEFT JOIN t_deliveryorderref ON t_deliveryorder.LocationCode=t_deliveryorderref.LocationCode AND t_deliveryorder.DONo=t_deliveryorderref.DONo AND t_deliveryorderref.RefSource='SO' ")
                                .Append("LEFT JOIN t_salesorder ON t_deliveryorderref.reflocationcode=t_salesorder.locationcode AND t_deliveryorderref.refdocno = t_salesorder.sono AND t_deliveryorderref.refSource = 'SO' ")
                                .Append("WHERE t_ibtref.LocationCode='" & g_strLogonLocationID & "' AND ibtno='" & g_strInvoiceNo & "'")
                                .Append("AND t_ibtref.refsource='DO' ")
                            End With
                        Else
                            With sbSQL
                                .Append("SELECT invoiceno AS 'InvoiceNo', t_invoiceref.refdocno AS 'RefDocNo', HasSO, IFNULL(t_deliveryorderref.refdocno,'') AS 'SONo',t_salesorder.createdby as createdby ")
                                .Append(", t_salesorder.IsInventoryEntry, t_salesorder.Priority FROM t_invoiceref ")
                                .Append("INNER JOIN t_deliveryorder ON t_invoiceref.LocationCode=t_deliveryorder.LocationCode AND t_invoiceref.RefDocNo=t_deliveryorder.DoNo ")
                                .Append("LEFT JOIN t_deliveryorderref ON t_deliveryorder.LocationCode=t_deliveryorderref.LocationCode AND t_deliveryorder.DONo=t_deliveryorderref.DONo AND t_deliveryorderref.RefSource='SO' ")
                                .Append("LEFT JOIN t_salesorder ON t_deliveryorderref.reflocationcode=t_salesorder.locationcode AND t_deliveryorderref.refdocno = t_salesorder.sono AND t_deliveryorderref.refSource = 'SO' ")
                                .Append("WHERE t_invoiceref.LocationCode='" & g_strLogonLocationID & "' AND invoiceno='" & g_strInvoiceNo & "'")
                                .Append("AND t_invoiceref.refsource='DO' ")
                            End With
                        End If

                        da = New OdbcDataAdapter(sbSQL.ToString, Conn)
                        dtDO = New DataTable
                        da.Fill(dtDO)

                        g_strDeliveryNo = dtDO.Rows(0).Item("RefDocNo")
                        g_strPriority = CNullStr(dtDO.Rows(0).Item("Priority"))
                        For i As Integer = 0 To dtDO.Rows.Count - 1
                            If g_strSONo <> "" Then
                                g_strSONo = g_strSONo & " , "
                            End If
                            g_strSONo = g_strSONo & dtDO.Rows(i).Item("SONo").ToString
                        Next
                        g_strOrderBy = IIf(dtDO.Rows(0).Item("IsInventoryEntry").ToString = "0", "RECIPIENT", "SENDER")
                        g_socreatedby = dtDO.Rows(0).Item("createdby").ToString
                    Else
                        g_strDeliveryNo = ""
                    End If

                    If DateValue(CDate(dt.Rows(0)("InvoiceDate"))) >= DateValue(g_CutOffDateINVUnitPrice) Then
                        If g_IsIBT Then
                            RptINV = New rptIBT
                        Else
                            If dt.Rows(0)("istransfer").ToString = "1" Then
                                RptINV = New rptTransferINVNew
                            Else
                                RptINV = New rptInvoiceNew
                            End If
                        End If
                    Else
                        If dt.Rows(0)("istransfer").ToString = "1" Then
                            If IsDBNull(dt.Rows(0)("BarcodeRefID")) Then
                                RptINV = New rptTransferInvNoID
                            Else
                                RptINV = New rptTransferInv
                            End If
                        Else
                            If Not IsDBNull(dt.Rows(0)("BarcodeRefID")) Then
                                RptINV = New rptInvoice
                            End If
                        End If
                    End If

                Else
                    strInvNo = ""
                End If
            End If

            RptINV.SetDataSource(dt)

            With RptINV
                '***************** Company Name *************************************
                .SetParameterValue("ComName", CNullStr(dtcom.Rows(0).Item("comName")))
                .SetParameterValue("ComAdd1", CNullStr(dtcom.Rows(0).Item("comAdd1")))
                .SetParameterValue("ComAdd2", CNullStr(dtcom.Rows(0).Item("comAdd2")))
                .SetParameterValue("ComPostCode", CNullStr(dtcom.Rows(0).Item("compostcode")))
                .SetParameterValue("ComCity", CNullStr(dtcom.Rows(0).Item("comcity")))
                .SetParameterValue("ComState", CNullStr(dtcom.Rows(0).Item("comstate")))
                .SetParameterValue("ComTelNo", CNullStr(dtcom.Rows(0).Item("comtelno")))
                .SetParameterValue("ComFaxNo", CNullStr(dtcom.Rows(0).Item("comfaxno")))
                .SetParameterValue("ComGSTNo", CNullStr(dtcom.Rows(0).Item("ComGSTNo")))
                .SetParameterValue("ComRegNo", CNullStr(dtcom.Rows(0).Item("ComRegNo")))
                '********************************************************************
                .SetParameterValue("mInvNo", strInvNo)
                .SetParameterValue("mDONo", CNullStr(g_strDeliveryNo))
                .SetParameterValue("mLocCode", CNullStr(g_strLogonLocationID))
                .SetParameterValue("mSONo", CNullStr(g_strSONo))
                .SetParameterValue("SOCreatedBy", CNullStr(g_socreatedby))
                .SetParameterValue("mOrderBy", CNullStr(g_strOrderBy))
                .SetParameterValue("BarcodeRefID", CNullStr(g_strInvoiceID))
                If g_IsIBT Then
                    .SetParameterValue("IsIBT", True)
                    .SetParameterValue("strTitle", "INTERNAL BRANCH TRANSFER")
                Else
                    .SetParameterValue("IsIBT", False)
                    .SetParameterValue("strTitle", "TAX INVOICE")
                End If
                .SetParameterValue("mPriority", CNullStr(g_strPriority))
            End With
            CrystalReportViewer1.ReportSource = RptINV
            CrystalReportViewer1.Show()


            '***************************************************** Delivery Order **************************************************************************************
            dtcom = DBReport.GetCompanyDO(Conn, g_strLogonLocationID, g_strDeliveryNo)
            If g_strDeliveryNo <> "" Then

                sbSQL = New StringBuilder
                With sbSQL
                    .Append("SELECT a.CreatedBy AS 'CreatedBy', a.IsTransfer, a.HasSO, IFNULL(r.refdocno,'') AS 'SONo', a.dono AS 'DONo', a.DODate, ")
                    .Append("a.custname AS 'CustName', a.add1 AS 'Add1', a.add2 AS 'Add2', a.postcode AS 'PostCode', ")
                    .Append("a.city AS 'City', a.state AS 'State', a.country AS 'Country', a.reference AS 'Reference', a.remark AS 'Remark', CASE WHEN a.isposted='1' THEN 'Y' ELSE 'N' END AS 'IsPosted', ")
                    .Append("b.rowno AS 'No', if((isnull(b.productcode) or b.productcode=''),d.productcode,b.productcode) AS 'SkuCode', IFNULL(c.RefProductCode,'') AS RefProductCode, IF(d.GSTCode='03',CONCAT('*',b.Description),IF(d.GSTCODE='02',CONCAT('**',b.Description),b.Description)) AS 'Description', b.artno AS 'ArtNo', b.batchcode AS 'BatchNo', ")
                    .Append("ROUND(b.quantity,2)AS 'QTY',(b.uomcode)as 'UOM', b.EntryDate, a.gstno ")
                    .Append("FROM t_deliveryorder a, t_deliveryorderdetail b LEFT JOIN mas_itempacking c ON b.barcode=c.barcode ")
                    .Append("LEFT JOIN mas_itemsellingprice d ON b.locationcode=d.locationcode AND b.itemcode=d.itemcode AND b.barcode=d.barcode AND a.priceid=d.priceid ")
                    .Append("LEFT JOIN t_deliveryorderref r ON a.LocationCode=r.LocationCode AND a.DONo=r.DONo AND r.RefSource='SO' ")
                    .Append("WHERE a.dono=b.dono AND a.locationcode=b.locationcode AND a.locationcode='" & g_strLogonLocationID & "' AND a.dono='" & g_strDeliveryNo & "' Group by rowno ORDER BY rowno")
                End With
                'Dim sbh As String = SQL.ToString

                'Console.WriteLine(sbh)

                da = New OdbcDataAdapter(sbSQL.ToString, Conn)
                dtDO = New DataTable
                da.Fill(dtDO)

                Dim dtSONo As New DataTable
                If dtDO.Rows.Count > 0 Then
                    strCustName = CNullStr(dtDO.Rows(0).Item("CustName"))
                    strAdd1 = CNullStr(dtDO.Rows(0).Item("Add1"))
                    strAdd2 = CNullStr(dtDO.Rows(0).Item("Add2"))
                    strPostCode = CNullStr(dtDO.Rows(0).Item("PostCode"))
                    strCity = CNullStr(dtDO.Rows(0).Item("City"))
                    strState = CNullStr(dtDO.Rows(0).Item("State"))
                    strCountry = CNullStr(dtDO.Rows(0).Item("Country"))
                    strRef = CNullStr(dtDO.Rows(0).Item("Reference"))
                    strRemark = CNullStr(dtDO.Rows(0).Item("Remark"))
                    strCreatedBy = CNullStr(dtDO.Rows(0).Item("CreatedBy"))

                    If CBool(dtDO.Rows(0).Item("HasSO")) Then
                        g_strSONo = ""
                        sbSQL = New StringBuilder
                        With sbSQL
                            .Append("SELECT IFNULL(r.refdocno,'') AS 'SONo', s.Priority ")
                            .Append("FROM t_deliveryorder a, t_deliveryorderdetail b ")
                            .Append("LEFT JOIN t_deliveryorderref r ON a.LocationCode=r.LocationCode AND a.DONo=r.DONo AND r.RefSource='SO' ")
                            .Append("LEFT JOIN t_salesorder s ON r.RefLocationCode=s.LocationCode AND r.RefDocno=s.SONo ")
                            .Append("WHERE a.dono=b.dono AND a.locationcode=b.locationcode AND a.locationcode='" & g_strLogonLocationID & "' AND a.dono='" & g_strDeliveryNo & "' Group by sono ORDER BY refdocno")
                        End With
                        da = New OdbcDataAdapter(sbSQL.ToString, Conn)
                        da.Fill(dtSONo)
                        g_strPriority = CNullStr(dtSONo.Rows(0).Item("Priority"))
                        For i As Integer = 0 To dtSONo.Rows.Count - 1
                            If g_strSONo <> "" Then
                                g_strSONo = g_strSONo & " , "
                            End If
                            g_strSONo = g_strSONo & dtSONo.Rows(i).Item("sono")
                        Next
                    Else
                        g_strSONo = ""
                    End If
                    RptDO.SetDataSource(dtDO)
                End If
            End If

            With RptDO
                '****************** Company Name *****************************************
                .SetParameterValue("ComName", CNullStr(dtcom.Rows(0).Item("comName")))
                .SetParameterValue("ComAdd1", CNullStr(dtcom.Rows(0).Item("comAdd1")))
                .SetParameterValue("ComAdd2", CNullStr(dtcom.Rows(0).Item("comAdd2")))
                .SetParameterValue("ComPostCode", CNullStr(dtcom.Rows(0).Item("compostcode")))
                .SetParameterValue("ComCity", CNullStr(dtcom.Rows(0).Item("comcity")))
                .SetParameterValue("ComState", CNullStr(dtcom.Rows(0).Item("comstate")))
                .SetParameterValue("ComTelNo", CNullStr(dtcom.Rows(0).Item("comtelno")))
                .SetParameterValue("ComFaxNo", CNullStr(dtcom.Rows(0).Item("comfaxno")))
                .SetParameterValue("ComGSTNo", CNullStr(dtcom.Rows(0).Item("ComGSTNo")))
                .SetParameterValue("ComRegNo", CNullStr(dtcom.Rows(0).Item("ComRegNo")))
                '*************************************************************************
                .SetParameterValue("mCustName", strCustName)
                .SetParameterValue("mAdd1", strAdd1)
                .SetParameterValue("mAdd2", strAdd2)
                .SetParameterValue("mPost", strPostCode)
                .SetParameterValue("mCity", strCity)
                .SetParameterValue("mState", strState)
                .SetParameterValue("mCountry", strCountry)
                .SetParameterValue("mRef", strRef)
                .SetParameterValue("mRem", strRemark)
                .SetParameterValue("mUser", strCreatedBy)
                .SetParameterValue("mDoNo", CNullStr(g_strDeliveryNo))
                .SetParameterValue("mDoInv", CNullStr(g_strInvoiceNo))
                .SetParameterValue("mLocCode", CNullStr(g_strLogonLocationID))
                .SetParameterValue("mSONo", CNullStr(g_strSONo))
                .SetParameterValue("mOrderBy", CNullStr(g_strOrderBy))
                .SetParameterValue("mPriority", CNullStr(g_strPriority))
            End With

            If dtDO.Rows.Count > 0 Then
                'MessageBox.Show("Done!", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                'MessageBox.Show("No Record Found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            CrystalReportViewer2.ReportSource = RptDO
            CrystalReportViewer2.Show()

            PreviewTPLLabelling(g_strLogonLocationID, g_strInvoiceNo, strComName)

            tbPrint.SelectedIndex = 0
            cbInvoice.SelectedIndex = 0

        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            Cursor.Current = Cursors.Default
        End Try

        If Conn.State <> ConnectionState.Closed Then
            Conn.Close()
        End If

        '***************************************************** End Report ******************************************************************************************
    End Sub

    Private Sub PreviewTPLLabelling(ByVal LocationCode As String, ByVal InvoiceNo As String, ByVal CompanyName As String)
        Dim dtItem As New DataTable

        Try
            dtTPL = New DataTable
            With dtTPL
                .Columns.Add("LocationCode", GetType(String))
                .Columns.Add("TPLNo", GetType(String))
                .Columns.Add("TPLFrom", GetType(String))
                .Columns.Add("TPLAttn", GetType(String))
                .Columns.Add("VLNo", GetType(String))
                .Columns.Add("TransCom", GetType(String))
                .Columns.Add("Reference", GetType(String))
                .Columns.Add("RowNo", GetType(Integer))
                .Columns.Add("CustCode", GetType(String))
                .Columns.Add("Custname", GetType(String))
                .Columns.Add("InvoiceNo", GetType(String))
                .Columns.Add("DONo", GetType(String))
                .Columns.Add("Section", GetType(String))
                .Columns.Add("Department", GetType(String))
                .Columns.Add("Remark", GetType(String))
                .Columns.Add("mCount", GetType(Integer))
                .Columns.Add("Quantity", GetType(Integer))
                .Columns.Add("UOM", GetType(String))
            End With

            sbSQL = New StringBuilder
            If g_IsIBT Then
                With sbSQL
                    .Append("SELECT a.LocationCode, d.TPLNo, d.TPLFrom, d.TPLAttn, d.VesselLorryNo AS VLNo, d.TransCom, a.Reference, IFNULL(c.No, -1) AS RowNo, a.CustCode, a.Custname")
                    .Append(", a.IBTNo AS InvoiceNo, b.RefDocNo AS DONo, c.Section, a.Remark AS Department, c.Remark")
                    .Append(", IFNULL(c.Quantity, a.TotalCTN) AS Quantity, IFNULL(c.UOM, a.UOM) AS UOM")
                    .Append(" FROM t_ibt a")
                    .Append(" LEFT JOIN t_ibtref b ON a.LocationCode=b.LocationCode AND a.IBTNo=b.IBTNo AND b.RefSource='DO'")
                    .Append(" LEFT JOIN tpl_detail c ON a.LocationCode=c.LocationCode AND a.IBTNo=c.DocNo AND c.DocSource='IBT'")
                    .Append(" LEFT JOIN tpl_header d ON c.TPLNo=d.TPLNo")
                    .Append(String.Format(" WHERE a.LocationCode='{0}' AND a.IBTNo='{1}'", LocationCode, InvoiceNo))
                    .Append(" AND NOT (c.Quantity IS NULL AND a.TotalCTN IS NULL)")
                    .Append(" ORDER BY RowNo")
                End With
            Else
                With sbSQL
                    .Append("SELECT a.LocationCode, d.TPLNo, d.TPLFrom, d.TPLAttn, d.VesselLorryNo AS VLNo, d.TransCom, a.Reference, IFNULL(c.No, -1) AS RowNo, a.CustCode, a.Custname")
                    .Append(", a.InvoiceNo, b.RefDocNo AS DONo, c.Section, a.Remark AS Department, c.Remark")
                    .Append(", IFNULL(c.Quantity, a.TotalCTN) AS Quantity, IFNULL(c.UOM, a.UOM) AS UOM")
                    .Append(" FROM t_invoice a")
                    .Append(" LEFT JOIN t_invoiceref b ON a.LocationCode=b.LocationCode AND a.InvoiceNo=b.InvoiceNo AND b.RefSource='DO'")
                    .Append(" LEFT JOIN tpl_detail c ON a.LocationCode=c.LocationCode AND a.InvoiceNo=c.DocNo AND c.DocSource='INV'")
                    .Append(" LEFT JOIN tpl_header d ON c.TPLNo=d.TPLNo")
                    .Append(String.Format(" WHERE a.LocationCode='{0}' AND a.InvoiceNo='{1}'", LocationCode, InvoiceNo))
                    .Append(" AND NOT (c.Quantity IS NULL AND a.TotalCTN IS NULL)")
                    .Append(" ORDER BY RowNo")
                End With
            End If

            da = New OdbcDataAdapter(sbSQL.ToString, Conn)
            da.Fill(dtItem)

            If dtItem.Rows.Count > 0 Then
                For Each dr As DataRow In dtItem.Rows
                    Dim Qty As Integer = CInt(dr("Quantity"))
                    Dim i As Integer

                    For i = 1 To Qty
                        dtTPL.Rows.Add(dr("LocationCode"), dr("TPLNo"), dr("TPLFrom"), dr("TPLAttn"), _
                                    dr("VLNo"), dr("TransCom"), dr("Reference"), _
                                    dr("RowNo"), dr("CustCode"), dr("Custname"), _
                                    dr("InvoiceNo"), dr("DONo"), dr("Section"), dr("Department"), _
                                    dr("Remark"), i, dr("Quantity"), dr("UOM"))
                    Next
                Next

                If dtTPL.Rows.Count > 0 Then
                    tbPrint.TabPages.Add(tpTPL)
                    RptTPL.SetDataSource(dtTPL)
                    With RptTPL
                        .SetParameterValue("ComName", CompanyName)
                    End With
                    CrystalReportViewer3.ReportSource = RptTPL
                    CrystalReportViewer3.Show()

                    btnPrintSticker.Enabled = True
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub


    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub

    Private Function checkno14(ByVal id As String) As String
        Dim odd As Integer = 0
        Dim even As Integer = 0

        Dim i As Integer

        For i = 0 To id.Length - 1
            If i Mod 2 Then
                odd = odd + Val(Mid(id, i + 1, 1))
            Else
                even = even + Val(Mid(id, i + 1, 1))
            End If
        Next
        Dim check As String
        check = (((even * 3) + odd) Mod 10)
        If check > 0 Then
            check = 10 - check
        End If

        Dim com As String
        com = CStr(id) + check

        Return com
    End Function

#Region "Code patterns"

    ' in principle these rows should each have 6 elements
    ' however, the last one -- STOP -- has 7. The cost of the
    ' extra integers is trivial, and this lets the code flow
    ' much more elegantly
    ' 0
    ' 1
    ' 2
    ' 3
    ' 4
    ' 5
    ' 6
    ' 7
    ' 8
    ' 9
    ' 10
    ' 11
    ' 12
    ' 13
    ' 14
    ' 15
    ' 16
    ' 17
    ' 18
    ' 19
    ' 20
    ' 21
    ' 22
    ' 23
    ' 24
    ' 25
    ' 26
    ' 27
    ' 28
    ' 29
    ' 30
    ' 31
    ' 32
    ' 33
    ' 34
    ' 35
    ' 36
    ' 37
    ' 38
    ' 39
    ' 40
    ' 41
    ' 42
    ' 43
    ' 44
    ' 45
    ' 46
    ' 47
    ' 48
    ' 49
    ' 50
    ' 51
    ' 52
    ' 53
    ' 54
    ' 55
    ' 56
    ' 57
    ' 58
    ' 59
    ' 60
    ' 61
    ' 62
    ' 63
    ' 64
    ' 65
    ' 66
    ' 67
    ' 68
    ' 69
    ' 70
    ' 71
    ' 72
    ' 73
    ' 74
    ' 75
    ' 76
    ' 77
    ' 78
    ' 79
    ' 80
    ' 81
    ' 82
    ' 83
    ' 84
    ' 85
    ' 86
    ' 87
    ' 88
    ' 89
    ' 90
    ' 91
    ' 92
    ' 93
    ' 94
    ' 95
    ' 96
    ' 97
    ' 98
    ' 99
    ' 100
    ' 101
    ' 102
    ' 103
    ' 104
    ' 105
    ' 106
    Private Shared ReadOnly cPatterns As Integer(,) = {{2, 1, 2, 2, 2, 2, _
     0, 0}, {2, 2, 2, 1, 2, 2, _
     0, 0}, {2, 2, 2, 2, 2, 1, _
     0, 0}, {1, 2, 1, 2, 2, 3, _
     0, 0}, {1, 2, 1, 3, 2, 2, _
     0, 0}, {1, 3, 1, 2, 2, 2, _
     0, 0}, _
     {1, 2, 2, 2, 1, 3, _
     0, 0}, {1, 2, 2, 3, 1, 2, _
     0, 0}, {1, 3, 2, 2, 1, 2, _
     0, 0}, {2, 2, 1, 2, 1, 3, _
     0, 0}, {2, 2, 1, 3, 1, 2, _
     0, 0}, {2, 3, 1, 2, 1, 2, _
     0, 0}, _
     {1, 1, 2, 2, 3, 2, _
     0, 0}, {1, 2, 2, 1, 3, 2, _
     0, 0}, {1, 2, 2, 2, 3, 1, _
     0, 0}, {1, 1, 3, 2, 2, 2, _
     0, 0}, {1, 2, 3, 1, 2, 2, _
     0, 0}, {1, 2, 3, 2, 2, 1, _
     0, 0}, _
     {2, 2, 3, 2, 1, 1, _
     0, 0}, {2, 2, 1, 1, 3, 2, _
     0, 0}, {2, 2, 1, 2, 3, 1, _
     0, 0}, {2, 1, 3, 2, 1, 2, _
     0, 0}, {2, 2, 3, 1, 1, 2, _
     0, 0}, {3, 1, 2, 1, 3, 1, _
     0, 0}, _
     {3, 1, 1, 2, 2, 2, _
     0, 0}, {3, 2, 1, 1, 2, 2, _
     0, 0}, {3, 2, 1, 2, 2, 1, _
     0, 0}, {3, 1, 2, 2, 1, 2, _
     0, 0}, {3, 2, 2, 1, 1, 2, _
     0, 0}, {3, 2, 2, 2, 1, 1, _
     0, 0}, _
     {2, 1, 2, 1, 2, 3, _
     0, 0}, {2, 1, 2, 3, 2, 1, _
     0, 0}, {2, 3, 2, 1, 2, 1, _
     0, 0}, {1, 1, 1, 3, 2, 3, _
     0, 0}, {1, 3, 1, 1, 2, 3, _
     0, 0}, {1, 3, 1, 3, 2, 1, _
     0, 0}, _
     {1, 1, 2, 3, 1, 3, _
     0, 0}, {1, 3, 2, 1, 1, 3, _
     0, 0}, {1, 3, 2, 3, 1, 1, _
     0, 0}, {2, 1, 1, 3, 1, 3, _
     0, 0}, {2, 3, 1, 1, 1, 3, _
     0, 0}, {2, 3, 1, 3, 1, 1, _
     0, 0}, _
     {1, 1, 2, 1, 3, 3, _
     0, 0}, {1, 1, 2, 3, 3, 1, _
     0, 0}, {1, 3, 2, 1, 3, 1, _
     0, 0}, {1, 1, 3, 1, 2, 3, _
     0, 0}, {1, 1, 3, 3, 2, 1, _
     0, 0}, {1, 3, 3, 1, 2, 1, _
     0, 0}, _
     {3, 1, 3, 1, 2, 1, _
     0, 0}, {2, 1, 1, 3, 3, 1, _
     0, 0}, {2, 3, 1, 1, 3, 1, _
     0, 0}, {2, 1, 3, 1, 1, 3, _
     0, 0}, {2, 1, 3, 3, 1, 1, _
     0, 0}, {2, 1, 3, 1, 3, 1, _
     0, 0}, _
     {3, 1, 1, 1, 2, 3, _
     0, 0}, {3, 1, 1, 3, 2, 1, _
     0, 0}, {3, 3, 1, 1, 2, 1, _
     0, 0}, {3, 1, 2, 1, 1, 3, _
     0, 0}, {3, 1, 2, 3, 1, 1, _
     0, 0}, {3, 3, 2, 1, 1, 1, _
     0, 0}, _
     {3, 1, 4, 1, 1, 1, _
     0, 0}, {2, 2, 1, 4, 1, 1, _
     0, 0}, {4, 3, 1, 1, 1, 1, _
     0, 0}, {1, 1, 1, 2, 2, 4, _
     0, 0}, {1, 1, 1, 4, 2, 2, _
     0, 0}, {1, 2, 1, 1, 2, 4, _
     0, 0}, _
     {1, 2, 1, 4, 2, 1, _
     0, 0}, {1, 4, 1, 1, 2, 2, _
     0, 0}, {1, 4, 1, 2, 2, 1, _
     0, 0}, {1, 1, 2, 2, 1, 4, _
     0, 0}, {1, 1, 2, 4, 1, 2, _
     0, 0}, {1, 2, 2, 1, 1, 4, _
     0, 0}, _
     {1, 2, 2, 4, 1, 1, _
     0, 0}, {1, 4, 2, 1, 1, 2, _
     0, 0}, {1, 4, 2, 2, 1, 1, _
     0, 0}, {2, 4, 1, 2, 1, 1, _
     0, 0}, {2, 2, 1, 1, 1, 4, _
     0, 0}, {4, 1, 3, 1, 1, 1, _
     0, 0}, _
     {2, 4, 1, 1, 1, 2, _
     0, 0}, {1, 3, 4, 1, 1, 1, _
     0, 0}, {1, 1, 1, 2, 4, 2, _
     0, 0}, {1, 2, 1, 1, 4, 2, _
     0, 0}, {1, 2, 1, 2, 4, 1, _
     0, 0}, {1, 1, 4, 2, 1, 2, _
     0, 0}, _
     {1, 2, 4, 1, 1, 2, _
     0, 0}, {1, 2, 4, 2, 1, 1, _
     0, 0}, {4, 1, 1, 2, 1, 2, _
     0, 0}, {4, 2, 1, 1, 1, 2, _
     0, 0}, {4, 2, 1, 2, 1, 1, _
     0, 0}, {2, 1, 2, 1, 4, 1, _
     0, 0}, _
     {2, 1, 4, 1, 2, 1, _
     0, 0}, {4, 1, 2, 1, 2, 1, _
     0, 0}, {1, 1, 1, 1, 4, 3, _
     0, 0}, {1, 1, 1, 3, 4, 1, _
     0, 0}, {1, 3, 1, 1, 4, 1, _
     0, 0}, {1, 1, 4, 1, 1, 3, _
     0, 0}, _
     {1, 1, 4, 3, 1, 1, _
     0, 0}, {4, 1, 1, 1, 1, 3, _
     0, 0}, {4, 1, 1, 3, 1, 1, _
     0, 0}, {1, 1, 3, 1, 4, 1, _
     0, 0}, {1, 1, 4, 1, 3, 1, _
     0, 0}, {3, 1, 1, 1, 4, 1, _
     0, 0}, _
     {4, 1, 1, 1, 3, 1, _
     0, 0}, {2, 1, 1, 4, 1, 2, _
     0, 0}, {2, 1, 1, 2, 1, 4, _
     0, 0}, {2, 1, 1, 2, 3, 2, _
     0, 0}, {2, 3, 3, 1, 1, 1, _
     2, 0}}

#End Region
    Private Function MakeBarcodeImage(ByVal strColName As String, ByVal InputData As String, ByVal BarWeight As Integer, ByVal AddQuietZone As Boolean) As Image
        ' get the Code128 codes to represent the message
        Dim content As New Code128Content(InputData)
        Dim codes As Integer() = content.Codes
        Dim strReportName As String = "report1"
        'Dim conn = "Driver={MySQL ODBC 3.51 Driver}" & _
        '    ";SERVER=192.168.2.22" & _
        '    ";DATABASE=penampang2" & _
        '    ";PORT=3306" & _
        '    ";UID=root" & _
        '    ";PWD=123" & _
        '    ";OPTION=" & 1 + 2 + 8 + 32 + 2048 + 16384
        'Dim connSuda As New OdbcConnection(conn)

        Dim barcode1 As String = "abc1234568795"
        Dim barcode2 As String = "0002410043"
        Dim width As Integer, height As Integer
        '
        Dim mStream As New System.IO.MemoryStream
        Dim ImageBytes As Byte()
        'mStream.Close()

        width = ((codes.Length - 3) * 20 + 35) * BarWeight
        height = Convert.ToInt32(System.Math.Ceiling(Convert.ToSingle(width) * 0.15F))

        If AddQuietZone Then
            ' on both sides
            width += 2 * 10 * BarWeight
        End If

        ' get surface to draw on
        Dim myimg As Image = New System.Drawing.Bitmap(width, height)
        Using gr As Graphics = Graphics.FromImage(myimg)

            ' set to white so we don't have to fill the spaces with white
            gr.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height)

            ' skip quiet zone
            Dim cursor As Integer = If(AddQuietZone, 10 * BarWeight, 0)

            For codeidx As Integer = 0 To codes.Length - 1
                Dim code As Integer = codes(codeidx)

                ' take the bars two at a time: a black and a white
                For bar As Integer = 0 To 7 Step 2
                    Dim barwidth As Integer = cPatterns(code, bar) * BarWeight
                    Dim spcwidth As Integer = cPatterns(code, bar + 1) * BarWeight

                    ' if width is zero, don't try to draw it
                    If barwidth > 0 Then
                        gr.FillRectangle(System.Drawing.Brushes.Black, cursor, 0, barwidth, height)
                    End If

                    ' note that we never need to draw the space, since we 
                    ' initialized the graphics to all white

                    ' advance cursor beyond this pair
                    cursor += (barwidth + spcwidth)
                Next
            Next

        End Using

        Try
            'myimg.Save("C:\Barcode\ mybarcode " & ".png", System.Drawing.Imaging.ImageFormat.Png)
            myimg.Save(mStream, Imaging.ImageFormat.Png)
            ImageBytes = mStream.GetBuffer()
            mStream.Close()
            'Conn.Open()
            Dim cmd As New OdbcCommand
            Dim strSQL As String
            If g_IsIBT Then
                strSQL = String.Format("UPDATE t_ibt SET {0}=(?) WHERE ibtNo='{1}' and locationcode='{2}'", strColName, g_strInvoiceNo, g_strLogonLocationID)
            Else
                strSQL = String.Format("UPDATE t_invoice SET {0}=(?) WHERE invoiceNo='{1}' and locationcode='{2}'", strColName, g_strInvoiceNo, g_strLogonLocationID)
            End If
            cmd = New OdbcCommand(strSQL, Conn)
            cmd.Parameters.Add(New OdbcParameter("?bar", ImageBytes))

            cmd.ExecuteNonQuery()
            'Conn.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Return myimg
    End Function

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim strsql As New StringBuilder
        Dim s As Integer = 0
        Dim t As Integer = 0
        Dim c As Integer
        Dim col As Boolean
        Dim dr As New DialogResult

        Try
            If Conn.State = ConnectionState.Closed Then Conn.Open()

            btnPrint.Enabled = False
            With PrintDialog1
                dr = .ShowDialog(Me)
                If dr = Windows.Forms.DialogResult.OK Then
                    s = .PrinterSettings.FromPage
                    t = .PrinterSettings.ToPage
                    c = .PrinterSettings.Copies()
                    col = .PrinterSettings.Collate

                    If tbPrint.SelectedIndex = 1 Then
                        RptDO.PrintOptions.PrinterName = .PrinterSettings.PrinterName
                        RptDO.PrintOptions.PaperSize = .PrinterSettings.DefaultPageSettings.PaperSize.Kind
                        If .PrinterSettings.DefaultPageSettings.Landscape Then
                            RptDO.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape
                        Else
                            RptDO.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait
                        End If
                        RptDO.PrintToPrinter(c, col, s, t)
                        DBReport.UpdateDO(Conn)
                    ElseIf tbPrint.SelectedIndex = 2 Then
                        RptTPL.PrintOptions.PrinterName = .PrinterSettings.PrinterName
                        RptTPL.PrintOptions.PaperSize = .PrinterSettings.DefaultPageSettings.PaperSize.Kind
                        If .PrinterSettings.DefaultPageSettings.Landscape Then
                            RptTPL.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape
                        Else
                            RptTPL.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait
                        End If
                        RptTPL.PrintToPrinter(c, col, s, t)
                    Else
                        RptINV.PrintOptions.PrinterName = .PrinterSettings.PrinterName
                        RptINV.PrintOptions.PaperSize = .PrinterSettings.DefaultPageSettings.PaperSize.Kind
                        If .PrinterSettings.DefaultPageSettings.Landscape Then
                            RptINV.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape
                        Else
                            RptINV.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait
                        End If
                        RptINV.PrintToPrinter(c, col, s, t)
                        DBReport.UpdateInvoice(Conn)
                        DBReport.UpdateDO(Conn)
                    End If
                End If
            End With

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        btnPrint.Enabled = True

    End Sub

    Private Sub btnClose_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub tbPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbPrint.Click
        'MsgBox(tbPrint.SelectedTab.ToString)
    End Sub

    Private Sub tbPrint_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbPrint.SelectedIndexChanged
        If tbPrint.SelectedTab.ToString = tpInvoice.ToString Then
            cbInvoice.SelectedIndex = 0
        ElseIf tbPrint.SelectedTab.ToString = tpDO.ToString Then
            cbInvoice.SelectedIndex = 1
        ElseIf tbPrint.SelectedTab.ToString = tpTPL.ToString Then
            cbInvoice.SelectedIndex = 2
        Else
            cbInvoice.SelectedIndex = -1
        End If
    End Sub

    Private Sub cbInvoice_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbInvoice.SelectedIndexChanged
        If UCase(cbInvoice.SelectedItem.ToString) = "INVOICE" Then
            tbPrint.SelectedTab = tpInvoice
        ElseIf UCase(cbInvoice.SelectedItem.ToString) = "DELIVERY ORDER" Then
            tbPrint.SelectedTab = tpDO
        ElseIf UCase(cbInvoice.SelectedItem.ToString) = "TPL LABELLING" Then
            tbPrint.SelectedTab = tpTPL
        Else
            'MsgBox("Empty")
            'tbPrint.SelectedTab = tpDO
        End If
    End Sub

    Private Sub btnPrintSticker_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrintSticker.Click
        Dim TotalCTN As Integer

        TotalCTN = 0
        If dtTPL.Rows.Count > 0 Then
            If MsgBox("Are you sure you want to print the sticker for Invoice No : " & dtTPL.Rows(0).Item("InvoiceNo"), MsgBoxStyle.YesNo, "PRINT STICKER") = MsgBoxResult.Yes Then
                TotalCTN = dtTPL.Rows(0).Item("Quantity")
                For Each dr As DataRow In dtTPL.Rows
                    Dim strPrint As String

                    strPrint = File.ReadAllText(Path.Combine(New DirectoryInfo(Application.StartupPath).Parent.FullName, "TagPack.bin"))

                    strPrint = strPrint.Replace("\%DATE", Format(Now(), "dd/MM/yyyy"))
                    strPrint = strPrint.Replace("\%REFNO", CNullStr(dr("TPLNo")))
                    strPrint = strPrint.Replace("\%DESTINATION", CNullStr(dr("Custname")))
                    strPrint = strPrint.Replace("\%DOCUMENTNO", CNullStr(dr("InvoiceNo")))
                    strPrint = strPrint.Replace("\%QUANTITY", dr("mCount") & " / " & Math.Round(dr("Quantity"), 0) & " " & dr("UOM"))
                    strPrint = strPrint.Replace("\%SECTION", CNullStr(dr("Section")))
                    strPrint = strPrint.Replace("\%REMARK", CNullStr(dr("Remark")))
                    strPrint = strPrint.Replace("\%VLNO", CNullStr(dr("VLNo")))
                    strPrint = strPrint.Replace("\%TRANSCOM", CNullStr(dr("TransCom")))
                    strPrint = strPrint.Replace("\%FROM", CNullStr(dr("TPLFrom")))
                    strPrint = strPrint.Replace("\%REFERENCE", CNullStr(dr("Reference")))
                    strPrint = strPrint.Replace("\%LABEL", dr("mCount"))

                    Thread.Sleep(1000)

                    'WriteLog("Print Sticker", strPrint)
                    PrintBarcode(g_PrinterPort, strPrint)
                Next
            End If
        Else
            MsgBox("No record to print!")
        End If
    End Sub

End Class
