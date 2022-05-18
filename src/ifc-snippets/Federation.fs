namespace ifc_snippets
open Xbim.Common.Step21
open Xbim.IO
open Xbim.Ifc
open Xbim.Ifc4.Interfaces

module Federation =
  let ifcPath1="/Users/balintbende/Developer/other/Lounge/IFC/ifc-snippets/src/ifc-snippets/resources/Competence_Centre_Stegersbach_ARC.ifc"
  let ifcPath2="/Users/balintbende/Developer/other/Lounge/IFC/ifc-snippets/src/ifc-snippets/resources/WA Feldmoos III BA1_Kalk_EIN.ifc"
  let credentials = XbimEditorCredentials (
      ApplicationDevelopersName = "Balint",
      ApplicationFullName = "Federation example",
      ApplicationVersion = "1.0",
      EditorsFamilyName = "Meszaros",
      EditorsGivenName = "Lorinc",
      EditorsOrganisationName = "Meszaros and Meszaros Ltd."
    )
  let createFederation =
    try
      let federation = IfcStore.Create (credentials, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel)
      federation.AddModelReference(ifcPath1, "Lorinc", "ARC") |> ignore
      federation.AddModelReference(ifcPath2, "Viktor", "STR") |> ignore
      
      printfn $"Number of overall entities: {federation.FederatedInstances.Count}"
      printfn $"Number of walls: {federation.FederatedInstances.CountOf<IIfcWall>()}"
      for refModel in federation.ReferencedModels do
        printfn $"\tReferenced model: {refModel.Name}"
        printfn $"\tReferenced model organization: {refModel.OwningOrganisation}"
        printfn $"\tNumber of walls: {refModel.Model.Instances.CountOf<IIfcWall>()}"
      federation.SaveAs("federation.ifc")
      printfn "Federation is completed."
     with
     | ex ->
       printfn $"Federation is failed.\n{ex.Message}\n{ex.StackTrace}"