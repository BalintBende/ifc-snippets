namespace ifc_snippets
open Xbim.Ifc
open Xbim.Ifc4.Interfaces

module Report =
  let ifcPath="/Users/balintbende/Developer/other/Lounge/IFC/ifc-snippets/src/ifc-snippets/resources/Competence_Centre_Stegersbach_ARC.ifc"
  let createSpaceReport =
    try
      printfn "Space report is started."
      let model = IfcStore.Open ifcPath
      let spaces = model.Instances.OfType<IIfcSpace>() |> List.ofSeq
      printfn $"Number of spaces: {spaces.Length}"
      
      for space in spaces do
        printfn $"Space name: {space.Name}"
        printfn $"\tGuid: {space.GlobalId}"
        space.Decomposes
        |> Seq.map(fun r -> r.RelatingObject)
        |> Seq.map(fun r -> r.GlobalId)
        |> Seq.head
        |> printfn "\tRelating storey guid: %A"
      
      printfn "Space report is finished."
    with
    | ex ->
      printfn "Space report is failed."

