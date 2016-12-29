module Tests.UltimateOscillator

    open System
    open Xunit
    open MyTrades.Analytics
    open TestData
    open UltimateOscillator
    open Xunit.Abstractions;
 
    [<Fact>]
    let ``Calculate Ultimate Oscillator Test`` () =     
        let prices = GetOlhcForUltimateOscillatorTest
        let ultimateOscillator = UltimateOscillator prices

        Assert.Equal( 2, ultimateOscillator |> Seq.length )
        
        Assert.Equal(new DateTime(2010, 11, 30), (ultimateOscillator |> Seq.item 0).Date)
        Assert.Equal(53.467837912763268732770826164M, (ultimateOscillator |> Seq.item 0).Value)

        Assert.Equal(new DateTime(2010, 12, 1), (ultimateOscillator |> Seq.item 1).Date)
        Assert.Equal(57.208319060852953391310230517M, (ultimateOscillator |> Seq.item 1).Value)