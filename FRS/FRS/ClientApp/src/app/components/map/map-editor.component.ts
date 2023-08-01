import { Component, ViewChild, Inject, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { AccountService } from 'src/app/services/account.service';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { MapModel } from 'src/app/models/map.model';
import { InstitutionService } from 'src/app/services/institution.service';
import { Utilities } from 'src/app/services/utilities';
import { Institution } from 'src/app/models/institution.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MapService } from 'src/app/services/map.service';
import { Subscription } from 'rxjs';
import { FloorService } from '../../services/floor.service';
import { Floor } from '../../models/floor.model';

import "ol/ol.css";
import ImageLayer from "ol/layer/Image";
import Map from "ol/Map";
import Projection from "ol/proj/Projection";
import Static from "ol/source/ImageStatic";
import View from "ol/View";
import { getCenter } from "ol/extent";

import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import VectorSource from 'ol/source/Vector';
import { Vector as VectorLayer } from 'ol/layer';
import { FullScreen, defaults as defaultControls } from 'ol/control';

import Modify from 'ol/interaction/Modify';
import Collection from 'ol/Collection';
import LineString from 'ol/geom/LineString';
import Select from 'ol/interaction/Select';
import { click } from 'ol/events/condition';
import { Circle, Style } from 'ol/style';
import Stroke from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';
import { PointModel } from '../../models/point.model';
import { LineModel } from '../../models/line.model';
import { DirectoryListingService } from '../../services/directory-listing.service';
import { DirectoryListing } from '../../models/directory-listing.model';
import Text from 'ol/style/Text';



@Component({
  selector: 'map-editor',
  templateUrl: './map-editor.component.html',
  styleUrls: ['./map-editor.component.css'],
  animations: [fadeInOut]
})
export class MapEditorComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  public formResetToggle = true;
  private loadingIndicator = false;
  private isNewMap = false;
  private originalMap: MapModel = new MapModel();
  private mapEdit: MapModel = new MapModel();
  private allInstitutions: Institution[] = [];
  private validation = { code: false, label: false };
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @ViewChild('f')
  private form;

  @ViewChild('code')
  private code;

  @ViewChild('label')
  private label;
  allFloor: Floor[];

  map: Map;
  source: any;
  vectorSource: VectorSource;
  collection: Collection;

  pointMap = {};
  lineMap = {};
  pointFeatureMap = {};
  lineFeatureMap = {};

  fill = new Fill({
    color: 'white'
  });
  stroke = new Stroke({
    color: '#40E0D0',
    width: 4
  });
  pointStyle = new Style({
    image: new Circle({
      fill: this.fill,
      stroke: this.stroke,
      radius: 7
    }),
    fill: this.fill,
    stroke: this.stroke
  })

  selectedStroke = new Stroke({
    color: '#FF7F50',
    width: 4
  });

  selectedPointStyle = new Style({
    image: new Circle({
      fill: this.fill,
      stroke: this.selectedStroke,
      radius: 7
    }),
    fill: this.fill,
    stroke: this.selectedStroke
  })

  selectedPoint: Feature;
  selectedPointObj: PointModel;

    allDirectory: DirectoryListing[];
    showEditInfo: boolean;
    allConnector: PointModel[];

  constructor(private alertService: AlertService, private accountService: AccountService, private institutionService: InstitutionService,
    private mapService: MapService,
    private floorService: FloorService,
    private directoryListingService: DirectoryListingService,
    //public dialogRef: MatDialogRef<MapEditorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.map) != typeof (undefined)) {
      if (data.map.id) {
        this.editMap(data.map);
      } else {
        this.newMap();
      }
    }

    this.floorService.getFloorByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {

        this.allFloor = results[0];
      },
        error => {

        });

    this.directoryListingService.getDirectoryListingByInstitutionId(this.accountService.currentUser.institutionId)
      .subscribe(results => {
        
        this.allDirectory = results[0];

      },
        error => {
        
        });

    this.mapService.getFloorConnector(this.mapEdit.id)
      .subscribe(results => {

        this.allConnector = results;
      },
        error => {

     });
  }

  add(selected: DirectoryListing, point: PointModel) {
    if (!selected) return;
    if (!point.directorys) point.directorys = [];

    let a = point.directorys.filter(obj => obj.directoryListingId === selected.id && obj.isActive);
    if (a.length > 0) return;

    a = point.directorys.filter(obj => obj.directoryListingId === selected.id);

    let f = a[0] || new DirectoryListing();
    Object.assign(f, selected);
    f.isActive = true;

    f.directoryListingId = f.id;
    f.pointId = point.id;
    f.id = "0";
    if (!a[0]) point.directorys.push(f);

    console.log("DIRECTORYS", point.directorys);
  }

  addConnector(point: PointModel, selectedConnector: PointModel) {
    if (!selectedConnector) return;
    if (!point.connectors) point.connectors = [];

    let a = point.connectors.filter(obj => obj.point1Id === selectedConnector.id && obj.isActive);
    if (a.length > 0) return;

    a = point.connectors.filter(obj => obj.point1Id === selectedConnector.id);

    let line = a[0] || new LineModel();
    line.code = this.createId();
    line.code0 = point.code;
    line.point0Id = point.id;
    line.code1 = selectedConnector.code;
    line.point1Id = selectedConnector.id;
    line.isActive = true;
    line.isFloorConnector = true;
    line.displayLabel = selectedConnector.label;

    if (!a[0]) point.connectors.push(line);
  }

  unselectPoint() {
    this.selectedPoint.setStyle(this.pointStyle);
    this.selectedPoint = null;
    this.selectedPointObj = null;
  }

  initMap() {
    if (!this.mapEdit.map_url) return;

    const img = new Image();
    img.onload = () => {
      var extent = [0, 0, img.width, img.height];
      var projection = new Projection({
        code: "xkcd-image",
        units: "pixels",
        extent: extent,
      });


      this.source = new Static({
        url: this.mapEdit.map_url,
        projection: projection,
        imageExtent: extent,
      });

      if (this.map) {
        let l = this.map.getLayers().getArray()[0];
        l.setSource(this.source);
      } else {
        this.vectorSource = new VectorSource();
        this.collection = new Collection();

        this.map = new Map({
          controls: defaultControls().extend([new FullScreen()]),
          layers: [
            new ImageLayer({
              source: this.source,
            }),
            new VectorLayer({
              source: this.vectorSource,
            })
          ],
          target: "mapdiv",
          view: new View({
            projection: projection,
            center: getCenter(extent),
            zoom: 2,
            maxZoom: 8,
          }),
        });

        var modify = new Modify({
          features: this.collection,
        });
        this.map.addInteraction(modify);

        this.map.on('click', (evt) => {
          var coordinate = evt.coordinate;

          let isFeatureClicked;
          this.map.forEachFeatureAtPixel(evt.pixel, (feature, layer) => {
            if (feature) isFeatureClicked = true;

            if (feature.getProperties().isLine) {
              this.deleteLine(feature);
            }
          })
          if (isFeatureClicked) return;

          if (this.selectedPoint) {
            this.unselectPoint()
          }

        });

        this.map.getViewport().addEventListener('contextmenu', (evt) => {
          evt.preventDefault();

          let coordinate = this.map.getEventCoordinate(evt);

          let isSelectPoint;
          let isFeatureClicked;
          this.map.forEachFeatureAtPixel(this.map.getPixelFromCoordinate(coordinate), (feature, layer) => {
            if (feature) isFeatureClicked = true;

            if (feature.getProperties().isPoint) {
              if (!this.selectedPoint) {
                this.selectedPoint = feature;
                this.selectedPoint.setStyle(this.selectedPointStyle);
                isSelectPoint = true;

                this.selectedPointObj = this.pointMap[this.selectedPoint.getId()];
              } else {
                if (feature.getId() != this.selectedPoint.getId()) {
                  this.createLine(this.selectedPoint, feature);
                }
              }
            }
          })

          if (this.selectedPoint && !isSelectPoint) {
            this.unselectPoint();
          }

          if (isFeatureClicked) return;

          this.createPoint(coordinate);
        });

        this.initPointsLines();
      }
    }
    img.src = this.mapEdit.map_url;
  }

  createId() {
    return Math.random().toString(36).substring(2) + Math.random().toString(36).substring(2);
  }

  createPoint(coordinate, code?: string) {
    let point = new Feature({
      geometry: new Point(coordinate),
      isPoint: true,
    });
    point.setId(code || this.createId());
    point.setStyle(this.pointStyle);
    this.vectorSource.addFeature(point);
    this.collection.push(point);

    point.on('change', () => {
      let pointObj = point.getProperties();

      for (let li of pointObj.line0 || []) {
        let line = li.getGeometry();
        let coords = line.getCoordinates();
        coords[0] = point.getGeometry().getFirstCoordinate();
        line.setCoordinates(coords);
      }

      for (let li of pointObj.line1 || []) {
        let line = li.getGeometry();
        let coords = line.getCoordinates();
        coords[1] = point.getGeometry().getFirstCoordinate();
        line.setCoordinates(coords);
      }
    }, point);

    this.pointFeatureMap[point.getId()] = point;

    if (!code) {
      let pointObj = new PointModel();
      pointObj.code = point.getId();
      pointObj.isActive = true;
      this.pointMap[point.getId()] = pointObj;
    }
  }

  createLine(feature0: Feature, feature1: Feature, code?: string, text?: string) {
    if (!feature0 || !feature1) return;

    let exist;
    for (let f of this.vectorSource.getFeatures()) {
      let prop = f.getProperties();
      if (prop.isLine) {
        if ((prop.code0 == feature0.getId() && prop.code1 == feature1.getId())
          || (prop.code0 == feature1.getId() && prop.code1 == feature0.getId())) exist = true;
      }
    }
    if (exist) return;

    let coord1 = feature0.getGeometry().getFirstCoordinate();
    let coord2 = feature1.getGeometry().getFirstCoordinate();

    var featureLine = new Feature({
      geometry: new LineString([coord1, coord2]),
      isLine: true,
      code0: feature0.getId(),
      code1: feature1.getId(),
    });
    featureLine.setId(code || this.createId());

    let lineStyle = new Style({
      stroke: new Stroke({
        color: '#6495ED',
        width: 3
      }),
      //text: new Text({
      //  text: "ID " + text,
        //font: '18px "Roboto", Helvetica Neue, Helvetica, Arial, sans-serif',
        //fill: new Fill({ color: 'black' }),
        //stroke: new Stroke({ color: 'black', width: 2 })
      //})
    })

    featureLine.setStyle(lineStyle);
    this.vectorSource.addFeature(featureLine);

    let selectedObj = feature0.getProperties();
    if (!selectedObj.line0) selectedObj.line0 = [];
    selectedObj.line0.push(featureLine);
    feature0.setProperties({ line0: selectedObj.line0 }, true);

    let featureObj = feature1.getProperties();
    if (!featureObj.line1) featureObj.line1 = [];
    featureObj.line1.push(featureLine);
    feature1.setProperties({ line1: featureObj.line1 }, true);

    this.lineFeatureMap[featureLine.getId()] = featureLine;
  }

  initPointsLines() {
    if (this.mapEdit.points) {
      this.pointMap = this.mapEdit.points.reduce((o, d) => ({ ...o, [d.code]: d }), {});

      for (let p of this.mapEdit.points) {
        this.createPoint([p.x, p.y], p.code);
      }
    }

    if (this.mapEdit.lines) {
      this.lineMap = this.mapEdit.lines.reduce((o, d) => ({ ...o, [d.code]: d }), {});

      for (let l of this.mapEdit.lines) {
        let f0 = this.pointFeatureMap[l.code0];
        let f1 = this.pointFeatureMap[l.code1];
        this.createLine(f0, f1, l.code, l.id);
      }
    }
  }

  deleteLine(line: Feature) {
    this.vectorSource.removeFeature(line);
    if (this.lineMap[line.getId()]) this.lineMap[line.getId()].isActive = false;
  }

  deletePoint(selected: Feature, selectedObj: PointModel) {
    if (!confirm("Are you sure want to delete point?")) return;


    this.vectorSource.removeFeature(selected);
    if (this.pointMap[selected.getId()]) this.pointMap[selected.getId()].isActive = false;

    let prop = selected.getProperties();

    for (let li of prop.line0 || []) {
      this.deleteLine(li);
    }

    for (let li of prop.line1 || []) {
      this.deleteLine(li);
    }

    this.unselectPoint(); 
  }

  okClicked() {
    this.showEditInfo = false;
    this.unselectPoint();
  }

  public uploadFinished = (event) => {
    this.mapEdit.map_url = event ? event.dbPath : null;

    if (this.mapEdit.map_url) this.mapEdit.map_url = this.mapEdit.map_url.replace(/\\/g, '/');

    this.initMap();
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    for (let f of this.vectorSource.getFeatures()) {
      let prop = f.getProperties();
      if (!prop.isPoint && !prop.isLine) continue;

      let code = f.getId();
      let coordinate = f.getGeometry().getFirstCoordinate();

      if (prop.isPoint) {
        let point = this.pointMap[code];

        if (point) {
          point.x = Math.round(coordinate[0]);
          point.y = Math.round(coordinate[1]);
          //} else {
          //  point = new PointModel();

          //  point.code = code;

          //  point.x = Math.round(coordinate[0]);
          //  point.y = Math.round(coordinate[1]);

          //  point.isActive = true;
          //}

          if (!point.id && point.isActive) {
            if (!this.mapEdit.points) this.mapEdit.points = [];
            this.mapEdit.points.push(point);
          }
        }
      } else if (prop.isLine) {
        if (!this.lineMap[code]) {
          let line = new LineModel();

          line.code = code;

          line.code0 = prop.code0;
          line.code1 = prop.code1;

          line.isActive = true;

          if (this.pointMap[line.code0].isActive && this.pointMap[line.code1].isActive) {
            if (!this.mapEdit.lines) this.mapEdit.lines = [];
            this.mapEdit.lines.push(line);
          }
        }
      }
    }

    if (!this.mapEdit.id) {
      this.mapService.newMap(this.mapEdit).subscribe(map => this.saveSuccessHelper(map), error => this.saveFailedHelper(error));
    }
    else {
      this.mapService.updateMap(this.mapEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(map?: MapModel) {
    if (map)
      Object.assign(this.mapEdit, map);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewMap) {
      this.alertService.showMessage("Success", `Data \"${this.mapEdit.code}\" was created successfully`, MessageSeverity.success);
    }
    else {
      this.alertService.showMessage("Success", `Changes to data \"${this.mapEdit.code}\" was saved successfully`, MessageSeverity.success);
    }

    this.mapEdit = new MapModel();
    this.removeMap();
    if (this.changesSavedCallback)
      this.changesSavedCallback();
  
    //this.dialogRef.close();
  }

  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    //if (this.changesFailedCallback)
    //  this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.mapEdit = new MapModel();
    this.removeMap();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();
    //this.dialogRef.close();
  }

  removeMap() {
    document.getElementById('mapdiv').innerHTML = '';
    this.map = null;
  }

  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }

    //this.map.destroy();
    
  }

  loadInstitutions() {
    this.institutionService.getInstitutions()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.allInstitutions = results[0];

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve institutions from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  newMap() {
    this.showValidationErrors = true;
    this.isNewMap = true;
    this.mapEdit = new MapModel();
    return this.mapEdit;
  }

  editMap(map: MapModel) {
    if (map) {
      this.isNewMap = false;
      this.showValidationErrors = true;
      this.originalMap = map;
      this.mapEdit = new MapModel();
      Object.assign(this.mapEdit, map);

      this.initMap();

      return this.mapEdit;
    }
    else {
      return this.newMap();
    }
  }

  get canManageMaps() {
    return true;//this.accountService.userHasPermission(Permission.manageMapPermission);
  }

}
