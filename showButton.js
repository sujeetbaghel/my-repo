function isRecordCreated(formControl) {
    // Get the record ID (GUID)
    var recordId = formControl.data.entity.getId();
    
    // Check if the record has an ID (it will be null if not created yet)
    return (recordId !== null && recordId !== "");
}
