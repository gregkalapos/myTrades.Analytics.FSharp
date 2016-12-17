open System
open Xunit
open MyTrades.Analytics
open MyTrades.Analytics.TestData
open MyTrades.Analytics.WilliamsPR
open Rsi
open Xunit.Abstractions
open UltimateOscillator

[<EntryPoint>]
let main argv = 
    // let prices = GetBmwOhcl;
    
    // let williams = WilliamsPR prices 14 |> Seq.toList

    // let pricesSkip = GetBmwQuotes |> List.skip ( (prices |> List.length) - (williams |> List.length));
    
    // let bTestResult = BackTestWilliamsPr williams pricesSkip
    // //let b = williams |> Seq.toList |> List.filter (fun n -> n.Value = 0m || n.Value = -100m)

    // printfn "%f" bTestResult.ResultInPercent
    // printfn "%A" bTestResult.Transactions
    
    // let prices = GetBmwQuotes

    // let rsiData = Rsi prices 14
    // let rsiBackTesingResult = BackTestRsiWithPrice rsiData (prices |> Seq.skip 14)

    // printfn "%s" (((Seq.head rsiData).Date).ToString());


    // printfn "%s" ((Seq.head (prices |> Seq.skip 14)).Date.ToString())

    
    let prices = GetOlhcForUltimateOscillatorTest
    let rsires = UltimateOscillator prices
    //printfn "RSI: "
   // printfn "%A" (rsi |> Seq.map (fun f -> f.Value)) // ((rsi |> Seq.map(fun f -> f.Value)) |> Seq.toList)

    // let prices2 = (GetSampleQuotes |> Seq.tail)
    // let rsi2 = Rsi prices2 14
    // printfn "RSI2: "
    // printfn "%A" ((rsi2 |> Seq.map(fun f -> f.Value)) |> Seq.toList)

    // let prices = GetSampleQuotes
    // let changes = calculateChanges prices
    
    printfn "%A" rsires


    0