namespace MyTrades.Analytics

open MyTrades.Analytics.Common

module Rsi =        
    type Change =
        | Upward of change: decimal
        | Downward of change: decimal
        | Equal    
    let private getChange closeDayNm1 closeDayN = 
        if closeDayNm1 < closeDayN then Upward((closeDayN - closeDayNm1))
        elif closeDayNm1 > closeDayN then Downward(closeDayNm1 - closeDayN)
        else Equal
           
    ///Calculates the changes in stockData
    let private calculateChanges (stockData: Quote array) = 
        [| 1..((Array.length stockData) - 1)|] |> Array.map (fun i -> getChange stockData.[(i - 1)].Value stockData.[i].Value)
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
        let rs = (avgGain / avgLoss)
        (100m - (100m / (1m+rs)))

    let Rsi (stockData: seq<Quote>) rsiLength  = 
        let changes = calculateChanges (Seq.toArray stockData)
        let rec rsiHelper (stockData: Quote array) rsiLength counter (res: Quote list) prevAvgGain prevAvgLoss =            
            match counter with
              | 1 ->                                  
                  let firstAvarageGain = Array.sub changes 0 rsiLength
                                          |> avgOfVlaues getUpwardValue
                  let firstAvarageLoss = Array.sub changes 0 rsiLength
                                          |> avgOfVlaues getDownwardValue                
                  let rsi = calculateRsi firstAvarageGain firstAvarageLoss                 
                  let newRsiItem = { Date = stockData.[(counter + rsiLength - 1)].Date; Value = rsi } 
                  rsiHelper stockData rsiLength (counter + 1) (newRsiItem::res) firstAvarageGain firstAvarageLoss
              | counter when counter = (stockData.Length - rsiLength + 1) ->       
                  res |> List.rev |> List.toSeq
              | _ ->    
                  let currentAvgGain = (((prevAvgGain * 13m) + (getUpwardValue changes.[counter + rsiLength - 2])) / 14m)
                  let currentAvgLoss = (((prevAvgLoss * 13m) + (getDownwardValue changes.[counter + rsiLength - 2])) / 14m)
                  let rsi = calculateRsi currentAvgGain currentAvgLoss
                  let newRsiItem = { Date = stockData.[(counter + rsiLength - 1)].Date; Value = rsi } 
                  rsiHelper stockData rsiLength (counter + 1) (newRsiItem::res) currentAvgGain currentAvgLoss
        rsiHelper (Seq.toArray stockData) rsiLength 1 [ ] 0m 0m  
    let isBuy iOrder =
        match iOrder with
        | Order.Buy -> true
        | _ -> false
    let isSell iOrder= 
        not (isBuy iOrder)

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