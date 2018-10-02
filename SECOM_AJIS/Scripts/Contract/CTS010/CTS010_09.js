function SetCTS010_09_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divInsuranceInformation").SetViewMode(true);
    }
    else {
        $("#divInsuranceInformation").SetViewMode(false);
    }
}