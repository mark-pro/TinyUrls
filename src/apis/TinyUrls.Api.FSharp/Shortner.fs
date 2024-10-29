module TinyUrls.Api.FSharp.Shortner

open System
open System.Collections.Generic
open TinyUrls.Prelude.Shortner
open TinyUrls.Types

type ShortnerConfig =
    { Alphabet: ReadOnlyMemory<char>
      MaxLength: uint8 }
    interface IShortnerConfig with
        member this.Alphabet = HashSet(this.Alphabet.ToArray())
        member this.MaxLength = this.MaxLength |> uint16
    
let defaultConfig = { Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".AsMemory()
                      MaxLength = 7uy }

let createShortCode (config: IShortnerConfig) (url: Uri) =
    let alphabet = config.Alphabet |> Array.ofSeq
    let rec create (acc: string) (idx: uint16) =
        match idx with
        | 0us -> acc
        | _ -> create (alphabet.[Random.Shared.Next(alphabet.Length)] |> string |> (+) acc) (idx - 1us)
    TinyUrl.Create(ShortCode.FromString (create "" config.MaxLength), url)
