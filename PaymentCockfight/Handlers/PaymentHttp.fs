namespace Payments.Http

open Giraffe
open Microsoft.AspNetCore.Http

module PaymentHttp =
    let handlers : HttpFunc -> HttpContext -> HttpFuncResult =
        choose [
            POST >=> route "/payments" >=>
                fun next context ->
                    text "Create" next context
            
            GET >=> route "/payments-summary" >=>
                fun next context ->
                    text "Read" next context
        ]
