"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.PlaylistService = void 0;
var core_1 = require("@angular/core");
//export type ContactGroupsChangedOperation = "add" | "delete" | "modify";
//export type ContactGroupsChangedEventArg = { contactgroups: ContactGroup[] | string[], operation: ContactGroupsChangedOperation };
var PlaylistService = /** @class */ (function () {
    function PlaylistService(router, http, authService, commonEndpoint, configurations) {
        this.router = router;
        this.http = http;
        this.authService = authService;
        this.commonEndpoint = commonEndpoint;
        this.configurations = configurations;
        //public static readonly contactgroupAddedOperation: ContactGroupsChangedOperation = "add";
        //public static readonly contactgroupDeletedOperation: ContactGroupsChangedOperation = "delete";
        //public static readonly contactgroupModifiedOperation: ContactGroupsChangedOperation = "modify";
        //private _contactgroupsChanged = new Subject<ContactGroupsChangedEventArg>();
        this._playlistUrl = "/api/playlist";
    }
    Object.defineProperty(PlaylistService.prototype, "playlistUrl", {
        get: function () { return this.configurations.baseUrl + this._playlistUrl; },
        enumerable: false,
        configurable: true
    });
    //private onContactGroupsChanged(contactgroups: ContactGroup[] | string[], op: ContactGroupsChangedOperation) {
    //  this._contactgroupsChanged.next({ contactgroups: contactgroups, operation: op });
    //}
    //onContactGroupsCountChanged(contactgroups: ContactGroup[] | string[]) {
    //  return this.onContactGroupsChanged(contactgroups, ContactGroupService.contactgroupModifiedOperation);
    //}
    //getContactGroupsChangedEvent(): Observable<ContactGroupsChangedEventArg> {
    //  return this._contactgroupsChanged.asObservable();
    //}
    PlaylistService.prototype.getPlaylistById = function (playlistId) {
        return this.commonEndpoint.getById(this.playlistUrl, playlistId);
    };
    PlaylistService.prototype.getPlaylists = function (page, pageSize, institutionId) {
        return this.commonEndpoint.getPagedList(this.playlistUrl + '/playlists/list?institutionId=' + institutionId, page, pageSize);
    };
    PlaylistService.prototype.updatePlaylist = function (playlist) {
        if (playlist.id) {
            return this.commonEndpoint.getUpdateEndpoint(this.playlistUrl, playlist, playlist.id);
        }
    };
    PlaylistService.prototype.newPlaylist = function (playlist) {
        return this.commonEndpoint.getNewEndpoint(this.playlistUrl, playlist);
    };
    PlaylistService.prototype.deletePlaylist = function (playlistOrPlaylistId) {
        if (typeof playlistOrPlaylistId === 'number' || playlistOrPlaylistId instanceof Number ||
            typeof playlistOrPlaylistId === 'string' || playlistOrPlaylistId instanceof String) {
            return this.commonEndpoint.getDeleteEndpoint(this.playlistUrl, playlistOrPlaylistId);
        }
        else {
            if (playlistOrPlaylistId.id) {
                return this.deletePlaylist(playlistOrPlaylistId.id);
            }
        }
    };
    PlaylistService = __decorate([
        core_1.Injectable()
    ], PlaylistService);
    return PlaylistService;
}());
exports.PlaylistService = PlaylistService;
//# sourceMappingURL=playlist.service.js.map