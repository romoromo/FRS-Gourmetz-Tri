"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.CommonEndpoint = void 0;
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var operators_1 = require("rxjs/operators");
var endpoint_factory_service_1 = require("./endpoint-factory.service");
var CommonEndpoint = /** @class */ (function (_super) {
    __extends(CommonEndpoint, _super);
    function CommonEndpoint(http, configurations, injector) {
        var _this = _super.call(this, http, configurations, injector) || this;
        _this._facilitiesUrl = "/api/facility";
        _this._facilityByFacilityNameUrl = "/api/facility/name";
        _this._facilityTypesUrl = "/api/facilitytype";
        _this._facilityTypeByFacilityTypeNameUrl = "/api/facilitytype/name";
        return _this;
    }
    Object.defineProperty(CommonEndpoint.prototype, "facilitiesUrl", {
        get: function () { return this.configurations.baseUrl + this._facilitiesUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(CommonEndpoint.prototype, "facilityByFacilityNameUrl", {
        get: function () { return this.configurations.baseUrl + this._facilityByFacilityNameUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(CommonEndpoint.prototype, "facilityTypesUrl", {
        get: function () { return this.configurations.baseUrl + this._facilityTypesUrl; },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(CommonEndpoint.prototype, "facilityTypeByFacilityTypeNameUrl", {
        get: function () { return this.configurations.baseUrl + this._facilityTypeByFacilityTypeNameUrl; },
        enumerable: false,
        configurable: true
    });
    //Common methods
    CommonEndpoint.prototype.getByInstitutionId = function (url, id) {
        //let endpointUrl = this.configurations.baseUrl + url + "?institutionId=" + id;
        var _this = this;
        var endpointUrl = url + "?institutionId=" + id;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getByInstitutionId(url, id); });
        }));
    };
    CommonEndpoint.prototype.getById = function (url, id) {
        var _this = this;
        //let endpointUrl = this.configurations.baseUrl + url + "/id/" + id;
        var endpointUrl = url + "/id/" + id;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getById(url, id); });
        }));
    };
    CommonEndpoint.prototype.getSieve = function (url, filter) {
        var _this = this;
        //let endpointUrl = isTrueUrl ? url : this.configurations.baseUrl + url;
        var headers = this.getRequestHeaders();
        var httpParams = { fromObject: filter };
        var options = { params: new http_1.HttpParams(httpParams), headers: headers.headers };
        return this.http.get(url, options).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getSieve(url, filter); });
        }));
    };
    CommonEndpoint.prototype.get = function (url, isPost, obj, isParams, isTrueUrl) {
        var _this = this;
        //let endpointUrl = isTrueUrl ? url : this.configurations.baseUrl + url;
        var endpointUrl = url;
        if (isParams) {
            var params = new http_1.HttpParams()
                .append('templateBody', obj);
            var requestBody = params.toString();
            return this.http.post(endpointUrl, requestBody, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
                return _this.handleError(error, function () { return _this.getRefreshLoginEndpoint(); });
            }));
        }
        else {
            if (isPost) {
                return this.http.post(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
                    return _this.handleError(error, function () { return _this.get(url, isPost, obj); });
                }));
            }
            else {
                return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
                    return _this.handleError(error, function () { return _this.getPagedList(url); });
                }));
            }
        }
    };
    CommonEndpoint.prototype.getFile = function (url, obj) {
        var _this = this;
        var endpointUrl = url;
        var headers = this.getRequestHeaders();
        var httpParams = { fromObject: obj };
        var options = { params: new http_1.HttpParams(httpParams), headers: headers.headers, responseType: 'blob' };
        return this.http.post(endpointUrl, JSON.stringify(obj), options).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getFile(url, obj); });
        }));
    };
    CommonEndpoint.prototype.getFileget = function (url) {
        var _this = this;
        var endpointUrl = url;
        var headers = this.getRequestHeaders();
        var options = { headers: headers.headers, responseType: 'blob' };
        return this.http.get(endpointUrl, options).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getFileget(url); });
        }));
    };
    CommonEndpoint.prototype.getPagedList = function (url, page, pageSize, isPost, obj) {
        var _this = this;
        //url = this.configurations.baseUrl + url;
        var endpointUrl = page && pageSize ? url + "/" + page + "/" + pageSize : url;
        if (isPost) {
            return this.http.post(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
                return _this.handleError(error, function () { return _this.getPagedList(url, page, pageSize, isPost, obj); });
            }));
        }
        else {
            return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
                return _this.handleError(error, function () { return _this.getPagedList(url, page, pageSize); });
            }));
        }
    };
    CommonEndpoint.prototype.getNewEndpoint = function (url, obj) {
        var _this = this;
        //url = this.configurations.baseUrl + url;
        return this.http.post(url, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewEndpoint(url, obj); });
        }));
    };
    CommonEndpoint.prototype.getUpdateEndpoint = function (url, obj, id) {
        var _this = this;
        //url = this.configurations.baseUrl + url;
        var endpointUrl = url + "/update/" + id;
        return this.http.put(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateEndpoint(url, obj, id); });
        }));
    };
    CommonEndpoint.prototype.getUpdateStoreEndpoint = function (url, obj, id) {
        var _this = this;
        //url = this.configurations.baseUrl + url;
        var endpointUrl = url + "/updatestores/" + id;
        return this.http.put(endpointUrl, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateStoreEndpoint(url, obj, id); });
        }));
    };
    CommonEndpoint.prototype.getDeleteEndpoint = function (url, id, query) {
        var _this = this;
        //url = this.configurations.baseUrl + url;
        var endpointUrl = query ? url + "/delete/" + id + "?" + query : url + "/delete/" + id;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteEndpoint(url, id, query); });
        }));
    };
    //Facilities methods
    CommonEndpoint.prototype.getFacilitiesEndpoint = function (page, pageSize, institutionId) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.facilitiesUrl + "/facilities/list/" + page + "/" + pageSize : this.facilitiesUrl + "/facilities/list?institutionId=" + institutionId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getFacilitiesEndpoint(page, pageSize); });
        }));
    };
    CommonEndpoint.prototype.getNewFacilityEndpoint = function (facilityObject) {
        var _this = this;
        return this.http.post(this.facilitiesUrl, JSON.stringify(facilityObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewFacilityEndpoint(facilityObject); });
        }));
    };
    CommonEndpoint.prototype.getUpdateFacilityEndpoint = function (facilityObject, facilityId) {
        var _this = this;
        var endpointUrl = this.facilitiesUrl + "/update/" + facilityId;
        return this.http.put(endpointUrl, JSON.stringify(facilityObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateFacilityEndpoint(facilityObject, facilityId); });
        }));
    };
    CommonEndpoint.prototype.getDeleteFacilityEndpoint = function (facilityId) {
        var _this = this;
        var endpointUrl = this.facilitiesUrl + "/delete/" + facilityId;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteFacilityEndpoint(facilityId); });
        }));
    };
    CommonEndpoint.prototype.getFacilityByFacilityNameEndpoint = function (facilityName) {
        var _this = this;
        var endpointUrl = this.facilityByFacilityNameUrl + "/" + facilityName;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getFacilityByFacilityNameEndpoint(facilityName); });
        }));
    };
    //Facility Type
    CommonEndpoint.prototype.getFacilityTypesEndpoint = function (page, pageSize, institutionId) {
        var _this = this;
        var endpointUrl = page && pageSize ? this.facilityTypesUrl + "/facilitytypes/list/" + page + "/" + pageSize : this.facilityTypesUrl + '/facilitytypes/list?institutionId=' + institutionId;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getFacilityTypesEndpoint(page, pageSize, institutionId); });
        }));
    };
    CommonEndpoint.prototype.getNewFacilityTypeEndpoint = function (facilityTypeObject) {
        var _this = this;
        return this.http.post(this.facilityTypesUrl, JSON.stringify(facilityTypeObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getNewFacilityTypeEndpoint(facilityTypeObject); });
        }));
    };
    CommonEndpoint.prototype.getUpdateFacilityTypeEndpoint = function (facilityTypeObject, facilityTypeId) {
        var _this = this;
        var endpointUrl = this.facilityTypesUrl + "/update/" + facilityTypeId;
        return this.http.put(endpointUrl, JSON.stringify(facilityTypeObject), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getUpdateFacilityTypeEndpoint(facilityTypeObject, facilityTypeId); });
        }));
    };
    CommonEndpoint.prototype.getDeleteFacilityTypeEndpoint = function (facilityTypeId) {
        var _this = this;
        var endpointUrl = this.facilityTypesUrl + "/delete/" + facilityTypeId;
        return this.http.delete(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeleteFacilityTypeEndpoint(facilityTypeId); });
        }));
    };
    CommonEndpoint.prototype.getFacilityTypeByFacilityTypeNameEndpoint = function (facilityTypeName) {
        var _this = this;
        var endpointUrl = this.facilityTypeByFacilityTypeNameUrl + "/" + facilityTypeName;
        return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getFacilityTypeByFacilityTypeNameEndpoint(facilityTypeName); });
        }));
    };
    //device
    CommonEndpoint.prototype.getDeviceApprovalEndpoint = function (url, obj) {
        var _this = this;
        //url = this.configurations.baseUrl + url;
        return this.http.post(url, JSON.stringify(obj), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getDeviceApprovalEndpoint(url, JSON.stringify(obj)); });
        }));
    };
    //Reservation methods
    CommonEndpoint.prototype.getReservations = function (url, filter) {
        var _this = this;
        return this.http.post(url, JSON.stringify(filter), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getReservations(url, filter); });
        }));
    };
    //Location methods
    CommonEndpoint.prototype.getLocationTree = function (url, filter) {
        var _this = this;
        return this.http.post(url, JSON.stringify(filter), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getLocationTree(url, filter); });
        }));
    };
    CommonEndpoint.prototype.postTemplateToDevice = function (url, template) {
        var _this = this;
        var header = new http_1.HttpHeaders({ 'Content-Type': 'multipart/form-data', });
        //header.append('Access-Control-Allow-Origin', 'http://localhost:56767');
        //header.append('Access-Control-Allow-Credentials', 'true');
        var params = new http_1.HttpParams();
        params.append('name', template.name);
        params.append('eImage', template.imgUrl);
        //const formData = new FormData();
        //formData.append(imgName, uri);
        return this.http.post(url, params, this.getRequestHeaders('multipart/form-data')).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.postTemplateToDevice(url, template); });
        }));
    };
    CommonEndpoint.prototype.sendQueueSimulator = function (url, data) {
        var _this = this;
        return this.http.post(url, JSON.stringify(data), this.getRequestHeaders()).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.getLocationTree(url, data); });
        }));
    };
    CommonEndpoint.prototype.importFile = function (url, data) {
        var _this = this;
        return this.http.post(url, data, { reportProgress: true, observe: 'events' }).pipe(operators_1.catchError(function (error) {
            return _this.handleError(error, function () { return _this.importFile(url, data); });
        }));
    };
    CommonEndpoint = __decorate([
        core_1.Injectable()
    ], CommonEndpoint);
    return CommonEndpoint;
}(endpoint_factory_service_1.EndpointFactory));
exports.CommonEndpoint = CommonEndpoint;
//# sourceMappingURL=common-endpoint.service.js.map