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

function setDepenChildNameAndIdIndexes(element, placeholder, index) {
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
    window.onload = function () {
        $(".dropDownKD").each(function () {
            var index = this.id;
            if (this.value == "Add New...") {
                $("#newKD-" + index).css("display", "inline-block");
            } else {
                $("#newKD-" + index).css("display", "none");
            }
        })
    }
    function DisplayComponentNameNewInputField(text, index) {
        if (text == "Add New...") {
            $("#newKD-" + index).css("display", "inline-block");
        } else {
            $("#newKD-" + index).css("display", "none");
        }
    }
    function checkIfKDExists(i, input) {
        var newKDName = input.value;
        var dropDown = document.getElementById(`Dependencies_${i}__KnownDependenciesId`);
        var exists = 0;
        for (j = 0; j < dropDown.options.length; j++) {
            if (newKDName == dropDown.options[j].text) {
                exists = 1;
                break;
            }
        }
        if (newKDName == "") {
            alert("Please input a value, the new component name cannot be blank");
        } else if (exists == 1) {
            alert("The component name entered already exists, please input a component name that does not exist or select the existing one");
        } else {
            addKDtoDropDown(i, newKDName, dropDown);
        }
    }
    function addKDtoDropDown(i, newKDName, dropDown) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            url: `@Url.Action("GenerateNewGuid", "Repository")`,
            data: { "componentName": newKDName },
            success: function (response) {
                var option = document.createElement("option");
                option.text = newKDName;
                option.value = response;
                option.selected = true;
                dropDown.add(option);

                var index = `Dependencies_${i}__KnownDependenciesId`;
                $("#newKD-" + index).css("display", "none");
            },
            failure: function (response) {
                alert("Failed to add new component name");
            }
        });
        return false;

        // 1. Get rid of known dependency function --
        // 2. Get rid of partial view --
        // 3. Add check to see if known dependency exists --
        // 4. Add css styling to make it look good
        // 5. Get rid of commented code
        // 6. Put my code into javascript file
    }
}