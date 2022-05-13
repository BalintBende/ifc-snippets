open app.utils.CmdLine

module Program =

  [<EntryPoint>]
  let main args =
    printfn "Application is started."
    
    let params = parseCmdLine args
    0
