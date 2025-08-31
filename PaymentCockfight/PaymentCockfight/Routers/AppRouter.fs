module Routers.AppRouter

open Giraffe
open Handlers.PaymentsHandler

let webApp : HttpHandler =
    choose [
        GET >=> choose [
            route "/payments-summary"   >=> text "ok"
        ]
            
        POST >=> choose [
            route "/payments"           >=> PaymentsHandler
        ]
    ]