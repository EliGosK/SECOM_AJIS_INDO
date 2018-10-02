/// <reference path="../../Scripts/jquery.maskedinput-1.3.js"/>
/// <reference path="../../Scripts/jquery-1.5.1.min.js"/>
/// <reference path="../../Scripts/jquery-ui-1.8.13.custom.min.js"/>
/// <reference path="../../Scripts/modernizr-1.7.min.js"/>
/// <reference path="../../Content/js/dhtmlxgrid/dhtmlxcommon.js"/>
/// <reference path="../../Scripts/Base/MessageDialog.js"/>
/// <reference path="../../Scripts/Base/Master.js"/>
/// <reference path="../../Scripts/Base/GridControl.js"/>

/// <reference path="../Base/object/ajax_method.js" />



var gridTest;
$(document).ready(function () {

    if ($.find("#gridTest").length > 0) {
        /*----initial grid-----*/
        gridTest = $("#gridTest").InitialGrid(0, false, "/Common/CMS430_InitialGrid", function () {

            SpecialGridControl(gridTest, ["Combobox1", "DateForm", "DateTo", "Date1"]);
            BindOnLoadedEvent(gridTest, function () {
                var colInx = gridTest.getColIndexById('Button');

                if (CheckFirstRowIsEmpty(gridTest, false) == false) {
                    for (var i = 0; i < gridTest.getRowsNum(); i++) {



                        var rowId = gridTest.getRowId(i);

                        // Generate Add button
                        GenerateAddButton(gridTest, "btnAdd", rowId, "Button", true);

                        // Generate Combobox
                        //var Combobox1Index = gridTest.getColIndexById("Combobox1");
                        var clt = "#" + GenerateGridControlID("Combobox1", rowId);
                        var Combobox1Val = $(clt).val();
                        GenerateGridCombobox(gridTest, rowId, "Combobox1", "Combobox1", "/Common/CMS430_GetComboBoxPaymentMethod", Combobox1Val, true);

                        // Generate DateTimePicker
                        //var Date1Index = gridTest.getColIndexById("Date1");
                        var clt = "#" + GenerateGridControlID("Date1", rowId);
                        var Date1Val = $(clt).val();
                        GenerateGridDateTimePicker(gridTest, rowId, "Date1", "Date1", Date1Val, true);


                        // Generate DateTimePicker (From - To)
                        //var DateFormIndex = gridTest.getColIndexById("DateForm");
                        var clt = "#" + GenerateGridControlID("DateForm", rowId);
                        var DateFormVal = $(clt).val();
                        //var DateToIndex = gridTest.getColIndexById("DateTo");
                        var clt = "#" + GenerateGridControlID("DateTo", rowId);
                        var DateToVal = $(clt).val();
                        GenerateGridDateTimePickerFromTo(gridTest, rowId, "DateForm", "DateForm", DateFormVal, "DateTo", "DateTo", DateToVal, true);
                    }
                }

                gridTest.setSizes();
            });

        });



    }

    // Load from server
    $("#btnLoad").click(function () {
        $("#gridTest").LoadDataToGrid(gridTest, 0, false, "/Common/CMS430_SearchResponse", "", "CMS430_Test", false,
                        function () { }, // post-load
                        function (result, controls, isWarning) { // pre-load
                            if (result != undefined) {
                               
                            }
                        });
    });

    // Add by user
    $("#btnAdd").click(function () {
        CheckFirstRowIsEmpty(gridTest, true);
        AddNewRow(gridTest, ["", "User is adding data", "", "", "", "", ""]);

        var row_idx = gridTest.getRowsNum() - 1;
        var row_id = gridTest.getRowId(row_idx);

        // Generate Add button
        GenerateAddButton(gridTest, "btnAdd", row_id, "Button", true);

        // Generate Combobox
        //var Combobox1Index = gridTest.getColIndexById("Combobox1");
        var clt = "#" + GenerateGridControlID("Combobox1", row_id);
        var Combobox1Val = $(clt).val();
        GenerateGridCombobox(gridTest, row_id, "Combobox1", "Combobox1", "/Common/CMS430_GetComboBoxPaymentMethod", Combobox1Val, true);

        // Generate DateTimePicker
        //var Date1Index = gridTest.getColIndexById("Date1");
        var clt = "#" + GenerateGridControlID("Date1", row_id);
        var Date1Val = $(clt).val();
        GenerateGridDateTimePicker(gridTest, row_id, "Date1", "Date1", Date1Val, true);


        // Generate DateTimePicker (From - To)
        //var DateFormIndex = gridTest.getColIndexById("DateForm");
        var clt = "#" + GenerateGridControlID("DateForm", row_id);
        var DateFormVal = $(clt).val();
        //var DateToIndex = gridTest.getColIndexById("DateTo");
        var clt = "#" + GenerateGridControlID("DateTo", row_id);
        var DateToVal = $(clt).val();
        GenerateGridDateTimePickerFromTo(gridTest, row_id, "DateForm", "DateForm", DateFormVal, "DateTo", "DateTo", DateToVal, true);

        gridTest.setSizes();
    });


    // Clear row
    $("#btnClearRow").click(function () {
        DeleteAllRow(gridTest);
    });


});


