$(document).ready(function () {
    $(".sortColumn").click(function () {
        console.log(this);
        console.log($(this).data("value"));
        $("#sortOrder").val($(this).data("value"));
        $("#indexForm").submit();
    });

    $(".select").click(function () {
        console.log(this);
        console.log($(this).data("value"));
        getUserInfo($(this).data("value"));
    });

    $(".cuClose").click(function () {
        $("#cuLanId").empty();
        $("#cuFirstName").empty();
        $("#cuLastName").empty();
        $("#cuEmail").empty();
        $("#cuRoles").empty();
    });

    $("#search").click(function () {
        $("#indexForm").submit();
    });

    //reset form and submit
    $("#reset").click(function () {
        $("#LastName").val("");

        $("#indexForm").submit();
    });

    function getUserInfo(id) {
        // Call Web API to get a userInformation
        return $.ajax({
            url: window.baseUrl + 'api/UserInformationsApi/' + id,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                console.log(data);
                if (!data.username) {
                    $('#LanIdDetails').html("No User Found");
                    alert(data.lastName);
                } else {
                    console.log(data);

                    $("#userInformationModal").modal("show");
                    $("#cuUsername").html(data.username);
                    $("#cuFirstName").html(data.firstName);
                    $("#cuLastName").html(data.lastName);
                    $("#cuEmail").html(data.emailAddress);

                    if (data.userRoles) {
                        var i = 0;
                        var roleString = "";
                        data.userRoles.forEach(function (item) {
                            if (i === 0) { roleString += "<div class=\"row\">"; }
                            roleString += "<div class=\"col-md-6\">" + item.name + "</div>";
                            i++;
                            if (i === 2) {
                                roleString += "</div>";
                                i = 0;
                            }
                        });
                        $("#cuRoles").html(roleString);
                    }

                    console.log($("#cuRoles").html());
                }
            },
            error: function (request, message, error) {
                $('#LanIdDetails').html("No User Found");
                console.log(error);
                console.log(message);
            }
        });
    }
});