module Handlers.PaymentsHandler

open Giraffe
open Microsoft.AspNetCore.Http
open PaymentCockfight.Models.Payment
open PaymentCockfight.Services

let PaymentsHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let processor = ctx.GetService<PaymentsProcessor>()
            let! payment = ctx.BindJsonAsync<Payment>()
            let x = processor.Execute payment |> string
            return! text x next ctx
        }

