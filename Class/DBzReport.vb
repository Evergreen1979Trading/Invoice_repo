Imports System.Text
Imports System.Data.Odbc
Public Class DBzReport
    Private strSQL As New StringBuilder
    Private da As OdbcDataAdapter
    Private cmd As OdbcCommand
    Public Function GetCompany(ByRef conn As OdbcConnection, ByRef locationcode As String, ByRef docno As String) As DataTable
        Dim dtCom As New DataTable
        Try
            Cursor.Current = Cursors.Default

            strSQL = New StringBuilder
            With strSQL
                .Append("SELECT ComName, ComAdd1, ComAdd2, ComPostcode, Comcity, ComState, ComCountry, ComTelNo, ComFaxNo, ComGSTNo, ComRegNo ")
                .Append("FROM MAS_COMPANY ")
                .Append("LEFT JOIN T_INVOICE ON MAS_COMPANY.ComID=T_INVOICE.ComID ")
                .Append(String.Format("WHERE T_INVOICE.LocationCode='{0}' AND T_INVOICE.invoiceNo='{1}' ", locationcode, docno))
            End With
            'Dim s As String = strSQL.ToString

            da = New OdbcDataAdapter(strSQL.ToString, conn)
            dtCom = New DataTable
            da.Fill(dtCom)

            If dtCom.Rows.Count = 0 Then
                dtCom = New DataTable
                strSQL = New StringBuilder
                With strSQL
                    .Append("SELECT ComName, ComAdd1, ComAdd2, ComPostcode, Comcity, ComState, ComCountry, ComTelNo, ComFaxNo, ComGSTNo, ComRegNo ")
                    .Append("FROM MAS_COMPANY ")
                    .Append("WHERE ComID=0")
                End With
                Dim s As String = strSQL.ToString

                da = New OdbcDataAdapter(strSQL.ToString, conn)
                dtCom = New DataTable
                da.Fill(dtCom)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            Cursor.Current = Cursors.WaitCursor
        End Try
        Return dtCom
    End Function
    Public Function GetCompanyDO(ByRef conn As OdbcConnection, ByRef locationcode As String, ByRef docno As String) As DataTable
        Dim dtCom As New DataTable
        Try
            Cursor.Current = Cursors.Default

            strSQL = New StringBuilder
            With strSQL
                .Append("SELECT ComName, ComAdd1, ComAdd2, ComPostcode, Comcity, ComState, ComCountry, ComTelNo, ComFaxNo, ComGSTNo, ComRegNo ")
                .Append("FROM MAS_COMPANY ")
                .Append("LEFT JOIN T_DELIVERYORDER ON MAS_COMPANY.ComID=T_DELIVERYORDER.ComID ")
                .Append(String.Format("WHERE T_DELIVERYORDER.LocationCode='{0}' AND T_DELIVERYORDER.doNo='{1}' ", locationcode, docno))
            End With
            'Dim s As String = strSQL.ToString

            da = New OdbcDataAdapter(strSQL.ToString, conn)
            dtCom = New DataTable
            da.Fill(dtCom)

            If dtCom.Rows.Count = 0 Then
                dtCom = New DataTable
                strSQL = New StringBuilder
                With strSQL
                    .Append("SELECT ComName, ComAdd1, ComAdd2, ComPostcode, Comcity, ComState, ComCountry, ComTelNo, ComFaxNo, ComGSTNo, ComRegNo ")
                    .Append("FROM MAS_COMPANY ")
                    .Append("WHERE ComID=0")
                End With
                Dim s As String = strSQL.ToString

                da = New OdbcDataAdapter(strSQL.ToString, conn)
                dtCom = New DataTable
                da.Fill(dtCom)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            Cursor.Current = Cursors.WaitCursor
        End Try
        Return dtCom
    End Function
    Public Function DropTable(ByVal TableName As String, ByRef Conn As OdbcConnection) As Boolean
        Try
            strSQL = New StringBuilder
            With strSQL
                .Append("DROP TALE IF EXISTS " & TableName)
            End With
            cmd = New OdbcCommand(strSQL.ToString, Conn)
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return False
        End Try
        Return Nothing
    End Function
    Public Function UpdateInvoice(ByRef Conn As OdbcConnection) As Boolean
        Try
            strSQL = New StringBuilder
            With strSQL
                .Append("UPDATE t_invoice")
                .Append(" SET IsPrinted='1'")
                .Append(String.Format(" WHERE LocationCode='{0}' AND InvoiceNo='{1}'", g_strLogonLocationID, g_strInvoiceNo))
            End With
            cmd = New OdbcCommand(strSQL.ToString, Conn)
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return False
        End Try
    End Function
    Public Function UpdateDO(ByRef Conn As OdbcConnection) As Boolean
        Try
            strSQL = New StringBuilder
            With strSQL
                .Append("UPDATE t_deliveryorder")
                .Append(" SET IsPrinted='1'")
                .Append(String.Format(" WHERE LocationCode='{0}' AND DONo='{1}'", g_strLogonLocationID, g_strDeliveryNo))
            End With
            cmd = New OdbcCommand(strSQL.ToString, Conn)
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return False
        End Try
    End Function
End Class
