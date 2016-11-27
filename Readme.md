# myTrades.Analytics
## A .NET library for technical analysis


API Overview

Every indicator gets one of these types as a paramter:
* `seq<Quote>` (If it needs a Value and a Date for the calculation)
* `seq<OHCLWithDate>` (If it needs a date and one of the OHCL Values for calculation)

And returns:
* `seq<Quote>` which contains the date and the value for the indicator


Currently implemented indicators:
* Simple Moving Avg
* RSI
* William's %R