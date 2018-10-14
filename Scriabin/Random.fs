module Scriabin.Random

open System

let rand = new Random()

let randDiscrimatedUnion<'T> () = 
  let cases = Reflection.FSharpType.GetUnionCases(typeof<'T>)
  let index = rand.Next(cases.Length)
  let case = cases.[index]
  Reflection.FSharpValue.MakeUnion(case, [||]) :?> 'T