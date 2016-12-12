namespace MyTrades.Analytics

open Common
open System

module Rsi =        
    //Decimal value is always positive
    //e.g. Down -2 is Downward 2
    type Change =
        | Upward of change: decimal
        | Downward of change: decimal
        | Equal    
    
    type DateWithChange = {Date: DateTime; Change: Change}

    let private getChange closeDayN closeDayNminus1 = 
        //printfn "closeDayN: %M" closeDayN
        //printfn "closeDayNminus1: %M" closeDayNminus1
        if closeDayNminus1.Value < closeDayN.Value then {Date = closeDayN.Date; Change = Upward(closeDayN.Value - closeDayNminus1.Value) }
        elif closeDayNminus1.Value > closeDayN.Value then {Date = closeDayN.Date; Change = Downward(closeDayNminus1.Value - closeDayN.Value)}
        else  {Date = closeDayN.Date; Change = Equal}
           
    ///Calculates the changes in stockData
    let calculateChanges (stockData: seq<Quote>) = 
        [| 1..((stockData |> Seq.length) - 1)|] |> Array.map (fun i -> getChange (stockData |> Seq.item (i)) (stockData |> Seq.item (i-1)))
    let private getUpwardValue changeValue=
        match changeValue with
         | Upward value -> value
         | _ -> 0m    
    let private getDownwardValue changeValue=
        match changeValue with
         | Downward value -> value
         | _ -> 0m
    let private avgOfVlaues (predicate: Change -> decimal ) (changes: Change array) =
        changes |> Array.map predicate
                |> Array.average
    let private calculateRsi avgGain avgLoss =
        //TODO: dev. by zero!
      //  printfn "avgGain: %M" avgGain
      //  printfn "avgLoss: %M" avgLoss
        let rs = (avgGain / avgLoss)
       // printfn "RS: %M" rs
        (100m - (100m / (1m+rs)))
    
    let Rsi (stockData: seq<Quote>) rsiLength  = 
        let changes = calculateChanges (stockData)         
        let rec rsiHelper (changes: DateWithChange array) rsiLength (res: Quote list) (lastAvgGain: decimal) (lastAvgLoss: decimal) =
           match (changes |> Array.length) with 
               | n when n < rsiLength -> res |> List.toSeq
               | _ ->  
                       // printfn "rsihelper"
                        let currentGain = getUpwardValue (changes |> Array.item (rsiLength-1)).Change;
                        let avgGain = ((lastAvgGain * ((decimal)rsiLength - 1m)) + currentGain )/(decimal)rsiLength
                        let currentLoss = getDownwardValue (changes |> Array.item (rsiLength-1)).Change;
                        let avgLoss = ((lastAvgLoss * ((decimal)rsiLength - 1m)) + currentLoss )/(decimal)rsiLength
                        let newrsiValue = calculateRsi avgGain avgLoss
                        let newRsi = { Value = newrsiValue; Date = (changes |> Array.item ((rsiLength-1)) ).Date  }
                        printfn "newitem: %M" newrsiValue
                        rsiHelper (changes |> Array.tail) rsiLength (res @ [newRsi]) avgGain avgLoss   
                    // let changesInRange = (changes |> Array.take rsiLength)
                    // printfn "ChangeisinRangel %A" changesInRange
                    // let changeValuesinRange = changesInRange |> Array.map (fun f -> f.Change)
                    // let avgGain = avgOfVlaues getUpwardValue changeValuesinRange
                    // printfn "AvgGain %M" avgGain
                    // let avgLoss = avgOfVlaues getDownwardValue changeValuesinRange
                    // printfn "avgLoss %M" avgLoss
                    // let newRsi = {Value = calculateRsi avgGain avgLoss; Date = (changesInRange |> Array.last).Date  }
                    // rsiHelper (changes |> Array.tail) rsiLength (res @ [newRsi])   
        let changesInRange = (changes |> Array.take rsiLength)
        //printfn "ChangeisinRangel %A" changesInRange
        let changeValuesinRange = changesInRange |> Array.map (fun f -> f.Change)
        let avgGain = avgOfVlaues getUpwardValue changeValuesinRange
        //printfn "AvgGain %M" avgGain
        let avgLoss = avgOfVlaues getDownwardValue changeValuesinRange
      //  printfn "avgLoss %M" avgLoss
        let newRsi = {Value = calculateRsi avgGain avgLoss; Date = (changesInRange |> Array.last).Date  }
        rsiHelper (changes |> Array.tail) rsiLength [newRsi] avgGain avgLoss                      
        rsiHelper changes rsiLength [] 

    //         match counter with
    //           | 1 ->                                  
    //               let firstAvarageGain = Array.sub changes 0 rsiLength
    //                                       |> avgOfVlaues getUpwardValue
    //               let firstAvarageLoss = Array.sub changes 0 rsiLength
    //                                       |> avgOfVlaues getDownwardValue                
    //               let rsi = calculateRsi firstAvarageGain firstAvarageLoss                 
    //               let newRsiItem = { Date = stockData.[(counter + rsiLength - 1)].Date; Value = rsi } 
    //               rsiHelper stockData rsiLength (counter + 1) (newRsiItem::res) firstAvarageGain firstAvarageLoss
    //           | counter when counter = (stockData.Length - rsiLength + 1) ->       
    //               res |> List.rev |> List.toSeq
    //           | _ ->    
    //               let currentAvgGain = (((prevAvgGain * 13m) + (getUpwardValue changes.[counter + rsiLength - 2])) / 14m)
    //               let currentAvgLoss = (((prevAvgLoss * 13m) + (getDownwardValue changes.[counter + rsiLength - 2])) / 14m)
    //               let rsi = calculateRsi currentAvgGain currentAvgLoss
    //               let newRsiItem = { Date = stockData.[(counter + rsiLength - 1)].Date; Value = rsi } 
    //               rsiHelper stockData rsiLength (counter + 1) (newRsiItem::res) currentAvgGain currentAvgLoss
    //     rsiHelper (Seq.toArray stockData) rsiLength 1 [ ] 0m 0m  
   
    //Buys when RSI is equal or below 30 and sells when RSI is equal or over 70
    //rsiData and price has to be the same size and at every position contain the data for the same day
    let BackTestRsiWithPrice (rsiData: seq<Quote>) (price: seq<Quote>) =        
        let rec backTestRsiWithPriceHelper (rsiData: Quote list) (price: Quote array) (lastOder: Order) (result: TransactionQuote list) lastBuyPrice = 
            match rsiData with
            | [] -> let sumResult = CalculateNetResult result
                    {Transactions = result; ResultInPercent = sumResult }
            | head :: tail ->  
                match head with
                | head when head.Value < 30m && (lastOder|> isSell) -> //Buy
                    let currentQuote = Array.head price
                    let buyItem = Buy { Date = currentQuote.Date; Value = currentQuote.Value }
                    backTestRsiWithPriceHelper tail (Array.tail price) Order.Buy (buyItem::result) currentQuote.Value
                | head when head.Value > 70m && (lastOder|> isBuy) -> //Sell
                    let currentQuote = Array.head price
                    let sellItem = Sell ( {Date = currentQuote.Date; Value = currentQuote.Value}, (((double((currentQuote.Value / lastBuyPrice ))) - 1.0) * 100.0) )
                    backTestRsiWithPriceHelper tail (Array.tail price) Order.Sell (sellItem::result) currentQuote.Value
                | _ ->  backTestRsiWithPriceHelper tail (Array.tail price) lastOder result lastBuyPrice //No action, just skip
        backTestRsiWithPriceHelper (List.ofSeq rsiData) (Seq.toArray price) Order.Sell [] 0m