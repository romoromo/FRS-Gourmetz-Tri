import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Component, Inject, OnInit, AfterViewInit } from "@angular/core";
import Konva from "konva";
import { ActivatedRoute, Router } from "@angular/router";
import { PIBService } from "src/app/services/pib.service";
import { PIBTemplate } from "src/app/models/department.model";
import { AlertService, MessageSeverity } from "src/app/services/alert.service";
import { ShapeService } from "src/app/services/shape.service";
import { TextNodeService } from "src/app/services/text-node.service";
import { AuthService } from "src/app/services/auth.service";
import { AccountService } from "src/app/services/account.service";

@Component({
  selector: 'template-editor-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.css'],
})
export class TemplateEditorPreviewDialog implements OnInit, AfterViewInit {
  dup_stage: Konva.Stage;
  dup_layer: Konva.Layer;
  imgSrc: string;

  origWidth = 1304;
  origHeight = 984;

  constructor(
    private shapeService: ShapeService,
    private textNodeService: TextNodeService,
    private pibService: PIBService,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private accountService: AccountService,
    private router: Router) {
  }

  ngOnInit() {
    if (this.router.url.indexOf('/pibdevicetemplate') > -1) {
      this.authService.reLoginDelegate = () => null;
      var queryParams = this.router.url.split('?');
      if (queryParams && queryParams.length > 0) {
        var queryParamSection = queryParams[1].split('&');
        if (queryParamSection && queryParamSection.length > 0) {
          var mac_address = queryParamSection[0].split('=')[1];
          this.pibService.getTemplateByMacAddress(mac_address).subscribe(result => {
            var template = result.data;
            template.isStaticLink = true;
            template.macAddress = mac_address;
            this.preview(template);
          });
        }
      }
    }
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
          var url = "http://183.90.63.88:3000/epaper";
          component.pibService.postTemplateToDevice(url, template).subscribe(res => {
            console.log(res);
            //alert('Posted to device');
            component.alertService.stopLoadingMessage();
            if (res.isSuccess) {
              component.alertService.showMessage("Success", "Successfully updated the device", MessageSeverity.success);
            }
            else {
              component.alertService.showStickyMessage("Post Error", "The below errors occured while posting your template to the device:" + "\n" + res.message, MessageSeverity.error);
            }

          }, error => function (error) {
            console.log(error);
          });
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
    //var template = this.dlgData.template;
    //this.dup_stage = new Konva.Stage({
    //  container: 'dup-container',
    //  width: this.dlgData.origWidth,
    //  height: this.dlgData.origHeight,
    //  drawBorder: template['attrs']['drawBorder'],
    //  //offsetWidth: .3,
    //  //offsetHeight: .3
    //});
    ////this.dup_stage.setAttr("drawBorder", true);
    //this.dup_layer = new Konva.Layer();
    //this.dup_stage.add(this.dup_layer);
    //var images = [];
    //for (var i = 0; i < template['children'][0]['children'].length; i++) {
    //  var t = template['children'][0]['children'][i];
    //  if (t['className'] == 'Text') {
    //    const shape = new Konva.Text(t['attrs']);
    //    this.dup_layer.add(shape);
    //  } else if (t['className'] == 'Line') {
    //    const shape = new Konva.Line(t['attrs']);
    //    this.dup_layer.add(shape);
    //  } else if (t['className'] == 'Rect') {
    //    const shape = new Konva.Rect(t['attrs']);
    //    this.dup_layer.add(shape);
    //  } else if (t['className'] == 'Image' && t['attrs']['src']) {
    //    var attrs = t['attrs'];
    //    var src = t['attrs']['src'];
    //    images.push({ src: src, attrs: attrs });
    //    var component = this;
    //    Konva.Image.fromURL(src, function (node) {
    //      for (var i in images) {
    //        if (images[i].src == node.attrs.image.src) {
    //          node.setAttrs(images[i].attrs);
    //        }
    //      }

    //      component.dup_layer.add(node);
    //      component.dup_layer.draw();
    //    });
    //  }

    //  this.dup_stage.add(this.dup_layer);
    //  this.dup_layer.draw();
    //}
  }
}

platformBrowserDynamic().bootstrapModule(TemplateEditorPreviewDialog);
