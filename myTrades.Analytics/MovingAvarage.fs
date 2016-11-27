namespace MyTrades.Analytics

open MyTrades.Analytics.Common

module MovingAvarage =    
   
    let private quoteArrayToString (arrayToConvert: Quote array) =
        let str = new System.Text.StringBuilder()
        let c = str.AppendLine("Array to print: ")
        let rec quotaArrayToStringInner (arrayToConvert: Quote array) (counter: int) =
            match arrayToConvert with
            | [| |] -> str
            | _ -> let b = str.AppendLine(counter.ToString() + ": " + arrayToConvert.[0].Date.ToString() + " " +  arrayToConvert.[0].Value.ToString()) 
                   quotaArrayToStringInner (Array.sub arrayToConvert 1 (arrayToConvert.Length - 1)) (counter + 1)
        quotaArrayToStringInner arrayToConvert 0

    let private printArray (arrayToPrint: Quote array) =
        System.Diagnostics.Debug.WriteLine (quoteArrayToString arrayToPrint)// (String.init ("Array to calculate %A" arayToPring))
        arrayToPrint 
    
    ///Gives the simlpe moving avarage for the stockData. e.g. if it contains 5 items 
    ///then the moving avg is for all the 5.
    let private simpleMovingAvarageSingle (stockData: seq<Quote>) =        
          stockData          
          |> Seq.map(fun f -> f.Value)
          |> Seq.average
          
    let SimpleMovingAvarage (stockData: seq<Quote>) forDays =     
        let firstIt = (Seq.length stockData) - forDays
        let rec simpleMovingAvarageHelper (stockData: Quote array) forDays counter (mvas: Quote list) =  
            match counter with
            | -1 ->
             List.toSeq mvas             
            | c when c = firstIt -> 
             let newAvg = simpleMovingAvarageSingle (stockData |> Seq.skip counter |> Seq.take forDays)   
             simpleMovingAvarageHelper stockData 
                    forDays (counter - 1) ({ Date = stockData.[(counter + forDays - 1)].Date; Value = newAvg} :: mvas) 
            | _ -> 
             let str = new System.Text.StringBuilder()
             let newAvg = mvas.Head.Value - (stockData.[counter+forDays].Value/decimal(forDays)) 
                            + (stockData.[counter].Value/decimal(forDays))            
             simpleMovingAvarageHelper stockData 
                forDays (counter - 1) ({ Date = stockData.[(counter + forDays - 1)].Date; Value = newAvg} :: mvas)                            
        simpleMovingAvarageHelper (Seq.toArray stockData) forDays firstIt []

    let GetTrendForData (stockData: seq<Quote>) =         
        let rec getTrendForDataHelper (stockData: Quote array) counter (trend: DateWithTrendDirection list) =
            match counter with
            | 0 -> List.toSeq trend
            | _ -> match (stockData.[(counter - 1)].Value, stockData.[counter].Value) with
                   | (x,y) when x < y -> getTrendForDataHelper stockData (counter - 1) ( (stockData.[counter].Date, TrendDirection.Up ) :: trend)
                   | (x,y) when x > y -> getTrendForDataHelper stockData (counter - 1) ( (stockData.[counter].Date, TrendDirection.Down ) :: trend)
                   | _ -> getTrendForDataHelper stockData (counter - 1) ( (stockData.[counter].Date, TrendDirection.Side ) :: trend)
        getTrendForDataHelper (Seq.toArray stockData) ((Seq.length stockData) - 1) []
            
    //cross over: buy when MA is down trend and Price goes over it from below 
    let BackTestMovingAvarageWithPrice (movingAvarageWithTrend: seq<QuoteWithDirection>) (price: Quote array) =       
        let rec backTestMovingAvarageWithPriceHelper counter (movingAvarageWithTrend: seq<QuoteWithDirection>) (price: Quote array) (result: TransactionQuote list) lastOrder lastBuyPrice =
            System.Diagnostics.Debug.WriteLine(price.[counter].Date.ToString());
            match counter with
            | c when c = ((Seq.length movingAvarageWithTrend) - 1) ->
                       let sumResult = CalculateNetResult result
                       {Transactions = result; ResultInPercent = sumResult}
            | _ -> match ( (fst (Seq.item (counter - 1)  movingAvarageWithTrend)).Value, (snd (Seq.item (counter - 1) movingAvarageWithTrend)), (price.[(counter - 1)]).Value, 
                           (fst (Seq.item (counter) movingAvarageWithTrend)).Value, (price.[counter]).Value, lastOrder ) with
                    | (maBofore, trendBefore, 
                       priceBofore, maCurrent, 
                       priceCurrent, lastOrder) 
                       when 
                        (maBofore > priceBofore && 
                             match trendBefore with
                                   |Down -> true
                                   |_ -> false 
                         && 
                             match lastOrder with
                                  | Order.Sell -> true
                                  | Order.Buy -> false                         
                        &&
                         priceCurrent > maCurrent) -> System.Diagnostics.Debug.WriteLine("Buy");
                                                      let buyItem = Buy {Date = price.[counter].Date; Value = price.[counter].Value }
                                                      backTestMovingAvarageWithPriceHelper (counter + 1) movingAvarageWithTrend price (buyItem::result) Order.Buy price.[counter].Value 
                    | (maBofore, trendBefore, 
                       priceBofore, maCurrent, 
                       priceCurrent, lastOrder) 
                       when 
                        (maBofore < priceBofore &&
                             match trendBefore with
                                   |Up -> true
                                   |_ -> false 
                         &&   
                            match lastOrder with
                                  | Order.Sell -> false
                                  | Order.Buy -> true     
                        &&
                         priceCurrent < maCurrent) -> System.Diagnostics.Debug.WriteLine("Sell");
                                                      let gain = (((double((price.[counter].Value / lastBuyPrice ))) - 1.0) * 100.0 )
                                                      let sellItem = Sell ({ Date = price.[counter].Date ; Value = price.[counter].Value } ,  gain)
                                                      backTestMovingAvarageWithPriceHelper (counter + 1) movingAvarageWithTrend price (sellItem::result) Order.Sell 0m                        
                    | _ -> backTestMovingAvarageWithPriceHelper (counter + 1) movingAvarageWithTrend price result lastOrder lastBuyPrice
        backTestMovingAvarageWithPriceHelper (1) movingAvarageWithTrend price [] Order.Sell 0m