open app.utils.CmdLine

module Program =

  [<EntryPoint>]
  let main args =
    printfn "Application is started."
    
    parseCmdLine args
    0
