//dynamically creates a new empty service account field
function newField() {
    var template = document.getElementById("serviceAccountrow");
    var newDetailRow = template.content.cloneNode(true);

    template.parentNode.appendChild(newDetailRow);
    var allDetailRows = $(".service-account-details");

    var nextDetailRowIndex = allDetailRows.length - 1;

    if (nextDetailRowIndex >= 0) {
        setChildNameAndIdIndexes(allDetailRows.last(), "{i}", nextDetailRowIndex);
    }
}
//dynamically deletes a service account field when the delete button is clicked
function remField(id) { 
    id = id.slice(-1);
    idStr = "ServiceAccounts_" + id;
    var account = document.getElementById(idStr);
    account.remove();
    let i = 0;
    $(".service-account-details").each(function(){
        setChildNameAndIdIndexes(this, (i >= id ? i + 1 : i), i);
        i++;
    })
    
}
//function that recurivly updates indicies by replacing the place holder with the correct value
function setChildNameAndIdIndexes(element, placeholder, index) {
    if (element instanceof jQuery) {
        element = element.get(0);
    }
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
            //Recursive call on children
            setChildNameAndIdIndexes(child, placeholder, index);
        });
    }
}
