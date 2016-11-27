module Tests

open System
open Xunit
open MyTrades.Analytics

[<Fact>]
let ``Tet With 10 Values`` () =
    let prices = [ {Value = 3m  ; Date =new DateTime(2015, 03, 1)};
                   {Value = 4m  ; Date =new DateTime(2015, 03, 2)};
                   {Value = 5m  ; Date =new DateTime(2015, 03, 3)};
                   {Value = 6m  ; Date =new DateTime(2015, 03, 4)};
                   {Value = 7m  ; Date =new DateTime(2015, 03, 5)};
                   {Value = 6m  ; Date =new DateTime(2015, 03, 6)};
                   {Value = 5m  ; Date =new DateTime(2015, 03, 7)};
                   {Value = 9m  ; Date =new DateTime(2015, 03, 8)} ];

    let mavgs = MyTrades.Analytics.MovingAvarage.SimpleMovingAvarage prices 5

    Assert.Equal(4, (Seq.length mavgs))
    let magvsArray = Seq.toArray mavgs

    Assert.Equal(5m, (magvsArray |> Array.item 0).Value);
    Assert.Equal(new DateTime(2015, 03, 5), (magvsArray |> Array.item 0).Date);

    Assert.Equal(5.6m, (magvsArray |> Array.item 1).Value);
    Assert.Equal(new DateTime(2015, 03, 6), (magvsArray |> Array.item 1).Date);

    Assert.Equal(5.8m, (magvsArray |> Array.item 2).Value);
    Assert.Equal(new DateTime(2015, 03, 7), (magvsArray |> Array.item 2).Date);

    Assert.Equal(6.6m, (magvsArray |> Array.item 3).Value);
    Assert.Equal(new DateTime(2015, 03, 8), (magvsArray |> Array.item 3).Date);