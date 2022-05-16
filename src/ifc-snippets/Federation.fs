namespace ifc_snippets
open Xbim.Common.Step21
open Xbim.IO
open Xbim.Ifc
open Xbim.Ifc4.Interfaces

module Federation =
  let createFederation =
    let editor = XbimEditorCredentials (
      ApplicationDevelopersName = "Balint",
      ApplicationFullName = "Federation example",
      ApplicationVersion = "1.0",
      EditorsFamilyName = "Meszaros",
      EditorsGivenName = "Lorinc",
      EditorsOrganisationName = "Meszaros and Meszaros Ltd."
    )
    
    try
      let federation = IfcStore.Create (editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel)
      federation.AddModelReference("/Users/balintbende/Developer/test/IFC/unitTestCase_01_wall_all_req_properties_zero.ifc", "Lorinc", "ARC") |> ignore
      federation.AddModelReference("/Users/balintbende/Developer/test/IFC/unitTestCase_06_curtainwall_related.ifc", "Viktor", "STR") |> ignore
      
      printfn $"Number of overall entities: {federation.FederatedInstances.Count}"
      printfn $"Number of walls: {federation.FederatedInstances.CountOf<IIfcWall>()}"
      federation.SaveAs("federation.ifc")
     with
     | ex ->
       printfn "Federation is failed."