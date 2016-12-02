namespace MyTrades.Analytics

//Williams %R
module WilliamsPR =

    open Common;

    //%R = (Highest High - Close)/(Highest High - Lowest Low) * -100
    let private singleWilliamsPr (prices: seq<OHCLWithDate>) =
            let closingQuote = (prices |> Seq.last)            
            let highestHigh = (prices |> Seq.maxBy ( fun x -> x.OHCL.High)).OHCL.High            
            let lowestLow = (prices |> Seq.minBy (fun x -> x.OHCL.Low)).OHCL.Low   
            { Date = closingQuote.Date ; Value = (((highestHigh - closingQuote.OHCL.Close)/(highestHigh - lowestLow)) * (-100m))}

    let WilliamsPR (prices: seq<OHCLWithDate>) nDays =
        let pricesLastIndex = ((prices |> Seq.length) - nDays )       
        let rec williamsPRHelper (prices: seq<OHCLWithDate>) nDays  counter (result: Quote list) =
            let subPrices = prices |> Seq.skip counter |> Seq.take (nDays)
            let nBefore = subPrices |> Seq.take nDays            
            let newVal = singleWilliamsPr nBefore 
            match counter with
            | counter when counter = pricesLastIndex -> ((newVal::result) |> List.rev |> List.toSeq)
            | _ -> williamsPRHelper prices nDays  (counter + 1) (newVal::result)
        williamsPRHelper prices nDays  0 []

    let BackTestWilliamsPr (williamsValues: seq<Quote>) (price: seq<Quote>) =
        let rec backtestingWilliamsPrInner (williamsValues: Quote list) (price: Quote list) (result: TransactionQuote list) (lastOder: Order) =
            match williamsValues with
            | [] -> { Transactions = result; ResultInPercent=(CalculateNetResult result)} 
            | head::t1::t2::t3::t4::t5::tRest when head.Value = 0m && t5.Value > 15m  && lastOder |> isBuy -> 
                //sell
                let lastBuy = result |> List.last
                let cPrice = List.head price
                let profit =  (((double((cPrice.Value / (lastBuy |> GetTransactionValue) ))) - 1.0) * 100.0)
                backtestingWilliamsPrInner (t1::t2::t3::t4::t5::tRest) (List.tail price) (Sell ({Value = cPrice.Value; Date = cPrice.Date}, profit)::result) Order.Sell
            | head::t1::t2::t3::t4::t5::tRest when head.Value = -100m && t5.Value < -95m && lastOder |> isSell ->
                //buy
                let cPrice = List.head price
                backtestingWilliamsPrInner (t1::t2::t3::t4::t5::tRest) (List.tail price) (Buy {Value = cPrice.Value; Date = cPrice.Date}::result) Order.Buy               
            | _ -> 
                ///just skip
                backtestingWilliamsPrInner (List.tail williamsValues) (List.tail price) result lastOder
        backtestingWilliamsPrInner (Seq.toList williamsValues) (Seq.toList price) [] Order.Sell
    