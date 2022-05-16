open app.utils.CmdLine
open ifc_snippets.Federation
module Program =

  [<EntryPoint>]
  let main args =
    printfn "Application is started."
    
    let params = parseCmdLine (args |> Array.toList)
    if params.federation then createFederation
    
    0
