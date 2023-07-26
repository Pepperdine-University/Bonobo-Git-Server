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

function DisplayComponentNameNewInputField(dropDownId) {
    var index = dropDownId.split('_')[1];
    var newKDId = `divNewKnownDependency_${index}`;
    dropDown = document.getElementById(dropDownId);
    selectedValue = dropDown[dropDown.selectedIndex].text;
    if (selectedValue == "Add New...") {
        $(`#${newKDId}`).css("display", "inline-block");
        $(`#${dropDownId}`).css("display", "none");
        $(`#${newKDId} input`).focus();
    }
}

function setComponentName(id) {
    $(`#${id}`)[0].value = $(`#${id}`).val();
}

function returnToDropdownFromKDInput(i) {
    $(`#divNewKnownDependency_${i}`).css("display", "none");
    $(`#Dependencies_${i}__KnownDependenciesId`).css("display", "inline-block");
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

function DisplayComponentNameNewInputFieldOnLoad(i) {
    $(`#divNewKnownDependency_${i}`).css("display", "inline-block");
    $(`#Dependencies_${i}__KnownDependenciesId`).css("display", "none");
}
