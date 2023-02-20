//dynamically creates a new empty service account field

   $(".datepicker").datepicker();

function newField() {
    var table = document.getElementById("service-account-table");
    var row = table.insertRow();
    row.classList.add("row");
    row.classList.add("service-account-details");
    row.id = "ServiceAccounts_{i}";
    row.name = "service-account-details";
    row.innerHTML = ` <tr class="row service-account-details" id="ServiceAccounts_{i}" name="ServiceAccounts_{i}">
                    <th class=" col pure-control-group" style="display:none;">
                        <input class="form-control" id="ServiceAccounts[{i}].Id" name="ServiceAccounts[{i}].Id" readonly disabled />
                    </th>
                    <th class="col col0 pure-control-group">
                        <input class="form-control" id="ServiceAccounts_{i}__.ServiceAccountName" name="ServiceAccounts[{i}].ServiceAccountName" />
                        <span class="field-validation-valid text-danger"
                              data-valmsg-for="ServiceAccounts[{i}].ServiceAccountName"
                              data-valmsg-replace="true">
                        </span>
                    </th>
                    <th class="col col1 pure-control-group">
                        <input class="form-control" type="checkbox" id="ServiceAccounts_{i}__.InPassManager" name="ServiceAccounts[{i}].InPassManager" value="true" data-val="true" data-val-required="The InPassManager field is required." />
                        <span class="field-validation-valid text-danger"
                              data-valmsg-for="ServiceAccounts[{i}].InPassManager"
                              data-valmsg-replace="true">
                        </span>
                    </th>
                    <th class="col col2 pure-control-group">
                        <input class="form-control datepicker" type="date" id="ServiceAccounts_{i}__.PassLastUpdated" name="ServiceAccounts[{i}].PassLastUpdated" value="1/1/1900" />
                        <span class="field-validation-valid text-danger"
                              data-valmsg-for="ServiceAccounts[{i}].PassLastUpdated"
                              data-valmsg-replace="true">
                        </span>
                        <button type="button" onclick="remField(this.id);" id=" ServiceAccountBtn_{i}" title="Remove Service Account" style="background-color: white; border: none;"><i style="color:red;" class="fa fa-minus-circle"></i></button>
                    </th>
                </tr>`;
    linkField();
}
function linkField() {

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
