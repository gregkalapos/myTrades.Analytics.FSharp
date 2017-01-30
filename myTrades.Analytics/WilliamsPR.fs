namespace MyTrades.Analytics

//Williams %R
module WilliamsPR =

    open Common;
    open System;

    //%R = (Highest High - Close)/(Highest High - Lowest Low) * -100
    let private singleWilliamsPr (prices: seq<OHCLWithDate>) =
            let closingQuote = (prices |> Seq.last)            
            let highestHigh = (prices |> Seq.maxBy ( fun x -> x.OHCL.High)).OHCL.High            
            let lowestLow = (prices |> Seq.minBy (fun x -> x.OHCL.Low)).OHCL.Low   
            { Date = closingQuote.Date ; Value = (((highestHigh - closingQuote.OHCL.Close)/(highestHigh - lowestLow)) * (-100m))}

    let private FsAsyncWilliamsPR (prices: seq<OHCLWithDate>) nDays (cT: Threading.CancellationToken) =
        async
            {
                let pricesLastIndex = ((prices |> Seq.length) - nDays )       
                let rec williamsPRHelper (prices: seq<OHCLWithDate>) nDays  counter (result: Quote list) =
                    cT.ThrowIfCancellationRequested();
                    let subPrices = prices |> Seq.skip counter |> Seq.take (nDays)
                    let nBefore = subPrices |> Seq.take nDays            
                    let newVal = singleWilliamsPr nBefore 
                    match counter with
                    | counter when counter = pricesLastIndex -> ((newVal::result) |> List.rev |> List.toSeq)
                    | _ -> williamsPRHelper prices nDays  (counter + 1) (newVal::result)
                return williamsPRHelper prices nDays  0 []        
            }

    let WilliamsPR (prices: seq<OHCLWithDate>) nDays =
        (FsAsyncWilliamsPR prices nDays (new Threading.CancellationToken())) |> Async.RunSynchronously

    let WilliamsPRAsync (prices: seq<OHCLWithDate>) nDays cT =
        Async.StartAsTask(FsAsyncWilliamsPR prices nDays cT)
                
    let private FsAsyncBackTestWilliamsPrWithThresholds (williamsValues: seq<Quote>) (price: seq<Quote>) buyThreshold sellThreshold (cT: Threading.CancellationToken) =
        async
            {
               let rec backtestingWilliamsPrInner (williamsValues: Quote list) (price: Quote list) (result: TransactionQuote list) (lastOder: Order) =
                      cT.ThrowIfCancellationRequested()
                      match williamsValues with
                      | [] -> { Transactions = result; ResultInPercent=(CalculateNetResult result)} 
                      | //head::t1::t2::t3::t4::t5::tRest when head.Value = 0m && t5.Value > 15m  && lastOder |> isBuy -> 
                          head::t1::tRest when head.Value > sellThreshold &&  head.Value > t1.Value && t1.Value < sellThreshold && lastOder |> isBuy ->
                          //sell
                          let lastBuy = result |> List.head
                          let cPrice = List.head (List.tail price)
                          let profit =  (((double((cPrice.Value / (lastBuy |> GetTransactionValue) ))) - 1.0) * 100.0)
                          backtestingWilliamsPrInner (t1::tRest) (List.tail price) (Sell ({Value = cPrice.Value; Date = cPrice.Date}, profit)::result) Order.Sell
                      | //head::t1::t2::t3::t4::t5::tRest when head.Value = -100m && t5.Value < -95m && lastOder |> isSell ->
                          head::t1::tRest when head.Value < buyThreshold && head.Value < t1.Value && t1.Value > buyThreshold && lastOder |> isSell  ->
                          //buy
                          let cPrice = List.head (List.tail price)
                          backtestingWilliamsPrInner (t1::tRest) (List.tail price) (Buy {Value = cPrice.Value; Date = cPrice.Date}::result) Order.Buy               
                      | _ -> 
                          ///just skip
                          backtestingWilliamsPrInner (List.tail williamsValues) (List.tail price) result lastOder
               return backtestingWilliamsPrInner (Seq.toList williamsValues) (Seq.toList price) [] Order.Sell
           }

    let BackTestWilliamsPrWithThresholdsAsync (williamsValues: seq<Quote>) (price: seq<Quote>) buyThreshold sellThreshold (cT: Threading.CancellationToken) =
        Async.StartAsTask(FsAsyncBackTestWilliamsPrWithThresholds williamsValues price buyThreshold sellThreshold cT)

//    let BackTestWilliamsPr (williamsValues: seq<Quote>) (price: seq<Quote>) =
//         FsAsyncBackTestWilliamsPrWithThresholds williamsValues price -80m -20m 