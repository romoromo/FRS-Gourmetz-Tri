import { Component, OnInit, Output, EventEmitter, Input, Inject, TemplateRef, ViewChild } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { Dish, DishDetail } from 'src/app/models/meal-order/dish.model';
import { DishService } from 'src/app/services/meal-order/dish.service';
import { StoreInfo } from 'src/app/models/meal-order/store-info.model';
import { Student } from 'src/app/models/meal-order/student.model';
import { Class } from 'src/app/models/meal-order/class.model';
import { InterestGroup } from 'src/app/models/meal-order/interest-group.model';
import { StudentGroup, StudentGroupDetail } from 'src/app/models/meal-order/student-group.model';
import { DeliveryService } from 'src/app/services/meal-order/delivery.service';
import { Filter, PagedResult } from 'src/app/models/sieve-filter.model';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { Outlet } from 'src/app/models/meal-order/outlet.model';
import { CatererInfo, CatererOutlet } from 'src/app/models/meal-order/caterer-info.model';
import { StudentService } from 'src/app/services/meal-order/student.service';
import { ClassService } from 'src/app/services/meal-order/class.service';

@Component({
  selector: 'student-selector',
  templateUrl: './student-selector.component.html',
  styleUrls: ['./student-selector.component.css']
})
export class StudentSelectorComponent implements OnInit {
  public studentIds: number[];
  students: Student[];
  studentsCache: Student[];
  selectedStudents: StudentGroupDetail[];
  filter: Filter;
  pagedResult: PagedResult;
  keyword: string = '';
  columns: any[] = [];
  rows: Student[] = [];
  rowsCache: Student[] = [];
  allRows: Student[] = [];
  allRowsCache: Student[] = [];
  outletId: string;
  editOutlet: Outlet;
  selectedCaterers: CatererOutlet[];
  public classes: Class[] = [];
  public interestGroups: InterestGroup[] = [];
  private selected: any[] = [];
  //private allRowsSelected = false;

  filterIsFas: boolean;
  filterClassId: string;
  filterIGId: string;

  @ViewChild('hdrTpl') hdrTpl: TemplateRef<any>;

  @ViewChild('actionsTemplate')
  actionsTemplate: TemplateRef<any>;

  @ViewChild('fasTemplate')
  fasTemplate: TemplateRef<any>;

  constructor(private http: HttpClient,
    private alertService: AlertService, public dialogRef: MatDialogRef<StudentSelectorComponent>, private studentService: StudentService, private classService: ClassService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    console.log("data inside: ", data);
    this.outletId = data.outletId;
    if (typeof (data.students) != typeof (undefined)) {
      this.selectedStudents = data.students;
      let selected = data.students;
      this.selected.splice(0, selected.length);
      //this.selected.push(...selected);
    }
  }

  initializeFilter() {
    this.filter = new Filter(1, 10);
    this.filter.sorts = 'name';
    this.filter.filters = '';
    this.filter.page = 0;


  }

  initializePagedResult() {
    this.pagedResult = new PagedResult();
    this.pagedResult.totalCount = 0;
    this.pagedResult.pagedData = [];
    this.pagedResult.filter = this.filter;
  }

  initializeTableDefinition() {

    this.columns = [
      { prop: 'name', name: 'Student Name' },
      { prop: 'classBatchName', name: 'Batch' },
      { prop: 'classLevelName', name: 'Class Level' },
      { prop: 'className', name: 'Class' },
      { name: 'Is FAS', cellTemplate: this.fasTemplate },
      { name: '', width: 150, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false, headerCheckboxable: true, headerTemplate: this.hdrTpl }
    ];
  }

  ngOnInit() {
    this.initializeFilter();
    this.initializePagedResult();
    this.initializeTableDefinition();
    this.loadData();
    this.getClasses();
    this.getInterestGroups();
    //this.loadAllData();
  }

  selectFn(ev) {
    console.log(ev);
  }

  onCheckboxChangeFn(ev) {
    console.log(ev);
  }

  onSelect({ selected }) {
    console.log(selected);
    //this.selected = selected;
    let length = selected.length;
    if (selected.length == 0) length = this.rows.length;

    this.selected.splice(0, length);
    this.selected.push(...selected);
    this.rows.forEach(row => (row.checked = this.selected.findIndex(e => e.id == row.id) > -1));
  }

  selectRowsOnInit(): void {
    let selected = this.rows.filter(e => e.checked);
    this.onSelect({ selected: selected });
    //this.rows.forEach(row => {
    //  // Set the isChecked property based on your criteria
    //  // For example, you can select all rows where isChecked is true
    //  if (row.checked) {
    //    this.onSelect({ selected: [row] });
    //  }
    //});
  }

  getClasses() {
    let filter = new Filter();
    let f = this.outletId ? '(classOutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';

    this.classService.getClassesByFilter(filter)
      .subscribe(results => {
        this.classes = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving classes.\r\n"`,
            MessageSeverity.error);
    });
  }

  getInterestGroups() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.studentService.getInterestGroupsByFilter(filter)
      .subscribe(results => {
        this.interestGroups = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving interest groups.\r\n"`,
            MessageSeverity.error);
     })
  }


  loadData(ev?: any) {
    this.filter.pageSize = -1;
    this.filter.page = 1;

    if (ev) {
      this.filter.page = ev.offset;
      if (ev.sorts) {
        this.filter.sorts = ev.sorts[0].dir == 'desc' ? '-' + ev.sorts[0].prop : ev.sorts[0].prop;
      }
    }

    if (!this.keyword) this.keyword = '';
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    let c = this.filterClassId ? '(ClassId)==' + this.filterClassId + ',' : '';
    let ig = this.filterIGId ? '(StudentInterestGroupId)==' + this.filterIGId + ',' : '';
    let fas = this.filterIsFas != null ? '(IsFAS)==' + this.filterIsFas + ',' : '';
    this.filter.filters = f + c + ig + fas + '(IsActive)==true,(Name)@=' + this.keyword;
    this.alertService.startLoadingMessage();
    this.studentService.getStudentsByFilter(this.filter)
      .subscribe(results => {
        this.pagedResult = results;

        console.log("result page: ", this.pagedResult)

        let students = results.pagedData;

        students.forEach((student, index, stores) => {
          (<any>student).index = index + 1;
        });

        console.log("selected students: ", this.selectedStudents)
        if (typeof (this.selectedStudents) != typeof (undefined)) {
          students.map(cg => {
            const index = this.selectedStudents.findIndex(item => item.studentId == cg.id)
            if (index >= 0) {
              cg.checked = true;
            }
          });
        }

        this.rowsCache = [...students];
        this.rows = students;
        this.selectRowsOnInit();
        this.alertService.stopLoadingMessage();
      },
        error => {
          this.alertService.stopLoadingMessage();

          this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
            MessageSeverity.error);
        });
  }

  loadAllData(ev?: any) {
    let allFilter = new Filter(-1, -1);
    allFilter.pageSize = -1;
    allFilter.page = -1;

    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    let c = this.filterClassId ? '(ClassId)==' + this.filterClassId + ',' : '';
    let ig = this.filterIGId ? '(StudentInterestGroupId)==' + this.filterIGId + ',' : '';
    let fas = this.filterIsFas != null ? '(IsFAS)==' + this.filterIsFas + ',' : '';
    allFilter.filters = f + c + ig + fas + '(IsActive)==true,(Name)@=' + this.keyword;

    this.studentService.getStudentsByFilter(allFilter)
      .subscribe(results => {
        //this.pagedResult = results;

        //console.log("result page: ", this.pagedResult)

        let students = results.pagedData;

        this.allRowsCache = [...students];
        this.allRows = students;

      },
        error => {
          this.alertService.stopLoadingMessage();

          //this.alertService.showStickyMessage("Load Error", `Unable to retrieve records from the server.\r\nErrors: "${Utilities.getHttpResponseMessage(error)}"`,
          //  MessageSeverity.error);
        });
  }

  onSearchChanged(value: string) {
    this.keyword = value;
    this.selected = [];
    this.loadData(null);
  }

  onClassChanged(value: any) {
    this.filterClassId = value.value;
    console.log("filterClassID: ", value)
    this.selected = [];
    this.loadData(null);
  }

  onGroupChanged(value: any) {
    this.filterIGId = value.value;
    console.log("filterInterestGroupID: ", value)
    this.selected = [];
    this.loadData(null);
  }

  onFASChange(value: any) {
    this.filterIsFas = value.value;
    console.log("filterFasID: ", value)
    this.selected = [];
    this.loadData(null);
  }

  clearFilter() {
    this.keyword = '';
    this.filterClassId = null;
    this.filterIGId = null;
    this.filterIsFas = null;
    this.selected = [];
    this.loadData(null);
  }

  toggleSelectAll() {
   // this.selected = this.allRowsSelected ? this.allRows : [];
  }

  public save = () => {
    //let studentIds= this.selected.map((e) => { return e.id; });

    //let students = this.rows.filter((cg) => (<any>cg).checked); 
    this.dialogRef.close({ isCancel: false, selectedStudents: this.selected });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }
}
