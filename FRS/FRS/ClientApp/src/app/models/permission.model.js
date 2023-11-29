"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Permission = /** @class */ (function () {
    function Permission(name, value, groupName, description) {
        this.name = name;
        this.value = value;
        this.groupName = groupName;
        this.description = description;
    }
    Permission.viewUsersPermission = "systemsetting.user.view";
    Permission.manageUsersPermission = "systemsetting.user.manage";
    Permission.viewRolesPermission = "systemsetting.role.view";
    Permission.manageRolesPermission = "systemsetting.role.manage";
    Permission.assignRolesPermission = "systemsetting.role.assign";
    Permission.viewFacilitiesPermission = "frsmgt.equipment.view";
    Permission.manageFacilitiesPermission = "frsmgt.equipment.manage";
    Permission.viewFacilityTypesPermission = "frsmgt.equipmenttype.view";
    Permission.manageFacilityTypesPermission = "frsmgt.equipmenttype.manage";
    Permission.viewSmartRoomSchedulerLogsPermission = "frsmgt.smartroomschedulerlog.view";
    Permission.addReservationsPermission = "frsmgt.calendar.view";
    Permission.manageReservationsPermission = "frsmgt.calendar.view";
    Permission.viewDevicesPermission = "sgnmanagement.devicemgt.device.view";
    Permission.manageDevicesPermission = "sgnmanagement.devicemgt.device.manage";
    Permission.approveDevicesPermission = "sgnmanagement.devicemgt.device.approve";
    Permission.viewLocationsPermission = "systemsetting.locationtree.view";
    Permission.manageLocationsPermission = "systemsetting.locationtree.manage";
    Permission.viewInstitutionsPermission = "systemsetting.institution.view";
    Permission.manageInstitutionsPermission = "systemsetting.institution.manage";
    Permission.viewDepartmentsPermission = "systemsetting.department.view";
    Permission.manageDepartmentsPermission = "systemsetting.department.manage";
    Permission.viewContactGroupsPermission = "frsmgt.accesscontrol.contactgroup.view";
    Permission.manageContactGroupsPermission = "frsmgt.accesscontrol.contactgroup.manage";
    Permission.viewUserPhonebooksPermission = "frsmgt.accesscontrol.phonebook.view";
    Permission.manageUserPhonebooksPermission = "frsmgt.accesscontrol.phonebook.manage";
    Permission.viewReportsPermission = "reportmgt.facilitymgt.view";
    Permission.viewVehicleLogsPermission = "reportmgt.facilitymgt.vehiclelog.view";
    Permission.viewOccupancyLogsPermission = "reportmgt.facilitymgt.occupancylog.view";
    Permission.viewPIBTemplatesPermission = "pibtemplates.view";
    Permission.managePIBTemplatesPermission = "pibtemplates.manage";
    Permission.viewPIBDevicesTemplatesPermission = "pibdevices.view";
    Permission.managePIBDevicesTemplatesPermission = "pibdevices.manage";
    Permission.viewEpaperTemplatesPermission = "sgnmanagement.epapermgt.view";
    Permission.manageEpaperTemplatesPermission = "sgnmanagement.epapermgt.manage";
    Permission.viewEpaperDevicesPermission = "epaperdevices.view";
    Permission.manageEpaperDevicesPermission = "epaperdevices.manage";
    Permission.viewDashboardPermission = "systemsetting.dashboard.view";
    Permission.manageUserVehiclesPermission = "frsmgt.accesscontrol.uservehicle.manage";
    //public static readonly viewCalendarPermission: PermissionValues = "calendar.view";
    Permission.approvePublicationsPermission = "frsmgt.accesscontrol.publication.approve";
    Permission.emailPublicationsPermission = "frsmgt.accesscontrol.publication.email";
    Permission.createComponentsPermission = "sgnmanagement.contentmanagement.component.create";
    Permission.updateComponentsPermission = "sgnmanagement.contentmanagement.component.update";
    Permission.deleteComponentsPermission = "sgnmanagement.contentmanagement.component.delete";
    Permission.createCompilationsPermission = "sgnmanagement.contentmanagement.compilation.create";
    Permission.updateCompilationsPermission = "sgnmanagement.contentmanagement.compilation.update";
    Permission.deleteCompilationsPermission = "sgnmanagement.contentmanagement.compilation.delete";
    Permission.createPublicationsPermission = "sgnmanagement.contentmanagement.publication.create";
    Permission.updatePublicationsPermission = "sgnmanagement.contentmanagement.publication.update";
    Permission.deletePublicationsPermission = "sgnmanagement.contentmanagement.publication.delete";
    Permission.viewSignageDashboardPermission = "systemsetting.dashboard.sgn.view";
    Permission.pushmessageSignageControlPermission = "systemsetting.dashboard.sgn.controls.pushmessage";
    Permission.previewSignageControlPermission = "systemsetting.dashboard.sgn.controls.preview";
    Permission.rebootSignageControlPermission = "systemsetting.dashboard.sgn.controls.reboot";
    Permission.refreshSignageControlPermission = "systemsetting.dashboard.sgn.controls.refresh";
    Permission.screenshotSignageControlPermission = "systemsetting.dashboard.sgn.controls.screenshot";
    Permission.deviceinfoSignageControlPermission = "systemsetting.dashboard.sgn.controls.deviceinfo";
    Permission.publicationSignageControlPermission = "systemsetting.dashboard.sgn.controls.publication";
    Permission.viewUserGroupsPermission = "systemsetting.usergroup.view";
    Permission.manageUserGroupsPermission = "systemsetting.usergroup.manage";
    Permission.viewAuthLogsPermission = "systemsetting.audit.authlog.view";
    Permission.viewUpDownTimeLogsPermission = "reportmgt.sgn.updownlog.view";
    Permission.viewDataLogsPermission = "systemsetting.audit.datalog.view";
    Permission.viewImageReferenceTypesPermission = "systemsetting.imagereferencetype.view";
    Permission.manageImageReferenceTypesPermission = "systemsetting.imagereferencetype.manage";
    Permission.viewImageReferenceColorsPermission = "systemsetting.imagereferencecolor.view";
    Permission.manageImageReferenceColorsPermission = "systemsetting.imagereferencecolor.manage";
    Permission.viewDeviceTypesPermission = "systemsetting.devicetype.view";
    Permission.manageDeviceTypesPermission = "systemsetting.devicetype.manage";
    Permission.viewAssetTypesPermission = "assetmgt.assettype.view";
    Permission.manageAssetTypesPermission = "assetmgt.assettype.manage";
    Permission.viewAssetModelsPermission = "assetmgt.assetmodel.view";
    Permission.manageAssetModelsPermission = "assetmgt.assetmodel.manage";
    Permission.viewAssetsPermission = "assetmgt.asset.view";
    Permission.manageAssetsPermission = "assetmgt.asset.manage";
    Permission.viewServiceContractsPermission = "assetmgt.servicecontract.view";
    Permission.manageServiceContractsPermission = "assetmgt.servicecontract.manage";
    //MEAL ORDER SYSTEM ACL
    Permission.viewMOSAccountMgtPermission = "mosmgt.accountmgt.view";
    Permission.viewMOSMealMgtPermission = "mosmgt.mealmgt.order.view";
    Permission.viewMOSOrgMgtPermission = "mosmgt.orgmgt.view";
    Permission.viewMOSSettingMgtPermission = "mosmgt.settingmgt.view";
    Permission.viewMOSReportMgtPermission = "mosmgt.reportmgt.view";
    Permission.viewMOSSysAdminMgtPermission = "mosmgt.sysadminmgt.view";
    Permission.viewMOSOrderMgtRestrictionTypesPermission = "mosmgt.mealmgt.order.restrictiontypes.view";
    Permission.viewMOSOrderMgtRestrictionsPermission = "mosmgt.mealmgt.order.restrictions.view";
    Permission.viewMOSOrderMgtCuisinesPermission = "mosmgt.mealmgt.order.cuisines.view";
    Permission.viewMOSOrderMgtDeliveryPermission = "mosmgt.mealmgt.order.delivery.view";
    Permission.viewMOSOrderMgtBentoBoxTypesPermission = "mosmgt.mealmgt.order.delivery.bentoboxtypes.view";
    Permission.viewMOSOrderMgtBentoAssetsPermission = "mosmgt.mealmgt.order.delivery.bentoassets.view";
    Permission.viewMOSOrderMgtCartonTypesPermission = "mosmgt.mealmgt.order.delivery.cartontypes.view";
    Permission.viewMOSOrderMgtCartonAssetsPermission = "mosmgt.mealmgt.order.delivery.cartonassets.view";
    Permission.viewMOSOrderMgtTrackingStatusPermission = "mosmgt.mealmgt.order.delivery.trackingstatus.view";
    Permission.viewMOSOrderMgtDeliveryOrdersPermission = "mosmgt.mealmgt.order.delivery.deliveryorders.view";
    Permission.viewMOSOrderMgtDishingProcessPermission = "mosmgt.mealmgt.order.delivery.dishingprocess.view";
    Permission.viewMOSOrderMgtPackingProcessPermission = "mosmgt.mealmgt.order.delivery.packingprocess.view";
    Permission.viewMOSOrderMgtDriversPermission = "mosmgt.mealmgt.order.delivery.drivers.view";
    Permission.viewMOSOrderMgtRoutesPermission = "mosmgt.mealmgt.order.delivery.routes.view";
    Permission.viewMOSOrderMgtCaterersPermission = "mosmgt.settingmgt.caterers.view";
    Permission.viewMOSOrderMgtCatererDishCalendarMenu = "mosmgt.settingmgt.caterers.dishcalendar.view";
    Permission.viewMOSOrderMgtCatererOutletsMenu = "mosmgt.settingmgt.caterers.outlets.view";
    Permission.viewMOSOrderMgtOutletProfilesPermission = "mosmgt.settingmgt.outletprofiles.view";
    Permission.viewMOSOrderMgtCatererMealTypesMenu = "mosmgt.settingmgt.caterers.mealtypes.view";
    Permission.viewMOSOrderMgtCatererMealPeriodsMenu = "mosmgt.settingmgt.caterers.mealperiods.view";
    Permission.viewMOSOrderMgtCatererDishTypesMenu = "mosmgt.settingmgt.caterers.dishtypes.view";
    Permission.viewMOSOrderMgtCatererDishesMenu = "mosmgt.settingmgt.caterers.dishes.view";
    Permission.viewMOSOrderMgtCatererDishCyclesMenu = "mosmgt.settingmgt.caterers.dishcycles.view";
    Permission.viewMOSOrderMgtOutletsPermission = "mosmgt.settingmgt.outlets.view";
    Permission.viewMOSOrderMgtStoreInfoPermission = "mosmgt.settingmgt.storeinfo.view";
    Permission.manageMOSOrderMgtRestrictionTypesPermission = "mosmgt.mealmgt.order.restrictiontypes.manage";
    Permission.manageMOSOrderMgtRestrictionsPermission = "mosmgt.mealmgt.order.restrictions.manage";
    Permission.manageMOSOrderMgtCuisinesPermission = "mosmgt.mealmgt.order.cuisines.manage";
    Permission.manageMOSOrderMgtDeliveryPermission = "mosmgt.mealmgt.order.delivery.manage";
    Permission.manageMOSOrderMgtBentoBoxTypesPermission = "mosmgt.mealmgt.order.delivery.bentoboxtypes.manage";
    Permission.manageMOSOrderMgtBentoAssetsPermission = "mosmgt.mealmgt.order.delivery.bentoassets.manage";
    Permission.manageMOSOrderMgtCartonTypesPermission = "mosmgt.mealmgt.order.delivery.cartontypes.manage";
    Permission.manageMOSOrderMgtCartonAssetsPermission = "mosmgt.mealmgt.order.delivery.cartonassets.manage";
    Permission.manageMOSOrderMgtTrackingStatusPermission = "mosmgt.mealmgt.order.delivery.trackingstatus.manage";
    Permission.manageMOSOrderMgtDeliveryOrdersPermission = "mosmgt.mealmgt.order.delivery.deliveryorders.manage";
    Permission.manageMOSOrderMgtDishingProcessPermission = "mosmgt.mealmgt.order.delivery.dishingprocess.manage";
    Permission.manageMOSOrderMgtPackingProcessPermission = "mosmgt.mealmgt.order.delivery.packingprocess.manage";
    Permission.manageMOSOrderMgtDriversPermission = "mosmgt.mealmgt.order.delivery.drivers.manage";
    Permission.manageMOSOrderMgtRoutesPermission = "mosmgt.mealmgt.order.delivery.routes.manage";
    Permission.manageMOSOrderMgtCaterersPermission = "mosmgt.settingmgt.caterers.manage";
    Permission.manageMOSOrderMgtOutletProfilesPermission = "mosmgt.settingmgt.outletprofiles.manage";
    Permission.manageMOSOrderMgtCatererMealTypesMenu = "mosmgt.settingmgt.caterers.mealtypes.manage";
    Permission.manageMOSOrderMgtCatererMealPeriodsMenu = "mosmgt.settingmgt.caterers.mealperiods.manage";
    Permission.manageMOSOrderMgtCatererDishTypesMenu = "mosmgt.settingmgt.caterers.dishtypes.manage";
    Permission.manageMOSOrderMgtCatererDishesMenu = "mosmgt.settingmgt.caterers.dishes.manage";
    Permission.manageMOSOrderMgtCatererDishCyclesMenu = "mosmgt.settingmgt.caterers.dishcycles.manage";
    Permission.manageMOSOrderMgtOutletsPermission = "mosmgt.settingmgt.outlets.manage";
    Permission.manageMOSOrderMgtStoreInfoPermission = "mosmgt.settingmgt.storeinfo.manage";
    Permission.manageMOSOrderMgtPaymentTypesPermission = "mosmgt.settingmgt.payment.paymenttype.manage";
    Permission.manageMOSOrderMgtTransactionFeesPermission = "mosmgt.settingmgt.payment.transactionfee.manage";
    Permission.manageMOSOrderMgtVoucherTypesPermission = "mosmgt.settingmgt.payment.vouchertype.manage";
    Permission.manageMOSOrderMgtVouchersPermission = "mosmgt.settingmgt.payment.voucher.manage";
    Permission.manageMOSOrderMgtWaiversPermission = "mosmgt.settingmgt.payment.waiver.manage";
    Permission.manageMOSOrderMgtContactUsSubjectsPermission = "mosmgt.settingmgt.contactus.subject.manage";
    Permission.manageMOSOrderMgtContactUsQuestionsPermission = "mosmgt.settingmgt.contactus.question.manage";
    Permission.manageMOSOrderMgtNotificationSettingsPermission = "mosmgt.settingmgt.notifications.settings.manage";
    Permission.manageMOSOrderMgtNotificationEventsPermission = "mosmgt.settingmgt.notifications.events.manage";
    // MOS SETTINGS MAIN MENU
    Permission.manageMOSOrderMgtCancelRequestsPermission = "mosmgt.settingmgt.orders.cancelrequest.manage";
    //Stores
    Permission.viewMOSOutletMgtStoresPermission = "mosmgt.outletmgt.stores.view";
    Permission.manageMOSOutletMgtStoresPermission = "mosmgt.outletmgt.stores.manage";
    //Caterers
    Permission.viewMOSOutletMgtCaterersPermission = "mosmgt.outletmgt.caterers.view";
    Permission.manageMOSOutletMgtCaterersPermission = "mosmgt.outletmgt.caterers.manage";
    //Reports
    Permission.viewMOSOutletMgtReportsPermission = "mosmgt.outletmgt.reports.view";
    //Meal Summary
    Permission.manageMOSOutletMgtMealSummaryPermission = "mosmgt.outletmgt.mealsummary.manage";
    //----OUTLET TABS-----
    //Terms
    Permission.viewMOSOutletMgtTermsPermission = "mosmgt.outletmgt.terms.view";
    Permission.manageMOSOutletMgtTermsPermission = "mosmgt.outletmgt.terms.manage";
    //Class Batches
    Permission.viewMOSOutletMgtClassBatchesPermission = "mosmgt.outletmgt.classbatches.view";
    Permission.manageMOSOutletMgtClassBatchesPermission = "mosmgt.outletmgt.classbatches.manage";
    //Class Levels
    Permission.viewMOSOutletMgtClassLevelsPermission = "mosmgt.outletmgt.levels.view";
    Permission.manageMOSOutletMgtClassLevelsPermission = "mosmgt.outletmgt.levels.manage";
    //Classes
    Permission.viewMOSOutletMgtClassesPermission = "mosmgt.outletmgt.classes.view";
    Permission.manageMOSOutletMgtClassesPermission = "mosmgt.outletmgt.classes.manage";
    //Students
    Permission.viewMOSOutletMgtStudentsPermission = "mosmgt.outletmgt.students.view";
    Permission.manageMOSOutletMgtStudentsPermission = "mosmgt.outletmgt.students.manage";
    //----STUDENT TABS----
    Permission.viewMOSOutletStudentMgtImportPermission = "mosmgt.outletmgt.students.import.view";
    Permission.manageMOSOutletStudentMgtPermissionNew = "mosmgt.outletmgt.students.manage.create";
    Permission.manageMOSOutletStudentMgtPermissionEdit = "mosmgt.outletmgt.students.manage.edit";
    Permission.manageMOSOutletStudentMgtPermissionDelete = "mosmgt.outletmgt.students.manage.delete";
    Permission.manageMOSOutletStudentMgtPermissionCreateAccount = "mosmgt.outletmgt.students.manage.account.create";
    Permission.manageMOSOutletStudentMgtPermissionOrderMgt = "mosmgt.outletmgt.students.manage.order";
    Permission.manageMOSOutletStudentMgtPermissionVoucher = "mosmgt.outletmgt.students.manage.voucher";
    Permission.manageMOSOutletStudentMgtPermissionNotification = "mosmgt.outletmgt.students.manage.notification";
    Permission.manageMOSOutletStudentMgtPermissionCalendar = "mosmgt.outletmgt.students.manage.calendar";
    //FAS
    Permission.viewMOSOutletMgtFasPermission = "mosmgt.outletmgt.fas.view";
    Permission.manageMOSOutletMgtFasPermission = "mosmgt.outletmgt.fas.manage";
    //Student Groups
    Permission.viewMOSOutletMgtStudentGroupsPermission = "mosmgt.outletmgt.studentgroups.view";
    Permission.manageMOSOutletMgtStudentGroupsPermission = "mosmgt.outletmgt.studentgroups.manage";
    //Menu Groups
    Permission.viewMOSOutletMgtMenusPermission = "mosmgt.outletmgt.menus.view";
    Permission.manageMOSOutletMgtMenusPermission = "mosmgt.outletmgt.menus.manage";
    //Order Cancellations
    Permission.viewMOSOutletMgtCancellationsPermission = "mosmgt.outletmgt.cancellations.view";
    Permission.manageMOSOutletMgtCancellationsPermission = "mosmgt.outletmgt.cancellations.manage";
    //Order Portal Content
    Permission.viewMOSOutletMgtPortalContentsPermission = "mosmgt.outletmgt.portalcontents.view";
    Permission.manageMOSOutletMgtPortalContentsPermission = "mosmgt.outletmgt.portalcontents.manage";
    //Email Templates
    Permission.viewMOSOutletMgtEmailTemplatesPermission = "mosmgt.outletmgt.emailtemplates.view";
    Permission.manageMOSOutletMgtEmailTemplatesPermission = "mosmgt.outletmgt.emailtemplates.manage";
    //Meal Allocations
    Permission.viewMOSOutletMgtMealAllocationsPermission = "mosmgt.outletmgt.mealallocations.view";
    Permission.manageMOSOutletMgtMealAllocationsPermission = "mosmgt.outletmgt.mealallocations.manage";
    //Packing Allocations
    Permission.viewMOSOutletMgtPackingAllocationsPermission = "mosmgt.outletmgt.packingallocations.view";
    Permission.manageMOSOutletMgtPackingAllocationsPermission = "mosmgt.outletmgt.packingallocations.manage";
    Permission.viewEmailQueuesPermission = "systemsetting.emailqueue.view";
    return Permission;
}());
exports.Permission = Permission;
var PermissionTree = /** @class */ (function () {
    function PermissionTree(id, value, type, children) {
        this.id = id;
        this.value = value;
        this.children = children;
    }
    return PermissionTree;
}());
exports.PermissionTree = PermissionTree;
//# sourceMappingURL=permission.model.js.map