namespace MyTrades.Analytics
open System

module UltimateOscillator =

    let private buyingPressure (prev: OHCL) (current: OHCL) =
        (current.Close - (min current.Low prev.Close))

    let private trueRange (prev: OHCL) (current: OHCL) =
        (max current.High prev.Close) - (min current.Low prev.Close)

    let Avg length start (data: decimal List) =
        //  printfn "entered Avg"
        //  printfn "list: %A" data
        //  printfn "start %d" start       
        //  printfn "length %d" length  
         let selectedData = data |> Seq.take length
         let retVal = (Seq.average selectedData)
        //  printfn "selectedData: %A" selectedData
        //  printfn "retVal: %A" retVal
        //  printfn ""
         retVal

    let UltimateOscillator (prices: seq<OHCLWithDate>) =
        let dataSizeM1 = (prices |> Seq.length) - 1
        let rec ultimateOscillatorHelper (prices: seq<OHCLWithDate>) counter (result: Quote list) (bps: decimal List) trs bptpSize shortAvgs midAvgs longAvgs =
            match dataSizeM1 with 
            | n when counter = dataSizeM1 ->
                result |> List.rev |> List.toSeq 
            | _ ->
                let current = (Seq.head (prices |> Seq.tail)).OHCL
                let prev = (prices |> Seq.head).OHCL
                let bP = buyingPressure prev current
                let tR = trueRange prev current
                match (bptpSize+1) with 
                | n when (n >= 28) ->
                    let newLAvg = (Avg 28 (counter - 27) (bP::bps)) / (Avg 28 (counter - 27) (tR::trs))
                    let newMAvg = (Avg 14 (counter - 13) (bP::bps)) / (Avg 14 (counter - 13) (tR::trs))
                    let newSAvg = (Avg 7 (counter - 6) (bP::bps)) / (Avg 7 (counter - 6) (tR::trs))
                    let newResItem = 100m * ((4m * newSAvg)+(2m * newMAvg)+newLAvg)/(4m+2m+1m) 
                    let newVal = {Value = newResItem; Date= (Seq.head (prices |> Seq.tail)).Date }
                    ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) (newVal::result) (bP::bps) (tR::trs) (bptpSize+1) (newSAvg::shortAvgs) (newMAvg::midAvgs) (newLAvg::longAvgs)
                | n when (n >= 14) ->                   
                    let newMAvg = (Avg 14 (counter - 13) (bP::bps)) / (Avg 14 (counter - 13) (tR::trs))
                    let newSAvg = (Avg 7 (counter - 6) (bP::bps)) / (Avg 7 (counter - 6) (tR::trs))
                    ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) (newSAvg::shortAvgs) (newMAvg::midAvgs) longAvgs
                | n when (n >= 7) -> 
                    let newSAvg = (Avg 7 (counter - 6) (bP::bps)) / (Avg 7 (counter - 6) (tR::trs))                   
                    ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) (newSAvg::shortAvgs) midAvgs longAvgs                    
                | _ -> ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) shortAvgs midAvgs longAvgs 
        ultimateOscillatorHelper prices 0 [] [] [] 0 [] [] []
