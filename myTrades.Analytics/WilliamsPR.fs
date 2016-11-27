namespace MyTrades.Analytics

//Williams %R
module WilliamsPR =

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