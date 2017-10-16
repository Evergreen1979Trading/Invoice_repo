Imports System.Drawing
Imports System.Diagnostics

Namespace GenCode128
    ''' <summary>
    ''' Summary description for Code128Rendering.
    ''' </summary>
    Public NotInheritable Class Code128Rendering
        Private Sub New()
        End Sub

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

        Private Const cQuietWidth As Integer = 10

        ''' <summary>
        ''' Make an image of a Code128 barcode for a given string
        ''' </summary>
        ''' <param name="InputData">Message to be encoded</param>
        ''' <param name="BarWeight">Base thickness for bar width (1 or 2 works well)</param>
        ''' <param name="AddQuietZone">Add required horiz margins (use if output is tight)</param>
        ''' <returns>An Image of the Code128 barcode representing the message</returns>
        Public Shared Function MakeBarcodeImage(ByVal InputData As String, ByVal BarWeight As Integer, ByVal AddQuietZone As Boolean) As Image
            ' get the Code128 codes to represent the message
            Dim content As New Code128Content(InputData)
            Dim codes As Integer() = content.Codes

            Dim width As Integer, height As Integer
            width = ((codes.Length - 3) * 11 + 35) * BarWeight
            height = Convert.ToInt32(System.Math.Ceiling(Convert.ToSingle(width) * 0.15F))

            If AddQuietZone Then
                ' on both sides
                width += 2 * cQuietWidth * BarWeight
            End If

            ' get surface to draw on
            Dim myimg As Image = New System.Drawing.Bitmap(width, height)
            Using gr As Graphics = Graphics.FromImage(myimg)

                ' set to white so we don't have to fill the spaces with white
                gr.FillRectangle(System.Drawing.Brushes.White, 0, 0, width, height)

                ' skip quiet zone
                Dim cursor As Integer = If(AddQuietZone, cQuietWidth * BarWeight, 0)

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

            Return myimg
        End Function

    End Class
End Namespace
