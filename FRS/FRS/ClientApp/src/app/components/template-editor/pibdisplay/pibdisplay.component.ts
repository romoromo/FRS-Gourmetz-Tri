///// <reference path="../../../../../node_modules/@types/jquery/dist/jquery.slim.d.ts" />
/// <reference path="../../../../../node_modules/@types/signalr/index.d.ts" />
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { Component, Inject, OnInit, AfterViewInit, ChangeDetectorRef, HostListener, OnDestroy } from "@angular/core";
import { DialogData } from "../../device-manager/unknown-device/devices-management.component";
import Konva from "konva";
import { ActivatedRoute, Router } from "@angular/router";
import { PIBService } from "src/app/services/pib.service";
import { PIBTemplate } from "src/app/models/department.model";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { ShapeService } from "src/app/services/shape.service";
import { TextNodeService } from "src/app/services/text-node.service";
import { AuthService } from "src/app/services/auth.service";
import { AccountService } from "src/app/services/account.service";
import { DomSanitizer } from "@angular/platform-browser";
//import { ajax, css } from "jquery";
//import * as jquery from 'jquery';
//import * as $ from 'jquery';

//interface sig extends JQueryStatic {
//  signalR: any;
//  connection: any;
//  hubConnection: any;
//}

@Component({
  selector: 'pib-display-preview',
  templateUrl: './pibdisplay.component.html',
  styleUrls: ['./pibdisplay.component.css'],
})



export class PIBDisplayComponent implements OnInit, AfterViewInit, OnDestroy {
  dup_stage: Konva.Stage;
  dup_layer: Konva.Layer;
  imgSrc: string;
  previewImgUri: any;
  origImgUri: any;
  origWidth = 1304;
  origHeight = 984;
  failedHeartbeatCount = 0;
  isFirstLoad = true;
  private signalRCoreconnection: signalR.HubConnection;

  constructor(
    private shapeService: ShapeService,
    private textNodeService: TextNodeService,
    private pibService: PIBService,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private accountService: AccountService,
    private router: Router,
    private domSanitizer: DomSanitizer,
    private cd: ChangeDetectorRef) {
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    console.log(event.target);
    //console.log(event.target.innerWidth, event.target.innerHeight);
    this.compressImage(this.origImgUri, event.target.innerWidth, event.target.innerHeight).then(compressed => {
      //console.log(compressed);
      this.previewImgUri = this.domSanitizer.bypassSecurityTrustUrl(compressed.toString());
    })
  }

  ngOnInit() {
    const component = this;
    //$(function () { console.log('test'); });
    //var hub = (<signalR>$).connection.patientHub;
    //$.connection.hub.url = 'http://119.73.206.36:89/signalr';
    //var signalR: sig;
    var connection = $.hubConnection('http://119.73.206.36:89/signalr');
    var patientHub = connection.createHubProxy('patientHub');
    if (this.router.url.indexOf('/pibdisplay') > -1) {
      this.authService.reLoginDelegate = () => null;
      var queryParams = this.router.url.split('?');
      if (queryParams && queryParams.length > 0) {
        var queryParamSection = queryParams[1].split('&');
        if (queryParamSection && queryParamSection.length > 0) {
          var mac_address = queryParamSection[0].split('=')[1];

          setInterval(() => {
            component.pibService.heartbeat(mac_address).subscribe(result => {
              console.log('interval: heartbeat');
              if (result.isSuccess) {
                if (connection.state != $.signalR.connectionState.connected) {
                  //patientHub.on('newPatientUpdate', function () {
                  //  console.log('interval: Patient update from eMOS');
                  //  //component.previewImgUri = '';
                  //  component.displayPreview(mac_address, component);
                  //});

                  //console.log(result);
                  connection.qs = { 'bed': result.data.data.location_code };
                  connection.start()
                    .done(function () {
                      if (connection.state == $.signalR.connectionState.connected) {
                        console.log('Now connected, connection ID=' + connection.id);
                        component.failedHeartbeatCount = 0;
                        component.displayPreview(mac_address, component);
                      }
                    })
                    .fail(function () { console.log('Could not connect to eMOS signalr'); component.failedHeartbeatCount++; });
                } else {
                  //still connected to emos 
                  if (result.data && (result.data.isNew || component.failedHeartbeatCount > 3 || component.isFirstLoad)) {
                    component.failedHeartbeatCount = 0;
                    //component.previewImgUri = '';
                    component.displayPreview(mac_address, component);
                  }
                }
              }
            }, error => function (error) {
              component.alertService.stopLoadingMessage();
              component.isFirstLoad = true;
            });
          }, 30000);

          patientHub.on('newPatientUpdate', function () {
            console.log('firstLoad: Patient update from eMOS');
            //component.previewImgUri = '';
            component.displayPreview(mac_address, component);
          });

          if (this.isFirstLoad) {
            this.displayPreview(mac_address, component);
            this.pibService.heartbeat(mac_address).subscribe(result => {
              console.log('firstLoad: heartbeat');
              if (result.isSuccess) {
                if (result.data && (result.data.isNew || this.failedHeartbeatCount > 3 || component.isFirstLoad)) {
                  this.failedHeartbeatCount = 0;
                  //this.previewImgUri = '';
                  component.displayPreview(mac_address, component);
                }

                if (connection.state != $.signalR.connectionState.connected) {
                  //console.log(result);
                  connection.qs = { 'bed': result.data.data.location_code };
                  connection.start()
                    .done(function () { console.log('Now connected, connection ID=' + connection.id); })
                    .fail(function () { console.log('Could not connect to eMOS signalr'); this.failedHeartbeatCount++; });
                }
              }
            }, error => function (error) {
              this.alertService.stopLoadingMessage();
              this.isFirstLoad = true;
            });
          }
        }
      }
    }
  }

  displayPreview(mac_address: string, component: any) {
    component.isFirstLoad = false;
    component.cd.detectChanges();
    component.alertService.startLoadingMessage("Updating display...");
    component.pibService.getTemplateByMacAddress(mac_address).subscribe(result => {
      var template = result.data;
      template.isStaticLink = true;
      template.macAddress = mac_address;
      component.previewFromImage(template, component);
    });
  }

  previewFromImage(template: PIBTemplate, component: any): void {
    template.isPostToDevice = false;
    //var cleanTemplate = this.cleanUpTemplate(this.stage.toJSON());
    //this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    component.pibService.previewFromImage(template)
      .subscribe(res => {
        component.alertService.stopLoadingMessage();
        if (!res.isSuccess) {
          component.alertService.showStickyMessage("Update Error", "The below errors occured while update the display:" + "\n" + res.message, MessageSeverity.error);
          return;
        }

        
        component.previewImgUri = this.domSanitizer.bypassSecurityTrustUrl('data:image/png;base64, ' + res.data);
        component.origImgUri = 'data:image/png;base64, ' + res.data;
        window.dispatchEvent(new Event('resize'));
        console.log('done image sanitizing');
        component.cd.detectChanges();
      }
        , error => function (error) {
          component.alertService.stopLoadingMessage();
          console.log(error);
        });
  }

  cleanUpTemplate(json: string) {
    var template = JSON.parse(json);
    var children = template['children'][0]['children'];
    var newChildren = []
    for (var i = 0; i < children.length; i++) {
      if ((children[i]['className'] !== 'Line') && (!children[i]['attrs'] || children[i]['attrs']['zLayer'] === undefined)) {
        continue;
      } else {
        newChildren.push(children[i]);
      }
    }
    template['children'][0]['children'] = newChildren;
    return template;
  }

  safeGet(obj, prop, defaultValue) {
    try {
      return obj[prop] != null && obj[prop] !== undefined ? obj[prop] : defaultValue;
    } catch (e) {
      return defaultValue
    }
  }

  redrawFromJson(template: any, callback?: any): void {
    //var template = JSON.parse(json);
    this.dup_stage = new Konva.Stage({
      container: 'dup-container',
      width: this.origWidth,
      height: this.origHeight,
      drawBorder: template['attrs']['drawBorder'],
      //scaleX: 1 / 2,
      //scaleY: 1 / 2,
      //offsetWidth: .3,
      //offsetHeight: .3
    });
    //this.dup_stage.setAttr("drawBorder", true);
    this.dup_layer = new Konva.Layer();
    this.dup_stage.add(this.dup_layer);
    var images = [];
    var imgCount = 0;
    var templateChildren = template['children'][0]['children'];
    var sortedChildren = templateChildren.sort((a, b) => parseInt(this.safeGet(a['attrs'], 'zLayer', Number.MAX_VALUE)) - parseInt(this.safeGet(b['attrs'], 'zLayer', Number.MAX_VALUE)));
    var childAddedCount = 0;
    var imgCount = 0;
    var newZIndex = sortedChildren.length - 1;

    for (var i = 0; i < sortedChildren.length; i++) {
      var t = sortedChildren[i];
      if (t['className'] == 'Text') {
        const shape = new Konva.Text(t['attrs']);
        shape.setAttr('zLayer', childAddedCount);
        childAddedCount++;
        this.dup_layer.add(shape);
      } else if (t['className'] == 'Line') {
        const shape = new Konva.Line(t['attrs']);
        shape.setAttr('zLayer', childAddedCount);
        childAddedCount++;
        this.dup_layer.add(shape);
      } else if (t['className'] == 'Rect') {
        const shape = new Konva.Rect(t['attrs']);
        shape.setAttr('zLayer', childAddedCount);
        childAddedCount++;
        this.dup_layer.add(shape);
      } else if (t['className'] == 'Image' && t['attrs']['src']) {
        var attrs = t['attrs'];
        var src = t['attrs']['src'];
        images.push({ src: src, attrs: attrs, zLayer: childAddedCount });
        childAddedCount++;
        var component = this;
        var imageObj = new Image()
        const image = this.shapeService.image(imageObj, attrs);
        imageObj.onload = function () {
          for (var i in images) {
            if (images[i].src == decodeURI(image.attrs.src)) {
              ++imgCount;
              var x = image.attrs.x === undefined ? 0 : parseInt(image.attrs.x);
              var y = image.attrs.y === undefined ? 0 : parseInt(image.attrs.y);

              image.setAttrs(images[i].attrs);
              image.x(x);
              image.y(y);
              image.setAttr('zLayer', images[i].zLayer);
              image.zIndex(parseInt(images[i].zLayer));

              component.dup_layer.draw();
            }
          }

          if (images != null && images.length == imgCount) {
            component.dup_layer.batchDraw();
            callback(component, component.dup_layer);
          }
        };
        imageObj.src = src;
        this.dup_layer.add(image);
      }

      this.dup_stage.add(this.dup_layer);
      this.dup_layer.draw();
    }
    if (!images || images == null || images.length == 0) {
      this.reorderComponents(this, this.dup_layer, callback);
    }
  }

  reorderComponents(component, layer, callback?: any) {
    var sortedChildren = layer.children.sort((a, b) => parseInt(component.safeGet(a['attrs'], 'zLayer', Number.MAX_VALUE)) - parseInt(component.safeGet(b['attrs'], 'zLayer', Number.MAX_VALUE)));

    for (var i = 0; i < sortedChildren.length; i++) {
      var shape = sortedChildren[i];
      var index = parseInt(sortedChildren[i].getAttr('zLayer'));
      shape.zIndex(index);
      shape.setZIndex(index);

    }
    layer.batchDraw();
    if (callback) {
      callback(component);
    }
  }

  preview(template: PIBTemplate): void {
    //var cleanTemplate = this.cleanUpTemplate(template.templateBody);
    //this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    template.isPostToDevice = false;
    this.alertService.startLoadingMessage("Generating display...");
    this.pibService.mapTemplate(template)
      .subscribe(res => {
        if (!res.isSuccess) {
          this.alertService.showStickyMessage("Post Error", "The below errors occured while posting your template to the device:" + "\n" + res.message, MessageSeverity.error);
          return;
        }
        //var component = this;
        this.redrawFromJson(res.data, function (component) {


          // clone original layer, and disable all events on it
          // we will use "let" here, because we can redefine layer later
          component.preview_stage = new Konva.Stage({
            container: 'preview-container',
            width: component.origWidth,
            height: component.origHeight,
            scaleX: 1,
            scaleY: 1,
            //offsetWidth: .3,
            //offsetHeight: .3
          });

          component.preview_layer = component.dup_layer.clone({ hitGraphEnabled: false });
          component.preview_stage.add(component.preview_layer);
          component.preview_layer.batchDraw();
          component.is_preview = true;
          component.alertService.stopLoadingMessage();
          var curi = component.dup_stage.toCanvas({}).toDataURL();
          var dataurl = component.dup_stage.toDataURL();
          template.name = 'template.png';
          template.imgUrl = dataurl;
          component.imgSrc = dataurl;
          //var url = "http://183.90.63.88:3000/epaper";
          //component.pibService.postTemplateToDevice(url, template).subscribe(res => {
          //  console.log(res);
          //  //alert('Posted to device');
          //  component.alertService.stopLoadingMessage();
          //  if (res.isSuccess) {
          //    component.alertService.showMessage("Success", "Successfully updated the device", MessageSeverity.success);
          //  }
          //  else {
          //    component.alertService.showStickyMessage("Post Error", "The below errors occured while posting your template to the device:" + "\n" + res.message, MessageSeverity.error);
          //  }

          //}, error => function (error) {
          //  console.log(error);
          //});
        });
      }
        , error => function (error) {
          this.alertService.stopLoadingMessage();
          console.log(error);
        });
  }

  updatePreview(layer, previewLayer) {
    // we just need to update ALL nodes in the preview
    layer.children.forEach(shape => {
      // find cloned node
      const clone = previewLayer.findOne('.' + shape.name());
      // update its position from the original
      clone.position(shape.position());
    });
    previewLayer.batchDraw();
  }

  ngAfterViewInit() {
  }

  compressImage(src, newX, newY) {
    var wScale = this.origWidth / newX;
    var hScale = this.origHeight / newY;
    return new Promise((res, rej) => {
      const img = new Image();
      img.onload = () => {
        const elem = document.createElement('canvas');
        elem.width = newX;
        elem.height = newY;
        const ctx = elem.getContext('2d');
        ctx.drawImage(img, 0, 0, newX, newY);
        const data = ctx.canvas.toDataURL('image/png', 0.9);
        res(data);

        //var canvas = document.createElement('canvas'),
        //  ctx = canvas.getContext("2d"),
        //  oc = document.createElement('canvas'),
        //  octx = oc.getContext('2d');

        //canvas.width = newX; // destination canvas size
        //canvas.height = canvas.width * newY / newX;

        //var cur = {
        //  width: Math.floor(newX * 0.5),
        //  height: Math.floor(newY * 0.5)
        //}

        //oc.width = cur.width;
        //oc.height = cur.height;

        //octx.drawImage(img, 0, 0, cur.width, cur.height);

        //while (cur.width * 0.5 > newX) {
        //  cur = {
        //    width: Math.floor(cur.width * 0.5),
        //    height: Math.floor(cur.height * 0.5)
        //  };
        //  octx.drawImage(oc, 0, 0, cur.width * 2, cur.height * 2, 0, 0, cur.width, cur.height);
        //}

        //ctx.drawImage(oc, 0, 0, cur.width, cur.height, 0, 0, canvas.width, canvas.height);

        //const data = ctx.canvas.toDataURL();
        //res(data);
      }
      img.src = src;
      img.onerror = error => rej(error);
    })
  }

  ngOnDestroy() {

    this.signalRCoreconnection.stop();
    this.alertService.stopLoadingMessage();
    //this.subscription.unsubscribe();
  }
}
