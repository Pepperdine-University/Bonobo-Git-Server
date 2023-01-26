function newField(count) {
    var template = document.getElementById("serviceAccountrow");
    var newDetailRow = template.content.cloneNode(true);

    template.parentNode.appendChild(newDetailRow);
    var allDetailRows = $(".service-account-details");

    var nextDetailRowIndex = allDetailRows.length -1;

    if (nextDetailRowIndex >= 0) {
        setChildNameAndIdIndexes(allDetailRows.last(), "{i}", nextDetailRowIndex+ count);
    }
   
    //if (nextDetailRowIndex < 0) {
    //    console.log("The index is less then 0");
    //    console.log("Index: " + count);
    //    //newDetailRow.find('#ServiceAccounts_{i}__.Id').attr({ name: "ServiceAccounts[{" + count + "}].InPassManager" });

    //    //$('#ServiceAccounts_{i}__.ServiceAccountName').attr("name": "ServiceAccounts[{' + count + '}].ServiceAccountName', 'id': 'ServiceAccounts_{ ' + count + '}__.ServiceAccountName');
    //    $('#ServiceAccounts_{i}__.Id').attr('id', 'test');
    //    $('#ServiceAccounts_{i}__.Id').attr('name', 'test');
    //    //$('#ServiceAccounts_{i}__.ServiceAccountName').attr('id', 'ServiceAccounts_{' + count + '}__.ServiceAccountName');
    //}
    //var newDetailRow = $('@Model.ServiceAccounts newServiceAccounts = new ServiceAccounts();' +
    //    '<div class="pure-control-group">' +
    //    '@Html.LabelFor(m=> m.newServiceAccount.ServiceAccountName)' +
    //    '@Html.TextBoxFor(m=> m.newServiceAccount.ServiceAccountName)' +
    //    '@Html.ValidationMessageFor(m=> m.newServiceAccount.ServiceAccountName) ' +
    //    '</div >' +
    //    '<div class="pure-control-group">' +
    //    '@Html.LabelFor(m=> m.newServiceAccount.InPassManager)' +
    //    '@Html.CheckBoxFor(m=> m.newServiceAccount.InPassManager)' +
    //    '@Html.ValidationMessageFor(m=> m.newServiceAccount.InPassManager)' +
    //    '</div>' +
    //    '<div class="pure-control-group">' +
    //    '@Html.LabelFor(m=> m.newServiceAccount.PassLastUpdated)' +
    //    '@Html.TextBoxFor(m=> m.newServiceAccount.PassLastUpdated)' +
    //    '@Html.ValidationMessageFor(m=> m.newServiceAccount.PassLastUpdated)' +
    //    '</div>' +
    //    '<div class="pure-control-group" style="display:none;">' +
    //    '@Html.LabelFor(m=> m.newServiceAccount.Id)' +
    //    '@Html.TextBoxFor(m=> m.newServiceAccount.Id)' +
    //    '@Html.LabelFor(m=> m.newServiceAccount.RepositoryId)' +
    //    '@Html.TextBoxFor(m=> m.newServiceAccount.RepositoryId)' +
    //    '</div>)' +
    //    ' @m.ServiceAccounts.Append(newServiceAccount);');
    
}
function setChildNameAndIdIndexes(element, placeholder, index) {
    if (element instanceof jQuery) {
        console.log("It is an element of Jquery")
        console.log(element)
        element = element.get(0);
        console.log("After. ")
        console.log(element);
    }

    console.log(element);
    if (element.children) {
        console.log("Element has children ")
        Array.from(element.children).forEach((child) => {
            if (child.id) {
                child.id = child.id.replace(placeholder, index);
            }
            if (child.name) {
                child.name = child.name.replace(placeholder, index);
            }
            if (child.dataset.valmsgFor) {
                child.dataset.valmsgFor = child.dataset.valmsgFor.replace(placeholder, index);
            }
            console.log(index);
            setChildNameAndIdIndexes(child, placeholder, index);
        });
    }
}
//var template = document.getElementById("serviceAccountrow");
//var newDetailRow = template.content.cloneNode(true);

//template.parentNode.appendChild(newDetailRow);



//    $('.service-account-details').on('focus', detailRowChanged);
//    addServiceAccountRow();

//    var idElement = $("#ServiceAccounts[i].Id");
//    var id = idElement.val();
//    /*
//      if (id != '') {
//        $('#hdrContractName').html(id);
//        idElement.attr('readonly', 'readonly');
//    } else {
//        $('#hdrContractName').html('New Contract');
//    }*/
//    $(".pure-button save").off('click').on('click', saveAccount);
 
//JQuery to not make the checkboxes be automatically set to true
$("input[type='checkbox']").on('change', function () {
    $(this).val(this.checked ? "TRUE" : "FALSE");
})

//function addServiceAccountRow() {
//    var allDetailRows = $(".service-account-details");
//    var nextDetailRowIndex = allDetailRows.length - 1;

//    if (nextDetailRowIndex >= 0) {
//        setChildNameAndIdIndexes(allDetailRows.last(), "[i]", nextDetailRowIndex);
//    }

//    var template = document.getElementById("serviceAccountrow");
//    var newDetailRow = template.content.cloneNode(true);

//    template.parentNode.appendChild(newDetailRow);
//    $('.service-account-details').last().on('change', detailRowChanged);
//}

function detailRowChanged() {
    if (this == $('.service-account-details').last().get(0)) {
        addServiceAccountRow();
    }
}
//function saveContract() {
//    const d1 = getFormValues(document.querySelector('form'));

//    $.ajax({
//        type: "POST",
//        url: saveUrl,
//        data: (d1),
//        success: function (xhr) {
//            successMessage('Changes were successfully saved.');
//            showInPopup(`${editUrl}/${id}`);
//        },
//        error: function (xhr) {
//            failureMessage('Changes could not be saved.')
//        }
//    })
//}
