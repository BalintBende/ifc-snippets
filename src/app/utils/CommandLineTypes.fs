namespace app.utils

[<AutoOpen>]
module CmdLineTypes = 
  type CmdLineOptions = {
    wallCreation: bool
    hierarchy: bool
    federation: bool
    spaceReport: bool
  }