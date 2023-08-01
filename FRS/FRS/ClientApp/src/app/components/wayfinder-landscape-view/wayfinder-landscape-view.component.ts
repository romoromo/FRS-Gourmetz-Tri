import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit, ChangeDetectorRef, ViewEncapsulation } from '@angular/core';
import { fadeInOut } from '../../services/animations';

import "ol/ol.css";
import Text from "ol/Style/Text";
import ImageLayer from "ol/layer/Image";
import Map from "ol/Map";
import Projection from "ol/proj/Projection";
import Static from "ol/source/ImageStatic";
import Path from "ol-ext/featureAnimation/Path";
import View from "ol/View";
import RegularShape from 'ol/style/RegularShape';
import { getCenter } from "ol/extent";
import VectorSource from "ol/source/Vector";
import { Vector as VectorLayer } from "ol/layer";
import Feature from "ol/Feature";
import LineString from "ol/geom/LineString";
import { Circle, Style, Icon } from "ol/style";
import Stroke from "ol/style/Stroke";
import Fill from "ol/style/Fill";
import Point from "ol/geom/Point";

import { MapService } from 'src/app/services/map.service';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { BuildingService } from 'src/app/services/building.service';
import { DirectoryListingService } from 'src/app/services/directory-listing.service';
import { DirectoryListingCategoryService } from 'src/app/services/directory-listing-category.service';
import { Utilities } from 'src/app/services/utilities';

import { Subscription } from 'rxjs';

import { OwlOptions } from 'ngx-owl-carousel-o';

import { KioskSettingsService } from 'src/app/services/kiosk-settings.service';
import { KioskSettings } from 'src/app/models/kiosk-settings.model';
import { Playlist } from '../../models/playlist.model';
import { PlaylistService } from 'src/app/services/playlist.service';
import { FileService } from 'src/app/services/file.service';
import { DomSanitizer } from '@angular/platform-browser';

import { ActivatedRoute, Router } from "@angular/router";
import { PointModel } from '../../models/point.model';
import { KeyValue } from '@angular/common';

interface floorPair {
  [key: number]: string;
}


@Component({
  selector: "wayfinder-landscape-view",
  templateUrl: "./wayfinder-landscape-view.component.html",
  styleUrls: ["./wayfinder-landscape-view.component.css"],
  encapsulation: ViewEncapsulation.None
})

export class WayfinderLandscapeComponent implements OnInit, OnDestroy {
  floors: any;
  //currentFloor: any;
  server_url: any;
  directorys: any;
  from: any;
  to: any;
  paths: any;
  maps: any[];
  mapMap: {};
  currentMap: any;
  vectorSource: any;
  interval: any;
  mapIndex: number;
  searchList: any;
  selectedCat: any;
  selectedLvl: any;

  map: Map;
  mode = 0;
  searchModel: string;
  wheelchair = false;
  mapMode: boolean;
  userData: any;
  defaultFrom: { id: any; };
  loadingIndicator: boolean;
  private subscription: Subscription = new Subscription();

  setting: KioskSettings;
  topBanner: Playlist;
  bottomBanner: Playlist;
  screenSaver: Playlist;
  topSlides = [];
  bottomSlides = [];
  screenSaverSlides = [];
  eventSlideSel = [];
  selectedEvent: string;
  eventSlides = [];

  active = true;

  dir_cats: any;

  floorPairs: floorPair = {};

  homeIcon = require("../../assets/images/home1.jpg");
  wheelIcon = require("../../assets/images/wheelchair1.jpg");
  hereIcon = require("../../assets/images/pin.png");

  defMap: any;

  dirload = false;

  point_id: string;

  startPoint: any;

  customOptions: OwlOptions = {
    loop: true,
    mouseDrag: false,
    touchDrag: false,
    pullDrag: false,
    dots: false,
    navSpeed: 700,
    navText: ['', ''],
    nav: false,
    animateOut: 'fadeOut',
    autoplay: true,
    autoplayTimeout: 5000,
    items: 1
  }

  eventOptions: OwlOptions = {
    loop: true,
    mouseDrag: false,
    touchDrag: false,
    pullDrag: false,
    dots: false,
    navSpeed: 700,
    navText: ['', ''],
    nav: false,
    animateOut: 'fadeOut',
    //autoplay: true,
    //autoplayTimeout: 5000,
    center: true,
    margin: 10,
    items: 3.5
  }
  pause: any;
  vectorLayer: any;
    up: boolean;
    down: boolean;
    showRouteOption: boolean;
    sheltered: boolean;
    arrowImageWidth: number;
    arrowImageHeight: number;

  constructor(
    //private navCtrl: NavController,
    //private dataS: DataService,
    //private http: HTTP,
    private chRef: ChangeDetectorRef,
    //private storage: Storage,
    private mapService: MapService,
    private alertService: AlertService,
    private buildingService: BuildingService,
    private directoryListingService: DirectoryListingService,
    private directoryListingCategoryService: DirectoryListingCategoryService,
    private kioskSettingsService: KioskSettingsService,
    private playlistService: PlaylistService,
    private fileService: FileService,
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute) {

    this.route.params.subscribe(queryParams => {
      this.point_id = queryParams["point_id"];
    });

    if (!this.point_id) {
      this.route.queryParams.subscribe(queryParams => {
        this.point_id = queryParams["point_id"];
      });
    }

    if (this.point_id) this.getDefPoint(this.point_id);

  }

  async ngOnInit() {
    //alert("1");
    window.onerror = function (message, file, line, col, error) {
      //alert("Error occurred: " + error.message);
      return false;
    };
    window.addEventListener("error", function (e) {
      //alert("Error occurred: " + e.error.message);
      return false;
    });

    window.addEventListener("unhandledrejection", function (e) {
      //alert("Error occurred: " + e.reason.message);
    });

    this.loadData();

    await this.getDirectory();

    if (this.point_id) {
      var intpoint: number = parseInt(this.point_id)
      //await this.getDefPoint(intpoint);
    }

    await this.getFloors();

    await this.getDirectoryCategories();



  }


  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  async getDefPoint(point_id) {
    console.log("PP", point_id);

    this.subscription.add(this.mapService.getPointById(point_id)
      .subscribe(results => {
        //this.alertService.stopLoadingMessage();
        //this.loadingIndicator = false;

        //console.log(results)

        console.log("def point res: ", results)

        this.startPoint = results;

        this.subscription.add(this.mapService.getMapById(this.startPoint.mapId).subscribe(resultsMap => {
          this.currentMap = resultsMap;
          this.defMap = this.currentMap;
          console.log("maps: ", this.currentMap)
          this.initMap();
        }));
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));

  }

  async getFloors() {

    this.loadingIndicator = true;

    this.subscription.add(this.buildingService.getBuildings()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //console.log(results)

        let buildings = results[0];

        for (let b of buildings) {
          if (b.floors && b.floors.filter((f) => f.map).length > 0) {
            this.floors = b.floors.filter((f) => f.map);

            if (!this.currentMap) this.currentMap = this.floors[0].map;
            if (!this.defMap) this.defMap = this.currentMap;

            console.log("floors: ", this.floors)
            this.initMap();
            break;
          }
        }
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  async getDirectoryCategories() {

    this.loadingIndicator = true;

    this.subscription.add(this.directoryListingCategoryService.getDirectoryListingCategorys()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        console.log("categories: ", results)

        this.dir_cats = results[0];
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  async getDirectory() {

    this.loadingIndicator = true;

    this.subscription.add(this.directoryListingService.getDirectoryListingsExcInt()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        //console.log(results)

        this.directorys = results[0];

        for (let d of this.directorys) {
          if (d.floorId != null && !this.floorPairs[d.floorId]) {
            this.floorPairs[d.floorId] = d.floorLabel;
          }
        }

        console.log("list of used floor: ", this.floorPairs)

        this.directorys = this.directorys.sort((a, b) => {
          let r = a.directoryListingCategoryLabel.localeCompare(
            b.directoryListingCategoryLabel
          );

          if (r != 0) return r;

          return a.label.localeCompare(b.label);
        });

        this.searchList = this.directorys;

        console.log("first directory: ", this.searchList)

        this.chRef.detectChanges();

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  initMap() {
    console.log("init map, currentMap: ", this.currentMap)
    if (this.currentMap == null || this.currentMap.map_url == null) return;

    var extent = [0, 0, this.currentMap.width, this.currentMap.height];
    var projection = new Projection({
      code: "xkcd-image",
      units: "pixels",
      extent: extent,
    });

    console.log("after projection");

    let fullUrl = "/" + this.currentMap.map_url;

    console.log("url map image: ", fullUrl)

    let source = new Static({
      url: fullUrl,
      projection: projection,
      imageExtent: extent,
    });

    this.vectorSource = new VectorSource();

    if (this.map) {
      let l = this.map.getLayers().getArray()[0];
      l.setSource(source);

      let v = this.map.getLayers().getArray()[1];
      v.setSource(this.vectorSource);
    } else {
      this.vectorLayer = new VectorLayer({
        source: this.vectorSource,
      });

      this.map = new Map({
        layers: [
          new ImageLayer({
            source: source,
          }),
          this.vectorLayer,
        ],
        target: "map",
        view: new View({
          projection: projection,
          //center: getCenter(extent),
          zoom: this.setting.zoom_level || 2,
          maxZoom: 8,
        }),
      });

      console.log("end initmap")

      const waitForMap = setInterval(function () {
        if (document.querySelectorAll(".ol-viewport").length) {
          console.log("Exists!");
          window.dispatchEvent(new Event("resize"));
          clearInterval(waitForMap);
        }
      }, 100);
    }

    //this.map.on('postrender', (event) => {
      
    //});

    if (this.currentMap && this.currentMap.paths && this.currentMap.paths[0]) {
      this.createLine();

      //setTimeout(() => { if (!this.pause) this.next(this.mapIndex + 1) }, (this.setting.floor_interval || 6) * 1000);
    }

    if (this.startPoint.mapId == this.currentMap.id) {
      const tt = this;

      if (this.setting.arrow_image) {
        const img = new Image();
        img.onload = function () {
          tt.createHereIcon(img.width, img.height);
        }
        img.src = this.setting.baloon_image;
      }

      if (this.setting.arrow_image) {
        const imgArrow = new Image();
        imgArrow.onload = function () {
          tt.arrowImageWidth = imgArrow.width;
          tt.arrowImageHeight = imgArrow.height;
        }
        imgArrow.src = this.setting.arrow_image;
      }

      this.map.getView().setCenter(getCenter(extent));
    }
  }

  createHereIcon(width, height) {
    const iconFeature = new Feature({
      geometry: new Point([this.startPoint.x, this.startPoint.y]),
      name: 'You Are Here',
    });

    const iconStyle = new Style({
      image: new Icon({
        //anchor: [this.setting.anchor_h, this.setting.anchor_v],
        //anchorXUnits: 'pixels',
        //anchorYUnits: 'pixels',
        anchor: [0.5, 1],
        size: [width, height],
        src: this.setting.baloon_image,
        scale: this.setting.baloon_width / width,
      }),
    });



    console.log("create iconsss", width, height, this.setting.baloon_width / width)

    iconFeature.setStyle(iconStyle);

    this.vectorSource.addFeature(iconFeature);
  }

  animationEnd(e?) {
    if (this.maps.length > this.mapIndex + 1 && this.currentMap.floorOrder !== null && this.maps[this.mapIndex + 1].floorOrder !== null) {
      if (this.currentMap.floorOrder < this.maps[this.mapIndex + 1].floorOrder) this.up = true;
      if (this.currentMap.floorOrder > this.maps[this.mapIndex + 1].floorOrder) this.down = true;
    }

    setTimeout(() => {
      this.up = false;
      this.down = false;

      if (e && e.feature) this.vectorSource.removeFeature(e.feature);
      if (!this.pause) this.next(this.mapIndex + 1)
    }, this.setting.floor_interval || (this.up || this.down || !this.maps[this.mapIndex + 1] ? 3000 : 500));
  }

  createLine() {
    if (!this.currentMap || !this.currentMap.paths || !this.currentMap.paths[0]) return;



    var points = new Array();
    for (let p of this.currentMap.paths) {
      let coord = [p.x, p.y];
      points.push(coord);
    }

    var featureLine = new Feature({
      geometry: new LineString(points),
      dashOffset: 0,
    });

    var outlineStroke = new Style({
      stroke: new Stroke({
        color: this.setting.line_color || 'blue', //[25, 25, 255, 1],
        width: 9,
      }),
    });

    // use style caching in production
    function getAnimationStrokeStyle() {
      return new Style({
        stroke: new Stroke({
          color: 'red', //[204, 204, 255, 1],
          width: 5,
          //lineDash: [2, 7],
          //lineDashOffset: featureLine.get("dashOffset"),
        }),
      });
    }

    function getStyle() {
      return [outlineStroke, getAnimationStrokeStyle()];
    }

    featureLine.setStyle(outlineStroke);

    this.vectorSource.addFeature(featureLine);

    //this.interval = setInterval(function () {
    //  let offset = featureLine.get("dashOffset");
    //  offset = offset == 0 ? 8 : offset - 1;
    //  featureLine.set("dashOffset", offset);
    //}, 100);
    var anim = new Path({
      path: featureLine.getGeometry(),
      rotate: true,
      //easing: ol.easing[$("#easing").val()],
      speed: this.setting.arrow_speed || 0.5,
      //revers: $("#revers").prop('checked')
    });

    var triangle = new RegularShape({
      radius: 14,
      points: 3,
      fill: new Fill({ color: this.setting.arrow_color || 'red' }),
      stroke: new Stroke({ color: '#fff', width: 2 })
    });

    let movingIcon = this.setting.arrow_image ? new Icon({
      //anchor: [this.setting.anchor_h, this.setting.anchor_v],
      //anchorXUnits: 'pixels',
      //anchorYUnits: 'pixels',
      //anchor: [0.5, 1],
      size: [this.arrowImageWidth, this.arrowImageHeight],
      src: this.setting.arrow_image,
      scale: 28 / this.arrowImageWidth,
    }) : null;

    var styleTriangle =
      new Style({
        image: movingIcon || triangle,
        stroke: new Stroke({
          color: [0, 0, 255],
          width: 2
        }),
        fill: new Fill({
          color: [0, 0, 255, 0.3]
        })
      });



    let start = featureLine.getGeometry().getFirstCoordinate();
    let end = featureLine.getGeometry().getLastCoordinate();

    let center = [Math.floor((start[0] + end[0]) / 2), Math.floor((start[1] + end[1]) / 2)];

    this.map.getView().setCenter(center);

    //console.log("center", start, center, end)
    //this.createPoint(center, "#40E000");

    let begin = new Feature(new Point(start));
    begin.setStyle(styleTriangle);

    this.vectorSource.addFeature(begin);
    anim.on('animationend', (e) => {
      this.animationEnd(e);
    });

    anim.on('animating', (e) => {
      //this.map.getView().setCenter(e.geom.getCoordinates())
      //this.map.getView().setRotation(e.rotation||0)
    })
    if (points.length > 1) this.vectorLayer.animateFeature(begin, anim);
    else this.animationEnd();

    //console.log("VV", vv);

    console.log("SESE", start, end);

    //if (this.mapIndex == 0) this.createPoint(start, "#40E0D0");
    //else this.createPoint(start, "#FFBF00");

    //if (this.maps[this.mapIndex + 1]) this.createPoint(end, "#FFBF00");
    //else this.createPoint(end, "#DE3163");

    for (let i in this.currentMap.paths) {
      let p = this.currentMap.paths[i];
      let coord = [p.x, p.y];

      if (+i == 0) {
        if (this.mapIndex == 0) this.createPoint(coord, "#40E0D0", p.label);
        else this.createPoint(coord, "#FFBF00", p.label);
      } else if (+i == this.currentMap.paths.length - 1) {
        if (this.maps[this.mapIndex + 1]) this.createPoint(coord, "#FFBF00", p.label);
        else this.createPoint(coord, "#DE3163", p.label);
      } else if (p.label) {
        this.createPoint(coord, null, p.label);
      }
    }
  }

  truncateString(str, num) {
    if (str.length > num) {
      return str.slice(0, num) + "...";
    } else {
      return str;
    }
  }

  createPoint(coordinate, strokeColor, label?: string) {
    let fill = new Fill({
      color: "white",
    });
    let stroke = new Stroke({
      color: strokeColor,
      width: 4,
    });
    let pointStyle = strokeColor ? new Style({
      image: new Circle({
        fill: fill,
        stroke: stroke,
        radius: 7,
      }),
      fill: fill,
      stroke: stroke,
    }) : new Style();
    let point = new Feature({
      geometry: new Point(coordinate),
      isPoint: true,
    });
    if (label) {
      pointStyle.setText(new Text({
        text: this.truncateString(label, 12),
        font: 'bold 16px Arial',
        overflow: true,
        outline: '#ffffff',
        outlineWidth: 3,
        stroke: new Stroke({
          color: '#ffffff',
          width: 3,
        }),
        fill: new Fill({
          color: "red",
        })
      }));
    }
    point.setStyle(pointStyle);
    this.vectorSource.addFeature(point);
  }

  async loadData() {
    this.subscription.add(this.kioskSettingsService.getKioskSettingsById("4")
      .subscribe(results => {

        let kioskSetting = results;

        console.log("Setting is: ", kioskSetting)
        this.setting = kioskSetting;

        this.getPlaylist(this.setting.top_banner_id, 1);
        this.getPlaylist(this.setting.bottom_banner_id, 2);
        this.getPlaylist(this.setting.def_event_id, 3);
        this.getPlaylist(this.setting.ss_playlist_id, 4);

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve kiosk settings from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  getPlaylist(id, pos) {
    this.subscription.add(this.playlistService.getPlaylistById(id)
      .subscribe(results => {

        let playlist = results;

        let images = playlist.images;

        if (pos == 1) {
          this.topBanner = playlist;
          for (let i = 0; i < images.length; i++) {
            let slide = { id: i, img: images[i].imageLocation }
            this.topSlides.push(slide);
          }

        } else if (pos == 2) {
          this.bottomBanner = playlist;
          for (let i = 0; i < images.length; i++) {
            let slide = { id: i, img: images[i].imageLocation }
            this.bottomSlides.push(slide);
          }
        } else if (pos == 3) {
          for (let i = 0; i < images.length; i++) {
            let slide = { id: i, img: images[i].imageLocation }
            this.eventSlideSel.push(slide);
          }
        } else if (pos == 4) {
          this.screenSaver = playlist;
          for (let i = 0; i < images.length; i++) {
            let slide = { id: i, img: images[i].imageLocation }
            this.screenSaverSlides.push(slide);
          }
        }

        if (this.eventSlideSel.length > 0 && this.eventSlides.length == 0) {
          let index = this.getRandomInt(this.eventSlideSel.length)
          this.selectedEvent = this.eventSlideSel[index].img
          for (let i = 0; i < 8; i++) {
            let slide = { id: i, img: this.selectedEvent }
            this.eventSlides.push(slide);
          }
        }
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve kiosk playlist from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  getFileImage(path) {
    let pathTxt = this.fileService.getFile(path);
    //console.log("image path: ", pathTxt);
    return pathTxt;
    //return "Resources/Images/Images/97d672b0-cc3d-4a6c-aefa-c94af06a375bbanner.jpg";
  }

  getSafeFile(path) {
    //console.log("image Path before safe: ", path)
    let newPath = this.sanitizer.bypassSecurityTrustStyle('url(&quot;' + decodeURI(encodeURI(path)) + '&quot;)')
    //console.log("Image Path after safe: ", newPath)
    return newPath;
  }

  getRandomInt(max) {
    return Math.floor(Math.random() * max);
  }

  selectSearch(searchModel) {
    this.searchModel = searchModel;
    this.updateSearchResults();
  }

  updateSearchResults() {
    console.log("searchModel: ", this.searchModel);

    console.log("directorys inside search: ", this.directorys)

    if (!this.searchModel && !this.selectedCat && !this.selectedLvl) {
      this.searchList = this.directorys;
      return;
    }

    this.searchList = this.directorys;

    if (this.selectedCat) {
      this.searchList = this.searchList
        ? this.searchList.filter(
          (item) => (item.directoryListingCategoryId == this.selectedCat.id))
        : [];
    }

    console.log("searchList after category filter: ", this.searchList);

    if (this.selectedLvl) {
      this.searchList = this.searchList
        ? this.searchList.filter(
          (item) => (item.floorId == this.selectedLvl))
        : [];
    }

    console.log("searchList after level filter: ", this.searchList);

    if (this.searchModel) {
      this.searchList = this.searchList
        ? this.searchList.filter(
          (item) => (item.label.toLowerCase()).indexOf(this.searchModel.toLowerCase()) !== -1)
        : [];
    }

    console.log("searchList after text search: ", this.searchList);

    console.log("searchList: ", this.searchList);
  }

  directoryClicked(d) {
    this.searchModel = "";
    this.selectedCat = null;
    this.selectedLvl = null;

    if (!this.to && (this.from || this.point_id)) {
      console.log("TOTOTO")
      this.to = d;
      this.directoryChanged(this.from, this.to);
      return;
    }
    if (!this.from && !this.point_id) {
      this.from = d;
      return;
    }
  }


  clearDirectory() {
    if (!this.defaultFrom) this.from = null;
    this.to = null;
    this.wheelchair = false;
  }

  async directoryChanged(from, to) {
    console.log("starting point id: ", this.point_id)
    if ((!this.point_id && !from) || !to) return;

    //if (!this.defaultFrom) this.from = null;
    //this.to = null;
    //this.wheelchair = false;

    if (this.point_id) {
      this.defaultFrom = { id: this.point_id };
      from = this.defaultFrom;
    }

    this.dirload = true;

    this.paths = null;
    clearInterval(this.interval);

    console.log("dir changed from to: ", from, to)

    this.from = from;
    this.to = to;
    await this.getRoute(from.id, to.id);

    console.log("path inside dir change: ", this.paths);
  }


  doMapUpdate() {
    if (!this.paths) return;

    this.mapMode = true;

    this.mapMap = {};
    this.maps = [];

    console.log("map update paths: ", this.paths)

    let i = 1;
    for (let p of this.paths) {
      if (!this.mapMap[p.mapId]) {
        this.mapMap[p.mapId] = p.mapInfo;
        this.maps.push(this.mapMap[p.mapId]);
      }

      if (!this.mapMap[p.mapId].paths) this.mapMap[p.mapId].paths = [];

      this.mapMap[p.mapId].paths.push(p);

      if (p.label) p.label = i++ + '. ' + p.label; 
    }

    console.log("maps: ", this.maps)
    console.log("mapmap: ", this.mapMap)

    if (this.maps[0]) {
      this.currentMap = this.maps[0];
      this.mapIndex = 0;
    }

    console.log("current map inside map update: ", this.currentMap)

    this.pause = false;

    this.initMap();

    this.mode = 0;
  }

  async getRoute(from, to) {

    if (!this.showRouteOption) {
      this.showRouteOption = true;
      return;
    }

    this.showRouteOption = false;

    this.loadingIndicator = true;

    this.subscription.add(this.mapService.getRoute(from, to, this.wheelchair, this.sheltered)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.wheelchair = false;
        this.sheltered = false;
        this.to = null;

        var res: any = results

        console.log("route: ", res)

        if (res.message) {
          alert(res.message);
          return;
        }

        let path = res.data
        this.paths = path;

        console.log("path inside get route: ", this.paths)

        this.doMapUpdate();

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        }));
  }

  next(index) {
    if (index >= this.maps.length) index = 0;

    this.currentMap = this.maps[index];
    this.mapIndex = index;

    this.initMap();
  }

  selectCat(cat) {
    console.log("cat selected: ", cat)
    this.selectedCat = cat;
    this.updateSearchResults();
  }

  selectLevel(l) {
    console.log("level selected: ", l);
    this.selectedLvl = l;
    this.updateSearchResults();
  }

  resetSearch() {
    this.clearDirectory();
    this.selectedCat = null;
    this.selectedLvl = null;
    this.searchModel = null;
    this.updateSearchResults();
    this.from = null;
    this.to = null;
    this.dirload = false;
    this.currentMap = this.defMap;
    this.paths = null;
    this.pause = true;
    this.initMap();
  }

  changeWheel() {
    this.wheelchair = !this.wheelchair;
  }

  valueAscOrder = (a: KeyValue<number, string>, b: KeyValue<number, string>): number => {
    return this.compareFloor(a.value, b.value);
  }

  compareFloor = function (a, b) {
    if (a.toLowerCase().includes("l") && b.toLowerCase().includes("b"))
      return 1;
    else if (a.toLowerCase().includes("b") && b.toLowerCase().includes("l"))
      return -1;
    else {
      if (a.toLowerCase().includes("l") && b.toLowerCase().includes("l")) {
        if (a > b) { return 1 } else if (a < b) { return -1 }
      } else if (a.toLowerCase().includes("b") && b.toLowerCase().includes("b")) {
        if (a > b) { return -1 } else if (a < b) { return 1 }
      } else {
        if (a > b) { return 1 } else if (a < b) { return -1 }
      }
    }
    return 0;
  }

}

////  constructor(
////    //private navCtrl: NavController,
////    //private dataS: DataService,
////    //private http: HTTP,
////    private chRef: ChangeDetectorRef,
////    private storage: Storage,
////    private mapService: MapService,
////    private alertService: AlertService,
////    private buildingService: BuildingService,
////    private directoryListingService: DirectoryListingService
////  ) {
////    //this.storage.get("ProfileStorage").then((ProfileStorage) => {
////    //  this.getUserDataById(ProfileStorage?.userId);
////    //});
////  }

////  async ngOnInit() {
////    //alert("1");
////    window.onerror = function (message, file, line, col, error) {
////      alert("Error occurred: " + error.message);
////      return false;
////    };
////    window.addEventListener("error", function (e) {
////      alert("Error occurred: " + e.error.message);
////      return false;
////    });

////    window.addEventListener("unhandledrejection", function (e) {
////      alert("Error occurred: " + e.reason.message);
////    });

////    await this.getDirectory();

////    await this.getFloors();

////    this.initMap();
////  }

////  async directoryChanged(from, to) {
////    if (!from || !to) return;

////    if (!this.defaultFrom) this.from = null;
////    this.to = null;
////    this.wheelchair = false;

////    this.paths = null;
////    clearInterval(this.interval);

////    await this.getRoute(from, to);

////    if (!this.paths) return;

////    this.mapMode = true;

////    this.mapMap = {};
////    this.maps = [];

////    for (let p of this.paths) {
////      if (!this.mapMap[p.mapId]) {
////        this.mapMap[p.mapId] = p.mapInfo;
////        this.maps.push(this.mapMap[p.mapId]);
////      }

////      if (!this.mapMap[p.mapId].paths) this.mapMap[p.mapId].paths = [];

////      this.mapMap[p.mapId].paths.push(p);
////    }

////    if (this.maps[0]) {
////      this.currentMap = this.maps[0];
////      this.mapIndex = 0;
////    }

////    this.initMap();

////    this.mode = 0;
////  }

////  async getRoute(from, to) {

////    this.loadingIndicator = true;

////    this.subscription.add(this.mapService.getRoute(from, to, this.wheelchair)
////      .subscribe(results => {
////        this.alertService.stopLoadingMessage();
////        this.loadingIndicator = false;

////        var res: any = results

////        console.log(res)

////        if (res.message) {
////          alert(res.message);
////          return;
////        }

////        let path = res.data

////      },
////        error => {
////          this.alertService.stopLoadingMessage();
////          this.loadingIndicator = false;

////          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
////            MessageSeverity.error);
////        }));
////  }

////  async getFloors() {

////    this.loadingIndicator = true;

////    this.subscription.add(this.buildingService.getBuildings()
////      .subscribe(results => {
////        this.alertService.stopLoadingMessage();
////        this.loadingIndicator = false;

////        console.log(results)

////        let buildings = results[0];

////        for (let b of buildings) {
////          if (b.floors && b.floors.filter((f) => f.map).length > 0) {
////            this.floors = b.floors.filter((f) => f.map);
////            this.currentMap = this.floors[0].map;
////            break;
////          }
////        }
////      },
////        error => {
////          this.alertService.stopLoadingMessage();
////          this.loadingIndicator = false;

////          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
////            MessageSeverity.error);
////        }));
////  }

////  async getDirectory() {

////    this.loadingIndicator = true;

////    this.subscription.add(this.directoryListingService.getDirectoryListingsExcInt()
////      .subscribe(results => {
////        this.alertService.stopLoadingMessage();
////        this.loadingIndicator = false;

////        console.log(results)

////        this.directorys = results[0];

////        this.directorys = this.directorys.sort((a, b) => {
////          let r = a.directoryListingCategoryLabel.localeCompare(
////            b.directoryListingCategoryLabel
////          );

////          if (r != 0) return r;

////          return a.label.localeCompare(b.label);
////        });

////        this.searchList = this.directorys;

////        this.chRef.detectChanges();

////      },
////        error => {
////          this.alertService.stopLoadingMessage();
////          this.loadingIndicator = false;

////          this.alertService.showStickyMessage("Load Error", `Unable to retrieve data from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
////            MessageSeverity.error);
////        }));
////  }

////  //async getUserDataById(id) {
////  //  const options = {
////  //    method: "get" as "get",
////  //  };

////  //  await this.http
////  //    .sendRequest(
////  //      this.dataS.getServerPath + "/api/Account/users/get/" + id,
////  //      options
////  //    )
////  //    .then((data) => {
////  //      let odata = JSON.parse(data.data);
////  //      this.userData = odata;

////  //      if (this.userData.directoryListingId) {
////  //        this.defaultFrom = {
////  //          id: this.userData.directoryListingId,
////  //          code: this.userData.directoryListingCode,
////  //          label: this.userData.directoryListingLabel,
////  //        };

////  //        this.from = this.defaultFrom;
////  //      }
////  //    })
////  //    .catch((error) => {
////  //      console.log(error.error);
////  //      this.dataS.dismiss();
////  //    });
////  //}

////  initMap() {
////    if (!this.currentMap?.map_url) return;

////    var extent = [0, 0, this.currentMap.width, this.currentMap.height];
////    var projection = new Projection({
////      code: "xkcd-image",
////      units: "pixels",
////      extent: extent,
////    });

////    let fullUrl = this.server_url + "/" + this.currentMap.map_url;

////    let source = new Static({
////      url: fullUrl,
////      projection: projection,
////      imageExtent: extent,
////    });

////    this.vectorSource = new VectorSource();

////    if (this.map) {
////      let l = this.map.getLayers().getArray()[0];
////      l.setSource(source);

////      let v = this.map.getLayers().getArray()[1];
////      v.setSource(this.vectorSource);
////    } else {
////      this.map = new Map({
////        layers: [
////          new ImageLayer({
////            source: source,
////          }),
////          new VectorLayer({
////            source: this.vectorSource,
////          }),
////        ],
////        target: "map",
////        view: new View({
////          projection: projection,
////          center: getCenter(extent),
////          zoom: 2,
////          maxZoom: 8,
////        }),
////      });

////      const waitForMap = setInterval(function () {
////        if (document.querySelectorAll(".ol-viewport").length) {
////          console.log("Exists!");
////          window.dispatchEvent(new Event("resize"));
////          clearInterval(waitForMap);
////        }
////      }, 100);
////    }

////    if (this.currentMap?.paths?.[0]) {
////      this.createLine();
////    }

////    // var view = this.map.getView();
////    // view.fit(extent, { constrainResolution: true });
////    // view.setMinZoom(view.getZoom());
////    // // set opening zoom level (zoom set by code must not be less than the minZoom)
////    // view.setZoom(Math.max(2, view.getMinZoom()));
////    //var center = getCenter(extent);
////    //view.on(['change:center', 'change:resolution'], function () {
////    //    // map.on('moveend', function() {        // alternative
////    //    var viewCenter = view.getCenter();
////    //    if ((viewCenter[0] != center[0] || viewCenter[1] != center[1]) && view.getZoom() == view.getMinZoom()) {
////    //        view.setCenter(center);
////    //    }
////    //});
////  }

////  createLine() {
////    if (!this.currentMap?.paths?.[0]) return;

////    var points = new Array();
////    for (let p of this.currentMap?.paths) {
////      points.push([p.x, p.y]);
////    }

////    var featureLine = new Feature({
////      geometry: new LineString(points),
////      dashOffset: 0,
////    });

////    var outlineStroke = new Style({
////      stroke: new Stroke({
////        color: [25, 25, 255, 1],
////        width: 5,
////      }),
////    });

////    // use style caching in production
////    function getAnimationStrokeStyle() {
////      return new Style({
////        stroke: new Stroke({
////          color: [204, 204, 255, 1],
////          width: 3,
////          lineDash: [2, 7],
////          lineDashOffset: featureLine.get("dashOffset"),
////        }),
////      });
////    }

////    function getStyle() {
////      return [outlineStroke, getAnimationStrokeStyle()];
////    }

////    featureLine.setStyle(getStyle);

////    this.vectorSource.addFeature(featureLine);

////    this.interval = setInterval(function () {
////      let offset = featureLine.get("dashOffset");
////      offset = offset == 0 ? 8 : offset - 1;
////      featureLine.set("dashOffset", offset);
////    }, 100);

////    let start = featureLine.getGeometry().getFirstCoordinate();
////    let end = featureLine.getGeometry().getLastCoordinate();

////    this.map.getView().setCenter(start);

////    if (this.mapIndex == 0) this.createPoint(start, "#40E0D0");
////    else this.createPoint(start, "#FFBF00");

////    if (this.maps[this.mapIndex + 1]) this.createPoint(end, "#FFBF00");
////    else this.createPoint(end, "#DE3163");
////  }

////  createPoint(coordinate, strokeColor) {
////    let fill = new Fill({
////      color: "white",
////    });
////    let stroke = new Stroke({
////      color: strokeColor,
////      width: 4,
////    });
////    let pointStyle = new Style({
////      image: new Circle({
////        fill: fill,
////        stroke: stroke,
////        radius: 7,
////      }),
////      fill: fill,
////      stroke: stroke,
////    });
////    let point = new Feature({
////      geometry: new Point(coordinate),
////      isPoint: true,
////    });
////    point.setStyle(pointStyle);
////    this.vectorSource.addFeature(point);
////  }

////  next(index) {
////    this.currentMap = this.maps[index];
////    this.mapIndex = index;
////    this.initMap();
////  }

////  goBack() {
////    //this.navCtrl.back();
////  }

////  updateSearchResults(searchModel) {
////    if (!searchModel) this.searchList = this.directorys;

////    this.searchList = this.directorys
////      ? this.directorys.filter(
////        (item) => item.label?.search(new RegExp(searchModel, "i")) > -1
////      )
////      : [];
////  }

////  directoryClicked(d) {
////    this.searchModel = "";

////    if (!this.to) {
////      this.to = d;
////      return;
////    }
////    if (!this.from) {
////      this.from = d;
////      return;
////    }
////  }

////  clearDirectory() {
////    if (!this.defaultFrom) this.from = null;
////    this.to = null;
////    this.wheelchair = false;
////  }
////}
