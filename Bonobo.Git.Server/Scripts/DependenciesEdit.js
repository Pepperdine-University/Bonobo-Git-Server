//dynamically creates a new empty dependency field
function DepennewField(count) { 
    var Depentemplate = document.getElementById("Dependencyrow");
    var DepennewDetailRow = Depentemplate.content.cloneNode(true);

    Depentemplate.parentNode.appendChild(DepennewDetailRow);
    var allDetailRows = $(".Dependencies-details");

    var nextDetailRowIndex = allDetailRows.length - 1;

    if (nextDetailRowIndex >= 0) {
        setChildNameAndIdIndexes(allDetailRows.last(), "{i}", nextDetailRowIndex );
    }
}
//dynamically deletes a dependency field when the delete button is clicked
function DepenremField(id) {
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
//function that recurivly updates indicies by replacing the place holder with the correct value
function setDepenChildNameAndIdIndexes(element, placeholder, index) {
    if (element instanceof jQuery) {
        element = element.get(0);
    }
;
    if (element.children) {
        Array.from(element.children).forEach((child) => {
            if (element.id) {
                element.id = element.id.replace(placeholder, index);
            }
            if (child.id) {
                child.id = child.id.replace(placeholder, index);
            }
            if (child.name) {
                child.name = child.name.replace(placeholder, index);
            }
            if (child.dataset.valmsgFor) {
                child.dataset.valmsgFor = child.dataset.valmsgFor.replace(placeholder, index);
            }
            setChildNameAndIdIndexes(child, placeholder, index);
        });
    }
}

