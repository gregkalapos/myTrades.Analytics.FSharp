namespace MyTrades.Analytics

    type Quote = { Value: decimal; Date: System.DateTime }    
    type OHCL = { Open: decimal; High: decimal; Close: decimal; Low: decimal }
    type OHCLWithDate = { OHCL: OHCL; Date: System.DateTime }

    type Order =        
         |Sell
         |Buy 
      
    type TransactionQuote =
        | Buy of Quote
        | Sell of Quote * double 
    
    type BacktestingResult = { Transactions: seq<TransactionQuote>; ResultInPercent: double }

    type TrendDirection =
        | Up
        | Down
        | Side
    
    type DateWithTrendDirection =
        System.DateTime * TrendDirection
   
    type QuoteWithDirection =
        Quote * TrendDirection