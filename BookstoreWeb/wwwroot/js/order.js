var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess"))
        loadDataTable("inprocess")
    else if (url.includes("pending"))
        loadDataTable("pending")
    else if (url.includes("completed"))
        loadDataTable("completed");
    else if (url.includes("approved"))
        loadDataTable("approved");
    else
        loadDataTable("all");
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' +  status },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'firstName', "width": "15%" },
            { data: 'lastName', "width": "15%" },
            { data: 'phoneNumber', "width": "12%" },
            { data: 'applicationUser.email', "width": "18%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div>
                                <a href="/admin/order/details?orderId=${data}" class="btn btn-success mx-2" style="width:90px">
		                        <i class="bi bi-pencil"></i>
                                </a>
                            </div>`
                },
                "width": "10%"
            }
        ]
    });
}



