open app.utils.CmdLine
open ifc_snippets.Federation
open ifc_snippets.Report
open ifc_snippets.Create
module Program =

  [<EntryPoint>]
  let main args =
    printfn "Application is started."
    
    let params = parseCmdLine (args |> Array.toList)
    if params.federation then createFederation
    if params.spaceReport then createSpaceReport
    if params.wallCreation then createWallModel
    
    0
