module Handlers.PaymentsHandler

open Giraffe
open Microsoft.AspNetCore.Http


let PaymentsHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        // let foobar = ctx.GetService<IPaymentProcessor>()
        text "a" next ctx
