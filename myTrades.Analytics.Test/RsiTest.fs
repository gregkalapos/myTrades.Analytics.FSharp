module Tests.Rsi

    open System
    open Xunit
    open MyTrades.Analytics
    open TestData
    open Rsi
    open Xunit.Abstractions;
 
    [<Fact>]
    let ``Calculate RSI Test`` () =  
        
        let prices = GetSampleQuotes
        let rsi = Rsi prices 14

        Assert.Equal( rsi |> Seq.length, ((prices |> Seq.length) - 14) )

        let rsiArray = rsi |> Seq.toArray

        Assert.Equal(new DateTime(2010, 1, 5), (rsiArray |> Array.item 0).Date )
        Assert.Equal( 70.464135021097046413502109707M, (rsiArray |> Array.item 0).Value )

        Assert.Equal(new DateTime(2010, 1, 6), (rsiArray |> Array.item 1).Date )
        Assert.Equal( 66.249618553555080866646322848M, (rsiArray |> Array.item 1).Value )

        Assert.Equal(new DateTime(2010, 1, 7), (rsiArray |> Array.item 2).Date )
        Assert.Equal( 66.480941834712670474414267391M, (rsiArray |> Array.item 2).Value )

        Assert.Equal(new DateTime(2010, 1, 8), (rsiArray |> Array.item 3).Date )
        Assert.Equal( 69.34685316290869851140838117M, (rsiArray |> Array.item 3).Value )

        Assert.Equal(new DateTime(2010, 1, 11), (rsiArray |> Array.item 4).Date )
        Assert.Equal( 66.294712658926250983631764438M, (rsiArray |> Array.item 4).Value )

        Assert.Equal(new DateTime(2010, 1, 12), (rsiArray |> Array.item 5).Date )
        Assert.Equal( 57.915020670085559689002670984M, (rsiArray |> Array.item 5).Value )

        Assert.Equal(new DateTime(2010, 1, 13), (rsiArray |> Array.item 6).Date )
        Assert.Equal( 62.880718309962394088884379344M, (rsiArray |> Array.item 6).Value )

        Assert.Equal(new DateTime(2010, 1, 14), (rsiArray |> Array.item 7).Date )
        Assert.Equal( 63.208788718287767240509901946M, (rsiArray |> Array.item 7).Value )

        Assert.Equal(new DateTime(2010, 1, 15), (rsiArray |> Array.item 8).Date )
        Assert.Equal( 56.011584789547567462137162395M, (rsiArray |> Array.item 8).Value )

        Assert.Equal(new DateTime(2010, 1, 19), (rsiArray |> Array.item 9).Date )
        Assert.Equal( 62.339929310897857299476921701M, (rsiArray |> Array.item 9).Value )

        Assert.Equal(new DateTime(2010, 1, 20), (rsiArray |> Array.item 10).Date )
        Assert.Equal( 54.670971377655154278683610213M, (rsiArray |> Array.item 10).Value )

        Assert.Equal(new DateTime(2010, 1, 21), (rsiArray |> Array.item 11).Date )
        Assert.Equal( 50.386815195114222922667375419M, (rsiArray |> Array.item 11).Value )

        Assert.Equal(new DateTime(2010, 1, 22), (rsiArray |> Array.item 12).Date )
        Assert.Equal( 40.019423791313573854253812379M, (rsiArray |> Array.item 12).Value )

        Assert.Equal(new DateTime(2010, 1, 25), (rsiArray |> Array.item 13).Date )
        Assert.Equal( 41.492635404222836031939613222M, (rsiArray |> Array.item 13).Value )

        Assert.Equal(new DateTime(2010, 1, 26), (rsiArray |> Array.item 14).Date )
        Assert.Equal( 41.902429678458132970793211486M, (rsiArray |> Array.item 14).Value )

        Assert.Equal(new DateTime(2010, 1, 27), (rsiArray |> Array.item 15).Date )
        Assert.Equal( 45.499497238680421428532593259M, (rsiArray |> Array.item 15).Value )

        Assert.Equal(new DateTime(2010, 1, 28), (rsiArray |> Array.item 16).Date )
        Assert.Equal(  37.322778313379953262308463861M, (rsiArray |> Array.item 16).Value )

        Assert.Equal(new DateTime(2010, 1, 29), (rsiArray |> Array.item 17).Date )
        Assert.Equal( 33.090482572723424195175328112M, (rsiArray |> Array.item 17).Value )

        Assert.Equal(new DateTime(2010, 2, 1), (rsiArray |> Array.item 18).Date )
        Assert.Equal(  37.788771982057801896722942343M, (rsiArray |> Array.item 18).Value )
