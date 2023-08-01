"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.TranslateLanguageLoader = exports.AppTranslationService = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var AppTranslationService = /** @class */ (function () {
    function AppTranslationService(translate) {
        this.translate = translate;
        this.defaultLanguage = "en";
        this.onLanguageChanged = new rxjs_1.Subject();
        this.languageChanged$ = this.onLanguageChanged.asObservable();
        this.setDefaultLanguage(this.defaultLanguage);
    }
    AppTranslationService.prototype.addLanguages = function (lang) {
        this.translate.addLangs(lang);
    };
    AppTranslationService.prototype.setDefaultLanguage = function (lang) {
        this.translate.setDefaultLang(lang);
    };
    AppTranslationService.prototype.getDefaultLanguage = function () {
        return this.translate.defaultLang;
    };
    AppTranslationService.prototype.getBrowserLanguage = function () {
        return this.translate.getBrowserLang();
    };
    AppTranslationService.prototype.useBrowserLanguage = function () {
        var browserLang = this.getBrowserLanguage();
        if (browserLang.match(/en|fr|de|ar|ko|cn|pt/)) {
            this.changeLanguage(browserLang);
            return browserLang;
        }
    };
    AppTranslationService.prototype.changeLanguage = function (language) {
        var _this = this;
        if (language === void 0) { language = "en"; }
        if (!language)
            language = this.translate.defaultLang;
        if (language != this.translate.currentLang) {
            setTimeout(function () {
                _this.translate.use(language);
                _this.onLanguageChanged.next(language);
            });
        }
        return language;
    };
    AppTranslationService.prototype.getTranslation = function (key, interpolateParams) {
        return this.translate.instant(key, interpolateParams);
    };
    AppTranslationService.prototype.getTranslationAsync = function (key, interpolateParams) {
        return this.translate.get(key, interpolateParams);
    };
    AppTranslationService = __decorate([
        core_1.Injectable()
    ], AppTranslationService);
    return AppTranslationService;
}());
exports.AppTranslationService = AppTranslationService;
var TranslateLanguageLoader = /** @class */ (function () {
    function TranslateLanguageLoader() {
    }
    TranslateLanguageLoader.prototype.getTranslation = function (lang) {
        //Note Dynamic require(variable) will not work. Require is always at compile time
        switch (lang) {
            case "en":
                return rxjs_1.of(require("../assets/locale/en.json"));
            case "fr":
                return rxjs_1.of(require("../assets/locale/fr.json"));
            case "de":
                return rxjs_1.of(require("../assets/locale/de.json"));
            case "pt":
                return rxjs_1.of(require("../assets/locale/pt.json"));
            case "ar":
                return rxjs_1.of(require("../assets/locale/ar.json"));
            case "ko":
                return rxjs_1.of(require("../assets/locale/ko.json"));
            case "cn":
                return rxjs_1.of(require("../assets/locale/cn.json"));
            default:
        }
    };
    return TranslateLanguageLoader;
}());
exports.TranslateLanguageLoader = TranslateLanguageLoader;
//# sourceMappingURL=app-translation.service.js.map