function DepennewField(count) { //dependencyNewField || DependencyNewField
    var Depentemplate = document.getElementById("Dependencyrow");
    //console.log(Depentemplate);
    var DepennewDetailRow = Depentemplate.content.cloneNode(true);
    //console.log(DepennewDetailRow);

    Depentemplate.parentNode.appendChild(DepennewDetailRow);
    var allDetailRows = $(".Dependencies-details");

    var nextDetailRowIndex = allDetailRows.length - 1;

    if (nextDetailRowIndex >= 0) {
        setChildNameAndIdIndexes(allDetailRows.last(), "{i}", nextDetailRowIndex + count);
    }
}
function DepenremField(id) {
    id = id.slice(-1);
    idStr = "Dependencies_" + id;
    console.log(idStr);
    var account = document.getElementById(idStr);
    account.remove();

    let i = 1;
    $(".Dependencies-details").each(function () {
        setChildNameAndIdIndexes(this, (i >= id ? i + 1 : i), i);
        i++;
    })
}

//function setChildNameAndIdIndexes(element, placeholder, index) {
//    if (element instanceof jQuery) {
//        console.log("It is an element of Jquery")
//        console.log(element)
//        element = element.get(0);
//        console.log("After. ");
//        console.log(element);
//    }

//    console.log(element);
//    if (element.children) {
//        console.log("Element has children ")
//        Array.from(element.children).forEach((child) => {
//            if (element.id) {
//                element.id = element.id.replace(placeholder, index);
//            }
//            if (child.id) {
//                child.id = child.id.replace(placeholder, index);
//            }
//            if (child.name) {
//                child.name = child.name.replace(placeholder, index);
//            }
//            if (child.dataset.valmsgFor) {
//                child.dataset.valmsgFor = child.dataset.valmsgFor.replace(placeholder, index);
//            }
//            console.log(index);
//            setChildNameAndIdIndexes(child, placeholder, index);
//        });
//    }
//}