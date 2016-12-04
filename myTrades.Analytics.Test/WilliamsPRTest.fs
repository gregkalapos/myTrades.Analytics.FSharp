module Tests.Willams

open System
open Xunit
open MyTrades.Analytics
open TestData
open WilliamsPR
open Xunit.Abstractions;

    type WilliamsTest(output:ITestOutputHelper) =        
        [<Fact>]
        member __.``Calculate William %R for BMW`` () =     
            let prices = GetBmwOhcl;
            let williams = WilliamsPR prices 14
            
            //TODO: write test
            0
       

