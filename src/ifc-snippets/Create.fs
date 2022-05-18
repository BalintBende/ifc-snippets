namespace ifc_snippets
open Xbim.Common
open Xbim.Common.Step21
open Xbim.Ifc
open Xbim.IO
open Xbim.Ifc4.GeometricConstraintResource
open Xbim.Ifc4.GeometricModelResource
open Xbim.Ifc4.GeometryResource
open Xbim.Ifc4.Interfaces
open Xbim.Ifc4.Kernel
open Xbim.Ifc4.MeasureResource
open Xbim.Ifc4.ProductExtension
open Xbim.Ifc4.ProfileResource
open Xbim.Ifc4.RepresentationResource
open Xbim.Ifc4.SharedBldgElements

module Create =
  let createAndInitModel name =
    let credentials = XbimEditorCredentials (
      ApplicationDevelopersName = "Balint",
      ApplicationFullName = "Proper wall example",
      ApplicationVersion = "1.0",
      EditorsFamilyName = "Meszaros",
      EditorsGivenName = "Lorinc",
      EditorsOrganisationName = "Meszaros and Meszaros Ltd."
    )
    
    let model = IfcStore.Create (credentials, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel)
    use txn = model.BeginTransaction("Init model")
    let project = model.Instances.New<IfcProject>()
    project.Initialize ProjectUnits.SIUnitsUK
    project.Name <- IfcLabel name
    txn.Commit()
    model

  let createBuilding (model: IfcStore) name =
    use txn = model.BeginTransaction("Create building")
    let building = model.Instances.New<IfcBuilding>()
    building.Name <- IfcLabel name
    building.CompositionType <- IfcElementCompositionEnum.ELEMENT
    
    // set placement
    let localPlacement = model.Instances.New<IfcLocalPlacement>()
    building.ObjectPlacement <- localPlacement
    let placement = model.Instances.New<IfcAxis2Placement3D>()
    localPlacement.RelativePlacement <- placement
    placement.Location <- model.Instances.New<IfcCartesianPoint> (fun p -> p.SetXYZ(0,0,0))
    let project =
      model.Instances.OfType<IfcProject>()
      |> Seq.head
    
    project.AddBuilding building
    txn.Commit()
    building
  
  let createBuildingStorey (building: IfcBuilding) storeyName =
    let model = building.Model
    use txn = model.BeginTransaction("Create building storey")
    let storey = model.Instances.New<IfcBuildingStorey>()
    storey.Name <- IfcLabel storeyName
    storey.Elevation <- IfcLengthMeasure 0
    building.AddToSpatialDecomposition storey
    txn.Commit()
    storey
  
  let createWall (model: IfcStore) (length: float) (width: float) (height: float) =
    use txn = model.BeginTransaction("Create wall")
    let wall = model.Instances.New<IfcWallStandardCase>()
    wall.Name <- IfcLabel "A Standard rectangular wall"
    
    //profile
    let rectProf = model.Instances.New<IfcRectangleHollowProfileDef>()
    rectProf.ProfileType <- IfcProfileTypeEnum.AREA
    rectProf.XDim <- width
    rectProf.YDim <- length
    
    let insertPoint = model.Instances.New<IfcCartesianPoint>()
    insertPoint.SetXY (0, 400); //insert at arbitrary position
    rectProf.Position <- model.Instances.New<IfcAxis2Placement2D>()
    rectProf.Position.Location <- insertPoint
    
    //model as a swept area solid
    let body = model.Instances.New<IfcExtrudedAreaSolid>()
    body.Depth <- height
    body.SweptArea <- rectProf
    body.ExtrudedDirection <- model.Instances.New<IfcDirection>()
    body.ExtrudedDirection.SetXYZ(0, 0, 1)
    
    //parameters to insert the geometry in the model
    let origin = model.Instances.New<IfcCartesianPoint>()
    origin.SetXYZ(0, 0, 0)
    body.Position <- model.Instances.New<IfcAxis2Placement3D>()
    body.Position.Location <- origin

    //Create a Definition shape to hold the geometry
    let shape = model.Instances.New<IfcShapeRepresentation>()
    let modelContext = model.Instances.OfType<IfcGeometricRepresentationContext>() |> Seq.head
    shape.ContextOfItems <- modelContext
    shape.RepresentationType <- IfcLabel "SweptSolid"
    shape.RepresentationIdentifier <- IfcLabel "Body"
    shape.Items.Add body
    
    //Create a Product Definition and add the model geometry to the wall
    let rep = model.Instances.New<IfcProductDefinitionShape>()
    rep.Representations.Add shape
    wall.Representation <- rep

    //now place the wall into the model
    let lp = model.Instances.New<IfcLocalPlacement>()
    let ax3D = model.Instances.New<IfcAxis2Placement3D>()
    ax3D.Location <- origin
    ax3D.RefDirection <- model.Instances.New<IfcDirection>()
    ax3D.RefDirection.SetXYZ(0, 1, 0)
    ax3D.Axis <- model.Instances.New<IfcDirection>()
    ax3D.Axis.SetXYZ(0, 0, 1)
    lp.RelativePlacement <- ax3D
    wall.ObjectPlacement <- lp
    
    // linear segment as IfcPolyline with two points is required for IfcWall
    let ifcPolyline = model.Instances.New<IfcPolyline>()
    let startPoint = model.Instances.New<IfcCartesianPoint>()
    startPoint.SetXY(0, 0)
    let endPoint = model.Instances.New<IfcCartesianPoint>()
    endPoint.SetXY(length, 0)
    ifcPolyline.Points.Add startPoint
    ifcPolyline.Points.Add endPoint

    let shape2D = model.Instances.New<IfcShapeRepresentation>()
    shape2D.ContextOfItems <- modelContext
    shape2D.RepresentationIdentifier <- IfcLabel "Axis"
    shape2D.RepresentationType <- IfcLabel "Curve2D"
    shape2D.Items.Add ifcPolyline
    rep.Representations.Add shape2D
    txn.Commit()
    wall
    
  let createWallModel =
    try
      printfn "Creation of standard wall is started."
      //create and init model
      use model = createAndInitModel "Default project"
      let building = createBuilding model "Default building"
      let buildingStorey = createBuildingStorey building "Default building storey"
      let wall = createWall model 4000 300 2400
      
      //add wall to storey
      use txn = model.BeginTransaction("Add wall to storey")
      buildingStorey.AddElement wall
      txn.Commit()
      printfn "Standard wall is successfully created."
      
      model.SaveAs ("ProperWall.ifc", StorageType.Ifc)
      printfn "ProperWall.ifc has been successfully written."
      
    with
    | ex ->
      printfn $"Standard wall creation is failed.\n{ex.Message}\n{ex.StackTrace}"