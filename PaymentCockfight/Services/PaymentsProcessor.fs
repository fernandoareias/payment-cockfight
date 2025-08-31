module PaymentCockfight.Services

open PaymentCockfight.Models.Payment

type PaymentsProcessor() =
    member _.Execute (x: Payment) =
        // Try to insert in kafka
        
        
        // If dont success, insert in mongodb
        0
        