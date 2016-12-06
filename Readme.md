# myTrades.Analytics
## A .NET library for technical analysis

myTrades.Analytics is an open source .NET library targeting netstandard1.6. 
The library provides [technical analysis](https://en.wikipedia.org/wiki/Technical_analysis "Wikipedia article about technical analysis") for financial instruments.
It is written in F#, but can be consumed from any .NET compatible programming language.  

## API Overview

Every indicator gets one of these types as a paramter:
* `seq<Quote>` (If it needs a Value and a Date for the calculation)
* `seq<OHCLWithDate>` (If it needs a date and one of the OHCL Values for calculation)

And returns:
* `seq<Quote>` which contains the date and the value for the indicator

Currently implemented indicators:
* Simple Moving Avg
* RSI
* William's %R
* Ultimate Oscillator (wip)

These are the types passed and returnd to/from the functions:

```Fsharp
type Quote = { Value: decimal; Date: System.DateTime }     
type OHCL = { Open: decimal; High: decimal; Close: decimal; Low: decimal } 
type OHCLWithDate = { OHCL: OHCL; Date: System.DateTime }
```

#### Calculate SMA

```Fsharp
open System 
open MyTrades.Analytics 
open MovingAvarage 
 
[<EntryPoint>] 
let main argv = 
        let prices = [ {Value = 3m  ; Date =new DateTime(2015, 03, 1)}; 
                       {Value = 4m  ; Date =new DateTime(2015, 03, 2)}; 
                       {Value = 5m  ; Date =new DateTime(2015, 03, 3)}; 
                       {Value = 6m  ; Date =new DateTime(2015, 03, 4)}; 
                       {Value = 7m  ; Date =new DateTime(2015, 03, 5)}; 
                       {Value = 6m  ; Date =new DateTime(2015, 03, 6)}; 
                       {Value = 5m  ; Date =new DateTime(2015, 03, 7)}; 
                       {Value = 9m  ; Date =new DateTime(2015, 03, 8)} ]; 
 
        let smas = SimpleMovingAvarage prices 5
```

#### Calculate RSI
```Fsharp
open System 
open MyTrades.Analytics 
open Rsi 
 
[<EntryPoint>] 
let main argv = 
        let prices  = …//populate prices seq<Quote> 
        let rsi = Rsi.Rsi prices 14; 
```

### Calculate William %R
```Fsharp
open System 
open MyTrades.Analytics 
open WilliamsPR 
 
[<EntryPoint>] 
let main argv = 
        let prices  = …//populate prices seq<Quote> 
        let williams = WilliamsPR prices 14
```

## Backtesting

The point of technical analysis is to give buy and sell signals. The back-testing functions in myTrades.Analytics basically looks for buy and sell signals and calculate the return of the transactions created based on the signals on the input data, which is typically a historical data for a given stock. 

The return type of every back-testing function is: 

```Fsharp
type BacktestingResult = { Transactions: seq<TransactionQuote>; ResultInPercent: double } 
```

where TransactionQuote is:
 
```Fsharp
type TransactionQuote = 
    | Buy of Quote 
    | Sell of Quote * double
``` 

Meaning the ResultInPercent stores the overall gain or loss and the the second item of the tuple in the TransactionQuote in Sell case stores the result of every sell transaction. 

### Backtesting RSI

```Fsharp
open System  
open MyTrades.Analytics 
open MyTrades.Analytics.TestData 
open Xunit.Abstractions 
 
[<EntryPoint>] 
let main argv = 
    let prices = GetBmwQuotes //Get historical data  
    let rsiData = Rsi prices 14 
    let rsiBackTesingResult = BackTestRsiWithPrice rsiData (prices |> Seq.skip 14)
``` 


Similar there are back-testing methods for the other indicators with the same parameters and the same return type. 
* BackTestMovingAvarageWithPrice: Buys when SMA is in down trend and Price goes over it from below. 
* BackTestWilliamsPr: Buys when Williams %R reaches -100 and after 5 days it is still below -85 and sells when it reaches 0 and after 5 days it is still above -15. (This is till WIP) 
* BackTestRsiWithPrice: (as already discussed) buys when the RSI is below 30 and sells when it reaches 70

## Repository structure
* [myTrades.Analytics:](../myTrades.Analytics) The library itself.   
* [myTrades.Analytics.Test:](../myTrades.Analytics.Test) Test code based on XUnit.  
* [myTrades.Analytics.Playground:](../myTrades.Analytics.Playground) This is a console application project in order to play with the library. During development it is very useful since currently there is no way to debug under CoreCLR with F# and XUnit cannot do printf output either (see https://github.com/xunit/xunit/issues/718 ). 