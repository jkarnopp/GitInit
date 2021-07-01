var selectedId = "";
var selectedUser = new Object();

$(document).ready(function () {

    $("#editFormSubmit").click(function () {
        $("#editForm").submit();
    });

    $("#LanId").on('focus',
        function () {
            $("#LanId").blur();
            $('#idSearchModal').modal('show');
            selectedId = this.id;
        });

    //When modal is shown, focus the usrname form field
    $('#idSearchModal').on('shown.bs.modal',
        function () {
            $('#usrname').focus();
        });

    $("#checkUser").click(getUserInfo);
    $("#idSearchModalCancel").click(clearUserInfo);
    $("#saveUser").click(saveUserInfo);

    $("#sendToUser").click(function () {
        $("#IsSendToUser").val("True");
        $("#notifyYesNoModal").modal("hide");
    });

    $("#Enabled").change(function () {
        if ($(this).is(":checked")) {
            $("#notifyYesNoModal").modal("show");
        }
    });
});

function saveUserInfo() {
    if (!$("#usrname").val()) return;

    if ($("#usrname").val() && $.isEmptyObject(selectedUser)) {
        $.when(getUserInfo().done(function () {
            if ($.isEmptyObject(selectedUser)) return;
            console.log("after Save only:" + selectedUser.LanId);
            setValues();
        }));
    } else setValues();
}

function setValues() {
    console.log("After Ifs: " + selectedUser.LanId);
    var names = getNamesWell(selectedUser);
    console.log(selectedId);

    $("#LanId").val(selectedUser.LanId);
    $("#FirstName").val(selectedUser.FirstName);
    $("#LastName").val(selectedUser.LastName);
    $("#Email").val(selectedUser.EmailAddress);
    //$("#SelectedLanIdDetails").html(names);

    clearUserInfo();
    $('#idSearchModal').modal('hide');
}

function clearUserInfo() {
    $("#usrname").val("");
    $('#LanIdDetails').html("");
    selectedUser = {};
}

function getUserInfo() {
    // Call Web API to get a list of Product
    return $.ajax({
        url: window.baseUrl + 'api/UserInformationsApi/GetByLanId?lanId=' +
            $("#usrname").val(),
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (!data.lanId) {
                $('#LanIdDetails').html("No User Found");
            } else {
                var names = getNamesWell(data);

                $('#LanIdDetails').html(names);
                selectedUser.LanId = data.lanId;
                selectedUser.FirstName = data.firstName;
                selectedUser.LastName = data.lastName;
                selectedUser.EmailAddress = data.emailAddress;

                console.log(window.selectedUser);
            }
        },
        error: function (request, message, error) {
            $('#LanIdDetails').html("No User Found");
            console.log(error);
            console.log(message);
        }
    });
}

function getNamesWell(data) {
    var names = "<br /><div class='card card-body bg-light'><ul><li><strong>LAN ID:</strong> " +
        data.lanId +
        "</li>" +
        "<li><strong>Name:</strong> " +
        data.firstName +
        " " +
        data.lastName +
        "</li>" +
        "<li><strong>Email:</strong> " +
        data.emailAddress +
        "</li>" +
        "</ul></div>";
    return names;
}

function userInfoSuccess(userInfo) {
    // Iterate over the collection of data
    var user = JSON.parse(userInfo);

    $("#LanIdDetails").html("<ul><li>" + user.FirstName + "</li></ul>");
}