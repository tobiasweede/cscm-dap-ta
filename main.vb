Module main

    Sub Main()
        Dim inDes As Integer = 3 '{3,5,7,25,50,75}
        Dim outDes As Integer = 7 '{3,5,7,25,50,75}
        Dim alphaa As Double = 0.3 '[0.1,0.5][0.6,0.9]
        Dim alphab As Double = 0.5 '[0.1,0.5][0.6,0.9]

        Dim instance As New instance(inDes, outDes, alphaa, alphab)

        thresholdAccepting(instance)
        Console.WriteLine("Press Enter to continue...")
        Console.ReadLine()
    End Sub

    Function evaluateObjective(ByRef instance As instance, ByRef inboundDestination() As Integer, ByRef outboundDestionation() As Integer) As Integer
        Dim objective As Integer = 0
        For i = 0 To instance.InbDes - 1 ' for all inbound destionations
            For j = 0 To instance.OutDes - 1 ' for all outbound destinations
                objective += instance.bio(i, j) * (1 + Math.Abs(inboundDestination(i) - outboundDestionation(j)))
            Next
        Next
        Return objective
    End Function

    Sub thresholdAccepting(ByRef instance As instance)
        ' init
        Dim temperature As Integer = 100 ' TA temperature
        Dim stopTemperature As Integer = 2 ' stop temperature
        Dim cooldownFactor As Double = 0.8 ' cooldown rate
        Dim objective As Integer = 0 ' objective value
        Dim inboundDestination(instance.InbDes - 1) As Integer ' array for inbound destinations
        Dim outboundDestination(instance.OutDes - 1) As Integer ' array for outbound destinations
        Dim bestOutboundDestination(instance.OutDes - 1) As Integer ' array for best outbound destinations

        ' start solution
        ' assign inbound destinations (fill up each sector sequentially)
        Dim inboundAssigned As Integer = 0
        For i = 0 To instance.InbDes - 1 ' for all inbound destionations
            inboundDestination(i) = 1 + (inboundAssigned \ instance.DsIN) ' DsIn is maximum amout of inbound stations
            inboundAssigned += 1
        Next
        ' assign outbound destinations (fill up each sector sequentially)
        Dim outboundAssigned As Integer = 0
        For j = 0 To instance.OutDes - 1 ' for all outbound destinations
            outboundDestination(j) = 1 + (outboundAssigned \ instance.DsOut)
            outboundAssigned += 1
        Next
        objective = evaluateObjective(instance, inboundDestination, outboundDestination) ' initial (best) objective
        Console.WriteLine("initial objective: " & objective)
        Console.WriteLine("initial output assignment: ")
        For j = 0 To instance.OutDes - 1 ' for all outbound destinations
            Console.Write(outboundDestination(j) & " ")
        Next
        Console.Write(Environment.NewLine)

        ' TA main loop
        Do
            ' create neighbor solution
            Dim newOutboundDestination(instance.OutDes - 1) As Integer ' array for new outbound destinations
            outboundDestination.CopyTo(newOutboundDestination, 0)
            ' get two random destinations to swap
            Dim rnd = New Random
            Dim dst1Idx As Integer = rnd.Next(instance.OutDes - 1) ' random index for first destination to swap
            Dim dst2Idx As Integer ' random index for second destination to swap
            Do ' dst1 and dst2 must be different!
                dst2Idx = rnd.Next(instance.OutDes - 1)
            Loop While dst1Idx = dst2Idx
            ' swap two elements within newOutboundDestination
            newOutboundDestination(dst1Idx) = outboundDestination(dst2Idx)
            newOutboundDestination(dst2Idx) = outboundDestination(dst1Idx)
            Dim newObjective As Integer = evaluateObjective(instance, inboundDestination, newOutboundDestination) ' initial (best) objective
            ' check if maximum output destinations per segment is violated
            Dim outputsPerSegment() As Integer = Enumerable.Repeat(0, instance.NumberOfSegments).ToArray  ' array to count outputs per segment initialized with zeros
            Dim validSolution As Boolean = True
            For j = 0 To instance.OutDes - 1 ' count outbound destinations for all segments
                outputsPerSegment(newOutboundDestination(j) - 1) += 1
                If outputsPerSegment(newOutboundDestination(j) - 1) > instance.DsOut Then
                    validSolution = False
                    Exit For
                End If
            Next
            ' save new (valid) solution if objective is better
            If validSolution And newObjective <= objective Then
                objective = newObjective
                newOutboundDestination.CopyTo(outboundDestination, 0)
            End If
            temperature *= cooldownFactor ' cooldown
            Debug.WriteLine("temperature: " & temperature)
        Loop While stopTemperature < temperature

        Console.WriteLine("best objective: " & objective)
        Console.WriteLine("best output assignment: ")
        For j = 0 To instance.OutDes - 1 ' for all outbound destinations
            Console.Write(outboundDestination(j) & " ")
        Next
        Console.Write(Environment.NewLine)
    End Sub

End Module
