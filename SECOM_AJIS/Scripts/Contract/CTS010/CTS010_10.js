function SetCTS010_10_SectionMode(isview) {
    /// <summary>Method to set View/Edit mode</summary>
    /// <param name="isview" type="bool">Flag to set View mode</param>
    if (isview) {
        $("#divOtherFeeInformation").SetViewMode(true);
    }
    else {
        $("#divOtherFeeInformation").SetViewMode(false);
    }
}