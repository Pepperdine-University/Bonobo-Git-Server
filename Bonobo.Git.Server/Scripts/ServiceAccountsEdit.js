function newField(count) {
    var template = document.getElementById("serviceAccountrow");
    var newDetailRow = template.content.cloneNode(true);

    template.parentNode.appendChild(newDetailRow);
    var allDetailRows = $(".service-account-details");

    var nextDetailRowIndex = allDetailRows.length -1;

    if (nextDetailRowIndex >= 0) {
        setChildNameAndIdIndexes(allDetailRows.last(), "{i}", nextDetailRowIndex+ count);
    }
}
function remField(id) { 
    id = id.slice(-1);
    id = "ServiceAccounts_" + id;
    console.log(id);
    var account = document.getElementById(id);
    account.remove();
    //Fix Id change after delete
}

function setChildNameAndIdIndexes(element, placeholder, index) {
    if (element instanceof jQuery) {
        console.log("It is an element of Jquery")
        console.log(element)
        element = element.get(0);
        console.log("After. ");
        console.log(element);
    }

    console.log(element);
    if (element.children) {
        console.log("Element has children ")
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
//$("input[type='checkbox']").on('change', function () {
//    $(this).val(this.checked ? "TRUE" : "FALSE");
//})

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
//function deleteSa() {
//    var d1 = { id: $("#contractId").val() };
//    $.ajax({
//        type: "POST",
//        url: deleteUrl,
//        data: (d1),
//        success: function (xhr) {
//            successMessage('Successfully deleted object.');
//            $("#xButton").click();
//        },
//        error: function (xhr) {
//            failureMessage('Could not delete object.');
//        }
//    })
//}

//function detailRowChanged() {
//    if (this == $('.service-account-details').last().get(0)) {
//        addServiceAccountRow();
//    }
//}
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
