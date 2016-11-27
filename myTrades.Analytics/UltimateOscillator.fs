namespace MyTrades.Analytics

module UltimateOscillator =

    let BuyingPressure (prev: OHCL) (current: OHCL) =
        (current.Close - (min current.Low prev.Close))

    let TrueRange (prev: OHCL) (current: OHCL) =
        (max current.High prev.Close) - (min current.Low prev.Close)

    let Avg length start data =         
         (List.average (data |> List.skip start))

    let UltimateOscillator (prices: seq<OHCLWithDate>) =
        //current: 2. element  -> make sure it runs until count - 2
        //first: 1. element 
        let dataSizeM2 = (prices |> Seq.length) - 2
        let rec ultimateOscillatorHelper (prices: seq<OHCLWithDate>) counter result bps trs bptpSize shortAvgs midAvgs longAvgs =
            let current = (Seq.head (prices |> Seq.tail)).OHCL
            let prev = (prices |> Seq.head).OHCL
            let bP = BuyingPressure prev current
            let tR = TrueRange prev current
            match dataSizeM2 with 
            | n when counter = dataSizeM2 ->
                result               
            | _ ->
                match bptpSize with 
                | n when (n > 28) -> 
                    let newLAvg = (Avg 28 counter bps) / (Avg 28 counter trs)
                    let newMAvg =  (Avg 14 counter bps) / (Avg 14 counter trs)
                    let newSAvg = (Avg 7 counter bps) / (Avg 7 counter trs)
                    ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) (newSAvg::shortAvgs) (newMAvg::midAvgs) (newLAvg::longAvgs)
                | n when (n > 14) -> 
                    let newMAvg =  (Avg 14 counter bps) / (Avg 14 counter trs)
                    let newSAvg = (Avg 7 counter bps) / (Avg 7 counter trs)
                    ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) (newSAvg::shortAvgs) (newMAvg::midAvgs) longAvgs
                | n when (n > 7) -> 
                    let newSAvg = (Avg 7 counter bps) / (Avg 7 counter trs)
                    ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) (newSAvg::shortAvgs) midAvgs longAvgs 
                | _ -> ultimateOscillatorHelper (prices |> Seq.tail) (counter+1) result (bP::bps) (tR::trs) (bptpSize+1) shortAvgs midAvgs longAvgs 
        ultimateOscillatorHelper prices 0 [] [] [] 0 [] [] []
