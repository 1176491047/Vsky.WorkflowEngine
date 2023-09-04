$(function () {
    var l = abp.localization.getResource("WorkflowEngine");
	
	var workflowEngineSettingsManageService = window.vsky.workflowEngine.workflowEngineSettingsManages.workflowEngineSettingsManages;
	
	
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "WorkflowEngineSettingsManages/CreateModal",
        scriptUrl: "/Pages/WorkflowEngineSettingsManages/createModal.js",
        modalClass: "workflowEngineSettingsManageCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "WorkflowEngineSettingsManages/EditModal",
        scriptUrl: "/Pages/WorkflowEngineSettingsManages/editModal.js",
        modalClass: "workflowEngineSettingsManageEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            projectName: $("#ProjectNameFilter").val(),
			description: $("#DescriptionFilter").val(),
			historySaveDaysMin: $("#HistorySaveDaysFilterMin").val(),
			historySaveDaysMax: $("#HistorySaveDaysFilterMax").val(),
			dBInfoDescription: $("#DBInfoDescriptionFilter").val()
        };
    };

    var dataTable = $("#WorkflowEngineSettingsManagesTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: true,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(workflowEngineSettingsManageService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('WorkflowEngine.WorkflowEngineSettingsManages.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('WorkflowEngine.WorkflowEngineSettingsManages.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    workflowEngineSettingsManageService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "projectName" },
			{ data: "description" },
			{ data: "historySaveDays" },
			{ data: "dBInfoDescription" }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewWorkflowEngineSettingsManageButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

	$("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $("#ExportToExcelButton").click(function (e) {
        e.preventDefault();

        workflowEngineSettingsManageService.getDownloadToken().then(
            function(result){
                    var input = getFilter();
                    var url =  abp.appPath + 'api/app/workflow-engine-settings-manages/as-excel-file' + 
                        abp.utils.buildQueryString([
                            { name: 'downloadToken', value: result.token },
                            { name: 'filterText', value: input.filterText }, 
                            { name: 'projectName', value: input.projectName }, 
                            { name: 'description', value: input.description },
                            { name: 'historySaveDaysMin', value: input.historySaveDaysMin },
                            { name: 'historySaveDaysMax', value: input.historySaveDaysMax }, 
                            { name: 'dBInfoDescription', value: input.dBInfoDescription }
                            ]);
                            
                    var downloadWindow = window.open(url, '_blank');
                    downloadWindow.focus();
            }
        )
    });

    $('#AdvancedFilterSectionToggler').on('click', function (e) {
        $('#AdvancedFilterSection').toggle();
    });

    $('#AdvancedFilterSection').on('keypress', function (e) {
        if (e.which === 13) {
            dataTable.ajax.reload();
        }
    });

    $('#AdvancedFilterSection select').change(function() {
        dataTable.ajax.reload();
    });
    
    
});
