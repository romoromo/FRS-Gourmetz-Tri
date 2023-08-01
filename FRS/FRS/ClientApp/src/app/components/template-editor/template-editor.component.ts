import { Component, OnInit, ChangeDetectorRef, ViewChild, TemplateRef } from '@angular/core';
import Konva from 'konva';
import { ShapeService } from 'src/app/services/shape.service';
import { TextNodeService } from 'src/app/services/text-node.service';
import { trigger, transition, animate, style, state } from '@angular/animations';
import { ConfigurationService } from 'src/app/services/configuration.service';
import { FileService } from 'src/app/services/file.service';
import { PIBService } from 'src/app/services/pib.service';
import { PIBTemplate } from 'src/app/models/department.model';
import { saveAs } from 'file-saver';
import { TemplateEditorPreviewDialog } from './preview/preview.component';
import { MatDialog } from '@angular/material';
import { Permission } from 'src/app/models/permission.model';
import { AlertService, MessageSeverity, DialogType } from 'src/app/services/alert.service';
import { AppTranslationService } from 'src/app/services/app-translation.service';
import { Utilities } from 'src/app/services/utilities';
import { TemplateSaveDialogPreviewDialog } from './save-dialog/save-dialog.component';
import { AccountService } from 'src/app/services/account.service';
import { map } from 'rxjs/operators';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-template-editor',
  templateUrl: './template-editor.component.html',
  styleUrls: ['./template-editor.component.css'],
  host: {
    '(window:resize)': 'onResize($event)'
  },
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)' }),
        animate('200ms ease-in', style({ transform: 'translateX(0%)' }))
      ]),
      transition(':leave', [
        animate('200ms ease-in', style({ transform: 'translateX(-100%)' }))
      ])
    ]),
    trigger('slide', [
      state('left', style({ transform: 'translateX(0)' })),
      state('right', style({ transform: 'translateX(-100%)' })),
      transition('* => *', animate(300))
    ])
  ]
})
export class TemplateEditorComponent implements OnInit {

  //#list
  columns: any[] = [];
  rows: PIBTemplate[] = [];
  rowsCache: PIBTemplate[] = [];
  allPermissions: Permission[] = [];
  editedTemplate: PIBTemplate;
  sourceTemplate: PIBTemplate;
  editingTemplateName: { name: string };
  loadingIndicator: boolean;
  private isSaving: boolean;


  @ViewChild('indexTemplate')
  indexTemplate: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  header: string;

  //#editor
  origWidth = 1304;
  origHeight = 984;
  stageWidth = this.origWidth * 1;
  stageHeight = this.origHeight * 1;
  selectedShape = { label: '', type: '', is_variable: false, is_font_bold: false, is_icon: false };
  jsonValue: any;
  is_variable: boolean = false;
  is_icon: boolean = false;
  is_patient_info: boolean = false;
  is_patient_restriction: boolean = false;
  is_group_patient_restriction: boolean = false;
  is_selected: boolean = false;
  is_preview: boolean = false;
  previewImgUri: any;

  shapes: any = [];
  stage: Konva.Stage;
  layer: Konva.Layer;
  currentText: Konva.Text;
  currentShape: Konva.Shape;
  currentShapeModel: any = {};

  //dups
  dup_stage: Konva.Stage;
  dup_layer: Konva.Layer;
  preview_stage: Konva.Stage;
  preview_layer: Konva.Layer;
  selectedButton: any = {
    'circle': false,
    'rectangle': false,
    'line': false,
    'undo': false,
    'erase': false,
    'text': false,
    'patient_info': false,
    'patient_restrictions': false
  }
  erase: boolean = false;
  transformers: Konva.Transformer[] = [];
  fileUploadResponse: { dbPath: '', fileId: null, fileName: '' };
  isFileUpload: boolean = false;
  font_families = ['Arial', 'Calibri', 'Tahoma', 'Times New Roman'];
  font_styles = ['normal', 'bold', 'italic'];
  font_colors = ['white', 'black'];
  text_overflows = ['wrap', 'resize', 'ellipsis', 'resize and wrap'];
  variable_names: any = [
    { label: 'AHP Remarks', val: 'ahp_remarks' },
    { label: 'Attending Doctor', val: 'attending_doctor' },
    { label: 'Attending Nurse', val: 'attending_nurse' },
    { label: 'Case No.', val: 'case_number' },
    { label: 'Drug Allergy', val: 'drug_allergies_flatten' },
    { label: 'Gender', val: 'gender' },
    { label: 'Languages', val: 'languages_flatten' },
    { label: 'Location Alias', val: 'location_alias' },
    { label: 'Location Label', val: 'location_label' },
    { label: 'Name', val: 'name' },
    { label: 'NRIC', val: 'identifier' },
    { label: 'Ongoing Remarks', val: 'ongoing_remarks' },
    { label: 'PIB Location', val: 'pib_location' },
    { label: 'Preferred Name', val: 'preferred_name' }
  ];

  locations: any = [];
  restriction_variable_names: any = [
    //{ label: 'Mode of Feeding', val: 'mode_of_feeding' },
    //{ label: 'Food Restrictions', val: 'food_restrictions' },
    //{ label: 'Diet Type', val: 'diet_type' },
    //{ label: 'Diet Texture & Fluid Consistency', val: 'diet_texture' },
    //{ label: 'Fluid Restrictions', val: 'fluid_restrictions' },
    //{ label: 'Functional Status', val: 'function_status' },
    //{ label: 'Special Instructions', val: 'special_instructions' },
  ];
  showNewTemplate: boolean;

  constructor(
    private shapeService: ShapeService,
    private textNodeService: TextNodeService,
    private fileService: FileService,
    private configurationService: ConfigurationService,
    private pibService: PIBService,
    private ref: ChangeDetectorRef,
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private dialog: MatDialog,
    private accountService: AccountService,
    private domSanitizer: DomSanitizer
  ) { }

  onResize(event) {
    this.fitStageIntoParentContainer();
  }

  public uploadFinished = (event) => {
    this.fileUploadResponse = event;
    //console.log(this.fileUploadResponse.dbPath);
    //console.log(this.fileUploadResponse.dbPath.replace(/\\/g, '/'));
    this.addImage(this.fileUploadResponse ? this.configurationService.baseUrl + '/' + this.fileUploadResponse.dbPath.replace(/\\/g, '/') : null);
  }

  getFileImage(path) {
    return this.fileService.getFile(path);
  }

  ngOnInit() {
    //this.variable_names = this.variable_names.pipe(map(arr => arr.sort((a, b) => a > b)));
    this.initListTable();
    this.initTemplate();
  }

  getLocations() {
    this.pibService.getLocations()
      .subscribe(results => {
        this.locations = results[0];
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\n"`,
            MessageSeverity.error);
        });
  }

  getRestrictionTypes() {
    this.pibService.getRestrictionTypes()
      .subscribe(results => {
        this.restriction_variable_names = results[0];
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving restriction types.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving restriction.\r\n"`,
          MessageSeverity.error);
        });
  }

  initTemplate() {
    //initialised values
    this.selectedShape = { label: '', type: '', is_variable: false, is_font_bold: false, is_icon: false };
    this.jsonValue = null;
    this.is_variable = false;
    this.is_icon = false;
    this.is_patient_info = false;
    this.is_patient_restriction = false;
    this.is_group_patient_restriction = false;
    this.is_selected = false;
    this.is_preview = false;

    this.shapes = [];
    this.stage = null;
    this.layer = null;
    this.currentText = null;
    this.currentShape = null;
    this.currentShapeModel = {};

    //dups
    this.dup_stage = null;
    this.dup_layer = null;
    this.preview_stage = null;
    this.preview_layer = null;
    this.selectedButton = {
      'circle': false,
      'rectangle': false,
      'line': false,
      'undo': false,
      'erase': false,
      'text': false,
      'patient_info': false,
      'patient_restrictions': false
    }
    this.erase = false;
    this.transformers = [];
    this.fileUploadResponse = { dbPath: '', fileId: null, fileName: '' };
    this.isFileUpload = false;
    this.showNewTemplate = false;

    //var element = document.getElementById('template_editor_container');
    //var positionInfo = element.getBoundingClientRect();
    //console.log(positionInfo);
    //var height = positionInfo.height;
    //var width = positionInfo.width;
    this.origWidth = 1304;
    this.origHeight = 984;
    let width = this.origWidth * 0.75;
    let height = this.origHeight * 0.75;

    //let width = window.innerWidth * 0.9;
    //let height = window.innerHeight;
    this.stage = new Konva.Stage({
      container: 'container',
      width: this.stageWidth,
      height: this.stageHeight,
      drawBorder: true,
      offsetWidth: .3,
      offsetHeight: .3
    });
    this.layer = new Konva.Layer();

    this.stage.add(this.layer);
    this.addLineListeners();

    //this.fitStageIntoParentContainer();
    window.dispatchEvent(new Event('resize'));
    // adapt the stage on any window resize
    //window.addEventListener('resize', function () { fitStageIntoParentContainer(this.stage, this.stageWidth, this.stageHeight) });
  }

  clearSelection() {
    Object.keys(this.selectedButton).forEach(key => {
      this.selectedButton[key] = false;
    })
  }
  setSelection(type: string) {
    this.selectedButton[type] = true;
  }
  addShape(type: string) {
    this.clearSelection();
    this.isFileUpload = false;
    this.setSelection(type);
    if (type == 'circle') {
      this.addCircle();
    }
    else if (type == 'line') {
      this.addLine();
    }
    else if (type == 'rect') {
      this.addRectangle();
    }
    else if (type == 'text') {
      this.addText('type here', false, false, false);
    }
    else if (type == 'patient_info') {
      this.addText('Patient Info Here', true, false, false);
    }
    else if (type == 'restrictions') {
      this.addText('Restriction Here', false, true, false);
    }
    else if (type == 'group_restrictions') {
      this.addText('Restriction Here', false, false, true);
    }
    else if (type == 'image') {
      //this.addImage();
      this.isFileUpload = true;
    }

    this.selectedShape.type = type;
  }
  addText(txt: string, isPatient: boolean, isRestriction: boolean, isGrpRestriction: boolean) {
    this.stage.container().style.cursor = 'text';
    const text = this.textNodeService.textNode(this.stage, this.layer, txt);
    text.textNode.setAttr('is_patient_info', isPatient);
    text.textNode.setAttr('is_patient_restriction', isRestriction);
    text.textNode.setAttr('is_group_patient_restriction', isGrpRestriction);
    text.textNode.setAttr('zLayer', this.layer.children.length);
    this.shapes.push(text.textNode);
    //this.transformers.push(text.tr);
    this.addTransformerListeners(text.textNode);
    //text.textNode.fire('click');
  }
  addCircle() {
    const circle = this.shapeService.circle();
    this.shapes.push(circle);
    this.layer.add(circle);
    this.stage.add(this.layer);
    this.addTransformerListeners();

    //circle.fire('click');
  }
  addRectangle() {
    const rectangle = this.shapeService.rectangle();
    rectangle.setAttr('zLayer', this.layer.children.length);
    this.shapes.push(rectangle);
    this.layer.add(rectangle);
    this.stage.add(this.layer);
    this.addTransformerListeners()
  }
  addLine() {
    this.selectedButton['line'] = true;
  }
  addImage(src) {
    //var imageSource = this.configurationService.baseUrl + '/Resources/Images/sunny.bmp';
    //var imageSource2 = this.configurationService.baseUrl + '/Resources/Images/darth.png';

    this.selectedButton['image'] = true;
    //var img = new Image()
    //const image = this.shapeService.image(img);
    //this.shapes.push(image);
    //this.layer.add(image);
    //this.stage.add(this.layer);
    //
    var component = this;
    //var imageObj = new Image();
    //
    //imageObj.onload = function () {
    //  image.image(imageObj);
    //  component.layer.batchDraw();
    //};
    //imageObj.src = imageSource;

    Konva.Image.fromURL(src, function (node) {
      node.setAttrs({
        x: 60,
        y: 20,
        draggable: true,
        src: src,
        zLayer: component.layer.children.length
      });
      component.layer.add(node);
      component.layer.batchDraw();
    });
    this.addTransformerListeners();

  }
  addLineListeners() {
    const component = this;
    let lastLine;
    let isPaint;
    let isHorizontal;
    let isVertical;
    this.stage.on('mousedown touchstart', function (e) {
      if (!component.selectedButton['line'] && !component.erase) {
        return;
      }
      isPaint = true;
      let pos = component.stage.getPointerPosition();
      const mode = component.erase ? 'erase' : 'brush';
      lastLine = component.shapeService.line(pos, mode)
      component.shapes.push(lastLine);
      component.layer.add(lastLine);
      this.container().style.cursor = mode === 'erase' ? 'grab' : 'crosshair';
      component.addTransformerListeners();
    });
    this.stage.on('mouseup touchend', function () {
      isPaint = false;
      isHorizontal = false;
      isVertical = false;
      //const pos = component.stage.getPointerPosition();
      //var newPoints = lastLine.points().concat([pos.x, pos.y]);
      //lastLine.points(newPoints);
      //component.layer.batchDraw();
      this.container().style.cursor = 'default';
      //component.addTransformerListeners();
      component.clearSelection();
    });
    // and core function - drawing
    this.stage.on('mousemove touchmove', function () {
      if (!isPaint) {
        return;
      }
      var oldPoints = lastLine.getSelfRect();
      const pos = component.stage.getPointerPosition();
      var newPoints = lastLine.points().concat([pos.x, pos.y]);
      if (!isVertical && pos.x > oldPoints.x) {
        newPoints = lastLine.points().concat([pos.x, oldPoints.y]);
        isHorizontal = true;
      } else {
        if (!isHorizontal && pos.y > oldPoints.y) {
          newPoints = lastLine.points().concat([oldPoints.x, pos.y]);
          isVertical = true;
        }
      }
      const mode = component.erase ? 'erase' : 'brush';
      this.container().style.cursor = mode === 'erase' ? 'grab' : 'crosshair';
      lastLine.points(newPoints);
      component.layer.batchDraw();
    });
  }
  undo() {
    const removedShape = this.shapes.pop();
    this.transformers.forEach(t => {
      t.detach();
    });
    if (removedShape) {
      removedShape.remove();
    }
    this.layer.draw();
    this.is_selected = false;
  }
  delete() {
    this.currentShape.remove();
    this.transformers.forEach(t => {
      t.detach();
    });
    const removeShape = this.shapes.find(s => s._id == this.currentShape._id);
    if (removeShape) {
      removeShape.remove();
    }
    this.layer.draw();
    this.is_selected = false;
  }
  addTransformerListeners(textNode?: any) {
    const component = this;
    const tr = new Konva.Transformer();
    //const tr = !textNode ? new Konva.Transformer() : new Konva.Transformer({
    //  node: textNode as any,
    //  enabledAnchors: ['middle-left', 'middle-right'],
    //  // set minimum width of text
    //  boundBoxFunc: function (oldBox, newBox) {
    //    newBox.width = Math.max(30, newBox.width);
    //    return newBox;
    //  }
    //});

    this.stage.on('click', function (e) {
      if (!this.clickStartShape) {
        return;
      }
      if (e.target._id == this.clickStartShape._id) {
        //tr.detach();
        //component.layer.draw();
        component.selectedShape.label = this.clickStartShape.className;
        component.currentShape = this.clickStartShape;
        if (this.clickStartShape.className === 'Text') {
          component.currentText = this.clickStartShape as Konva.Text;
          component.setCurrentTextValues(component, component.currentText);
        } else {
          component.selectedShape.is_variable = false;
          component.selectedShape.is_icon = false;
          component.selectedShape.type = this.clickStartShape.className.toLowerCase();
          if (this.clickStartShape.className === 'Line') {
            component.currentShape = this.clickStartShape as Konva.Line;
            component.currentShapeModel.stroke = component.currentShape.stroke();
            component.currentShapeModel.strokeWidth = component.currentShape.strokeWidth();
            //component.currentShapeModel.zIndex = component.currentShape.zIndex();
            component.currentShapeModel.zLayer = component.currentShape.getAttr('zLayer') === undefined ? component.currentShape.zIndex() : component.currentShape.getAttr('zLayer');
            //component.currentShapeModel.x = component.currentShape.points
            //component.currentShapeModel.y = component.currentShape.y();
          } else {
            component.currentShape = this.clickStartShape;
            component.currentShapeModel.x = component.currentShape.x();
            component.currentShapeModel.y = component.currentShape.y();
            component.currentShapeModel.fill = component.currentShape.fill();
            //component.currentShapeModel.zIndex = component.currentShape.zIndex();
            component.currentShapeModel.zLayer = component.currentShape.getAttr('zLayer') === undefined ? component.currentShape.zIndex() : component.currentShape.getAttr('zLayer');
            //component.currentShapeModel.width = component.currentShape.width();
            //component.currentShapeModel.width = component.currentShape.width();
          }
          component.currentShapeModel.id = component.currentShape.id;
        }
        component.is_selected = true;
        //component.addDeleteListener(e.target);
        component.layer.add(tr);
        tr.nodes([e.target]);
        //tr.attachTo(e.target);
        component.transformers.push(tr);
        //component.transformers.push(tr);
        component.layer.draw();
        component.ref.detectChanges();
      }
      else {
        tr.detach();
        component.layer.draw();
      }
    });
    this.stage.on('dragend', function (e) {
      if (!this.clickStartShape) {
        return;
      }
      if (e.target._id == this.clickStartShape._id) {
        if (this.clickStartShape.className === 'Text') {
          component.currentText = this.clickStartShape as Konva.Text;
          component.setCurrentTextValues(component, component.currentText);
        } else {
          component.selectedShape.is_variable = false;
          component.selectedShape.is_icon = false;
          component.selectedShape.type = this.clickStartShape.className.toLowerCase();
          if (this.clickStartShape.className === 'Line') {
            component.currentShape = this.clickStartShape as Konva.Line;
            component.currentShapeModel.stroke = component.currentShape.stroke();
            component.currentShapeModel.strokeWidth = component.currentShape.strokeWidth();
            //component.currentShapeModel.zIndex = component.currentShape.zIndex();
            component.currentShapeModel.zLayer = component.currentShape.getAttr('zLayer') === undefined ? component.currentShape.zIndex() : component.currentShape.getAttr('zLayer');
            //component.currentShapeModel.x = component.currentShape.points
            //component.currentShapeModel.y = component.currentShape.y();
          } else {
            component.currentShape = this.clickStartShape;
            component.currentShapeModel.x = component.currentShape.x();
            component.currentShapeModel.y = component.currentShape.y();
            component.currentShapeModel.fill = component.currentShape.fill();
            //component.currentShapeModel.zIndex = component.currentShape.zIndex();
            component.currentShapeModel.zLayer = component.currentShape.getAttr('zLayer') === undefined ? component.currentShape.zIndex() : component.currentShape.getAttr('zLayer');
            //component.currentShapeModel.width = component.currentShape.width();
            //component.currentShapeModel.width = component.currentShape.width();
          }
          component.currentShapeModel.id = component.currentShape.id;
        }

        //component.is_selected = true;
      }
    });
  }

  setCurrentTextValues(component: any, txtNode: Konva.Text) {
    component.currentShapeModel.fontSize = component.currentText.fontSize();
    component.currentShapeModel.fontFamily = component.currentText.fontFamily();
    component.currentShapeModel.fontStyle = component.currentText.fontStyle();
    //component.currentShapeModel.stroke = component.currentText.stroke();
    component.currentShapeModel.fill = component.currentText.fill();
    component.currentShapeModel.text = component.currentText.text();
    component.currentShapeModel.x = component.currentText.x();
    component.currentShapeModel.y = component.currentText.y();
    component.currentShapeModel.is_variable = component.currentText.getAttr('is_variable') === undefined ? false : component.currentText.getAttr('is_variable');
    component.currentShapeModel.is_icon = component.currentText.getAttr('is_icon') === undefined ? false : component.currentText.getAttr('is_icon');
    component.currentShapeModel.is_patient_info = component.currentText.getAttr('is_patient_info') === undefined ? false : component.currentText.getAttr('is_patient_info');
    component.currentShapeModel.is_patient_restriction = component.currentText.getAttr('is_patient_restriction') === undefined ? false : component.currentText.getAttr('is_patient_restriction');
    component.currentShapeModel.is_group_patient_restriction = component.currentText.getAttr('is_group_patient_restriction') === undefined ? false : component.currentText.getAttr('is_group_patient_restriction');
    component.currentShapeModel.variable_name = component.currentText.getAttr('variable_name') === undefined ? '' : component.currentText.getAttr('variable_name');
    component.currentShapeModel.group_name = component.currentText.getAttr('group_name') === undefined ? '' : component.currentText.getAttr('group_name');
    component.currentShapeModel.group_variable_name = component.currentText.getAttr('group_variable_name') === undefined ? '' : component.currentText.getAttr('group_variable_name');
    component.currentShapeModel.is_background_image = component.currentText.getAttr('is_background_image');
    component.currentShapeModel.zLayer = component.currentText.getAttr('zLayer') === undefined ? component.currentText.zIndex() : component.currentText.getAttr('zLayer');
    component.currentShapeModel.id = component.currentText.id;
    component.currentShapeModel.text_wrap = component.currentText.getAttr('text_wrap') === undefined ? '' : component.currentText.getAttr('text_wrap');
    //component.currentShapeModel.zIndex = component.currentText.zIndex();

    component.selectedShape.is_variable = component.currentShapeModel.is_variable;
    component.selectedShape.is_icon = component.currentShapeModel.is_icon;
    //component.selectedShape.is_font_bold = component.currentText.fontStyle() == 'bold';
    if (component.currentShapeModel.is_patient_info) {
      component.selectedShape.label = 'Patient Info';
      component.selectedShape.type = 'patient_info';
    } else if (component.currentShapeModel.is_patient_restriction) {
      component.selectedShape.label = 'Restrictions';
      component.selectedShape.type = 'restrictions';
    } else if (component.currentShapeModel.is_group_patient_restriction) {
      component.selectedShape.label = 'Group Restrictions';
      component.selectedShape.type = 'group_restrictions';
    } else {
      component.selectedShape.label = 'Text';
      component.selectedShape.type = 'text';
    }
  }
  addDeleteListener(shape) {
    const component = this;
    window.addEventListener('keydown', function (e) {
      if (e.keyCode === 46) {
        shape.remove();
        component.transformers.forEach(t => {
          t.detach();
        });
        const selectedShape = component.shapes.find(s => s._id == shape._id);
        selectedShape.remove();
        e.preventDefault();
      }
      component.layer.batchDraw();
    });
  }
  generateJson() {
    //console.log(this.stage);
    //console.log(JSON.stringify(this.stage));
    this.jsonValue = this.stage.toJSON();
  }
  fitStageIntoParentContainer() {
    //var container = <HTMLElement> document.querySelector('#stage-parent');

    //// now we need to fit stage into parent
    //var containerWidth = container.offsetWidth;
    //var containerHeight = container.offsetHeight;
    //// to do this we need to scale the stage
    //var scaleW = containerWidth / this.stageWidth;
    //var scaleH = containerHeight / this.stageHeight;

    //if (scaleW == 0) scaleW = 1;
    //if (scaleH == 0) scaleH = 1;
    //this.stage.width(this.stageWidth * scaleW);
    //this.stage.height(this.stageHeight * scaleH);
    //this.stage.scale({ x: scaleW, y: scaleH });
    //this.stage.draw();
  }
  updateValue(type, val, triggerValue?: any) {
    if (this.selectedShape.type != 'text' && this.selectedShape.type != 'patient_info' && this.selectedShape.type != 'restrictions' && this.selectedShape.type != 'group_restrictions') {
      if (type == 'x') {
        this.currentShape.x(val);
      }
      else if (type == 'y') {
        this.currentShape.y(val);
      }
      else if (type == 'width') {
        this.currentShape.size().width = val;
      }
      else if (type == 'height') {
        this.currentShape.size().height = val;
      }
      else if (type == 'stroke') {
        this.currentShape.stroke(val);
      }
      else if (type == 'strokeWidth') {
        this.currentShape.strokeWidth(val);
      }
      else if (type == 'strokeWidth') {
        this.currentShape.strokeWidth(val);
      }
      else if (type == 'zLayer') {
        this.currentShape.zIndex(val);
        this.currentShape.setAttr('zLayer', val);
      } else if (type == 'fill') {
        this.currentShape.fill(val);
        //this.currentText.stroke(val);
      }
      this.layer.draw();
      //this.currentText.getStage().batchDraw();
    } else {
      if (type == 'x') {
        this.currentText.x(val);
      }
      else if (type == 'y') {
        this.currentText.y(val);
      }
      else if (type == 'fontSize') {
        this.currentText.fontSize(val);
      }
      else if (type == 'fontFamily') {
        this.currentText.fontFamily(val);
      }
      else if (type == 'fontStyle') {
        this.currentText.fontStyle(val);
      }
      else if (type == 'zLayer') {
        this.currentText.zIndex(val);
        this.currentText.setAttr('zLayer', val);
      }
      else if (type == 'text_wrap') {
        this.currentText.setAttr('text_wrap', val);
      }
      else if (type == 'fill') {
        this.currentText.fill(val);
        //this.currentText.stroke(val);
      }
      //else if (type == 'fontStyle') {
      //  this.selectedShape.is_font_bold = val;
      //  if (val) {
      //    this.currentText.fontStyle('bold');
      //  } else {
      //    this.currentText.fontStyle('normal');
      //  }
      //}
      else if (type == 'group_name') {
        this.currentText.setAttr('group_name', val);
      }
      else if (type == 'group_variable_name') {
        this.currentText.setAttr('group_variable_name', val);
        var text = '[' + triggerValue + ']';
        this.currentText.text(text)
        this.currentShapeModel.text = text;
      }
      else if (type == 'text') {
        this.currentText.text(val);
      }
      else if (type == 'variable_name') {

        if (this.selectedShape.is_variable) {
          var text = '';
          if (this.is_patient_restriction && this.currentText.getAttr('is_patient_restriction')) {
            text = '[' + triggerValue + ']';
            this.currentText.setAttr('variable_name', val);
          } if (this.is_group_patient_restriction && this.currentText.getAttr('is_group_patient_restriction')) {
            text = '[' + triggerValue + ']';
            this.currentText.setAttr('variable_name', val);
          } else {
            this.currentText.setAttr('variable_name', val);
            text = '[' + val + ']';
          }
          this.currentText.text(text);
          //this.currentText.fontStyle('italic');
          this.currentShapeModel.text = text;
        } else {
          this.currentText.fontStyle('normal');
        }
      }
      else if (type == 'is_variable') {
        this.selectedShape.is_variable = val;
        this.is_variable = val;
        if (!this.selectedShape.is_variable) {
          this.currentText.setAttr('variable_name', '');
        } else {
          this.is_patient_info = this.currentText.getAttr('is_patient_info');
          this.is_patient_restriction = this.currentText.getAttr('is_patient_restriction');
          this.is_group_patient_restriction = this.currentText.getAttr('is_group_patient_restriction');
        }

        this.currentText.setAttr('is_variable', val);
      }
      else if (type == 'is_icon') {
        this.selectedShape.is_icon = val;
        this.is_icon = val;
        this.currentText.setAttr('is_icon', val);
      }
      this.layer.draw();
      this.ref.detectChanges();
    }
  }

  imgSrc: string = '';
  postToDevice() {
    this.alertService.resetStickyMessage();
    var cleanTemplate = this.cleanUpTemplate(this.stage.toJSON());
    this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    this.editedTemplate.isPostToDevice = true;
    //this.editedTemplate.deviceAPIUrl = 'http://183.90.63.88:3000/template'
    //this.editedTemplate.deviceAPIUrl = "http://119.73.206.36:91/api/registration/GetRegistrationsByLocationCode";
    this.alertService.startLoadingMessage("Posting template to device...");
    this.pibService.mapTemplate(this.editedTemplate)
      .subscribe(res => {

        this.alertService.stopLoadingMessage();
        if (res.isSuccess) {
          this.alertService.showMessage("Success", "Successfully updated the device", MessageSeverity.success);
        }
        else {
          this.alertService.showStickyMessage("Post Error", "The below errors occured while posting your template to the device:" + "\n" + res.message, MessageSeverity.error);
        }
        //this.redrawFromJson(res.data);
        /*this.imgSrc = this.dup_stage.toDataURL();
        var curi = this.dup_stage.toCanvas({}).toDataURL();
        console.log(curi);
        var dataurl = this.dup_stage.toDataURL();
        var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
          bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
        while (n--) {
          u8arr[n] = bstr.charCodeAt(n);
        }
        var uri = new Blob([u8arr], { type: mime });


        console.log(dataurl.replace("image/png", "image/octet-stream"));
        var link = document.createElement('a');
        link.download = "image.png";
        link.href = dataurl;//.replace(/^data:image\/[^;]/, 'data:application/octet-stream');

        //link.download = 'template.png';
        //link.href = dataurl.replace("image/png", "image/octet-stream");
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        //delete link;
        var template = new PIBTemplate('', 'template.png', dataurl)
        var url = "http://183.90.63.88:3000/epaper";
        //this.pibService.postTemplateToDevice2(url, template).subscribe(res => {
        //  console.log(res);
        //  alert('Posted to device');
        //}, error => function (error) {
        //  console.log(error);
        //});

        //this.pibService.postTemplateToDevice(url, template).subscribe(res => {
        //  console.log(res);
        //  alert('Posted to device');
        //}, error => function (error) {
        //  console.log(error);
        //});
        */
      }
        , error => function (error) {
          this.alertService.showStickyMessage("Post Error", "The below errors occured while posting your template to the device:", MessageSeverity.error);
          console.log(error);
        });
    //this.pibService.mapTemplate(template);
  }

  postToDeviceAsImage() {
    this.alertService.resetStickyMessage();
    var cleanTemplate = this.cleanUpTemplate(this.stage.toJSON());
    this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    this.editedTemplate.isPostToDevice = false;
    this.alertService.startLoadingMessage("Posting template as image to device...");
    var component = this;
    this.pibService.mapTemplate(this.editedTemplate)
      .subscribe(res => {

        this.redrawFromJson(res.data, function () {
          var curi = component.dup_stage.toCanvas({}).toDataURL();
          //console.log(curi);
          var dataurl = component.dup_stage.toDataURL();
          //var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
          //  bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
          //while (n--) {
          //  u8arr[n] = bstr.charCodeAt(n);
          //}
          //var uri = new Blob([u8arr], { type: mime });

          var template = new PIBTemplate('', 'template.png', dataurl)
          var url = "http://183.90.63.88:3000/epaper";
          component.pibService.postTemplateToDevice(url, template).subscribe(res => {
            //console.log(res);
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

        //this.imgSrc = this.dup_stage.toDataURL();



        //this.redrawFromJson(res.data);
        /*this.imgSrc = this.dup_stage.toDataURL();
        var curi = this.dup_stage.toCanvas({}).toDataURL();
        console.log(curi);
        var dataurl = this.dup_stage.toDataURL();
        var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
          bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
        while (n--) {
          u8arr[n] = bstr.charCodeAt(n);
        }
        var uri = new Blob([u8arr], { type: mime });


        console.log(dataurl.replace("image/png", "image/octet-stream"));
        var link = document.createElement('a');
        link.download = "image.png";
        link.href = dataurl;//.replace(/^data:image\/[^;]/, 'data:application/octet-stream');

        //link.download = 'template.png';
        //link.href = dataurl.replace("image/png", "image/octet-stream");
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        //delete link;
        var template = new PIBTemplate('', 'template.png', dataurl)
        var url = "http://183.90.63.88:3000/epaper";
        //this.pibService.postTemplateToDevice2(url, template).subscribe(res => {
        //  console.log(res);
        //  alert('Posted to device');
        //}, error => function (error) {
        //  console.log(error);
        //});

        //this.pibService.postTemplateToDevice(url, template).subscribe(res => {
        //  console.log(res);
        //  alert('Posted to device');
        //}, error => function (error) {
        //  console.log(error);
        //});
        */
      }
        , error => function (error) {
          this.alertService.showStickyMessage("Post Error", "The below errors occured while posting your template to the device:", MessageSeverity.error);
          console.log(error);
        });
    //this.pibService.mapTemplate(template);
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
        //if (t['attrs']['icon'] != null && t['attrs']['icon'] != '') {
        //  var icons = (t['attrs']['icon']).split(',');
        //  for (var ic = 0; ic < icons.length; ic++) {
        //    var attrs = t['attrs'];
        //    var src = t['attrs']['src'];
        //    images.push({ src: src, attrs: attrs, zLayer: childAddedCount });
        //    childAddedCount++;
        //    var component = this;
        //    var imageObj = new Image()
        //    const image = this.shapeService.image(imageObj, attrs);
        //    imageObj.onload = function () {
        //      for (var i in images) {
        //        if (images[i].src == decodeURI(image.attrs.src)) {
        //          ++imgCount;
        //          var x = image.attrs.x === undefined ? 0 : parseInt(image.attrs.x);
        //          var y = image.attrs.y === undefined ? 0 : parseInt(image.attrs.y);

        //          image.setAttrs(images[i].attrs);
        //          image.x(x);
        //          image.y(y);
        //          image.setAttr('zLayer', images[i].zLayer);
        //          image.zIndex(parseInt(images[i].zLayer));

        //          component.dup_layer.draw();
        //        }
        //      }

        //      if (images != null && images.length == imgCount) {
        //        component.dup_layer.batchDraw();
        //        callback(component, component.dup_layer);
        //      }
        //    };
        //    imageObj.src = src;
        //    this.dup_layer.add(image);
        //  }
        //} else {
          const shape = new Konva.Text(t['attrs']);
          shape.setAttr('zLayer', childAddedCount);
          childAddedCount++;
          this.dup_layer.add(shape);
        //}
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
        //this.shapes.push(image);
        this.dup_layer.add(image);

        //Konva.Image.fromURL(src, function (node) {
        //  var is_background_image = false;
        //  for (var i in images) {
        //    if (images[i].src == decodeURI(node.attrs.image.src)) {
        //      ++imgCount;
        //      node.setAttrs(images[i].attrs);
        //      node.setAttr('zLayer', images[i].zLayer);
        //      node.zIndex(images[i].zLayer);
        //      is_background_image = node.attrs.is_background_image;
        //      component.dup_layer.add(node);
        //      component.dup_layer.draw();
        //      if (is_background_image) {
        //        node.zIndex(0);
        //      }
        //    }
        //  }

        //  if (images != null && images.length == imgCount) {
        //    component.reorderComponents(component, component.dup_layer, callback);
        //  }
        //});
      }

      this.dup_stage.add(this.dup_layer);
      this.dup_layer.draw();
    }
    //this.loadImages(images, this.buildImages);
    if (!images || images == null || images.length == 0) {
      this.reorderComponents(this, this.dup_layer, callback);
    }
  }

  loadImages(sources, callback) {
    var images: Konva.Image[] = [];
    var loadedImages = 0;
    var numImages = 0;
    for (var src in sources) {
      numImages++;
    }
    for (var src in sources) {
      images[src] = new Konva.Image(sources[src].attrs);
      images[src].onload = function () {
        if (++loadedImages >= numImages) {
          callback(images);
        }
      };
      images[src].src = sources[src].src;
    }
  }

  buildImages(images) {
    for (var i in images) {
      var img = new Konva.Image(images[i].attrs);
      this.dup_layer.add(img);
    }

    // this.dup_stage.add(this.dup_layer);
    this.dup_layer.batchDraw();
  }

  saveFile(s: string, filename: string) {
    var s = this.stage.toJSON(),
      filename = 'template.txt';

    const blob = new Blob([s], { type: 'text/plain' });
    saveAs(blob, filename);
  }

  public import = (files) => {
    if (files.length === 0) {
      return;
    }
    let fileToUpload = <File>files[0];
    var reader = new FileReader();
    var component = this;
    reader.onload = function (evt) {
      //if (evt.target.readyState != 2) return;
      //if (evt.target.error) {
      //  alert('Error while reading file');
      //  return;
      //}

      var filecontent = reader.result as string;
      var templateBody = JSON.parse(filecontent);
      var template = component.cleanUpTemplate(JSON.stringify(templateBody));
      component.redrawFromImport(template, component.reorderComponents);
      component.showNewTemplate = !component.showNewTemplate;
    };

    reader.readAsText(fileToUpload);

    //let fileToUpload = <File>files[0];
    //const formData = new FormData();
    //formData.append('file', fileToUpload, fileToUpload.name);
  }

  redrawFromImport(template?: any, callback?: any) {
    this.initTemplate();
    this.shapes = [];
    this.transformers = [];
    this.stage = new Konva.Stage({
      container: 'container',
      width: this.origWidth,
      height: this.origHeight,
      //drawBorder: template['attrs']['drawBorder']
    });

    this.layer = new Konva.Layer();
    this.stage.add(this.layer);
    this.addLineListeners();
    var images = [];
    if (template) {
      var templateChildren = [];
      templateChildren = template['children'][0]['children'];
      var sortedChildren = templateChildren.sort((a, b) => parseInt(this.safeGet(a['attrs'], 'zLayer', Number.MAX_VALUE)) - parseInt(this.safeGet(b['attrs'], 'zLayer', Number.MAX_VALUE)));
      var childAddedCount = 0;
      var imgCount = 0;
      var newZIndex = sortedChildren.length - 1;
      for (var i = 0; i < sortedChildren.length; i++) {
        var t = sortedChildren[i];
        if (t['className'] == 'Text') {
          const shape = new Konva.Text(t['attrs']);
          //shape.zIndex(childAddedCount);
          shape.setAttr('zLayer', childAddedCount);
          childAddedCount++;
          //shape.zIndex(newZIndex);
          shape.on('transform', function () {
            // reset scale, so only with is changing by transformer
            shape.setAttrs({
              width: shape.width() * shape.scaleX(),
              scaleX: 1
            });
          });
          //let tr = new Konva.Transformer({
          //  node: shape as any,
          //  enabledAnchors: ['middle-left', 'middle-right'],
          //  // set minimum width of text
          //  boundBoxFunc: function (oldBox, newBox) {
          //    newBox.width = Math.max(30, newBox.width);
          //    return newBox;
          //  }
          //});
          //this.transformers.push(tr);
          this.layer.add(shape);
        } else if (t['className'] == 'Line') {
          const shape = new Konva.Line(t['attrs']);
          //shape.zIndex(childAddedCount);
          shape.setAttr('zLayer', childAddedCount);
          childAddedCount++;
          //shape.setAttr('zLayer', newZIndex);
          //shape.zIndex(newZIndex);
          this.layer.add(shape);
        } else if (t['className'] == 'Rect') {
          const shape = new Konva.Rect(t['attrs']);
          //shape.zIndex(childAddedCount);
          shape.setAttr('zLayer', childAddedCount);
          childAddedCount++;
          //shape.setAttr('zLayer', newZIndex);
          //shape.zIndex(newZIndex);
          this.layer.add(shape);
        } else if (t['className'] == 'Image' && t['attrs']['src']) {
          var attrs = t['attrs'];
          var src = t['attrs']['src'];
          images.push({ src: src, attrs: attrs, zLayer: childAddedCount });
          childAddedCount++;
          var component = this;

          var imageObj = new Image()
          const image = this.shapeService.image(imageObj, attrs);
          imageObj.onload = function () {
            //image.image(imageObj);
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

                image.draggable(true);
                //is_background_image = node.attrs.is_background_image;
                //component.layer.add(node);
                //if (is_background_image) {
                //  node.zIndex(0);
                //}

                component.layer.draw();
              }
            }

            if (images != null && images.length == imgCount) {
              component.layer.batchDraw();
              callback(component, component.layer);
            }
          };
          imageObj.src = src;
          //this.shapes.push(image);
          this.layer.add(image);
          //Konva.Image.fromURL(src, function (node) {
          //  var is_background_image = false;
          //  for (var i in images) {
          //    if (images[i].src == decodeURI(node.attrs.image.src)) {
          //      ++imgCount;
          //      node.setAttrs(images[i].attrs);
          //      //node.zIndex(parseInt(images[i].zLayer));
          //      node.setAttr('zLayer', images[i].zLayer);
          //      node.zIndex(images[i].zLayer);
          //      //is_background_image = node.attrs.is_background_image;
          //      component.layer.add(node);
          //      //if (is_background_image) {
          //      //  node.zIndex(0);
          //      //}

          //     //component.layer.draw();
          //    }
          //  }

          //  if (images != null && images.length == imgCount) {
          //    component.layer.batchDraw();
          //    callback(component, component.layer);
          //  }

          //});
        }

        this.stage.add(this.layer);
        //this.layer.draw();
        this.addTransformerListeners();
      }

      if (!images || images == null || images.length == 0) {
        this.layer.batchDraw();
        callback(this, this.layer);
      }
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

  safeGet(obj, prop, defaultValue) {
    try {
      return obj[prop] != null && obj[prop] !== undefined ? obj[prop] : defaultValue;
    } catch (e) {
      return defaultValue
    }
  }

  preview(): void {
    var cleanTemplate = this.cleanUpTemplate(this.stage.toJSON());
    this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    this.editedTemplate.isPostToDevice = false;
    //this.editedTemplate.deviceAPIUrl = 'http://183.90.63.88:3000/template'
    //this.editedTemplate.deviceAPIUrl = "http://119.73.206.36:91/api/registration/GetRegistrationsByLocationCode";
    this.alertService.startLoadingMessage("Generating preview...");
    this.pibService.mapTemplate(this.editedTemplate)
      .subscribe(res => {
        this.alertService.stopLoadingMessage();
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
            width: component.origWidth / 2,
            height: component.origHeight / 2,
            scaleX: 1 / 2,
            scaleY: 1 / 2,
            //offsetWidth: .3,
            //offsetHeight: .3
          });

          component.preview_layer = component.dup_layer.clone({ hitGraphEnabled: false });
          component.preview_stage.add(component.preview_layer);
          component.preview_layer.batchDraw();
          component.is_preview = true;
        });



        this.imgSrc = this.dup_stage.toDataURL();
        //var curi = this.dup_stage.toCanvas({}).toDataURL();
        //console.log(curi);

        //const dialogRef = this.dialog.open(TemplateEditorPreviewDialog, {
        //  width: '100%',
        //  //minHeight: 'calc(100vh - 90px)',
        //  height: 'auto',
        //  //position: { left: '5vw' };
        //  data: { origHeight: this.origHeight, origWidth: this.origWidth, layer: this.layer, template: res.data, imgSrc: this.imgSrc}
        //});

        //dialogRef.afterClosed().subscribe(result => {
        //});
      }
        , error => function (error) {
          this.alertService.stopLoadingMessage();
          console.log(error);
        });
  }

  previewFromImage(isPostToDevice: boolean): void {
    var cleanTemplate = this.cleanUpTemplate(this.stage.toJSON());
    this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    this.editedTemplate.isPostToDevice = isPostToDevice;
    if (!isPostToDevice) {
      this.alertService.startLoadingMessage("Generating preview...");
    }
    else {
      this.alertService.startLoadingMessage("Posting template to device...");
    }
    this.pibService.previewFromImage(this.editedTemplate)
      .subscribe(res => {
        this.alertService.stopLoadingMessage();
        if (!res.isSuccess) {
          if (!isPostToDevice) {
            this.alertService.showStickyMessage("Post Error", "The below errors occured while posting the template to the device:" + "\n" + res.message, MessageSeverity.error);
          } else {
            this.alertService.showStickyMessage("Generation Error", "The below errors occured while generating the template:" + "\n" + res.message, MessageSeverity.error);
          }
          return;
        }
        if (!isPostToDevice) {
          this.is_preview = !isPostToDevice;
          //console.log(res.data);
          this.previewImgUri = this.domSanitizer.bypassSecurityTrustUrl('data:image/png;base64, ' + res.data);
        }
      }
        , error => function (error) {
          this.alertService.stopLoadingMessage();
          console.log(error);
        });
  }

  updatePreview() {
    //// we just need to update ALL nodes in the preview
    //this.layer.children.forEach(shape => {
    //  // find cloned node
    //  const clone = previewLayer.findOne('.' + shape.name());
    //  // update its position from the original
    //  clone.position(shape.position());
    //});
    //this.dup_layer.batchDraw();
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

  saveTemplate() {
    //var template = new PIBTemplate();
    var cleanTemplate = this.cleanUpTemplate(this.stage.toJSON());
    this.editedTemplate.templateBody = JSON.stringify(cleanTemplate);
    const dialogRef = this.dialog.open(TemplateSaveDialogPreviewDialog, {
      width: '500px',
      data: { template: this.editedTemplate}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result || !result.isCancel) {
        this.showNewTemplate = !this.showNewTemplate;
        //this.loadListData();
        this.editedTemplate = new PIBTemplate();
        this.ngOnInit();
      }
    });
  }


  //List
  initListTable() {
    let gT = (key: string) => this.translationService.getTranslation(key);

    this.columns = [
      { prop: "index", name: '#', width: 50, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'name', name: gT('templates.management.Name') },
      { prop: 'description', name: gT('templates.management.Description') },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false }
    ];

    this.loadListData();
  }

  loadListData() {
    this.alertService.startLoadingMessage();
    this.loadingIndicator = true;

    this.pibService.getTemplates()
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        let templates = results[0];

        templates.forEach((template, index, institutions) => {
          (<any>template).index = index + 1;
        });


        this.rowsCache = [...templates];
        this.rows = templates;

      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve templates from the server.\r\n"`,
            MessageSeverity.error);
        });
  }

  backToList() {
    this.showNewTemplate = !this.showNewTemplate;
    this.initListTable();
    this.editedTemplate = new PIBTemplate();
  }

  newTemplate() {
    this.initTemplate();
    //this.getLocations();
    this.getRestrictionTypes();
    this.editedTemplate = new PIBTemplate();
    this.showNewTemplate = !this.showNewTemplate;
  }

  editTemplate(row: PIBTemplate) {

    this.editedTemplate = row;
    //var template = JSON.parse(row.templateBody);
    var template = this.cleanUpTemplate(row.templateBody)
    this.redrawFromImport(template, this.reorderComponents);
    this.getRestrictionTypes();
    //this.getLocations();
    this.showNewTemplate = !this.showNewTemplate;
  }

  deleteTemplate(row: PIBTemplate) {
    this.alertService.showDialog('Are you sure you want to delete the \"' + row.name + '\" template?', DialogType.confirm, () => this.deleteDepartmentHelper(row));
  }


  deleteDepartmentHelper(row: PIBTemplate) {

    this.alertService.startLoadingMessage("Deleting...");
    this.loadingIndicator = true;

    this.pibService.deleteTemplate(row)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        this.rowsCache = this.rowsCache.filter(item => item !== row)
        this.rows = this.rows.filter(item => item !== row)
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage("Delete Error", `An error occured while deleting the template.`,
            MessageSeverity.error);
        });
  }

  onSearchChanged(value: string) {
    this.rows = this.rowsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
  }

  get canManageTemplates() {
    return this.accountService.userHasPermission(Permission.managePIBTemplatesPermission)
  }
}
