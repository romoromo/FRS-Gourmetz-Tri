"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.imageIndex = exports.Playlist = void 0;
var Playlist = /** @class */ (function () {
    function Playlist(id, name, description) {
        this.id = id;
        this.name = name;
        this.description = description;
    }
    return Playlist;
}());
exports.Playlist = Playlist;
var imageIndex = /** @class */ (function () {
    function imageIndex(imageId, imgIndex, duration, animation) {
        this.imageId = imageId;
        this.imgIndex = imgIndex;
        this.duration = duration;
        this.animation = animation;
    }
    return imageIndex;
}());
exports.imageIndex = imageIndex;
//# sourceMappingURL=playlist.model.js.map