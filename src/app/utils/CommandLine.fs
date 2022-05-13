namespace app.utils
open CmdLineTypes

module CmdLine =
  let rec parseCmdLineRec args optsAccum =
    match args with
    //empty list, parsing is done
    | [] ->
      optsAccum
    //match wallCreation
    | "-w"::xs ->
      let newOptsAccum = { optsAccum with wallCreation=true }
      parseCmdLineRec xs newOptsAccum
    //match hierarchy
    | "-h"::xs ->
      let newOptsAccum = { optsAccum with hierarchy=true }
      parseCmdLineRec xs newOptsAccum
    //match hierarchy
    | "-f"::xs ->
      let newOptsAccum = { optsAccum with federation=true }
      parseCmdLineRec xs newOptsAccum
    //match hierarchy
    | "-s"::xs ->
      let newOptsAccum = { optsAccum with spaceReport=true }
      parseCmdLineRec xs newOptsAccum
    // handle unrecognized option and keep looping
    | x::xs ->
      printfn $"Option '%s{x}' is unrecognized"
      parseCmdLineRec xs optsAccum

  let parseCmdLine args =
    // defaults
    let defaultOptions = {
      wallCreation = false;
      hierarchy = false;
      federation = false;
      spaceReport = false;
    }

    parseCmdLineRec args defaultOptions