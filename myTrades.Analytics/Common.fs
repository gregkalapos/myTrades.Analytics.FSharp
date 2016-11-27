namespace MyTrades.Analytics

module Common =
    let CalculateNetResult (transactions: TransactionQuote list) = 
        transactions |> List.map(fun f -> match f with 
                                            | Sell (a,b) -> b
                                            | _  -> 0.0 )                                       
                     |> List.sum