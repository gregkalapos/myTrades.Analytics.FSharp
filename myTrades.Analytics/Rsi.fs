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
       if avgLoss = Decimal.Zero then 100m
       else if avgGain = Decimal.Zero then 0m
       else  
            let rs = (avgGain / avgLoss)
            (100m - (100m / (1m+rs)))

    let Rsi (stockData: seq<Quote>) rsiLength  = 
        let changes = calculateChanges (stockData)         
        let rec rsiHelper (changes: DateWithChange array) rsiLength (res: Quote list) (lastAvgGain: decimal) (lastAvgLoss: decimal) =
           match (changes |> Array.length) with 
               | n when n < rsiLength -> (res |> List.toSeq)
               | _ ->   let currentGain = getUpwardValue (changes |> Array.item (rsiLength-1)).Change;
                        let avgGain = ((lastAvgGain * ((decimal)rsiLength - 1m)) + currentGain )/(decimal)rsiLength
                        let currentLoss = getDownwardValue (changes |> Array.item (rsiLength-1)).Change;
                        let avgLoss = ((lastAvgLoss * ((decimal)rsiLength - 1m)) + currentLoss )/(decimal)rsiLength
                        let newrsiValue = calculateRsi avgGain avgLoss
                        let newRsi = { Value = newrsiValue; Date = (changes |> Array.item ((rsiLength-1)) ).Date  }
                        rsiHelper (changes |> Array.tail) rsiLength (res @ [newRsi]) avgGain avgLoss     
        let changesInRange = (changes |> Array.take rsiLength)
        let changeValuesinRange = changesInRange |> Array.map (fun f -> f.Change)
        let avgGain = avgOfVlaues getUpwardValue changeValuesinRange
        let avgLoss = avgOfVlaues getDownwardValue changeValuesinRange
        let newRsi = {Value = calculateRsi avgGain avgLoss; Date = (changesInRange |> Array.last).Date  }
        rsiHelper (changes |> Array.tail) rsiLength [newRsi] avgGain avgLoss  

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