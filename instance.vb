Public Class instance
    Public InbDes As Integer 'number inbound
    Public OutDes As Integer 'number outbound

    Public alphaA As Double 'start of the interval of alpha
    Public alphaB As Double 'end of the interval of alpha

    Public NumberOfSegments As Integer
    Public DsIN As Integer 'number of inboutDes per Segment
    Public DsOut As Integer 'number of inboutDes per Segment
    Private OutPerIn() As Integer 'number of outbound destinations served by each inbound destination 
    Private rnd As Random

    Public bio(,) As Integer 'number of parcels to be shipped from inbound destination i to outbound destination o

    Sub New(ByVal inbDes As Integer, ByVal outDes As Integer, ByVal alphaa As Double, ByVal alphab As Double)
        Me.rnd = New Random
        Me.InbDes = inbDes - 1
        Me.OutDes = outDes - 1
        Me.alphaA = alphaa
        Me.alphaB = alphab
        ReDim Me.OutPerIn(Me.InbDes)
        ReDim Me.bio(Me.InbDes, Me.OutDes)

        NumberOfSegments = CInt((2 / 3) * Math.Min(inbDes + 1, outDes + 1))

        DsIN = Math.Round(inbDes / NumberOfSegments)
        DsOut = Math.Round(outDes / NumberOfSegments)

        OutPerIn = getOutPerIn(OutPerIn)
        getBio()
    End Sub

    Function getOutPerIn(array() As Integer) 'support function for bio
        Dim zValue(UBound(array)) As Integer

        For i = 0 To UBound(zValue)
            zValue(i) = CInt(getRND() * OutDes)
        Next

        Return zValue
    End Function

    Sub getBio() ' draw a number of parcels to be shipped from inbound destination i to outbound destination o, for each i and o
        For i = 0 To InbDes
            Dim RndSeq() As Integer
            RndSeq = getRndOutSeq()
            For j = 0 To OutPerIn(i) - 1
                Dim RndAlpha As Double = CInt(getRND() * OutDes)
                Dim Muy As Double = CInt(5000 / RndAlpha)
                Dim sigma As Double = CInt(0.2 * 5000 / RndAlpha)
                bio(i, RndSeq(j)) = rnd.Next(Muy - sigma, Muy + sigma)
            Next
        Next
    End Sub

    Function getRND() 'draw a random double in [alphaA,alphaB]
        Dim zRnd As Double
        Do
            zRnd = rnd.NextDouble
        Loop Until zRnd >= alphaA And zRnd <= alphaB
        Return zRnd
    End Function

    Function getRndOutSeq() 'draw random sequence of outbound destinations
        Dim outSeq(OutDes) As Integer
        Dim zList As New List(Of Integer)
        For i = 0 To OutDes
            zList.Add(i)
        Next
        For i = 0 To OutDes
            Dim zRnd As Integer = rnd.Next(0, zList.Count)
            outSeq(i) = zList(zRnd)
            zList.RemoveAt(zRnd)
        Next
        Return outSeq
    End Function
End Class
