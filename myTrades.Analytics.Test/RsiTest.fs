// module Tests.Rsi

//     open System
//     open Xunit
//     open MyTrades.Analytics
//     open TestData
//     open Rsi
//     open Xunit.Abstractions;
 
//     [<Fact>]
//     let ``Calculate RSI Test`` () =  
        
//         let prices = GetSampleQuotes
//         let rsi = Rsi prices 14

//         Assert.Equal( rsi |> Seq.length, ((prices |> Seq.length) - 14) )
    
//     [<Fact>]
//     let ``RSI Test shift by 1 day`` () =           
//         let prices = GetSampleQuotes
//         let rsi = Rsi prices 14

//         Assert.Equal( rsi |> Seq.length, ((prices |> Seq.length) - 14) )

//         let prices2 = (GetSampleQuotes |> Seq.tail)
//         let rsi2 = Rsi prices2 14

//         Assert.Equal( rsi2 |> Seq.length, ((prices2 |> Seq.length) - 14) )

//         for i = 0 to (prices2 |> Seq.length) do
//           Assert.Equal( (( (rsi |> Seq.toArray) |> Array.item (i+1)).Value), (((rsi2 |> Seq.toArray) |> Array.item i).Value ));