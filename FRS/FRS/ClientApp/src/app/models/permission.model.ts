export type PermissionNames =
  "View Users" | "Manage Users" |
  "View Roles" | "Manage Roles" | "Assign Roles";

export type PermissionValues =
  "systemsetting.user.view" | "systemsetting.user.manage" |
  "systemsetting.role.view" | "systemsetting.role.manage" | "systemsetting.role.assign" |
  "frsmgt.equipment.view" | "frsmgt.equipment.manage" |
  "frsmgt.equipmenttype.view" | "frsmgt.equipmenttype.manage" | "frsmgt.smartroomschedulerlog.view" |
  "frsmgt.calendar.view" | "reservations.manage" |
  "sgnmanagement.devicemgt.device.view" | "sgnmanagement.devicemgt.device.manage" | "sgnmanagement.devicemgt.device.approve" |
  "systemsetting.locationtree.view" | "systemsetting.locationtree.manage" |
  "systemsetting.institution.view" | "systemsetting.institution.manage" |
  "systemsetting.department.view" | "systemsetting.department.manage" |
  "frsmgt.accesscontrol.contactgroup.view" | "frsmgt.accesscontrol.contactgroup.manage" |
  "frsmgt.accesscontrol.phonebook.view" | "frsmgt.accesscontrol.phonebook.manage" |
  "pibtemplates.view" | "pibtemplates.manage" |
  "pibdevices.view" | "pibdevices.manage" |
  "sgnmanagement.epapermgt.view" | "sgnmanagement.epapermgt.manage" |
  "epaperdevices.view" | "epaperdevices.manage" |
  //"calendar.view" |
  "systemsetting.dashboard.view" |
  "reportmgt.facilitymgt.view" | "reportmgt.facilitymgt.vehiclelog.view" | "reportmgt.facilitymgt.occupancylog.view" |
  "frsmgt.accesscontrol.uservehicle.manage" |
  "frsmgt.accesscontrol.publication.approve" |
  "frsmgt.accesscontrol.publication.email" |
  "systemsetting.dashboard.sgn.view" | "systemsetting.dashboard.sgn.controls.preview" | "systemsetting.dashboard.sgn.controls.reboot" | "systemsetting.dashboard.sgn.controls.refresh" | "systemsetting.dashboard.sgn.controls.screenshot" | "systemsetting.dashboard.sgn.controls.pushmessage" | "systemsetting.dashboard.sgn.controls.deviceinfo" | "systemsetting.dashboard.sgn.controls.publication" |
  "systemsetting.usergroup.view" | "systemsetting.usergroup.manage" |
  "systemsetting.imagereferencetype.view" | "systemsetting.imagereferencetype.manage" |
  "systemsetting.imagereferencecolor.view" | "systemsetting.imagereferencecolor.manage" |
  "systemsetting.devicetype.view" | "systemsetting.devicetype.manage" |
  "systemsetting.audit.view" | "systemsetting.audit.authlog.view" | "reportmgt.sgn.updownlog.view" | "systemsetting.audit.datalog.view" |
  "assetmgt.assettype.view" | "assetmgt.assettype.manage" |
  "assetmgt.assetmodel.view" | "assetmgt.assetmodel.manage" |
  "assetmgt.asset.view" | "assetmgt.asset.manage" |
  "assetmgt.servicecontract.view" | "assetmgt.servicecontract.manage" | "sgnmanagement.contentmanagement.component.create" | "sgnmanagement.contentmanagement.component.update" | "sgnmanagement.contentmanagement.component.delete" | "sgnmanagement.contentmanagement.compilation.create" | "sgnmanagement.contentmanagement.compilation.update" | "sgnmanagement.contentmanagement.compilation.delete" | "sgnmanagement.contentmanagement.publication.create" | "sgnmanagement.contentmanagement.publication.update" | "sgnmanagement.contentmanagement.publication.delete" |
  "systemsetting.emailqueue.view" |
  "mosmgt.accountmgt.view" |
  "mosmgt.mealmgt.order.view" |
  "mosmgt.orgmgt.view" |
  "mosmgt.settingmgt.view" |
  "mosmgt.reportmgt.view" |
  "mosmgt.sysadminmgt.view" |
  "mosmgt.mealmgt.order.restrictiontypes.view" |
  "mosmgt.mealmgt.order.restrictions.view" |
  "mosmgt.mealmgt.order.cuisines.view" |
  "mosmgt.mealmgt.order.delivery.view" |
  "mosmgt.mealmgt.order.delivery.bentoboxtypes.view" |
  "mosmgt.mealmgt.order.delivery.bentoassets.view" |
  "mosmgt.mealmgt.order.delivery.cartontypes.view" |
  "mosmgt.mealmgt.order.delivery.cartonassets.view" |
  "mosmgt.mealmgt.order.delivery.trackingstatus.view" |
  "mosmgt.mealmgt.order.delivery.deliveryorders.view" |
  "mosmgt.mealmgt.order.delivery.dishingprocess.view" |
  "mosmgt.mealmgt.order.delivery.packingprocess.view" |
  "mosmgt.mealmgt.order.delivery.drivers.view" |
  "mosmgt.mealmgt.order.delivery.routes.view" |
  "mosmgt.settingmgt.caterers.view" |
  "mosmgt.settingmgt.caterers.dishcalendar.view" |
  "mosmgt.settingmgt.caterers.outlets.view" |

  "mosmgt.settingmgt.outletprofiles.view" |
  "mosmgt.settingmgt.caterers.mealtypes.view" |
  "mosmgt.settingmgt.caterers.mealperiods.view" |
  "mosmgt.settingmgt.caterers.dishtypes.view" |
  "mosmgt.settingmgt.caterers.dishes.view" |
  "mosmgt.settingmgt.caterers.dishcycles.view" |

  "mosmgt.settingmgt.outlets.view" |
  "mosmgt.settingmgt.storeinfo.view" |
  "mosmgt.mealmgt.order.restrictiontypes.manage" |
  "mosmgt.mealmgt.order.restrictions.manage" |
  "mosmgt.mealmgt.order.cuisines.manage" |
  "mosmgt.mealmgt.order.delivery.manage" |
  "mosmgt.mealmgt.order.delivery.bentoboxtypes.manage" |
  "mosmgt.mealmgt.order.delivery.bentoassets.manage" |
  "mosmgt.mealmgt.order.delivery.cartontypes.manage" |
  "mosmgt.mealmgt.order.delivery.cartonassets.manage" |
  "mosmgt.mealmgt.order.delivery.trackingstatus.manage" |
  "mosmgt.mealmgt.order.delivery.deliveryorders.manage" |
  "mosmgt.mealmgt.order.delivery.dishingprocess.manage" |
  "mosmgt.mealmgt.order.delivery.packingprocess.manage" |
  "mosmgt.mealmgt.order.delivery.drivers.manage" |
  "mosmgt.mealmgt.order.delivery.routes.manage" |
  "mosmgt.settingmgt.caterers.manage" |
  "mosmgt.settingmgt.outletprofiles.manage" |

  "mosmgt.settingmgt.caterers.mealtypes.manage" |
  "mosmgt.settingmgt.caterers.mealperiods.manage" |
  "mosmgt.settingmgt.caterers.dishtypes.manage" |
  "mosmgt.settingmgt.caterers.dishes.manage" |
  "mosmgt.settingmgt.caterers.dishcycles.manage" |

  "mosmgt.settingmgt.outlets.manage" |
  "mosmgt.settingmgt.storeinfo.manage" |
  "mosmgt.settingmgt.payment.paymenttype.manage" |
  "mosmgt.settingmgt.payment.transactionfee.manage" |
  "mosmgt.settingmgt.payment.vouchertype.manage" |
  "mosmgt.settingmgt.payment.voucher.manage" |
  "mosmgt.settingmgt.payment.voucheradministration.manage" |
  "mosmgt.settingmgt.payment.waiver.manage" |
  "mosmgt.outletmgt.terms.view" |
  "mosmgt.outletmgt.terms.manage" |
  "mosmgt.outletmgt.classbatches.view" |
  "mosmgt.outletmgt.classbatches.manage" |
  "mosmgt.outletmgt.levels.view" |
  "mosmgt.outletmgt.levels.manage" |
  "mosmgt.outletmgt.classes.view" |
  "mosmgt.outletmgt.classes.manage" |
  "mosmgt.outletmgt.students.view" |
  "mosmgt.outletmgt.students.manage" |
  "mosmgt.outletmgt.fas.view" |
  "mosmgt.outletmgt.fas.manage" |
  "mosmgt.outletmgt.studentgroups.view" |
  "mosmgt.outletmgt.studentgroups.manage" |
  "mosmgt.outletmgt.menus.view" |
  "mosmgt.outletmgt.menus.manage" |
  "mosmgt.outletmgt.cancellations.view" |
  "mosmgt.outletmgt.cancellations.manage" |
  "mosmgt.outletmgt.portalcontents.view" |
  "mosmgt.outletmgt.portalcontents.manage" |
  "mosmgt.outletmgt.emailtemplates.view" |
  "mosmgt.outletmgt.emailtemplates.manage" |
  "mosmgt.outletmgt.mealallocations.view" |
  "mosmgt.outletmgt.mealallocations.manage" |
  "mosmgt.outletmgt.packingallocations.view" |
  "mosmgt.outletmgt.packingallocations.manage" |
  "mosmgt.settingmgt.contactus.subject.manage" |
  "mosmgt.settingmgt.contactus.question.manage" |
  "mosmgt.settingmgt.notifications.settings.manage" |
  "mosmgt.settingmgt.notifications.events.manage" |
  "mosmgt.settingmgt.orders.cancelrequest.manage" |
  "mosmgt.settingmgt.student.wallet.manage" |

  "mosmgt.outletmgt.stores.view" |
  "mosmgt.outletmgt.stores.manage" |

  "mosmgt.outletmgt.caterers.view" |
  "mosmgt.outletmgt.caterers.manage" |

  "mosmgt.outletmgt.reports.view" |
  "mosmgt.outletmgt.reports.delivery.view" |
  "mosmgt.outletmgt.reports.salesorderdetail.view" |
  "mosmgt.outletmgt.reports.ordercollection.view" |
  "mosmgt.outletmgt.reports.orderpayment.view" |
  "mosmgt.outletmgt.reports.voucherutilisation.view" |
  "mosmgt.outletmgt.reports.bentousage.view" |
  "mosmgt.outletmgt.mealsummary.manage" |

  "mosmgt.outletmgt.students.import.view" |
  "mosmgt.outletmgt.students.manage.create" |
  "mosmgt.outletmgt.students.manage.edit" |
  "mosmgt.outletmgt.students.manage.delete" |
  "mosmgt.outletmgt.students.manage.account.create" |
  "mosmgt.outletmgt.students.manage.order" |
  "mosmgt.outletmgt.students.manage.voucher" |
  "mosmgt.outletmgt.students.manage.notification" |
  "mosmgt.outletmgt.students.manage.calendar" |

  "mosmgt.settingmgt.faq.subject.view" |
  "mosmgt.settingmgt.faq.subject.manage" |
  "mosmgt.settingmgt.faq.detail.view" |
  "mosmgt.settingmgt.faq.detail.manage" 
;

export class Permission {

  public static readonly viewUsersPermission: PermissionValues = "systemsetting.user.view";
  public static readonly manageUsersPermission: PermissionValues = "systemsetting.user.manage";

  public static readonly viewRolesPermission: PermissionValues = "systemsetting.role.view";
  public static readonly manageRolesPermission: PermissionValues = "systemsetting.role.manage";
  public static readonly assignRolesPermission: PermissionValues = "systemsetting.role.assign";

  public static readonly viewFacilitiesPermission: PermissionValues = "frsmgt.equipment.view";
  public static readonly manageFacilitiesPermission: PermissionValues = "frsmgt.equipment.manage";

  public static readonly viewFacilityTypesPermission: PermissionValues = "frsmgt.equipmenttype.view";
  public static readonly manageFacilityTypesPermission: PermissionValues = "frsmgt.equipmenttype.manage";

  public static readonly viewSmartRoomSchedulerLogsPermission: PermissionValues = "frsmgt.smartroomschedulerlog.view";

  public static readonly addReservationsPermission: PermissionValues = "frsmgt.calendar.view";
  public static readonly manageReservationsPermission: PermissionValues = "frsmgt.calendar.view";

  public static readonly viewDevicesPermission: PermissionValues = "sgnmanagement.devicemgt.device.view";
  public static readonly manageDevicesPermission: PermissionValues = "sgnmanagement.devicemgt.device.manage";
  public static readonly approveDevicesPermission: PermissionValues = "sgnmanagement.devicemgt.device.approve";

  public static readonly viewLocationsPermission: PermissionValues = "systemsetting.locationtree.view";
  public static readonly manageLocationsPermission: PermissionValues = "systemsetting.locationtree.manage";

  public static readonly viewInstitutionsPermission: PermissionValues = "systemsetting.institution.view";
  public static readonly manageInstitutionsPermission: PermissionValues = "systemsetting.institution.manage";

  public static readonly viewDepartmentsPermission: PermissionValues = "systemsetting.department.view";
  public static readonly manageDepartmentsPermission: PermissionValues = "systemsetting.department.manage";

  public static readonly viewContactGroupsPermission: PermissionValues = "frsmgt.accesscontrol.contactgroup.view";
  public static readonly manageContactGroupsPermission: PermissionValues = "frsmgt.accesscontrol.contactgroup.manage";

  public static readonly viewUserPhonebooksPermission: PermissionValues = "frsmgt.accesscontrol.phonebook.view";
  public static readonly manageUserPhonebooksPermission: PermissionValues = "frsmgt.accesscontrol.phonebook.manage";

  public static readonly viewReportsPermission: PermissionValues = "reportmgt.facilitymgt.view";
  public static readonly viewVehicleLogsPermission: PermissionValues = "reportmgt.facilitymgt.vehiclelog.view";
  public static readonly viewOccupancyLogsPermission: PermissionValues = "reportmgt.facilitymgt.occupancylog.view";

  public static readonly viewPIBTemplatesPermission: PermissionValues = "pibtemplates.view";
  public static readonly managePIBTemplatesPermission: PermissionValues = "pibtemplates.manage";

  public static readonly viewPIBDevicesTemplatesPermission: PermissionValues = "pibdevices.view";
  public static readonly managePIBDevicesTemplatesPermission: PermissionValues = "pibdevices.manage";

  public static readonly viewEpaperTemplatesPermission: PermissionValues = "sgnmanagement.epapermgt.view";
  public static readonly manageEpaperTemplatesPermission: PermissionValues = "sgnmanagement.epapermgt.manage";

  public static readonly viewEpaperDevicesPermission: PermissionValues = "epaperdevices.view";
  public static readonly manageEpaperDevicesPermission: PermissionValues = "epaperdevices.manage";

  public static readonly viewDashboardPermission: PermissionValues = "systemsetting.dashboard.view";

  public static readonly manageUserVehiclesPermission: PermissionValues = "frsmgt.accesscontrol.uservehicle.manage";
  //public static readonly viewCalendarPermission: PermissionValues = "calendar.view";

  public static readonly approvePublicationsPermission: PermissionValues = "frsmgt.accesscontrol.publication.approve";
  public static readonly emailPublicationsPermission: PermissionValues = "frsmgt.accesscontrol.publication.email";
  public static readonly createComponentsPermission: PermissionValues = "sgnmanagement.contentmanagement.component.create";
  public static readonly updateComponentsPermission: PermissionValues = "sgnmanagement.contentmanagement.component.update";
  public static readonly deleteComponentsPermission: PermissionValues = "sgnmanagement.contentmanagement.component.delete";

  public static readonly createCompilationsPermission: PermissionValues = "sgnmanagement.contentmanagement.compilation.create";
  public static readonly updateCompilationsPermission: PermissionValues = "sgnmanagement.contentmanagement.compilation.update";
  public static readonly deleteCompilationsPermission: PermissionValues = "sgnmanagement.contentmanagement.compilation.delete";

  public static readonly createPublicationsPermission: PermissionValues = "sgnmanagement.contentmanagement.publication.create";
  public static readonly updatePublicationsPermission: PermissionValues = "sgnmanagement.contentmanagement.publication.update";
  public static readonly deletePublicationsPermission: PermissionValues = "sgnmanagement.contentmanagement.publication.delete";

  public static readonly viewSignageDashboardPermission: PermissionValues = "systemsetting.dashboard.sgn.view";
  public static readonly pushmessageSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.pushmessage";
  public static readonly previewSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.preview";
  public static readonly rebootSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.reboot";
  public static readonly refreshSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.refresh";
  public static readonly screenshotSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.screenshot";
  public static readonly deviceinfoSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.deviceinfo";
  public static readonly publicationSignageControlPermission: PermissionValues = "systemsetting.dashboard.sgn.controls.publication";

  public static readonly viewUserGroupsPermission: PermissionValues = "systemsetting.usergroup.view";
  public static readonly manageUserGroupsPermission: PermissionValues = "systemsetting.usergroup.manage";

  public static readonly viewAuthLogsPermission: PermissionValues = "systemsetting.audit.authlog.view";
  public static readonly viewUpDownTimeLogsPermission: PermissionValues = "reportmgt.sgn.updownlog.view";
  public static readonly viewDataLogsPermission: PermissionValues = "systemsetting.audit.datalog.view";

  public static readonly viewImageReferenceTypesPermission: PermissionValues = "systemsetting.imagereferencetype.view";
  public static readonly manageImageReferenceTypesPermission: PermissionValues = "systemsetting.imagereferencetype.manage";

  public static readonly viewImageReferenceColorsPermission: PermissionValues = "systemsetting.imagereferencecolor.view";
  public static readonly manageImageReferenceColorsPermission: PermissionValues = "systemsetting.imagereferencecolor.manage";

  public static readonly viewDeviceTypesPermission: PermissionValues = "systemsetting.devicetype.view";
  public static readonly manageDeviceTypesPermission: PermissionValues = "systemsetting.devicetype.manage";

  public static readonly viewAssetTypesPermission: PermissionValues = "assetmgt.assettype.view";
  public static readonly manageAssetTypesPermission: PermissionValues = "assetmgt.assettype.manage";

  public static readonly viewAssetModelsPermission: PermissionValues = "assetmgt.assetmodel.view";
  public static readonly manageAssetModelsPermission: PermissionValues = "assetmgt.assetmodel.manage";

  public static readonly viewAssetsPermission: PermissionValues = "assetmgt.asset.view";
  public static readonly manageAssetsPermission: PermissionValues = "assetmgt.asset.manage";

  public static readonly viewServiceContractsPermission: PermissionValues = "assetmgt.servicecontract.view";
  public static readonly manageServiceContractsPermission: PermissionValues = "assetmgt.servicecontract.manage";

  //MEAL ORDER SYSTEM ACL
  public static readonly viewMOSAccountMgtPermission: PermissionValues = "mosmgt.accountmgt.view";
  public static readonly viewMOSMealMgtPermission: PermissionValues = "mosmgt.mealmgt.order.view";
  public static readonly viewMOSOrgMgtPermission: PermissionValues = "mosmgt.orgmgt.view";
  public static readonly viewMOSSettingMgtPermission: PermissionValues = "mosmgt.settingmgt.view";
  public static readonly viewMOSReportMgtPermission: PermissionValues = "mosmgt.reportmgt.view";
  public static readonly viewMOSSysAdminMgtPermission: PermissionValues = "mosmgt.sysadminmgt.view";

  public static readonly viewMOSOrderMgtRestrictionTypesPermission: PermissionValues = "mosmgt.mealmgt.order.restrictiontypes.view";
  public static readonly viewMOSOrderMgtRestrictionsPermission: PermissionValues = "mosmgt.mealmgt.order.restrictions.view";
  public static readonly viewMOSOrderMgtCuisinesPermission: PermissionValues = "mosmgt.mealmgt.order.cuisines.view";
  public static readonly viewMOSOrderMgtDeliveryPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.view";
  public static readonly viewMOSOrderMgtBentoBoxTypesPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.bentoboxtypes.view";
  public static readonly viewMOSOrderMgtBentoAssetsPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.bentoassets.view";
  public static readonly viewMOSOrderMgtCartonTypesPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.cartontypes.view";
  public static readonly viewMOSOrderMgtCartonAssetsPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.cartonassets.view";
  public static readonly viewMOSOrderMgtTrackingStatusPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.trackingstatus.view";
  public static readonly viewMOSOrderMgtDeliveryOrdersPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.deliveryorders.view";
  public static readonly viewMOSOrderMgtDishingProcessPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.dishingprocess.view";
  public static readonly viewMOSOrderMgtPackingProcessPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.packingprocess.view";
  public static readonly viewMOSOrderMgtDriversPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.drivers.view";
  public static readonly viewMOSOrderMgtRoutesPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.routes.view";
  public static readonly viewMOSOrderMgtCaterersPermission: PermissionValues = "mosmgt.settingmgt.caterers.view";
  public static readonly viewMOSOrderMgtCatererDishCalendarMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishcalendar.view";
  public static readonly viewMOSOrderMgtCatererOutletsMenu: PermissionValues = "mosmgt.settingmgt.caterers.outlets.view";
  public static readonly viewMOSOrderMgtOutletProfilesPermission: PermissionValues = "mosmgt.settingmgt.outletprofiles.view";
  public static readonly viewMOSOrderMgtCatererMealTypesMenu: PermissionValues = "mosmgt.settingmgt.caterers.mealtypes.view";
  public static readonly viewMOSOrderMgtCatererMealPeriodsMenu: PermissionValues = "mosmgt.settingmgt.caterers.mealperiods.view";
  public static readonly viewMOSOrderMgtCatererDishTypesMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishtypes.view";
  public static readonly viewMOSOrderMgtCatererDishesMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishes.view";
  public static readonly viewMOSOrderMgtCatererDishCyclesMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishcycles.view";

  public static readonly viewMOSOrderMgtOutletsPermission: PermissionValues = "mosmgt.settingmgt.outlets.view";
  public static readonly viewMOSOrderMgtStoreInfoPermission: PermissionValues = "mosmgt.settingmgt.storeinfo.view";
  public static readonly manageMOSOrderMgtRestrictionTypesPermission: PermissionValues = "mosmgt.mealmgt.order.restrictiontypes.manage";
  public static readonly manageMOSOrderMgtRestrictionsPermission: PermissionValues = "mosmgt.mealmgt.order.restrictions.manage";
  public static readonly manageMOSOrderMgtCuisinesPermission: PermissionValues = "mosmgt.mealmgt.order.cuisines.manage";
  public static readonly manageMOSOrderMgtDeliveryPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.manage";
  public static readonly manageMOSOrderMgtBentoBoxTypesPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.bentoboxtypes.manage";
  public static readonly manageMOSOrderMgtBentoAssetsPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.bentoassets.manage";
  public static readonly manageMOSOrderMgtCartonTypesPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.cartontypes.manage";
  public static readonly manageMOSOrderMgtCartonAssetsPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.cartonassets.manage";
  public static readonly manageMOSOrderMgtTrackingStatusPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.trackingstatus.manage";
  public static readonly manageMOSOrderMgtDeliveryOrdersPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.deliveryorders.manage";
  public static readonly manageMOSOrderMgtDishingProcessPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.dishingprocess.manage";
  public static readonly manageMOSOrderMgtPackingProcessPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.packingprocess.manage";
  public static readonly manageMOSOrderMgtDriversPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.drivers.manage";
  public static readonly manageMOSOrderMgtRoutesPermission: PermissionValues = "mosmgt.mealmgt.order.delivery.routes.manage";
  public static readonly manageMOSOrderMgtCaterersPermission: PermissionValues = "mosmgt.settingmgt.caterers.manage";
  public static readonly manageMOSOrderMgtOutletProfilesPermission: PermissionValues = "mosmgt.settingmgt.outletprofiles.manage";

  public static readonly manageMOSOrderMgtCatererMealTypesMenu: PermissionValues = "mosmgt.settingmgt.caterers.mealtypes.manage";
  public static readonly manageMOSOrderMgtCatererMealPeriodsMenu: PermissionValues = "mosmgt.settingmgt.caterers.mealperiods.manage";
  public static readonly manageMOSOrderMgtCatererDishTypesMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishtypes.manage";
  public static readonly manageMOSOrderMgtCatererDishesMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishes.manage";
  public static readonly manageMOSOrderMgtCatererDishCyclesMenu: PermissionValues = "mosmgt.settingmgt.caterers.dishcycles.manage";

  public static readonly manageMOSOrderMgtOutletsPermission: PermissionValues = "mosmgt.settingmgt.outlets.manage";
  public static readonly manageMOSOrderMgtStoreInfoPermission: PermissionValues = "mosmgt.settingmgt.storeinfo.manage";
  public static readonly manageMOSOrderMgtPaymentTypesPermission: PermissionValues = "mosmgt.settingmgt.payment.paymenttype.manage";
  public static readonly manageMOSOrderMgtTransactionFeesPermission: PermissionValues = "mosmgt.settingmgt.payment.transactionfee.manage";
  public static readonly manageMOSOrderMgtVoucherTypesPermission: PermissionValues = "mosmgt.settingmgt.payment.vouchertype.manage";
  public static readonly manageMOSOrderMgtVouchersPermission: PermissionValues = "mosmgt.settingmgt.payment.voucher.manage";
  public static readonly manageMOSOrderMgtVoucherUnAssignPermission: PermissionValues = "mosmgt.settingmgt.payment.voucheradministration.manage";
  public static readonly manageMOSOrderMgtWaiversPermission: PermissionValues = "mosmgt.settingmgt.payment.waiver.manage";
  public static readonly manageMOSOrderMgtContactUsSubjectsPermission: PermissionValues = "mosmgt.settingmgt.contactus.subject.manage";
  public static readonly manageMOSOrderMgtContactUsQuestionsPermission: PermissionValues = "mosmgt.settingmgt.contactus.question.manage";
  public static readonly manageMOSOrderMgtNotificationSettingsPermission: PermissionValues = "mosmgt.settingmgt.notifications.settings.manage";
  public static readonly manageMOSOrderMgtNotificationEventsPermission: PermissionValues = "mosmgt.settingmgt.notifications.events.manage";

  public static readonly manageMOSOrderMgtWalletTransactionsPermission: PermissionValues = "mosmgt.settingmgt.student.wallet.manage";

  // MOS SETTINGS MAIN MENU

  public static readonly manageMOSOrderMgtCancelRequestsPermission: PermissionValues = "mosmgt.settingmgt.orders.cancelrequest.manage";
  //Stores
  public static readonly viewMOSOutletMgtStoresPermission: PermissionValues = "mosmgt.outletmgt.stores.view";
  public static readonly manageMOSOutletMgtStoresPermission: PermissionValues = "mosmgt.outletmgt.stores.manage";

  //Caterers
  public static readonly viewMOSOutletMgtCaterersPermission: PermissionValues = "mosmgt.outletmgt.caterers.view";
  public static readonly manageMOSOutletMgtCaterersPermission: PermissionValues = "mosmgt.outletmgt.caterers.manage";

  //Reports
  public static readonly viewMOSOutletMgtReportsPermission: PermissionValues = "mosmgt.outletmgt.reports.view";
  public static readonly viewMOSOutletMgtReportsDeliveryPermission: PermissionValues = "mosmgt.outletmgt.reports.delivery.view"; 
  public static readonly viewMOSOutletMgtReportsSalesOrderDetailPermission: PermissionValues = "mosmgt.outletmgt.reports.salesorderdetail.view";
  public static readonly viewMOSOutletMgtReportsOrderCollectionPermission: PermissionValues = "mosmgt.outletmgt.reports.ordercollection.view";
  public static readonly viewMOSOutletMgtReportsOrderPaymentPermission: PermissionValues = "mosmgt.outletmgt.reports.orderpayment.view";
  public static readonly viewMOSOutletMgtReportsVoucherUtilisationPermission: PermissionValues = "mosmgt.outletmgt.reports.voucherutilisation.view";
  public static readonly viewMOSOutletMgtReportsBentoUsagePermission: PermissionValues = "mosmgt.outletmgt.reports.bentousage.view";

  //Meal Summary
  public static readonly manageMOSOutletMgtMealSummaryPermission: PermissionValues = "mosmgt.outletmgt.mealsummary.manage";

  //----OUTLET TABS-----

  //Terms
  public static readonly viewMOSOutletMgtTermsPermission: PermissionValues = "mosmgt.outletmgt.terms.view";
  public static readonly manageMOSOutletMgtTermsPermission: PermissionValues = "mosmgt.outletmgt.terms.manage";

  //Class Batches
  public static readonly viewMOSOutletMgtClassBatchesPermission: PermissionValues = "mosmgt.outletmgt.classbatches.view";
  public static readonly manageMOSOutletMgtClassBatchesPermission: PermissionValues = "mosmgt.outletmgt.classbatches.manage";

  //Class Levels
  public static readonly viewMOSOutletMgtClassLevelsPermission: PermissionValues = "mosmgt.outletmgt.levels.view";
  public static readonly manageMOSOutletMgtClassLevelsPermission: PermissionValues = "mosmgt.outletmgt.levels.manage";

  //Classes
  public static readonly viewMOSOutletMgtClassesPermission: PermissionValues = "mosmgt.outletmgt.classes.view";
  public static readonly manageMOSOutletMgtClassesPermission: PermissionValues = "mosmgt.outletmgt.classes.manage";

  //Students
  public static readonly viewMOSOutletMgtStudentsPermission: PermissionValues = "mosmgt.outletmgt.students.view";
  public static readonly manageMOSOutletMgtStudentsPermission: PermissionValues = "mosmgt.outletmgt.students.manage";

  //----STUDENT TABS----
  public static readonly viewMOSOutletStudentMgtImportPermission: PermissionValues = "mosmgt.outletmgt.students.import.view";
  public static readonly manageMOSOutletStudentMgtPermissionNew: PermissionValues = "mosmgt.outletmgt.students.manage.create";
  public static readonly manageMOSOutletStudentMgtPermissionEdit: PermissionValues = "mosmgt.outletmgt.students.manage.edit";
  public static readonly manageMOSOutletStudentMgtPermissionDelete: PermissionValues = "mosmgt.outletmgt.students.manage.delete";
  public static readonly manageMOSOutletStudentMgtPermissionCreateAccount: PermissionValues = "mosmgt.outletmgt.students.manage.account.create";
  public static readonly manageMOSOutletStudentMgtPermissionOrderMgt: PermissionValues = "mosmgt.outletmgt.students.manage.order";
  public static readonly manageMOSOutletStudentMgtPermissionVoucher: PermissionValues = "mosmgt.outletmgt.students.manage.voucher";
  public static readonly manageMOSOutletStudentMgtPermissionNotification: PermissionValues = "mosmgt.outletmgt.students.manage.notification";
  public static readonly manageMOSOutletStudentMgtPermissionCalendar: PermissionValues = "mosmgt.outletmgt.students.manage.calendar";

  //FAS
  public static readonly viewMOSOutletMgtFasPermission: PermissionValues = "mosmgt.outletmgt.fas.view";
  public static readonly manageMOSOutletMgtFasPermission: PermissionValues = "mosmgt.outletmgt.fas.manage";

  //Student Groups
  public static readonly viewMOSOutletMgtStudentGroupsPermission: PermissionValues = "mosmgt.outletmgt.studentgroups.view";
  public static readonly manageMOSOutletMgtStudentGroupsPermission: PermissionValues = "mosmgt.outletmgt.studentgroups.manage";

  //Menu Groups
  public static readonly viewMOSOutletMgtMenusPermission: PermissionValues = "mosmgt.outletmgt.menus.view";
  public static readonly manageMOSOutletMgtMenusPermission: PermissionValues = "mosmgt.outletmgt.menus.manage";

  //Order Cancellations
  public static readonly viewMOSOutletMgtCancellationsPermission: PermissionValues = "mosmgt.outletmgt.cancellations.view";
  public static readonly manageMOSOutletMgtCancellationsPermission: PermissionValues = "mosmgt.outletmgt.cancellations.manage";

  //Order Portal Content
  public static readonly viewMOSOutletMgtPortalContentsPermission: PermissionValues = "mosmgt.outletmgt.portalcontents.view";
  public static readonly manageMOSOutletMgtPortalContentsPermission: PermissionValues = "mosmgt.outletmgt.portalcontents.manage";

  //Email Templates
  public static readonly viewMOSOutletMgtEmailTemplatesPermission: PermissionValues = "mosmgt.outletmgt.emailtemplates.view";
  public static readonly manageMOSOutletMgtEmailTemplatesPermission: PermissionValues = "mosmgt.outletmgt.emailtemplates.manage";

  //Meal Allocations
  public static readonly viewMOSOutletMgtMealAllocationsPermission: PermissionValues = "mosmgt.outletmgt.mealallocations.view";
  public static readonly manageMOSOutletMgtMealAllocationsPermission: PermissionValues = "mosmgt.outletmgt.mealallocations.manage";

  //Packing Allocations
  public static readonly viewMOSOutletMgtPackingAllocationsPermission: PermissionValues = "mosmgt.outletmgt.packingallocations.view";
  public static readonly manageMOSOutletMgtPackingAllocationsPermission: PermissionValues = "mosmgt.outletmgt.packingallocations.manage";

  public static readonly viewEmailQueuesPermission: PermissionValues = "systemsetting.emailqueue.view";

  // FAQ
  public static readonly viewMOSOrderMgtFaqSubjectsPermission: PermissionValues = "mosmgt.settingmgt.faq.subject.view";
  public static readonly manageMOSOrderMgtFaqSubjectsPermission: PermissionValues = "mosmgt.settingmgt.faq.subject.manage";

  public static readonly viewMOSOrderMgtFaqDetailsPermission: PermissionValues = "mosmgt.settingmgt.faq.detail.view";
  public static readonly manageMOSOrderMgtFaqDetailsPermission: PermissionValues = "mosmgt.settingmgt.faq.detail.manage";

  constructor(name?: PermissionNames, value?: PermissionValues, groupName?: string, description?: string) {
    this.name = name;
    this.value = value;
    this.groupName = groupName;
    this.description = description;
  }

  public name: PermissionNames;
  public value: PermissionValues;
  public groupName: string;
  public description: string;
}


export class PermissionTree {

  constructor(id?: number, value?: string, type?: string, children?: PermissionTree[]) {
    this.id = id;
    this.value = value;
    this.children = children;
  }

  public id: number;
  public value: string;
  public type: string;
  public parent: PermissionTree;
  public children: PermissionTree[];
}

