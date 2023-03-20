//dynamically creates a new empty dependency field
function createDependency(count) { 
    var table = document.getElementById("dependencies-table");
    var row = table.insertRow();
    row.classList.add("row");
    row.classList.add("Dependencies-details");
    row.id = "Dependencies_{i}";
    row.name = "Dependencies-details";
    row.innerHTML = document.getElementById("Dependencyrow").innerHTML;
    var allDetailRows = $(".Dependencies-details");
    var nextDetailRowIndex = allDetailRows.length - 1;
    if (nextDetailRowIndex >= 0) {
        setChildNameAndIdIndexes(allDetailRows.last(), "{i}", nextDetailRowIndex );
    }
}
//dynamically deletes a dependency field when the delete button is clicked
function removeDependency(id) {
    id = id.slice(-1);
    idStr = "Dependencies_" + id;
    console.log(idStr);
    var account = document.getElementById(idStr);
    account.remove();

    let i = 0;
    $(".Dependencies-details").each(function () {
        setChildNameAndIdIndexes(this, (i >= id ? i + 1 : i), i);
        i++;
    })
}

function DisplayComponentNameNewInputField(dropDownID) {
    inputField = document.getElementById("KDInput" + dropDownID);
    dropDown = document.getElementById(dropDownID);
    selectedValue = dropDown[dropDown.selectedIndex].text;
    if (selectedValue == "Add New...") {
        $("#newKD-" + dropDownID).css("display", "inline-block");
        $("#" + dropDownID).css("display", "none");
        changeNameOfInput(inputField, dropDown, 1);
    }
}

function returnToDropdownFromKDInput(i) {
    dropDownID = "Dependencies_" + i + "__KnownDependenciesId";
    $("#" + dropDownID).css("display", "inline-block");
    $("#newKD-" + dropDownID).css("display", "none");

    document.getElementById("KDInput" + dropDownID).value = "";
    document.getElementById(dropDownID).selectedIndex = 0;

    changeNameOfInput(inputField, dropDown, 0);
}

function changeNameOfInput(inputField, dropDown, addNewSelected) {
    if (addNewSelected) {
        inputField.setAttribute("name", dropDown.name);
        dropDown.setAttribute("name", "");
    } else if (dropDown.name == "") {
        dropDown.setAttribute("name", inputField.name);
        inputField.setAttribute("name", "");
    }
}
