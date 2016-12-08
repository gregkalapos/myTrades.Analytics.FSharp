module Tests.Willams

    open System
    open Xunit
    open MyTrades.Analytics
    open TestData
    open WilliamsPR
    open Xunit.Abstractions;
 
    [<Fact>]
    let ``Calculate William %R  Test`` () =     
        let prices = GetSampleOhcl;
        let williams = WilliamsPR prices 14

        //printfn "%A" ((williams |> Seq.map(fun f -> f.Value)) |> Seq.toList)
        Assert.Equal(17, williams |> Seq.length)
        let williamsArray = Seq.toArray williams

        Assert.Equal(-29.457364341085271317829457360M, (williamsArray |> Array.item 0).Value);
        Assert.Equal(new DateTime(2010,3, 12), (williamsArray |> Array.item 0).Date);

        Assert.Equal(-32.299741602067183462532299740M, (williamsArray |> Array.item 1).Value);
        Assert.Equal(new DateTime(2010,3, 15), (williamsArray |> Array.item 1).Date);

        Assert.Equal(-10.852713178294573643410852710M, (williamsArray |> Array.item 2).Value);
        Assert.Equal(new DateTime(2010,3, 16), (williamsArray |> Array.item 2).Date);

        Assert.Equal(-34.108527131782945736434108530M, (williamsArray |> Array.item 3).Value);
        Assert.Equal(new DateTime(2010,3, 17), (williamsArray |> Array.item 3).Date);

        Assert.Equal(-18.087855297157622739018087860M, (williamsArray |> Array.item 4).Value);
        Assert.Equal(new DateTime(2010,3, 18), (williamsArray |> Array.item 4).Date);

        Assert.Equal(-35.400516795865633074935400520M, (williamsArray |> Array.item 5).Value);
        Assert.Equal(new DateTime(2010,3, 19), (williamsArray |> Array.item 5).Date);

        Assert.Equal(-25.336927223719676549865229110M, (williamsArray |> Array.item 6).Value);
        Assert.Equal(new DateTime(2010,3, 22), (williamsArray |> Array.item 6).Date);


        Assert.Equal(-1.4251781472684085510688836100M, (williamsArray |> Array.item 7).Value);
        Assert.Equal(new DateTime(2010,3, 23), (williamsArray |> Array.item 7).Date);

        Assert.Equal(-30.021141649048625792811839320M, (williamsArray |> Array.item 8).Value);
        Assert.Equal(new DateTime(2010,3, 24), (williamsArray |> Array.item 8).Date);

        Assert.Equal(-26.909090909090909090909090910M, (williamsArray |> Array.item 9).Value);
        Assert.Equal(new DateTime(2010,3, 25), (williamsArray |> Array.item 9).Date);

        Assert.Equal(-26.545454545454545454545454550M, (williamsArray |> Array.item 10).Value);
        Assert.Equal(new DateTime(2010,3, 26), (williamsArray |> Array.item 10).Date);

        Assert.Equal(-38.797814207650273224043715850M, (williamsArray |> Array.item 11).Value);
        Assert.Equal(new DateTime(2010,3, 29), (williamsArray |> Array.item 11).Date);

        Assert.Equal(-39.078156312625250501002004010M, (williamsArray |> Array.item 12).Value);
        Assert.Equal(new DateTime(2010,3, 30), (williamsArray |> Array.item 12).Date);

        Assert.Equal(-59.420289855072463768115942030M, (williamsArray |> Array.item 13).Value);
        Assert.Equal(new DateTime(2010,3, 31), (williamsArray |> Array.item 13).Date);

        Assert.Equal(-59.420289855072463768115942030M, (williamsArray |> Array.item 14).Value);
        Assert.Equal(new DateTime(2010,4, 1), (williamsArray |> Array.item 14).Date);

        Assert.Equal(-33.091787439613526570048309180M, (williamsArray |> Array.item 15).Value);
        Assert.Equal(new DateTime(2010,4, 5), (williamsArray |> Array.item 15).Date);

        Assert.Equal( -43.236714975845410628019323670M, (williamsArray |> Array.item 16).Value);
        Assert.Equal(new DateTime(2010,4, 6), (williamsArray |> Array.item 16).Date);
        0
            
    
       

