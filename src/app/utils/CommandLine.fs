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
    //match federation
    | "-f"::xs ->
      let newOptsAccum = { optsAccum with federation=true }
      parseCmdLineRec xs newOptsAccum
    //match space report
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
      federation = false;
      spaceReport = false;
    }

    parseCmdLineRec args defaultOptions