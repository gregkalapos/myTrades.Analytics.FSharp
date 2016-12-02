namespace MyTrades.Analytics

module Common =

    ///Returns the net result of a transaction list
    let CalculateNetResult (transactions: TransactionQuote list) = 
        transactions |> List.map(fun f -> match f with 
                                            | Sell (a,b) -> b
                                            | _  -> 0.0 )                                       
                     |> List.sum

    ///Returns the Value from a TransactionQuote
    let GetTransactionValue (transaction: TransactionQuote) =
        match transaction with 
        | Sell (q, v) -> q.Value
        | Buy q -> q.Value

   
    let isBuy iOrder =
        match iOrder with
        | Order.Buy -> true
        | _ -> false
    let isSell iOrder= 
        not (isBuy iOrder)