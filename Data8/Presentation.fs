namespace Data8

module Presentation =

    open System

    open System.Windows.Forms
    open System.Windows.Forms.Integration

    open FsXaml
    open FSharp.Data

    open Microsoft.Office.Interop.Excel

    open ExcelDna.Integration
    open ExcelDna.Integration.CustomUI
    open ExcelDna.Integration.Extensibility

    
// based on
//From <https://raw.githubusercontent.com/mndrake/ExcelCustomTaskPane/master/CustomTaskPane.fs> 
//From <http://exceldna.codeplex.com/SourceControl/latest#Distribution/Samples/CustomTaskPane.dna> 

    type PaneControl = XAML<"PaneControl.xaml", true>

    [<ExcelFunction(Description="My first .NET function")>]
    let HelloDna name = 
        "Hello " + name

    let thePaneControl = PaneControl()
    let mutable theApp: Microsoft.Office.Interop.Excel.Application = null 

    type public Data8Control() as this =
        inherit UserControl()
        do
            let wpfElementHost = new ElementHost(Dock = DockStyle.Fill)        
            wpfElementHost.HostContainer.Children.Add(thePaneControl) |> ignore
            thePaneControl. MyButton.Click.Add(fun _ -> this.MyButton_Click())
            thePaneControl.theButton.Click.Add(fun _ -> thePaneControl.theLabel.Content <- "changed")
            this.Controls.Add(wpfElementHost)

        member this.Content with get() = thePaneControl
        member this.MyButton_Click() = MessageBox.Show("You clicked the button.") |> ignore  

    module CTPManager =
        let mutable theCTP:CustomTaskPane option = None

        let ShowCTP() =
            if theCTP.IsNone then
                theCTP <- 
                CustomTaskPaneFactory.CreateCustomTaskPane(typeof<Data8Control>, "My Custom Task Pane")
                |> fun c -> c.Visible <- true
                            c.DockPosition <- MsoCTPDockPosition.msoCTPDockPositionLeft
                            Some(c)
            elif theCTP.Value.Visible = false then
                theCTP.Value.Visible <- true 

        let HideCTP() =
            if theCTP.IsSome then
                theCTP.Value.Visible <- false


    let LoadPaneIndicatorList() = 
        let indicatorNames = Queries.loadIndicatorNamesFromCache() |> Seq.take 200 |> Seq.toList
        thePaneControl.theIndicatorsListView.Items.Clear()
        indicatorNames |> List.iter (fun x -> thePaneControl.theIndicatorsListView.Items.Add( x.ID ) |> ignore )
        ()

    let LoadIndicatorData() =
        let book = theApp.ActiveSheet
        let cell = theApp.ActiveCell
//        let l = Queries.loadIndicator(["fr";"gb";"us"],"SP.POP.TOTL","2008","2010")
        let l = Queries.loadIndicator(["all"], thePaneControl.theIndicatorsListView.SelectedItem.ToString(), "all", "all")
        cell.Offset(0,0).Value2 <- "Country"
        cell.Offset(0,1).Value2 <- "ID"
        cell.Offset(0,2).Value2 <- "Name"
        cell.Offset(0,3).Value2 <- "Value"
        cell.Offset(0,4).Value2 <- "Year"

        l |> List.iteri(fun i x -> cell.Offset(i+1,0).Value2 <- x.Country
                                   cell.Offset(i+1,1).Value2 <- x.Id
                                   cell.Offset(i+1,2).Value2 <- x.Name
                                   cell.Offset(i+1,3).Value2 <- x.Value
                                   cell.Offset(i+1,4).Value2 <- x.Year )

    type public MyRibbon() =
        inherit ExcelRibbon()
        member this.ShowCTP(control:IRibbonControl, isPressed:bool) = CTPManager.ShowCTP()
        member this.HideCTP(control:IRibbonControl, isPressed:bool) = CTPManager.HideCTP()
        member this.LoadIndicatorNames(control:IRibbonControl, isPressed:bool) = LoadPaneIndicatorList()
        member this.LoadIndicatorData(control:IRibbonControl, isPressed:bool) = LoadIndicatorData()

        member this.SetACell(control:IRibbonControl, isPressed:bool) = 
            let book = theApp.Workbooks.Item(1)
            let sheet = book.Sheets.Item(1)  :?> Worksheet
            let cell = sheet.Range("A1")
            cell.Offset(2,2).Value2 <- "hey there finally maybe"

        override this.OnConnection(application: obj,connectMode: ext_ConnectMode, addInInst: obj, custom: byref<Array>)=
            theApp <- application :?> Microsoft.Office.Interop.Excel.Application
           
        override this.OnDisconnection(removeMode: ext_DisconnectMode, custom: byref<Array>)=
            ()    

        override this.OnStartupComplete(custom: byref<Array>)=
            MessageBox.Show("OnStartupComplete") |> ignore


        

