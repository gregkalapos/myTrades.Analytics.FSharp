open System
open Xunit
open MyTrades.Analytics
open TestData
open WilliamsPR
open Xunit.Abstractions;

[<EntryPoint>]
let main argv = 
    let prices = GetBmwOhcl;
    
    let williams = WilliamsPR prices 14 |> Seq.toList

    let pricesSkip = GetBmwQuotes |> List.skip ( (prices |> List.length) - (williams |> List.length));
    
    let bTestResult = BackTestWilliamsPr williams pricesSkip
    //let b = williams |> Seq.toList |> List.filter (fun n -> n.Value = 0m || n.Value = -100m)

    printfn "%f" bTestResult.ResultInPercent
    printfn "%A" bTestResult.Transactions
    
    0